namespace AdGuard.DataAccess.Entities;

/// <summary>
/// Represents a DNS query log entry stored in the local database.
/// </summary>
public class QueryLogEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for this query log entry.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the external identifier from the AdGuard API.
    /// </summary>
    public string? ExternalId { get; set; }

    /// <summary>
    /// Gets or sets the domain that was queried.
    /// </summary>
    public required string Domain { get; set; }

    /// <summary>
    /// Gets or sets the DNS response code (e.g., NOERROR, NXDOMAIN, SERVFAIL).
    /// </summary>
    public string? ResponseCode { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the query was made.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the device identifier that made the query.
    /// </summary>
    public string? DeviceId { get; set; }

    /// <summary>
    /// Gets or sets the DNS server identifier that processed the query.
    /// </summary>
    public string? DnsServerId { get; set; }

    /// <summary>
    /// Gets or sets the query response time in milliseconds.
    /// </summary>
    public long? ResponseTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the reason if the query was blocked.
    /// </summary>
    public string? BlockedReason { get; set; }

    /// <summary>
    /// Gets or sets whether the query was blocked.
    /// </summary>
    public bool IsBlocked { get; set; }

    /// <summary>
    /// Gets or sets the DNS query type (e.g., A, AAAA, CNAME, MX).
    /// </summary>
    public string? QueryType { get; set; }

    /// <summary>
    /// Gets or sets the client IP address that made the query.
    /// </summary>
    public string? ClientIp { get; set; }

    /// <summary>
    /// Gets or sets the upstream DNS server that was used.
    /// </summary>
    public string? UpstreamServer { get; set; }

    /// <summary>
    /// Gets or sets the filter list that caused the block (if applicable).
    /// </summary>
    public string? FilterListName { get; set; }

    /// <summary>
    /// Gets or sets the specific rule that matched (if blocked).
    /// </summary>
    public string? MatchedRule { get; set; }

    /// <summary>
    /// Gets or sets additional metadata as JSON.
    /// </summary>
    public string? MetadataJson { get; set; }

    /// <summary>
    /// Gets or sets when this record was created locally.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets when this record was last synced from the API.
    /// </summary>
    public DateTime? SyncedAt { get; set; }
}
