using System.Collections.Concurrent;
using EmployeeManagement.Api.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace EmployeeManagement.Api.Idempotency;

/// <summary>
/// In-process idempotency store: completed responses live in <see cref="IMemoryCache"/> for
/// <see cref="IdempotencySettings.EntryTtlSeconds"/> seconds, reservations for requests that are
/// still running live in a concurrent dictionary.
/// </summary>
/// <remarks>
/// <para>
/// The reservation is always released - in a <see langword="finally"/> block when the write fails,
/// or by <see cref="SaveAsync"/> when it succeeds - so a single failure can never block a key for
/// good.
/// </para>
/// <para>
/// Every entry keeps a copy of the response body for its lifetime, so this implementation is meant
/// for the small payloads these endpoints produce - the create employee response, not a bulk export.
/// </para>
/// <para>
/// State is per process and per cache lifetime. A deployment that runs several instances, or that
/// recycles often, needs a shared implementation behind <see cref="IIdempotencyStore"/> (Redis, or a
/// table with the key under a unique index) for the guarantee to hold across instances.
/// </para>
/// </remarks>
public sealed class InMemoryIdempotencyStore : IIdempotencyStore
{
    private readonly IMemoryCache _cache;
    private readonly ConcurrentDictionary<string, byte> _reservations = new(StringComparer.Ordinal);
    private readonly TimeSpan _timeToLive;

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryIdempotencyStore"/> class.
    /// </summary>
    /// <param name="cache">Cache that holds the captured responses.</param>
    /// <param name="settings">Idempotency policy providing the entry lifetime.</param>
    public InMemoryIdempotencyStore(IMemoryCache cache, IdempotencySettings settings)
    {
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentNullException.ThrowIfNull(settings);

        _cache = cache;
        _timeToLive = TimeSpan.FromSeconds(settings.EntryTtlSeconds);
    }

    /// <inheritdoc />
    public Task<IdempotencyEntry?> TryGetAsync(string key, CancellationToken cancellationToken = default) =>
        Task.FromResult(_cache.TryGetValue(key, out IdempotencyEntry? entry) ? entry : null);

    /// <inheritdoc />
    public Task<bool> TryReserveAsync(string key, CancellationToken cancellationToken = default) =>
        Task.FromResult(_reservations.TryAdd(key, 0));

    /// <inheritdoc />
    public Task SaveAsync(string key, IdempotencyEntry entry, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entry);

        _cache.Set(key, entry, _timeToLive);
        _reservations.TryRemove(key, out _);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task ReleaseAsync(string key, CancellationToken cancellationToken = default)
    {
        _reservations.TryRemove(key, out _);

        return Task.CompletedTask;
    }
}
