using AdGuard.ApiClient.Helpers;
using AdGuard.Repositories.Abstractions;
using AdGuard.Repositories.Exceptions;
using Microsoft.Extensions.Configuration;

namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Factory implementation for creating API client instances.
/// </summary>
public partial class ApiClientFactory : IApiClientFactory
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApiClientFactory> _logger;
    private Configuration? _apiConfiguration;
    private string? _apiKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiClientFactory"/> class.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="logger">The logger.</param>
    public ApiClientFactory(IConfiguration configuration, ILogger<ApiClientFactory> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public bool IsConfigured => _apiConfiguration != null && !string.IsNullOrWhiteSpace(_apiKey);

    /// <inheritdoc />
    public string? MaskedApiKey
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
                return null;

            if (_apiKey.Length <= 8)
                return new string('*', _apiKey.Length);

            return string.Concat(_apiKey.AsSpan(0, 4), new string('*', _apiKey.Length - 8), _apiKey.AsSpan(_apiKey.Length - 4));
        }
    }

    /// <inheritdoc />
    public void Configure(string apiKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);

        LogConfiguringApiClient();
        _apiKey = apiKey;
        _apiConfiguration = ConfigurationHelper.CreateWithApiKey(apiKey);
        LogApiClientConfigured();
    }

    /// <inheritdoc />
    public void ConfigureFromSettings()
    {
        LogConfiguringFromSettings();

        var apiKey = _configuration["AdGuard:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            LogNoApiKeyInSettings();
            return;
        }

        Configure(apiKey);
    }

    /// <inheritdoc />
    public async Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
        {
            LogNotConfigured();
            return false;
        }

        LogTestingConnection();

        try
        {
            using var api = CreateAccountApi();
            await api.GetAccountLimitsAsync(cancellationToken).ConfigureAwait(false);
            LogConnectionSuccessful();
            return true;
        }
        catch (ApiException ex)
        {
            LogConnectionFailed(ex.ErrorCode, ex.Message, ex);
            return false;
        }
    }

    /// <inheritdoc />
    public AccountApi CreateAccountApi()
    {
        EnsureConfigured();
        return new AccountApi(_apiConfiguration!);
    }

    /// <inheritdoc />
    public DevicesApi CreateDevicesApi()
    {
        EnsureConfigured();
        return new DevicesApi(_apiConfiguration!);
    }

    /// <inheritdoc />
    public DNSServersApi CreateDnsServersApi()
    {
        EnsureConfigured();
        return new DNSServersApi(_apiConfiguration!);
    }

    /// <inheritdoc />
    public StatisticsApi CreateStatisticsApi()
    {
        EnsureConfigured();
        return new StatisticsApi(_apiConfiguration!);
    }

    /// <inheritdoc />
    public FilterListsApi CreateFilterListsApi()
    {
        EnsureConfigured();
        return new FilterListsApi(_apiConfiguration!);
    }

    /// <inheritdoc />
    public QueryLogApi CreateQueryLogApi()
    {
        EnsureConfigured();
        return new QueryLogApi(_apiConfiguration!);
    }

    /// <inheritdoc />
    public WebServicesApi CreateWebServicesApi()
    {
        EnsureConfigured();
        return new WebServicesApi(_apiConfiguration!);
    }

    /// <inheritdoc />
    public DedicatedIPAddressesApi CreateDedicatedIpAddressesApi()
    {
        EnsureConfigured();
        return new DedicatedIPAddressesApi(_apiConfiguration!);
    }

    private void EnsureConfigured()
    {
        if (!IsConfigured)
        {
            throw new ApiNotConfiguredException();
        }
    }
}
