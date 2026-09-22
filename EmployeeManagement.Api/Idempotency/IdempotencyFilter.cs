using System.Security.Cryptography;
using EmployeeManagement.Api.Configuration;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace EmployeeManagement.Api.Idempotency;

/// <summary>
/// Applies the idempotency contract described on <see cref="IdempotentAttribute"/>.
/// </summary>
/// <remarks>
/// <para>
/// The filter is a resource filter because that is the only point of the MVC pipeline that wraps the
/// whole request: it runs before model binding, so a retry can be answered without touching the
/// action, and it observes the response after the result has been written, so the body it captures is
/// the body the client received.
/// </para>
/// <para>
/// Only successful responses (<c>2xx</c>) are stored. A request that failed validation - or that
/// failed at all - leaves no entry behind, so the same key can be retried once the payload has been
/// corrected.
/// </para>
/// </remarks>
public sealed class IdempotencyFilter(
    IIdempotencyStore store,
    IdempotencySettings settings,
    ILogger<IdempotencyFilter> logger) : IAsyncResourceFilter
{
    private const string ReplayedHeaderValue = "true";

    /// <inheritdoc />
    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
        var request = context.HttpContext.Request;

        // Idempotency is opt-in: a request without the header keeps the behaviour it had before.
        if (!request.Headers.TryGetValue(IdempotencyHeaders.Key, out var suppliedKeys) || suppliedKeys.Count == 0)
        {
            await next();
            return;
        }

        if (!TryValidateKey(suppliedKeys, out var key, out var validationFailure))
        {
            logger.LogWarning(
                "Rejected {RequestMethod} {RequestPath}: {Reason}",
                request.Method,
                request.Path,
                validationFailure);

            context.Result = CreateProblemResult(
                context.HttpContext,
                StatusCodes.Status400BadRequest,
                "Invalid idempotency key.",
                validationFailure);

            return;
        }

        var storeKey = BuildStoreKey(context.HttpContext, key);
        var requestHash = await ComputeRequestHashAsync(request, context.HttpContext.RequestAborted);
        var stored = await store.TryGetAsync(storeKey, context.HttpContext.RequestAborted);

        if (stored is not null)
        {
            if (!string.Equals(stored.RequestHash, requestHash, StringComparison.Ordinal))
            {
                logger.LogWarning(
                    "Idempotency key {IdempotencyKey} was already used for a different payload on {RequestMethod} {RequestPath}.",
                    key,
                    request.Method,
                    request.Path);

                context.Result = CreateProblemResult(
                    context.HttpContext,
                    StatusCodes.Status409Conflict,
                    "Idempotency key already used.",
                    $"The key in the {IdempotencyHeaders.Key} header already answered a different payload. "
                    + "Send a new key for a new record, or repeat the earlier payload exactly.");

                return;
            }

            logger.LogInformation(
                "Replaying the stored response for idempotency key {IdempotencyKey} on {RequestMethod} {RequestPath}.",
                key,
                request.Method,
                request.Path);

            context.Result = new StoredResponseResult(stored);
            return;
        }

        if (!await store.TryReserveAsync(storeKey, context.HttpContext.RequestAborted))
        {
            logger.LogWarning(
                "Idempotency key {IdempotencyKey} is still being processed on {RequestMethod} {RequestPath}.",
                key,
                request.Method,
                request.Path);

            context.HttpContext.Response.Headers.RetryAfter = "1";

            context.Result = CreateProblemResult(
                context.HttpContext,
                StatusCodes.Status409Conflict,
                "Idempotency key already in use.",
                $"An earlier request with the same {IdempotencyHeaders.Key} header is still running. "
                + "Retry it once it has finished to receive the stored response.");

            return;
        }

        await ExecuteAndCaptureAsync(context, next, storeKey, requestHash);
    }

    // -------------------------------------------------------------------------
    // Execution and capture
    // -------------------------------------------------------------------------

    /// <summary>
    /// Runs the rest of the pipeline with the response redirected into a buffer, stores the response
    /// when the write succeeded, and forwards it to the client either way.
    /// </summary>
    private async Task ExecuteAndCaptureAsync(
        ResourceExecutingContext context,
        ResourceExecutionDelegate next,
        string storeKey,
        string requestHash)
    {
        var httpContext = context.HttpContext;

        using var buffer = new ResponseBuffer(httpContext);

        try
        {
            var executed = await next();
            await buffer.FlushAsync(httpContext.RequestAborted);

            if (executed.Exception is not null && !executed.ExceptionHandled)
            {
                // The action failed: MVC carries the exception to the exception handler instead of
                // throwing it through the filter, so nothing may be stored for this key.
                await store.ReleaseAsync(storeKey, CancellationToken.None);
                await buffer.CopyToClientAsync(httpContext.RequestAborted);
                return;
            }

            var statusCode = httpContext.Response.StatusCode;
            var body = buffer.Content.ToArray();

            if (statusCode is >= StatusCodes.Status200OK and < StatusCodes.Status300MultipleChoices)
            {
                var location = httpContext.Response.Headers.Location.ToString();

                await store.SaveAsync(
                    storeKey,
                    new IdempotencyEntry(
                        statusCode,
                        httpContext.Response.ContentType,
                        string.IsNullOrEmpty(location) ? null : location,
                        requestHash,
                        body),
                    CancellationToken.None);
            }
            else
            {
                // A failed write is not worth remembering: the client may retry the same key.
                await store.ReleaseAsync(storeKey, CancellationToken.None);
            }

            await buffer.CopyToClientAsync(httpContext.RequestAborted);
        }
        catch
        {
            // The write did not complete, so the key is handed back to the client.
            await store.ReleaseAsync(storeKey, CancellationToken.None);
            throw;
        }
    }

    /// <summary>
    /// Redirects everything the action writes into an in-memory buffer, and puts the real response
    /// body back when it is disposed.
    /// </summary>
    private sealed class ResponseBuffer : IDisposable
    {
        private readonly HttpContext _httpContext;
        private readonly IHttpResponseBodyFeature? _originalBodyFeature;
        private readonly Stream _originalBody;

        /// <summary>
        /// Redirects the response body of the given request into <see cref="Content"/>.
        /// </summary>
        public ResponseBuffer(HttpContext httpContext)
        {
            _httpContext = httpContext;
            _originalBodyFeature = httpContext.Features.Get<IHttpResponseBodyFeature>();
            _originalBody = httpContext.Response.Body;

            // The body feature is what Response.Body and Response.BodyWriter are served from, but the
            // stream is assigned as well: a host that keeps the stream on the response object cannot
            // bypass the buffer that way.
            httpContext.Response.Body = Content;
            httpContext.Features.Set<IHttpResponseBodyFeature>(
                new StreamResponseBodyFeature(Content, _originalBodyFeature));
        }

        /// <summary>Bytes written by the action and by the result it returned.</summary>
        public MemoryStream Content { get; } = new();

        /// <summary>
        /// Waits until everything the action wrote has reached <see cref="Content"/>. The JSON output
        /// formatter writes through a pipe that keeps a buffer of its own.
        /// </summary>
        public async Task FlushAsync(CancellationToken cancellationToken) =>
            await _httpContext.Response.BodyWriter.FlushAsync(cancellationToken);

        /// <summary>Sends the buffered response on to the real response body.</summary>
        public async Task CopyToClientAsync(CancellationToken cancellationToken)
        {
            Content.Position = 0;

            await Content.CopyToAsync(_originalBody, cancellationToken);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _httpContext.Response.Body = _originalBody;

            if (_originalBodyFeature is not null)
            {
                _httpContext.Features.Set(_originalBodyFeature);
            }

            Content.Dispose();
        }
    }

    /// <summary>
    /// Writes a captured response back to the client without running the action again.
    /// </summary>
    private sealed class StoredResponseResult(IdempotencyEntry entry) : IActionResult
    {
        /// <inheritdoc />
        public async Task ExecuteResultAsync(ActionContext context)
        {
            var response = context.HttpContext.Response;

            response.StatusCode = entry.StatusCode;
            response.ContentType = entry.ContentType;
            response.Headers[IdempotencyHeaders.Replayed] = ReplayedHeaderValue;

            if (!string.IsNullOrEmpty(entry.Location))
            {
                response.Headers.Location = entry.Location;
            }

            if (entry.Body.Length == 0)
            {
                return;
            }

            response.ContentLength = entry.Body.Length;

            await response.Body.WriteAsync(entry.Body, context.HttpContext.RequestAborted);
        }
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    /// <summary>
    /// Validates the supplied header value(s) and returns the single usable key.
    /// </summary>
    private bool TryValidateKey(StringValues suppliedKeys, out string key, out string failureReason)
    {
        key = string.Empty;
        failureReason = string.Empty;

        if (suppliedKeys.Count != 1)
        {
            failureReason = $"Send the {IdempotencyHeaders.Key} header exactly once.";
            return false;
        }

        var candidate = suppliedKeys[0];

        if (string.IsNullOrWhiteSpace(candidate))
        {
            failureReason = $"The {IdempotencyHeaders.Key} header must not be empty.";
            return false;
        }

        if (candidate.Length > settings.MaxKeyLength)
        {
            failureReason = $"The {IdempotencyHeaders.Key} header must not exceed {settings.MaxKeyLength} characters.";
            return false;
        }

        foreach (var character in candidate)
        {
            if (character is < '!' or > '~')
            {
                failureReason =
                    $"The {IdempotencyHeaders.Key} header may only contain printable ASCII characters and no spaces.";
                return false;
            }
        }

        key = candidate;

        return true;
    }

    /// <summary>
    /// Builds the key a response is stored under: the client key scoped to the endpoint it was sent
    /// to, so the same value used against another action cannot replay that action's response.
    /// </summary>
    private static string BuildStoreKey(HttpContext httpContext, string key)
    {
        var scope = httpContext.GetEndpoint() is RouteEndpoint routeEndpoint
            ? routeEndpoint.RoutePattern.RawText
            : httpContext.Request.Path.Value;

        return $"{httpContext.Request.Method} {scope}:{key}";
    }

    /// <summary>
    /// Hashes the request body, so a key that is reused with a different payload can be told apart
    /// from a genuine retry.
    /// </summary>
    /// <remarks>
    /// The body is read before model binding runs, so buffering is enabled and the stream is rewound
    /// afterwards: the action still binds from the start of the body.
    /// </remarks>
    private static async Task<string> ComputeRequestHashAsync(
        HttpRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.Body.CanSeek)
        {
            request.EnableBuffering();
        }

        request.Body.Position = 0;

        var hash = await SHA256.HashDataAsync(request.Body, cancellationToken);

        request.Body.Position = 0;

        return Convert.ToHexString(hash);
    }

    /// <summary>
    /// Builds an RFC 7807 response in the same shape the global exception handler produces, so clients
    /// see a single error contract across the API.
    /// </summary>
    private static ObjectResult CreateProblemResult(
        HttpContext httpContext,
        int statusCode,
        string title,
        string detail)
    {
        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = $"https://httpstatuses.io/{statusCode}",
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        problem.Extensions["traceId"] = httpContext.TraceIdentifier;

        return new ObjectResult(problem)
        {
            StatusCode = statusCode,
            ContentTypes = { "application/problem+json" }
        };
    }
}
