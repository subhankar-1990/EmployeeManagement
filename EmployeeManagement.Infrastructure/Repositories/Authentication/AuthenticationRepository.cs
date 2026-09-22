using EmployeeManagement.Application.Abstractions.Persistence;
using EmployeeManagement.Domain.Models;
using EmployeeManagement.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Repositories.Authentication;

/// <summary>
/// EF Core implementation of <see cref="IAuthenticationRepository"/>.
/// </summary>
internal sealed class AuthenticationRepository(EmployeeDbContext context) : IAuthenticationRepository
{
}
