# CategoryQueriesStats
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**CategoryType** | [**CategoryType**](CategoryType.md) |  | 
**Queries** | **Int64** | Queries | 

## Examples

- Prepare the resource
```powershell
$CategoryQueriesStats = Initialize-PSAdGuardDNSCategoryQueriesStats  -CategoryType null `
 -Queries null
```

- Convert the resource to JSON
```powershell
$CategoryQueriesStats | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

