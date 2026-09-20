using EmployeeManagement.Application.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.Application;

/// <summary>
/// Registers the Application layer services.
/// </summary>
public static class ApplicationServiceRegistration
{
    /// <summary>
    /// Adds mapping, request handling and validation services discovered in this assembly.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var applicationAssembly = typeof(ApplicationServiceRegistration).Assembly;

        services.AddAutoMapper(configuration => configuration.AddMaps(applicationAssembly));
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(applicationAssembly));

        // Scans for every AbstractValidator<T> in the application assembly.
        services.AddValidatorsFromAssembly(applicationAssembly, includeInternalTypes: true);

        // Pipeline behaviors execute in registration order: logging is outermost, then validation.
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestLoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}

