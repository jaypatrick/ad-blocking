# PSAdGuardDNS.PSAdGuardDNS\Api.DevicesApi

All URIs are relative to *https://api.adguard-dns.io*

Method | HTTP request | Description
------------- | ------------- | -------------
[**New-Device**](DevicesApi.md#New-Device) | **POST** /oapi/v1/devices | Creates a new device
[**Get-Device**](DevicesApi.md#Get-Device) | **GET** /oapi/v1/devices/{device_id} | Gets an existing device by ID
[**Get-DoHMobileConfig**](DevicesApi.md#Get-DoHMobileConfig) | **GET** /oapi/v1/devices/{device_id}/doh.mobileconfig | Gets DNS-over-HTTPS .mobileconfig file.
[**Get-DoTMobileConfig**](DevicesApi.md#Get-DoTMobileConfig) | **GET** /oapi/v1/devices/{device_id}/dot.mobileconfig | Gets DNS-over-TLS .mobileconfig file.
[**Invoke-LinkDedicatedIPv4Address**](DevicesApi.md#Invoke-LinkDedicatedIPv4Address) | **POST** /oapi/v1/devices/{device_id}/dedicated_addresses/ipv4 | Link dedicated IPv4 to the device
[**Invoke-ListDedicatedAddressesForDevice**](DevicesApi.md#Invoke-ListDedicatedAddressesForDevice) | **GET** /oapi/v1/devices/{device_id}/dedicated_addresses | List dedicated IPv4 and IPv6 addresses for a device
[**Invoke-ListDevices**](DevicesApi.md#Invoke-ListDevices) | **GET** /oapi/v1/devices | Lists devices
[**Remove-Device**](DevicesApi.md#Remove-Device) | **DELETE** /oapi/v1/devices/{device_id} | Removes a device
[**Reset-DOHPassword**](DevicesApi.md#Reset-DOHPassword) | **PUT** /oapi/v1/devices/{device_id}/doh_password/reset | Generate and set new DNS-over-HTTPS password
[**Invoke-UnlinkDedicatedIPv4Address**](DevicesApi.md#Invoke-UnlinkDedicatedIPv4Address) | **DELETE** /oapi/v1/devices/{device_id}/dedicated_addresses/ipv4 | Unlink dedicated IPv4 from the device
[**Update-Device**](DevicesApi.md#Update-Device) | **PUT** /oapi/v1/devices/{device_id} | Updates an existing device
[**Update-DeviceSettings**](DevicesApi.md#Update-DeviceSettings) | **PUT** /oapi/v1/devices/{device_id}/settings | Updates device settings


<a id="New-Device"></a>
# **New-Device**
> Device New-Device<br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-DeviceCreate] <PSCustomObject><br>

Creates a new device

### Example
```powershell
# general setting of the PowerShell module, e.g. base URL, authentication, etc
$Configuration = Get-Configuration
# Configure API key authorization: ApiKey
$Configuration.ApiKey.Authorization = "YOUR_API_KEY"
# Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
#$Configuration.ApiKeyPrefix.Authorization = "Bearer"


$DeviceCreate = Initialize-DeviceCreate -DeviceType "WINDOWS" -DnsServerId "a9f29be1" -Name "My android" # DeviceCreate | 

# Creates a new device
try {
    $Result = New-Device -DeviceCreate $DeviceCreate
} catch {
    Write-Host ("Exception occurred when calling New-Device: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
    Write-Host ("Response headers: {0}" -f ($_.Exception.Response.Headers | ConvertTo-Json))
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **DeviceCreate** | [**DeviceCreate**](DeviceCreate.md)|  | 

### Return type

[**Device**](Device.md) (PSCustomObject)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: */*, application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="Get-Device"></a>
# **Get-Device**
> Device Get-Device<br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-DeviceId] <String><br>

Gets an existing device by ID

### Example
```powershell
# general setting of the PowerShell module, e.g. base URL, authentication, etc
$Configuration = Get-Configuration
# Configure API key authorization: ApiKey
$Configuration.ApiKey.Authorization = "YOUR_API_KEY"
# Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
#$Configuration.ApiKeyPrefix.Authorization = "Bearer"


$DeviceId = "MyDeviceId" # String | 

# Gets an existing device by ID
try {
    $Result = Get-Device -DeviceId $DeviceId
} catch {
    Write-Host ("Exception occurred when calling Get-Device: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
    Write-Host ("Response headers: {0}" -f ($_.Exception.Response.Headers | ConvertTo-Json))
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **DeviceId** | **String**|  | 

### Return type

[**Device**](Device.md) (PSCustomObject)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: */*

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="Get-DoHMobileConfig"></a>
# **Get-DoHMobileConfig**
> void Get-DoHMobileConfig<br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-DeviceId] <String><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-ExcludeWifiNetworks] <String[]><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-ExcludeDomain] <String[]><br>

Gets DNS-over-HTTPS .mobileconfig file.

### Example
```powershell
# general setting of the PowerShell module, e.g. base URL, authentication, etc
$Configuration = Get-Configuration
# Configure API key authorization: ApiKey
$Configuration.ApiKey.Authorization = "YOUR_API_KEY"
# Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
#$Configuration.ApiKeyPrefix.Authorization = "Bearer"


$DeviceId = "MyDeviceId" # String | 
$ExcludeWifiNetworks = "MyExcludeWifiNetworks" # String[] | List Wi-Fi networks by their SSID in which you want AdGuard DNS to be disabled (optional)
$ExcludeDomain = "MyExcludeDomain" # String[] | List domains that will use default DNS servers instead of AdGuard DNS (optional)

# Gets DNS-over-HTTPS .mobileconfig file.
try {
    $Result = Get-DoHMobileConfig -DeviceId $DeviceId -ExcludeWifiNetworks $ExcludeWifiNetworks -ExcludeDomain $ExcludeDomain
} catch {
    Write-Host ("Exception occurred when calling Get-DoHMobileConfig: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
    Write-Host ("Response headers: {0}" -f ($_.Exception.Response.Headers | ConvertTo-Json))
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **DeviceId** | **String**|  | 
 **ExcludeWifiNetworks** | [**String[]**](String.md)| List Wi-Fi networks by their SSID in which you want AdGuard DNS to be disabled | [optional] 
 **ExcludeDomain** | [**String[]**](String.md)| List domains that will use default DNS servers instead of AdGuard DNS | [optional] 

### Return type

void (empty response body)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="Get-DoTMobileConfig"></a>
# **Get-DoTMobileConfig**
> void Get-DoTMobileConfig<br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-DeviceId] <String><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-ExcludeWifiNetworks] <String[]><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-ExcludeDomain] <String[]><br>

Gets DNS-over-TLS .mobileconfig file.

### Example
```powershell
# general setting of the PowerShell module, e.g. base URL, authentication, etc
$Configuration = Get-Configuration
# Configure API key authorization: ApiKey
$Configuration.ApiKey.Authorization = "YOUR_API_KEY"
# Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
#$Configuration.ApiKeyPrefix.Authorization = "Bearer"


$DeviceId = "MyDeviceId" # String | 
$ExcludeWifiNetworks = "MyExcludeWifiNetworks" # String[] | List Wi-Fi networks by their SSID in which you want AdGuard DNS to be disabled (optional)
$ExcludeDomain = "MyExcludeDomain" # String[] | List domains that will use default DNS servers instead of AdGuard DNS (optional)

# Gets DNS-over-TLS .mobileconfig file.
try {
    $Result = Get-DoTMobileConfig -DeviceId $DeviceId -ExcludeWifiNetworks $ExcludeWifiNetworks -ExcludeDomain $ExcludeDomain
} catch {
    Write-Host ("Exception occurred when calling Get-DoTMobileConfig: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
    Write-Host ("Response headers: {0}" -f ($_.Exception.Response.Headers | ConvertTo-Json))
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **DeviceId** | **String**|  | 
 **ExcludeWifiNetworks** | [**String[]**](String.md)| List Wi-Fi networks by their SSID in which you want AdGuard DNS to be disabled | [optional] 
 **ExcludeDomain** | [**String[]**](String.md)| List domains that will use default DNS servers instead of AdGuard DNS | [optional] 

### Return type

void (empty response body)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="Invoke-LinkDedicatedIPv4Address"></a>
# **Invoke-LinkDedicatedIPv4Address**
> void Invoke-LinkDedicatedIPv4Address<br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-DeviceId] <String><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-LinkDedicatedIPv4] <PSCustomObject><br>

Link dedicated IPv4 to the device

### Example
```powershell
# general setting of the PowerShell module, e.g. base URL, authentication, etc
$Configuration = Get-Configuration
# Configure API key authorization: ApiKey
$Configuration.ApiKey.Authorization = "YOUR_API_KEY"
# Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
#$Configuration.ApiKeyPrefix.Authorization = "Bearer"


$DeviceId = "MyDeviceId" # String | 
$LinkDedicatedIPv4 = Initialize-LinkDedicatedIPv4 -Ip "MyIp" # LinkDedicatedIPv4 | 

# Link dedicated IPv4 to the device
try {
    $Result = Invoke-LinkDedicatedIPv4Address -DeviceId $DeviceId -LinkDedicatedIPv4 $LinkDedicatedIPv4
} catch {
    Write-Host ("Exception occurred when calling Invoke-LinkDedicatedIPv4Address: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
    Write-Host ("Response headers: {0}" -f ($_.Exception.Response.Headers | ConvertTo-Json))
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **DeviceId** | **String**|  | 
 **LinkDedicatedIPv4** | [**LinkDedicatedIPv4**](LinkDedicatedIPv4.md)|  | 

### Return type

void (empty response body)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="Invoke-ListDedicatedAddressesForDevice"></a>
# **Invoke-ListDedicatedAddressesForDevice**
> DedicatedIps Invoke-ListDedicatedAddressesForDevice<br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-DeviceId] <String><br>

List dedicated IPv4 and IPv6 addresses for a device

### Example
```powershell
# general setting of the PowerShell module, e.g. base URL, authentication, etc
$Configuration = Get-Configuration
# Configure API key authorization: ApiKey
$Configuration.ApiKey.Authorization = "YOUR_API_KEY"
# Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
#$Configuration.ApiKeyPrefix.Authorization = "Bearer"


$DeviceId = "MyDeviceId" # String | 

# List dedicated IPv4 and IPv6 addresses for a device
try {
    $Result = Invoke-ListDedicatedAddressesForDevice -DeviceId $DeviceId
} catch {
    Write-Host ("Exception occurred when calling Invoke-ListDedicatedAddressesForDevice: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
    Write-Host ("Response headers: {0}" -f ($_.Exception.Response.Headers | ConvertTo-Json))
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **DeviceId** | **String**|  | 

### Return type

[**DedicatedIps**](DedicatedIps.md) (PSCustomObject)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: */*

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="Invoke-ListDevices"></a>
# **Invoke-ListDevices**
> Device[] Invoke-ListDevices<br>

Lists devices

### Example
```powershell
# general setting of the PowerShell module, e.g. base URL, authentication, etc
$Configuration = Get-Configuration
# Configure API key authorization: ApiKey
$Configuration.ApiKey.Authorization = "YOUR_API_KEY"
# Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
#$Configuration.ApiKeyPrefix.Authorization = "Bearer"



# Lists devices
try {
    $Result = Invoke-ListDevices
} catch {
    Write-Host ("Exception occurred when calling Invoke-ListDevices: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
    Write-Host ("Response headers: {0}" -f ($_.Exception.Response.Headers | ConvertTo-Json))
}
```

### Parameters
This endpoint does not need any parameter.

### Return type

[**Device[]**](Device.md) (PSCustomObject)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: */*

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="Remove-Device"></a>
# **Remove-Device**
> void Remove-Device<br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-DeviceId] <String><br>

Removes a device

### Example
```powershell
# general setting of the PowerShell module, e.g. base URL, authentication, etc
$Configuration = Get-Configuration
# Configure API key authorization: ApiKey
$Configuration.ApiKey.Authorization = "YOUR_API_KEY"
# Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
#$Configuration.ApiKeyPrefix.Authorization = "Bearer"


$DeviceId = "MyDeviceId" # String | 

# Removes a device
try {
    $Result = Remove-Device -DeviceId $DeviceId
} catch {
    Write-Host ("Exception occurred when calling Remove-Device: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
    Write-Host ("Response headers: {0}" -f ($_.Exception.Response.Headers | ConvertTo-Json))
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **DeviceId** | **String**|  | 

### Return type

void (empty response body)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="Reset-DOHPassword"></a>
# **Reset-DOHPassword**
> void Reset-DOHPassword<br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-DeviceId] <String><br>

Generate and set new DNS-over-HTTPS password

### Example
```powershell
# general setting of the PowerShell module, e.g. base URL, authentication, etc
$Configuration = Get-Configuration
# Configure API key authorization: ApiKey
$Configuration.ApiKey.Authorization = "YOUR_API_KEY"
# Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
#$Configuration.ApiKeyPrefix.Authorization = "Bearer"


$DeviceId = "MyDeviceId" # String | 

# Generate and set new DNS-over-HTTPS password
try {
    $Result = Reset-DOHPassword -DeviceId $DeviceId
} catch {
    Write-Host ("Exception occurred when calling Reset-DOHPassword: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
    Write-Host ("Response headers: {0}" -f ($_.Exception.Response.Headers | ConvertTo-Json))
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **DeviceId** | **String**|  | 

### Return type

void (empty response body)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="Invoke-UnlinkDedicatedIPv4Address"></a>
# **Invoke-UnlinkDedicatedIPv4Address**
> void Invoke-UnlinkDedicatedIPv4Address<br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-DeviceId] <String><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-Ip] <String><br>

Unlink dedicated IPv4 from the device

### Example
```powershell
# general setting of the PowerShell module, e.g. base URL, authentication, etc
$Configuration = Get-Configuration
# Configure API key authorization: ApiKey
$Configuration.ApiKey.Authorization = "YOUR_API_KEY"
# Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
#$Configuration.ApiKeyPrefix.Authorization = "Bearer"


$DeviceId = "MyDeviceId" # String | 
$Ip = "MyIp" # String | Dedicated IPv4

# Unlink dedicated IPv4 from the device
try {
    $Result = Invoke-UnlinkDedicatedIPv4Address -DeviceId $DeviceId -Ip $Ip
} catch {
    Write-Host ("Exception occurred when calling Invoke-UnlinkDedicatedIPv4Address: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
    Write-Host ("Response headers: {0}" -f ($_.Exception.Response.Headers | ConvertTo-Json))
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **DeviceId** | **String**|  | 
 **Ip** | **String**| Dedicated IPv4 | 

### Return type

void (empty response body)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="Update-Device"></a>
# **Update-Device**
> void Update-Device<br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-DeviceId] <String><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-DeviceUpdate] <PSCustomObject><br>

Updates an existing device

### Example
```powershell
# general setting of the PowerShell module, e.g. base URL, authentication, etc
$Configuration = Get-Configuration
# Configure API key authorization: ApiKey
$Configuration.ApiKey.Authorization = "YOUR_API_KEY"
# Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
#$Configuration.ApiKeyPrefix.Authorization = "Bearer"


$DeviceId = "MyDeviceId" # String | 
$DeviceUpdate = Initialize-DeviceUpdate -DeviceType "WINDOWS" -DnsServerId "a9f29be1" -Name "My android" # DeviceUpdate | 

# Updates an existing device
try {
    $Result = Update-Device -DeviceId $DeviceId -DeviceUpdate $DeviceUpdate
} catch {
    Write-Host ("Exception occurred when calling Update-Device: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
    Write-Host ("Response headers: {0}" -f ($_.Exception.Response.Headers | ConvertTo-Json))
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **DeviceId** | **String**|  | 
 **DeviceUpdate** | [**DeviceUpdate**](DeviceUpdate.md)|  | 

### Return type

void (empty response body)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="Update-DeviceSettings"></a>
# **Update-DeviceSettings**
> void Update-DeviceSettings<br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-DeviceId] <String><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-DeviceSettingsUpdate] <PSCustomObject><br>

Updates device settings

### Example
```powershell
# general setting of the PowerShell module, e.g. base URL, authentication, etc
$Configuration = Get-Configuration
# Configure API key authorization: ApiKey
$Configuration.ApiKey.Authorization = "YOUR_API_KEY"
# Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
#$Configuration.ApiKeyPrefix.Authorization = "Bearer"


$DeviceId = "MyDeviceId" # String | 
$DeviceSettingsUpdate = Initialize-DeviceSettingsUpdate -DetectDohAuthOnly $false -ProtectionEnabled $false # DeviceSettingsUpdate | 

# Updates device settings
try {
    $Result = Update-DeviceSettings -DeviceId $DeviceId -DeviceSettingsUpdate $DeviceSettingsUpdate
} catch {
    Write-Host ("Exception occurred when calling Update-DeviceSettings: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
    Write-Host ("Response headers: {0}" -f ($_.Exception.Response.Headers | ConvertTo-Json))
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **DeviceId** | **String**|  | 
 **DeviceSettingsUpdate** | [**DeviceSettingsUpdate**](DeviceSettingsUpdate.md)|  | 

### Return type

void (empty response body)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

