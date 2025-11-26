using AdGuard.ApiClient.Api;
using AdGuard.ApiClient.Client;
using AdGuard.ApiClient.Helpers;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

namespace AdGuard.ConsoleUI.Services;

public class ApiClientFactory
{
    private readonly IConfiguration _configuration;
    private Configuration? _apiConfiguration;
    private string? _currentApiKey;

    public ApiClientFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool IsConfigured => _apiConfiguration != null && !string.IsNullOrEmpty(_currentApiKey);

    public string? CurrentApiKey => _currentApiKey;

    public void Configure(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentException("API key cannot be null or empty", nameof(apiKey));
        }

        _currentApiKey = apiKey;
        _apiConfiguration = ConfigurationHelper.CreateWithApiKey(apiKey);
    }

    public void ConfigureFromSettings()
    {
        var apiKey = _configuration["AdGuard:ApiKey"];
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            Configure(apiKey);
        }
    }

    private Configuration GetConfiguration()
    {
        if (_apiConfiguration == null)
        {
            throw new InvalidOperationException(
                "API client is not configured. Please configure your API key first.");
        }

        return _apiConfiguration;
    }

    public AccountApi CreateAccountApi() => new(GetConfiguration());

    public DevicesApi CreateDevicesApi() => new(GetConfiguration());

    public DNSServersApi CreateDnsServersApi() => new(GetConfiguration());

    public StatisticsApi CreateStatisticsApi() => new(GetConfiguration());

    public FilterListsApi CreateFilterListsApi() => new(GetConfiguration());

    public QueryLogApi CreateQueryLogApi() => new(GetConfiguration());

    public WebServicesApi CreateWebServicesApi() => new(GetConfiguration());

    public DedicatedIPAddressesApi CreateDedicatedIpAddressesApi() => new(GetConfiguration());

    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            using var api = CreateAccountApi();
            await api.GetAccountLimitsAsync();
            return true;
        }
        catch (ApiException ex) when (ex.ErrorCode == 401)
        {
            AnsiConsole.MarkupLine("[red]Authentication failed. Invalid API key.[/]");
            return false;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Connection test failed: {ex.Message}[/]");
            return false;
        }
    }
}
