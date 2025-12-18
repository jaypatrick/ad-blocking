using AdGuard.DataAccess.Entities;

namespace AdGuard.DataAccess.Abstractions;

/// <summary>
/// Repository interface for audit log local persistence.
/// </summary>
public interface IAuditLogLocalRepository : ILocalRepository<AuditLogEntity>
{
    /// <summary>
    /// Gets audit logs for a specific time range.
    /// </summary>
    /// <param name="start">The start time.</param>
    /// <param name="end">The end time.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Audit logs within the specified time range.</returns>
    Task<List<AuditLogEntity>> GetByTimeRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audit logs for a specific entity type.
    /// </summary>
    /// <param name="entityType">The entity type (e.g., Device, DnsServer).</param>
    /// <param name="limit">Maximum number of records to return.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Audit logs for the specified entity type.</returns>
    Task<List<AuditLogEntity>> GetByEntityTypeAsync(string entityType, int limit = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audit logs for a specific entity.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <param name="entityId">The entity identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Audit logs for the specified entity.</returns>
    Task<List<AuditLogEntity>> GetByEntityAsync(string entityType, string entityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audit logs by operation type.
    /// </summary>
    /// <param name="operationType">The operation type.</param>
    /// <param name="limit">Maximum number of records to return.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Audit logs for the specified operation type.</returns>
    Task<List<AuditLogEntity>> GetByOperationTypeAsync(AuditOperationType operationType, int limit = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets failed operation logs.
    /// </summary>
    /// <param name="limit">Maximum number of records to return.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Failed operation audit logs.</returns>
    Task<List<AuditLogEntity>> GetFailedAsync(int limit = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the most recent audit logs.
    /// </summary>
    /// <param name="count">Number of records to return.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The most recent audit logs.</returns>
    Task<List<AuditLogEntity>> GetRecentAsync(int count = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs an operation.
    /// </summary>
    /// <param name="operationType">The operation type.</param>
    /// <param name="entityType">The entity type.</param>
    /// <param name="entityId">The entity identifier.</param>
    /// <param name="success">Whether the operation succeeded.</param>
    /// <param name="errorMessage">The error message if failed.</param>
    /// <param name="durationMs">The operation duration in milliseconds.</param>
    /// <param name="source">The operation source.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created audit log entry.</returns>
    Task<AuditLogEntity> LogOperationAsync(
        AuditOperationType operationType,
        string entityType,
        string? entityId = null,
        bool success = true,
        string? errorMessage = null,
        long? durationMs = null,
        string? source = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes audit logs older than the specified date.
    /// </summary>
    /// <param name="olderThan">Delete logs older than this date.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of deleted records.</returns>
    Task<int> DeleteOlderThanAsync(DateTime olderThan, CancellationToken cancellationToken = default);
}
