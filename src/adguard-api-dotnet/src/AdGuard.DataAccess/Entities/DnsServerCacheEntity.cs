namespace AdGuard.DataAccess.Entities;

/// <summary>
/// Represents a cached DNS server record from the AdGuard API.
/// </summary>
public class DnsServerCacheEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for this cache entry.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the DNS server identifier from the AdGuard API.
    /// </summary>
    public required string ServerId { get; set; }

    /// <summary>
    /// Gets or sets the DNS server name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets whether this is the default DNS server.
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Gets or sets the JSON-serialized list of device IDs associated with this server.
    /// </summary>
    public string? DeviceIdsJson { get; set; }

    /// <summary>
    /// Gets or sets the JSON-serialized DNS server settings.
    /// </summary>
    public string? SettingsJson { get; set; }

    /// <summary>
    /// Gets or sets the JSON-serialized DNS addresses.
    /// </summary>
    public string? DnsAddressesJson { get; set; }

    /// <summary>
    /// Gets or sets when the DNS server was created in AdGuard.
    /// </summary>
    public DateTime? CreatedInAdGuard { get; set; }

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
