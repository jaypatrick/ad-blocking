# DeviceSettings
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**DetectDohAuthOnly** | **Boolean** | Use only DNS-over-HTTPS with authentication | 
**ProtectionEnabled** | **Boolean** | Is protection enabled | 

## Examples

- Prepare the resource
```powershell
$DeviceSettings = Initialize-PSAdGuardDNSDeviceSettings  -DetectDohAuthOnly null `
 -ProtectionEnabled null
```

- Convert the resource to JSON
```powershell
$DeviceSettings | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

