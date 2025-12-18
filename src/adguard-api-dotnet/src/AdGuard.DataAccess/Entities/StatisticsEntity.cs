namespace AdGuard.DataAccess.Entities;

/// <summary>
/// Represents aggregated DNS statistics for a specific time period.
/// </summary>
public class StatisticsEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for this statistics entry.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the date for which these statistics apply.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the DNS server identifier these statistics belong to.
    /// </summary>
    public string? DnsServerId { get; set; }

    /// <summary>
    /// Gets or sets the device identifier these statistics belong to (null for server-wide stats).
    /// </summary>
    public string? DeviceId { get; set; }

    /// <summary>
    /// Gets or sets the total number of DNS queries.
    /// </summary>
    public long TotalQueries { get; set; }

    /// <summary>
    /// Gets or sets the number of blocked queries.
    /// </summary>
    public long BlockedQueries { get; set; }

    /// <summary>
    /// Gets or sets the number of allowed queries.
    /// </summary>
    public long AllowedQueries { get; set; }

    /// <summary>
    /// Gets or sets the number of cached responses.
    /// </summary>
    public long CachedQueries { get; set; }

    /// <summary>
    /// Gets or sets the average response time in milliseconds.
    /// </summary>
    public double AverageResponseTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the JSON-serialized dictionary of top blocked domains with counts.
    /// </summary>
    public string? TopBlockedDomainsJson { get; set; }

    /// <summary>
    /// Gets or sets the JSON-serialized dictionary of top allowed domains with counts.
    /// </summary>
    public string? TopAllowedDomainsJson { get; set; }

    /// <summary>
    /// Gets or sets the JSON-serialized dictionary of top clients with query counts.
    /// </summary>
    public string? TopClientsJson { get; set; }

    /// <summary>
    /// Gets or sets the JSON-serialized dictionary of query types with counts.
    /// </summary>
    public string? QueryTypesJson { get; set; }

    /// <summary>
    /// Gets or sets the JSON-serialized dictionary of upstream servers with counts.
    /// </summary>
    public string? UpstreamServersJson { get; set; }

    /// <summary>
    /// Gets or sets the granularity of this statistics entry (e.g., Hourly, Daily, Weekly).
    /// </summary>
    public StatisticsGranularity Granularity { get; set; } = StatisticsGranularity.Daily;

    /// <summary>
    /// Gets or sets when this record was created locally.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets when this record was last synced from the API.
    /// </summary>
    public DateTime? SyncedAt { get; set; }
}

/// <summary>
/// Represents the time granularity for statistics aggregation.
/// </summary>
public enum StatisticsGranularity
{
    /// <summary>
    /// Statistics aggregated by hour.
    /// </summary>
    Hourly = 0,

    /// <summary>
    /// Statistics aggregated by day.
    /// </summary>
    Daily = 1,

    /// <summary>
    /// Statistics aggregated by week.
    /// </summary>
    Weekly = 2,

    /// <summary>
    /// Statistics aggregated by month.
    /// </summary>
    Monthly = 3
}
