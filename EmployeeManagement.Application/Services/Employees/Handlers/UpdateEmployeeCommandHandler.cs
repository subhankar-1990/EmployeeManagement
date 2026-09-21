using EmployeeManagement.Application.Abstractions.Persistence;
using EmployeeManagement.Application.Services.Employees.Commands;
using EmployeeManagement.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Application.Services.Employees.Handlers;

/// <summary>
/// Handles <see cref="UpdateEmployeeCommand"/>.
/// </summary>
public sealed class UpdateEmployeeCommandHandler(
    IEmployeeRepository employeeRepository,
    ILogger<UpdateEmployeeCommandHandler> logger) : IRequestHandler<UpdateEmployeeCommand, bool>
{
    /// <inheritdoc />
    public async Task<bool> Handle(
        UpdateEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        var employee = new Employee
        {
            EmpId = request.Id,
            Name = request.Name,
            Mobile = request.Mobile,
            Email = request.Email,
            IsActive = request.IsActive
        };

        var updated = await employeeRepository.UpdateAsync(employee, cancellationToken);

        if (updated)
        {
            logger.LogInformation("Updated employee with id {Id}.", request.Id);
        }
        else
        {
            logger.LogWarning("Update failed — employee with id {Id} was not found.", request.Id);
        }

        return updated;
    }
}

