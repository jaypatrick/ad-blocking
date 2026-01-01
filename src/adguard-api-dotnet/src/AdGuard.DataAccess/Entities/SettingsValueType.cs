namespace AdGuard.DataAccess.Entities;

/// <summary>
/// Represents the data type of a settings value.
/// </summary>
public enum SettingsValueType
{
    /// <summary>
    /// A string value.
    /// </summary>
    String = 0,

    /// <summary>
    /// An integer value.
    /// </summary>
    Integer = 1,

    /// <summary>
    /// A boolean value.
    /// </summary>
    Boolean = 2,

    /// <summary>
    /// A decimal/double value.
    /// </summary>
    Decimal = 3,

    /// <summary>
    /// A DateTime value.
    /// </summary>
    DateTime = 4,

    /// <summary>
    /// A JSON-serialized object.
    /// </summary>
    Json = 5,

    /// <summary>
    /// An encrypted string value.
    /// </summary>
    EncryptedString = 6
}