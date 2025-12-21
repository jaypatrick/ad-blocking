namespace AdGuard.ApiClient.Api;

/// <summary>
/// Represents a collection of functions to interact with the API endpoints
/// </summary>
public interface IQueryLogApiAsync : IApiAccessor
{
    #region Asynchronous Operations
    /// <summary>
    /// Clears query log
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of void</returns>
    System.Threading.Tasks.Task ClearQueryLogAsync(System.Threading.CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears query log
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse</returns>
    System.Threading.Tasks.Task<ApiResponse<Object>> ClearQueryLogWithHttpInfoAsync(System.Threading.CancellationToken cancellationToken = default);
    /// <summary>
    /// Gets query log
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="timeFromMillis">Time from in milliseconds (inclusive)</param>
    /// <param name="timeToMillis">Time to in milliseconds (inclusive)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of QueryLogResponse</returns>
    System.Threading.Tasks.Task<QueryLogResponse> GetQueryLogAsync(long timeFromMillis, long timeToMillis, System.Threading.CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets query log
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="timeFromMillis">Time from in milliseconds (inclusive)</param>
    /// <param name="timeToMillis">Time to in milliseconds (inclusive)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (QueryLogResponse)</returns>
    System.Threading.Tasks.Task<ApiResponse<QueryLogResponse>> GetQueryLogWithHttpInfoAsync(long timeFromMillis, long timeToMillis, System.Threading.CancellationToken cancellationToken = default);
    #endregion Asynchronous Operations
}