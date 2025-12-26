# DNSServerSettingsUpdate
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**AccessSettings** | [**DNSServerAccessSettingsUpdate**](DNSServerAccessSettingsUpdate.md) |  | [optional] 
**AutoConnectDevicesEnabled** | **Boolean** | Approval for auto-connecting devices through a specific link type | [optional] 
**BlockChromePrefetch** | **Boolean** | Block prefetch proxy in Google chrome | [optional] 
**BlockFirefoxCanary** | **Boolean** | If Firefox Canary should be blocked | [optional] 
**BlockPrivateRelay** | **Boolean** | Is private relay should be blocked | [optional] 
**BlockTtlSeconds** | **Int32** | TTL for blocked request | [optional] 
**BlockingModeSettings** | [**BlockingModeSettingsUpdate**](BlockingModeSettingsUpdate.md) |  | [optional] 
**FilterListsSettings** | [**FilterListsSettingsUpdate**](FilterListsSettingsUpdate.md) |  | [optional] 
**IpLogEnabled** | **Boolean** | Consent to log IP addresses of requests | [optional] 
**ParentalControlSettings** | [**ParentalControlSettingsUpdate**](ParentalControlSettingsUpdate.md) |  | [optional] 
**ProtectionEnabled** | **Boolean** | Is protection enabled | [optional] 
**SafebrowsingSettings** | [**SafebrowsingSettingsUpdate**](SafebrowsingSettingsUpdate.md) |  | [optional] 
**UserRulesSettings** | [**UserRulesSettingsUpdate**](UserRulesSettingsUpdate.md) |  | [optional] 

## Examples

- Prepare the resource
```powershell
$DNSServerSettingsUpdate = Initialize-PSAdGuardDNSDNSServerSettingsUpdate  -AccessSettings null `
 -AutoConnectDevicesEnabled null `
 -BlockChromePrefetch null `
 -BlockFirefoxCanary null `
 -BlockPrivateRelay null `
 -BlockTtlSeconds null `
 -BlockingModeSettings null `
 -FilterListsSettings null `
 -IpLogEnabled null `
 -ParentalControlSettings null `
 -ProtectionEnabled null `
 -SafebrowsingSettings null `
 -UserRulesSettings null
```

- Convert the resource to JSON
```powershell
$DNSServerSettingsUpdate | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

