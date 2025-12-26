# DNSServerCreate
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Name** | **String** | DNS server name | 
**Settings** | [**DNSServerSettingsUpdate**](DNSServerSettingsUpdate.md) |  | [optional] 

## Examples

- Prepare the resource
```powershell
$DNSServerCreate = Initialize-PSAdGuardDNSDNSServerCreate  -Name My profile `
 -Settings null
```

- Convert the resource to JSON
```powershell
$DNSServerCreate | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

