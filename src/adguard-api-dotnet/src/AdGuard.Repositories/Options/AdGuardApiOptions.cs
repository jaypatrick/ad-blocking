using System.ComponentModel.DataAnnotations;

namespace AdGuard.Repositories.Options;

/// <summary>
/// Configuration options for the AdGuard DNS API client.
/// </summary>
/// <remarks>
/// These options can be bound from configuration files, environment variables,
/// or other configuration sources using the Options pattern.
/// </remarks>
/// <example>
/// In appsettings.json:
/// <code>
/// {
///   "AdGuard": {
///     "Api": {
///       "ApiKey": "your-api-key",
///       "BasePath": "https://api.adguard-dns.io",
///       "TimeoutMilliseconds": 30000,
///       "RetryCount": 3
///     }
///   }
/// }
/// </code>
///
/// In Program.cs or Startup.cs:
/// <code>
/// services.Configure&lt;AdGuardApiOptions&gt;(
///     configuration.GetSection(AdGuardApiOptions.SectionName));
/// </code>
/// </example>
public class AdGuardApiOptions
{
    /// <summary>
    /// The configuration section name for these options.
    /// </summary>
    public const string SectionName = "AdGuard:Api";

    /// <summary>
    /// Gets or sets the API key for authentication.
    /// </summary>
    /// <remarks>
    /// The API key can be obtained from the AdGuard DNS dashboard.
    /// </remarks>
    [Required(ErrorMessage = "API key is required")]
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the base path for API requests.
    /// </summary>
    /// <remarks>
    /// Defaults to the production API endpoint.
    /// </remarks>
    [Required(ErrorMessage = "Base path is required")]
    [Url(ErrorMessage = "Base path must be a valid URL")]
    public string BasePath { get; set; } = "https://api.adguard-dns.io";

    /// <summary>
    /// Gets or sets the request timeout in milliseconds.
    /// </summary>
    /// <remarks>
    /// Defaults to 30 seconds.
    /// </remarks>
    [Range(1000, 600000, ErrorMessage = "Timeout must be between 1 and 600 seconds")]
    public int TimeoutMilliseconds { get; set; } = 30000;

    /// <summary>
    /// Gets or sets the number of retry attempts for failed requests.
    /// </summary>
    /// <remarks>
    /// Retries are performed for transient errors (408, 429, 5xx).
    /// </remarks>
    [Range(0, 10, ErrorMessage = "Retry count must be between 0 and 10")]
    public int RetryCount { get; set; } = 3;

    /// <summary>
    /// Gets or sets the user agent string sent with API requests.
    /// </summary>
    public string UserAgent { get; set; } = "AdGuard.ApiClient/1.0";

    /// <summary>
    /// Gets or sets a value indicating whether to enable debug logging.
    /// </summary>
    public bool EnableDebugLogging { get; set; }

    /// <summary>
    /// Gets the timeout as a TimeSpan.
    /// </summary>
    public TimeSpan Timeout => TimeSpan.FromMilliseconds(TimeoutMilliseconds);

    /// <summary>
    /// Validates the options.
    /// </summary>
    /// <returns>A list of validation errors, if any.</returns>
    public IEnumerable<ValidationResult> Validate()
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(this);
        Validator.TryValidateObject(this, context, results, validateAllProperties: true);
        return results;
    }

    /// <summary>
    /// Gets a value indicating whether the options are valid.
    /// </summary>
    public bool IsValid => !Validate().Any();
}

/// <summary>
/// Configuration options for caching behavior.
/// </summary>
public class CacheOptions
{
    /// <summary>
    /// The configuration section name for these options.
    /// </summary>
    public const string SectionName = "AdGuard:Cache";

    /// <summary>
    /// Gets or sets a value indicating whether caching is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the default cache duration in minutes.
    /// </summary>
    [Range(1, 1440, ErrorMessage = "Cache duration must be between 1 and 1440 minutes")]
    public int DefaultDurationMinutes { get; set; } = 5;

    /// <summary>
    /// Gets or sets the cache duration for device lists in minutes.
    /// </summary>
    [Range(1, 60, ErrorMessage = "Device cache duration must be between 1 and 60 minutes")]
    public int DevicesDurationMinutes { get; set; } = 2;

    /// <summary>
    /// Gets or sets the cache duration for DNS server lists in minutes.
    /// </summary>
    [Range(1, 60, ErrorMessage = "DNS server cache duration must be between 1 and 60 minutes")]
    public int DnsServersDurationMinutes { get; set; } = 5;

    /// <summary>
    /// Gets or sets the cache duration for filter lists in minutes.
    /// </summary>
    [Range(1, 1440, ErrorMessage = "Filter list cache duration must be between 1 and 1440 minutes")]
    public int FilterListsDurationMinutes { get; set; } = 30;

    /// <summary>
    /// Gets the default cache duration as a TimeSpan.
    /// </summary>
    public TimeSpan DefaultDuration => TimeSpan.FromMinutes(DefaultDurationMinutes);

    /// <summary>
    /// Gets the device cache duration as a TimeSpan.
    /// </summary>
    public TimeSpan DevicesDuration => TimeSpan.FromMinutes(DevicesDurationMinutes);

    /// <summary>
    /// Gets the DNS server cache duration as a TimeSpan.
    /// </summary>
    public TimeSpan DnsServersDuration => TimeSpan.FromMinutes(DnsServersDurationMinutes);

    /// <summary>
    /// Gets the filter list cache duration as a TimeSpan.
    /// </summary>
    public TimeSpan FilterListsDuration => TimeSpan.FromMinutes(FilterListsDurationMinutes);
}

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
