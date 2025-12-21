/**
 * DNS Server-related models
 */

import { BlockingMode, DayOfWeek, FilterListCategoryType } from './enums';
import { Limit } from './account';

/** Blocking mode settings */
export interface BlockingModeSettings {
  /** Blocking mode */
  blocking_mode: BlockingMode;
  /** Custom IPv4 address for CUSTOM_IP mode */
  ipv4_blocking_address?: string;
  /** Custom IPv6 address for CUSTOM_IP mode */
  ipv6_blocking_address?: string;
}

/** Blocking mode settings update */
export interface BlockingModeSettingsUpdate {
  /** Blocking mode */
  blocking_mode: BlockingMode;
  /** Custom IPv4 address for CUSTOM_IP mode */
  ipv4_blocking_address?: string;
  /** Custom IPv6 address for CUSTOM_IP mode */
  ipv6_blocking_address?: string;
}

/** DNS server access settings */
export interface DNSServerAccessSettings {
  /** Whether access settings are enabled */
  enabled: boolean;
  /** Allowed IPs, CIDRs or ASNs */
  allowed_clients: string[];
  /** Blocked IPs, CIDRs or ASNs */
  blocked_clients: string[];
  /** Blocked domain rules */
  blocked_domain_rules: string[];
  /** Block known scanners */
  block_known_scanners: boolean;
}

/** DNS server access settings update */
export interface DNSServerAccessSettingsUpdate {
  /** Whether access settings are enabled */
  enabled?: boolean;
  /** Allowed IPs, CIDRs or ASNs */
  allowed_clients?: string[];
  /** Blocked IPs, CIDRs or ASNs */
  blocked_clients?: string[];
  /** Blocked domain rules (max 1024 chars each) */
  blocked_domain_rules?: string[];
  /** Block known scanners */
  block_known_scanners?: boolean;
}

/** User rules settings */
export interface UserRulesSettings {
  /** Whether user rules are enabled */
  enabled: boolean;
  /** User rules list */
  rules: string[];
}

/** User rules settings update */
export interface UserRulesSettingsUpdate {
  /** Whether user rules are enabled */
  enabled?: boolean;
  /** User rules list */
  rules?: string[];
}

/** Filter list item */
export interface FilterListItem {
  /** Filter identifier */
  filter_id: string;
  /** Whether the filter is enabled */
  enabled: boolean;
}

/** Filter list item update */
export interface FilterListItemUpdate {
  /** Filter identifier */
  filter_id: string;
  /** Whether the filter is enabled */
  enabled?: boolean;
}

/** Filter lists settings */
export interface FilterListsSettings {
  /** Whether all filters are enabled */
  enabled: boolean;
  /** Filter list */
  filter_list: FilterListItem[];
}

/** Filter lists settings update */
export interface FilterListsSettingsUpdate {
  /** Whether all filters are enabled */
  enabled?: boolean;
  /** Filter list */
  filter_list?: FilterListItemUpdate[];
}

/** Safebrowsing settings */
export interface SafebrowsingSettings {
  /** Whether safebrowsing is enabled */
  enabled: boolean;
  /** Block newly registered domains */
  block_nrd: boolean;
}

/** Safebrowsing settings update */
export interface SafebrowsingSettingsUpdate {
  /** Whether safebrowsing is enabled */
  enabled?: boolean;
  /** Block newly registered domains */
  block_nrd?: boolean;
}

/** Blocked web service */
export interface BlockedWebService {
  /** Web service ID */
  id: string;
  /** Whether blocking is enabled */
  enabled: boolean;
}

/** Blocked web service update */
export interface BlockedWebServiceUpdate {
  /** Web service ID (1-64 chars) */
  id: string;
  /** Whether blocking is enabled */
  enabled?: boolean;
}

/** Time range for schedule */
export interface ScheduleTimeRange {
  /** Start time in minutes from midnight */
  start: number;
  /** End time in minutes from midnight */
  end: number;
}

/** Schedule day */
export interface ScheduleDay {
  /** Day of week */
  day: DayOfWeek;
  /** Time ranges */
  time_ranges: ScheduleTimeRange[];
}

/** Parental control schedule */
export interface ParentalSchedule {
  /** Whether schedule is enabled */
  enabled: boolean;
  /** Timezone */
  timezone: string;
  /** Schedule days */
  schedule_days: ScheduleDay[];
}

/** Parental control settings */
export interface ParentalControlSettings {
  /** Whether parental control is enabled */
  enabled: boolean;
  /** Safe search enabled */
  safe_search_enabled: boolean;
  /** YouTube restricted mode */
  youtube_restricted_mode: boolean;
  /** Block adult content */
  block_adult: boolean;
  /** Blocked web services */
  blocked_web_services: BlockedWebService[];
  /** Schedule */
  schedule: ParentalSchedule;
}

/** Parental control settings update */
export interface ParentalControlSettingsUpdate {
  /** Whether parental control is enabled */
  enabled?: boolean;
  /** Safe search enabled */
  safe_search_enabled?: boolean;
  /** YouTube restricted mode */
  youtube_restricted_mode?: boolean;
  /** Block adult content */
  block_adult?: boolean;
  /** Blocked web services */
  blocked_web_services?: BlockedWebServiceUpdate[];
  /** Schedule */
  schedule?: Partial<ParentalSchedule>;
}

/** DNS server settings */
export interface DNSServerSettings {
  /** Protection enabled */
  protection_enabled: boolean;
  /** IP logging consent */
  ip_log_enabled: boolean;
  /** Auto-connect devices enabled */
  auto_connect_devices_enabled: boolean;
  /** Block Chrome prefetch proxy */
  block_chrome_prefetch: boolean;
  /** Block Firefox canary */
  block_firefox_canary: boolean;
  /** Block Apple Private Relay */
  block_private_relay: boolean;
  /** TTL for blocked requests (0-3600) */
  block_ttl_seconds: number;
  /** Blocking mode settings */
  blocking_mode_settings: BlockingModeSettings;
  /** Access settings */
  access_settings: DNSServerAccessSettings;
  /** User rules settings */
  user_rules_settings: UserRulesSettings;
  /** Filter lists settings */
  filter_lists_settings: FilterListsSettings;
  /** Safebrowsing settings */
  safebrowsing_settings: SafebrowsingSettings;
  /** Parental control settings */
  parental_control_settings: ParentalControlSettings;
}

/** DNS server settings update */
export interface DNSServerSettingsUpdate {
  /** Protection enabled */
  protection_enabled?: boolean;
  /** IP logging consent */
  ip_log_enabled?: boolean;
  /** Auto-connect devices enabled */
  auto_connect_devices_enabled?: boolean;
  /** Block Chrome prefetch proxy */
  block_chrome_prefetch?: boolean;
  /** Block Firefox canary */
  block_firefox_canary?: boolean;
  /** Block Apple Private Relay */
  block_private_relay?: boolean;
  /** TTL for blocked requests (0-3600) */
  block_ttl_seconds?: number;
  /** Blocking mode settings */
  blocking_mode_settings?: BlockingModeSettingsUpdate;
  /** Access settings */
  access_settings?: DNSServerAccessSettingsUpdate;
  /** User rules settings */
  user_rules_settings?: UserRulesSettingsUpdate;
  /** Filter lists settings */
  filter_lists_settings?: FilterListsSettingsUpdate;
  /** Safebrowsing settings */
  safebrowsing_settings?: SafebrowsingSettingsUpdate;
  /** Parental control settings */
  parental_control_settings?: ParentalControlSettingsUpdate;
}

/** DNS server entity */
export interface DNSServer {
  /** DNS server ID */
  id: string;
  /** DNS server name */
  name: string;
  /** Whether this is the default server */
  default: boolean;
  /** Connected device IDs */
  device_ids: string[];
  /** Server settings */
  settings: DNSServerSettings;
}

/** DNS server create action */
export interface DNSServerCreate {
  /** DNS server name (1-64 chars) */
  name: string;
  /** Initial settings */
  settings?: DNSServerSettingsUpdate;
}

/** DNS server update action */
export interface DNSServerUpdate {
  /** DNS server name (1-64 chars) */
  name?: string;
}

/** Dedicated IPv4 address */
export interface DedicatedIPv4Address {
  /** IP address */
  ip: string;
  /** Linked device ID, or null if vacant */
  device_id?: string;
}

/** Dedicated IPs for a device */
export interface DedicatedIps {
  /** Dedicated IPv4 addresses */
  ipv4: string[];
  /** IPv4 limit */
  ipv4_limit: Limit;
  /** Dedicated IPv6 addresses */
  ipv6: string[];
}

/** Link dedicated IPv4 to device */
export interface LinkDedicatedIPv4 {
  /** IPv4 address to link */
  ip: string;
}

/** Filter list category */
export interface FilterListCategory {
  /** Category type */
  category: FilterListCategoryType;
  /** Localized description */
  description: string;
  /** Category value */
  value?: string;
}

/** Filter list */
export interface FilterList {
  /** Filter ID */
  filter_id: string;
  /** Filter name */
  name: string;
  /** Filter description */
  description: string;
  /** Homepage URL */
  homepage_url: string;
  /** Source URL */
  source_url: string;
  /** Download URL */
  download_url: string;
  /** Rules count */
  rules_count: number;
  /** Last updated time */
  time_updated: string;
  /** Filter tags */
  tags: string[];
  /** Filter categories */
  categories: FilterListCategory[];
}

/** Web service */
export interface WebService {
  /** Service ID */
  id: string;
  /** Service name */
  name: string;
  /** Service icon URL */
  icon_url?: string;
}
