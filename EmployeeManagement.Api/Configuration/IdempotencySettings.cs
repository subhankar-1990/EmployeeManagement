namespace EmployeeManagement.Api.Configuration;

/// <summary>
/// Strongly-typed settings for idempotent write endpoints, bound from <c>appsettings.json</c> under
/// the <c>Idempotency</c> key.
/// </summary>
public sealed class IdempotencySettings
{
    /// <summary>Configuration section key.</summary>
    public const string SectionName = "Idempotency";

    /// <summary>
    /// Seconds a completed response stays replayable. A retry that arrives after this window is
    /// treated as a new request. Defaults to 24 hours.
    /// </summary>
    public int EntryTtlSeconds { get; init; } = 86_400;

    /// <summary>
    /// Maximum accepted length of an idempotency key. A longer key is rejected with
    /// <c>400 Bad Request</c> before the action runs. Defaults to 255 characters.
    /// </summary>
    public int MaxKeyLength { get; init; } = 255;
}
