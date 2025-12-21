namespace AdGuard.ApiClient.Api;

/// <summary>
/// Represents a collection of functions to interact with the API endpoints
/// </summary>
public interface IDevicesApiSync : IApiAccessor
{
    #region Synchronous Operations
    /// <summary>
    /// Creates a new device
    /// </summary>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="deviceCreate"></param>
    /// <returns>Device</returns>
    Device CreateDevice(DeviceCreate deviceCreate);

    /// <summary>
    /// Creates a new device
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="deviceCreate"></param>
    /// <returns>ApiResponse of Device</returns>
    ApiResponse<Device> CreateDeviceWithHttpInfo(DeviceCreate deviceCreate);
    /// <summary>
    /// Gets an existing device by ID
    /// </summary>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="deviceId"></param>
    /// <returns>Device</returns>
    Device GetDevice(string deviceId);

    /// <summary>
    /// Gets an existing device by ID
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="deviceId"></param>
    /// <returns>ApiResponse of Device</returns>
    ApiResponse<Device> GetDeviceWithHttpInfo(string deviceId);
    /// <summary>
    /// Lists devices
    /// </summary>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <returns>List&lt;Device&gt;</returns>
    List<Device> ListDevices();

    /// <summary>
    /// Lists devices
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <returns>ApiResponse of List&lt;Device&gt;</returns>
    ApiResponse<List<Device>> ListDevicesWithHttpInfo();
    /// <summary>
    /// Removes a device
    /// </summary>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    void RemoveDevice(string deviceId);

    /// <summary>
    /// Removes a device
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="deviceId"></param>
    /// <returns>ApiResponse of Object(void)</returns>
    ApiResponse<Object> RemoveDeviceWithHttpInfo(string deviceId);
    /// <summary>
    /// Updates an existing device
    /// </summary>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="deviceId"></param>
    /// <param name="deviceUpdate"></param>
    /// <returns></returns>
    void UpdateDevice(string deviceId, DeviceUpdate deviceUpdate);

    /// <summary>
    /// Updates an existing device
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="deviceId"></param>
    /// <param name="deviceUpdate"></param>
    /// <returns>ApiResponse of Object(void)</returns>
    ApiResponse<Object> UpdateDeviceWithHttpInfo(string deviceId, DeviceUpdate deviceUpdate);
    #endregion Synchronous Operations
}