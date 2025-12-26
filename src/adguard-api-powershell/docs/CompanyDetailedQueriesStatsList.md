# CompanyDetailedQueriesStatsList
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Pages** | [**Page[]**](Page.md) | Pagination | 
**Stats** | [**CompanyDetailedQueriesStats[]**](CompanyDetailedQueriesStats.md) | List of queries stats | 

## Examples

- Prepare the resource
```powershell
$CompanyDetailedQueriesStatsList = Initialize-PSAdGuardDNSCompanyDetailedQueriesStatsList  -Pages null `
 -Stats null
```

- Convert the resource to JSON
```powershell
$CompanyDetailedQueriesStatsList | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

