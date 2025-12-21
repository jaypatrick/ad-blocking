namespace AdGuard.ApiClient.Api;

/// <summary>
/// Represents a collection of functions to interact with the API endpoints
/// </summary>
public interface IDevicesApiAsync : IApiAccessor
{
    #region Asynchronous Operations
    /// <summary>
    /// Creates a new device
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="deviceCreate"></param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of Device</returns>
    System.Threading.Tasks.Task<Device> CreateDeviceAsync(DeviceCreate deviceCreate, System.Threading.CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new device
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="deviceCreate"></param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (Device)</returns>
    System.Threading.Tasks.Task<ApiResponse<Device>> CreateDeviceWithHttpInfoAsync(DeviceCreate deviceCreate, System.Threading.CancellationToken cancellationToken = default);
    /// <summary>
    /// Gets an existing device by ID
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="deviceId"></param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of Device</returns>
    System.Threading.Tasks.Task<Device> GetDeviceAsync(string deviceId, System.Threading.CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an existing device by ID
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="deviceId"></param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (Device)</returns>
    System.Threading.Tasks.Task<ApiResponse<Device>> GetDeviceWithHttpInfoAsync(string deviceId, System.Threading.CancellationToken cancellationToken = default);
    /// <summary>
    /// Lists devices
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of List&lt;Device&gt;</returns>
    System.Threading.Tasks.Task<List<Device>> ListDevicesAsync(System.Threading.CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists devices
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (List&lt;Device&gt;)</returns>
    System.Threading.Tasks.Task<ApiResponse<List<Device>>> ListDevicesWithHttpInfoAsync(System.Threading.CancellationToken cancellationToken = default);
    /// <summary>
    /// Removes a device
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="deviceId"></param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of void</returns>
    System.Threading.Tasks.Task RemoveDeviceAsync(string deviceId, System.Threading.CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a device
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="deviceId"></param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse</returns>
    System.Threading.Tasks.Task<ApiResponse<Object>> RemoveDeviceWithHttpInfoAsync(string deviceId, System.Threading.CancellationToken cancellationToken = default);
    /// <summary>
    /// Updates an existing device
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="deviceId"></param>
    /// <param name="deviceUpdate"></param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of void</returns>
    System.Threading.Tasks.Task UpdateDeviceAsync(string deviceId, DeviceUpdate deviceUpdate, System.Threading.CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing device
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="deviceId"></param>
    /// <param name="deviceUpdate"></param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse</returns>
    System.Threading.Tasks.Task<ApiResponse<Object>> UpdateDeviceWithHttpInfoAsync(string deviceId, DeviceUpdate deviceUpdate, System.Threading.CancellationToken cancellationToken = default);
    #endregion Asynchronous Operations
}