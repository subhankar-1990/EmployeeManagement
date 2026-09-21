namespace EmployeeManagement.Application.Contracts.Employees;

/// <summary>
/// Payload required to update an existing employee.
/// </summary>
public sealed class UpdateEmployeeRequest
{
    /// <summary>Employee display name.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Contact mobile number.</summary>
    public string Mobile { get; init; } = string.Empty;

    /// <summary>Email address.</summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>Indicates whether the employee is active.</summary>
    public bool IsActive { get; init; }
}

