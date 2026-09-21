using EmployeeManagement.Application.Services.Employees.Commands;
using FluentValidation;

namespace EmployeeManagement.Application.Services.Employees.Validations;

/// <summary>
/// Validates <see cref="UpdateEmployeeCommand"/>.
/// </summary>
public sealed class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    /// <summary>Maximum length of the <c>Name</c> field.</summary>
    public const int MaxNameLength = 100;

    /// <summary>Maximum length of the <c>Mobile</c> field.</summary>
    public const int MaxMobileLength = 20;

    /// <summary>Maximum length of the <c>Email</c> field.</summary>
    public const int MaxEmailLength = 150;

    public UpdateEmployeeCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0).WithMessage("Employee id must be greater than 0.");

        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(MaxNameLength).WithMessage($"Name must not exceed {MaxNameLength} characters.");

        RuleFor(command => command.Mobile)
            .NotEmpty().WithMessage("Mobile number is required.")
            .MaximumLength(MaxMobileLength).WithMessage($"Mobile must not exceed {MaxMobileLength} characters.");

        RuleFor(command => command.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .MaximumLength(MaxEmailLength).WithMessage($"Email must not exceed {MaxEmailLength} characters.")
            .EmailAddress().WithMessage("Email address is not valid.");
    }
}

