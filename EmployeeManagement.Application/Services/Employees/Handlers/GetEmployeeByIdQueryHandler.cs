using AutoMapper;
using EmployeeManagement.Application.Abstractions.Persistence;
using EmployeeManagement.Application.Contracts.Employees;
using EmployeeManagement.Application.Services.Employees.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Application.Services.Employees.Handlers;

/// <summary>
/// Handles <see cref="GetEmployeeByIdQuery"/>.
/// </summary>
public sealed class GetEmployeeByIdQueryHandler(
    IEmployeeRepository employeeRepository,
    IMapper mapper,
    ILogger<GetEmployeeByIdQueryHandler> logger) : IRequestHandler<GetEmployeeByIdQuery, EmployeeResponse?>
{
    /// <inheritdoc />
    public async Task<EmployeeResponse?> Handle(
        GetEmployeeByIdQuery request,
        CancellationToken cancellationToken)
    {
        var employee = await employeeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (employee is null)
        {
            logger.LogWarning("Employee with id {Id} was not found.", request.Id);
            return null;
        }

        logger.LogInformation("Returned employee with id {Id}.", request.Id);

        return mapper.Map<EmployeeResponse>(employee);
    }
}

