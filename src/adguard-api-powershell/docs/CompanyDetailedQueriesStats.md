# CompanyDetailedQueriesStats
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**CompanyName** | **String** | Company name | 
**DomainsCount** | **Int32** | Domains count | 
**TopBlockedDomain** | **String** | Top domain by blocked queries | [optional] 
**TopQueriesDomain** | **String** | Top domain by queries | 
**Value** | [**QueriesStats**](QueriesStats.md) |  | 

## Examples

- Prepare the resource
```powershell
$CompanyDetailedQueriesStats = Initialize-PSAdGuardDNSCompanyDetailedQueriesStats  -CompanyName Google `
 -DomainsCount 100 `
 -TopBlockedDomain advertising.google.com `
 -TopQueriesDomain ads.apple.com `
 -Value null
```

- Convert the resource to JSON
```powershell
$CompanyDetailedQueriesStats | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

