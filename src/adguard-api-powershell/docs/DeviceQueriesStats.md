# DeviceQueriesStats
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**DeviceId** | **String** | Device ID | 
**LastActivityTimeMillis** | **Int64** | Last activity time in millis | [optional] 
**Value** | [**QueriesStats**](QueriesStats.md) |  | 

## Examples

- Prepare the resource
```powershell
$DeviceQueriesStats = Initialize-PSAdGuardDNSDeviceQueriesStats  -DeviceId b3e82cd1 `
 -LastActivityTimeMillis null `
 -Value null
```

- Convert the resource to JSON
```powershell
$DeviceQueriesStats | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

