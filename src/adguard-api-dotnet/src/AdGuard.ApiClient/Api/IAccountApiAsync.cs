namespace AdGuard.ApiClient.Api;

/// <summary>
/// Represents a collection of functions to interact with the API endpoints
/// </summary>
public interface IAccountApiAsync : IApiAccessor
{
    #region Asynchronous Operations
    /// <summary>
    /// Gets account limits
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of AccountLimits</returns>
    System.Threading.Tasks.Task<AccountLimits> GetAccountLimitsAsync(System.Threading.CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets account limits
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (AccountLimits)</returns>
    System.Threading.Tasks.Task<ApiResponse<AccountLimits>> GetAccountLimitsWithHttpInfoAsync(System.Threading.CancellationToken cancellationToken = default);
    #endregion Asynchronous Operations
}