# ScheduleDayUpdate
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**DayOfWeek** | [**DayOfWeek**](DayOfWeek.md) |  | 
**Enabled** | **Boolean** | Shows enabled/disabled day | [optional] 
**FromTime** | [**ScheduleTime**](ScheduleTime.md) |  | [optional] 
**ToTime** | [**ScheduleTime**](ScheduleTime.md) |  | [optional] 

## Examples

- Prepare the resource
```powershell
$ScheduleDayUpdate = Initialize-PSAdGuardDNSScheduleDayUpdate  -DayOfWeek null `
 -Enabled null `
 -FromTime null `
 -ToTime null
```

- Convert the resource to JSON
```powershell
$ScheduleDayUpdate | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

