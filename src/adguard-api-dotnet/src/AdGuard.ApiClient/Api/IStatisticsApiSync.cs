namespace AdGuard.ApiClient.Api;

/// <summary>
/// Represents a collection of functions to interact with the API endpoints
/// </summary>
public interface IStatisticsApiSync : IApiAccessor
{
    #region Synchronous Operations
    /// <summary>
    /// Gets time statistics
    /// </summary>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="timeFromMillis">Time from in milliseconds (inclusive)</param>
    /// <param name="timeToMillis">Time to in milliseconds (inclusive)</param>
    /// <returns>TimeQueriesStatsList</returns>
    TimeQueriesStatsList GetTimeQueriesStats(long timeFromMillis, long timeToMillis);

    /// <summary>
    /// Gets time statistics
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="timeFromMillis">Time from in milliseconds (inclusive)</param>
    /// <param name="timeToMillis">Time to in milliseconds (inclusive)</param>
    /// <returns>ApiResponse of TimeQueriesStatsList</returns>
    ApiResponse<TimeQueriesStatsList> GetTimeQueriesStatsWithHttpInfo(long timeFromMillis, long timeToMillis);
    #endregion Synchronous Operations
}