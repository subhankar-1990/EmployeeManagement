using EmployeeManagement.Application.Contracts.Common;
using EmployeeManagement.Application.Contracts.Employees;
using MediatR;

namespace EmployeeManagement.Application.Services.Employees.Queries;

/// <summary>
/// Returns one page of employees ordered by employee id.
/// </summary>
/// <param name="PageNumber">One based page number. Defaults to the first page.</param>
/// <param name="PageSize">Number of records per page. Defaults to ten.</param>
/// <param name="Search">
/// Optional case insensitive filter applied to the name, mobile number and email address.
/// </param>
/// <returns>The requested page together with the total number of matching employees.</returns>
public sealed record GetEmployeesQuery(int PageNumber = 1, int PageSize = 10, string? Search = null)
    : IRequest<PagedResponse<EmployeeResponse>>;
