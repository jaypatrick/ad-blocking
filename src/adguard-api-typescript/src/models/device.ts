/**
 * Device-related models
 */

import { DeviceType } from './enums.js';

/** IP address info */
export interface IpAddress {
  /** IP address string */
  ip_address: string;
  /** IP type (IPv4/IPv6) */
  type: string;
}

/** DNS connection addresses */
export interface DNSAddresses {
  /** DNS-over-HTTPS URL */
  dns_over_https_url: string;
  /** DNS-over-HTTPS with authentication URL */
  dns_over_https_with_auth_url?: string;
  /** DNS-over-TLS URL */
  dns_over_tls_url: string;
  /** DNS-over-QUIC URL */
  dns_over_quic_url: string;
  /** AdGuard DNS-over-HTTPS URL (app link) */
  adguard_dns_over_https_url: string;
  /** AdGuard DNS-over-HTTPS with authentication URL */
  adguard_dns_over_https_with_auth_url?: string;
  /** AdGuard DNS-over-TLS URL (app link) */
  adguard_dns_over_tls_url: string;
  /** AdGuard DNS-over-QUIC URL (app link) */
  adguard_dns_over_quic_url: string;
  /** AdGuard VPN DNS-over-HTTPS URL */
  adguard_vpn_dns_over_https_url: string;
  /** AdGuard VPN DNS-over-HTTPS with authentication URL */
  adguard_vpn_dns_over_https_with_auth_url?: string;
  /** AdGuard VPN DNS-over-TLS URL */
  adguard_vpn_dns_over_tls_url: string;
  /** AdGuard VPN DNS-over-QUIC URL */
  adguard_vpn_dns_over_quic_url: string;
  /** IP addresses */
  ip_addresses?: IpAddress[];
}

/** Device settings */
export interface DeviceSettings {
  /** Is protection enabled */
  protection_enabled: boolean;
  /** Use only DNS-over-HTTPS with authentication */
  detect_doh_auth_only: boolean;
}

/** Device settings update */
export interface DeviceSettingsUpdate {
  /** Enable protection */
  protection_enabled?: boolean;
  /** Use only DNS-over-HTTPS with authentication */
  detect_doh_auth_only?: boolean;
}

/** Device entity */
export interface Device {
  /** Device ID */
  id: string;
  /** Device name */
  name: string;
  /** Device type */
  device_type: DeviceType;
  /** DNS server ID */
  dns_server_id: string;
  /** DNS addresses */
  dns_addresses: DNSAddresses;
  /** Device settings */
  settings: DeviceSettings;
}

/** Device create action */
export interface DeviceCreate {
  /** Device name (1-64 chars) */
  name: string;
  /** Device type */
  device_type: DeviceType;
  /** DNS server ID */
  dns_server_id: string;
}

/** Device update action */
export interface DeviceUpdate {
  /** Device name (1-64 chars) */
  name?: string;
  /** Device type */
  device_type?: DeviceType;
  /** DNS server ID */
  dns_server_id?: string;
}
