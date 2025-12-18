namespace AdGuard.DataAccess.Entities;

/// <summary>
/// Represents a cached filter list record from the AdGuard API.
/// </summary>
public class FilterListCacheEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for this cache entry.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the filter list identifier from the AdGuard API.
    /// </summary>
    public required string FilterListId { get; set; }

    /// <summary>
    /// Gets or sets the filter list name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the filter list URL.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets whether the filter list is enabled.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets the number of rules in the filter list.
    /// </summary>
    public int? RuleCount { get; set; }

    /// <summary>
    /// Gets or sets the filter list description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the filter list homepage.
    /// </summary>
    public string? Homepage { get; set; }

    /// <summary>
    /// Gets or sets when the filter list was last updated.
    /// </summary>
    public DateTime? LastUpdated { get; set; }

    /// <summary>
    /// Gets or sets the DNS server ID this filter is associated with (if server-specific).
    /// </summary>
    public string? DnsServerId { get; set; }

    /// <summary>
    /// Gets or sets when this cache entry was created.
    /// </summary>
    public DateTime CachedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets when this cache entry was last synced from the API.
    /// </summary>
    public DateTime LastSyncedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets whether this cache entry is stale and needs refresh.
    /// </summary>
    public bool IsStale { get; set; }

    /// <summary>
    /// Gets or sets the ETag from the API for cache validation.
    /// </summary>
    public string? ETag { get; set; }
}
