# CountryQueriesStatsList
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Stats** | [**CountryQueriesStats[]**](CountryQueriesStats.md) | List of queries stats | 

## Examples

- Prepare the resource
```powershell
$CountryQueriesStatsList = Initialize-PSAdGuardDNSCountryQueriesStatsList  -Stats null
```

- Convert the resource to JSON
```powershell
$CountryQueriesStatsList | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

