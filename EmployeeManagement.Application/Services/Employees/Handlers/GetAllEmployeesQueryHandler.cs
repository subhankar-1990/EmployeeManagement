using AutoMapper;
using EmployeeManagement.Application.Abstractions.Persistence;
using EmployeeManagement.Application.Contracts.Employees;
using EmployeeManagement.Application.Services.Employees.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Application.Services.Employees.Handlers;

/// <summary>
/// Handles <see cref="GetAllEmployeesQuery"/>.
/// </summary>
public sealed class GetAllEmployeesQueryHandler(
    IEmployeeRepository employeeRepository,
    IMapper mapper,
    ILogger<GetAllEmployeesQueryHandler> logger) : IRequestHandler<GetAllEmployeesQuery, IReadOnlyList<EmployeeResponse>>
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<EmployeeResponse>> Handle(
        GetAllEmployeesQuery request,
        CancellationToken cancellationToken)
    {
        var employees = await employeeRepository.GetAllAsync(cancellationToken);

        logger.LogInformation("Returned {Count} employees.", employees.Count);

        return employees.Select(mapper.Map<EmployeeResponse>).ToList();
    }
}

