using System.Globalization;
using System.Security.Cryptography;
using EmployeeManagement.Application.Abstractions.Security;
using EmployeeManagement.Application.Configuration;
using Microsoft.Extensions.Options;

namespace EmployeeManagement.Infrastructure.Security;

/// <summary>
/// PBKDF2 (SHA-256) password hasher with a per-password random salt.
/// </summary>
internal sealed class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const string Algorithm = "PBKDF2-SHA256";
    private const int SaltSizeInBytes = 16;
    private const int KeySizeInBytes = 32;
    private const char Delimiter = '.';
    private const int FieldCount = 4;

    private static readonly byte[] DummySalt = new byte[SaltSizeInBytes];

    private readonly int _iterations;

    public Pbkdf2PasswordHasher(IOptions<SecuritySettings> securitySettings)
    {
        _iterations = securitySettings.Value.PasswordHashIterations;
    }

    /// <inheritdoc />
    public string Hash(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var salt = RandomNumberGenerator.GetBytes(SaltSizeInBytes);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, _iterations, HashAlgorithmName.SHA256, KeySizeInBytes);

        return string.Join(
            Delimiter,
            Algorithm,
            _iterations.ToString(CultureInfo.InvariantCulture),
            Convert.ToBase64String(salt),
            Convert.ToBase64String(key));
    }

    /// <inheritdoc />
    public bool Verify(string password, string passwordHash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(passwordHash))
        {
            return false;
        }

        var parts = passwordHash.Split(Delimiter);

        if (parts.Length != FieldCount || !string.Equals(parts[0], Algorithm, StringComparison.Ordinal))
        {
            return false;
        }

        if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var iterations) || iterations < 1)
        {
            return false;
        }

        try
        {
            var salt = Convert.FromBase64String(parts[2]);
            var expectedKey = Convert.FromBase64String(parts[3]);

            var actualKey = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                expectedKey.Length);

            return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    /// <inheritdoc />
    public void PerformDummyVerification(string password)
    {
        var derived = Rfc2898DeriveBytes.Pbkdf2(
            password ?? string.Empty,
            DummySalt,
            _iterations,
            HashAlgorithmName.SHA256,
            KeySizeInBytes);

        CryptographicOperations.ZeroMemory(derived);
    }
}

