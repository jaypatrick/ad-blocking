# FilteringInfo
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**BlockedServiceId** | **String** | Web service ID | [optional] 
**FilterId** | **String** | Filter ID | [optional] 
**FilterRule** | **String** | Filter rule | [optional] 
**FilteringStatus** | [**FilteringActionStatus**](FilteringActionStatus.md) |  | [optional] 
**FilteringType** | [**FilteringActionSource**](FilteringActionSource.md) |  | [optional] 

## Examples

- Prepare the resource
```powershell
$FilteringInfo = Initialize-PSAdGuardDNSFilteringInfo  -BlockedServiceId instagram `
 -FilterId adguard_dns_filter `
 -FilterRule ||example.org^ `
 -FilteringStatus null `
 -FilteringType null
```

- Convert the resource to JSON
```powershell
$FilteringInfo | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

