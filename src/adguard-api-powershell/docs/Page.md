# Page
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Current** | **Boolean** | Is current page | 
**PageCursor** | **String** | Pagination cursor | 
**PageNumber** | **Int32** | Page number | 

## Examples

- Prepare the resource
```powershell
$Page = Initialize-PSAdGuardDNSPage  -Current false `
 -PageCursor 1645451419441:1:20:3118 `
 -PageNumber 1
```

- Convert the resource to JSON
```powershell
$Page | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

