namespace AdGuard.Repositories.Options;

/// <summary>
/// Configuration options for the Console UI application.
/// </summary>
public class ConsoleUIOptions
{
    /// <summary>
    /// The configuration section name for these options.
    /// </summary>
    public const string SectionName = "AdGuard:ConsoleUI";

    /// <summary>
    /// Gets or sets a value indicating whether to show detailed errors.
    /// </summary>
    public bool ShowDetailedErrors { get; set; }

    /// <summary>
    /// Gets or sets the default page size for list displays.
    /// </summary>
    [Range(5, 100, ErrorMessage = "Page size must be between 5 and 100")]
    public int DefaultPageSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets the default time range for statistics in days.
    /// </summary>
    [Range(1, 90, ErrorMessage = "Statistics days must be between 1 and 90")]
    public int DefaultStatisticsDays { get; set; } = 7;

    /// <summary>
    /// Gets or sets the color theme.
    /// </summary>
    public string Theme { get; set; } = "default";
}