using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace EmployeeManagement.Tests.Api;

/// <summary>
/// Boots the real API in-memory with test configuration.
/// </summary>
/// <remarks>
/// A connection string is supplied only so the DbContext can be registered. No test in this class
/// reaches the database, so no SQL Server instance is required.
/// </remarks>
public sealed class EmployeeManagementApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.UseSetting(
            "ConnectionStrings:DefaultConnection",
            "Server=(localdb)\\MSSQLLocalDB;Database=EmployeeManagementTests;Trusted_Connection=True;TrustServerCertificate=True");
    }
}
