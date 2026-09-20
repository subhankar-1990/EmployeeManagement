using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Middleware;

/// <summary>
/// Translates unhandled exceptions into RFC 7807 problem details responses.
/// </summary>
/// <remarks>
/// Internal exception messages are only surfaced when the host environment is Development.
/// Production responses expose a correlation id (<c>traceId</c>) instead, which is also written
/// to the log so an operator can join the two.
/// </remarks>
public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;
    private readonly IHostEnvironment _environment = environment;

    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // A cancelled request means the client disconnected; it is not a server fault.
        if (exception is OperationCanceledException && httpContext.RequestAborted.IsCancellationRequested)
        {
            _logger.LogDebug(
                "Request {RequestMethod} {RequestPath} was cancelled by the client.",
                httpContext.Request.Method,
                httpContext.Request.Path);

            return false;
        }

        var (statusCode, title) = MapException(exception);

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                "Unhandled exception for {RequestMethod} {RequestPath}. TraceId: {TraceId}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                httpContext.TraceIdentifier);
        }
        else
        {
            _logger.LogWarning(
                exception,
                "Handled {ExceptionType} for {RequestMethod} {RequestPath}. TraceId: {TraceId}",
                exception.GetType().Name,
                httpContext.Request.Method,
                httpContext.Request.Path,
                httpContext.TraceIdentifier);
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = $"https://httpstatuses.io/{statusCode}",
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        if (exception is ValidationException validationException)
        {
            // Field level errors let clients highlight the offending properties.
            problemDetails.Extensions["errors"] = validationException.Errors
                .GroupBy(failure => failure.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(failure => failure.ErrorMessage).Distinct().ToArray());
        }

        if (_environment.IsDevelopment())
        {
            problemDetails.Detail = exception.Message;
        }

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            options: null,
            contentType: "application/problem+json",
            cancellationToken: cancellationToken);

        return true;
    }

    private static (int StatusCode, string Title) MapException(Exception exception) => exception switch
    {
        ValidationException => (StatusCodes.Status400BadRequest, "One or more validation errors occurred."),
        KeyNotFoundException => (StatusCodes.Status404NotFound, "The requested resource was not found."),
        UnauthorizedAccessException => (StatusCodes.Status403Forbidden, "You are not permitted to perform this operation."),
        ArgumentException => (StatusCodes.Status400BadRequest, "The request was invalid."),
        _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
    };
}

