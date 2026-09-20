namespace EmployeeManagement.Infrastructure.Entity;

/// <summary>
/// Maps to the database table <c>tblEmployee</c>.
/// </summary>
public sealed class TblEmployee
{
    /// <summary>Database generated primary key (Identity).</summary>
    public int EmpId { get; set; }

    /// <summary>Employee name (NVARCHAR(100)).</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Mobile number (VARCHAR(15)).</summary>
    public string Mobile { get; set; } = string.Empty;

    /// <summary>Email address (VARCHAR(100)).</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Active status indicator (BIT).</summary>
    public bool IsActive { get; set; }
}
