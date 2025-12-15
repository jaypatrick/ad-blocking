using AdGuard.DataAccess.Entities;

namespace AdGuard.DataAccess.Abstractions;

/// <summary>
/// Repository interface for statistics local persistence.
/// </summary>
public interface IStatisticsLocalRepository : ILocalRepository<StatisticsEntity>
{
    /// <summary>
    /// Gets statistics for a specific date range.
    /// </summary>
    /// <param name="start">The start date.</param>
    /// <param name="end">The end date.</param>
    /// <param name="granularity">The statistics granularity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Statistics within the specified date range.</returns>
    Task<List<StatisticsEntity>> GetByDateRangeAsync(
        DateTime start,
        DateTime end,
        StatisticsGranularity granularity = StatisticsGranularity.Daily,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets statistics for a specific DNS server.
    /// </summary>
    /// <param name="dnsServerId">The DNS server identifier.</param>
    /// <param name="days">Number of days to look back.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Statistics for the specified DNS server.</returns>
    Task<List<StatisticsEntity>> GetByDnsServerAsync(string dnsServerId, int days = 30, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets statistics for a specific device.
    /// </summary>
    /// <param name="deviceId">The device identifier.</param>
    /// <param name="days">Number of days to look back.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Statistics for the specified device.</returns>
    Task<List<StatisticsEntity>> GetByDeviceAsync(string deviceId, int days = 30, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets aggregate totals for a date range.
    /// </summary>
    /// <param name="start">The start date.</param>
    /// <param name="end">The end date.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Aggregate statistics totals.</returns>
    Task<StatisticsTotals> GetTotalsAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the latest statistics entry.
    /// </summary>
    /// <param name="dnsServerId">Optional DNS server filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The latest statistics entry.</returns>
    Task<StatisticsEntity?> GetLatestAsync(string? dnsServerId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Upserts a statistics entry (insert or update if exists).
    /// </summary>
    /// <param name="entity">The statistics entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The upserted entity.</returns>
    Task<StatisticsEntity> UpsertAsync(StatisticsEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes statistics older than the specified date.
    /// </summary>
    /// <param name="olderThan">Delete statistics older than this date.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of deleted records.</returns>
    Task<int> DeleteOlderThanAsync(DateTime olderThan, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents aggregate statistics totals.
/// </summary>
public record StatisticsTotals
{
    /// <summary>
    /// Gets the total number of queries.
    /// </summary>
    public long TotalQueries { get; init; }

    /// <summary>
    /// Gets the total number of blocked queries.
    /// </summary>
    public long BlockedQueries { get; init; }

    /// <summary>
    /// Gets the total number of allowed queries.
    /// </summary>
    public long AllowedQueries { get; init; }

    /// <summary>
    /// Gets the total number of cached queries.
    /// </summary>
    public long CachedQueries { get; init; }

    /// <summary>
    /// Gets the average response time in milliseconds.
    /// </summary>
    public double AverageResponseTimeMs { get; init; }

    /// <summary>
    /// Gets the block rate as a percentage.
    /// </summary>
    public double BlockRate => TotalQueries > 0 ? (double)BlockedQueries / TotalQueries * 100 : 0;
}
