namespace AdGuard.Repositories.Options;

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