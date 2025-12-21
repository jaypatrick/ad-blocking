namespace AdGuard.ApiClient.Api;

/// <summary>
/// Represents a collection of functions to interact with the API endpoints
/// </summary>
public interface IDNSServersApiSync : IApiAccessor
{
    #region Synchronous Operations
    /// <summary>
    /// Creates a new DNS server
    /// </summary>
    /// <remarks>
    /// Creates a new DNS server. You can attach custom settings, otherwise DNS server will be created with default settings.
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="dNSServerCreate"></param>
    /// <returns>DNSServer</returns>
    DNSServer CreateDNSServer(DNSServerCreate dNSServerCreate);

    /// <summary>
    /// Creates a new DNS server
    /// </summary>
    /// <remarks>
    /// Creates a new DNS server. You can attach custom settings, otherwise DNS server will be created with default settings.
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="dNSServerCreate"></param>
    /// <returns>ApiResponse of DNSServer</returns>
    ApiResponse<DNSServer> CreateDNSServerWithHttpInfo(DNSServerCreate dNSServerCreate);
    /// <summary>
    /// Lists DNS servers that belong to the user.
    /// </summary>
    /// <remarks>
    /// Lists DNS servers that belong to the user. By default there is at least one default server.
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <returns>List&lt;DNSServer&gt;</returns>
    List<DNSServer> ListDNSServers();

    /// <summary>
    /// Lists DNS servers that belong to the user.
    /// </summary>
    /// <remarks>
    /// Lists DNS servers that belong to the user. By default there is at least one default server.
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <returns>ApiResponse of List&lt;DNSServer&gt;</returns>
    ApiResponse<List<DNSServer>> ListDNSServersWithHttpInfo();
    /// <summary>
    /// Updates DNS server settings
    /// </summary>
    /// <remarks>
    /// Updates the settings for a specific DNS server including user rules.
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="dnsServerId">The DNS server ID.</param>
    /// <param name="dnsServerSettingsUpdate">The settings update payload.</param>
    void UpdateDNSServerSettings(string dnsServerId, DNSServerSettingsUpdate dnsServerSettingsUpdate);

    /// <summary>
    /// Updates DNS server settings
    /// </summary>
    /// <remarks>
    /// Updates the settings for a specific DNS server including user rules.
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="dnsServerId">The DNS server ID.</param>
    /// <param name="dnsServerSettingsUpdate">The settings update payload.</param>
    /// <returns>ApiResponse of Object(void)</returns>
    ApiResponse<Object> UpdateDNSServerSettingsWithHttpInfo(string dnsServerId, DNSServerSettingsUpdate dnsServerSettingsUpdate);
    #endregion Synchronous Operations
}