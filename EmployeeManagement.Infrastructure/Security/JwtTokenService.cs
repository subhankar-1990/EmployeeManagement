using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmployeeManagement.Application.Abstractions.Security;
using EmployeeManagement.Application.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManagement.Infrastructure.Security;

/// <summary>
/// Issues HS256 signed JWT access tokens.
/// </summary>
internal sealed class JwtTokenService : ITokenService
{
    private readonly JwtSettings _settings;

    public JwtTokenService(IOptions<JwtSettings> jwtSettings)
    {
        _settings = jwtSettings.Value;
    }

    /// <inheritdoc />
    public IssuedToken CreateAccessToken(long employeeNumber, string name, IEnumerable<string> roles)
    {
        var utcNow = DateTime.UtcNow;
        var expiresAtUtc = utcNow.AddMinutes(_settings.ExpiryMinutes);
        var subject = employeeNumber.ToString(CultureInfo.InvariantCulture);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, subject),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new(ClaimTypes.NameIdentifier, subject),
            new(ClaimTypes.Name, name)
        };

        claims.AddRange(roles
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(role => new Claim(ClaimTypes.Role, role.Trim())));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            NotBefore = utcNow,
            IssuedAt = utcNow,
            Expires = expiresAtUtc,
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

        return new IssuedToken(token, expiresAtUtc, "Bearer");
    }
}

