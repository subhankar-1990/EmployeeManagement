namespace EmployeeManagement.Application.Configuration;

/// <summary>
/// Detects the placeholder text used in <c>appsettings.json</c> so that an unconfigured
/// deployment fails fast instead of starting with a dummy secret.
/// </summary>
public static class ConfigurationPlaceholder
{
    /// <summary>Prefix shared by every placeholder value committed to source control.</summary>
    public const string Marker = "Define in";

    /// <summary>
    /// Determines whether <paramref name="value"/> is missing or still holds a placeholder value.
    /// </summary>
    public static bool IsPlaceholder(string? value) =>
        string.IsNullOrWhiteSpace(value)
        || value.TrimStart().StartsWith(Marker, StringComparison.OrdinalIgnoreCase);
}

