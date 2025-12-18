# AdGuard.ApiClient.Api.QueryLogApi

All URIs are relative to *https://api.adguard-dns.io*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ClearQueryLog**](QueryLogApi.md#clearquerylog) | **DELETE** /oapi/v1/query_log | Clears query log |
| [**GetQueryLog**](QueryLogApi.md#getquerylog) | **GET** /oapi/v1/query_log | Gets query log |

<a id="clearquerylog"></a>
# **ClearQueryLog**
> void ClearQueryLog ()

Clears query log

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
    public class ClearQueryLogExample
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
            var apiInstance = new QueryLogApi(httpClient, config, httpClientHandler);

            try
            {
                // Clears query log
                apiInstance.ClearQueryLog();
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling QueryLogApi.ClearQueryLog: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ClearQueryLogWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Clears query log
    apiInstance.ClearQueryLogWithHttpInfo();
}
catch (ApiException e)
{
    Debug.Print("Exception when calling QueryLogApi.ClearQueryLogWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
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


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **202** | Query log was cleared |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="getquerylog"></a>
# **GetQueryLog**
> QueryLogResponse GetQueryLog (long timeFromMillis, long timeToMillis)

Gets query log

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
    public class GetQueryLogExample
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
            var apiInstance = new QueryLogApi(httpClient, config, httpClientHandler);
            var timeFromMillis = 789L;  // long | Time from in milliseconds (inclusive)
            var timeToMillis = 789L;  // long | Time to in milliseconds (inclusive)

            try
            {
                // Gets query log
                QueryLogResponse result = apiInstance.GetQueryLog(timeFromMillis, timeToMillis);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling QueryLogApi.GetQueryLog: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetQueryLogWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Gets query log
    ApiResponse<QueryLogResponse> response = apiInstance.GetQueryLogWithHttpInfo(timeFromMillis, timeToMillis);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling QueryLogApi.GetQueryLogWithHttpInfo: " + e.Message);
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

[**QueryLogResponse**](QueryLogResponse.md)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: */*


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Query log |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

