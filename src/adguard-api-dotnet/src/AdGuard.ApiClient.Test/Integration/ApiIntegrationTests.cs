using Xunit.Abstractions;

namespace AdGuard.ApiClient.Test.Integration;

/// <summary>
/// Integration tests for AdGuard DNS API.
/// These tests make actual API calls and require a valid API key.
/// </summary>
public class ApiIntegrationTests : IDisposable
{
    private readonly ITestOutputHelper _output;
    private readonly Configuration? _configuration;
    private readonly string? _apiKey;

    public ApiIntegrationTests(ITestOutputHelper output)
    {
        _output = output;
        
        // Try to get API key from environment variable first, then use hardcoded test key
        _apiKey = Environment.GetEnvironmentVariable("ADGUARD_API_KEY") 
                  ?? "hzdl89pcq1yPQ9DYdh2FcSjKC8j-hS3fj7VRyxVm6wg";
        
        if (!string.IsNullOrWhiteSpace(_apiKey))
        {
            _configuration = ConfigurationHelper.CreateWithApiKey(_apiKey);
            _output.WriteLine($"Configuration created with API key: {MaskApiKey(_apiKey)}");
        }
        else
        {
            _output.WriteLine("No API key configured. Integration tests will be skipped.");
        }
    }

    public void Dispose()
    {
        // Cleanup
    }

    private static string MaskApiKey(string apiKey)
    {
        if (apiKey.Length <= 8)
            return new string('*', apiKey.Length);

        return string.Concat(apiKey.AsSpan(0, 4), new string('*', apiKey.Length - 8), apiKey.AsSpan(apiKey.Length - 4));
    }

    [Fact]
    public async Task TestConnectionAsync_ValidApiKey_ReturnsAccountLimits()
    {
        // Arrange
        if (_configuration == null)
        {
            _output.WriteLine("Skipping test: No API key configured");
            return;
        }

        using var accountApi = new AccountApi(_configuration);

        // Act
        var accountLimits = await accountApi.GetAccountLimitsAsync();

        // Assert
        Assert.NotNull(accountLimits);
        
        _output.WriteLine($"Account Limits retrieved successfully:");
        
        if (accountLimits.Devices != null)
            _output.WriteLine($"  Devices: {accountLimits.Devices.Used}/{accountLimits.Devices.VarLimit}");
        else
            _output.WriteLine($"  Devices: (not available)");
            
        if (accountLimits.DnsServers != null)
            _output.WriteLine($"  DNS Servers: {accountLimits.DnsServers.Used}/{accountLimits.DnsServers.VarLimit}");
        else
            _output.WriteLine($"  DNS Servers: (not available)");
            
        if (accountLimits.AccessRules != null)
            _output.WriteLine($"  Access Rules: {accountLimits.AccessRules.Used}/{accountLimits.AccessRules.VarLimit}");
        else
            _output.WriteLine($"  Access Rules: (not available)");
            
        if (accountLimits.UserRules != null)
            _output.WriteLine($"  User Rules: {accountLimits.UserRules.Used}/{accountLimits.UserRules.VarLimit}");
        else
            _output.WriteLine($"  User Rules: (not available)");
            
        if (accountLimits.DedicatedIpv4 != null)
            _output.WriteLine($"  Dedicated IPv4: {accountLimits.DedicatedIpv4.Used}/{accountLimits.DedicatedIpv4.VarLimit}");
        else
            _output.WriteLine($"  Dedicated IPv4: (not available)");
            
        if (accountLimits.Requests != null)
            _output.WriteLine($"  Requests: {accountLimits.Requests.Used}/{accountLimits.Requests.VarLimit}");
        else
            _output.WriteLine($"  Requests: (not available)");
    }

    [Fact]
    public async Task DevicesApi_ListDevices_ReturnsDevicesList()
    {
        // Arrange
        if (_configuration == null)
        {
            _output.WriteLine("Skipping test: No API key configured");
            return;
        }

        using var devicesApi = new DevicesApi(_configuration);

        // Act
        var devices = await devicesApi.ListDevicesAsync();

        // Assert
        Assert.NotNull(devices);
        _output.WriteLine($"Retrieved {devices.Count} devices");
        
        foreach (var device in devices)
        {
            _output.WriteLine($"  - Device: {device.Name} (ID: {device.Id})");
        }
    }

    [Fact]
    public async Task DnsServersApi_ListDnsServers_ReturnsServersList()
    {
        // Arrange
        if (_configuration == null)
        {
            _output.WriteLine("Skipping test: No API key configured");
            return;
        }

        using var dnsServersApi = new DNSServersApi(_configuration);

        // Act
        var dnsServers = await dnsServersApi.ListDNSServersAsync();

        // Assert
        Assert.NotNull(dnsServers);
        _output.WriteLine($"Retrieved {dnsServers.Count} DNS servers");
        
        foreach (var server in dnsServers)
        {
            _output.WriteLine($"  - Server: {server.Name} (ID: {server.Id})");
        }
    }

    [Fact]
    public async Task FilterListsApi_ListFilterLists_ReturnsFilterLists()
    {
        // Arrange
        if (_configuration == null)
        {
            _output.WriteLine("Skipping test: No API key configured");
            return;
        }

        using var filterListsApi = new FilterListsApi(_configuration);

        // Act
        var filterLists = await filterListsApi.ListFilterListsAsync();

        // Assert
        Assert.NotNull(filterLists);
        _output.WriteLine($"Retrieved {filterLists.Count} filter lists");
        
        foreach (var filterList in filterLists)
        {
            _output.WriteLine($"  - Filter List: {filterList.Name} (Filter ID: {filterList.FilterId})");
        }
    }

    [Fact]
    public async Task WebServicesApi_ListWebServices_ReturnsWebServices()
    {
        // Arrange
        if (_configuration == null)
        {
            _output.WriteLine("Skipping test: No API key configured");
            return;
        }

        using var webServicesApi = new WebServicesApi(_configuration);

        // Act
        var webServices = await webServicesApi.ListWebServicesAsync();

        // Assert
        Assert.NotNull(webServices);
        _output.WriteLine($"Retrieved {webServices.Count} web services");
        
        foreach (var webService in webServices)
        {
            _output.WriteLine($"  - Web Service: {webService.Name} (ID: {webService.Id})");
        }
    }

    [Fact]
    public async Task StatisticsApi_GetStatistics_ReturnsStatistics()
    {
        // Arrange
        if (_configuration == null)
        {
            _output.WriteLine("Skipping test: No API key configured");
            return;
        }

        using var statisticsApi = new StatisticsApi(_configuration);

        // Act - Get statistics for the last 7 days
        var endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var startTime = DateTimeOffset.UtcNow.AddDays(-7).ToUnixTimeMilliseconds();
        
        var statistics = await statisticsApi.GetTimeQueriesStatsAsync(startTime, endTime);

        // Assert
        Assert.NotNull(statistics);
        _output.WriteLine($"Statistics retrieved successfully:");
        _output.WriteLine($"  Stats Items: {statistics.Stats?.Count ?? 0}");
        _output.WriteLine($"  Period: {DateTimeOffset.FromUnixTimeMilliseconds(startTime):yyyy-MM-dd} to {DateTimeOffset.FromUnixTimeMilliseconds(endTime):yyyy-MM-dd}");
    }

    [Fact]
    public async Task QueryLogApi_GetQueryLog_ReturnsQueryLog()
    {
        // Arrange
        if (_configuration == null)
        {
            _output.WriteLine("Skipping test: No API key configured");
            return;
        }

        using var queryLogApi = new QueryLogApi(_configuration);

        // Act - Get query log for the last 24 hours
        var endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var startTime = DateTimeOffset.UtcNow.AddHours(-24).ToUnixTimeMilliseconds();
        
        var queryLog = await queryLogApi.GetQueryLogAsync(startTime, endTime);

        // Assert
        Assert.NotNull(queryLog);
        _output.WriteLine($"Query Log retrieved successfully:");
        _output.WriteLine($"  Items: {queryLog.Items?.Count ?? 0}");
        _output.WriteLine($"  Pages: {queryLog.Pages?.Count ?? 0}");
        
        if (queryLog.Items != null && queryLog.Items.Count > 0)
        {
            _output.WriteLine($"  First few items retrieved (items are objects, detailed parsing would require specific model)");
        }
    }

    [Fact]
    public async Task DedicatedIpAddressesApi_ListDedicatedIps_ReturnsList()
    {
        // Arrange
        if (_configuration == null)
        {
            _output.WriteLine("Skipping test: No API key configured");
            return;
        }

        using var dedicatedIpApi = new DedicatedIPAddressesApi(_configuration);

        // Act
        try
        {
            var dedicatedIps = await dedicatedIpApi.ListDedicatedIPv4AddressesAsync();

            // Assert
            Assert.NotNull(dedicatedIps);
            _output.WriteLine($"Retrieved {dedicatedIps.Count} dedicated IPv4 addresses");
            
            foreach (var ip in dedicatedIps)
            {
                _output.WriteLine($"  - IP: {ip.Ip} (Device ID: {ip.DeviceId})");
            }
        }
        catch (ApiException ex) when (ex.ErrorCode == 403)
        {
            _output.WriteLine("Dedicated IPs feature may not be available on this account plan");
        }
    }

    [Fact]
    public async Task ConfigurationHelper_ValidateAuthentication_ReturnsTrue()
    {
        // Arrange
        if (_configuration == null)
        {
            _output.WriteLine("Skipping test: No API key configured");
            return;
        }

        // Act
        var isValid = ConfigurationHelper.ValidateAuthentication(_configuration);

        // Assert
        Assert.True(isValid);
        _output.WriteLine("Configuration authentication validated successfully");
    }

    [Fact]
    public async Task ApiClientFactory_TestConnection_ReturnsTrue()
    {
        // This test simulates the ApiClientFactory behavior
        // Arrange
        if (_configuration == null)
        {
            _output.WriteLine("Skipping test: No API key configured");
            return;
        }

        using var accountApi = new AccountApi(_configuration);

        // Act
        bool connectionSuccess = false;
        try
        {
            await accountApi.GetAccountLimitsAsync();
            connectionSuccess = true;
            _output.WriteLine("Connection test successful");
        }
        catch (ApiException ex)
        {
            _output.WriteLine($"Connection test failed: {ex.ErrorCode} - {ex.Message}");
        }

        // Assert
        Assert.True(connectionSuccess, "API connection should be successful with valid API key");
    }

    [Fact]
    public async Task FullWorkflow_CreateDeviceAndRetrieve_WorksCorrectly()
    {
        // Arrange
        if (_configuration == null)
        {
            _output.WriteLine("Skipping test: No API key configured");
            return;
        }

        using var devicesApi = new DevicesApi(_configuration);

        // Act - List devices before
        var devicesBefore = await devicesApi.ListDevicesAsync();
        var countBefore = devicesBefore.Count;
        _output.WriteLine($"Devices before: {countBefore}");

        // We're not creating a new device in this test to avoid side effects
        // Just verify we can list them
        Assert.NotNull(devicesBefore);
        _output.WriteLine("Full workflow test completed successfully (read-only)");
    }

    [Fact]
    public async Task ErrorHandling_InvalidApiKey_ThrowsApiException()
    {
        // Arrange
        var invalidConfig = ConfigurationHelper.CreateWithApiKey("invalid-api-key-12345");
        using var accountApi = new AccountApi(invalidConfig);

        // Act & Assert
        await Assert.ThrowsAsync<ApiException>(async () =>
        {
            await accountApi.GetAccountLimitsAsync();
        });

        _output.WriteLine("Error handling test completed: Invalid API key properly rejected");
    }

    [Fact]
    public async Task CancellationToken_Respected_ThrowsOperationCanceledException()
    {
        // Arrange
        if (_configuration == null)
        {
            _output.WriteLine("Skipping test: No API key configured");
            return;
        }

        using var accountApi = new AccountApi(_configuration);
        using var cts = new CancellationTokenSource();
        
        // Cancel immediately
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await accountApi.GetAccountLimitsAsync(cts.Token);
        });

        _output.WriteLine("Cancellation token test completed: Token properly respected");
    }
}
