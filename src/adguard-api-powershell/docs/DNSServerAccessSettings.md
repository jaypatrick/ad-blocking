# DNSServerAccessSettings
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**AllowedClients** | **String[]** | Allowed IP&#39;s, CIDR&#39;s or ASN&#39;s | 
**BlockKnownScanners** | **Boolean** | If known scanners should be blocked | 
**BlockedClients** | **String[]** | Blocked IP&#39;s, CIDR&#39;s or ASN&#39;s | 
**BlockedDomainRules** | **String[]** | Blocked domain rules | 
**Enabled** | **Boolean** | Flag that access settings are enabled | 

## Examples

- Prepare the resource
```powershell
$DNSServerAccessSettings = Initialize-PSAdGuardDNSDNSServerAccessSettings  -AllowedClients null `
 -BlockKnownScanners null `
 -BlockedClients null `
 -BlockedDomainRules null `
 -Enabled null
```

- Convert the resource to JSON
```powershell
$DNSServerAccessSettings | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

