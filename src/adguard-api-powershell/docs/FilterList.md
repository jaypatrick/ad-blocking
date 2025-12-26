# FilterList
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Categories** | [**FilterListCategory[]**](FilterListCategory.md) | Filter categories | 
**Description** | **String** | Filter description | 
**DownloadUrl** | **String** | Filter download url | 
**FilterId** | **String** | Filter ID | 
**HomepageUrl** | **String** | Filter homepage url | 
**Name** | **String** | Filter name | 
**RulesCount** | **Int32** | Rules count in filter | 
**SourceUrl** | **String** | Filter source url | 
**Tags** | **String[]** | Filter tags | 
**TimeUpdated** | **System.DateTime** | Filter last updated time | 

## Examples

- Prepare the resource
```powershell
$FilterList = Initialize-PSAdGuardDNSFilterList  -Categories null `
 -Description Filter composed of several other filters (AdGuard Base filter, Social Media filter)... `
 -DownloadUrl https://adguardteam.github.io/HostlistsRegistry/assets/filter_1.txt `
 -FilterId adguard_dns_filter `
 -HomepageUrl https://github.com/AdguardTeam/AdGuardSDNSFilter `
 -Name AdGuard DNS filter `
 -RulesCount 10000 `
 -SourceUrl https://adguardteam.github.io/AdGuardSDNSFilter/Filters/filter.txt `
 -Tags [&quot;purpose:general&quot;] `
 -TimeUpdated null
```

- Convert the resource to JSON
```powershell
$FilterList | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

