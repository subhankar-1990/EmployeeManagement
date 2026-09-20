namespace EmployeeManagement.Domain.Common;

/// <summary>
/// Provides the current date and time for the application.
/// </summary>
public static class DateTimeProvider
{
    private static readonly TimeZoneInfo IndiaTimeZone = ResolveIndiaTimeZone();

    /// <summary>
    /// Gets the current date and time in India Standard Time.
    /// </summary>
    public static DateTime GetIndiaStandardTime() =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndiaTimeZone);

    private static TimeZoneInfo ResolveIndiaTimeZone()
    {
        foreach (var timeZoneId in new[] { "Asia/Kolkata", "India Standard Time" })
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            catch (TimeZoneNotFoundException)
            {
            }
            catch (InvalidTimeZoneException)
            {
            }
        }

        return TimeZoneInfo.Utc;
    }
}

