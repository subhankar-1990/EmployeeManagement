namespace EmployeeManagement.Application.Contracts.Employees;

/// <summary>
/// Employee representation returned to API consumers.
/// </summary>
public sealed class EmployeeResponse
{
    /// <summary>Employee identifier (<c>tblEmployee.EmpId</c>).</summary>
    public int Id { get; init; }

    /// <summary>Display name.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Contact mobile number.</summary>
    public string Mobile { get; init; } = string.Empty;

    /// <summary>Email address.</summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>Indicates whether the employee is active.</summary>
    public bool IsActive { get; init; }
}
