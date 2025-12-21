/**
 * Statistics repository
 */

import { BaseRepository } from './base';
import { StatisticsApi } from '../api/statistics';
import { Logger } from '../helpers/configuration';
import { DateTime } from '../helpers/datetime';
import {
  TimeQueriesStatsList,
  CategoryQueriesStatsList,
  CompanyQueriesStatsList,
  CompanyDetailedQueriesStatsList,
  CountryQueriesStatsList,
  DeviceQueriesStatsList,
  DomainQueriesStatsList,
  StatsQueryParams,
} from '../models';

/** Time range presets */
export type TimeRange = 'today' | '24h' | '7d' | '30d' | 'custom';

/** Statistics repository for querying DNS statistics */
export class StatisticsRepository extends BaseRepository {
  private readonly api: StatisticsApi;

  constructor(api: StatisticsApi, logger?: Logger) {
    super(logger);
    this.api = api;
  }

  /**
   * Get time range parameters based on preset
   * @param range - Time range preset
   * @param customFrom - Custom from timestamp (for 'custom' range)
   * @param customTo - Custom to timestamp (for 'custom' range)
   * @returns Stats query parameters
   */
  getTimeRangeParams(
    range: TimeRange,
    customFrom?: number,
    customTo?: number
  ): Pick<StatsQueryParams, 'time_from_millis' | 'time_to_millis'> {
    switch (range) {
      case 'today':
        return {
          time_from_millis: DateTime.startOfToday(),
          time_to_millis: DateTime.endOfToday(),
        };
      case '24h':
        return {
          time_from_millis: DateTime.hoursAgo(24),
          time_to_millis: DateTime.now(),
        };
      case '7d':
        return {
          time_from_millis: DateTime.daysAgo(7),
          time_to_millis: DateTime.now(),
        };
      case '30d':
        return {
          time_from_millis: DateTime.daysAgo(30),
          time_to_millis: DateTime.now(),
        };
      case 'custom':
        if (!customFrom || !customTo) {
          throw new Error('Custom time range requires from and to timestamps');
        }
        return {
          time_from_millis: customFrom,
          time_to_millis: customTo,
        };
    }
  }

  /**
   * Get time-based statistics
   * @param params - Query parameters
   */
  async getTimeStats(params: StatsQueryParams): Promise<TimeQueriesStatsList> {
    return this.execute('Get time statistics', () => this.api.getTimeQueriesStats(params));
  }

  /**
   * Get time-based statistics using preset
   * @param range - Time range preset
   * @param devices - Optional device filter
   * @param countries - Optional country filter
   */
  async getTimeStatsByRange(
    range: TimeRange,
    devices?: string[],
    countries?: string[]
  ): Promise<TimeQueriesStatsList> {
    const params = {
      ...this.getTimeRangeParams(range),
      devices,
      countries,
    };
    return this.getTimeStats(params);
  }

  /**
   * Get category statistics
   * @param params - Query parameters
   */
  async getCategoryStats(params: StatsQueryParams): Promise<CategoryQueriesStatsList> {
    return this.execute('Get category statistics', () =>
      this.api.getCategoriesQueriesStats(params)
    );
  }

  /**
   * Get company statistics
   * @param params - Query parameters
   */
  async getCompanyStats(params: StatsQueryParams): Promise<CompanyQueriesStatsList> {
    return this.execute('Get company statistics', () => this.api.getCompaniesStats(params));
  }

  /**
   * Get detailed company statistics
   * @param params - Query parameters with pagination
   */
  async getDetailedCompanyStats(
    params: StatsQueryParams & { cursor?: string }
  ): Promise<CompanyDetailedQueriesStatsList> {
    return this.execute('Get detailed company statistics', () =>
      this.api.getDetailedCompaniesStats(params)
    );
  }

  /**
   * Get country statistics
   * @param params - Query parameters
   */
  async getCountryStats(params: StatsQueryParams): Promise<CountryQueriesStatsList> {
    return this.execute('Get country statistics', () =>
      this.api.getCountriesQueriesStats(params)
    );
  }

  /**
   * Get device statistics
   * @param params - Query parameters
   */
  async getDeviceStats(params: StatsQueryParams): Promise<DeviceQueriesStatsList> {
    return this.execute('Get device statistics', () => this.api.getDevicesQueriesStats(params));
  }

  /**
   * Get domain statistics
   * @param params - Query parameters
   */
  async getDomainStats(params: StatsQueryParams): Promise<DomainQueriesStatsList> {
    return this.execute('Get domain statistics', () => this.api.getDomainsQueriesStats(params));
  }

  /**
   * Get summary statistics for the last 24 hours
   */
  async getSummary(devices?: string[]): Promise<{
    time: TimeQueriesStatsList;
    categories: CategoryQueriesStatsList;
    companies: CompanyQueriesStatsList;
    domains: DomainQueriesStatsList;
  }> {
    const params = {
      ...this.getTimeRangeParams('24h'),
      devices,
    };

    const [time, categories, companies, domains] = await Promise.all([
      this.getTimeStats(params),
      this.getCategoryStats(params),
      this.getCompanyStats(params),
      this.getDomainStats(params),
    ]);

    return { time, categories, companies, domains };
  }
}
