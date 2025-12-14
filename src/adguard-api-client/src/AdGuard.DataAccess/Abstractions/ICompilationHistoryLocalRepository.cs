using AdGuard.DataAccess.Entities;

namespace AdGuard.DataAccess.Abstractions;

/// <summary>
/// Repository interface for compilation history local persistence.
/// </summary>
public interface ICompilationHistoryLocalRepository : ILocalRepository<CompilationHistoryEntity>
{
    /// <summary>
    /// Gets compilation history for a specific time range.
    /// </summary>
    /// <param name="start">The start time.</param>
    /// <param name="end">The end time.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Compilation history within the specified time range.</returns>
    Task<List<CompilationHistoryEntity>> GetByTimeRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets compilation history for a specific filter list.
    /// </summary>
    /// <param name="filterListName">The filter list name.</param>
    /// <param name="limit">Maximum number of records to return.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Compilation history for the specified filter list.</returns>
    Task<List<CompilationHistoryEntity>> GetByFilterListAsync(string filterListName, int limit = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets failed compilations.
    /// </summary>
    /// <param name="limit">Maximum number of records to return.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Failed compilation records.</returns>
    Task<List<CompilationHistoryEntity>> GetFailedAsync(int limit = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets successful compilations.
    /// </summary>
    /// <param name="limit">Maximum number of records to return.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Successful compilation records.</returns>
    Task<List<CompilationHistoryEntity>> GetSuccessfulAsync(int limit = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the most recent compilation history.
    /// </summary>
    /// <param name="count">Number of records to return.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The most recent compilation history.</returns>
    Task<List<CompilationHistoryEntity>> GetRecentAsync(int count = 20, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the latest compilation for a specific configuration path.
    /// </summary>
    /// <param name="configurationPath">The configuration path.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The latest compilation for the path, or null if none exists.</returns>
    Task<CompilationHistoryEntity?> GetLatestByConfigPathAsync(string configurationPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets compilation by output hash to check for duplicates.
    /// </summary>
    /// <param name="outputHash">The output hash.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The compilation with the specified hash, or null if not found.</returns>
    Task<CompilationHistoryEntity?> GetByOutputHashAsync(string outputHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets compilation statistics summary.
    /// </summary>
    /// <param name="days">Number of days to include in statistics.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Compilation statistics summary.</returns>
    Task<CompilationStatistics> GetStatisticsAsync(int days = 30, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes compilation history older than the specified date.
    /// </summary>
    /// <param name="olderThan">Delete history older than this date.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of deleted records.</returns>
    Task<int> DeleteOlderThanAsync(DateTime olderThan, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents compilation statistics summary.
/// </summary>
public record CompilationStatistics
{
    /// <summary>
    /// Gets the total number of compilations.
    /// </summary>
    public int TotalCompilations { get; init; }

    /// <summary>
    /// Gets the number of successful compilations.
    /// </summary>
    public int SuccessfulCompilations { get; init; }

    /// <summary>
    /// Gets the number of failed compilations.
    /// </summary>
    public int FailedCompilations { get; init; }

    /// <summary>
    /// Gets the average compilation duration in milliseconds.
    /// </summary>
    public double AverageDurationMs { get; init; }

    /// <summary>
    /// Gets the total rules compiled across all successful compilations.
    /// </summary>
    public long TotalRulesCompiled { get; init; }

    /// <summary>
    /// Gets the average rules per compilation.
    /// </summary>
    public double AverageRulesPerCompilation { get; init; }

    /// <summary>
    /// Gets the success rate as a percentage.
    /// </summary>
    public double SuccessRate => TotalCompilations > 0 ? (double)SuccessfulCompilations / TotalCompilations * 100 : 0;
}
