# UserRulesSettings
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Enabled** | **Boolean** | The flag that all rules are on or off | 
**Rules** | **String[]** | List of rules | 
**RulesCount** | **Int32** | Rules count in user list | 

## Examples

- Prepare the resource
```powershell
$UserRulesSettings = Initialize-PSAdGuardDNSUserRulesSettings  -Enabled null `
 -Rules null `
 -RulesCount null
```

- Convert the resource to JSON
```powershell
$UserRulesSettings | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

