# ScheduleTime
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Hours** | **Int32** | Hour in day | 
**Minutes** | **Int32** | Minute in hour | 

## Examples

- Prepare the resource
```powershell
$ScheduleTime = Initialize-PSAdGuardDNSScheduleTime  -Hours null `
 -Minutes null
```

- Convert the resource to JSON
```powershell
$ScheduleTime | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

