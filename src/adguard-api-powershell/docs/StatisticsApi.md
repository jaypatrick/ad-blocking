# PSAdGuardDNS.PSAdGuardDNS\Api.StatisticsApi

All URIs are relative to *https://api.adguard-dns.io*

Method | HTTP request | Description
------------- | ------------- | -------------
[**Get-CategoriesQueriesStats**](StatisticsApi.md#Get-CategoriesQueriesStats) | **GET** /oapi/v1/stats/categories | Gets categories statistics
[**Get-CompaniesStats**](StatisticsApi.md#Get-CompaniesStats) | **GET** /oapi/v1/stats/companies | Gets companies statistics
[**Get-CountriesQueriesStats**](StatisticsApi.md#Get-CountriesQueriesStats) | **GET** /oapi/v1/stats/countries | Gets countries statistics
[**Get-DetailedCompaniesStats**](StatisticsApi.md#Get-DetailedCompaniesStats) | **GET** /oapi/v1/stats/companies/detailed | Gets detailed companies statistics
[**Get-DevicesQueriesStats**](StatisticsApi.md#Get-DevicesQueriesStats) | **GET** /oapi/v1/stats/devices | Gets devices statistics
[**Get-DomainsQueriesStats**](StatisticsApi.md#Get-DomainsQueriesStats) | **GET** /oapi/v1/stats/domains | Gets domains statistics
[**Get-TimeQueriesStats**](StatisticsApi.md#Get-TimeQueriesStats) | **GET** /oapi/v1/stats/time | Gets time statistics


<a id="Get-CategoriesQueriesStats"></a>
# **Get-CategoriesQueriesStats**
> CategoryQueriesStatsList Get-CategoriesQueriesStats<br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-TimeFromMillis] <Int64><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-TimeToMillis] <Int64><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-Devices] <String[]><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-Countries] <String[]><br>

Gets categories statistics

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

# Gets categories statistics
try {
    $Result = Get-CategoriesQueriesStats -TimeFromMillis $TimeFromMillis -TimeToMillis $TimeToMillis -Devices $Devices -Countries $Countries
} catch {
    Write-Host ("Exception occurred when calling Get-CategoriesQueriesStats: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
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

### Return type

[**CategoryQueriesStatsList**](CategoryQueriesStatsList.md) (PSCustomObject)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: */*, application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="Get-CompaniesStats"></a>
# **Get-CompaniesStats**
> CompanyQueriesStatsList Get-CompaniesStats<br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-TimeFromMillis] <Int64><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-TimeToMillis] <Int64><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-Devices] <String[]><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-Countries] <String[]><br>

Gets companies statistics

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

# Gets companies statistics
try {
    $Result = Get-CompaniesStats -TimeFromMillis $TimeFromMillis -TimeToMillis $TimeToMillis -Devices $Devices -Countries $Countries
} catch {
    Write-Host ("Exception occurred when calling Get-CompaniesStats: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
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

### Return type

[**CompanyQueriesStatsList**](CompanyQueriesStatsList.md) (PSCustomObject)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: */*, application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="Get-CountriesQueriesStats"></a>
# **Get-CountriesQueriesStats**
> CountryQueriesStatsList Get-CountriesQueriesStats<br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-TimeFromMillis] <Int64><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-TimeToMillis] <Int64><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-Devices] <String[]><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-Countries] <String[]><br>

Gets countries statistics

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

# Gets countries statistics
try {
    $Result = Get-CountriesQueriesStats -TimeFromMillis $TimeFromMillis -TimeToMillis $TimeToMillis -Devices $Devices -Countries $Countries
} catch {
    Write-Host ("Exception occurred when calling Get-CountriesQueriesStats: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
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

### Return type

[**CountryQueriesStatsList**](CountryQueriesStatsList.md) (PSCustomObject)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: */*, application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="Get-DetailedCompaniesStats"></a>
# **Get-DetailedCompaniesStats**
> CompanyDetailedQueriesStatsList Get-DetailedCompaniesStats<br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-TimeFromMillis] <Int64><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-TimeToMillis] <Int64><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-Devices] <String[]><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-Countries] <String[]><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-Cursor] <String><br>

Gets detailed companies statistics

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
$Cursor = "MyCursor" # String | Pagination cursor (optional)

# Gets detailed companies statistics
try {
    $Result = Get-DetailedCompaniesStats -TimeFromMillis $TimeFromMillis -TimeToMillis $TimeToMillis -Devices $Devices -Countries $Countries -Cursor $Cursor
} catch {
    Write-Host ("Exception occurred when calling Get-DetailedCompaniesStats: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
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
 **Cursor** | **String**| Pagination cursor | [optional] 

### Return type

[**CompanyDetailedQueriesStatsList**](CompanyDetailedQueriesStatsList.md) (PSCustomObject)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: */*, application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="Get-DevicesQueriesStats"></a>
# **Get-DevicesQueriesStats**
> DeviceQueriesStatsList Get-DevicesQueriesStats<br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-TimeFromMillis] <Int64><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-TimeToMillis] <Int64><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-Devices] <String[]><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-Countries] <String[]><br>

Gets devices statistics

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

# Gets devices statistics
try {
    $Result = Get-DevicesQueriesStats -TimeFromMillis $TimeFromMillis -TimeToMillis $TimeToMillis -Devices $Devices -Countries $Countries
} catch {
    Write-Host ("Exception occurred when calling Get-DevicesQueriesStats: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
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

### Return type

[**DeviceQueriesStatsList**](DeviceQueriesStatsList.md) (PSCustomObject)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: */*, application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="Get-DomainsQueriesStats"></a>
# **Get-DomainsQueriesStats**
> DomainQueriesStatsList Get-DomainsQueriesStats<br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-TimeFromMillis] <Int64><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-TimeToMillis] <Int64><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-Devices] <String[]><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-Countries] <String[]><br>

Gets domains statistics

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

# Gets domains statistics
try {
    $Result = Get-DomainsQueriesStats -TimeFromMillis $TimeFromMillis -TimeToMillis $TimeToMillis -Devices $Devices -Countries $Countries
} catch {
    Write-Host ("Exception occurred when calling Get-DomainsQueriesStats: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
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

### Return type

[**DomainQueriesStatsList**](DomainQueriesStatsList.md) (PSCustomObject)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: */*, application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="Get-TimeQueriesStats"></a>
# **Get-TimeQueriesStats**
> TimeQueriesStatsList Get-TimeQueriesStats<br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-TimeFromMillis] <Int64><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-TimeToMillis] <Int64><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-Devices] <String[]><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-Countries] <String[]><br>

Gets time statistics

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

# Gets time statistics
try {
    $Result = Get-TimeQueriesStats -TimeFromMillis $TimeFromMillis -TimeToMillis $TimeToMillis -Devices $Devices -Countries $Countries
} catch {
    Write-Host ("Exception occurred when calling Get-TimeQueriesStats: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
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

### Return type

[**TimeQueriesStatsList**](TimeQueriesStatsList.md) (PSCustomObject)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: */*, application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

