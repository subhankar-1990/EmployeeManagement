using AutoMapper;
using EmployeeManagement.Application.Contracts.Employees;
using EmployeeManagement.Domain.Models;

namespace EmployeeManagement.Application.MappingProfiles;

/// <summary>
/// AutoMapper profile describing how domain models are projected onto API contracts.
/// </summary>
/// <remarks>
/// Every destination member is mapped explicitly so that <c>AssertConfigurationIsValid</c>
/// fails the build (through the unit test suite) if the contract and model drift apart.
/// </remarks>
public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Employee, EmployeeResponse>()
            .ForMember(destination => destination.Id, options => options.MapFrom(source => source.EmpId))
            .ForMember(destination => destination.Name, options => options.MapFrom(source => source.Name))
            .ForMember(destination => destination.Mobile, options => options.MapFrom(source => source.Mobile))
            .ForMember(destination => destination.Email, options => options.MapFrom(source => source.Email))
            .ForMember(destination => destination.IsActive, options => options.MapFrom(source => source.IsActive));
    }
}
