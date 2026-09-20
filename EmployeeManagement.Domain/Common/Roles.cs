namespace EmployeeManagement.Domain.Common;

/// <summary>
/// Well known role names used for authorization decisions.
/// </summary>
public static class Roles
{
    /// <summary>Full administrative access.</summary>
    public const string Admin = "Admin";

    /// <summary>Human resources - may manage employee records.</summary>
    public const string Hr = "Hr";

    /// <summary>Line manager - read only access to employee records.</summary>
    public const string Manager = "Manager";

    /// <summary>Default role for a standard employee.</summary>
    public const string Employee = "Employee";

    /// <summary>All roles known to the system.</summary>
    public static readonly IReadOnlyList<string> All = [Admin, Hr, Manager, Employee];

    /// <summary>
    /// Determines whether <paramref name="role"/> is a known role name (case insensitive).
    /// </summary>
    public static bool IsKnown(string? role) =>
        !string.IsNullOrWhiteSpace(role) && All.Contains(role.Trim(), StringComparer.OrdinalIgnoreCase);
}

