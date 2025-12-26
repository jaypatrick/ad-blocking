# FilterListsSettings
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Enabled** | **Boolean** | The flag that all filters are on or off | 
**FilterList** | [**FilterListItem[]**](FilterListItem.md) | Filter list | 

## Examples

- Prepare the resource
```powershell
$FilterListsSettings = Initialize-PSAdGuardDNSFilterListsSettings  -Enabled null `
 -FilterList null
```

- Convert the resource to JSON
```powershell
$FilterListsSettings | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

