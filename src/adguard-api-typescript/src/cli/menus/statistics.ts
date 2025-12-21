/**
 * Statistics menu
 */

import { BaseMenu, MenuItem } from './base';
import { StatisticsRepository, TimeRange } from '../../repositories/statistics';
import {
  createTable,
  displayTable,
  showPanel,
  showNoItems,
  withSpinner,
  formatPercentage,
} from '../utils';
import inquirer from 'inquirer';

export class StatisticsMenu extends BaseMenu {
  protected get title(): string {
    return 'Statistics';
  }

  constructor(private readonly statsRepo: StatisticsRepository) {
    super();
  }

  protected getMenuItems(): MenuItem[] {
    return [
      { name: 'View summary (24h)', action: () => this.viewSummary() },
      { name: 'View time statistics', action: () => this.viewTimeStats() },
      { name: 'View category statistics', action: () => this.viewCategoryStats() },
      { name: 'View company statistics', action: () => this.viewCompanyStats() },
      { name: 'View domain statistics', action: () => this.viewDomainStats() },
      { name: 'View device statistics', action: () => this.viewDeviceStats() },
      { name: 'View country statistics', action: () => this.viewCountryStats() },
    ];
  }

  private async selectTimeRange(): Promise<TimeRange> {
    const { range } = await inquirer.prompt([
      {
        type: 'list',
        name: 'range',
        message: 'Select time range:',
        choices: [
          { name: 'Today', value: 'today' },
          { name: 'Last 24 hours', value: '24h' },
          { name: 'Last 7 days', value: '7d' },
          { name: 'Last 30 days', value: '30d' },
        ],
      },
    ]);
    return range;
  }

  private async viewSummary(): Promise<void> {
    const summary = await withSpinner('Loading summary...', () =>
      this.statsRepo.getSummary()
    );

    // Calculate totals
    const totalQueries = summary.time.stats.reduce((sum, s) => sum + s.value.queries, 0);
    const totalBlocked = summary.time.stats.reduce((sum, s) => sum + s.value.blocked, 0);
    const blockPercentage = totalQueries > 0 ? (totalBlocked / totalQueries) * 100 : 0;

    showPanel('Summary (Last 24 Hours)', {
      'Total Queries': totalQueries,
      'Blocked Queries': totalBlocked,
      'Block Rate': formatPercentage(blockPercentage),
      'Data Points': summary.time.stats.length,
    });

    // Top categories
    if (summary.categories.stats.length > 0) {
      console.log('\nTop Categories:');
      const catTable = createTable(['Category', 'Queries']);
      summary.categories.stats.slice(0, 5).forEach(s => {
        catTable.push([s.category_type, s.queries.toString()]);
      });
      displayTable(catTable);
    }

    // Top companies
    if (summary.companies.stats.length > 0) {
      console.log('\nTop Companies:');
      const compTable = createTable(['Company', 'Queries', 'Blocked']);
      summary.companies.stats.slice(0, 5).forEach(s => {
        compTable.push([s.company_name, s.value.queries.toString(), s.value.blocked.toString()]);
      });
      displayTable(compTable);
    }

    // Top domains
    if (summary.domains.stats.length > 0) {
      console.log('\nTop Domains:');
      const domTable = createTable(['Domain', 'Queries', 'Blocked']);
      summary.domains.stats.slice(0, 5).forEach(s => {
        domTable.push([s.domain, s.value.queries.toString(), s.value.blocked.toString()]);
      });
      displayTable(domTable);
    }
  }

  private async viewTimeStats(): Promise<void> {
    const range = await this.selectTimeRange();
    const stats = await withSpinner('Loading time statistics...', () =>
      this.statsRepo.getTimeStatsByRange(range)
    );

    if (stats.stats.length === 0) {
      showNoItems('time statistics');
      return;
    }

    const table = createTable(['Time', 'Queries', 'Blocked', 'Block %']);
    stats.stats.forEach(s => {
      const percentage = s.value.queries > 0
        ? (s.value.blocked / s.value.queries) * 100
        : 0;
      table.push([
        new Date(s.time_millis).toLocaleString(),
        s.value.queries.toString(),
        s.value.blocked.toString(),
        formatPercentage(percentage),
      ]);
    });
    displayTable(table);
  }

  private async viewCategoryStats(): Promise<void> {
    const range = await this.selectTimeRange();
    const params = this.statsRepo.getTimeRangeParams(range);
    const stats = await withSpinner('Loading category statistics...', () =>
      this.statsRepo.getCategoryStats(params)
    );

    if (stats.stats.length === 0) {
      showNoItems('category statistics');
      return;
    }

    const table = createTable(['Category', 'Queries']);
    stats.stats.forEach(s => {
      table.push([s.category_type, s.queries.toString()]);
    });
    displayTable(table);
  }

  private async viewCompanyStats(): Promise<void> {
    const range = await this.selectTimeRange();
    const params = this.statsRepo.getTimeRangeParams(range);
    const stats = await withSpinner('Loading company statistics...', () =>
      this.statsRepo.getCompanyStats(params)
    );

    if (stats.stats.length === 0) {
      showNoItems('company statistics');
      return;
    }

    const table = createTable(['Company', 'Queries', 'Blocked', 'Block %']);
    stats.stats.slice(0, 20).forEach(s => {
      const percentage = s.value.queries > 0
        ? (s.value.blocked / s.value.queries) * 100
        : 0;
      table.push([
        s.company_name,
        s.value.queries.toString(),
        s.value.blocked.toString(),
        formatPercentage(percentage),
      ]);
    });
    displayTable(table);
  }

  private async viewDomainStats(): Promise<void> {
    const range = await this.selectTimeRange();
    const params = this.statsRepo.getTimeRangeParams(range);
    const stats = await withSpinner('Loading domain statistics...', () =>
      this.statsRepo.getDomainStats(params)
    );

    if (stats.stats.length === 0) {
      showNoItems('domain statistics');
      return;
    }

    const table = createTable(['Domain', 'Queries', 'Blocked', 'Block %']);
    stats.stats.slice(0, 20).forEach(s => {
      const percentage = s.value.queries > 0
        ? (s.value.blocked / s.value.queries) * 100
        : 0;
      table.push([
        s.domain,
        s.value.queries.toString(),
        s.value.blocked.toString(),
        formatPercentage(percentage),
      ]);
    });
    displayTable(table);
  }

  private async viewDeviceStats(): Promise<void> {
    const range = await this.selectTimeRange();
    const params = this.statsRepo.getTimeRangeParams(range);
    const stats = await withSpinner('Loading device statistics...', () =>
      this.statsRepo.getDeviceStats(params)
    );

    if (stats.stats.length === 0) {
      showNoItems('device statistics');
      return;
    }

    const table = createTable(['Device ID', 'Queries', 'Blocked', 'Block %', 'Last Active']);
    stats.stats.forEach(s => {
      const percentage = s.value.queries > 0
        ? (s.value.blocked / s.value.queries) * 100
        : 0;
      const lastActive = s.last_activity_time_millis
        ? new Date(s.last_activity_time_millis).toLocaleString()
        : 'N/A';
      table.push([
        s.device_id,
        s.value.queries.toString(),
        s.value.blocked.toString(),
        formatPercentage(percentage),
        lastActive,
      ]);
    });
    displayTable(table);
  }

  private async viewCountryStats(): Promise<void> {
    const range = await this.selectTimeRange();
    const params = this.statsRepo.getTimeRangeParams(range);
    const stats = await withSpinner('Loading country statistics...', () =>
      this.statsRepo.getCountryStats(params)
    );

    if (stats.stats.length === 0) {
      showNoItems('country statistics');
      return;
    }

    const table = createTable(['Country', 'Queries', 'Blocked', 'Block %']);
    stats.stats.forEach(s => {
      const percentage = s.value.queries > 0
        ? (s.value.blocked / s.value.queries) * 100
        : 0;
      table.push([
        s.country,
        s.value.queries.toString(),
        s.value.blocked.toString(),
        formatPercentage(percentage),
      ]);
    });
    displayTable(table);
  }
}
