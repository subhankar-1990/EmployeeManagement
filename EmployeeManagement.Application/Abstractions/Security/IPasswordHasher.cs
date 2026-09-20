namespace EmployeeManagement.Application.Abstractions.Security;

/// <summary>
/// Hashes and verifies passwords using a one-way key derivation function.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Produces an encoded, salted password hash that can be persisted.
    /// </summary>
    string Hash(string password);

    /// <summary>
    /// Verifies a clear text password against an encoded hash in constant time.
    /// </summary>
    bool Verify(string password, string passwordHash);

    /// <summary>
    /// Performs constant-time dummy verification against a throwaway hash.
    /// </summary>
    void PerformDummyVerification(string password);
}

