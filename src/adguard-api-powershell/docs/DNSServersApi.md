# PSAdGuardDNS.PSAdGuardDNS\Api.DNSServersApi

All URIs are relative to *https://api.adguard-dns.io*

Method | HTTP request | Description
------------- | ------------- | -------------
[**New-DNSServer**](DNSServersApi.md#New-DNSServer) | **POST** /oapi/v1/dns_servers | Creates a new DNS server
[**Get-DNSServer**](DNSServersApi.md#Get-DNSServer) | **GET** /oapi/v1/dns_servers/{dns_server_id} | Gets an existing DNS server by ID
[**Invoke-ListDNSServers**](DNSServersApi.md#Invoke-ListDNSServers) | **GET** /oapi/v1/dns_servers | Lists DNS servers that belong to the user.
[**Remove-DNSServer**](DNSServersApi.md#Remove-DNSServer) | **DELETE** /oapi/v1/dns_servers/{dns_server_id} | Removes a DNS server
[**Update-DNSServer**](DNSServersApi.md#Update-DNSServer) | **PUT** /oapi/v1/dns_servers/{dns_server_id} | Updates an existing DNS server
[**Update-DNSServerSettings**](DNSServersApi.md#Update-DNSServerSettings) | **PUT** /oapi/v1/dns_servers/{dns_server_id}/settings | Updates DNS server settings


<a id="New-DNSServer"></a>
# **New-DNSServer**
> DNSServer New-DNSServer<br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-DNSServerCreate] <PSCustomObject><br>

Creates a new DNS server

Creates a new DNS server. You can attach custom settings, otherwise DNS server will be created with default settings.

### Example
```powershell
# general setting of the PowerShell module, e.g. base URL, authentication, etc
$Configuration = Get-Configuration
# Configure API key authorization: ApiKey
$Configuration.ApiKey.Authorization = "YOUR_API_KEY"
# Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
#$Configuration.ApiKeyPrefix.Authorization = "Bearer"


$DNSServerAccessSettingsUpdate = Initialize-DNSServerAccessSettingsUpdate -AllowedClients "MyAllowedClients" -BlockKnownScanners $false -BlockedClients "MyBlockedClients" -BlockedDomainRules "MyBlockedDomainRules" -Enabled $false
$BlockingModeSettingsUpdate = Initialize-BlockingModeSettingsUpdate -BlockingMode "NONE" -Ipv4BlockingAddress "MyIpv4BlockingAddress" -Ipv6BlockingAddress "MyIpv6BlockingAddress"

$FilterListItemUpdate = Initialize-FilterListItemUpdate -Enabled $false -FilterId "MyFilterId"
$FilterListsSettingsUpdate = Initialize-FilterListsSettingsUpdate -Enabled $false -FilterList $FilterListItemUpdate

$BlockedWebServiceUpdate = Initialize-BlockedWebServiceUpdate -Enabled $false -Id "9gag"

$ScheduleTime = Initialize-ScheduleTime -Hours 0 -Minutes 0
$ScheduleDayUpdate = Initialize-ScheduleDayUpdate -DayOfWeek "MONDAY" -Enabled $false -FromTime $ScheduleTime -ToTime $ScheduleTime

$ScheduleWeekUpdate = Initialize-ScheduleWeekUpdate -DailySchedule $ScheduleDayUpdate

$ParentalControlSettingsUpdate = Initialize-ParentalControlSettingsUpdate -BlockAdultWebsitesEnabled $false -BlockedServices $BlockedWebServiceUpdate -Enabled $false -EnginesSafeSearchEnabled $false -ScreenTimeSchedule $ScheduleWeekUpdate -YoutubeSafeSearchEnabled $false

$SafebrowsingSettingsUpdate = Initialize-SafebrowsingSettingsUpdate -BlockDangerousDomains $false -BlockNrd $false -Enabled $false
$UserRulesSettingsUpdate = Initialize-UserRulesSettingsUpdate -Enabled $false -Rules "MyRules"
$DNSServerSettingsUpdate = Initialize-DNSServerSettingsUpdate -AccessSettings $DNSServerAccessSettingsUpdate -AutoConnectDevicesEnabled $false -BlockChromePrefetch $false -BlockFirefoxCanary $false -BlockPrivateRelay $false -BlockTtlSeconds 0 -BlockingModeSettings $BlockingModeSettingsUpdate -FilterListsSettings $FilterListsSettingsUpdate -IpLogEnabled $false -ParentalControlSettings $ParentalControlSettingsUpdate -ProtectionEnabled $false -SafebrowsingSettings $SafebrowsingSettingsUpdate -UserRulesSettings $UserRulesSettingsUpdate

$DNSServerCreate = Initialize-DNSServerCreate -Name "My profile" -Settings $DNSServerSettingsUpdate # DNSServerCreate | 

# Creates a new DNS server
try {
    $Result = New-DNSServer -DNSServerCreate $DNSServerCreate
} catch {
    Write-Host ("Exception occurred when calling New-DNSServer: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
    Write-Host ("Response headers: {0}" -f ($_.Exception.Response.Headers | ConvertTo-Json))
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **DNSServerCreate** | [**DNSServerCreate**](DNSServerCreate.md)|  | 

### Return type

[**DNSServer**](DNSServer.md) (PSCustomObject)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: */*, application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="Get-DNSServer"></a>
# **Get-DNSServer**
> DNSServer Get-DNSServer<br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-DnsServerId] <String><br>

Gets an existing DNS server by ID

### Example
```powershell
# general setting of the PowerShell module, e.g. base URL, authentication, etc
$Configuration = Get-Configuration
# Configure API key authorization: ApiKey
$Configuration.ApiKey.Authorization = "YOUR_API_KEY"
# Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
#$Configuration.ApiKeyPrefix.Authorization = "Bearer"


$DnsServerId = "MyDnsServerId" # String | 

# Gets an existing DNS server by ID
try {
    $Result = Get-DNSServer -DnsServerId $DnsServerId
} catch {
    Write-Host ("Exception occurred when calling Get-DNSServer: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
    Write-Host ("Response headers: {0}" -f ($_.Exception.Response.Headers | ConvertTo-Json))
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **DnsServerId** | **String**|  | 

### Return type

[**DNSServer**](DNSServer.md) (PSCustomObject)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: */*

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="Invoke-ListDNSServers"></a>
# **Invoke-ListDNSServers**
> DNSServer[] Invoke-ListDNSServers<br>

Lists DNS servers that belong to the user.

Lists DNS servers that belong to the user. By default there is at least one default server.

### Example
```powershell
# general setting of the PowerShell module, e.g. base URL, authentication, etc
$Configuration = Get-Configuration
# Configure API key authorization: ApiKey
$Configuration.ApiKey.Authorization = "YOUR_API_KEY"
# Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
#$Configuration.ApiKeyPrefix.Authorization = "Bearer"



# Lists DNS servers that belong to the user.
try {
    $Result = Invoke-ListDNSServers
} catch {
    Write-Host ("Exception occurred when calling Invoke-ListDNSServers: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
    Write-Host ("Response headers: {0}" -f ($_.Exception.Response.Headers | ConvertTo-Json))
}
```

### Parameters
This endpoint does not need any parameter.

### Return type

[**DNSServer[]**](DNSServer.md) (PSCustomObject)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: */*

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="Remove-DNSServer"></a>
# **Remove-DNSServer**
> void Remove-DNSServer<br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-DnsServerId] <String><br>

Removes a DNS server

Removes a DNS server. All devices attached to this DNS server will be moved to the default DNS server. Deleting the default DNS server is forbidden.

### Example
```powershell
# general setting of the PowerShell module, e.g. base URL, authentication, etc
$Configuration = Get-Configuration
# Configure API key authorization: ApiKey
$Configuration.ApiKey.Authorization = "YOUR_API_KEY"
# Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
#$Configuration.ApiKeyPrefix.Authorization = "Bearer"


$DnsServerId = "MyDnsServerId" # String | 

# Removes a DNS server
try {
    $Result = Remove-DNSServer -DnsServerId $DnsServerId
} catch {
    Write-Host ("Exception occurred when calling Remove-DNSServer: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
    Write-Host ("Response headers: {0}" -f ($_.Exception.Response.Headers | ConvertTo-Json))
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **DnsServerId** | **String**|  | 

### Return type

void (empty response body)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="Update-DNSServer"></a>
# **Update-DNSServer**
> void Update-DNSServer<br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-DnsServerId] <String><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-DNSServerUpdate] <PSCustomObject><br>

Updates an existing DNS server

### Example
```powershell
# general setting of the PowerShell module, e.g. base URL, authentication, etc
$Configuration = Get-Configuration
# Configure API key authorization: ApiKey
$Configuration.ApiKey.Authorization = "YOUR_API_KEY"
# Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
#$Configuration.ApiKeyPrefix.Authorization = "Bearer"


$DnsServerId = "MyDnsServerId" # String | 
$DNSServerUpdate = Initialize-DNSServerUpdate -Name "My profile" # DNSServerUpdate | 

# Updates an existing DNS server
try {
    $Result = Update-DNSServer -DnsServerId $DnsServerId -DNSServerUpdate $DNSServerUpdate
} catch {
    Write-Host ("Exception occurred when calling Update-DNSServer: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
    Write-Host ("Response headers: {0}" -f ($_.Exception.Response.Headers | ConvertTo-Json))
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **DnsServerId** | **String**|  | 
 **DNSServerUpdate** | [**DNSServerUpdate**](DNSServerUpdate.md)|  | 

### Return type

void (empty response body)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="Update-DNSServerSettings"></a>
# **Update-DNSServerSettings**
> void Update-DNSServerSettings<br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-DnsServerId] <String><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-DNSServerSettingsUpdate] <PSCustomObject><br>

Updates DNS server settings

### Example
```powershell
# general setting of the PowerShell module, e.g. base URL, authentication, etc
$Configuration = Get-Configuration
# Configure API key authorization: ApiKey
$Configuration.ApiKey.Authorization = "YOUR_API_KEY"
# Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
#$Configuration.ApiKeyPrefix.Authorization = "Bearer"


$DnsServerId = "MyDnsServerId" # String | 
$DNSServerAccessSettingsUpdate = Initialize-DNSServerAccessSettingsUpdate -AllowedClients "MyAllowedClients" -BlockKnownScanners $false -BlockedClients "MyBlockedClients" -BlockedDomainRules "MyBlockedDomainRules" -Enabled $false
$BlockingModeSettingsUpdate = Initialize-BlockingModeSettingsUpdate -BlockingMode "NONE" -Ipv4BlockingAddress "MyIpv4BlockingAddress" -Ipv6BlockingAddress "MyIpv6BlockingAddress"

$FilterListItemUpdate = Initialize-FilterListItemUpdate -Enabled $false -FilterId "MyFilterId"
$FilterListsSettingsUpdate = Initialize-FilterListsSettingsUpdate -Enabled $false -FilterList $FilterListItemUpdate

$BlockedWebServiceUpdate = Initialize-BlockedWebServiceUpdate -Enabled $false -Id "9gag"

$ScheduleTime = Initialize-ScheduleTime -Hours 0 -Minutes 0
$ScheduleDayUpdate = Initialize-ScheduleDayUpdate -DayOfWeek "MONDAY" -Enabled $false -FromTime $ScheduleTime -ToTime $ScheduleTime

$ScheduleWeekUpdate = Initialize-ScheduleWeekUpdate -DailySchedule $ScheduleDayUpdate

$ParentalControlSettingsUpdate = Initialize-ParentalControlSettingsUpdate -BlockAdultWebsitesEnabled $false -BlockedServices $BlockedWebServiceUpdate -Enabled $false -EnginesSafeSearchEnabled $false -ScreenTimeSchedule $ScheduleWeekUpdate -YoutubeSafeSearchEnabled $false

$SafebrowsingSettingsUpdate = Initialize-SafebrowsingSettingsUpdate -BlockDangerousDomains $false -BlockNrd $false -Enabled $false
$UserRulesSettingsUpdate = Initialize-UserRulesSettingsUpdate -Enabled $false -Rules "MyRules"
$DNSServerSettingsUpdate = Initialize-DNSServerSettingsUpdate -AccessSettings $DNSServerAccessSettingsUpdate -AutoConnectDevicesEnabled $false -BlockChromePrefetch $false -BlockFirefoxCanary $false -BlockPrivateRelay $false -BlockTtlSeconds 0 -BlockingModeSettings $BlockingModeSettingsUpdate -FilterListsSettings $FilterListsSettingsUpdate -IpLogEnabled $false -ParentalControlSettings $ParentalControlSettingsUpdate -ProtectionEnabled $false -SafebrowsingSettings $SafebrowsingSettingsUpdate -UserRulesSettings $UserRulesSettingsUpdate # DNSServerSettingsUpdate | 

# Updates DNS server settings
try {
    $Result = Update-DNSServerSettings -DnsServerId $DnsServerId -DNSServerSettingsUpdate $DNSServerSettingsUpdate
} catch {
    Write-Host ("Exception occurred when calling Update-DNSServerSettings: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
    Write-Host ("Response headers: {0}" -f ($_.Exception.Response.Headers | ConvertTo-Json))
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **DnsServerId** | **String**|  | 
 **DNSServerSettingsUpdate** | [**DNSServerSettingsUpdate**](DNSServerSettingsUpdate.md)|  | 

### Return type

void (empty response body)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

