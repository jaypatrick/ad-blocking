# \QueryLogApi

All URIs are relative to *https://api.adguard-dns.io*

Method | HTTP request | Description
------------- | ------------- | -------------
[**clear_query_log**](QueryLogApi.md#clear_query_log) | **DELETE** /oapi/v1/query_log | Clears query log
[**get_query_log**](QueryLogApi.md#get_query_log) | **GET** /oapi/v1/query_log | Gets query log



## clear_query_log

> clear_query_log()
Clears query log

### Parameters

This endpoint does not need any parameter.

### Return type

 (empty response body)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)


## get_query_log

> models::QueryLogResponse get_query_log(time_from_millis, time_to_millis, devices, countries, companies, statuses, categories, search, limit, cursor)
Gets query log

### Parameters


Name | Type | Description  | Required | Notes
------------- | ------------- | ------------- | ------------- | -------------
**time_from_millis** | **i64** | Time from in milliseconds (inclusive) | [required] |
**time_to_millis** | **i64** | Time to in milliseconds (inclusive) | [required] |
**devices** | Option<[**Vec<String>**](String.md)> | Filter by devices |  |
**countries** | Option<[**Vec<String>**](String.md)> | Filter by countries |  |
**companies** | Option<[**Vec<String>**](String.md)> | Filter by companies |  |
**statuses** | Option<[**Vec<models::FilteringActionStatus>**](models::FilteringActionStatus.md)> | Filter by statuses |  |
**categories** | Option<[**Vec<models::CategoryType>**](models::CategoryType.md)> | Filter by categories |  |
**search** | Option<**String**> | Filter by domain name |  |
**limit** | Option<**i32**> | Limit the number of records to be returned |  |[default to 20]
**cursor** | Option<**String**> | Pagination cursor. Use cursor from response to paginate through the pages. |  |

### Return type

[**models::QueryLogResponse**](QueryLogResponse.md)

### Authorization

[ApiKey](../README.md#ApiKey), [AuthToken](../README.md#AuthToken)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: */*

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

