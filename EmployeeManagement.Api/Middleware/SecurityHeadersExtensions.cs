namespace EmployeeManagement.Api.Middleware;

/// <summary>
/// Adds defence in depth hardening headers to every response.
/// </summary>
/// <remarks>
/// A JSON API never renders HTML, so the strictest possible policy is applied. Paths that serve a
/// browser rendered user interface (the development only documentation UI) are the single exception
/// and receive <see cref="DocumentationContentSecurityPolicy"/> instead.
/// </remarks>
public static class SecurityHeadersExtensions
{
    /// <summary>
    /// Policy for JSON responses: no resource may be loaded and nothing may be executed.
    /// </summary>
    private const string ApiContentSecurityPolicy =
        "default-src 'none'; frame-ancestors 'none'; base-uri 'none'; form-action 'none'";

    /// <summary>
    /// Policy for paths that serve HTML (the development only documentation UI).
    /// </summary>
    private const string DocumentationContentSecurityPolicy =
        "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; " +
        "img-src 'self' data:; font-src 'self' data:; connect-src 'self'; " +
        "frame-ancestors 'none'; base-uri 'none'; form-action 'none'";

    /// <summary>
    /// Adds <c>X-Content-Type-Options</c>, <c>X-Frame-Options</c>, <c>Referrer-Policy</c>,
    /// <c>Permissions-Policy</c> and <c>Content-Security-Policy</c> headers to all responses.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="browserRenderedPathPrefixes">
    /// Path prefixes that serve a browser rendered user interface and therefore need the relaxed
    /// content security policy. Leave empty to apply the strict policy to every response.
    /// </param>
    public static IApplicationBuilder UseSecurityHeaders(
        this IApplicationBuilder app,
        params string[] browserRenderedPathPrefixes)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(browserRenderedPathPrefixes);

        return app.Use(async (context, next) =>
        {
            var headers = context.Response.Headers;

            headers["X-Content-Type-Options"] = "nosniff";
            headers["X-Frame-Options"] = "DENY";
            headers["Referrer-Policy"] = "no-referrer";
            headers["X-Permitted-Cross-Domain-Policies"] = "none";
            headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";
            headers["Content-Security-Policy"] = IsBrowserRendered(context.Request.Path, browserRenderedPathPrefixes)
                ? DocumentationContentSecurityPolicy
                : ApiContentSecurityPolicy;

            await next();
        });
    }

    private static bool IsBrowserRendered(PathString path, string[] prefixes)
    {
        foreach (var prefix in prefixes)
        {
            if (path.StartsWithSegments(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}

