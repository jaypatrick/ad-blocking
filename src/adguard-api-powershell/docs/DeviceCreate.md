# DeviceCreate
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**DeviceType** | [**ConnectDeviceType**](ConnectDeviceType.md) |  | 
**DnsServerId** | **String** | DNS server ID | 
**Name** | **String** | Device name | 

## Examples

- Prepare the resource
```powershell
$DeviceCreate = Initialize-PSAdGuardDNSDeviceCreate  -DeviceType null `
 -DnsServerId a9f29be1 `
 -Name My android
```

- Convert the resource to JSON
```powershell
$DeviceCreate | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

