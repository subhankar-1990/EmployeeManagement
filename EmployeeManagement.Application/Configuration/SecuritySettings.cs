namespace EmployeeManagement.Application.Configuration;

/// <summary>
/// Strongly typed representation of the <c>Security</c> configuration section.
/// </summary>
public sealed class SecuritySettings
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "Security";

    /// <summary>
    /// Number of consecutive failed sign in attempts before an account is temporarily locked.
    /// Defaults to 5.
    /// </summary>
    public int MaxFailedSignInAttempts { get; set; } = 5;

    /// <summary>
    /// Duration of the temporary lockout in minutes. Defaults to 15.
    /// </summary>
    public int LockoutMinutes { get; set; } = 15;

    /// <summary>
    /// PBKDF2 iteration count used when hashing passwords. Defaults to 210,000 (OWASP guidance).
    /// </summary>
    public int PasswordHashIterations { get; set; } = 210_000;
}

