# QueryLogResponse
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Items** | [**QueryLogItem[]**](QueryLogItem.md) | Query log items | 
**Pages** | [**Page[]**](Page.md) | Pagination | 

## Examples

- Prepare the resource
```powershell
$QueryLogResponse = Initialize-PSAdGuardDNSQueryLogResponse  -Items null `
 -Pages null
```

- Convert the resource to JSON
```powershell
$QueryLogResponse | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

