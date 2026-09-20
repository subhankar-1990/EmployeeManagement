namespace EmployeeManagement.Domain.Models;

/// <summary>
/// The authentication material required to verify an employee's sign in attempt.
/// </summary>
public sealed class UserCredential
{
    /// <summary>Surrogate key.</summary>
    public Guid Id { get; init; }

    /// <summary>The employee number this credential belongs to.</summary>
    public long EmployeeNumber { get; init; }

    /// <summary>Encoded password hash.</summary>
    public string PasswordHash { get; init; } = string.Empty;

    /// <summary>Indicates that the account is locked.</summary>
    public bool IsLocked { get; init; }

    /// <summary>Roles granted to the account.</summary>
    public IReadOnlyCollection<string> Roles { get; init; } = [];
}

