namespace EmployeeManagement.Api.Configuration;

/// <summary>
/// Strongly-typed settings for the output-cache policies, bound from <c>appsettings.json</c>
/// under the <c>Caching</c> key.
/// </summary>
public sealed class CachingSettings
{
    /// <summary>Configuration section key.</summary>
    public const string SectionName = "Caching";

    /// <summary>
    /// Seconds a collection response (paged list, full list) is served from the cache
    /// before the origin is re-queried. Defaults to 30 s.
    /// </summary>
    public int CollectionTtlSeconds { get; init; } = 30;

    /// <summary>
    /// Seconds a single-item response is served from the cache before the origin is
    /// re-queried. Defaults to 60 s.
    /// </summary>
    public int ItemTtlSeconds { get; init; } = 60;
}

