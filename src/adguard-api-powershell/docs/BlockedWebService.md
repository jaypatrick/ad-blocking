# BlockedWebService
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Enabled** | **Boolean** | Whether service blocking is enabled | 
**Id** | **String** | Web-service identifier | 

## Examples

- Prepare the resource
```powershell
$BlockedWebService = Initialize-PSAdGuardDNSBlockedWebService  -Enabled null `
 -Id 9gag
```

- Convert the resource to JSON
```powershell
$BlockedWebService | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

