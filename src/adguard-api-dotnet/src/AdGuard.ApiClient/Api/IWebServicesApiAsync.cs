namespace AdGuard.ApiClient.Api;

/// <summary>
/// Represents a collection of functions to interact with the API endpoints
/// </summary>
public interface IWebServicesApiAsync : IApiAccessor
{
    #region Asynchronous Operations
    /// <summary>
    /// Lists web services
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of List&lt;WebService&gt;</returns>
    System.Threading.Tasks.Task<List<WebService>> ListWebServicesAsync(System.Threading.CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists web services
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (List&lt;WebService&gt;)</returns>
    System.Threading.Tasks.Task<ApiResponse<List<WebService>>> ListWebServicesWithHttpInfoAsync(System.Threading.CancellationToken cancellationToken = default);
    #endregion Asynchronous Operations
}