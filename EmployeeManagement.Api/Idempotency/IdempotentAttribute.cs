using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Idempotency;

/// <summary>
/// Makes the decorated action idempotent.
/// </summary>
/// <remarks>
/// <para>
/// A request that carries the <see cref="IdempotencyHeaders.Key"/> header is answered from the
/// response stored the first time that key was used, so a retry after a lost or timed out response
/// cannot create a second record. A replayed response is marked with
/// <see cref="IdempotencyHeaders.Replayed"/> set to <c>true</c>, and requests without the header are
/// left untouched by this attribute.
/// </para>
/// <para>
/// The key is scoped to the endpoint it was sent to (HTTP method plus route). Only successful
/// responses are stored, so a request that failed validation can be retried with the same key.
/// Reusing a key with a different request body, or sending it while the first request is still
/// running, is rejected with <c>409 Conflict</c>.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class IdempotentAttribute : ServiceFilterAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IdempotentAttribute"/> class, backed by
    /// <see cref="IdempotencyFilter"/>. The filter is resolved from the request services, so it must
    /// be registered - <c>AddApiIdempotency</c> does that.
    /// </summary>
    public IdempotentAttribute()
        : base(typeof(IdempotencyFilter))
    {
    }
}
