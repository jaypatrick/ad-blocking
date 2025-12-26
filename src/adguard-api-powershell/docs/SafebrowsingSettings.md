# SafebrowsingSettings
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**BlockDangerousDomains** | **Boolean** | Whether filtering dangerous domains are enabled | 
**BlockNrd** | **Boolean** | Whether filtering newly registered domains are enabled | 
**Enabled** | **Boolean** | Whether safebrowsing settings are enabled | 

## Examples

- Prepare the resource
```powershell
$SafebrowsingSettings = Initialize-PSAdGuardDNSSafebrowsingSettings  -BlockDangerousDomains null `
 -BlockNrd null `
 -Enabled null
```

- Convert the resource to JSON
```powershell
$SafebrowsingSettings | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

