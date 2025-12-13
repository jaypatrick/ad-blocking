namespace AdGuard.ConsoleUI.Services;

/// <summary>
/// Factory class for creating AdGuard DNS API client instances.
/// Implements <see cref="IApiClientFactory"/> for dependency inversion.
/// </summary>
/// <remarks>
/// This class manages the API configuration and provides factory methods for creating
/// various API client instances. It supports both configuration-based and manual API key setup.
/// </remarks>
public class ApiClientFactory : IApiClientFactory
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

    /// <inheritdoc />
    public bool IsConfigured => _apiConfiguration != null && !string.IsNullOrEmpty(_currentApiKey);

    /// <summary>
    /// Gets the currently configured API key.
    /// </summary>
    public string? CurrentApiKey => _currentApiKey;

    /// <inheritdoc />
    public string? MaskedApiKey => _currentApiKey != null
        ? $"{_currentApiKey[..Math.Min(4, _currentApiKey.Length)]}..."
        : null;

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
    public AccountApi CreateAccountApi()
    {
        _logger.LogDebug("Creating AccountApi instance");
        return new AccountApi(GetConfiguration());
    }

    /// <inheritdoc />
    public DevicesApi CreateDevicesApi()
    {
        _logger.LogDebug("Creating DevicesApi instance");
        return new DevicesApi(GetConfiguration());
    }

    /// <inheritdoc />
    public DNSServersApi CreateDnsServersApi()
    {
        _logger.LogDebug("Creating DNSServersApi instance");
        return new DNSServersApi(GetConfiguration());
    }

    /// <inheritdoc />
    public StatisticsApi CreateStatisticsApi()
    {
        _logger.LogDebug("Creating StatisticsApi instance");
        return new StatisticsApi(GetConfiguration());
    }

    /// <inheritdoc />
    public FilterListsApi CreateFilterListsApi()
    {
        _logger.LogDebug("Creating FilterListsApi instance");
        return new FilterListsApi(GetConfiguration());
    }

    /// <inheritdoc />
    public QueryLogApi CreateQueryLogApi()
    {
        _logger.LogDebug("Creating QueryLogApi instance");
        return new QueryLogApi(GetConfiguration());
    }

    /// <inheritdoc />
    public WebServicesApi CreateWebServicesApi()
    {
        _logger.LogDebug("Creating WebServicesApi instance");
        return new WebServicesApi(GetConfiguration());
    }

    /// <inheritdoc />
    public DedicatedIPAddressesApi CreateDedicatedIpAddressesApi()
    {
        _logger.LogDebug("Creating DedicatedIPAddressesApi instance");
        return new DedicatedIPAddressesApi(GetConfiguration());
    }

    /// <inheritdoc />
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
