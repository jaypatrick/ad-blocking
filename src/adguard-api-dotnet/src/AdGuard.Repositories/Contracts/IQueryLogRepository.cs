namespace AdGuard.Repositories.Contracts;

/// <summary>
/// Repository interface for query log operations.
/// </summary>
public interface IQueryLogRepository
{
    /// <summary>
    /// Gets query log entries for a time range.
    /// </summary>
    /// <param name="startTime">Start time in Unix milliseconds.</param>
    /// <param name="endTime">End time in Unix milliseconds.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The query log response.</returns>
    Task<QueryLogResponse> GetQueryLogAsync(long startTime, long endTime, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears the query log.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ClearAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets query log entries for the last 24 hours.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The query log response.</returns>
    Task<QueryLogResponse> GetLast24HoursAsync(CancellationToken cancellationToken = default);
}
