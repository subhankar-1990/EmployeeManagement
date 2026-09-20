namespace EmployeeManagement.Infrastructure.Entity;

/// <summary>
/// Maps to the <c>EmployeeMaster</c> table.
/// </summary>
public sealed class EmployeeMaster
{
    /// <summary>Surrogate key.</summary>
    public Guid EmpId { get; set; }

    /// <summary>Database generated employee number.</summary>
    public long EmpNo { get; set; }

    /// <summary>Display name (maximum 100 characters).</summary>
    public string EmpName { get; set; } = string.Empty;

    /// <summary>Optional mobile number (maximum 20 characters).</summary>
    public string? Mobile { get; set; }

    /// <summary>Optional email address (maximum 254 characters).</summary>
    public string? Email { get; set; }

    /// <summary>Creation timestamp.</summary>
    public DateTime CreateDate { get; set; }

    /// <summary>Indicates whether the employee may sign in.</summary>
    public bool IsActive { get; set; }
}

