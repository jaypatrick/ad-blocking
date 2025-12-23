/**
 * Query Log repository
 */

import { BaseRepository } from './base.js';
import { QueryLogApi } from '../api/query-log.js';
import { Logger } from '../helpers/configuration.js';
import { DateTime } from '../helpers/datetime.js';
import {
  QueryLogResponse,
  QueryLogEntry,
  QueryLogParams,
  FilteringActionStatus,
  FilteringActionSource,
} from '../models/index.js';

/** Query Log repository for managing DNS query logs */
export class QueryLogRepository extends BaseRepository {
  private readonly api: QueryLogApi;

  constructor(api: QueryLogApi, logger?: Logger) {
    super(logger);
    this.api = api;
  }

  /**
   * Get query log entries
   * @param params - Query parameters
   * @returns Query log response
   */
  async getLog(params: QueryLogParams = {}): Promise<QueryLogResponse> {
    return this.execute('Get query log', () => this.api.getQueryLog(params));
  }

  /**
   * Get query log entries for a specific device
   * @param deviceId - Device ID
   * @param limit - Maximum entries to return
   */
  async getLogByDevice(deviceId: string, limit: number = 100): Promise<QueryLogEntry[]> {
    const response = await this.getLog({
      device_ids: [deviceId],
      limit,
    });
    return response.entries;
  }

  /**
   * Get query log entries for the last N hours
   * @param hours - Number of hours
   * @param limit - Maximum entries to return
   */
  async getRecentLog(hours: number = 24, limit: number = 100): Promise<QueryLogEntry[]> {
    const response = await this.getLog({
      time_from_millis: DateTime.hoursAgo(hours),
      time_to_millis: DateTime.now(),
      limit,
    });
    return response.entries;
  }

  /**
   * Get blocked queries only
   * @param limit - Maximum entries to return
   */
  async getBlockedQueries(limit: number = 100): Promise<QueryLogEntry[]> {
    const response = await this.getLog({
      filtering_status: FilteringActionStatus.REQUEST_BLOCKED,
      limit,
    });
    return response.entries;
  }

  /**
   * Search query log
   * @param search - Search string (domain, IP, etc.)
   * @param limit - Maximum entries to return
   */
  async search(search: string, limit: number = 100): Promise<QueryLogEntry[]> {
    const response = await this.getLog({
      search,
      limit,
    });
    return response.entries;
  }

  /**
   * Get all query log entries (handles pagination)
   * @param params - Base query parameters (cursor will be managed)
   * @param maxPages - Maximum number of pages to fetch
   * @returns All entries across pages
   */
  async getAllEntries(
    params: Omit<QueryLogParams, 'cursor'> = {},
    maxPages: number = 10
  ): Promise<QueryLogEntry[]> {
    const allEntries: QueryLogEntry[] = [];
    let cursor: string | undefined;
    let pageCount = 0;

    do {
      const response = await this.getLog({ ...params, cursor });
      allEntries.push(...response.entries);
      cursor = response.cursor;
      pageCount++;
    } while (cursor && pageCount < maxPages);

    return allEntries;
  }

  /**
   * Clear all query log entries
   */
  async clear(): Promise<void> {
    return this.execute('Clear query log', () => this.api.clearQueryLog());
  }

  /**
   * Get statistics from query log
   * @param entries - Query log entries to analyze
   */
  getStatistics(entries: QueryLogEntry[]): {
    total: number;
    blocked: number;
    allowed: number;
    blockedPercentage: number;
    topDomains: { domain: string; count: number }[];
    byDevice: { deviceId: string; count: number }[];
    byFilteringType: { type: FilteringActionSource; count: number }[];
  } {
    const total = entries.length;
    const blocked = entries.filter(
      e => e.filtering_info?.filtering_status === FilteringActionStatus.REQUEST_BLOCKED
    ).length;
    const allowed = total - blocked;

    // Count domains
    const domainCounts = new Map<string, number>();
    entries.forEach(e => {
      const count = domainCounts.get(e.domain) || 0;
      domainCounts.set(e.domain, count + 1);
    });

    // Count by device
    const deviceCounts = new Map<string, number>();
    entries.forEach(e => {
      const count = deviceCounts.get(e.device_id) || 0;
      deviceCounts.set(e.device_id, count + 1);
    });

    // Count by filtering type
    const typeCounts = new Map<FilteringActionSource, number>();
    entries.forEach(e => {
      const type = e.filtering_info?.filtering_type;
      if (type) {
        const count = typeCounts.get(type) || 0;
        typeCounts.set(type, count + 1);
      }
    });

    return {
      total,
      blocked,
      allowed,
      blockedPercentage: total > 0 ? (blocked / total) * 100 : 0,
      topDomains: Array.from(domainCounts.entries())
        .map(([domain, count]) => ({ domain, count }))
        .sort((a, b) => b.count - a.count)
        .slice(0, 10),
      byDevice: Array.from(deviceCounts.entries())
        .map(([deviceId, count]) => ({ deviceId, count }))
        .sort((a, b) => b.count - a.count),
      byFilteringType: Array.from(typeCounts.entries())
        .map(([type, count]) => ({ type, count }))
        .sort((a, b) => b.count - a.count),
    };
  }
}
