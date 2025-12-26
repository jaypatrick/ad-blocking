# DeviceUpdate
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**DeviceType** | [**ConnectDeviceType**](ConnectDeviceType.md) |  | [optional] 
**DnsServerId** | **String** | DNS server ID | [optional] 
**Name** | **String** | Device name | [optional] 

## Examples

- Prepare the resource
```powershell
$DeviceUpdate = Initialize-PSAdGuardDNSDeviceUpdate  -DeviceType null `
 -DnsServerId a9f29be1 `
 -Name My android
```

- Convert the resource to JSON
```powershell
$DeviceUpdate | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

