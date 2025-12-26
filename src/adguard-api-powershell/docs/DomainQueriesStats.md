# DomainQueriesStats
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Domain** | **String** | Domain name | 
**Value** | [**QueriesStats**](QueriesStats.md) |  | 

## Examples

- Prepare the resource
```powershell
$DomainQueriesStats = Initialize-PSAdGuardDNSDomainQueriesStats  -Domain facebook.com `
 -Value null
```

- Convert the resource to JSON
```powershell
$DomainQueriesStats | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

