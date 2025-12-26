# DNSServerSettings
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**AccessSettings** | [**DNSServerAccessSettings**](DNSServerAccessSettings.md) |  | 
**AutoConnectDevicesEnabled** | **Boolean** | Approval for auto-connecting devices through a specific link type | 
**BlockChromePrefetch** | **Boolean** | Block prefetch proxy in Google chrome | 
**BlockFirefoxCanary** | **Boolean** | If Firefox Canary should be blocked | 
**BlockPrivateRelay** | **Boolean** | Is private relay should be blocked | 
**BlockTtlSeconds** | **Int32** | TTL for blocked request | 
**BlockingModeSettings** | [**BlockingModeSettings**](BlockingModeSettings.md) |  | 
**FilterListsSettings** | [**FilterListsSettings**](FilterListsSettings.md) |  | 
**IpLogEnabled** | **Boolean** | Consent to log IP addresses of requests | 
**ParentalControlSettings** | [**ParentalControlSettings**](ParentalControlSettings.md) |  | 
**ProtectionEnabled** | **Boolean** | Is protection enabled | 
**SafebrowsingSettings** | [**SafebrowsingSettings**](SafebrowsingSettings.md) |  | 
**UserRulesSettings** | [**UserRulesSettings**](UserRulesSettings.md) |  | 

## Examples

- Prepare the resource
```powershell
$DNSServerSettings = Initialize-PSAdGuardDNSDNSServerSettings  -AccessSettings null `
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
$DNSServerSettings | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

