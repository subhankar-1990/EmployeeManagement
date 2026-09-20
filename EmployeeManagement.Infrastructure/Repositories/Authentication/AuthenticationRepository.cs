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
    /// <inheritdoc />
    public async Task<UserCredential?> GetCredentialAsync(long employeeNumber, CancellationToken cancellationToken)
    {
        var auth = await context.AuthMasters
            .AsNoTracking()
            .FirstOrDefaultAsync(record => record.EmpNo == employeeNumber, cancellationToken);

        return auth is null
            ? null
            : new UserCredential
            {
                Id = auth.AuthId,
                EmployeeNumber = auth.EmpNo,
                PasswordHash = auth.PasswordHash,
                IsLocked = auth.IsLocked,
                Roles = ParseRoles(auth.Role)
            };
    }

    private static string[] ParseRoles(string? roles) =>
        string.IsNullOrWhiteSpace(roles)
            ? []
            : roles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}

