# \AuthenticationApi

All URIs are relative to *https://api.adguard-dns.io*

Method | HTTP request | Description
------------- | ------------- | -------------
[**access_token**](AuthenticationApi.md#access_token) | **POST** /oapi/v1/oauth_token | Generates Access and Refresh token
[**revoke_token**](AuthenticationApi.md#revoke_token) | **POST** /oapi/v1/revoke_token | Revokes a Refresh Token



## access_token

> models::AccessTokenResponse access_token(mfa_token, password, refresh_token, username)
Generates Access and Refresh token

### Parameters


Name | Type | Description  | Required | Notes
------------- | ------------- | ------------- | ------------- | -------------
**mfa_token** | Option<**String**> | Two-Factor authentication token |  |
**password** | Option<**String**> | Account password |  |
**refresh_token** | Option<**String**> | Refresh Token using which a new access token has to be generated |  |
**username** | Option<**String**> | Account email |  |

### Return type

[**models::AccessTokenResponse**](AccessTokenResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/x-www-form-urlencoded
- **Accept**: */*

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)


## revoke_token

> revoke_token(refresh_token)
Revokes a Refresh Token

### Parameters


Name | Type | Description  | Required | Notes
------------- | ------------- | ------------- | ------------- | -------------
**refresh_token** | **String** | Refresh Token | [required] |

### Return type

 (empty response body)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

