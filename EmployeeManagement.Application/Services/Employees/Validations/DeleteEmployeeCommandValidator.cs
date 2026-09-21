using EmployeeManagement.Application.Services.Employees.Commands;
using FluentValidation;

namespace EmployeeManagement.Application.Services.Employees.Validations;

/// <summary>
/// Validates <see cref="DeleteEmployeeCommand"/>.
/// </summary>
public sealed class DeleteEmployeeCommandValidator : AbstractValidator<DeleteEmployeeCommand>
{
    public DeleteEmployeeCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0).WithMessage("Employee id must be greater than 0.");
    }
}

