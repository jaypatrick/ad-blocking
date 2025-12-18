# AdGuard.ApiClient.Api.AccountApi

All URIs are relative to *https://api.adguard-dns.io*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**GetAccountLimits**](AccountApi.md#getaccountlimits) | **GET** /oapi/v1/account/limits | Gets account limits |

<a id="getaccountlimits"></a>
# **GetAccountLimits**
> AccountLimits GetAccountLimits ()

Gets account limits

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
    public class GetAccountLimitsExample
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
            var apiInstance = new AccountApi(httpClient, config, httpClientHandler);

            try
            {
                // Gets account limits
                AccountLimits result = apiInstance.GetAccountLimits();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AccountApi.GetAccountLimits: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetAccountLimitsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Gets account limits
    ApiResponse<AccountLimits> response = apiInstance.GetAccountLimitsWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AccountApi.GetAccountLimitsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**AccountLimits**](AccountLimits.md)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: */*


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Account limits info |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

