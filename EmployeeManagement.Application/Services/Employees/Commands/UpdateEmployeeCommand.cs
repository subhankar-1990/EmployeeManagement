using MediatR;

namespace EmployeeManagement.Application.Services.Employees.Commands;

/// <summary>
/// Updates the mutable fields of an existing employee.
/// </summary>
/// <param name="Id">Id of the employee to update.</param>
/// <param name="Name">New display name.</param>
/// <param name="Mobile">New mobile number.</param>
/// <param name="Email">New email address.</param>
/// <param name="IsActive">New active status.</param>
/// <returns><see langword="true"/> if the update succeeded; <see langword="false"/> when not found.</returns>
public sealed record UpdateEmployeeCommand(
    int Id,
    string Name,
    string Mobile,
    string Email,
    bool IsActive) : IRequest<bool>;

