namespace EmployeeManagement.Application.Contracts.Employees;

/// <summary>
/// Payload required to create a new employee.
/// </summary>
public sealed class CreateEmployeeRequest
{
    /// <summary>Employee display name.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Contact mobile number.</summary>
    public string Mobile { get; init; } = string.Empty;

    /// <summary>Email address.</summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>Indicates whether the employee is active. Defaults to <see langword="true"/>.</summary>
    public bool IsActive { get; init; } = true;
}

