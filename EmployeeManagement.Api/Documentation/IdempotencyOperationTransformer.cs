using EmployeeManagement.Api.Configuration;
using EmployeeManagement.Api.Idempotency;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace EmployeeManagement.Api.Documentation;

/// <summary>
/// Declares the <c>Idempotency-Key</c> header on every action marked with
/// <see cref="IdempotentAttribute"/>.
/// </summary>
/// <remarks>
/// The header is read by <see cref="IdempotencyFilter"/> rather than bound to a parameter, so without
/// this transformer it would be invisible in the generated OpenAPI documents.
/// </remarks>
internal sealed class IdempotencyOperationTransformer(IdempotencySettings settings) : IOpenApiOperationTransformer
{
    /// <inheritdoc />
    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        var isIdempotent = context.Description.ActionDescriptor is ControllerActionDescriptor descriptor
            && descriptor.EndpointMetadata.OfType<IdempotentAttribute>().Any();

        if (!isIdempotent)
        {
            return Task.CompletedTask;
        }

        operation.Parameters ??= [];

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = IdempotencyHeaders.Key,
            In = ParameterLocation.Header,
            Required = false,
            Description =
                $"Optional client generated key, at most {settings.MaxKeyLength} characters. A retry that "
                + "carries the same key and the same body is answered from the response stored for the first "
                + $"request, marked with the {IdempotencyHeaders.Replayed} header, instead of creating a second "
                + "record. Reusing a key with a different body, or while the first request is still running, "
                + "is rejected with 409 Conflict.",
            Schema = new OpenApiSchema
            {
                Type = JsonSchemaType.String,
                MaxLength = settings.MaxKeyLength
            }
        });

        return Task.CompletedTask;
    }
}
