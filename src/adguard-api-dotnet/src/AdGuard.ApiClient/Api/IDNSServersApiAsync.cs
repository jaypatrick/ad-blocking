namespace AdGuard.ApiClient.Api;

/// <summary>
/// Represents a collection of functions to interact with the API endpoints
/// </summary>
public interface IDNSServersApiAsync : IApiAccessor
{
    #region Asynchronous Operations
    /// <summary>
    /// Creates a new DNS server
    /// </summary>
    /// <remarks>
    /// Creates a new DNS server. You can attach custom settings, otherwise DNS server will be created with default settings.
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="dNSServerCreate"></param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of DNSServer</returns>
    System.Threading.Tasks.Task<DNSServer> CreateDNSServerAsync(DNSServerCreate dNSServerCreate, System.Threading.CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new DNS server
    /// </summary>
    /// <remarks>
    /// Creates a new DNS server. You can attach custom settings, otherwise DNS server will be created with default settings.
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="dNSServerCreate"></param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (DNSServer)</returns>
    System.Threading.Tasks.Task<ApiResponse<DNSServer>> CreateDNSServerWithHttpInfoAsync(DNSServerCreate dNSServerCreate, System.Threading.CancellationToken cancellationToken = default);
    /// <summary>
    /// Lists DNS servers that belong to the user.
    /// </summary>
    /// <remarks>
    /// Lists DNS servers that belong to the user. By default there is at least one default server.
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of List&lt;DNSServer&gt;</returns>
    System.Threading.Tasks.Task<List<DNSServer>> ListDNSServersAsync(System.Threading.CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists DNS servers that belong to the user.
    /// </summary>
    /// <remarks>
    /// Lists DNS servers that belong to the user. By default there is at least one default server.
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (List&lt;DNSServer&gt;)</returns>
    System.Threading.Tasks.Task<ApiResponse<List<DNSServer>>> ListDNSServersWithHttpInfoAsync(System.Threading.CancellationToken cancellationToken = default);
    /// <summary>
    /// Updates DNS server settings
    /// </summary>
    /// <remarks>
    /// Updates the settings for a specific DNS server including user rules.
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="dnsServerId">The DNS server ID.</param>
    /// <param name="dnsServerSettingsUpdate">The settings update payload.</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of void</returns>
    System.Threading.Tasks.Task UpdateDNSServerSettingsAsync(string dnsServerId, DNSServerSettingsUpdate dnsServerSettingsUpdate, System.Threading.CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates DNS server settings
    /// </summary>
    /// <remarks>
    /// Updates the settings for a specific DNS server including user rules.
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="dnsServerId">The DNS server ID.</param>
    /// <param name="dnsServerSettingsUpdate">The settings update payload.</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse</returns>
    System.Threading.Tasks.Task<ApiResponse<Object>> UpdateDNSServerSettingsWithHttpInfoAsync(string dnsServerId, DNSServerSettingsUpdate dnsServerSettingsUpdate, System.Threading.CancellationToken cancellationToken = default);
    #endregion Asynchronous Operations
}