using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace EmployeeManagement.Tests.Api;

/// <summary>
/// Verifies the versioned employee list endpoint at the HTTP boundary.
/// </summary>
/// <remarks>
/// Only the paths that run before data access are asserted. Rejecting a bad page size is handled by
/// the validation pipeline, which lets the endpoint be covered without a database.
/// </remarks>
public sealed class EmployeesEndpointTests(EmployeeManagementApiFactory factory)
    : IClassFixture<EmployeeManagementApiFactory>
{
    private const string EmployeesUrl = "/api/v1/employees";

    [Theory]
    [InlineData("pageNumber=0")]
    [InlineData("pageNumber=-3")]
    [InlineData("pageSize=0")]
    [InlineData("pageSize=101")]
    [InlineData("pageNumber=0&pageSize=0")]
    public async Task GetEmployees_WithInvalidPaging_ReturnsBadRequestWithFieldErrors(string queryString)
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"{EmployeesUrl}?{queryString}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        var problem = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.True(problem.TryGetProperty("errors", out var errors));
        Assert.NotEqual(JsonValueKind.Undefined, errors.ValueKind);

        // A correlation id lets an operator join the response to the server log.
        Assert.True(problem.TryGetProperty("traceId", out _));
    }

    [Theory]
    [InlineData("/api/employees")]
    [InlineData("/api/v2/employees")]
    public async Task GetEmployees_WithoutASupportedVersion_IsNotRouted(string url)
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
