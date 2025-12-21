namespace AdGuard.ApiClient.Api;

/// <summary>
/// Represents a collection of functions to interact with the API endpoints
/// </summary>
public interface IDedicatedIPAddressesApiAsync : IApiAccessor
{
    #region Asynchronous Operations
    /// <summary>
    /// Allocates new dedicated IPv4
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of DedicatedIPv4Address</returns>
    System.Threading.Tasks.Task<DedicatedIPv4Address> AllocateDedicatedIPv4AddressAsync(System.Threading.CancellationToken cancellationToken = default);

    /// <summary>
    /// Allocates new dedicated IPv4
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (DedicatedIPv4Address)</returns>
    System.Threading.Tasks.Task<ApiResponse<DedicatedIPv4Address>> AllocateDedicatedIPv4AddressWithHttpInfoAsync(System.Threading.CancellationToken cancellationToken = default);
    /// <summary>
    /// Lists allocated dedicated IPv4 addresses
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of List&lt;DedicatedIPv4Address&gt;</returns>
    System.Threading.Tasks.Task<List<DedicatedIPv4Address>> ListDedicatedIPv4AddressesAsync(System.Threading.CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists allocated dedicated IPv4 addresses
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (List&lt;DedicatedIPv4Address&gt;)</returns>
    System.Threading.Tasks.Task<ApiResponse<List<DedicatedIPv4Address>>> ListDedicatedIPv4AddressesWithHttpInfoAsync(System.Threading.CancellationToken cancellationToken = default);
    #endregion Asynchronous Operations
}