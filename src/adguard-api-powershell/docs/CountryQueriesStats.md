# CountryQueriesStats
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Country** | **String** | Country code | 
**Value** | [**QueriesStats**](QueriesStats.md) |  | 

## Examples

- Prepare the resource
```powershell
$CountryQueriesStats = Initialize-PSAdGuardDNSCountryQueriesStats  -Country US `
 -Value null
```

- Convert the resource to JSON
```powershell
$CountryQueriesStats | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

