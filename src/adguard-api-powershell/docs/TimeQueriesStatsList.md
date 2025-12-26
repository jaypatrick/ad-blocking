# TimeQueriesStatsList
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Stats** | [**TimeQueriesStats[]**](TimeQueriesStats.md) | List of queries stats | 

## Examples

- Prepare the resource
```powershell
$TimeQueriesStatsList = Initialize-PSAdGuardDNSTimeQueriesStatsList  -Stats null
```

- Convert the resource to JSON
```powershell
$TimeQueriesStatsList | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

