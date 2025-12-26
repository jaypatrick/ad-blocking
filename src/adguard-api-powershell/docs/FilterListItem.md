# FilterListItem
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Enabled** | **Boolean** | The flag that the filter is on or off | 
**FilterId** | **String** | Filter identifier | 

## Examples

- Prepare the resource
```powershell
$FilterListItem = Initialize-PSAdGuardDNSFilterListItem  -Enabled null `
 -FilterId null
```

- Convert the resource to JSON
```powershell
$FilterListItem | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

