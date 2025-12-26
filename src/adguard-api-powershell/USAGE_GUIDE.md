# AdGuard DNS PowerShell API Client - Usage Guide

## Overview

This PowerShell module provides a complete API client for the AdGuard DNS API, automatically generated from the OpenAPI specification.

## Installation

### Option 1: Import from Source
```powershell
# Navigate to the module directory
cd D:\source\ad-blocking\src\adguard-api-powershell

# Build the module (if needed)
.\Build.ps1

# Import the module
Import-Module .\src\PSAdGuardDNS -Verbose
```

### Option 2: Import with Prefix (to avoid naming conflicts)
```powershell
Import-Module .\src\PSAdGuardDNS -Prefix AdGuard
```

## Configuration

### Set up Authentication

The API supports two authentication methods:

#### 1. API Key Authentication
```powershell
# Get the configuration object
$Configuration = Get-PSAdGuardDNSConfiguration

# Set your API key
$Configuration.ApiKey.Authorization = $env:ADGUARD_AdGuard__ApiKey

# Optionally set the Bearer prefix (if required)
# $Configuration.ApiKeyPrefix.Authorization = "Bearer"
```

#### 2. OAuth Token Authentication
```powershell
# First, get an access token
$tokenResponse = Invoke-AccessToken -GrantType "client_credentials"

# Set the bearer token
$Configuration = Get-PSAdGuardDNSConfiguration
$Configuration.AccessToken = $tokenResponse.AccessToken
```

## Common Operations

### Account Management

#### Get Account Limits
```powershell
# Get your account limits
$limits = Get-AccountLimits

Write-Host "DNS Servers: $($limits.DnsServers.Current) / $($limits.DnsServers.Total)"
Write-Host "Devices: $($limits.Devices.Current) / $($limits.Devices.Total)"
Write-Host "Rules: $($limits.Rules.Current) / $($limits.Rules.Total)"
```

### DNS Server Management

#### List All DNS Servers
```powershell
# Get all DNS servers
$servers = Invoke-ListDNSServers

foreach ($server in $servers) {
    Write-Host "Server: $($server.Name) (ID: $($server.Id))"
}
```

#### Create a New DNS Server
```powershell
# Create a new DNS server
$newServer = @{
    Name = "My Home DNS Server"
}

$server = New-DNSServer -DNSServerCreate $newServer
Write-Host "Created server with ID: $($server.Id)"
```

#### Get DNS Server Details
```powershell
# Get specific server details
$serverId = "your-server-id"
$server = Get-DNSServer -DnsServerId $serverId

Write-Host "Server Name: $($server.Name)"
Write-Host "DNS Addresses: $($server.DnsAddresses | ConvertTo-Json)"
```

#### Update DNS Server
```powershell
# Update server settings
$updateData = @{
    Name = "Updated Server Name"
}

Update-DNSServer -DnsServerId $serverId -DNSServerUpdate $updateData
```

#### Delete DNS Server
```powershell
# Remove a DNS server
Remove-DNSServer -DnsServerId $serverId
```

### Device Management

#### List All Devices
```powershell
# Get all devices
$devices = Invoke-ListDevices

foreach ($device in $devices) {
    Write-Host "Device: $($device.Name) (ID: $($device.Id))"
    Write-Host "  Type: $($device.ConnectDeviceType)"
}
```

#### Create a New Device
```powershell
# Create a new device
$newDevice = @{
    Name = "My iPhone"
    ConnectDeviceType = "mobile"
}

$device = New-Device -DeviceCreate $newDevice
Write-Host "Created device with ID: $($device.Id)"
```

#### Get Device Details
```powershell
$deviceId = "your-device-id"
$device = Get-Device -DeviceId $deviceId

Write-Host "Device: $($device.Name)"
Write-Host "DNS Addresses:"
Write-Host "  IPv4: $($device.DnsAddresses.Ipv4)"
Write-Host "  IPv6: $($device.DnsAddresses.Ipv6)"
```

#### Update Device Settings
```powershell
# Update device settings
$settings = @{
    Filtering = @{
        Enabled = $true
    }
    SafeBrowsing = @{
        Enabled = $true
    }
}

Update-DeviceSettings -DeviceId $deviceId -DeviceSettingsUpdate $settings
```

#### Reset DNS-over-HTTPS Password
```powershell
# Reset DoH password for a device
Reset-DOHPassword -DeviceId $deviceId
```

### Statistics

#### Get Time-Based Statistics
```powershell
# Get statistics for the last 24 hours
$timeStats = Get-TimeQueriesStats -TimeFromMs (Get-Date).AddDays(-1).ToFileTimeUtc() `
                                   -TimeToMs (Get-Date).ToFileTimeUtc()

Write-Host "Total Queries: $($timeStats.TotalQueries)"
Write-Host "Blocked Queries: $($timeStats.BlockedQueries)"
```

#### Get Domain Statistics
```powershell
# Get top queried domains
$domainStats = Get-DomainsQueriesStats -DeviceIds $deviceId `
                                        -TimeFromMs (Get-Date).AddDays(-7).ToFileTimeUtc() `
                                        -TimeToMs (Get-Date).ToFileTimeUtc()

foreach ($stat in $domainStats.Stats) {
    Write-Host "Domain: $($stat.Domain) - Queries: $($stat.Queries)"
}
```

#### Get Device Statistics
```powershell
# Get per-device statistics
$deviceStats = Get-DevicesQueriesStats -TimeFromMs (Get-Date).AddDays(-1).ToFileTimeUtc() `
                                        -TimeToMs (Get-Date).ToFileTimeUtc()

foreach ($stat in $deviceStats.Stats) {
    Write-Host "Device: $($stat.DeviceId) - Queries: $($stat.Queries)"
}
```

#### Get Category Statistics
```powershell
# Get blocked categories
$categoryStats = Get-CategoriesQueriesStats -DeviceIds $deviceId `
                                             -TimeFromMs (Get-Date).AddDays(-1).ToFileTimeUtc() `
                                             -TimeToMs (Get-Date).ToFileTimeUtc()

foreach ($stat in $categoryStats.Stats) {
    Write-Host "Category: $($stat.Category) - Blocked: $($stat.BlockedQueries)"
}
```

### Query Log

#### Get Query Log
```powershell
# Get recent query log entries
$queryLog = Get-QueryLog -Limit 50 -Offset 0

foreach ($entry in $queryLog.Queries) {
    Write-Host "[$($entry.Time)] $($entry.Domain) - Status: $($entry.Status)"
}
```

#### Clear Query Log
```powershell
# Clear all query log entries
Clear-QueryLog
Write-Host "Query log cleared"
```

### Filter Lists

#### Get Available Filter Lists
```powershell
# List all available filter lists
$filterLists = Invoke-ListFilterLists

foreach ($list in $filterLists) {
    Write-Host "Filter: $($list.Name)"
    Write-Host "  ID: $($list.Id)"
    Write-Host "  Enabled: $($list.Enabled)"
}
```

### Web Services

#### List Web Services
```powershell
# Get available web services for blocking
$webServices = Invoke-ListWebServices

foreach ($service in $webServices) {
    Write-Host "Service: $($service.Name) - ID: $($service.Id)"
}
```

### Dedicated IP Addresses

#### List Dedicated IPv4 Addresses
```powershell
# Get all allocated dedicated IPs
$dedicatedIPs = Invoke-ListDedicatedIPv4Addresses

foreach ($ip in $dedicatedIPs) {
    Write-Host "IP: $($ip.Ipv4Address) - Server: $($ip.DnsServerId)"
}
```

#### Allocate New Dedicated IPv4
```powershell
# Allocate a new dedicated IP
$newIP = New-DedicatedIPv4Address
Write-Host "Allocated IP: $($newIP.Ipv4Address)"
```

#### Link Dedicated IP to Device
```powershell
# Link a dedicated IP to a device
$linkData = @{
    DedicatedIPv4Id = "your-dedicated-ip-id"
}

Invoke-LinkDedicatedIPv4Address -DeviceId $deviceId -LinkDedicatedIPv4 $linkData
```

## Advanced Usage

### Error Handling
```powershell
try {
    $server = Get-DNSServer -DnsServerId "invalid-id"
} catch {
    Write-Host "Error occurred: $($_.Exception.Message)"
    
    # Get detailed error response
    $errorDetails = $_.ErrorDetails | ConvertFrom-Json
    Write-Host "Error Code: $($errorDetails.Code)"
    Write-Host "Error Message: $($errorDetails.Message)"
}
```

### Debugging
```powershell
# Enable debug output
$DebugPreference = 'Continue'

# Make API calls (will show detailed information)
$limits = Get-AccountLimits

# Disable debug output
$DebugPreference = 'SilentlyContinue'
```

### Using with Prefix
```powershell
# Import with prefix to avoid naming conflicts
Import-Module .\src\PSAdGuardDNS -Prefix AG

# Use cmdlets with prefix
$limits = Get-AGAccountLimits
$servers = Invoke-AGListDNSServers
```

## Complete Example Script

```powershell
# Complete example: Create DNS server and device, get statistics

# Import module
Import-Module D:\source\ad-blocking\src\adguard-api-powershell\src\PSAdGuardDNS

# Configure authentication
$Configuration = Get-PSAdGuardDNSConfiguration
$Configuration.ApiKey.Authorization = $env:ADGUARD_AdGuard__ApiKey

# Check account limits
Write-Host "=== Account Limits ===" -ForegroundColor Cyan
$limits = Get-AccountLimits
Write-Host "DNS Servers: $($limits.DnsServers.Current) / $($limits.DnsServers.Total)"
Write-Host "Devices: $($limits.Devices.Current) / $($limits.Devices.Total)"
Write-Host ""

# List existing DNS servers
Write-Host "=== Existing DNS Servers ===" -ForegroundColor Cyan
$servers = Invoke-ListDNSServers
foreach ($server in $servers) {
    Write-Host "- $($server.Name) (ID: $($server.Id))"
}
Write-Host ""

# List existing devices
Write-Host "=== Existing Devices ===" -ForegroundColor Cyan
$devices = Invoke-ListDevices
foreach ($device in $devices) {
    Write-Host "- $($device.Name) (ID: $($device.Id), Type: $($device.ConnectDeviceType))"
}
Write-Host ""

# Get statistics for the first device (if any)
if ($devices -and $devices.Count -gt 0) {
    $firstDevice = $devices[0]
    Write-Host "=== Statistics for $($firstDevice.Name) ===" -ForegroundColor Cyan
    
    $yesterday = (Get-Date).AddDays(-1)
    $now = Get-Date
    
    $timeStats = Get-TimeQueriesStats -DeviceIds $firstDevice.Id `
                                       -TimeFromMs ([int64]($yesterday - [datetime]'1970-01-01').TotalMilliseconds) `
                                       -TimeToMs ([int64]($now - [datetime]'1970-01-01').TotalMilliseconds)
    
    Write-Host "Total Queries: $($timeStats.TotalQueries)"
    Write-Host "Blocked: $($timeStats.BlockedQueries)"
    Write-Host "Encrypted: $($timeStats.EncryptedQueries)"
}

Write-Host ""
Write-Host "Done!" -ForegroundColor Green
```

## Testing

To run the included Pester tests:

```powershell
# Install Pester if not already installed
Install-Module -Name Pester -Force -SkipPublisherCheck

# Run tests
Invoke-Pester
```

## Regenerating the Client

If the API specification changes, regenerate the client:

```powershell
cd D:\source\ad-blocking\src\adguard-api-powershell

# Using Docker
.\Generate-PowerShellClient.ps1 -UseDocker

# Or using npm-based generator (if installed)
.\Generate-PowerShellClient.ps1
```

## Documentation

Full documentation is available in the `docs/` directory:
- API endpoint documentation: `docs/*Api.md`
- Model documentation: `docs/*Model.md`
- Main README: `README.md`

## Support

For API-specific questions, refer to:
- AdGuard DNS API Documentation: https://adguard-dns.io/kb/private-dns/api/overview/
- OpenAPI Specification: https://api.adguard-dns.io/swagger/openapi.json
