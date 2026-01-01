namespace AdGuard.DataAccess.Entities;

/// <summary>
/// Represents user settings and preferences stored locally.
/// </summary>
public class UserSettingsEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for this settings entry.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the settings key.
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    /// Gets or sets the settings value.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Gets or sets the settings value type (e.g., String, Int, Bool, Json).
    /// </summary>
    public SettingsValueType ValueType { get; set; } = SettingsValueType.String;

    /// <summary>
    /// Gets or sets the category for grouping settings.
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Gets or sets a description of this setting.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets whether this is a sensitive setting (e.g., API keys).
    /// </summary>
    public bool IsSensitive { get; set; }

    /// <summary>
    /// Gets or sets whether this setting is encrypted.
    /// </summary>
    public bool IsEncrypted { get; set; }

    /// <summary>
    /// Gets or sets when this setting was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets when this setting was last modified.
    /// </summary>
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
}