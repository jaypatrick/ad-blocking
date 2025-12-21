namespace AdGuard.ApiClient.Api;

/// <summary>
/// Represents a collection of functions to interact with the API endpoints
/// </summary>
public interface IFilterListsApiSync : IApiAccessor
{
    #region Synchronous Operations
    /// <summary>
    /// Gets filter lists
    /// </summary>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <returns>List&lt;FilterList&gt;</returns>
    List<FilterList> ListFilterLists();

    /// <summary>
    /// Gets filter lists
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <returns>ApiResponse of List&lt;FilterList&gt;</returns>
    ApiResponse<List<FilterList>> ListFilterListsWithHttpInfo();
    #endregion Synchronous Operations
}