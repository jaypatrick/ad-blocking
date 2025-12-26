# PSAdGuardDNS.PSAdGuardDNS\Api.AccountApi

All URIs are relative to *https://api.adguard-dns.io*

Method | HTTP request | Description
------------- | ------------- | -------------
[**Get-AccountLimits**](AccountApi.md#Get-AccountLimits) | **GET** /oapi/v1/account/limits | Gets account limits


<a id="Get-AccountLimits"></a>
# **Get-AccountLimits**
> AccountLimits Get-AccountLimits<br>

Gets account limits

### Example
```powershell
# general setting of the PowerShell module, e.g. base URL, authentication, etc
$Configuration = Get-Configuration
# Configure API key authorization: ApiKey
$Configuration.ApiKey.Authorization = "YOUR_API_KEY"
# Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
#$Configuration.ApiKeyPrefix.Authorization = "Bearer"



# Gets account limits
try {
    $Result = Get-AccountLimits
} catch {
    Write-Host ("Exception occurred when calling Get-AccountLimits: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
    Write-Host ("Response headers: {0}" -f ($_.Exception.Response.Headers | ConvertTo-Json))
}
```

### Parameters
This endpoint does not need any parameter.

### Return type

[**AccountLimits**](AccountLimits.md) (PSCustomObject)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: */*

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

