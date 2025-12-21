namespace AdGuard.ApiClient.Api;

/// <summary>
/// Represents a collection of functions to interact with the API endpoints
/// </summary>
public interface IWebServicesApiSync : IApiAccessor
{
    #region Synchronous Operations
    /// <summary>
    /// Lists web services
    /// </summary>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <returns>List&lt;WebService&gt;</returns>
    List<WebService> ListWebServices();

    /// <summary>
    /// Lists web services
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <returns>ApiResponse of List&lt;WebService&gt;</returns>
    ApiResponse<List<WebService>> ListWebServicesWithHttpInfo();
    #endregion Synchronous Operations
}