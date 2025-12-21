namespace AdGuard.ApiClient.Api;

/// <summary>
/// Represents a collection of functions to interact with the API endpoints
/// </summary>
public interface IStatisticsApiAsync : IApiAccessor
{
    #region Asynchronous Operations
    /// <summary>
    /// Gets time statistics
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="timeFromMillis">Time from in milliseconds (inclusive)</param>
    /// <param name="timeToMillis">Time to in milliseconds (inclusive)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of TimeQueriesStatsList</returns>
    System.Threading.Tasks.Task<TimeQueriesStatsList> GetTimeQueriesStatsAsync(long timeFromMillis, long timeToMillis, System.Threading.CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets time statistics
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="timeFromMillis">Time from in milliseconds (inclusive)</param>
    /// <param name="timeToMillis">Time to in milliseconds (inclusive)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (TimeQueriesStatsList)</returns>
    System.Threading.Tasks.Task<ApiResponse<TimeQueriesStatsList>> GetTimeQueriesStatsWithHttpInfoAsync(long timeFromMillis, long timeToMillis, System.Threading.CancellationToken cancellationToken = default);
    #endregion Asynchronous Operations
}