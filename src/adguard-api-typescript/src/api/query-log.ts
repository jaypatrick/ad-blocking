/**
 * Query Log API client
 */

import { BaseApi } from './base.ts';
import { ApiConfiguration } from '../helpers/configuration.ts';
import { QueryLogResponse, QueryLogParams } from '../models/index.ts';

/** Query Log API endpoints */
export class QueryLogApi extends BaseApi {
  constructor(config: ApiConfiguration) {
    super(config);
  }

  /**
   * Get query log entries
   * @param params - Query parameters
   * @returns Query log response with entries and pagination cursor
   */
  async getQueryLog(params: QueryLogParams = {}): Promise<QueryLogResponse> {
    this.logger.debug('Getting query log');
    const query: Record<string, unknown> = {};

    if (params.device_ids && params.device_ids.length > 0) {
      query['device_ids'] = params.device_ids;
    }
    if (params.time_from_millis !== undefined) {
      query['time_from_millis'] = params.time_from_millis;
    }
    if (params.time_to_millis !== undefined) {
      query['time_to_millis'] = params.time_to_millis;
    }
    if (params.search) {
      query['search'] = params.search;
    }
    if (params.filtering_status) {
      query['filtering_status'] = params.filtering_status;
    }
    if (params.filtering_types && params.filtering_types.length > 0) {
      query['filtering_types'] = params.filtering_types;
    }
    if (params.limit !== undefined) {
      query['limit'] = params.limit;
    }
    if (params.cursor) {
      query['cursor'] = params.cursor;
    }

    return this.get<QueryLogResponse>('/oapi/v1/query_log', query);
  }

  /**
   * Clear query log
   */
  async clearQueryLog(): Promise<void> {
    this.logger.debug('Clearing query log');
    await this.delete<void>('/oapi/v1/query_log');
  }
}
