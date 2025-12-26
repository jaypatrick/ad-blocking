# DNSAddresses
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**AdguardDnsOverHttpsUrl** | **String** | AdGuard DNS-over-HTTPS url | 
**AdguardDnsOverHttpsWithAuthUrl** | **String** | AdGuard DNS-over-HTTPS with authentication url | [optional] 
**AdguardDnsOverQuicUrl** | **String** | AdGuard DNS-over-QUIC url | 
**AdguardDnsOverTlsUrl** | **String** | AdGuard DNS-over-TLS url | 
**AdguardVpnDnsOverHttpsUrl** | **String** | AdGuard VPN DNS-over-HTTPS url | 
**AdguardVpnDnsOverHttpsWithAuthUrl** | **String** | AdGuard VPN DNS-over-HTTPS with authentication url | [optional] 
**AdguardVpnDnsOverQuicUrl** | **String** | AdGuard VPN DNS-over-QUIC url | 
**AdguardVpnDnsOverTlsUrl** | **String** | AdGuard VPN DNS-over-TLS url | 
**DnsOverHttpsUrl** | **String** | DNS-over-HTTPS | 
**DnsOverHttpsWithAuthUrl** | **String** | DNS-over-HTTPS with authentication | [optional] 
**DnsOverQuicUrl** | **String** | DNS-over-QUIC | 
**DnsOverTlsUrl** | **String** | DNS-over-TLS | 
**IpAddresses** | [**IpAddress[]**](IpAddress.md) | IP addresses | [optional] 

## Examples

- Prepare the resource
```powershell
$DNSAddresses = Initialize-PSAdGuardDNSDNSAddresses  -AdguardDnsOverHttpsUrl adguard:add_dns_server?address&#x3D;https%3A%2F%2Ff3750181.d.adguard.ch%2Fdns-query%26name%3DAdGuard%20Personal%20DNS `
 -AdguardDnsOverHttpsWithAuthUrl adguard:add_dns_server?address&#x3D;https%3A%2F%2Ff3750181:jaNh9iXS@d.adguard.ch%2Fdns-query%26name%3DAdGuard%20Personal%20DNS `
 -AdguardDnsOverQuicUrl adguard:add_dns_server?address&#x3D;quic%3A%2F%2Ff3750181.d.adguard.ch%2Fdns-query%26name%3DAdGuard%20Personal%20DNS `
 -AdguardDnsOverTlsUrl adguard:add_dns_server?address&#x3D;tls%3A%2F%2Ff3750181.d.adguard.ch%2Fdns-query%26name%3DAdGuard%20Personal%20DNS `
 -AdguardVpnDnsOverHttpsUrl adguardvpn:add_dns_server?address&#x3D;https%3A%2F%2Ff3750181.d.adguard.ch%2Fdns-query%26name%3DAdGuard%20Personal%20DNS `
 -AdguardVpnDnsOverHttpsWithAuthUrl adguardvpn:add_dns_server?address&#x3D;https%3A%2F%2Ff3750181:jaNh9iXS@.d.adguard.ch%2Fdns-query%26name%3DAdGuard%20Personal%20DNS `
 -AdguardVpnDnsOverQuicUrl adguardvpn:add_dns_server?address&#x3D;quic%3A%2F%2Ff3750181.d.adguard.ch%2Fdns-query%26name%3DAdGuard%20Personal%20DNS `
 -AdguardVpnDnsOverTlsUrl adguardvpn:add_dns_server?address&#x3D;tls%3A%2F%2Ff3750181.d.adguard.ch%2Fdns-query%26name%3DAdGuard%20Personal%20DNS `
 -DnsOverHttpsUrl https://b3e82cd1.adguard-dns.com/dns-query `
 -DnsOverHttpsWithAuthUrl https://b3e82cd1:jaNh9iXS@d.adguard-dns.com/dns-query `
 -DnsOverQuicUrl quic://b3e82cd1.adguard-dns.com `
 -DnsOverTlsUrl tls://b3e82cd1.adguard-dns.com `
 -IpAddresses null
```

- Convert the resource to JSON
```powershell
$DNSAddresses | ConvertTo-JSON
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

