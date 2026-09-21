using System.Threading.RateLimiting;
using EmployeeManagement.Api.Configuration;

namespace EmployeeManagement.Api.Middleware;

/// <summary>
/// Extension methods that register and configure ASP.NET Core's built-in rate limiter.
/// </summary>
/// <remarks>
/// Three layered policies are applied:
/// <list type="bullet">
///   <item>
///     <term>Global concurrency limiter</term>
///     <description>
///       Caps the total number of requests being processed simultaneously across the whole API.
///       Excess requests queue; requests beyond the queue limit receive <c>503</c>.
///     </description>
///   </item>
///   <item>
///     <term><c>write-limit</c> fixed-window policy</term>
///     <description>
///       Applied to every POST, PUT, and DELETE endpoint. Limits write operations per client IP
///       within a rolling time window. Excess requests receive <c>429 Too Many Requests</c>.
///     </description>
///   </item>
///   <item>
///     <term><c>read-limit</c> fixed-window policy</term>
///     <description>
///       Applied to every GET endpoint. A more generous limit than writes.
///       Excess requests receive <c>429 Too Many Requests</c>.
///     </description>
///   </item>
/// </list>
/// </remarks>
public static class RateLimitingExtensions
{
    /// <summary>Name of the write-operations rate-limit policy.</summary>
    public const string WritePolicy = "write-limit";

    /// <summary>Name of the read-operations rate-limit policy.</summary>
    public const string ReadPolicy = "read-limit";

    /// <summary>
    /// Registers the rate-limiting services and configures all named policies.
    /// Call this from <c>Program.cs</c> before <c>builder.Build()</c>.
    /// </summary>
    public static IServiceCollection AddApiRateLimiting(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = configuration
            .GetSection(RateLimitingSettings.SectionName)
            .Get<RateLimitingSettings>() ?? new RateLimitingSettings();

        services.AddRateLimiter(limiterOptions =>
        {
            // ---------------------------------------------------------------
            // 429 / 503 response shape
            // ---------------------------------------------------------------
            limiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            limiterOptions.OnRejected = async (context, cancellationToken) =>
            {
                var response = context.HttpContext.Response;

                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    response.Headers.RetryAfter =
                        ((int)retryAfter.TotalSeconds).ToString(System.Globalization.CultureInfo.InvariantCulture);
                }

                response.StatusCode = StatusCodes.Status429TooManyRequests;
                response.ContentType = "application/problem+json";

                var problem = new
                {
                    type = "https://httpstatuses.io/429",
                    title = "Too Many Requests",
                    status = 429,
                    detail = "You have exceeded the allowed request rate. Please slow down and retry.",
                    instance = context.HttpContext.Request.Path.Value
                };

                await response.WriteAsJsonAsync(problem, cancellationToken);
            };

            // ---------------------------------------------------------------
            // Global concurrency limiter
            // Cap the number of requests processed at the same time.
            // ---------------------------------------------------------------
            limiterOptions.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                httpContext => RateLimitPartition.GetConcurrencyLimiter(
                    partitionKey: "global",
                    factory: _ => new ConcurrencyLimiterOptions
                    {
                        PermitLimit = settings.ConcurrencyPermitLimit,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = settings.ConcurrencyQueueLimit
                    }));

            // ---------------------------------------------------------------
            // Named policy: write-limit  (POST / PUT / DELETE)
            // Partitioned per client IP so one client cannot starve others.
            // ---------------------------------------------------------------
            limiterOptions.AddPolicy(WritePolicy, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: ResolveClientIp(httpContext),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = settings.WritePermitLimit,
                        Window = TimeSpan.FromSeconds(settings.WriteWindowSeconds),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = settings.WriteQueueLimit
                    }));

            // ---------------------------------------------------------------
            // Named policy: read-limit  (GET)
            // More generous than the write policy.
            // ---------------------------------------------------------------
            limiterOptions.AddPolicy(ReadPolicy, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: ResolveClientIp(httpContext),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = settings.ReadPermitLimit,
                        Window = TimeSpan.FromSeconds(settings.ReadWindowSeconds),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = settings.ReadQueueLimit
                    }));
        });

        return services;
    }

    /// <summary>
    /// Adds <c>UseRateLimiter()</c> to the middleware pipeline.
    /// Call this from <c>Program.cs</c> after <c>app.Build()</c>, before <c>app.MapControllers()</c>.
    /// </summary>
    public static IApplicationBuilder UseApiRateLimiting(this IApplicationBuilder app)
        => app.UseRateLimiter();

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    /// <summary>
    /// Resolves the real client IP, honouring the <c>X-Forwarded-For</c> header set by a
    /// trusted reverse proxy. Falls back to the direct connection address.
    /// </summary>
    private static string ResolveClientIp(HttpContext httpContext)
    {
        // X-Forwarded-For is already unwrapped by UseForwardedHeaders() in Program.cs,
        // so httpContext.Connection.RemoteIpAddress already reflects the real client.
        return httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}

