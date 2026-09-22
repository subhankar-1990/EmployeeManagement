using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EmployeeManagement.Api.Configuration;
using EmployeeManagement.Api.Idempotency;
using EmployeeManagement.Application.Abstractions.Persistence;
using EmployeeManagement.Application.Contracts.Employees;
using EmployeeManagement.Domain.Common;
using EmployeeManagement.Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EmployeeManagement.Tests.Api;

/// <summary>
/// Verifies the idempotency contract of the create employee endpoint: a retry that carries the same
/// <c>Idempotency-Key</c> is answered from the stored response instead of creating a second record.
/// </summary>
/// <remarks>
/// Nothing here needs a database. The repository is swapped for the in-memory double, so the only
/// thing a replayed request saves is a call to that double.
/// </remarks>
public sealed class EmployeeIdempotencyTests(EmployeeManagementApiFactory factory)
    : IClassFixture<EmployeeManagementApiFactory>
{
    private const string EmployeesUrl = "/api/v1/employees";

    [Fact]
    public void CreateEmployee_IsTheOnlyIdempotentAction()
    {
        var idempotentActions = factory.Services
            .GetRequiredService<EndpointDataSource>()
            .Endpoints
            .OfType<RouteEndpoint>()
            .Where(endpoint => endpoint.Metadata.GetMetadata<IdempotentAttribute>() is not null)
            .Select(endpoint => endpoint.Metadata.GetMetadata<ActionDescriptor>() is ControllerActionDescriptor descriptor
                ? descriptor.ActionName
                : null)
            .ToList();

        Assert.Equal(["CreateEmployee"], idempotentActions);
    }

    [Fact]
    public async Task CreateEmployee_WithTheSameKeyAndBody_ReplaysTheStoredResponse()
    {
        var repository = new RecordingEmployeeRepository();
        using var idempotentFactory = CreateFactoryWith(repository);
        using var client = idempotentFactory.CreateClient();

        var request = ValidRequest();
        var key = Guid.NewGuid().ToString("N");

        using var first = await PostAsync(client, request, key);
        using var retry = await PostAsync(client, request, key);

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.Created, retry.StatusCode);

        // The employee was created once and only once.
        Assert.Equal(1, repository.AddCallCount);

        // The first response is the real one, the second is marked as a replay.
        Assert.False(first.Headers.Contains(IdempotencyHeaders.Replayed));
        Assert.Equal("true", retry.Headers.GetValues(IdempotencyHeaders.Replayed).Single());

        // A replay reproduces the stored response byte for byte, Location header included.
        Assert.Equal(
            await first.Content.ReadAsStringAsync(),
            await retry.Content.ReadAsStringAsync());
        Assert.Equal(first.Headers.Location, retry.Headers.Location);

        var created = await first.Content.ReadFromJsonAsync<EmployeeResponse>();

        Assert.NotNull(created);
        Assert.Equal(1, created.Id);
        Assert.Equal(request.Name, created.Name);
    }

    [Fact]
    public async Task CreateEmployee_WithTheSameKeyAndADifferentBody_IsRejectedWithConflict()
    {
        var repository = new RecordingEmployeeRepository();
        using var idempotentFactory = CreateFactoryWith(repository);
        using var client = idempotentFactory.CreateClient();

        var key = Guid.NewGuid().ToString("N");

        using var first = await PostAsync(client, ValidRequest("Ada Lovelace"), key);
        using var reused = await PostAsync(client, ValidRequest("Grace Hopper"), key);

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, reused.StatusCode);
        Assert.Equal("application/problem+json", reused.Content.Headers.ContentType?.MediaType);

        // The payload that reused the key never reached the repository.
        Assert.Equal(1, repository.AddCallCount);

        var problem = await reused.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(409, problem.GetProperty("status").GetInt32());
        Assert.True(problem.TryGetProperty("traceId", out _));
    }

    [Fact]
    public async Task CreateEmployee_AfterARejectedPayload_AllowsTheKeyToBeReused()
    {
        var repository = new RecordingEmployeeRepository();
        using var idempotentFactory = CreateFactoryWith(repository);
        using var client = idempotentFactory.CreateClient();

        var key = Guid.NewGuid().ToString("N");

        // An invalid payload fails validation, so nothing is stored for the key.
        using var rejected = await PostAsync(
            client,
            new CreateEmployeeRequest { Name = string.Empty, Mobile = "9000000001", Email = "ada@example.com" },
            key);

        using var accepted = await PostAsync(client, ValidRequest(), key);

        Assert.Equal(HttpStatusCode.BadRequest, rejected.StatusCode);
        Assert.Equal(HttpStatusCode.Created, accepted.StatusCode);

        // The corrected payload ran for real: it was not replayed from the rejected attempt.
        Assert.False(accepted.Headers.Contains(IdempotencyHeaders.Replayed));
        Assert.Equal(1, repository.AddCallCount);
    }

    [Fact]
    public async Task CreateEmployee_WithoutAKey_IsNotIdempotent()
    {
        var repository = new RecordingEmployeeRepository();
        using var idempotentFactory = CreateFactoryWith(repository);
        using var client = idempotentFactory.CreateClient();

        using var first = await PostAsync(client, ValidRequest(), idempotencyKey: null);
        using var second = await PostAsync(client, ValidRequest(), idempotencyKey: null);

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.Created, second.StatusCode);

        // Idempotency is opt-in: two requests without a key create two records.
        Assert.Equal(2, repository.AddCallCount);
        Assert.False(second.Headers.Contains(IdempotencyHeaders.Replayed));
    }

    [Fact]
    public async Task CreateEmployee_WithAnOverlyLongKey_IsRejectedWithBadRequest()
    {
        var settings = factory.Services.GetRequiredService<IdempotencySettings>();
        var repository = new RecordingEmployeeRepository();
        using var idempotentFactory = CreateFactoryWith(repository);
        using var client = idempotentFactory.CreateClient();

        using var response = await PostAsync(client, ValidRequest(), new string('k', settings.MaxKeyLength + 1));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal(0, repository.AddCallCount);
    }

    [Fact]
    public async Task CreateEmployee_WithANonPrintableKey_IsRejectedWithBadRequest()
    {
        var repository = new RecordingEmployeeRepository();
        using var idempotentFactory = CreateFactoryWith(repository);
        using var client = idempotentFactory.CreateClient();

        // Only printable ASCII without spaces can be round-tripped through a header.
        using var response = await PostAsync(client, ValidRequest(), "key with spaces");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, repository.AddCallCount);
    }

    [Fact]
    public async Task CreateEmployee_WithARepeatedKeyHeader_IsRejectedWithBadRequest()
    {
        var repository = new RecordingEmployeeRepository();
        using var idempotentFactory = CreateFactoryWith(repository);
        using var client = idempotentFactory.CreateClient();

        using var message = new HttpRequestMessage(HttpMethod.Post, EmployeesUrl)
        {
            Content = JsonContent.Create(ValidRequest())
        };

        message.Headers.TryAddWithoutValidation(IdempotencyHeaders.Key, "key-one");
        message.Headers.TryAddWithoutValidation(IdempotencyHeaders.Key, "key-two");

        using var response = await client.SendAsync(message);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, repository.AddCallCount);
    }

    [Fact]
    public async Task CreateEmployee_WhileTheSameKeyIsStillRunning_IsRejectedWithConflict()
    {
        var repository = new RecordingEmployeeRepository { BlockAdds = true };
        using var idempotentFactory = CreateFactoryWith(repository);
        using var client = idempotentFactory.CreateClient();

        var request = ValidRequest();
        var key = Guid.NewGuid().ToString("N");

        // The first attempt reaches the repository and stays there until it is released.
        var firstCall = PostAsync(client, request, key);

        await repository.AddStarted.WaitAsync(TimeSpan.FromSeconds(10));

        using var whileRunning = await PostAsync(client, request, key);

        Assert.Equal(HttpStatusCode.Conflict, whileRunning.StatusCode);

        repository.LetAddsFinish();

        using var first = await firstCall;

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(1, repository.AddCallCount);

        // Now that the first attempt has stored its response, the retry is replayed instead.
        using var retry = await PostAsync(client, request, key);

        Assert.Equal(HttpStatusCode.Created, retry.StatusCode);
        Assert.Equal("true", retry.Headers.GetValues(IdempotencyHeaders.Replayed).Single());
        Assert.Equal(1, repository.AddCallCount);
    }

    [Fact]
    public async Task CreateEmployee_DocumentsTheIdempotencyHeader()
    {
        // The OpenAPI document only exists in Development, which is where it is mapped.
        using var documentationFactory = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Development"));
        using var client = documentationFactory.CreateClient();

        var document = await client.GetFromJsonAsync<JsonElement>("/openapi/v1.json");

        var post = document.GetProperty("paths").GetProperty(EmployeesUrl).GetProperty("post");

        var header = post
            .GetProperty("parameters")
            .EnumerateArray()
            .Single(parameter =>
                parameter.GetProperty("name").GetString() == IdempotencyHeaders.Key
                && parameter.GetProperty("in").GetString() == "header");

        Assert.True(header.TryGetProperty("description", out var description));
        Assert.False(string.IsNullOrWhiteSpace(description.GetString()));
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    /// <summary>Minimal valid create payload. The name is what makes two payloads differ.</summary>
    private static CreateEmployeeRequest ValidRequest(string name = "Ada Lovelace") => new()
    {
        Name = name,
        Mobile = "9000000001",
        Email = "ada@example.com",
        IsActive = true
    };

    /// <summary>Posts an employee, optionally carrying an idempotency key.</summary>
    private static Task<HttpResponseMessage> PostAsync(
        HttpClient client,
        CreateEmployeeRequest request,
        string? idempotencyKey)
    {
        var message = new HttpRequestMessage(HttpMethod.Post, EmployeesUrl)
        {
            Content = JsonContent.Create(request)
        };

        if (idempotencyKey is not null)
        {
            message.Headers.TryAddWithoutValidation(IdempotencyHeaders.Key, idempotencyKey);
        }

        return client.SendAsync(message);
    }

    /// <summary>
    /// Boots a host whose employee repository is the supplied in-memory double, so the create
    /// endpoint can be exercised without SQL Server.
    /// </summary>
    private WebApplicationFactory<Program> CreateFactoryWith(IEmployeeRepository repository) =>
        factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IEmployeeRepository>();
            services.AddSingleton(repository);
        }));

    /// <summary>
    /// In-memory repository double that records inserts and can hold them open, so the race between
    /// two requests that carry the same idempotency key can be exercised.
    /// </summary>
    private sealed class RecordingEmployeeRepository : IEmployeeRepository
    {
        private readonly ConcurrentDictionary<int, Employee> _employees = new();
        private readonly TaskCompletionSource _addStarted = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource _addsMayFinish = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private int _lastId;

        /// <summary>When set, <see cref="AddAsync"/> waits until <see cref="LetAddsFinish"/> is called.</summary>
        public bool BlockAdds { get; init; }

        /// <summary>Number of inserts that reached the repository.</summary>
        public int AddCallCount { get; private set; }

        /// <summary>Completes as soon as an insert has entered the repository.</summary>
        public Task AddStarted => _addStarted.Task;

        /// <summary>Lets a blocked insert finish.</summary>
        public void LetAddsFinish() => _addsMayFinish.TrySetResult();

        /// <inheritdoc />
        public async Task<int> AddAsync(Employee employee, CancellationToken cancellationToken = default)
        {
            _addStarted.TrySetResult();

            if (BlockAdds)
            {
                await _addsMayFinish.Task.WaitAsync(TimeSpan.FromSeconds(10), cancellationToken);
            }

            AddCallCount++;

            // The handler re-projects the persisted employee, so the request contract only needs a new id.
            var id = Interlocked.Increment(ref _lastId);

            _employees[id] = employee;

            return id;
        }

        /// <inheritdoc />
        public Task<PagedResult<Employee>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search,
            CancellationToken cancellationToken = default)
        {
            var items = _employees.Values
                .OrderBy(employee => employee.EmpId)
                .ToList();

            return Task.FromResult(new PagedResult<Employee>(items, pageNumber, pageSize, items.Count));
        }

        /// <inheritdoc />
        public Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
            Task.FromResult(_employees.TryGetValue(id, out var employee) ? employee : null);

        /// <inheritdoc />
        public Task<bool> UpdateAsync(Employee employee, CancellationToken cancellationToken = default) =>
            Task.FromResult(_employees.ContainsKey(employee.EmpId));

        /// <inheritdoc />
        public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) =>
            Task.FromResult(_employees.TryRemove(id, out _));
    }
}
