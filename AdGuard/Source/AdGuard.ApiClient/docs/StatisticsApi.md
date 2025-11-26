# AdGuard.ApiClient.Api.StatisticsApi

All URIs are relative to *https://api.adguard-dns.io*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**GetTimeQueriesStats**](StatisticsApi.md#gettimequeriesstats) | **GET** /oapi/v1/stats/time | Gets time statistics |

<a id="gettimequeriesstats"></a>
# **GetTimeQueriesStats**
> TimeQueriesStatsList GetTimeQueriesStats (long timeFromMillis, long timeToMillis)

Gets time statistics

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using AdGuard.ApiClient.Api;
using AdGuard.ApiClient.Client;
using AdGuard.ApiClient.Model;

namespace Example
{
    public class GetTimeQueriesStatsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.adguard-dns.io";
            // Configure API key authorization: ApiKey
            config.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("Authorization", "Bearer");
            // Configure Bearer token for authorization: AuthToken
            config.AccessToken = "YOUR_BEARER_TOKEN";

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new StatisticsApi(httpClient, config, httpClientHandler);
            var timeFromMillis = 789L;  // long | Time from in milliseconds (inclusive)
            var timeToMillis = 789L;  // long | Time to in milliseconds (inclusive)

            try
            {
                // Gets time statistics
                TimeQueriesStatsList result = apiInstance.GetTimeQueriesStats(timeFromMillis, timeToMillis);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StatisticsApi.GetTimeQueriesStats: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTimeQueriesStatsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Gets time statistics
    ApiResponse<TimeQueriesStatsList> response = apiInstance.GetTimeQueriesStatsWithHttpInfo(timeFromMillis, timeToMillis);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StatisticsApi.GetTimeQueriesStatsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **timeFromMillis** | **long** | Time from in milliseconds (inclusive) |  |
| **timeToMillis** | **long** | Time to in milliseconds (inclusive) |  |

### Return type

[**TimeQueriesStatsList**](TimeQueriesStatsList.md)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: */*, application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Time statistics received |  -  |
| **400** | Validation failed |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

