# AccessTokenResponse
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**AccessToken** | **String** | Access token | [optional] 
**ExpiresIn** | **Int32** | The lifetime in seconds of the access token | 
**RefreshToken** | **String** | Refresh token | [optional] 
**TokenType** | **String** | The type of the token issued | [optional] 

## Examples

- Prepare the resource
```powershell
$AccessTokenResponse = Initialize-PSAdGuardDNSAccessTokenResponse  -AccessToken jTFho_aymtN20pZR5RRSQAzd81I `
 -ExpiresIn 2620978 `
 -RefreshToken H3SW6YFJ-tOPe0FQCM1Jd6VnMiA `
 -TokenType bearer
```

- Convert the resource to JSON
```powershell
$AccessTokenResponse | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

