# AdGuard.ApiClient.Api.FilterListsApi

All URIs are relative to *https://api.adguard-dns.io*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ListFilterLists**](FilterListsApi.md#listfilterlists) | **GET** /oapi/v1/filter_lists | Gets filter lists |

<a id="listfilterlists"></a>
# **ListFilterLists**
> List&lt;FilterList&gt; ListFilterLists ()

Gets filter lists

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
    public class ListFilterListsExample
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
            var apiInstance = new FilterListsApi(httpClient, config, httpClientHandler);

            try
            {
                // Gets filter lists
                List<FilterList> result = apiInstance.ListFilterLists();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FilterListsApi.ListFilterLists: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ListFilterListsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Gets filter lists
    ApiResponse<List<FilterList>> response = apiInstance.ListFilterListsWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FilterListsApi.ListFilterListsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;FilterList&gt;**](FilterList.md)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: */*


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | List of filters |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

