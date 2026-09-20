using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Application.Behaviors;

/// <summary>
/// Measures and logs the execution time of every request handled by MediatR.
/// </summary>
public sealed class RequestLoggingBehavior<TRequest, TResponse>(
    ILogger<RequestLoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next(cancellationToken);

            logger.LogInformation(
                "Handled {RequestName} in {ElapsedMilliseconds} ms.",
                requestName,
                stopwatch.ElapsedMilliseconds);

            return response;
        }
        catch (Exception)
        {
            logger.LogWarning(
                "Request {RequestName} failed after {ElapsedMilliseconds} ms.",
                requestName,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}

