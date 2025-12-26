# ParentalControlSettingsUpdate
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**BlockAdultWebsitesEnabled** | **Boolean** | Disable porno content | [optional] 
**BlockedServices** | [**BlockedWebServiceUpdate[]**](BlockedWebServiceUpdate.md) | List of services with restricted access | [optional] 
**Enabled** | **Boolean** | Is parental control enabled or not | [optional] 
**EnginesSafeSearchEnabled** | **Boolean** | Enforces safe search for some search engines | [optional] 
**ScreenTimeSchedule** | [**ScheduleWeekUpdate**](ScheduleWeekUpdate.md) |  | [optional] 
**YoutubeSafeSearchEnabled** | **Boolean** | Enforces safe search on YouTube | [optional] 

## Examples

- Prepare the resource
```powershell
$ParentalControlSettingsUpdate = Initialize-PSAdGuardDNSParentalControlSettingsUpdate  -BlockAdultWebsitesEnabled null `
 -BlockedServices null `
 -Enabled null `
 -EnginesSafeSearchEnabled null `
 -ScreenTimeSchedule null `
 -YoutubeSafeSearchEnabled null
```

- Convert the resource to JSON
```powershell
$ParentalControlSettingsUpdate | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

