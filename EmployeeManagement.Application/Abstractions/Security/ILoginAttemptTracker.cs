namespace EmployeeManagement.Application.Abstractions.Security;

/// <summary>
/// Tracks consecutive failed sign in attempts so that credentials can be throttled.
/// </summary>
public interface ILoginAttemptTracker
{
    /// <summary>
    /// Determines whether the account has exceeded the configured failed attempt threshold.
    /// </summary>
    bool IsLockedOut(long employeeNumber);

    /// <summary>
    /// Records a failed attempt and returns the resulting consecutive failure count.
    /// </summary>
    int RegisterFailure(long employeeNumber);

    /// <summary>
    /// Clears the failure counter after a successful sign in.
    /// </summary>
    void Reset(long employeeNumber);
}

