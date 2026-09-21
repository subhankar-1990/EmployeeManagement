using EmployeeManagement.Application.Abstractions.Persistence;
using EmployeeManagement.Application.Services.Employees.Commands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Application.Services.Employees.Handlers;

/// <summary>
/// Handles <see cref="DeleteEmployeeCommand"/>.
/// </summary>
public sealed class DeleteEmployeeCommandHandler(
    IEmployeeRepository employeeRepository,
    ILogger<DeleteEmployeeCommandHandler> logger) : IRequestHandler<DeleteEmployeeCommand, bool>
{
    /// <inheritdoc />
    public async Task<bool> Handle(
        DeleteEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        var deleted = await employeeRepository.DeleteAsync(request.Id, cancellationToken);

        if (deleted)
        {
            logger.LogInformation("Deleted employee with id {Id}.", request.Id);
        }
        else
        {
            logger.LogWarning("Delete failed — employee with id {Id} was not found.", request.Id);
        }

        return deleted;
    }
}

