using EmployeeManagement.Api.Configuration;
using Microsoft.AspNetCore.OutputCaching;

namespace EmployeeManagement.Api.Middleware;

/// <summary>
/// Extension methods that register and configure ASP.NET Core's output cache for the employee read
/// endpoints.
/// </summary>
/// <remarks>
/// <para>
/// Two named policies are defined, and the controller opts into them with an
/// <see cref="OutputCacheAttribute"/>:
/// <list type="bullet">
///   <item>
///     <term><c>employees-collection</c></term>
///     <description>
///       Paged and unpaged employee lists. Lives for
///       <see cref="CachingSettings.CollectionTtlSeconds"/> seconds and varies only by page number,
///       page size and search term.
///     </description>
///   </item>
///   <item>
///     <term><c>employees-item</c></term>
///     <description>
///       A single employee looked up by id. Lives for
///       <see cref="CachingSettings.ItemTtlSeconds"/> seconds.
///     </description>
///   </item>
/// </list>
/// </para>
/// <para>
/// Every cached response is tagged with <see cref="EmployeesTag"/>, and the write endpoints evict
/// that tag, so a create, update or delete can never leave a stale list or item behind.
/// </para>
/// <para>
/// Both policies are built on ASP.NET Core's default output-cache policy, which only stores
/// anonymous GET/HEAD requests answered with <c>200 OK</c> and without a <c>Set-Cookie</c> header.
/// If bearer authentication is switched on later, requests that carry an <c>Authorization</c>
/// header are no longer cached, so the policies have to be extended with a user specific vary rule
/// (for example <c>SetVaryByValue</c>) for these endpoints to stay cacheable.
/// </para>
/// </remarks>
public static class CachingExtensions
{
    /// <summary>Name of the policy that caches paged and unpaged employee lists.</summary>
    public const string CollectionPolicy = "employees-collection";

    /// <summary>Name of the policy that caches a single employee.</summary>
    public const string ItemPolicy = "employees-item";

    /// <summary>
    /// Tag shared by every cached employee response. Evicting it with
    /// <see cref="IOutputCacheStore.EvictByTagAsync"/> clears the item and the collection entries in
    /// a single call.
    /// </summary>
    public const string EmployeesTag = "employees";

    /// <summary>
    /// Registers the output cache together with the employee policies. Durations are read from the
    /// <c>Caching</c> configuration section.
    /// Call this from <c>Program.cs</c> before <c>builder.Build()</c>.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">Application configuration holding the <c>Caching</c> section.</param>
    /// <returns>The service collection so that further calls can be chained.</returns>
    public static IServiceCollection AddApiOutputCaching(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var settings = configuration
            .GetSection(CachingSettings.SectionName)
            .Get<CachingSettings>() ?? new CachingSettings();

        services.AddOutputCache(options =>
        {
            // ---------------------------------------------------------------
            // Named policy: employees-collection  (GET /employees, GET /employees/all)
            // Only the paging and search arguments take part in the cache key, so an
            // unexpected query string cannot flood the cache with duplicate entries.
            // ---------------------------------------------------------------
            options.AddPolicy(CollectionPolicy, builder => builder
                .Expire(TimeSpan.FromSeconds(settings.CollectionTtlSeconds))
                .SetVaryByQuery("pageNumber", "pageSize", "search")
                .Tag(EmployeesTag));

            // ---------------------------------------------------------------
            // Named policy: employees-item  (GET /employees/{id})
            // The id already forms part of the path, which is the base of every key.
            // ---------------------------------------------------------------
            options.AddPolicy(ItemPolicy, builder => builder
                .Expire(TimeSpan.FromSeconds(settings.ItemTtlSeconds))
                .Tag(EmployeesTag));
        });

        return services;
    }

    /// <summary>
    /// Adds <c>UseOutputCache()</c> to the middleware pipeline.
    /// Call this from <c>Program.cs</c> after <c>app.Build()</c>, after authorization and rate
    /// limiting, and before <c>app.MapControllers()</c>.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder so that further calls can be chained.</returns>
    public static IApplicationBuilder UseApiOutputCaching(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        return app.UseOutputCache();
    }
}
