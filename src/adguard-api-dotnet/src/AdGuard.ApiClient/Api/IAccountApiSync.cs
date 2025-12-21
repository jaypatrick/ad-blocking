namespace AdGuard.ApiClient.Api;

/// <summary>
/// Represents a collection of functions to interact with the API endpoints
/// </summary>
public interface IAccountApiSync : IApiAccessor
{
    #region Synchronous Operations
    /// <summary>
    /// Gets account limits
    /// </summary>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <returns>AccountLimits</returns>
    AccountLimits GetAccountLimits();

    /// <summary>
    /// Gets account limits
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <returns>ApiResponse of AccountLimits</returns>
    ApiResponse<AccountLimits> GetAccountLimitsWithHttpInfo();
    #endregion Synchronous Operations
}