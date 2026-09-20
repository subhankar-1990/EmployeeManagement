using EmployeeManagement.Domain.Models;

namespace EmployeeManagement.Application.Abstractions.Persistence;

/// <summary>
/// Read access to authentication material.
/// </summary>
public interface IAuthenticationRepository
{
    /// <summary>
    /// Retrieves the stored credential for an employee number.
    /// </summary>
    /// <param name="employeeNumber">The employee number to look up.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The credential, or <see langword="null"/> when the employee number is not registered.</returns>
    Task<UserCredential?> GetCredentialAsync(long employeeNumber, CancellationToken cancellationToken);
}

