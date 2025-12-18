namespace AdGuard.Repositories.Abstractions;

/// <summary>
/// Factory interface for creating API client instances.
/// Enables dependency inversion and testability.
/// </summary>
public interface IApiClientFactory
{
    /// <summary>
    /// Gets a value indicating whether the API client is configured with valid credentials.
    /// </summary>
    bool IsConfigured { get; }

    /// <summary>
    /// Gets the masked version of the currently configured API key (for display).
    /// </summary>
    string? MaskedApiKey { get; }

    /// <summary>
    /// Configures the factory with the specified API key.
    /// </summary>
    /// <param name="apiKey">The AdGuard DNS API key.</param>
    void Configure(string apiKey);

    /// <summary>
    /// Attempts to configure the factory from application settings.
    /// </summary>
    void ConfigureFromSettings();

    /// <summary>
    /// Tests the API connection using the currently configured credentials.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the connection is successful; otherwise, false.</returns>
    Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new Account API client instance.
    /// </summary>
    AccountApi CreateAccountApi();

    /// <summary>
    /// Creates a new Devices API client instance.
    /// </summary>
    DevicesApi CreateDevicesApi();

    /// <summary>
    /// Creates a new DNS Servers API client instance.
    /// </summary>
    DNSServersApi CreateDnsServersApi();

    /// <summary>
    /// Creates a new Statistics API client instance.
    /// </summary>
    StatisticsApi CreateStatisticsApi();

    /// <summary>
    /// Creates a new Filter Lists API client instance.
    /// </summary>
    FilterListsApi CreateFilterListsApi();

    /// <summary>
    /// Creates a new Query Log API client instance.
    /// </summary>
    QueryLogApi CreateQueryLogApi();

    /// <summary>
    /// Creates a new Web Services API client instance.
    /// </summary>
    WebServicesApi CreateWebServicesApi();

    /// <summary>
    /// Creates a new Dedicated IP Addresses API client instance.
    /// </summary>
    DedicatedIPAddressesApi CreateDedicatedIpAddressesApi();
}
