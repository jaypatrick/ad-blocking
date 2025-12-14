# \DnsServersApi

All URIs are relative to *https://api.adguard-dns.io*

Method | HTTP request | Description
------------- | ------------- | -------------
[**create_dns_server**](DnsServersApi.md#create_dns_server) | **POST** /oapi/v1/dns_servers | Creates a new DNS server
[**get_dns_server**](DnsServersApi.md#get_dns_server) | **GET** /oapi/v1/dns_servers/{dns_server_id} | Gets an existing DNS server by ID
[**list_dns_servers**](DnsServersApi.md#list_dns_servers) | **GET** /oapi/v1/dns_servers | Lists DNS servers that belong to the user.
[**remove_dns_server**](DnsServersApi.md#remove_dns_server) | **DELETE** /oapi/v1/dns_servers/{dns_server_id} | Removes a DNS server
[**update_dns_server**](DnsServersApi.md#update_dns_server) | **PUT** /oapi/v1/dns_servers/{dns_server_id} | Updates an existing DNS server
[**update_dns_server_settings**](DnsServersApi.md#update_dns_server_settings) | **PUT** /oapi/v1/dns_servers/{dns_server_id}/settings | Updates DNS server settings



## create_dns_server

> models::DnsServer create_dns_server(dns_server_create)
Creates a new DNS server

Creates a new DNS server. You can attach custom settings, otherwise DNS server will be created with default settings.

### Parameters


Name | Type | Description  | Required | Notes
------------- | ------------- | ------------- | ------------- | -------------
**dns_server_create** | [**DnsServerCreate**](DnsServerCreate.md) |  | [required] |

### Return type

[**models::DnsServer**](DNSServer.md)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: */*, application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)


## get_dns_server

> models::DnsServer get_dns_server(dns_server_id)
Gets an existing DNS server by ID

### Parameters


Name | Type | Description  | Required | Notes
------------- | ------------- | ------------- | ------------- | -------------
**dns_server_id** | **String** |  | [required] |

### Return type

[**models::DnsServer**](DNSServer.md)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: */*

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)


## list_dns_servers

> Vec<models::DnsServer> list_dns_servers()
Lists DNS servers that belong to the user.

Lists DNS servers that belong to the user. By default there is at least one default server.

### Parameters

This endpoint does not need any parameter.

### Return type

[**Vec<models::DnsServer>**](DNSServer.md)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: */*

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)


## remove_dns_server

> remove_dns_server(dns_server_id)
Removes a DNS server

Removes a DNS server. All devices attached to this DNS server will be moved to the default DNS server. Deleting the default DNS server is forbidden.

### Parameters


Name | Type | Description  | Required | Notes
------------- | ------------- | ------------- | ------------- | -------------
**dns_server_id** | **String** |  | [required] |

### Return type

 (empty response body)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)


## update_dns_server

> update_dns_server(dns_server_id, dns_server_update)
Updates an existing DNS server

### Parameters


Name | Type | Description  | Required | Notes
------------- | ------------- | ------------- | ------------- | -------------
**dns_server_id** | **String** |  | [required] |
**dns_server_update** | [**DnsServerUpdate**](DnsServerUpdate.md) |  | [required] |

### Return type

 (empty response body)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)


## update_dns_server_settings

> update_dns_server_settings(dns_server_id, dns_server_settings_update)
Updates DNS server settings

### Parameters


Name | Type | Description  | Required | Notes
------------- | ------------- | ------------- | ------------- | -------------
**dns_server_id** | **String** |  | [required] |
**dns_server_settings_update** | [**DnsServerSettingsUpdate**](DnsServerSettingsUpdate.md) |  | [required] |

### Return type

 (empty response body)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

