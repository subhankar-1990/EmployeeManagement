using EmployeeManagement.Application.Contracts.Employees;
using MediatR;

namespace EmployeeManagement.Application.Services.Employees.Queries;

/// <summary>
/// Returns every employee ordered by employee id.
/// </summary>
/// <returns>All employees as a read-only list.</returns>
public sealed record GetAllEmployeesQuery : IRequest<IReadOnlyList<EmployeeResponse>>;

