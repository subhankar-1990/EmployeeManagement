namespace EmployeeManagement.Application.Abstractions.Security;

/// <summary>
/// Creates signed access tokens for authenticated employees.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Creates a signed access token for the supplied employee.
    /// </summary>
    /// <param name="employeeNumber">Employee number used as the token subject.</param>
    /// <param name="name">Display name placed in the <c>name</c> claim.</param>
    /// <param name="roles">Roles granted to the employee.</param>
    /// <returns>The issued token including its expiry.</returns>
    IssuedToken CreateAccessToken(long employeeNumber, string name, IEnumerable<string> roles);
}

/// <summary>
/// A freshly issued access token.
/// </summary>
public sealed record IssuedToken(string AccessToken, DateTime ExpiresAtUtc, string TokenType);

