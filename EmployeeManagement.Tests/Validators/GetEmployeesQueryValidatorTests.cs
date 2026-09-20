using EmployeeManagement.Application.Services.Employees.Queries;
using EmployeeManagement.Application.Services.Employees.Validations;

namespace EmployeeManagement.Tests.Validators;

/// <summary>
/// Verifies the paging guard rails of the employee list query.
/// </summary>
public sealed class GetEmployeesQueryValidatorTests
{
    private readonly GetEmployeesQueryValidator _validator = new();

    [Fact]
    public void Validate_WithTheDefaultPaging_Succeeds()
    {
        var result = _validator.Validate(new GetEmployeesQuery());

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(GetEmployeesQueryValidator.MaxPageSize)]
    public void Validate_WithASupportedPageSize_Succeeds(int pageSize)
    {
        var result = _validator.Validate(new GetEmployeesQuery(PageSize: pageSize));

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_WithAPageNumberBelowOne_Fails(int pageNumber)
    {
        var result = _validator.Validate(new GetEmployeesQuery(PageNumber: pageNumber));

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            failure => failure.PropertyName == nameof(GetEmployeesQuery.PageNumber));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    [InlineData(GetEmployeesQueryValidator.MaxPageSize + 1)]
    public void Validate_WithAnUnsupportedPageSize_Fails(int pageSize)
    {
        var result = _validator.Validate(new GetEmployeesQuery(PageSize: pageSize));

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            failure => failure.PropertyName == nameof(GetEmployeesQuery.PageSize));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("ada")]
    public void Validate_WithAnAcceptableSearchTerm_Succeeds(string? search)
    {
        var result = _validator.Validate(new GetEmployeesQuery(Search: search));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithAnOverlongSearchTerm_Fails()
    {
        var search = new string('a', GetEmployeesQueryValidator.MaxSearchLength + 1);

        var result = _validator.Validate(new GetEmployeesQuery(Search: search));

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            failure => failure.PropertyName == nameof(GetEmployeesQuery.Search));
    }

    [Fact]
    public void Validate_WithControlCharactersInTheSearchTerm_Fails()
    {
        var result = _validator.Validate(new GetEmployeesQuery(Search: "ada\u0000lovelace"));

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            failure => failure.PropertyName == nameof(GetEmployeesQuery.Search));
    }
}
