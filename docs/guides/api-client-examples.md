# AdGuard DNS API Client - Code Examples

This document provides practical examples using the refactored helper classes.

## Configuration Examples

### Using ConfigurationHelper

```csharp
using AdGuard.ApiClient.Helpers;
using AdGuard.ApiClient.Api;

// Simple API Key configuration
var config = ConfigurationHelper.CreateWithApiKey("your-api-key-here");

// With custom base path
var config = ConfigurationHelper.CreateWithApiKey(
    "your-api-key-here",
    "https://custom-api.adguard-dns.io"
);

// Bearer token configuration
var config = ConfigurationHelper.CreateWithBearerToken("your-access-token");

// Fluent configuration with method chaining
var config = ConfigurationHelper
    .CreateCustom()
    .WithApiKey("your-api-key-here")
    .WithTimeout(30000) // 30 seconds
    .WithUserAgent("MyApp/1.0");

// Use the configuration
var devicesApi = new DevicesApi(config);
```

## DateTime Helper Examples

### Working with Unix Milliseconds

```csharp
using AdGuard.ApiClient.Helpers;
using AdGuard.ApiClient.Api;

var queryLogApi = new QueryLogApi(config);

// Get query log for the last 24 hours
var timeFrom = DateTimeExtensions.DaysAgo(1);
var timeTo = DateTimeExtensions.Now();

var queryLog = await queryLogApi.GetQueryLogAsync(timeFrom, timeTo);

// Get query log for the last week
var weekAgo = DateTimeExtensions.DaysAgo(7);
var now = DateTimeExtensions.Now();
var weeklyLog = await queryLogApi.GetQueryLogAsync(weekAgo, now);

// Get query log for today only
var startOfDay = DateTimeExtensions.StartOfToday();
var endOfDay = DateTimeExtensions.EndOfToday();
var todayLog = await queryLogApi.GetQueryLogAsync(startOfDay, endOfDay);

// Get query log for the last 6 hours
var sixHoursAgo = DateTimeExtensions.HoursAgo(6);
var recentLog = await queryLogApi.GetQueryLogAsync(sixHoursAgo, now);

// Convert DateTime to Unix milliseconds
var specificDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
var unixMillis = specificDate.ToUnixMilliseconds();

// Convert Unix milliseconds back to DateTime
var dateTime = DateTimeExtensions.FromUnixMilliseconds(unixMillis);

// Using TimeSpan for relative times
var customTime = DateTimeExtensions.FromNow(TimeSpan.FromHours(-12));
```

## Retry Policy Examples

### Basic Retry Usage

```csharp
using AdGuard.ApiClient.Helpers;
using AdGuard.ApiClient.Api;
using AdGuard.ApiClient.Client;

var devicesApi = new DevicesApi(config);

// Execute with automatic retry on transient failures
var devices = await RetryPolicyHelper.ExecuteWithRetryAsync(
    () => devicesApi.ListDevicesAsync()
);

// Custom retry count
var devices = await RetryPolicyHelper.ExecuteWithRetryAsync(
    () => devicesApi.ListDevicesAsync(),
    maxRetries: 5
);

// Using retry policy directly
var retryPolicy = RetryPolicyHelper.CreateDefaultRetryPolicy<List<Device>>();
var devices = await retryPolicy.ExecuteAsync(
    () => devicesApi.ListDevicesAsync()
);
```

### Rate Limit Handling

```csharp
// Create rate limit specific retry policy (for 429 errors)
var rateLimitPolicy = RetryPolicyHelper.CreateRateLimitRetryPolicy(
    maxRetries: 5,
    baseDelay: 5 // 5 seconds base delay
);

// Execute with rate limit handling
var result = await rateLimitPolicy.ExecuteAsync(async () =>
{
    return await devicesApi.CreateDeviceAsync(newDevice);
});
```

### Custom Retry Logic

```csharp
using Polly;

// Combine multiple policies
var retryPolicy = RetryPolicyHelper.CreateDefaultRetryPolicy<Device>();
var timeoutPolicy = Policy.TimeoutAsync<Device>(TimeSpan.FromSeconds(30));

var combinedPolicy = Policy.WrapAsync(retryPolicy, timeoutPolicy);

var device = await combinedPolicy.ExecuteAsync(
    () => devicesApi.GetDeviceAsync(deviceId)
);
```

## Complete Examples

### Device Management with Retry

```csharp
using AdGuard.ApiClient.Api;
using AdGuard.ApiClient.Client;
using AdGuard.ApiClient.Model;
using AdGuard.ApiClient.Helpers;

// Configure client
var config = ConfigurationHelper
    .CreateWithApiKey("your-api-key")
    .WithTimeout(30000);

var devicesApi = new DevicesApi(config);

// Create device with automatic retry
var newDevice = new DeviceCreate
{
    Name = "My iPhone",
    DeviceType = "IOS",
    DnsServerId = "dns-server-id"
};

var createdDevice = await RetryPolicyHelper.ExecuteWithRetryAsync(
    () => devicesApi.CreateDeviceAsync(newDevice)
);

Console.WriteLine($"Device created: {createdDevice.Id}");

// List devices with retry
var devices = await RetryPolicyHelper.ExecuteWithRetryAsync(
    () => devicesApi.ListDevicesAsync()
);

foreach (var device in devices)
{
    Console.WriteLine($"{device.Name}: {device.Id}");
}
```

### Query Log Analysis

```csharp
using AdGuard.ApiClient.Api;
using AdGuard.ApiClient.Helpers;

var config = ConfigurationHelper.CreateWithApiKey("your-api-key");
var queryLogApi = new QueryLogApi(config);
var statsApi = new StatisticsApi(config);

// Get last 7 days of query logs
var weekAgo = DateTimeExtensions.DaysAgo(7);
var now = DateTimeExtensions.Now();

// Fetch query log with retry
var queryLog = await RetryPolicyHelper.ExecuteWithRetryAsync(
    () => queryLogApi.GetQueryLogAsync(weekAgo, now, limit: 1000)
);

// Fetch statistics with retry
var stats = await RetryPolicyHelper.ExecuteWithRetryAsync(
    () => statsApi.GetTimeQueriesStatsAsync(weekAgo, now)
);

// Analyze results
var totalQueries = stats.Stats.Sum(s => s.Value.Queries);
var totalBlocked = stats.Stats.Sum(s => s.Value.Blocked);
var blockRate = (double)totalBlocked / totalQueries * 100;

Console.WriteLine($"Total Queries: {totalQueries:N0}");
Console.WriteLine($"Total Blocked: {totalBlocked:N0}");
Console.WriteLine($"Block Rate: {blockRate:F2}%");
```

### Bulk Operations with Error Handling

```csharp
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdGuard.ApiClient.Api;
using AdGuard.ApiClient.Client;
using AdGuard.ApiClient.Model;
using AdGuard.ApiClient.Helpers;

public class DeviceManager
{
    private readonly DevicesApi _devicesApi;
    private readonly int _maxRetries = 3;

    public DeviceManager(string apiKey)
    {
        var config = ConfigurationHelper.CreateWithApiKey(apiKey);
        _devicesApi = new DevicesApi(config);
    }

    public async Task<List<Device>> MigrateDevicesToServer(
        string oldServerId,
        string newServerId)
    {
        // Get all devices with retry
        var devices = await RetryPolicyHelper.ExecuteWithRetryAsync(
            () => _devicesApi.ListDevicesAsync(),
            _maxRetries
        );

        var devicesToMigrate = devices
            .Where(d => d.DnsServerId == oldServerId)
            .ToList();

        var migratedDevices = new List<Device>();
        var failures = new List<(Device device, Exception error)>();

        foreach (var device in devicesToMigrate)
        {
            try
            {
                var update = new DeviceUpdate
                {
                    DnsServerId = newServerId
                };

                await RetryPolicyHelper.ExecuteWithRetryAsync(
                    () => _devicesApi.UpdateDeviceAsync(device.Id, update),
                    _maxRetries
                );

                // Fetch updated device
                var updatedDevice = await RetryPolicyHelper.ExecuteWithRetryAsync(
                    () => _devicesApi.GetDeviceAsync(device.Id),
                    _maxRetries
                );

                migratedDevices.Add(updatedDevice);
                Console.WriteLine($"✓ Migrated: {device.Name}");
            }
            catch (ApiException ex)
            {
                failures.Add((device, ex));
                Console.WriteLine($"✗ Failed to migrate {device.Name}: {ex.Message}");
            }
        }

        if (failures.Any())
        {
            Console.WriteLine($"\nMigration completed with {failures.Count} failures:");
            foreach (var (device, error) in failures)
            {
                Console.WriteLine($"  - {device.Name}: {error.Message}");
            }
        }

        return migratedDevices;
    }

    public async Task<Dictionary<string, int>> GetDeviceCountByType()
    {
        var devices = await RetryPolicyHelper.ExecuteWithRetryAsync(
            () => _devicesApi.ListDevicesAsync(),
            _maxRetries
        );

        return devices
            .GroupBy(d => d.DeviceType)
            .ToDictionary(g => g.Key, g => g.Count());
    }
}
```

### Authentication Flow

```csharp
using AdGuard.ApiClient.Api;
using AdGuard.ApiClient.Client;
using AdGuard.ApiClient.Model;
using AdGuard.ApiClient.Helpers;

public class AuthenticationManager
{
    private Configuration _config;
    private AccessTokenResponse? _currentToken;

    public async Task<bool> LoginAsync(string username, string password, string? mfaToken = null)
    {
        // Create config without auth for initial login
        _config = ConfigurationHelper.CreateCustom();

        var authApi = new AuthenticationApi(_config);

        var credentials = new AccessTokenCredentials
        {
            Username = username,
            Password = password,
            MfaToken = mfaToken
        };

        try
        {
            // No retry on login - if it fails, credentials are likely wrong
            _currentToken = await authApi.AccessTokenAsync(credentials);

            // Update configuration with the access token
            _config.WithBearerToken(_currentToken.AccessToken);

            return true;
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"Login failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> RefreshTokenAsync()
    {
        if (_currentToken?.RefreshToken == null)
            return false;

        var authApi = new AuthenticationApi(_config);

        var credentials = new AccessTokenCredentials
        {
            RefreshToken = _currentToken.RefreshToken
        };

        try
        {
            _currentToken = await authApi.AccessTokenAsync(credentials);
            _config.WithBearerToken(_currentToken.AccessToken);
            return true;
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"Token refresh failed: {ex.Message}");
            return false;
        }
    }

    public Configuration GetConfiguration()
    {
        if (_config == null)
            throw new InvalidOperationException("Must login first");

        return _config;
    }
}
```

### Statistics Monitoring

```csharp
using System;
using System.Threading.Tasks;
using AdGuard.ApiClient.Api;
using AdGuard.ApiClient.Helpers;

public class StatsMonitor
{
    private readonly StatisticsApi _statsApi;

    public StatsMonitor(string apiKey)
    {
        var config = ConfigurationHelper.CreateWithApiKey(apiKey);
        _statsApi = new StatisticsApi(config);
    }

    public async Task<DailyStats> GetDailyStatsAsync()
    {
        var startOfDay = DateTimeExtensions.StartOfToday();
        var now = DateTimeExtensions.Now();

        var stats = await RetryPolicyHelper.ExecuteWithRetryAsync(
            () => _statsApi.GetTimeQueriesStatsAsync(startOfDay, now)
        );

        return new DailyStats
        {
            TotalQueries = stats.Stats.Sum(s => s.Value.Queries),
            BlockedQueries = stats.Stats.Sum(s => s.Value.Blocked),
            Companies = stats.Stats.FirstOrDefault()?.Value.Companies ?? 0
        };
    }

    public async Task<WeeklyReport> GetWeeklyReportAsync()
    {
        var weekAgo = DateTimeExtensions.DaysAgo(7);
        var now = DateTimeExtensions.Now();

        // Fetch all stats in parallel
        var timeStatsTask = RetryPolicyHelper.ExecuteWithRetryAsync(
            () => _statsApi.GetTimeQueriesStatsAsync(weekAgo, now)
        );

        var deviceStatsTask = RetryPolicyHelper.ExecuteWithRetryAsync(
            () => _statsApi.GetDevicesQueriesStatsAsync(weekAgo, now)
        );

        var companyStatsTask = RetryPolicyHelper.ExecuteWithRetryAsync(
            () => _statsApi.GetCompaniesStatsAsync(weekAgo, now)
        );

        await Task.WhenAll(timeStatsTask, deviceStatsTask, companyStatsTask);

        var timeStats = await timeStatsTask;
        var deviceStats = await deviceStatsTask;
        var companyStats = await companyStatsTask;

        return new WeeklyReport
        {
            StartTime = DateTimeExtensions.FromUnixMilliseconds(weekAgo),
            EndTime = DateTimeExtensions.FromUnixMilliseconds(now),
            TotalQueries = timeStats.Stats.Sum(s => s.Value.Queries),
            TotalBlocked = timeStats.Stats.Sum(s => s.Value.Blocked),
            ActiveDevices = deviceStats.Stats.Count,
            TopCompanies = companyStats.Stats.Take(10).ToList()
        };
    }
}

public class DailyStats
{
    public long TotalQueries { get; set; }
    public long BlockedQueries { get; set; }
    public int Companies { get; set; }
    public double BlockRate => TotalQueries > 0 ? (double)BlockedQueries / TotalQueries * 100 : 0;
}

public class WeeklyReport
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public long TotalQueries { get; set; }
    public long TotalBlocked { get; set; }
    public int ActiveDevices { get; set; }
    public List<CompanyQueriesStats> TopCompanies { get; set; }
}
```

## Best Practices

1. **Always use retry policies** for production code to handle transient failures
2. **Use ConfigurationHelper** for clean, fluent configuration setup
3. **Use DateTimeExtensions** for all time-based operations to avoid manual Unix milliseconds conversion
4. **Handle rate limiting** by using appropriate retry policies with exponential backoff
5. **Parallelize independent API calls** using Task.WhenAll for better performance
6. **Cache Configuration instances** - create once, reuse across API calls
7. **Use async/await** throughout your code for better resource utilization
8. **Implement proper error handling** and logging for production scenarios
