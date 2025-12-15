using AdGuard.DataAccess.Abstractions;
using AdGuard.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AdGuard.DataAccess.Repositories;

/// <summary>
/// Repository implementation for query log local persistence.
/// </summary>
public class QueryLogLocalRepository : LocalRepositoryBase<QueryLogEntity>, IQueryLogLocalRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QueryLogLocalRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger.</param>
    public QueryLogLocalRepository(AdGuardDbContext context, ILogger<QueryLogLocalRepository> logger)
        : base(context, logger)
    {
    }

    /// <inheritdoc />
    public async Task<List<QueryLogEntity>> GetByTimeRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(q => q.Timestamp >= start && q.Timestamp <= end)
            .OrderByDescending(q => q.Timestamp)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<QueryLogEntity>> GetByDeviceAsync(string deviceId, int limit = 100, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(q => q.DeviceId == deviceId)
            .OrderByDescending(q => q.Timestamp)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<QueryLogEntity>> GetBlockedAsync(int limit = 100, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(q => q.IsBlocked)
            .OrderByDescending(q => q.Timestamp)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<QueryLogEntity>> GetByDomainAsync(string domain, bool exactMatch = false, int limit = 100, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking();

        if (exactMatch)
        {
            query = query.Where(q => q.Domain == domain);
        }
        else
        {
            query = query.Where(q => q.Domain.Contains(domain));
        }

        return await query
            .OrderByDescending(q => q.Timestamp)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<QueryLogEntity>> GetRecentAsync(int count = 50, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .OrderByDescending(q => q.Timestamp)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Dictionary<string, int>> GetTopBlockedDomainsAsync(int count = 10, DateTime? since = null, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Where(q => q.IsBlocked);

        if (since.HasValue)
        {
            query = query.Where(q => q.Timestamp >= since.Value);
        }

        var result = await query
            .GroupBy(q => q.Domain)
            .Select(g => new { Domain = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(count)
            .ToListAsync(cancellationToken);

        return result.ToDictionary(x => x.Domain, x => x.Count);
    }

    /// <inheritdoc />
    public async Task<Dictionary<string, int>> GetTopQueriedDomainsAsync(int count = 10, DateTime? since = null, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking();

        if (since.HasValue)
        {
            query = query.Where(q => q.Timestamp >= since.Value);
        }

        var result = await query
            .GroupBy(q => q.Domain)
            .Select(g => new { Domain = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(count)
            .ToListAsync(cancellationToken);

        return result.ToDictionary(x => x.Domain, x => x.Count);
    }

    /// <inheritdoc />
    public async Task<int> DeleteOlderThanAsync(DateTime olderThan, CancellationToken cancellationToken = default)
    {
        var itemsToDelete = await DbSet
            .Where(q => q.Timestamp < olderThan)
            .ToListAsync(cancellationToken);
        
        DbSet.RemoveRange(itemsToDelete);
        await Context.SaveChangesAsync(cancellationToken);
        
        return itemsToDelete.Count;
    }
}
