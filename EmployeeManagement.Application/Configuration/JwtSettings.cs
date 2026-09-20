namespace EmployeeManagement.Application.Configuration;

/// <summary>
/// Strongly typed representation of the <c>JwtSettings</c> configuration section.
/// </summary>
public sealed class JwtSettings
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "JwtSettings";

    /// <summary>
    /// HMAC signing secret. Must be at least 32 bytes (256 bits) to satisfy HS256.
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>Token issuer (<c>iss</c> claim).</summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>Intended audience (<c>aud</c> claim).</summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>Access token lifetime in minutes. Defaults to 30 minutes.</summary>
    public int ExpiryMinutes { get; set; } = 30;
}

