# DedicatedIPv4Address
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**DeviceId** | **String** | Linked device ID, or null if address is vacant | [optional] 
**Ip** | **String** | IP address | 

## Examples

- Prepare the resource
```powershell
$DedicatedIPv4Address = Initialize-PSAdGuardDNSDedicatedIPv4Address  -DeviceId b3e82cd1 `
 -Ip 94.140.14.15
```

- Convert the resource to JSON
```powershell
$DedicatedIPv4Address | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

