namespace AdGuard.ApiClient.Api;

/// <summary>
/// Represents a collection of functions to interact with the API endpoints
/// </summary>
public interface IDedicatedIPAddressesApiSync : IApiAccessor
{
    #region Synchronous Operations
    /// <summary>
    /// Allocates new dedicated IPv4
    /// </summary>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <returns>DedicatedIPv4Address</returns>
    DedicatedIPv4Address AllocateDedicatedIPv4Address();

    /// <summary>
    /// Allocates new dedicated IPv4
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <returns>ApiResponse of DedicatedIPv4Address</returns>
    ApiResponse<DedicatedIPv4Address> AllocateDedicatedIPv4AddressWithHttpInfo();
    /// <summary>
    /// Lists allocated dedicated IPv4 addresses
    /// </summary>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <returns>List&lt;DedicatedIPv4Address&gt;</returns>
    List<DedicatedIPv4Address> ListDedicatedIPv4Addresses();

    /// <summary>
    /// Lists allocated dedicated IPv4 addresses
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <returns>ApiResponse of List&lt;DedicatedIPv4Address&gt;</returns>
    ApiResponse<List<DedicatedIPv4Address>> ListDedicatedIPv4AddressesWithHttpInfo();
    #endregion Synchronous Operations
}