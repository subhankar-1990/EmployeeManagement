using AutoMapper;
using EmployeeManagement.Application;
using EmployeeManagement.Application.Contracts.Employees;
using EmployeeManagement.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Tests.Mapping;

/// <summary>
/// Guards the mapping configuration that stands between the domain model and the API contract.
/// </summary>
/// <remarks>
/// The mapper is resolved through <c>AddApplicationServices</c> so that the production
/// registration path is exercised rather than a hand built configuration.
/// </remarks>
public sealed class MappingProfileTests
{
    private static readonly IServiceProvider ServiceProvider = BuildServiceProvider();

    private static IMapper Mapper => ServiceProvider.GetRequiredService<IMapper>();

    [Fact]
    public void AutoMapperConfiguration_IsValid()
    {
        // Throws AutoMapperConfigurationException when a destination member is unmapped.
        Mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public void Employee_IsMappedToEmployeeResponse_FieldByField()
    {
        var employee = new Employee
        {
            EmpId = 42,
            Name = "Ada Lovelace",
            Mobile = "9876543210",
            Email = "ada@example.com",
            IsActive = true
        };

        var response = Mapper.Map<EmployeeResponse>(employee);

        Assert.Equal(42, response.Id);
        Assert.Equal("Ada Lovelace", response.Name);
        Assert.Equal("9876543210", response.Mobile);
        Assert.Equal("ada@example.com", response.Email);
        Assert.True(response.IsActive);
    }

    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));
        services.AddApplicationServices();

        return services.BuildServiceProvider();
    }
}
