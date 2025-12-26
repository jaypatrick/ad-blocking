# CompanyQueriesStats
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**CompanyName** | **String** | Company name | 
**Value** | [**QueriesStats**](QueriesStats.md) |  | 

## Examples

- Prepare the resource
```powershell
$CompanyQueriesStats = Initialize-PSAdGuardDNSCompanyQueriesStats  -CompanyName Google `
 -Value null
```

- Convert the resource to JSON
```powershell
$CompanyQueriesStats | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

