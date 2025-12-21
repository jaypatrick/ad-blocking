namespace AdGuard.ApiClient.Api;

/// <summary>
/// Represents a collection of functions to interact with the API endpoints
/// </summary>
public interface IFilterListsApiAsync : IApiAccessor
{
    #region Asynchronous Operations
    /// <summary>
    /// Gets filter lists
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of List&lt;FilterList&gt;</returns>
    System.Threading.Tasks.Task<List<FilterList>> ListFilterListsAsync(System.Threading.CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets filter lists
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (List&lt;FilterList&gt;)</returns>
    System.Threading.Tasks.Task<ApiResponse<List<FilterList>>> ListFilterListsWithHttpInfoAsync(System.Threading.CancellationToken cancellationToken = default);
    #endregion Asynchronous Operations
}