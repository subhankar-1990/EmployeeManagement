namespace EmployeeManagement.Api.Idempotency;

/// <summary>
/// Remembers the response produced for an idempotency key, so that a retry can be answered without
/// running the write again.
/// </summary>
/// <remarks>
/// A store is expected to be thread safe: several requests can carry the same key at the same time.
/// <see cref="TryReserveAsync"/> is the gate that serialises them, and it must be atomic - exactly
/// one caller may hold the reservation for a key that has no stored entry.
/// </remarks>
public interface IIdempotencyStore
{
    /// <summary>
    /// Returns the response captured for the key, or <see langword="null"/> when nothing is stored
    /// for it.
    /// </summary>
    /// <param name="key">The scoped store key built by the filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<IdempotencyEntry?> TryGetAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reserves the key for the caller.
    /// </summary>
    /// <param name="key">The scoped store key built by the filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// <see langword="true"/> when the key was free; <see langword="false"/> when another request
    /// holds it, which means an earlier attempt with the same key is still running.
    /// </returns>
    Task<bool> TryReserveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores a completed response so that later retries can be replayed from it, and releases the
    /// reservation taken by <see cref="TryReserveAsync"/>.
    /// </summary>
    /// <param name="key">The scoped store key built by the filter.</param>
    /// <param name="entry">The captured response.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SaveAsync(string key, IdempotencyEntry entry, CancellationToken cancellationToken = default);

    /// <summary>
    /// Releases a reservation without storing a response, so that the client is free to retry the
    /// same key.
    /// </summary>
    /// <param name="key">The scoped store key built by the filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ReleaseAsync(string key, CancellationToken cancellationToken = default);
}
