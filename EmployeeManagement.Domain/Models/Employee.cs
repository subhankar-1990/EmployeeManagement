namespace EmployeeManagement.Domain.Models;

/// <summary>
/// An employee record as understood by the business layer.
/// </summary>
public sealed class Employee
{
    /// <summary>Surrogate key.</summary>
    public Guid Id { get; init; }

    /// <summary>Human facing employee number.</summary>
    public long EmployeeNumber { get; init; }

    /// <summary>Display name.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Optional 10 digit mobile number.</summary>
    public string? MobileNumber { get; init; }

    /// <summary>Optional email address.</summary>
    public string? Email { get; init; }

    /// <summary>Creation timestamp.</summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>Indicates whether the employee is active.</summary>
    public bool IsActive { get; init; } = true;
}

