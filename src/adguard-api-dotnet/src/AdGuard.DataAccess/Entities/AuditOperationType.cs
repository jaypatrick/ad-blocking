namespace AdGuard.DataAccess.Entities;

/// <summary>
/// Represents the type of audit operation.
/// </summary>
public enum AuditOperationType
{
    /// <summary>
    /// An entity was created.
    /// </summary>
    Create = 0,

    /// <summary>
    /// An entity was read/retrieved.
    /// </summary>
    Read = 1,

    /// <summary>
    /// An entity was updated.
    /// </summary>
    Update = 2,

    /// <summary>
    /// An entity was deleted.
    /// </summary>
    Delete = 3,

    /// <summary>
    /// A list of entities was retrieved.
    /// </summary>
    List = 4,

    /// <summary>
    /// Configuration was changed.
    /// </summary>
    Configure = 5,

    /// <summary>
    /// An authentication operation was performed.
    /// </summary>
    Authenticate = 6,

    /// <summary>
    /// A sync operation was performed.
    /// </summary>
    Sync = 7,

    /// <summary>
    /// An export operation was performed.
    /// </summary>
    Export = 8,

    /// <summary>
    /// An import operation was performed.
    /// </summary>
    Import = 9
}