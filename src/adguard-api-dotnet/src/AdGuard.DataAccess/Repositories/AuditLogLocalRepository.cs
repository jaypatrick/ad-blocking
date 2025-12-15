using AdGuard.DataAccess.Abstractions;
using AdGuard.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AdGuard.DataAccess.Repositories;

/// <summary>
/// Repository implementation for audit log local persistence.
/// </summary>
public class AuditLogLocalRepository : LocalRepositoryBase<AuditLogEntity>, IAuditLogLocalRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuditLogLocalRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger.</param>
    public AuditLogLocalRepository(AdGuardDbContext context, ILogger<AuditLogLocalRepository> logger)
        : base(context, logger)
    {
    }

    /// <inheritdoc />
    public async Task<List<AuditLogEntity>> GetByTimeRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(a => a.Timestamp >= start && a.Timestamp <= end)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<AuditLogEntity>> GetByEntityTypeAsync(string entityType, int limit = 100, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(a => a.EntityType == entityType)
            .OrderByDescending(a => a.Timestamp)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<AuditLogEntity>> GetByEntityAsync(string entityType, string entityId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(a => a.EntityType == entityType && a.EntityId == entityId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<AuditLogEntity>> GetByOperationTypeAsync(AuditOperationType operationType, int limit = 100, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(a => a.OperationType == operationType)
            .OrderByDescending(a => a.Timestamp)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<AuditLogEntity>> GetFailedAsync(int limit = 100, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(a => !a.Success)
            .OrderByDescending(a => a.Timestamp)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<AuditLogEntity>> GetRecentAsync(int count = 50, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .OrderByDescending(a => a.Timestamp)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AuditLogEntity> LogOperationAsync(
        AuditOperationType operationType,
        string entityType,
        string? entityId = null,
        bool success = true,
        string? errorMessage = null,
        long? durationMs = null,
        string? source = null,
        CancellationToken cancellationToken = default)
    {
        var entry = new AuditLogEntity
        {
            Timestamp = DateTime.UtcNow,
            OperationType = operationType,
            EntityType = entityType,
            EntityId = entityId,
            Success = success,
            ErrorMessage = errorMessage,
            DurationMs = durationMs,
            Source = source
        };

        return await AddAsync(entry, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> DeleteOlderThanAsync(DateTime olderThan, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(a => a.Timestamp < olderThan)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
