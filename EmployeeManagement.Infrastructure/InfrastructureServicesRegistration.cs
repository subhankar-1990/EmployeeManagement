using EmployeeManagement.Application.Abstractions.Persistence;
using EmployeeManagement.Application.Abstractions.Security;
using EmployeeManagement.Application.Configuration;
using EmployeeManagement.Infrastructure.Context;
using EmployeeManagement.Infrastructure.Repositories.Authentication;
using EmployeeManagement.Infrastructure.Repositories.Employees;
using EmployeeManagement.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.Infrastructure;

/// <summary>
/// Registers the Infrastructure layer services.
/// </summary>
public static class InfrastructureServicesRegistration
{
    /// <summary>
    /// Adds the database context, repositories and security services.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">Application configuration used to resolve the connection string.</param>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (!string.IsNullOrWhiteSpace(connectionString) && !ConfigurationPlaceholder.IsPlaceholder(connectionString))
        {
            services.AddDbContext<EmployeeDbContext>(options =>
                options.UseSqlServer(
                    connectionString,
                    sqlServer => sqlServer
                        .EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null)
                        .CommandTimeout(15)));
        }
        else
        {
            // Configures fallback SqlServer connection string so DbContext is registered
            // even when a real database is not yet configured.
            services.AddDbContext<EmployeeDbContext>(options =>
                options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=EmployeeManagement;Trusted_Connection=True;TrustServerCertificate=True;"));
        }

        // Required by the in-process failed sign in tracker
        services.AddMemoryCache();

        services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        services.AddSingleton<ILoginAttemptTracker, InMemoryLoginAttemptTracker>();
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();

        return services;
    }
}

