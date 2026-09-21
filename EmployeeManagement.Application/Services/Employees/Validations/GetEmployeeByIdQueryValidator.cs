using EmployeeManagement.Application.Services.Employees.Queries;
using FluentValidation;

namespace EmployeeManagement.Application.Services.Employees.Validations;

/// <summary>
/// Validates <see cref="GetEmployeeByIdQuery"/>.
/// </summary>
public sealed class GetEmployeeByIdQueryValidator : AbstractValidator<GetEmployeeByIdQuery>
{
    public GetEmployeeByIdQueryValidator()
    {
        RuleFor(query => query.Id)
            .GreaterThan(0).WithMessage("Employee id must be greater than 0.");
    }
}

