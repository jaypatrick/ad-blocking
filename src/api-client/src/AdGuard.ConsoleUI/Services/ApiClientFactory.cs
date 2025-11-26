using AdGuard.ApiClient.Api;
using AdGuard.ApiClient.Client;
using AdGuard.ApiClient.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace AdGuard.ConsoleUI.Services;

/// <summary>
/// Factory class for creating AdGuard DNS API client instances.
/// </summary>
/// <remarks>
/// This class manages the API configuration and provides factory methods for creating
/// various API client instances. It supports both configuration-based and manual API key setup.
/// </remarks>
public class ApiClientFactory
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApiClientFactory> _logger;
    private Configuration? _apiConfiguration;
    private string? _currentApiKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiClientFactory"/> class.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="logger">The logger instance.</param>
    /// <exception cref="ArgumentNullException">Thrown when configuration is null.</exception>
    public ApiClientFactory(IConfiguration configuration, ILogger<ApiClientFactory> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogDebug("ApiClientFactory initialized");
    }

    /// <summary>
    /// Gets a value indicating whether the API client is configured with a valid API key.
    /// </summary>
    public bool IsConfigured => _apiConfiguration != null && !string.IsNullOrEmpty(_currentApiKey);

    /// <summary>
    /// Gets the currently configured API key (masked for security).
    /// </summary>
    public string? CurrentApiKey => _currentApiKey != null
        ? $"{_currentApiKey[..Math.Min(4, _currentApiKey.Length)]}..."
        : null;

    /// <summary>
    /// Configures the factory with the specified API key.
    /// </summary>
    /// <param name="apiKey">The AdGuard DNS API key.</param>
    /// <exception cref="ArgumentException">Thrown when the API key is null or empty.</exception>
    public void Configure(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogError("Attempted to configure with null or empty API key");
            throw new ArgumentException("API key cannot be null or empty", nameof(apiKey));
        }

        _logger.LogInformation("Configuring API client with new API key");
        _currentApiKey = apiKey;
        _apiConfiguration = ConfigurationHelper.CreateWithApiKey(apiKey);
        _logger.LogDebug("API configuration created successfully");
    }

    /// <summary>
    /// Attempts to configure the factory from application settings.
    /// </summary>
    /// <remarks>
    /// Reads the API key from the "AdGuard:ApiKey" configuration section.
    /// If no API key is found, the factory remains unconfigured.
    /// </remarks>
    public void ConfigureFromSettings()
    {
        _logger.LogDebug("Attempting to configure from settings");
        var apiKey = _configuration["AdGuard:ApiKey"];
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogInformation("Found API key in configuration settings");
            Configure(apiKey);
        }
        else
        {
            _logger.LogDebug("No API key found in configuration settings");
        }
    }

    /// <summary>
    /// Gets the current API configuration.
    /// </summary>
    /// <returns>The current API configuration.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the API is not configured.</exception>
    private Configuration GetConfiguration()
    {
        if (_apiConfiguration == null)
        {
            _logger.LogError("Attempted to get configuration before API was configured");
            throw new InvalidOperationException(
                "API client is not configured. Please configure your API key first.");
        }

        return _apiConfiguration;
    }

    /// <summary>
    /// Creates a new Account API client instance.
    /// </summary>
    /// <returns>A new <see cref="AccountApi"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the API is not configured.</exception>
    public AccountApi CreateAccountApi()
    {
        _logger.LogDebug("Creating AccountApi instance");
        return new AccountApi(GetConfiguration());
    }

    /// <summary>
    /// Creates a new Devices API client instance.
    /// </summary>
    /// <returns>A new <see cref="DevicesApi"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the API is not configured.</exception>
    public DevicesApi CreateDevicesApi()
    {
        _logger.LogDebug("Creating DevicesApi instance");
        return new DevicesApi(GetConfiguration());
    }

    /// <summary>
    /// Creates a new DNS Servers API client instance.
    /// </summary>
    /// <returns>A new <see cref="DNSServersApi"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the API is not configured.</exception>
    public DNSServersApi CreateDnsServersApi()
    {
        _logger.LogDebug("Creating DNSServersApi instance");
        return new DNSServersApi(GetConfiguration());
    }

    /// <summary>
    /// Creates a new Statistics API client instance.
    /// </summary>
    /// <returns>A new <see cref="StatisticsApi"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the API is not configured.</exception>
    public StatisticsApi CreateStatisticsApi()
    {
        _logger.LogDebug("Creating StatisticsApi instance");
        return new StatisticsApi(GetConfiguration());
    }

    /// <summary>
    /// Creates a new Filter Lists API client instance.
    /// </summary>
    /// <returns>A new <see cref="FilterListsApi"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the API is not configured.</exception>
    public FilterListsApi CreateFilterListsApi()
    {
        _logger.LogDebug("Creating FilterListsApi instance");
        return new FilterListsApi(GetConfiguration());
    }

    /// <summary>
    /// Creates a new Query Log API client instance.
    /// </summary>
    /// <returns>A new <see cref="QueryLogApi"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the API is not configured.</exception>
    public QueryLogApi CreateQueryLogApi()
    {
        _logger.LogDebug("Creating QueryLogApi instance");
        return new QueryLogApi(GetConfiguration());
    }

    /// <summary>
    /// Creates a new Web Services API client instance.
    /// </summary>
    /// <returns>A new <see cref="WebServicesApi"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the API is not configured.</exception>
    public WebServicesApi CreateWebServicesApi()
    {
        _logger.LogDebug("Creating WebServicesApi instance");
        return new WebServicesApi(GetConfiguration());
    }

    /// <summary>
    /// Creates a new Dedicated IP Addresses API client instance.
    /// </summary>
    /// <returns>A new <see cref="DedicatedIPAddressesApi"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the API is not configured.</exception>
    public DedicatedIPAddressesApi CreateDedicatedIpAddressesApi()
    {
        _logger.LogDebug("Creating DedicatedIPAddressesApi instance");
        return new DedicatedIPAddressesApi(GetConfiguration());
    }

    /// <summary>
    /// Tests the API connection using the currently configured API key.
    /// </summary>
    /// <returns>True if the connection is successful; otherwise, false.</returns>
    /// <remarks>
    /// This method attempts to fetch account limits as a connection test.
    /// It catches and logs authentication errors and other exceptions.
    /// </remarks>
    public async Task<bool> TestConnectionAsync()
    {
        _logger.LogInformation("Testing API connection");

        try
        {
            using var api = CreateAccountApi();
            await api.GetAccountLimitsAsync();
            _logger.LogInformation("API connection test successful");
            return true;
        }
        catch (ApiException ex) when (ex.ErrorCode == 401)
        {
            _logger.LogWarning("API connection test failed: Authentication error (401)");
            AnsiConsole.MarkupLine("[red]Authentication failed. Invalid API key.[/]");
            return false;
        }
        catch (ApiException ex)
        {
            _logger.LogWarning(ex, "API connection test failed with API error: {ErrorCode} - {Message}",
                ex.ErrorCode, ex.Message);
            AnsiConsole.MarkupLine($"[red]Connection test failed: {Markup.Escape(ex.Message)}[/]");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API connection test failed with unexpected error");
            AnsiConsole.MarkupLine($"[red]Connection test failed: {Markup.Escape(ex.Message)}[/]");
            return false;
        }
    }
}
