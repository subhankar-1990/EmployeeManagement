using EmployeeManagement.Application.Contracts.Employees;
using MediatR;

namespace EmployeeManagement.Application.Services.Employees.Queries;

/// <summary>
/// Returns a single employee by employee id.
/// </summary>
/// <param name="Id">The employee id to look up.</param>
/// <returns>The matching <see cref="EmployeeResponse"/>, or <see langword="null"/> when not found.</returns>
public sealed record GetEmployeeByIdQuery(int Id) : IRequest<EmployeeResponse?>;

