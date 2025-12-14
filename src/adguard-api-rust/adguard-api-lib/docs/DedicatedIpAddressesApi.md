# \DedicatedIpAddressesApi

All URIs are relative to *https://api.adguard-dns.io*

Method | HTTP request | Description
------------- | ------------- | -------------
[**allocate_dedicated_ipv4_address**](DedicatedIpAddressesApi.md#allocate_dedicated_ipv4_address) | **POST** /oapi/v1/dedicated_addresses/ipv4 | Allocates new dedicated IPv4
[**list_dedicated_ipv4_addresses**](DedicatedIpAddressesApi.md#list_dedicated_ipv4_addresses) | **GET** /oapi/v1/dedicated_addresses/ipv4 | Lists allocated dedicated IPv4 addresses



## allocate_dedicated_ipv4_address

> models::DedicatedIpv4Address allocate_dedicated_ipv4_address()
Allocates new dedicated IPv4

### Parameters

This endpoint does not need any parameter.

### Return type

[**models::DedicatedIpv4Address**](DedicatedIPv4Address.md)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: */*

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)


## list_dedicated_ipv4_addresses

> Vec<models::DedicatedIpv4Address> list_dedicated_ipv4_addresses()
Lists allocated dedicated IPv4 addresses

### Parameters

This endpoint does not need any parameter.

### Return type

[**Vec<models::DedicatedIpv4Address>**](DedicatedIPv4Address.md)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: */*

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

