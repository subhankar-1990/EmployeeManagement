using MediatR;

namespace EmployeeManagement.Application.Services.Employees.Commands;

/// <summary>
/// Deletes an employee by employee id.
/// </summary>
/// <param name="Id">Id of the employee to delete.</param>
/// <returns><see langword="true"/> if the delete succeeded; <see langword="false"/> when not found.</returns>
public sealed record DeleteEmployeeCommand(int Id) : IRequest<bool>;

