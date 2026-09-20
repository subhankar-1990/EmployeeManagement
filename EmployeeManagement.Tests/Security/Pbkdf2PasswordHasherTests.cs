using EmployeeManagement.Application.Configuration;
using EmployeeManagement.Infrastructure.Security;
using Microsoft.Extensions.Options;

namespace EmployeeManagement.Tests.Security;

/// <summary>
/// Verifies the password hashing contract.
/// </summary>
public sealed class Pbkdf2PasswordHasherTests
{
    private const string Password = "Correct-Horse-Battery-Staple-42";

    private static Pbkdf2PasswordHasher CreateHasher(int iterations = 100_000) =>
        new(Options.Create(new SecuritySettings { PasswordHashIterations = iterations }));

    [Fact]
    public void Hash_AndThenVerify_WithTheCorrectPassword_Succeeds()
    {
        var hasher = CreateHasher();

        var hash = hasher.Hash(Password);

        Assert.True(hasher.Verify(Password, hash));
    }

    [Fact]
    public void Verify_WithAnIncorrectPassword_Fails()
    {
        var hasher = CreateHasher();

        var hash = hasher.Hash(Password);

        Assert.False(hasher.Verify("Wrong-Password", hash));
        Assert.False(hasher.Verify(string.Empty, hash));
    }

    [Fact]
    public void Hash_UsesARandomSalt_SoTheSamePasswordProducesDifferentHashes()
    {
        var hasher = CreateHasher();

        var first = hasher.Hash(Password);
        var second = hasher.Hash(Password);

        Assert.NotEqual(first, second);
        Assert.True(hasher.Verify(Password, first));
        Assert.True(hasher.Verify(Password, second));
    }

    [Fact]
    public void Hash_DoesNotEmbedTheClearTextPassword()
    {
        var hasher = CreateHasher();

        var hash = hasher.Hash(Password);

        Assert.DoesNotContain(Password, hash, StringComparison.OrdinalIgnoreCase);
        Assert.StartsWith("PBKDF2-SHA256.", hash, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("")]
    [InlineData("plaintext-password")]
    [InlineData("PBKDF2-SHA256.not-a-number.c2FsdA==.a2V5")]
    [InlineData("PBKDF2-SHA256.1000.!!!not-base64!!!.a2V5")]
    [InlineData("PBKDF2-SHA256.1000.c2FsdA==")]
    [InlineData("MD5.1000.c2FsdA==.a2V5")]
    public void Verify_WithAMalformedHash_ReturnsFalseInsteadOfThrowing(string malformedHash)
    {
        var hasher = CreateHasher();

        Assert.False(hasher.Verify(Password, malformedHash));
    }

    [Fact]
    public void Hash_WithAWhitespacePassword_ThrowsArgumentException()
    {
        var hasher = CreateHasher();

        Assert.Throws<ArgumentException>(() => hasher.Hash("   "));
    }

    [Fact]
    public void PerformDummyVerification_DoesNotThrowAndDoesNotReturnAnything()
    {
        var hasher = CreateHasher(iterations: 1_000);

        hasher.PerformDummyVerification(Password);
        hasher.PerformDummyVerification(string.Empty);
    }
}

