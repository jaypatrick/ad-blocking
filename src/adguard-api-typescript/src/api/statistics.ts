/**
 * Statistics API client
 */

import { BaseApi } from './base.ts';
import { ApiConfiguration } from '../helpers/configuration.ts';
import {
  TimeQueriesStatsList,
  CategoryQueriesStatsList,
  CompanyQueriesStatsList,
  CompanyDetailedQueriesStatsList,
  CountryQueriesStatsList,
  DeviceQueriesStatsList,
  DomainQueriesStatsList,
  StatsQueryParams,
  DetailedStatsQueryParams,
} from '../models/index.ts';

/** Statistics API endpoints */
export class StatisticsApi extends BaseApi {
  constructor(config: ApiConfiguration) {
    super(config);
  }

  /**
   * Convert stats params to query params object
   */
  private toQueryParams(params: StatsQueryParams): Record<string, unknown> {
    const query: Record<string, unknown> = {
      time_from_millis: params.time_from_millis,
      time_to_millis: params.time_to_millis,
    };
    if (params.devices && params.devices.length > 0) {
      query['devices'] = params.devices;
    }
    if (params.countries && params.countries.length > 0) {
      query['countries'] = params.countries;
    }
    return query;
  }

  /**
   * Get time-based statistics
   * @param params - Query parameters
   * @returns Time statistics list
   */
  async getTimeQueriesStats(params: StatsQueryParams): Promise<TimeQueriesStatsList> {
    this.logger.debug('Getting time queries stats');
    return this.get<TimeQueriesStatsList>('/oapi/v1/stats/time', this.toQueryParams(params));
  }

  /**
   * Get category statistics
   * @param params - Query parameters
   * @returns Category statistics list
   */
  async getCategoriesQueriesStats(params: StatsQueryParams): Promise<CategoryQueriesStatsList> {
    this.logger.debug('Getting categories queries stats');
    return this.get<CategoryQueriesStatsList>(
      '/oapi/v1/stats/categories',
      this.toQueryParams(params)
    );
  }

  /**
   * Get company statistics
   * @param params - Query parameters
   * @returns Company statistics list
   */
  async getCompaniesStats(params: StatsQueryParams): Promise<CompanyQueriesStatsList> {
    this.logger.debug('Getting companies stats');
    return this.get<CompanyQueriesStatsList>(
      '/oapi/v1/stats/companies',
      this.toQueryParams(params)
    );
  }

  /**
   * Get detailed company statistics
   * @param params - Query parameters with pagination
   * @returns Detailed company statistics list
   */
  async getDetailedCompaniesStats(
    params: DetailedStatsQueryParams
  ): Promise<CompanyDetailedQueriesStatsList> {
    this.logger.debug('Getting detailed companies stats');
    const query = this.toQueryParams(params);
    if (params.cursor) {
      query['cursor'] = params.cursor;
    }
    return this.get<CompanyDetailedQueriesStatsList>('/oapi/v1/stats/companies/detailed', query);
  }

  /**
   * Get country statistics
   * @param params - Query parameters
   * @returns Country statistics list
   */
  async getCountriesQueriesStats(params: StatsQueryParams): Promise<CountryQueriesStatsList> {
    this.logger.debug('Getting countries queries stats');
    return this.get<CountryQueriesStatsList>(
      '/oapi/v1/stats/countries',
      this.toQueryParams(params)
    );
  }

  /**
   * Get device statistics
   * @param params - Query parameters
   * @returns Device statistics list
   */
  async getDevicesQueriesStats(params: StatsQueryParams): Promise<DeviceQueriesStatsList> {
    this.logger.debug('Getting devices queries stats');
    return this.get<DeviceQueriesStatsList>('/oapi/v1/stats/devices', this.toQueryParams(params));
  }

  /**
   * Get domain statistics
   * @param params - Query parameters
   * @returns Domain statistics list
   */
  async getDomainsQueriesStats(params: StatsQueryParams): Promise<DomainQueriesStatsList> {
    this.logger.debug('Getting domains queries stats');
    return this.get<DomainQueriesStatsList>('/oapi/v1/stats/domains', this.toQueryParams(params));
  }
}
