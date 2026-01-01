namespace AdGuard.DataAccess.Entities;

/// <summary>
/// Represents an audit log entry tracking API operations.
/// </summary>
public class AuditLogEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for this audit log entry.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets when the operation occurred.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the type of operation performed.
    /// </summary>
    public AuditOperationType OperationType { get; set; }

    /// <summary>
    /// Gets or sets the type of entity that was affected.
    /// </summary>
    public required string EntityType { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the entity that was affected.
    /// </summary>
    public string? EntityId { get; set; }

    /// <summary>
    /// Gets or sets the name of the entity for display purposes.
    /// </summary>
    public string? EntityName { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who performed the operation.
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Gets or sets the source of the operation (e.g., ConsoleUI, API, Sync).
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// Gets or sets whether the operation succeeded.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the error code if the operation failed.
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Gets or sets the duration of the operation in milliseconds.
    /// </summary>
    public long? DurationMs { get; set; }

    /// <summary>
    /// Gets or sets the JSON-serialized request data.
    /// </summary>
    public string? RequestDataJson { get; set; }

    /// <summary>
    /// Gets or sets the JSON-serialized response data.
    /// </summary>
    public string? ResponseDataJson { get; set; }

    /// <summary>
    /// Gets or sets the JSON-serialized previous state (for updates).
    /// </summary>
    public string? PreviousStateJson { get; set; }

    /// <summary>
    /// Gets or sets the JSON-serialized new state (for creates/updates).
    /// </summary>
    public string? NewStateJson { get; set; }

    /// <summary>
    /// Gets or sets the IP address of the client that performed the operation.
    /// </summary>
    public string? ClientIpAddress { get; set; }

    /// <summary>
    /// Gets or sets the user agent of the client that performed the operation.
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Gets or sets additional context as JSON.
    /// </summary>
    public string? ContextJson { get; set; }
}