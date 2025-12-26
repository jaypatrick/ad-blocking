# DomainQueriesStatsList
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Stats** | [**DomainQueriesStats[]**](DomainQueriesStats.md) | List of queries stats | 

## Examples

- Prepare the resource
```powershell
$DomainQueriesStatsList = Initialize-PSAdGuardDNSDomainQueriesStatsList  -Stats null
```

- Convert the resource to JSON
```powershell
$DomainQueriesStatsList | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

