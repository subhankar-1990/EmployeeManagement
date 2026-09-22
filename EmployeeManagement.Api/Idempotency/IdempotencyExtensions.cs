using EmployeeManagement.Api.Configuration;

namespace EmployeeManagement.Api.Idempotency;

/// <summary>
/// Extension methods that register the idempotency contract for write endpoints.
/// </summary>
/// <remarks>
/// Idempotency is enforced by <see cref="IdempotentAttribute"/>, which the controller applies to an
/// action, so unlike the caching and rate-limiting extensions this one adds no middleware to the
/// pipeline.
/// </remarks>
public static class IdempotencyExtensions
{
    /// <summary>
    /// Registers the idempotency store, the filter behind <see cref="IdempotentAttribute"/>, and the
    /// policy read from the <c>Idempotency</c> configuration section.
    /// Call this from <c>Program.cs</c> before <c>builder.Build()</c>.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">Application configuration holding the <c>Idempotency</c> section.</param>
    /// <returns>The service collection so that further calls can be chained.</returns>
    public static IServiceCollection AddApiIdempotency(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var settings = configuration
            .GetSection(IdempotencySettings.SectionName)
            .Get<IdempotencySettings>() ?? new IdempotencySettings();

        // The store holds the captured responses for the whole process, so it is a singleton.
        // The filter keeps no state of its own and is created per request by the attribute.
        services.AddMemoryCache();
        services.AddSingleton(settings);
        services.AddSingleton<IIdempotencyStore, InMemoryIdempotencyStore>();
        services.AddScoped<IdempotencyFilter>();

        return services;
    }
}
