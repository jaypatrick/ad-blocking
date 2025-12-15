namespace AdGuard.Repositories.Contracts;

/// <summary>
/// Repository interface for statistics operations.
/// </summary>
public interface IStatisticsRepository
{
    /// <summary>
    /// Gets time-based query statistics.
    /// </summary>
    /// <param name="startTime">Start time in Unix milliseconds.</param>
    /// <param name="endTime">End time in Unix milliseconds.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The query statistics.</returns>
    Task<TimeQueriesStatsList> GetTimeQueriesStatsAsync(long startTime, long endTime, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets statistics for the last 24 hours.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The query statistics.</returns>
    Task<TimeQueriesStatsList> GetLast24HoursStatsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets statistics for the last 7 days.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The query statistics.</returns>
    Task<TimeQueriesStatsList> GetLast7DaysStatsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets statistics for the last 30 days.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The query statistics.</returns>
    Task<TimeQueriesStatsList> GetLast30DaysStatsAsync(CancellationToken cancellationToken = default);
}
