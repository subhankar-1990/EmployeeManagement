using EmployeeManagement.Application.Services.Employees.Queries;
using FluentValidation;

namespace EmployeeManagement.Application.Services.Employees.Validations;

/// <summary>
/// Validates <see cref="GetEmployeesQuery"/>.
/// </summary>
/// <remarks>
/// The page size ceiling keeps a single request from materialising the whole table, which is the
/// failure mode this endpoint is most exposed to.
/// </remarks>
public sealed class GetEmployeesQueryValidator : AbstractValidator<GetEmployeesQuery>
{
    /// <summary>Largest page a caller may request.</summary>
    public const int MaxPageSize = 100;

    /// <summary>Longest accepted search term.</summary>
    public const int MaxSearchLength = 100;

    public GetEmployeesQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("Page number must be at least 1.");

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, MaxPageSize)
            .WithMessage($"Page size must be between 1 and {MaxPageSize}.");

        RuleFor(query => query.Search)
            .MaximumLength(MaxSearchLength)
            .WithMessage($"Search must not exceed {MaxSearchLength} characters.")
            .Must(search => search is null || !search.Any(char.IsControl))
            .WithMessage("Search contains invalid characters.");
    }
}
