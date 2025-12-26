# ScheduleWeek
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**DailySchedule** | [**ScheduleDay[]**](ScheduleDay.md) | Schedule by days | 

## Examples

- Prepare the resource
```powershell
$ScheduleWeek = Initialize-PSAdGuardDNSScheduleWeek  -DailySchedule null
```

- Convert the resource to JSON
```powershell
$ScheduleWeek | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

