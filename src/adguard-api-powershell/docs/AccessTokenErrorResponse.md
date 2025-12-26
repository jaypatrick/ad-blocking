# AccessTokenErrorResponse
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**VarError** | **String** | Error type | [optional] 
**ErrorCode** | **String** | Error code | [optional] 
**ErrorDescription** | **String** | Error description | [optional] 

## Examples

- Prepare the resource
```powershell
$AccessTokenErrorResponse = Initialize-PSAdGuardDNSAccessTokenErrorResponse  -VarError unauthorized `
 -ErrorCode 2fa_required `
 -ErrorDescription 2FA is required
```

- Convert the resource to JSON
```powershell
$AccessTokenErrorResponse | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

