# SafebrowsingSettingsUpdate
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**BlockDangerousDomains** | **Boolean** | Whether filtering dangerous domains are enabled | [optional] 
**BlockNrd** | **Boolean** | Whether filtering newly registered domains are enabled | [optional] 
**Enabled** | **Boolean** | Whether safebrowsing settings are enabled | [optional] 

## Examples

- Prepare the resource
```powershell
$SafebrowsingSettingsUpdate = Initialize-PSAdGuardDNSSafebrowsingSettingsUpdate  -BlockDangerousDomains null `
 -BlockNrd null `
 -Enabled null
```

- Convert the resource to JSON
```powershell
$SafebrowsingSettingsUpdate | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

