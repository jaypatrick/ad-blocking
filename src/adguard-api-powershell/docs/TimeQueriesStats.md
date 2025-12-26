# TimeQueriesStats
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**TimeMillis** | **Int64** | Time in millis | 
**Value** | [**QueriesStats**](QueriesStats.md) |  | 

## Examples

- Prepare the resource
```powershell
$TimeQueriesStats = Initialize-PSAdGuardDNSTimeQueriesStats  -TimeMillis 1655804673 `
 -Value null
```

- Convert the resource to JSON
```powershell
$TimeQueriesStats | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

