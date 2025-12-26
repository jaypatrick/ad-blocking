# AccountLimits
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**AccessRules** | [**Limit**](Limit.md) |  | 
**DedicatedIpv4** | [**Limit**](Limit.md) |  | 
**Devices** | [**Limit**](Limit.md) |  | 
**DnsServers** | [**Limit**](Limit.md) |  | 
**Requests** | [**Limit**](Limit.md) |  | 
**UserRules** | [**Limit**](Limit.md) |  | 

## Examples

- Prepare the resource
```powershell
$AccountLimits = Initialize-PSAdGuardDNSAccountLimits  -AccessRules null `
 -DedicatedIpv4 null `
 -Devices null `
 -DnsServers null `
 -Requests null `
 -UserRules null
```

- Convert the resource to JSON
```powershell
$AccountLimits | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

