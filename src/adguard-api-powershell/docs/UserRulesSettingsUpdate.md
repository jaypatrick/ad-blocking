# UserRulesSettingsUpdate
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Enabled** | **Boolean** | The flag that all rules are on or off | [optional] 
**Rules** | **String[]** | List of rules | [optional] 

## Examples

- Prepare the resource
```powershell
$UserRulesSettingsUpdate = Initialize-PSAdGuardDNSUserRulesSettingsUpdate  -Enabled null `
 -Rules null
```

- Convert the resource to JSON
```powershell
$UserRulesSettingsUpdate | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

