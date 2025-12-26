# DedicatedIps
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Ipv4** | **String[]** | Dedicated IPv4 address | 
**Ipv4Limit** | [**Limit**](Limit.md) |  | 
**Ipv6** | **String[]** | Dedicated IPv6 address | 

## Examples

- Prepare the resource
```powershell
$DedicatedIps = Initialize-PSAdGuardDNSDedicatedIps  -Ipv4 null `
 -Ipv4Limit null `
 -Ipv6 null
```

- Convert the resource to JSON
```powershell
$DedicatedIps | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

