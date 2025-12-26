# ScheduleDay
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**DayOfWeek** | [**DayOfWeek**](DayOfWeek.md) |  | 
**Enabled** | **Boolean** | Shows enabled/disabled day | 
**FromTime** | [**ScheduleTime**](ScheduleTime.md) |  | 
**ToTime** | [**ScheduleTime**](ScheduleTime.md) |  | 

## Examples

- Prepare the resource
```powershell
$ScheduleDay = Initialize-PSAdGuardDNSScheduleDay  -DayOfWeek null `
 -Enabled null `
 -FromTime null `
 -ToTime null
```

- Convert the resource to JSON
```powershell
$ScheduleDay | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

