# IpAddress
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**IpAddress** | **String** | IP address string | 
**Type** | [**IpType**](IpType.md) |  | 

## Examples

- Prepare the resource
```powershell
$IpAddress = Initialize-PSAdGuardDNSIpAddress  -IpAddress null `
 -Type null
```

- Convert the resource to JSON
```powershell
$IpAddress | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

