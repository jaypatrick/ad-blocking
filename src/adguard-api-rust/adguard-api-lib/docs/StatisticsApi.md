# \StatisticsApi

All URIs are relative to *https://api.adguard-dns.io*

Method | HTTP request | Description
------------- | ------------- | -------------
[**get_categories_queries_stats**](StatisticsApi.md#get_categories_queries_stats) | **GET** /oapi/v1/stats/categories | Gets categories statistics
[**get_companies_stats**](StatisticsApi.md#get_companies_stats) | **GET** /oapi/v1/stats/companies | Gets companies statistics
[**get_countries_queries_stats**](StatisticsApi.md#get_countries_queries_stats) | **GET** /oapi/v1/stats/countries | Gets countries statistics
[**get_detailed_companies_stats**](StatisticsApi.md#get_detailed_companies_stats) | **GET** /oapi/v1/stats/companies/detailed | Gets detailed companies statistics
[**get_devices_queries_stats**](StatisticsApi.md#get_devices_queries_stats) | **GET** /oapi/v1/stats/devices | Gets devices statistics
[**get_domains_queries_stats**](StatisticsApi.md#get_domains_queries_stats) | **GET** /oapi/v1/stats/domains | Gets domains statistics
[**get_time_queries_stats**](StatisticsApi.md#get_time_queries_stats) | **GET** /oapi/v1/stats/time | Gets time statistics



## get_categories_queries_stats

> models::CategoryQueriesStatsList get_categories_queries_stats(time_from_millis, time_to_millis, devices, countries)
Gets categories statistics

### Parameters


Name | Type | Description  | Required | Notes
------------- | ------------- | ------------- | ------------- | -------------
**time_from_millis** | **i64** | Time from in milliseconds (inclusive) | [required] |
**time_to_millis** | **i64** | Time to in milliseconds (inclusive) | [required] |
**devices** | Option<[**Vec<String>**](String.md)> | Filter by devices |  |
**countries** | Option<[**Vec<String>**](String.md)> | Filter by countries |  |

### Return type

[**models::CategoryQueriesStatsList**](CategoryQueriesStatsList.md)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: */*, application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)


## get_companies_stats

> models::CompanyQueriesStatsList get_companies_stats(time_from_millis, time_to_millis, devices, countries)
Gets companies statistics

### Parameters


Name | Type | Description  | Required | Notes
------------- | ------------- | ------------- | ------------- | -------------
**time_from_millis** | **i64** | Time from in milliseconds (inclusive) | [required] |
**time_to_millis** | **i64** | Time to in milliseconds (inclusive) | [required] |
**devices** | Option<[**Vec<String>**](String.md)> | Filter by devices |  |
**countries** | Option<[**Vec<String>**](String.md)> | Filter by countries |  |

### Return type

[**models::CompanyQueriesStatsList**](CompanyQueriesStatsList.md)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: */*, application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)


## get_countries_queries_stats

> models::CountryQueriesStatsList get_countries_queries_stats(time_from_millis, time_to_millis, devices, countries)
Gets countries statistics

### Parameters


Name | Type | Description  | Required | Notes
------------- | ------------- | ------------- | ------------- | -------------
**time_from_millis** | **i64** | Time from in milliseconds (inclusive) | [required] |
**time_to_millis** | **i64** | Time to in milliseconds (inclusive) | [required] |
**devices** | Option<[**Vec<String>**](String.md)> | Filter by devices |  |
**countries** | Option<[**Vec<String>**](String.md)> | Filter by countries |  |

### Return type

[**models::CountryQueriesStatsList**](CountryQueriesStatsList.md)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: */*, application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)


## get_detailed_companies_stats

> models::CompanyDetailedQueriesStatsList get_detailed_companies_stats(time_from_millis, time_to_millis, devices, countries, cursor)
Gets detailed companies statistics

### Parameters


Name | Type | Description  | Required | Notes
------------- | ------------- | ------------- | ------------- | -------------
**time_from_millis** | **i64** | Time from in milliseconds (inclusive) | [required] |
**time_to_millis** | **i64** | Time to in milliseconds (inclusive) | [required] |
**devices** | Option<[**Vec<String>**](String.md)> | Filter by devices |  |
**countries** | Option<[**Vec<String>**](String.md)> | Filter by countries |  |
**cursor** | Option<**String**> | Pagination cursor |  |

### Return type

[**models::CompanyDetailedQueriesStatsList**](CompanyDetailedQueriesStatsList.md)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: */*, application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)


## get_devices_queries_stats

> models::DeviceQueriesStatsList get_devices_queries_stats(time_from_millis, time_to_millis, devices, countries)
Gets devices statistics

### Parameters


Name | Type | Description  | Required | Notes
------------- | ------------- | ------------- | ------------- | -------------
**time_from_millis** | **i64** | Time from in milliseconds (inclusive) | [required] |
**time_to_millis** | **i64** | Time to in milliseconds (inclusive) | [required] |
**devices** | Option<[**Vec<String>**](String.md)> | Filter by devices |  |
**countries** | Option<[**Vec<String>**](String.md)> | Filter by countries |  |

### Return type

[**models::DeviceQueriesStatsList**](DeviceQueriesStatsList.md)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: */*, application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)


## get_domains_queries_stats

> models::DomainQueriesStatsList get_domains_queries_stats(time_from_millis, time_to_millis, devices, countries)
Gets domains statistics

### Parameters


Name | Type | Description  | Required | Notes
------------- | ------------- | ------------- | ------------- | -------------
**time_from_millis** | **i64** | Time from in milliseconds (inclusive) | [required] |
**time_to_millis** | **i64** | Time to in milliseconds (inclusive) | [required] |
**devices** | Option<[**Vec<String>**](String.md)> | Filter by devices |  |
**countries** | Option<[**Vec<String>**](String.md)> | Filter by countries |  |

### Return type

[**models::DomainQueriesStatsList**](DomainQueriesStatsList.md)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: */*, application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)


## get_time_queries_stats

> models::TimeQueriesStatsList get_time_queries_stats(time_from_millis, time_to_millis, devices, countries)
Gets time statistics

### Parameters


Name | Type | Description  | Required | Notes
------------- | ------------- | ------------- | ------------- | -------------
**time_from_millis** | **i64** | Time from in milliseconds (inclusive) | [required] |
**time_to_millis** | **i64** | Time to in milliseconds (inclusive) | [required] |
**devices** | Option<[**Vec<String>**](String.md)> | Filter by devices |  |
**countries** | Option<[**Vec<String>**](String.md)> | Filter by countries |  |

### Return type

[**models::TimeQueriesStatsList**](TimeQueriesStatsList.md)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: */*, application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

