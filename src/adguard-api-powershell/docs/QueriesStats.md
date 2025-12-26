# QueriesStats
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Blocked** | **Int64** | Blocked queries count | 
**Companies** | **Int32** | Companies count | 
**Queries** | **Int64** | Overall queries count | 

## Examples

- Prepare the resource
```powershell
$QueriesStats = Initialize-PSAdGuardDNSQueriesStats  -Blocked 14 `
 -Companies 4 `
 -Queries 86
```

- Convert the resource to JSON
```powershell
$QueriesStats | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

