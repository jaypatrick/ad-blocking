namespace AdGuard.ApiClient.Api;

/// <summary>
/// Represents a collection of functions to interact with the API endpoints
/// </summary>
public interface IQueryLogApiSync : IApiAccessor
{
    #region Synchronous Operations
    /// <summary>
    /// Clears query log
    /// </summary>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <returns></returns>
    void ClearQueryLog();

    /// <summary>
    /// Clears query log
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <returns>ApiResponse of Object(void)</returns>
    ApiResponse<Object> ClearQueryLogWithHttpInfo();
    /// <summary>
    /// Gets query log
    /// </summary>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="timeFromMillis">Time from in milliseconds (inclusive)</param>
    /// <param name="timeToMillis">Time to in milliseconds (inclusive)</param>
    /// <returns>QueryLogResponse</returns>
    QueryLogResponse GetQueryLog(long timeFromMillis, long timeToMillis);

    /// <summary>
    /// Gets query log
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="timeFromMillis">Time from in milliseconds (inclusive)</param>
    /// <param name="timeToMillis">Time to in milliseconds (inclusive)</param>
    /// <returns>ApiResponse of QueryLogResponse</returns>
    ApiResponse<QueryLogResponse> GetQueryLogWithHttpInfo(long timeFromMillis, long timeToMillis);
    #endregion Synchronous Operations
}