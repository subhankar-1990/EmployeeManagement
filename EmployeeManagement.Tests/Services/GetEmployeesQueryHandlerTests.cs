using AutoMapper;
using EmployeeManagement.Application;
using EmployeeManagement.Application.Services.Employees.Handlers;
using EmployeeManagement.Application.Services.Employees.Queries;
using EmployeeManagement.Domain.Models;
using EmployeeManagement.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace EmployeeManagement.Tests.Services;

/// <summary>
/// Verifies the employee list query: paging metadata, filtering and projection.
/// </summary>
public sealed class GetEmployeesQueryHandlerTests
{
    private static readonly int[] ExpectedFirstPageIds = [1, 2];

    private static readonly IMapper Mapper = BuildServiceProvider().GetRequiredService<IMapper>();

    private static readonly Employee[] Employees =
    [
        new() { EmpId = 1, Name = "Ada Lovelace", Mobile = "9876543210", Email = "ada@example.com", IsActive = true },
        new() { EmpId = 2, Name = "Grace Hopper", Mobile = "9876543211", Email = "grace@example.com", IsActive = true },
        new() { EmpId = 3, Name = "Alan Turing", Mobile = "9876543212", Email = "alan@example.com", IsActive = false }
    ];

    [Fact]
    public async Task Handle_WithTheFirstPage_ReturnsTheRequestedSliceAndTotalCount()
    {
        var handler = CreateHandler(Employees);

        var page = await handler.Handle(new GetEmployeesQuery(PageNumber: 1, PageSize: 2), CancellationToken.None);

        Assert.Equal(2, page.Items.Count);
        Assert.Equal(3, page.TotalCount);
        Assert.Equal(2, page.TotalPages);
        Assert.Equal(1, page.PageNumber);
        Assert.Equal(2, page.PageSize);
        Assert.False(page.HasPreviousPage);
        Assert.True(page.HasNextPage);
        Assert.Equal(ExpectedFirstPageIds, page.Items.Select(item => item.Id));
    }

    [Fact]
    public async Task Handle_WithTheLastPage_ReturnsTheRemainderAndReportsNoNextPage()
    {
        var handler = CreateHandler(Employees);

        var page = await handler.Handle(new GetEmployeesQuery(PageNumber: 2, PageSize: 2), CancellationToken.None);

        Assert.Single(page.Items);
        Assert.Equal(3, page.TotalCount);
        Assert.True(page.HasPreviousPage);
        Assert.False(page.HasNextPage);
        Assert.Equal(3, page.Items[0].Id);
    }

    [Fact]
    public async Task Handle_WithAPageBeyondTheResultSet_ReturnsAnEmptyPageThatStillReportsTheTotalCount()
    {
        var handler = CreateHandler(Employees);

        var page = await handler.Handle(new GetEmployeesQuery(PageNumber: 5, PageSize: 2), CancellationToken.None);

        Assert.Empty(page.Items);
        Assert.Equal(3, page.TotalCount);
        Assert.Equal(2, page.TotalPages);
        Assert.False(page.HasNextPage);
    }

    [Fact]
    public async Task Handle_WithASearchTerm_ReturnsOnlyMatchingEmployees()
    {
        var handler = CreateHandler(Employees);

        var page = await handler.Handle(
            new GetEmployeesQuery(PageNumber: 1, PageSize: 10, Search: "grace@example.com"),
            CancellationToken.None);

        Assert.Single(page.Items);
        Assert.Equal(2, page.Items[0].Id);
        Assert.Equal(1, page.TotalCount);
    }

    [Fact]
    public async Task Handle_WithASearchTermThatMatchesNothing_ReturnsAnEmptyPage()
    {
        var handler = CreateHandler(Employees);

        var page = await handler.Handle(
            new GetEmployeesQuery(PageNumber: 1, PageSize: 10, Search: "no-such-employee"),
            CancellationToken.None);

        Assert.Empty(page.Items);
        Assert.Equal(0, page.TotalCount);
        Assert.Equal(0, page.TotalPages);
    }

    [Fact]
    public async Task Handle_MapsEveryFieldOntoTheResponse()
    {
        var handler = CreateHandler(Employees);

        var page = await handler.Handle(
            new GetEmployeesQuery(PageNumber: 1, PageSize: 1, Search: "Ada"),
            CancellationToken.None);

        var employee = Assert.Single(page.Items);

        Assert.Equal(1, employee.Id);
        Assert.Equal("Ada Lovelace", employee.Name);
        Assert.Equal("9876543210", employee.Mobile);
        Assert.Equal("ada@example.com", employee.Email);
        Assert.True(employee.IsActive);
    }

    private static GetEmployeesQueryHandler CreateHandler(IReadOnlyList<Employee> employees) =>
        new(FakeEmployeeRepository.Containing([.. employees]), Mapper, NullLogger<GetEmployeesQueryHandler>.Instance);

    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));
        services.AddApplicationServices();

        return services.BuildServiceProvider();
    }
}
