# AdGuard DNS API Client - Usage Guide

This C# client library provides a strongly-typed interface to the AdGuard DNS API (v1.11).

## Overview

The AdGuard DNS API Client was generated from the official AdGuard DNS OpenAPI specification and provides access to all AdGuard DNS API endpoints for managing DNS servers, devices, statistics, query logs, and more.

## Installation

### As a Project Reference

Add a reference to the `AdGuard.ApiClient` project in your .csproj file:

```xml
<ItemGroup>
  <ProjectReference Include="path\to\src\adguard-api-client\src\AdGuard.ApiClient\AdGuard.ApiClient.csproj" />
</ItemGroup>
```

Or if your project is within this repository:

```xml
<ItemGroup>
  <ProjectReference Include="..\..\src\adguard-api-client\src\AdGuard.ApiClient\AdGuard.ApiClient.csproj" />
</ItemGroup>
```

## Authentication

AdGuard DNS API supports two authentication methods:

### 1. API Key Authentication

```csharp
using AdGuard.ApiClient.Client;

var config = new Configuration
{
    BasePath = "https://api.adguard-dns.io",
    ApiKey = new Dictionary<string, string>
    {
        { "Authorization", "ApiKey your-api-key-here" }
    }
};
```

### 2. Bearer Token Authentication

```csharp
using AdGuard.ApiClient.Client;
using AdGuard.ApiClient.Api;

// First, obtain an access token
var authApi = new AuthenticationApi();
var tokenRequest = new AccessTokenCredentials
{
    Username = "your-email@example.com",
    Password = "your-password",
    MfaToken = "123456" // Optional, if 2FA is enabled
};

var tokenResponse = await authApi.AccessTokenAsync(tokenRequest);

// Configure client with bearer token
var config = new Configuration
{
    BasePath = "https://api.adguard-dns.io",
    AccessToken = tokenResponse.AccessToken
};
```

## Quick Start Examples

### 1. Account Management

```csharp
using AdGuard.ApiClient.Api;
using AdGuard.ApiClient.Client;

// Configure authentication
var config = new Configuration
{
    BasePath = "https://api.adguard-dns.io",
    ApiKey = new Dictionary<string, string>
    {
        { "Authorization", "ApiKey your-api-key-here" }
    }
};

// Get account limits
var accountApi = new AccountApi(config);
var limits = await accountApi.GetAccountLimitsAsync();

Console.WriteLine($"Devices: {limits.Devices.Used}/{limits.Devices.Limit}");
Console.WriteLine($"DNS Servers: {limits.DnsServers.Used}/{limits.DnsServers.Limit}");
Console.WriteLine($"User Rules: {limits.UserRules.Used}/{limits.UserRules.Limit}");
```

### 2. Device Management

```csharp
var devicesApi = new DevicesApi(config);

// List all devices
var devices = await devicesApi.ListDevicesAsync();
foreach (var device in devices)
{
    Console.WriteLine($"{device.Name} ({device.DeviceType}): {device.Id}");
}

// Create a new device
var newDevice = new DeviceCreate
{
    Name = "My iPhone",
    DeviceType = "IOS",
    DnsServerId = "dns-server-id-here"
};
var createdDevice = await devicesApi.CreateDeviceAsync(newDevice);
Console.WriteLine($"Device created: {createdDevice.Id}");

// Get a specific device
var deviceId = "device-id-here";
var device = await devicesApi.GetDeviceAsync(deviceId);

// Update a device
var deviceUpdate = new DeviceUpdate
{
    Name = "My New iPhone 15",
    DeviceType = "IOS"
};
await devicesApi.UpdateDeviceAsync(deviceId, deviceUpdate);

// Delete a device
await devicesApi.RemoveDeviceAsync(deviceId);
```

### 3. DNS Server Management

```csharp
var dnsServersApi = new DNSServersApi(config);

// List all DNS servers (profiles)
var servers = await dnsServersApi.ListDNSServersAsync();
foreach (var server in servers)
{
    Console.WriteLine($"{server.Name} (Default: {server.Default})");
    Console.WriteLine($"  Devices: {server.DeviceIds.Count}");
}

// Create a new DNS server profile
var newServer = new DNSServerCreate
{
    Name = "Family Profile",
    Settings = new {
        ProtectionEnabled = true,
        BlockAdultWebsitesEnabled = true
    }
};
var createdServer = await dnsServersApi.CreateDNSServerAsync(newServer);

// Get a specific DNS server
var serverId = "dns-server-id-here";
var server = await dnsServersApi.GetDNSServerAsync(serverId);

// Update DNS server
var serverUpdate = new DNSServerUpdate
{
    Name = "Updated Family Profile"
};
await dnsServersApi.UpdateDNSServerAsync(serverId, serverUpdate);

// Delete DNS server (devices will be moved to default server)
await dnsServersApi.RemoveDNSServerAsync(serverId);
```

### 4. Query Log

```csharp
var queryLogApi = new QueryLogApi(config);

// Get query log for the last 24 hours
var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
var yesterday = DateTimeOffset.UtcNow.AddDays(-1).ToUnixTimeMilliseconds();

var queryLog = await queryLogApi.GetQueryLogAsync(
    timeFromMillis: yesterday,
    timeToMillis: now,
    limit: 100
);

foreach (var item in queryLog.Items)
{
    Console.WriteLine($"{item.Domain} - {item.FilteringStatus}");
}

// Clear query log
await queryLogApi.ClearQueryLogAsync();
```

### 5. Statistics

```csharp
var statsApi = new StatisticsApi(config);

// Get time-based statistics
var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
var lastWeek = DateTimeOffset.UtcNow.AddDays(-7).ToUnixTimeMilliseconds();

var timeStats = await statsApi.GetTimeQueriesStatsAsync(
    timeFromMillis: lastWeek,
    timeToMillis: now
);

foreach (var stat in timeStats.Stats)
{
    Console.WriteLine($"Time: {stat.TimeMillis}");
    Console.WriteLine($"  Queries: {stat.Value.Queries}");
    Console.WriteLine($"  Blocked: {stat.Value.Blocked}");
    Console.WriteLine($"  Companies: {stat.Value.Companies}");
}
```

### 6. Dedicated IPv4 Addresses

```csharp
var dedicatedIPApi = new DedicatedIPAddressesApi(config);

// List allocated dedicated IPv4 addresses
var ipv4Addresses = await dedicatedIPApi.ListDedicatedIPv4AddressesAsync();
foreach (var ip in ipv4Addresses)
{
    Console.WriteLine($"IP: {ip.Ip}, Device: {ip.DeviceId ?? "Not linked"}");
}

// Allocate a new dedicated IPv4
var newIP = await dedicatedIPApi.AllocateDedicatedIPv4AddressAsync();
Console.WriteLine($"Allocated new IP: {newIP.Ip}");
```

### 7. Filter Lists

```csharp
var filterListsApi = new FilterListsApi(config);

// Get available filter lists
var filterLists = await filterListsApi.ListFilterListsAsync();
foreach (var filter in filterLists)
{
    Console.WriteLine($"{filter.Name}: {filter.Description}");
}
```

### 8. Web Services (Blocking)

```csharp
var webServicesApi = new WebServicesApi(config);

// Get available web services for blocking
var webServices = await webServicesApi.ListWebServicesAsync();
foreach (var service in webServices)
{
    Console.WriteLine($"{service.Name} (ID: {service.Id})");
}
```

## Integration with ASP.NET Core

For ASP.NET Core or other DI frameworks:

```csharp
// In Startup.cs or Program.cs
services.AddSingleton<IConfiguration>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();

    return new AdGuard.ApiClient.Client.Configuration
    {
        BasePath = config["AdGuardDNS:BasePath"],
        ApiKey = new Dictionary<string, string>
        {
            { "Authorization", $"ApiKey {config["AdGuardDNS:ApiKey"]}" }
        }
    };
});

services.AddHttpClient<IAccountApi, AccountApi>();
services.AddHttpClient<IDevicesApi, DevicesApi>();
services.AddHttpClient<IDNSServersApi, DNSServersApi>();
services.AddHttpClient<IQueryLogApi, QueryLogApi>();
services.AddHttpClient<IStatisticsApi, StatisticsApi>();
// Add other APIs as needed
```

Then in appsettings.json:

```json
{
  "AdGuardDNS": {
    "BasePath": "https://api.adguard-dns.io",
    "ApiKey": "your-api-key-here"
  }
}
```

## Error Handling

```csharp
using AdGuard.ApiClient.Client;

try
{
    var devices = await devicesApi.ListDevicesAsync();
}
catch (ApiException ex)
{
    Console.WriteLine($"Error Code: {ex.ErrorCode}");
    Console.WriteLine($"Message: {ex.Message}");
    Console.WriteLine($"Response: {ex.ErrorContent}");

    if (ex.ErrorCode == 429)
    {
        Console.WriteLine("Rate limit reached!");
    }
}
```

## Available API Classes

- **AccountApi** - Account limits and information
- **AuthenticationApi** - OAuth token generation and management
- **DevicesApi** - Device management (create, update, delete, list)
- **DNSServersApi** - DNS server profile management
- **DedicatedIPAddressesApi** - Dedicated IPv4 address management
- **FilterListsApi** - Available filter lists
- **QueryLogApi** - Query log retrieval and management
- **StatisticsApi** - DNS query statistics
- **WebServicesApi** - Web services for blocking

## API Documentation

For detailed API documentation, see the [API Reference](../api/) directory which contains markdown documentation for all models and API endpoints.

- [API Endpoints](../README.md#documentation-for-api-endpoints)
- [Data Models](../README.md#documentation-for-models)

## Common Use Cases

### Automated Device Provisioning

```csharp
public async Task<Device> ProvisionDevice(string deviceName, string deviceType, string dnsServerId)
{
    var devicesApi = new DevicesApi(config);

    var deviceCreate = new DeviceCreate
    {
        Name = deviceName,
        DeviceType = deviceType,
        DnsServerId = dnsServerId
    };

    return await devicesApi.CreateDeviceAsync(deviceCreate);
}
```

### Monitoring Query Statistics

```csharp
public async Task<Dictionary<string, long>> GetDailyStats()
{
    var statsApi = new StatisticsApi(config);

    var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    var dayAgo = DateTimeOffset.UtcNow.AddDays(-1).ToUnixTimeMilliseconds();

    var stats = await statsApi.GetTimeQueriesStatsAsync(dayAgo, now);

    return new Dictionary<string, long>
    {
        { "TotalQueries", stats.Stats.Sum(s => s.Value.Queries) },
        { "BlockedQueries", stats.Stats.Sum(s => s.Value.Blocked) },
        { "Companies", stats.Stats.FirstOrDefault()?.Value.Companies ?? 0 }
    };
}
```

### Bulk Device Management

```csharp
public async Task MigrateDevicesToNewServer(string oldServerId, string newServerId)
{
    var devicesApi = new DevicesApi(config);

    // Get all devices
    var devices = await devicesApi.ListDevicesAsync();

    // Filter devices on old server
    var devicesToMigrate = devices.Where(d => d.DnsServerId == oldServerId);

    // Update each device
    foreach (var device in devicesToMigrate)
    {
        var update = new DeviceUpdate
        {
            DnsServerId = newServerId
        };

        await devicesApi.UpdateDeviceAsync(device.Id, update);
        Console.WriteLine($"Migrated device: {device.Name}");
    }
}
```

## Rate Limiting

The AdGuard DNS API has rate limits. If you receive a 429 status code, you should implement retry logic with exponential backoff:

```csharp
public async Task<T> WithRetry<T>(Func<Task<T>> apiCall, int maxRetries = 3)
{
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            return await apiCall();
        }
        catch (ApiException ex) when (ex.ErrorCode == 429)
        {
            if (i == maxRetries - 1) throw;

            var delay = TimeSpan.FromSeconds(Math.Pow(2, i));
            await Task.Delay(delay);
        }
    }

    throw new Exception("Max retries exceeded");
}

// Usage
var devices = await WithRetry(() => devicesApi.ListDevicesAsync());
```

## Links

- [AdGuard DNS API Documentation](https://adguard-dns.io/kb/private-dns/api/overview/)
- [AdGuard DNS Website](https://adguard-dns.io/)
- [OpenAPI Generator](https://openapi-generator.tech)

## Notes

- All timestamps are in Unix milliseconds (epoch time * 1000)
- Device types include: WINDOWS, ANDROID, MAC, IOS, LINUX, ROUTER, SMART_TV, GAME_CONSOLE, UNKNOWN
- The API uses both ApiKey and Bearer token authentication - choose based on your use case
- Rate limits apply - implement proper retry logic for production use
