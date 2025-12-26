# PSAdGuardDNS.PSAdGuardDNS\Api.WebServicesApi

All URIs are relative to *https://api.adguard-dns.io*

Method | HTTP request | Description
------------- | ------------- | -------------
[**Invoke-ListWebServices**](WebServicesApi.md#Invoke-ListWebServices) | **GET** /oapi/v1/web_services | Lists web services


<a id="Invoke-ListWebServices"></a>
# **Invoke-ListWebServices**
> WebService[] Invoke-ListWebServices<br>

Lists web services

### Example
```powershell
# general setting of the PowerShell module, e.g. base URL, authentication, etc
$Configuration = Get-Configuration
# Configure API key authorization: ApiKey
$Configuration.ApiKey.Authorization = "YOUR_API_KEY"
# Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
#$Configuration.ApiKeyPrefix.Authorization = "Bearer"



# Lists web services
try {
    $Result = Invoke-ListWebServices
} catch {
    Write-Host ("Exception occurred when calling Invoke-ListWebServices: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
    Write-Host ("Response headers: {0}" -f ($_.Exception.Response.Headers | ConvertTo-Json))
}
```

### Parameters
This endpoint does not need any parameter.

### Return type

[**WebService[]**](WebService.md) (PSCustomObject)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: */*

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

