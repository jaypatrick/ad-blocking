# ParentalControlSettings
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**BlockAdultWebsitesEnabled** | **Boolean** | Disable porno content | 
**BlockedServices** | [**BlockedWebService[]**](BlockedWebService.md) | List of services with restricted access | 
**Enabled** | **Boolean** | Is parental control enabled or not | 
**EnginesSafeSearchEnabled** | **Boolean** | Enforces safe search for some search engines | 
**ScreenTimeSchedule** | [**ScheduleWeek**](ScheduleWeek.md) |  | 
**YoutubeSafeSearchEnabled** | **Boolean** | Enforces safe search on YouTube | 

## Examples

- Prepare the resource
```powershell
$ParentalControlSettings = Initialize-PSAdGuardDNSParentalControlSettings  -BlockAdultWebsitesEnabled null `
 -BlockedServices null `
 -Enabled null `
 -EnginesSafeSearchEnabled null `
 -ScreenTimeSchedule null `
 -YoutubeSafeSearchEnabled null
```

- Convert the resource to JSON
```powershell
$ParentalControlSettings | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

