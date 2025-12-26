# ErrorResponse
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**ErrorCode** | [**ErrorCodes**](ErrorCodes.md) |  | 
**Fields** | [**FieldError[]**](FieldError.md) | Fields errors | 
**Message** | **String** | Error message | [optional] 

## Examples

- Prepare the resource
```powershell
$ErrorResponse = Initialize-PSAdGuardDNSErrorResponse  -ErrorCode null `
 -Fields null `
 -Message Validation failed
```

- Convert the resource to JSON
```powershell
$ErrorResponse | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

