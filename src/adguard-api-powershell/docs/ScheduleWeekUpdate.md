# ScheduleWeekUpdate
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**DailySchedule** | [**ScheduleDayUpdate[]**](ScheduleDayUpdate.md) | Schedule by days | 

## Examples

- Prepare the resource
```powershell
$ScheduleWeekUpdate = Initialize-PSAdGuardDNSScheduleWeekUpdate  -DailySchedule null
```

- Convert the resource to JSON
```powershell
$ScheduleWeekUpdate | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

