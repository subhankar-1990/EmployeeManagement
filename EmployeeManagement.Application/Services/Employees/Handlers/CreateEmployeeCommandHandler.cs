using AutoMapper;
using EmployeeManagement.Application.Abstractions.Persistence;
using EmployeeManagement.Application.Contracts.Employees;
using EmployeeManagement.Application.Services.Employees.Commands;
using EmployeeManagement.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Application.Services.Employees.Handlers;

/// <summary>
/// Handles <see cref="CreateEmployeeCommand"/>.
/// </summary>
public sealed class CreateEmployeeCommandHandler(
    IEmployeeRepository employeeRepository,
    IMapper mapper,
    ILogger<CreateEmployeeCommandHandler> logger) : IRequestHandler<CreateEmployeeCommand, EmployeeResponse>
{
    /// <inheritdoc />
    public async Task<EmployeeResponse> Handle(
        CreateEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        var employee = new Employee
        {
            Name = request.Name,
            Mobile = request.Mobile,
            Email = request.Email,
            IsActive = request.IsActive
        };

        var id = await employeeRepository.AddAsync(employee, cancellationToken);

        logger.LogInformation("Created employee with id {Id}.", id);

        // Re-project the persisted data (id is now set) back to the response contract.
        var created = new Employee
        {
            EmpId = id,
            Name = request.Name,
            Mobile = request.Mobile,
            Email = request.Email,
            IsActive = request.IsActive
        };

        return mapper.Map<EmployeeResponse>(created);
    }
}

