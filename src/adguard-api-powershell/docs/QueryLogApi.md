# PSAdGuardDNS.PSAdGuardDNS\Api.QueryLogApi

All URIs are relative to *https://api.adguard-dns.io*

Method | HTTP request | Description
------------- | ------------- | -------------
[**Clear-QueryLog**](QueryLogApi.md#Clear-QueryLog) | **DELETE** /oapi/v1/query_log | Clears query log
[**Get-QueryLog**](QueryLogApi.md#Get-QueryLog) | **GET** /oapi/v1/query_log | Gets query log


<a id="Clear-QueryLog"></a>
# **Clear-QueryLog**
> void Clear-QueryLog<br>

Clears query log

### Example
```powershell
# general setting of the PowerShell module, e.g. base URL, authentication, etc
$Configuration = Get-Configuration
# Configure API key authorization: ApiKey
$Configuration.ApiKey.Authorization = "YOUR_API_KEY"
# Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
#$Configuration.ApiKeyPrefix.Authorization = "Bearer"



# Clears query log
try {
    $Result = Clear-QueryLog
} catch {
    Write-Host ("Exception occurred when calling Clear-QueryLog: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
    Write-Host ("Response headers: {0}" -f ($_.Exception.Response.Headers | ConvertTo-Json))
}
```

### Parameters
This endpoint does not need any parameter.

### Return type

void (empty response body)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="Get-QueryLog"></a>
# **Get-QueryLog**
> QueryLogResponse Get-QueryLog<br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-TimeFromMillis] <Int64><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-TimeToMillis] <Int64><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-Devices] <String[]><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-Countries] <String[]><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-Companies] <String[]><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-Statuses] <PSCustomObject[]><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-Categories] <PSCustomObject[]><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-Search] <String><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-Limit] <System.Nullable[Int32]><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-Cursor] <String><br>

Gets query log

### Example
```powershell
# general setting of the PowerShell module, e.g. base URL, authentication, etc
$Configuration = Get-Configuration
# Configure API key authorization: ApiKey
$Configuration.ApiKey.Authorization = "YOUR_API_KEY"
# Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
#$Configuration.ApiKeyPrefix.Authorization = "Bearer"


$TimeFromMillis = 789 # Int64 | Time from in milliseconds (inclusive)
$TimeToMillis = 789 # Int64 | Time to in milliseconds (inclusive)
$Devices = "MyDevices" # String[] | Filter by devices (optional)
$Countries = "MyCountries" # String[] | Filter by countries (optional)
$Companies = "MyCompanies" # String[] | Filter by companies (optional)
$Statuses = "UNKNOWN" # FilteringActionStatus[] | Filter by statuses (optional)
$Categories = "ADS" # CategoryType[] | Filter by categories (optional)
$Search = "MySearch" # String | Filter by domain name (optional)
$Limit = 56 # Int32 | Limit the number of records to be returned (optional) (default to 20)
$Cursor = "MyCursor" # String | Pagination cursor. Use cursor from response to paginate through the pages. (optional)

# Gets query log
try {
    $Result = Get-QueryLog -TimeFromMillis $TimeFromMillis -TimeToMillis $TimeToMillis -Devices $Devices -Countries $Countries -Companies $Companies -Statuses $Statuses -Categories $Categories -Search $Search -Limit $Limit -Cursor $Cursor
} catch {
    Write-Host ("Exception occurred when calling Get-QueryLog: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
    Write-Host ("Response headers: {0}" -f ($_.Exception.Response.Headers | ConvertTo-Json))
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **TimeFromMillis** | **Int64**| Time from in milliseconds (inclusive) | 
 **TimeToMillis** | **Int64**| Time to in milliseconds (inclusive) | 
 **Devices** | [**String[]**](String.md)| Filter by devices | [optional] 
 **Countries** | [**String[]**](String.md)| Filter by countries | [optional] 
 **Companies** | [**String[]**](String.md)| Filter by companies | [optional] 
 **Statuses** | [**FilteringActionStatus[]**](FilteringActionStatus.md)| Filter by statuses | [optional] 
 **Categories** | [**CategoryType[]**](CategoryType.md)| Filter by categories | [optional] 
 **Search** | **String**| Filter by domain name | [optional] 
 **Limit** | **Int32**| Limit the number of records to be returned | [optional] [default to 20]
 **Cursor** | **String**| Pagination cursor. Use cursor from response to paginate through the pages. | [optional] 

### Return type

[**QueryLogResponse**](QueryLogResponse.md) (PSCustomObject)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: */*

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

