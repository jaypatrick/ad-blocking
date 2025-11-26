# AdGuard.ApiClient.Api.DedicatedIPAddressesApi

All URIs are relative to *https://api.adguard-dns.io*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**AllocateDedicatedIPv4Address**](DedicatedIPAddressesApi.md#allocatededicatedipv4address) | **POST** /oapi/v1/dedicated_addresses/ipv4 | Allocates new dedicated IPv4 |
| [**ListDedicatedIPv4Addresses**](DedicatedIPAddressesApi.md#listdedicatedipv4addresses) | **GET** /oapi/v1/dedicated_addresses/ipv4 | Lists allocated dedicated IPv4 addresses |

<a id="allocatededicatedipv4address"></a>
# **AllocateDedicatedIPv4Address**
> DedicatedIPv4Address AllocateDedicatedIPv4Address ()

Allocates new dedicated IPv4

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
    public class AllocateDedicatedIPv4AddressExample
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
            var apiInstance = new DedicatedIPAddressesApi(httpClient, config, httpClientHandler);

            try
            {
                // Allocates new dedicated IPv4
                DedicatedIPv4Address result = apiInstance.AllocateDedicatedIPv4Address();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DedicatedIPAddressesApi.AllocateDedicatedIPv4Address: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the AllocateDedicatedIPv4AddressWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Allocates new dedicated IPv4
    ApiResponse<DedicatedIPv4Address> response = apiInstance.AllocateDedicatedIPv4AddressWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DedicatedIPAddressesApi.AllocateDedicatedIPv4AddressWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**DedicatedIPv4Address**](DedicatedIPv4Address.md)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: */*


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | New IPv4 successfully allocated |  -  |
| **429** | Dedicated IPv4 count reached the limit |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="listdedicatedipv4addresses"></a>
# **ListDedicatedIPv4Addresses**
> List&lt;DedicatedIPv4Address&gt; ListDedicatedIPv4Addresses ()

Lists allocated dedicated IPv4 addresses

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
    public class ListDedicatedIPv4AddressesExample
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
            var apiInstance = new DedicatedIPAddressesApi(httpClient, config, httpClientHandler);

            try
            {
                // Lists allocated dedicated IPv4 addresses
                List<DedicatedIPv4Address> result = apiInstance.ListDedicatedIPv4Addresses();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DedicatedIPAddressesApi.ListDedicatedIPv4Addresses: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ListDedicatedIPv4AddressesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Lists allocated dedicated IPv4 addresses
    ApiResponse<List<DedicatedIPv4Address>> response = apiInstance.ListDedicatedIPv4AddressesWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DedicatedIPAddressesApi.ListDedicatedIPv4AddressesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;DedicatedIPv4Address&gt;**](DedicatedIPv4Address.md)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: */*


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | List of dedicated IPv4 addresses |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

