namespace EmployeeManagement.Api.Configuration;

/// <summary>
/// Strongly-typed settings for the rate-limiting policies, bound from <c>appsettings.json</c>
/// under the <c>RateLimiting</c> key.
/// </summary>
public sealed class RateLimitingSettings
{
    /// <summary>Configuration section key.</summary>
    public const string SectionName = "RateLimiting";

    // -------------------------------------------------------------------------
    // Global concurrency limiter
    // -------------------------------------------------------------------------

    /// <summary>
    /// Maximum number of requests that may be processed concurrently across the entire API.
    /// Additional requests are queued up to <see cref="ConcurrencyQueueLimit"/>.
    /// </summary>
    public int ConcurrencyPermitLimit { get; init; } = 100;

    /// <summary>
    /// Maximum number of requests that may wait in the concurrency queue.
    /// Requests beyond this limit receive <c>503 Service Unavailable</c>.
    /// </summary>
    public int ConcurrencyQueueLimit { get; init; } = 50;

    // -------------------------------------------------------------------------
    // Write-endpoint fixed-window limiter (POST / PUT / DELETE)
    // -------------------------------------------------------------------------

    /// <summary>
    /// Maximum number of write requests (POST / PUT / DELETE) allowed per
    /// <see cref="WriteWindowSeconds"/> sliding window, per client IP.
    /// </summary>
    public int WritePermitLimit { get; init; } = 30;

    /// <summary>Length of the write-rate fixed window in seconds.</summary>
    public int WriteWindowSeconds { get; init; } = 60;

    /// <summary>
    /// Maximum number of write requests that may queue while the window is full.
    /// </summary>
    public int WriteQueueLimit { get; init; } = 5;

    // -------------------------------------------------------------------------
    // Read-endpoint fixed-window limiter (GET)
    // -------------------------------------------------------------------------

    /// <summary>
    /// Maximum number of read requests (GET) allowed per
    /// <see cref="ReadWindowSeconds"/> sliding window, per client IP.
    /// </summary>
    public int ReadPermitLimit { get; init; } = 100;

    /// <summary>Length of the read-rate fixed window in seconds.</summary>
    public int ReadWindowSeconds { get; init; } = 60;

    /// <summary>
    /// Maximum number of read requests that may queue while the window is full.
    /// </summary>
    public int ReadQueueLimit { get; init; } = 10;
}

