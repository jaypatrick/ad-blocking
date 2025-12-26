# QueryLogItem
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Asn** | **Int32** | AS number | [optional] 
**CategoryType** | [**CategoryType**](CategoryType.md) |  | 
**ClientCountry** | **String** | Client country code | [optional] 
**CompanyId** | **String** | Company ID | 
**DeviceId** | **String** | Device ID | [optional] 
**DnsProtoType** | [**DnsProtoType**](DnsProtoType.md) |  | [optional] 
**DnsRequestType** | **String** | DNS protocol request type | [optional] 
**DnsResponseType** | [**DnsProtoResponseType**](DnsProtoResponseType.md) |  | [optional] 
**Dnssec** | **Boolean** | Requested with DNSSec | 
**Domain** | **String** | Domain name | 
**FilteringInfo** | [**FilteringInfo**](FilteringInfo.md) |  | 
**Network** | **String** | Network name | [optional] 
**ResponseCountry** | **String** | Response country code | [optional] 
**TimeIso** | **System.DateTime** | Event time | 
**TimeMillis** | **Int64** | Event time | 

## Examples

- Prepare the resource
```powershell
$QueryLogItem = Initialize-PSAdGuardDNSQueryLogItem  -Asn 25227 `
 -CategoryType null `
 -ClientCountry RU `
 -CompanyId google `
 -DeviceId b3e82cd1 `
 -DnsProtoType null `
 -DnsRequestType TypeA `
 -DnsResponseType null `
 -Dnssec false `
 -Domain ads.apple.com `
 -FilteringInfo null `
 -Network JSC Avantel `
 -ResponseCountry US `
 -TimeIso 2022-02-21T16:50:19.441+03:00 `
 -TimeMillis 1645451419441
```

- Convert the resource to JSON
```powershell
$QueryLogItem | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

