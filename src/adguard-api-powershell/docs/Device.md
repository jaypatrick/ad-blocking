# Device
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**DeviceType** | [**ConnectDeviceType**](ConnectDeviceType.md) |  | 
**DnsAddresses** | [**DNSAddresses**](DNSAddresses.md) |  | 
**DnsServerId** | **String** | DNS server ID | 
**Id** | **String** | Device ID | 
**Name** | **String** | Device name | 
**Settings** | [**DeviceSettings**](DeviceSettings.md) |  | 

## Examples

- Prepare the resource
```powershell
$Device = Initialize-PSAdGuardDNSDevice  -DeviceType null `
 -DnsAddresses null `
 -DnsServerId a9f29be1 `
 -Id b3e82cd1 `
 -Name My iphone `
 -Settings null
```

- Convert the resource to JSON
```powershell
$Device | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

