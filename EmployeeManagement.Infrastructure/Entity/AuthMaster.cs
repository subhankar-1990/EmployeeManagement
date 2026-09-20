using EmployeeManagement.Domain.Common;

namespace EmployeeManagement.Infrastructure.Entity;

/// <summary>
/// Maps to the <c>AuthMaster</c> table.
/// </summary>
public sealed class AuthMaster
{
    /// <summary>Surrogate key.</summary>
    public Guid AuthId { get; set; }

    /// <summary>The employee this credential belongs to.</summary>
    public long EmpNo { get; set; }

    /// <summary>Encoded password hash produced by <c>IPasswordHasher</c>.</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Indicates that an administrator locked the account.</summary>
    public bool IsLocked { get; set; }

    /// <summary>Comma separated role list, for example <c>Hr,Manager</c>.</summary>
    public string Role { get; set; } = Roles.Employee;
}

