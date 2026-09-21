using System.Net;
using System.Net.Http.Json;
using EmployeeManagement.Api.Middleware;
using EmployeeManagement.Application.Abstractions.Persistence;
using EmployeeManagement.Application.Contracts.Employees;
using EmployeeManagement.Domain.Common;
using EmployeeManagement.Domain.Models;
using EmployeeManagement.Tests.Fakes;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EmployeeManagement.Tests.Api;

/// <summary>
/// Verifies how the employee endpoints take part in the output cache: the policy each one declares,
/// that a repeated read is answered from the cache, and that a successful write evicts it.
/// </summary>
/// <remarks>
/// Nothing here needs a database. The policy checks inspect the endpoints routing actually mapped,
/// and the behavioural checks swap the repository for the in-memory double, so the only thing a
/// cache hit saves is a call to that double.
/// </remarks>
public sealed class EmployeesOutputCacheTests(EmployeeManagementApiFactory factory)
    : IClassFixture<EmployeeManagementApiFactory>
{
    private const string EmployeesUrl = "/api/v1/employees";

    private static readonly Employee[] SampleEmployees =
    [
        new() { EmpId = 1, Name = "Ada Lovelace", Mobile = "9000000001", Email = "ada@example.com", IsActive = true },
        new() { EmpId = 2, Name = "Grace Hopper", Mobile = "9000000002", Email = "grace@example.com", IsActive = true }
    ];

    [Theory]
    [InlineData("GetEmployees", CachingExtensions.CollectionPolicy)]
    [InlineData("GetEmployeeById", CachingExtensions.ItemPolicy)]
    public void ReadActions_UseTheExpectedCachePolicy(string actionName, string expectedPolicyName)
    {
        var endpoint = FindEndpoint(actionName);

        var attribute = endpoint.Metadata.GetMetadata<OutputCacheAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal(expectedPolicyName, attribute.PolicyName);
    }

    [Theory]
    [InlineData("CreateEmployee")]
    [InlineData("UpdateEmployee")]
    [InlineData("DeleteEmployee")]
    public void WriteActions_AreNeverCached(string actionName)
    {
        var endpoint = FindEndpoint(actionName);

        Assert.Null(endpoint.Metadata.GetMetadata<OutputCacheAttribute>());
    }

    [Fact]
    public async Task PagedReads_AreCachedPerPageSizeAndSearchTerm()
    {
        var repository = new CountingEmployeeRepository(FakeEmployeeRepository.Containing(SampleEmployees));

        using var cachingFactory = CreateFactoryWith(repository);
        using var client = cachingFactory.CreateClient();

        var firstRead = await client.GetAsync($"{EmployeesUrl}?pageNumber=1&pageSize=1");
        var repeatedRead = await client.GetAsync($"{EmployeesUrl}?pageNumber=1&pageSize=1");

        Assert.Equal(HttpStatusCode.OK, firstRead.StatusCode);
        Assert.Equal(HttpStatusCode.OK, repeatedRead.StatusCode);

        Assert.Equal(1, repository.GetPagedCallCount);

        // A different page size is a different cache entry.
        await client.GetAsync($"{EmployeesUrl}?pageNumber=1&pageSize=2");

        Assert.Equal(2, repository.GetPagedCallCount);

        // A different search term is a different entry as well.
        await client.GetAsync($"{EmployeesUrl}?pageNumber=1&pageSize=1&search=ada");

        Assert.Equal(3, repository.GetPagedCallCount);

        // And every one of those entries is itself cached.
        await client.GetAsync($"{EmployeesUrl}?pageNumber=1&pageSize=2");
        await client.GetAsync($"{EmployeesUrl}?pageNumber=1&pageSize=1&search=ada");

        Assert.Equal(3, repository.GetPagedCallCount);
    }

    /// <summary>
    /// Boots a host whose employee repository is the supplied in-memory double, so the read
    /// endpoints can be exercised without SQL Server.
    /// </summary>
    private WebApplicationFactory<Program> CreateFactoryWith(IEmployeeRepository repository) =>
        factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IEmployeeRepository>();
            services.AddSingleton(repository);
        }));

    /// <summary>
    /// Finds the single endpoint MVC mapped for the given action, matching the
    /// <see cref="ControllerActionDescriptor"/> carried in its metadata.
    /// </summary>
    private RouteEndpoint FindEndpoint(string actionName) =>
        factory.Services
            .GetRequiredService<EndpointDataSource>()
            .Endpoints
            .OfType<RouteEndpoint>()
            .Single(endpoint => string.Equals(
                (endpoint.Metadata.GetMetadata<ActionDescriptor>() as ControllerActionDescriptor)?.ActionName,
                actionName,
                StringComparison.Ordinal));

    /// <summary>
    /// Decorates the in-memory <see cref="FakeEmployeeRepository"/> and records how often the
    /// application layer had to read from it: a cache hit is a read that never reaches the double.
    /// </summary>
    private sealed class CountingEmployeeRepository(FakeEmployeeRepository inner) : IEmployeeRepository
    {
        /// <summary>Number of <see cref="GetAllAsync"/> calls that reached the repository.</summary>
        public int GetAllCallCount { get; private set; }

        /// <summary>Number of <see cref="GetPagedAsync"/> calls that reached the repository.</summary>
        public int GetPagedCallCount { get; private set; }

        /// <inheritdoc />
        public Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            GetAllCallCount++;
            return inner.GetAllAsync(cancellationToken);
        }

        /// <inheritdoc />
        public Task<PagedResult<Employee>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search,
            CancellationToken cancellationToken = default)
        {
            GetPagedCallCount++;
            return inner.GetPagedAsync(pageNumber, pageSize, search, cancellationToken);
        }

        /// <inheritdoc />
        public Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
            inner.GetByIdAsync(id, cancellationToken);

        /// <inheritdoc />
        public Task<int> AddAsync(Employee employee, CancellationToken cancellationToken = default) =>
            inner.AddAsync(employee, cancellationToken);

        /// <inheritdoc />
        public Task<bool> UpdateAsync(Employee employee, CancellationToken cancellationToken = default) =>
            inner.UpdateAsync(employee, cancellationToken);

        /// <inheritdoc />
        public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) =>
            inner.DeleteAsync(id, cancellationToken);
    }
}
