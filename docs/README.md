# Documentation

This directory contains documentation for the ad-blocking repository.

## Contents

### API Reference (`api/`)

Auto-generated API documentation for the AdGuard DNS API Client.

#### API Endpoints

| API Class | Description |
|-----------|-------------|
| [AccountApi](api/AccountApi.md) | Account limits and information |
| [AuthenticationApi](api/AuthenticationApi.md) | OAuth token generation |
| [DevicesApi](api/DevicesApi.md) | Device management (CRUD) |
| [DNSServersApi](api/DNSServersApi.md) | DNS server profile management |
| [DedicatedIPAddressesApi](api/DedicatedIPAddressesApi.md) | Dedicated IPv4 management |
| [FilterListsApi](api/FilterListsApi.md) | Filter list retrieval |
| [QueryLogApi](api/QueryLogApi.md) | Query log operations |
| [StatisticsApi](api/StatisticsApi.md) | DNS query statistics |
| [WebServicesApi](api/WebServicesApi.md) | Web services for blocking |

#### Data Models

| Model | Description |
|-------|-------------|
| [AccessTokenResponse](api/AccessTokenResponse.md) | OAuth token response |
| [AccessTokenErrorResponse](api/AccessTokenErrorResponse.md) | OAuth error response |
| [AccountLimits](api/AccountLimits.md) | Account usage limits |
| [Device](api/Device.md) | Device information |
| [DeviceCreate](api/DeviceCreate.md) | Device creation request |
| [DeviceUpdate](api/DeviceUpdate.md) | Device update request |
| [DNSServer](api/DNSServer.md) | DNS server profile |
| [DNSServerCreate](api/DNSServerCreate.md) | DNS server creation request |
| [DedicatedIPv4Address](api/DedicatedIPv4Address.md) | Dedicated IPv4 address |
| [ErrorResponse](api/ErrorResponse.md) | API error response |
| [FilterList](api/FilterList.md) | Filter list information |
| [Limit](api/Limit.md) | Usage limit details |
| [QueryLogResponse](api/QueryLogResponse.md) | Query log data |
| [TimeQueriesStatsList](api/TimeQueriesStatsList.md) | Time-based statistics |
| [WebService](api/WebService.md) | Web service for blocking |

### Usage Guides (`guides/`)

| Guide | Description |
|-------|-------------|
| [API Client Usage Guide](guides/api-client-usage.md) | Detailed usage instructions |
| [API Client Examples](guides/api-client-examples.md) | Code examples with helper classes |

## Quick Links

- [Main README](../README.md) - Project overview
- [API Client README](../src/adguard-api-client/README.md) - API client overview
- [Security Policy](../SECURITY.md) - Security guidelines

<a id="documentation-for-api-endpoints"></a>
## Documentation for API Endpoints

All URIs are relative to *https://api.adguard-dns.io*

| Class | Method | HTTP request | Description |
|-------|--------|--------------|-------------|
| *AccountApi* | **GetAccountLimits** | **GET** /oapi/v1/account/limits | Gets account limits |
| *AuthenticationApi* | **AccessToken** | **POST** /oapi/v1/oauth_token | Generates tokens |
| *DNSServersApi* | **CreateDNSServer** | **POST** /oapi/v1/dns_servers | Creates DNS server |
| *DNSServersApi* | **ListDNSServers** | **GET** /oapi/v1/dns_servers | Lists DNS servers |
| *DedicatedIPAddressesApi* | **AllocateDedicatedIPv4Address** | **POST** /oapi/v1/dedicated_addresses/ipv4 | Allocates IPv4 |
| *DedicatedIPAddressesApi* | **ListDedicatedIPv4Addresses** | **GET** /oapi/v1/dedicated_addresses/ipv4 | Lists IPv4 addresses |
| *DevicesApi* | **CreateDevice** | **POST** /oapi/v1/devices | Creates device |
| *DevicesApi* | **GetDevice** | **GET** /oapi/v1/devices/{device_id} | Gets device |
| *DevicesApi* | **ListDevices** | **GET** /oapi/v1/devices | Lists devices |
| *DevicesApi* | **RemoveDevice** | **DELETE** /oapi/v1/devices/{device_id} | Removes device |
| *DevicesApi* | **UpdateDevice** | **PUT** /oapi/v1/devices/{device_id} | Updates device |
| *FilterListsApi* | **ListFilterLists** | **GET** /oapi/v1/filter_lists | Gets filter lists |
| *QueryLogApi* | **ClearQueryLog** | **DELETE** /oapi/v1/query_log | Clears query log |
| *QueryLogApi* | **GetQueryLog** | **GET** /oapi/v1/query_log | Gets query log |
| *StatisticsApi* | **GetTimeQueriesStats** | **GET** /oapi/v1/stats/time | Gets statistics |
| *WebServicesApi* | **ListWebServices** | **GET** /oapi/v1/web_services | Lists web services |

<a id="documentation-for-models"></a>
## Documentation for Models

- [AccessTokenErrorResponse](api/AccessTokenErrorResponse.md)
- [AccessTokenResponse](api/AccessTokenResponse.md)
- [AccountLimits](api/AccountLimits.md)
- [DNSServer](api/DNSServer.md)
- [DNSServerCreate](api/DNSServerCreate.md)
- [DedicatedIPv4Address](api/DedicatedIPv4Address.md)
- [Device](api/Device.md)
- [DeviceCreate](api/DeviceCreate.md)
- [DeviceUpdate](api/DeviceUpdate.md)
- [ErrorResponse](api/ErrorResponse.md)
- [FilterList](api/FilterList.md)
- [Limit](api/Limit.md)
- [QueryLogResponse](api/QueryLogResponse.md)
- [TimeQueriesStatsList](api/TimeQueriesStatsList.md)
- [WebService](api/WebService.md)

<a id="ApiKey"></a>
## Authentication: ApiKey

- **Type**: API key
- **API key parameter name**: Authorization
- **Location**: HTTP header

<a id="AuthToken"></a>
## Authentication: AuthToken

- **Type**: Bearer Authentication
