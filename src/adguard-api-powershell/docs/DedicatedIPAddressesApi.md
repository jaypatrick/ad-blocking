# PSAdGuardDNS.PSAdGuardDNS\Api.DedicatedIPAddressesApi

All URIs are relative to *https://api.adguard-dns.io*

Method | HTTP request | Description
------------- | ------------- | -------------
[**New-DedicatedIPv4Address**](DedicatedIPAddressesApi.md#New-DedicatedIPv4Address) | **POST** /oapi/v1/dedicated_addresses/ipv4 | Allocates new dedicated IPv4
[**Invoke-ListDedicatedIPv4Addresses**](DedicatedIPAddressesApi.md#Invoke-ListDedicatedIPv4Addresses) | **GET** /oapi/v1/dedicated_addresses/ipv4 | Lists allocated dedicated IPv4 addresses


<a id="New-DedicatedIPv4Address"></a>
# **New-DedicatedIPv4Address**
> DedicatedIPv4Address New-DedicatedIPv4Address<br>

Allocates new dedicated IPv4

### Example
```powershell
# general setting of the PowerShell module, e.g. base URL, authentication, etc
$Configuration = Get-Configuration
# Configure API key authorization: ApiKey
$Configuration.ApiKey.Authorization = "YOUR_API_KEY"
# Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
#$Configuration.ApiKeyPrefix.Authorization = "Bearer"



# Allocates new dedicated IPv4
try {
    $Result = New-DedicatedIPv4Address
} catch {
    Write-Host ("Exception occurred when calling New-DedicatedIPv4Address: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
    Write-Host ("Response headers: {0}" -f ($_.Exception.Response.Headers | ConvertTo-Json))
}
```

### Parameters
This endpoint does not need any parameter.

### Return type

[**DedicatedIPv4Address**](DedicatedIPv4Address.md) (PSCustomObject)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: */*

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="Invoke-ListDedicatedIPv4Addresses"></a>
# **Invoke-ListDedicatedIPv4Addresses**
> DedicatedIPv4Address[] Invoke-ListDedicatedIPv4Addresses<br>

Lists allocated dedicated IPv4 addresses

### Example
```powershell
# general setting of the PowerShell module, e.g. base URL, authentication, etc
$Configuration = Get-Configuration
# Configure API key authorization: ApiKey
$Configuration.ApiKey.Authorization = "YOUR_API_KEY"
# Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
#$Configuration.ApiKeyPrefix.Authorization = "Bearer"



# Lists allocated dedicated IPv4 addresses
try {
    $Result = Invoke-ListDedicatedIPv4Addresses
} catch {
    Write-Host ("Exception occurred when calling Invoke-ListDedicatedIPv4Addresses: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
    Write-Host ("Response headers: {0}" -f ($_.Exception.Response.Headers | ConvertTo-Json))
}
```

### Parameters
This endpoint does not need any parameter.

### Return type

[**DedicatedIPv4Address[]**](DedicatedIPv4Address.md) (PSCustomObject)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: */*

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

