# AdGuard.ApiClient.Api.AuthenticationApi

All URIs are relative to *https://api.adguard-dns.io*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**AccessToken**](AuthenticationApi.md#accesstoken) | **POST** /oapi/v1/oauth_token | Generates Access and Refresh token |

<a id="accesstoken"></a>
# **AccessToken**
> AccessTokenResponse AccessToken (string? mfaToken = null, string? password = null, string? refreshToken = null, string? username = null)

Generates Access and Refresh token

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
    public class AccessTokenExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.adguard-dns.io";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new AuthenticationApi(httpClient, config, httpClientHandler);
            var mfaToken = "mfaToken_example";  // string? |  (optional) 
            var password = "password_example";  // string? |  (optional) 
            var refreshToken = "refreshToken_example";  // string? |  (optional) 
            var username = "username_example";  // string? |  (optional) 

            try
            {
                // Generates Access and Refresh token
                AccessTokenResponse result = apiInstance.AccessToken(mfaToken, password, refreshToken, username);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AuthenticationApi.AccessToken: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the AccessTokenWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Generates Access and Refresh token
    ApiResponse<AccessTokenResponse> response = apiInstance.AccessTokenWithHttpInfo(mfaToken, password, refreshToken, username);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AuthenticationApi.AccessTokenWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **mfaToken** | **string?** |  | [optional]  |
| **password** | **string?** |  | [optional]  |
| **refreshToken** | **string?** |  | [optional]  |
| **username** | **string?** |  | [optional]  |

### Return type

[**AccessTokenResponse**](AccessTokenResponse.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/x-www-form-urlencoded
 - **Accept**: */*


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Access token issued |  -  |
| **400** | Missing required parameters |  -  |
| **401** | Invalid credentials, MFA token or refresh token provided |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

