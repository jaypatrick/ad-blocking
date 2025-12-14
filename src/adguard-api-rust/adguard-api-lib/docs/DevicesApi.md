# \DevicesApi

All URIs are relative to *https://api.adguard-dns.io*

Method | HTTP request | Description
------------- | ------------- | -------------
[**create_device**](DevicesApi.md#create_device) | **POST** /oapi/v1/devices | Creates a new device
[**get_device**](DevicesApi.md#get_device) | **GET** /oapi/v1/devices/{device_id} | Gets an existing device by ID
[**get_do_h_mobile_config**](DevicesApi.md#get_do_h_mobile_config) | **GET** /oapi/v1/devices/{device_id}/doh.mobileconfig | Gets DNS-over-HTTPS .mobileconfig file.
[**get_do_t_mobile_config**](DevicesApi.md#get_do_t_mobile_config) | **GET** /oapi/v1/devices/{device_id}/dot.mobileconfig | Gets DNS-over-TLS .mobileconfig file.
[**link_dedicated_ipv4_address**](DevicesApi.md#link_dedicated_ipv4_address) | **POST** /oapi/v1/devices/{device_id}/dedicated_addresses/ipv4 | Link dedicated IPv4 to the device
[**list_dedicated_addresses_for_device**](DevicesApi.md#list_dedicated_addresses_for_device) | **GET** /oapi/v1/devices/{device_id}/dedicated_addresses | List dedicated IPv4 and IPv6 addresses for a device
[**list_devices**](DevicesApi.md#list_devices) | **GET** /oapi/v1/devices | Lists devices
[**remove_device**](DevicesApi.md#remove_device) | **DELETE** /oapi/v1/devices/{device_id} | Removes a device
[**reset_doh_password**](DevicesApi.md#reset_doh_password) | **PUT** /oapi/v1/devices/{device_id}/doh_password/reset | Generate and set new DNS-over-HTTPS password
[**unlink_dedicated_ipv4_address**](DevicesApi.md#unlink_dedicated_ipv4_address) | **DELETE** /oapi/v1/devices/{device_id}/dedicated_addresses/ipv4 | Unlink dedicated IPv4 from the device
[**update_device**](DevicesApi.md#update_device) | **PUT** /oapi/v1/devices/{device_id} | Updates an existing device
[**update_device_settings**](DevicesApi.md#update_device_settings) | **PUT** /oapi/v1/devices/{device_id}/settings | Updates device settings



## create_device

> models::Device create_device(device_create)
Creates a new device

### Parameters


Name | Type | Description  | Required | Notes
------------- | ------------- | ------------- | ------------- | -------------
**device_create** | [**DeviceCreate**](DeviceCreate.md) |  | [required] |

### Return type

[**models::Device**](Device.md)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: */*, application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)


## get_device

> models::Device get_device(device_id)
Gets an existing device by ID

### Parameters


Name | Type | Description  | Required | Notes
------------- | ------------- | ------------- | ------------- | -------------
**device_id** | **String** |  | [required] |

### Return type

[**models::Device**](Device.md)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: */*

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)


## get_do_h_mobile_config

> get_do_h_mobile_config(device_id, exclude_wifi_networks, exclude_domain)
Gets DNS-over-HTTPS .mobileconfig file.

### Parameters


Name | Type | Description  | Required | Notes
------------- | ------------- | ------------- | ------------- | -------------
**device_id** | **String** |  | [required] |
**exclude_wifi_networks** | Option<[**Vec<String>**](String.md)> | List Wi-Fi networks by their SSID in which you want AdGuard DNS to be disabled |  |
**exclude_domain** | Option<[**Vec<String>**](String.md)> | List domains that will use default DNS servers instead of AdGuard DNS |  |

### Return type

 (empty response body)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)


## get_do_t_mobile_config

> get_do_t_mobile_config(device_id, exclude_wifi_networks, exclude_domain)
Gets DNS-over-TLS .mobileconfig file.

### Parameters


Name | Type | Description  | Required | Notes
------------- | ------------- | ------------- | ------------- | -------------
**device_id** | **String** |  | [required] |
**exclude_wifi_networks** | Option<[**Vec<String>**](String.md)> | List Wi-Fi networks by their SSID in which you want AdGuard DNS to be disabled |  |
**exclude_domain** | Option<[**Vec<String>**](String.md)> | List domains that will use default DNS servers instead of AdGuard DNS |  |

### Return type

 (empty response body)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)


## link_dedicated_ipv4_address

> link_dedicated_ipv4_address(device_id, link_dedicated_ipv4)
Link dedicated IPv4 to the device

### Parameters


Name | Type | Description  | Required | Notes
------------- | ------------- | ------------- | ------------- | -------------
**device_id** | **String** |  | [required] |
**link_dedicated_ipv4** | [**LinkDedicatedIpv4**](LinkDedicatedIpv4.md) |  | [required] |

### Return type

 (empty response body)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)


## list_dedicated_addresses_for_device

> models::DedicatedIps list_dedicated_addresses_for_device(device_id)
List dedicated IPv4 and IPv6 addresses for a device

### Parameters


Name | Type | Description  | Required | Notes
------------- | ------------- | ------------- | ------------- | -------------
**device_id** | **String** |  | [required] |

### Return type

[**models::DedicatedIps**](DedicatedIps.md)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: */*

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)


## list_devices

> Vec<models::Device> list_devices()
Lists devices

### Parameters

This endpoint does not need any parameter.

### Return type

[**Vec<models::Device>**](Device.md)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: */*

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)


## remove_device

> remove_device(device_id)
Removes a device

### Parameters


Name | Type | Description  | Required | Notes
------------- | ------------- | ------------- | ------------- | -------------
**device_id** | **String** |  | [required] |

### Return type

 (empty response body)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)


## reset_doh_password

> reset_doh_password(device_id)
Generate and set new DNS-over-HTTPS password

### Parameters


Name | Type | Description  | Required | Notes
------------- | ------------- | ------------- | ------------- | -------------
**device_id** | **String** |  | [required] |

### Return type

 (empty response body)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)


## unlink_dedicated_ipv4_address

> unlink_dedicated_ipv4_address(device_id, ip)
Unlink dedicated IPv4 from the device

### Parameters


Name | Type | Description  | Required | Notes
------------- | ------------- | ------------- | ------------- | -------------
**device_id** | **String** |  | [required] |
**ip** | **String** | Dedicated IPv4 | [required] |

### Return type

 (empty response body)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)


## update_device

> update_device(device_id, device_update)
Updates an existing device

### Parameters


Name | Type | Description  | Required | Notes
------------- | ------------- | ------------- | ------------- | -------------
**device_id** | **String** |  | [required] |
**device_update** | [**DeviceUpdate**](DeviceUpdate.md) |  | [required] |

### Return type

 (empty response body)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)


## update_device_settings

> update_device_settings(device_id, device_settings_update)
Updates device settings

### Parameters


Name | Type | Description  | Required | Notes
------------- | ------------- | ------------- | ------------- | -------------
**device_id** | **String** |  | [required] |
**device_settings_update** | [**DeviceSettingsUpdate**](DeviceSettingsUpdate.md) |  | [required] |

### Return type

 (empty response body)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

