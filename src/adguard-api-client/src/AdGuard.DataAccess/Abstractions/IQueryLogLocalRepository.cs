using AdGuard.DataAccess.Entities;

namespace AdGuard.DataAccess.Abstractions;

/// <summary>
/// Repository interface for query log local persistence.
/// </summary>
public interface IQueryLogLocalRepository : ILocalRepository<QueryLogEntity>
{
    /// <summary>
    /// Gets query logs for a specific time range.
    /// </summary>
    /// <param name="start">The start time.</param>
    /// <param name="end">The end time.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Query logs within the specified time range.</returns>
    Task<List<QueryLogEntity>> GetByTimeRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets query logs for a specific device.
    /// </summary>
    /// <param name="deviceId">The device identifier.</param>
    /// <param name="limit">Maximum number of records to return.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Query logs for the specified device.</returns>
    Task<List<QueryLogEntity>> GetByDeviceAsync(string deviceId, int limit = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets blocked query logs.
    /// </summary>
    /// <param name="limit">Maximum number of records to return.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Blocked query logs.</returns>
    Task<List<QueryLogEntity>> GetBlockedAsync(int limit = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets query logs for a specific domain.
    /// </summary>
    /// <param name="domain">The domain to search for.</param>
    /// <param name="exactMatch">Whether to match exactly or use contains.</param>
    /// <param name="limit">Maximum number of records to return.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Query logs for the specified domain.</returns>
    Task<List<QueryLogEntity>> GetByDomainAsync(string domain, bool exactMatch = false, int limit = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the most recent query logs.
    /// </summary>
    /// <param name="count">Number of records to return.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The most recent query logs.</returns>
    Task<List<QueryLogEntity>> GetRecentAsync(int count = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets top blocked domains with their counts.
    /// </summary>
    /// <param name="count">Number of domains to return.</param>
    /// <param name="since">Start time for the aggregation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Dictionary of domain names and their block counts.</returns>
    Task<Dictionary<string, int>> GetTopBlockedDomainsAsync(int count = 10, DateTime? since = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets top queried domains with their counts.
    /// </summary>
    /// <param name="count">Number of domains to return.</param>
    /// <param name="since">Start time for the aggregation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Dictionary of domain names and their query counts.</returns>
    Task<Dictionary<string, int>> GetTopQueriedDomainsAsync(int count = 10, DateTime? since = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes query logs older than the specified date.
    /// </summary>
    /// <param name="olderThan">Delete logs older than this date.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of deleted records.</returns>
    Task<int> DeleteOlderThanAsync(DateTime olderThan, CancellationToken cancellationToken = default);
}
