# PSAdGuardDNS.PSAdGuardDNS\Api.AuthenticationApi

All URIs are relative to *https://api.adguard-dns.io*

Method | HTTP request | Description
------------- | ------------- | -------------
[**Invoke-AccessToken**](AuthenticationApi.md#Invoke-AccessToken) | **POST** /oapi/v1/oauth_token | Generates Access and Refresh token
[**Revoke-Token**](AuthenticationApi.md#Revoke-Token) | **POST** /oapi/v1/revoke_token | Revokes a Refresh Token


<a id="Invoke-AccessToken"></a>
# **Invoke-AccessToken**
> AccessTokenResponse Invoke-AccessToken<br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-MfaToken] <String><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-Password] <String><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-RefreshToken] <String><br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-Username] <String><br>

Generates Access and Refresh token

### Example
```powershell
$MfaToken = "MyMfaToken" # String | Two-Factor authentication token (optional)
$Password = "MyPassword" # String | Account password (optional)
$RefreshToken = "MyRefreshToken" # String | Refresh Token using which a new access token has to be generated (optional)
$Username = "MyUsername" # String | Account email (optional)

# Generates Access and Refresh token
try {
    $Result = Invoke-AccessToken -MfaToken $MfaToken -Password $Password -RefreshToken $RefreshToken -Username $Username
} catch {
    Write-Host ("Exception occurred when calling Invoke-AccessToken: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
    Write-Host ("Response headers: {0}" -f ($_.Exception.Response.Headers | ConvertTo-Json))
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **MfaToken** | **String**| Two-Factor authentication token | [optional] 
 **Password** | **String**| Account password | [optional] 
 **RefreshToken** | **String**| Refresh Token using which a new access token has to be generated | [optional] 
 **Username** | **String**| Account email | [optional] 

### Return type

[**AccessTokenResponse**](AccessTokenResponse.md) (PSCustomObject)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/x-www-form-urlencoded
 - **Accept**: */*

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="Revoke-Token"></a>
# **Revoke-Token**
> void Revoke-Token<br>
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-RefreshToken] <String><br>

Revokes a Refresh Token

### Example
```powershell
$RefreshToken = "MyRefreshToken" # String | Refresh Token

# Revokes a Refresh Token
try {
    $Result = Revoke-Token -RefreshToken $RefreshToken
} catch {
    Write-Host ("Exception occurred when calling Revoke-Token: {0}" -f ($_.ErrorDetails | ConvertFrom-Json))
    Write-Host ("Response headers: {0}" -f ($_.Exception.Response.Headers | ConvertTo-Json))
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **RefreshToken** | **String**| Refresh Token | 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

