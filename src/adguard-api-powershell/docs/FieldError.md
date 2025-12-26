# FieldError
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**ErrorCode** | [**ErrorCodes**](ErrorCodes.md) |  | 
**Field** | **String** | Field name | 
**Message** | **String** | Error message | [optional] 

## Examples

- Prepare the resource
```powershell
$FieldError = Initialize-PSAdGuardDNSFieldError  -ErrorCode null `
 -Field name `
 -Message Field is required
```

- Convert the resource to JSON
```powershell
$FieldError | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

