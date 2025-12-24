/**
 * Query log-related models
 */

import {
  DnsProtoResponseType,
  FilteringActionSource,
  FilteringActionStatus,
  DnsQueryType,
  SecureDnsProtoType,
  RegularDnsProtoType,
} from './enums.ts';

/** Filtering info in query log */
export interface FilteringInfo {
  /** Filtering status */
  filtering_status?: FilteringActionStatus;
  /** Filtering type/source */
  filtering_type?: FilteringActionSource;
  /** Filter ID if blocked by filter */
  filter_id?: string;
  /** Filter rule that matched */
  filter_rule?: string;
  /** Blocked web service ID */
  blocked_service_id?: string;
}

/** DNS answer in query log */
export interface DnsAnswer {
  /** Answer type */
  type: string;
  /** Answer value */
  value: string;
  /** TTL in seconds */
  ttl: number;
}

/** Query log entry */
export interface QueryLogEntry {
  /** Query ID */
  id: string;
  /** Request time in milliseconds */
  time_millis: number;
  /** Queried domain */
  domain: string;
  /** Query type (A, AAAA, etc.) */
  query_type: DnsQueryType;
  /** DNS protocol type */
  protocol: SecureDnsProtoType | RegularDnsProtoType;
  /** Response type */
  response_type: DnsProtoResponseType;
  /** Client IP */
  client_ip?: string;
  /** Device ID */
  device_id: string;
  /** DNS server ID */
  dns_server_id: string;
  /** Country code */
  country?: string;
  /** City */
  city?: string;
  /** Filtering info */
  filtering_info?: FilteringInfo;
  /** DNS answers */
  answers?: DnsAnswer[];
  /** Elapsed time in milliseconds */
  elapsed_ms?: number;
  /** Whether response was cached */
  cached?: boolean;
}

/** Query log response */
export interface QueryLogResponse {
  /** Query log entries */
  entries: QueryLogEntry[];
  /** Cursor for next page */
  cursor?: string;
}

/** Query log query parameters */
export interface QueryLogParams {
  /** Filter by device IDs */
  device_ids?: string[];
  /** Time from in milliseconds */
  time_from_millis?: number;
  /** Time to in milliseconds */
  time_to_millis?: number;
  /** Search string */
  search?: string;
  /** Filter by filtering status */
  filtering_status?: FilteringActionStatus;
  /** Filter by filtering types */
  filtering_types?: FilteringActionSource[];
  /** Page size (default 20) */
  limit?: number;
  /** Pagination cursor */
  cursor?: string;
}
