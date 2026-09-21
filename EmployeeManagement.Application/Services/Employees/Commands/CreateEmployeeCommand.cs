using EmployeeManagement.Application.Contracts.Employees;
using MediatR;

namespace EmployeeManagement.Application.Services.Employees.Commands;

/// <summary>
/// Creates a new employee record.
/// </summary>
/// <param name="Name">Employee display name.</param>
/// <param name="Mobile">Contact mobile number.</param>
/// <param name="Email">Email address.</param>
/// <param name="IsActive">Indicates whether the employee is active.</param>
/// <returns>The generated employee id.</returns>
public sealed record CreateEmployeeCommand(
    string Name,
    string Mobile,
    string Email,
    bool IsActive) : IRequest<EmployeeResponse>;

