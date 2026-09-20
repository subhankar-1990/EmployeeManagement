namespace EmployeeManagement.Domain.Models;

/// <summary>
/// An employee record as understood by the business layer.
/// </summary>
public sealed class Employee
{
    /// <summary>Employee identifier / primary key (<c>tblEmployee.EmpId</c>).</summary>
    public int EmpId { get; init; }

    /// <summary>Employee name.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Contact mobile number.</summary>
    public string Mobile { get; init; } = string.Empty;

    /// <summary>Email address.</summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>Indicates whether the employee is active.</summary>
    public bool IsActive { get; init; }
}
