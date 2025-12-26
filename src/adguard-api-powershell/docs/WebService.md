# WebService
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**IconSvg** | **String** | SVG icon | 
**Id** | **String** | Web-service identifier | 
**Name** | **String** | Web-service name | 

## Examples

- Prepare the resource
```powershell
$WebService = Initialize-PSAdGuardDNSWebService  -IconSvg &lt;svg&gt;&lt;path d&#x3D;&quot;M 44 14 C 44 13...&quot; /&gt;&lt;/svg&gt; `
 -Id 9gag `
 -Name 9GAG
```

- Convert the resource to JSON
```powershell
$WebService | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

