# BlockingModeSettings
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**BlockingMode** | [**BlockingMode**](BlockingMode.md) |  | 
**Ipv4BlockingAddress** | **String** | Custom IPv4 address for blocking mode CUSTOM IP | [optional] 
**Ipv6BlockingAddress** | **String** | Custom IPv6 address for blocking mode CUSTOM IP | [optional] 

## Examples

- Prepare the resource
```powershell
$BlockingModeSettings = Initialize-PSAdGuardDNSBlockingModeSettings  -BlockingMode null `
 -Ipv4BlockingAddress null `
 -Ipv6BlockingAddress null
```

- Convert the resource to JSON
```powershell
$BlockingModeSettings | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

