using AutoMapper;
using EmployeeManagement.Application.Abstractions.Persistence;
using EmployeeManagement.Application.Contracts.Common;
using EmployeeManagement.Application.Contracts.Employees;
using EmployeeManagement.Application.Services.Employees.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Application.Services.Employees.Handlers;

/// <summary>
/// Handles <see cref="GetEmployeesQuery"/>.
/// </summary>
public sealed class GetEmployeesQueryHandler(
    IEmployeeRepository employeeRepository,
    IMapper mapper,
    ILogger<GetEmployeesQueryHandler> logger) : IRequestHandler<GetEmployeesQuery, PagedResponse<EmployeeResponse>>
{
    /// <inheritdoc />
    public async Task<PagedResponse<EmployeeResponse>> Handle(
        GetEmployeesQuery request,
        CancellationToken cancellationToken)
    {
        var page = await employeeRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.Search,
            cancellationToken);

        logger.LogInformation(
            "Returned {ItemCount} of {TotalCount} employees for page {PageNumber} of {TotalPages}.",
            page.Items.Count,
            page.TotalCount,
            page.PageNumber,
            page.TotalPages);

        return new PagedResponse<EmployeeResponse>
        {
            Items = page.Items.Select(mapper.Map<EmployeeResponse>).ToList(),
            PageNumber = page.PageNumber,
            PageSize = page.PageSize,
            TotalCount = page.TotalCount
        };
    }
}
