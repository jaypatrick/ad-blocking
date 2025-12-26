# FilterListsSettingsUpdate
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Enabled** | **Boolean** | The flag that all filters are on or off | [optional] 
**FilterList** | [**FilterListItemUpdate[]**](FilterListItemUpdate.md) | Filter list | [optional] 

## Examples

- Prepare the resource
```powershell
$FilterListsSettingsUpdate = Initialize-PSAdGuardDNSFilterListsSettingsUpdate  -Enabled null `
 -FilterList null
```

- Convert the resource to JSON
```powershell
$FilterListsSettingsUpdate | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

