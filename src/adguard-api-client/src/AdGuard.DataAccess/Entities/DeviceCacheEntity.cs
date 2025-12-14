namespace AdGuard.DataAccess.Entities;

/// <summary>
/// Represents a cached device record from the AdGuard API.
/// </summary>
public class DeviceCacheEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for this cache entry.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the device identifier from the AdGuard API.
    /// </summary>
    public required string DeviceId { get; set; }

    /// <summary>
    /// Gets or sets the device name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the device type (e.g., WINDOWS, ANDROID, MAC, IOS, LINUX).
    /// </summary>
    public required string DeviceType { get; set; }

    /// <summary>
    /// Gets or sets the DNS server identifier this device is associated with.
    /// </summary>
    public string? DnsServerId { get; set; }

    /// <summary>
    /// Gets or sets the DNS server name for display.
    /// </summary>
    public string? DnsServerName { get; set; }

    /// <summary>
    /// Gets or sets the JSON-serialized DNS addresses for this device.
    /// </summary>
    public string? DnsAddressesJson { get; set; }

    /// <summary>
    /// Gets or sets the JSON-serialized device settings.
    /// </summary>
    public string? SettingsJson { get; set; }

    /// <summary>
    /// Gets or sets when the device was created in AdGuard.
    /// </summary>
    public DateTime? CreatedInAdGuard { get; set; }

    /// <summary>
    /// Gets or sets when the device was last active.
    /// </summary>
    public DateTime? LastActiveAt { get; set; }

    /// <summary>
    /// Gets or sets whether the device is currently active.
    /// </summary>
    public bool IsActive { get; set; }

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
