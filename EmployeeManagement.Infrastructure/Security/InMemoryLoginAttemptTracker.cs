using EmployeeManagement.Application.Abstractions.Security;
using EmployeeManagement.Application.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace EmployeeManagement.Infrastructure.Security;

/// <summary>
/// In-process failed sign in counter backed by <see cref="IMemoryCache"/>.
/// </summary>
internal sealed class InMemoryLoginAttemptTracker : ILoginAttemptTracker
{
    private readonly IMemoryCache _cache;
    private readonly SecuritySettings _settings;

    public InMemoryLoginAttemptTracker(IMemoryCache cache, IOptions<SecuritySettings> securitySettings)
    {
        _cache = cache;
        _settings = securitySettings.Value;
    }

    /// <inheritdoc />
    public bool IsLockedOut(long employeeNumber) =>
        GetFailureCount(employeeNumber) >= _settings.MaxFailedSignInAttempts;

    /// <inheritdoc />
    public int RegisterFailure(long employeeNumber)
    {
        var failureCount = GetFailureCount(employeeNumber) + 1;

        _cache.Set(
            CacheKey(employeeNumber),
            failureCount,
            TimeSpan.FromMinutes(_settings.LockoutMinutes));

        return failureCount;
    }

    /// <inheritdoc />
    public void Reset(long employeeNumber) => _cache.Remove(CacheKey(employeeNumber));

    private int GetFailureCount(long employeeNumber) =>
        _cache.TryGetValue(CacheKey(employeeNumber), out int failureCount) ? failureCount : 0;

    private static string CacheKey(long employeeNumber) => $"sign-in-failures:{employeeNumber}";
}

