using EmployeeManagement.Api.Configuration;
using EmployeeManagement.Api.Idempotency;
using Microsoft.Extensions.Caching.Memory;

namespace EmployeeManagement.Tests.Idempotency;

/// <summary>
/// Verifies the reservation, replay and expiry semantics of the in-process idempotency store.
/// </summary>
public sealed class InMemoryIdempotencyStoreTests
{
    private const string Key = "POST api/v{version:apiVersion}/employees:key-1";

    private static readonly IdempotencyEntry Entry = new(
        StatusCode: 201,
        ContentType: "application/json; charset=utf-8",
        Location: "/api/v1/employees/1",
        RequestHash: "3A5C",
        Body: [1, 2, 3]);

    [Fact]
    public async Task TryGetAsync_WithoutAStoredEntry_ReturnsNull()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var store = CreateStore(cache);

        Assert.Null(await store.TryGetAsync(Key));
    }

    [Fact]
    public async Task TryReserveAsync_WhileTheKeyIsReserved_Fails()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var store = CreateStore(cache);

        Assert.True(await store.TryReserveAsync(Key));

        // A second request with the same key must not be allowed to write at the same time.
        Assert.False(await store.TryReserveAsync(Key));
    }

    [Fact]
    public async Task SaveAsync_PublishesTheEntryAndFreesTheReservation()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var store = CreateStore(cache);

        Assert.True(await store.TryReserveAsync(Key));

        await store.SaveAsync(Key, Entry);

        Assert.Equal(Entry, await store.TryGetAsync(Key));

        // The write is finished, so the key is no longer held.
        Assert.True(await store.TryReserveAsync(Key));
    }

    [Fact]
    public async Task ReleaseAsync_AfterAReservation_LeavesNothingBehind()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var store = CreateStore(cache);

        Assert.True(await store.TryReserveAsync(Key));

        await store.ReleaseAsync(Key);

        Assert.Null(await store.TryGetAsync(Key));

        // A failed write hands the key back to the client.
        Assert.True(await store.TryReserveAsync(Key));
    }

    [Fact]
    public async Task TryGetAsync_AfterTheEntryLifetime_ReturnsNull()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var store = CreateStore(cache, entryTtlSeconds: 1);

        await store.SaveAsync(Key, Entry);

        Assert.Equal(Entry, await store.TryGetAsync(Key));

        await Task.Delay(TimeSpan.FromMilliseconds(1500));

        Assert.Null(await store.TryGetAsync(Key));
    }

    private static InMemoryIdempotencyStore CreateStore(IMemoryCache cache, int entryTtlSeconds = 60) =>
        new(cache, new IdempotencySettings { EntryTtlSeconds = entryTtlSeconds });
}
