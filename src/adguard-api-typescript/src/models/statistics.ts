/**
 * Statistics-related models
 */

import { CategoryType } from './enums.ts';

/** Base queries statistics */
export interface QueriesStats {
  /** Total queries */
  queries: number;
  /** Blocked queries */
  blocked: number;
}

/** Time-based queries stats */
export interface TimeQueriesStats {
  /** Timestamp in milliseconds */
  time_millis: number;
  /** Queries stats */
  value: QueriesStats;
}

/** Time queries stats list */
export interface TimeQueriesStatsList {
  /** List of time-based stats */
  stats: TimeQueriesStats[];
}

/** Category queries stats */
export interface CategoryQueriesStats {
  /** Category type */
  category_type: CategoryType;
  /** Query count */
  queries: number;
}

/** Category queries stats list */
export interface CategoryQueriesStatsList {
  /** List of category stats */
  stats: CategoryQueriesStats[];
}

/** Company queries stats */
export interface CompanyQueriesStats {
  /** Company name */
  company_name: string;
  /** Queries stats */
  value: QueriesStats;
}

/** Company queries stats list */
export interface CompanyQueriesStatsList {
  /** List of company stats */
  stats: CompanyQueriesStats[];
}

/** Pagination page info */
export interface Page {
  /** Cursor for next page */
  cursor?: string;
  /** Whether this is the current page */
  is_current: boolean;
}

/** Company detailed queries stats */
export interface CompanyDetailedQueriesStats {
  /** Company name */
  company_name: string;
  /** Top domain by queries */
  top_queries_domain: string;
  /** Top domain by blocked queries */
  top_blocked_domain?: string;
  /** Number of domains */
  domains_count: number;
  /** Queries stats */
  value: QueriesStats;
}

/** Company detailed queries stats list */
export interface CompanyDetailedQueriesStatsList {
  /** List of detailed company stats */
  stats: CompanyDetailedQueriesStats[];
  /** Pagination */
  pages: Page[];
}

/** Country queries stats */
export interface CountryQueriesStats {
  /** Country code */
  country: string;
  /** Queries stats */
  value: QueriesStats;
}

/** Country queries stats list */
export interface CountryQueriesStatsList {
  /** List of country stats */
  stats: CountryQueriesStats[];
}

/** Device queries stats */
export interface DeviceQueriesStats {
  /** Device ID */
  device_id: string;
  /** Last activity time in milliseconds */
  last_activity_time_millis?: number;
  /** Queries stats */
  value: QueriesStats;
}

/** Device queries stats list */
export interface DeviceQueriesStatsList {
  /** List of device stats */
  stats: DeviceQueriesStats[];
}

/** Domain queries stats */
export interface DomainQueriesStats {
  /** Domain name */
  domain: string;
  /** Queries stats */
  value: QueriesStats;
}

/** Domain queries stats list */
export interface DomainQueriesStatsList {
  /** List of domain stats */
  stats: DomainQueriesStats[];
}

/** Statistics query parameters */
export interface StatsQueryParams {
  /** Time from in milliseconds (inclusive) */
  time_from_millis: number;
  /** Time to in milliseconds (inclusive) */
  time_to_millis: number;
  /** Filter by device IDs */
  devices?: string[];
  /** Filter by country codes */
  countries?: string[];
}

/** Detailed stats query parameters */
export interface DetailedStatsQueryParams extends StatsQueryParams {
  /** Pagination cursor */
  cursor?: string;
}
