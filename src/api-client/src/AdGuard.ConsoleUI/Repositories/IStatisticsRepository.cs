using AdGuard.ApiClient.Model;

namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Repository interface for statistics operations.
/// Abstracts data access from the UI layer.
/// </summary>
public interface IStatisticsRepository
{
    /// <summary>
    /// Gets time-based query statistics for the specified time range.
    /// </summary>
    /// <param name="fromMillis">Start time in milliseconds since Unix epoch.</param>
    /// <param name="toMillis">End time in milliseconds since Unix epoch.</param>
    Task<TimeQueriesStatsList> GetTimeQueriesStatsAsync(long fromMillis, long toMillis);
}
