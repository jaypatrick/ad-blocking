namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Repository interface for query log operations.
/// Abstracts data access from the UI layer.
/// </summary>
public interface IQueryLogRepository
{
    /// <summary>
    /// Gets query log entries for the specified time range.
    /// </summary>
    /// <param name="fromMillis">Start time in milliseconds since Unix epoch.</param>
    /// <param name="toMillis">End time in milliseconds since Unix epoch.</param>
    Task<QueryLogResponse> GetQueryLogAsync(long fromMillis, long toMillis);

    /// <summary>
    /// Clears all query log entries.
    /// </summary>
    Task ClearAsync();
}
