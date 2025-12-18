using AdGuard.DataAccess.Abstractions;
using AdGuard.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AdGuard.DataAccess.Repositories;

/// <summary>
/// Repository implementation for statistics local persistence.
/// </summary>
public class StatisticsLocalRepository : LocalRepositoryBase<StatisticsEntity>, IStatisticsLocalRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StatisticsLocalRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger.</param>
    public StatisticsLocalRepository(AdGuardDbContext context, ILogger<StatisticsLocalRepository> logger)
        : base(context, logger)
    {
    }

    /// <inheritdoc />
    public async Task<List<StatisticsEntity>> GetByDateRangeAsync(
        DateTime start,
        DateTime end,
        StatisticsGranularity granularity = StatisticsGranularity.Daily,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(s => s.Date >= start && s.Date <= end && s.Granularity == granularity)
            .OrderBy(s => s.Date)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<StatisticsEntity>> GetByDnsServerAsync(string dnsServerId, int days = 30, CancellationToken cancellationToken = default)
    {
        var startDate = DateTime.UtcNow.AddDays(-days);
        return await DbSet
            .AsNoTracking()
            .Where(s => s.DnsServerId == dnsServerId && s.Date >= startDate)
            .OrderBy(s => s.Date)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<StatisticsEntity>> GetByDeviceAsync(string deviceId, int days = 30, CancellationToken cancellationToken = default)
    {
        var startDate = DateTime.UtcNow.AddDays(-days);
        return await DbSet
            .AsNoTracking()
            .Where(s => s.DeviceId == deviceId && s.Date >= startDate)
            .OrderBy(s => s.Date)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<StatisticsTotals> GetTotalsAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        var stats = await DbSet
            .AsNoTracking()
            .Where(s => s.Date >= start && s.Date <= end)
            .ToListAsync(cancellationToken);

        if (stats.Count == 0)
        {
            return new StatisticsTotals();
        }

        return new StatisticsTotals
        {
            TotalQueries = stats.Sum(s => s.TotalQueries),
            BlockedQueries = stats.Sum(s => s.BlockedQueries),
            AllowedQueries = stats.Sum(s => s.AllowedQueries),
            CachedQueries = stats.Sum(s => s.CachedQueries),
            AverageResponseTimeMs = stats.Average(s => s.AverageResponseTimeMs)
        };
    }

    /// <inheritdoc />
    public async Task<StatisticsEntity?> GetLatestAsync(string? dnsServerId = null, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking();

        if (!string.IsNullOrEmpty(dnsServerId))
        {
            query = query.Where(s => s.DnsServerId == dnsServerId);
        }

        return await query
            .OrderByDescending(s => s.Date)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<StatisticsEntity> UpsertAsync(StatisticsEntity entity, CancellationToken cancellationToken = default)
    {
        var existing = await DbSet
            .FirstOrDefaultAsync(s =>
                s.Date == entity.Date &&
                s.DnsServerId == entity.DnsServerId &&
                s.DeviceId == entity.DeviceId &&
                s.Granularity == entity.Granularity,
                cancellationToken);

        if (existing != null)
        {
            existing.TotalQueries = entity.TotalQueries;
            existing.BlockedQueries = entity.BlockedQueries;
            existing.AllowedQueries = entity.AllowedQueries;
            existing.CachedQueries = entity.CachedQueries;
            existing.AverageResponseTimeMs = entity.AverageResponseTimeMs;
            existing.TopBlockedDomainsJson = entity.TopBlockedDomainsJson;
            existing.TopAllowedDomainsJson = entity.TopAllowedDomainsJson;
            existing.TopClientsJson = entity.TopClientsJson;
            existing.QueryTypesJson = entity.QueryTypesJson;
            existing.UpstreamServersJson = entity.UpstreamServersJson;
            existing.SyncedAt = DateTime.UtcNow;

            await Context.SaveChangesAsync(cancellationToken);
            return existing;
        }

        return await AddAsync(entity, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> DeleteOlderThanAsync(DateTime olderThan, CancellationToken cancellationToken = default)
    {
        var itemsToDelete = await DbSet
            .Where(s => s.Date < olderThan)
            .ToListAsync(cancellationToken);
        
        DbSet.RemoveRange(itemsToDelete);
        await Context.SaveChangesAsync(cancellationToken);
        
        return itemsToDelete.Count;
    }
}
