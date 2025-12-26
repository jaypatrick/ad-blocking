/**
 * Query Log menu
 */

import { BaseMenu, MenuItem } from './base.ts';
import { QueryLogRepository } from '../../repositories/query-log.ts';
import {
  createTable,
  displayTable,
  formatRelativeTime,
  showNoItems,
  showPanel,
  showSuccess,
  truncate,
  withSpinner,
} from '../utils.ts';
import { FilteringActionStatus } from '../../models/index.ts';

export class QueryLogMenu extends BaseMenu {
  protected get title(): string {
    return 'Query Log';
  }

  constructor(private readonly queryLogRepo: QueryLogRepository) {
    super();
  }

  protected getMenuItems(): MenuItem[] {
    return [
      { name: 'View recent queries', action: () => this.viewRecent() },
      { name: 'View blocked queries', action: () => this.viewBlocked() },
      { name: 'Search queries', action: () => this.searchQueries() },
      { name: 'View statistics', action: () => this.viewStats() },
      { name: 'Clear query log', action: () => this.clearLog() },
    ];
  }

  private async viewRecent(): Promise<void> {
    const entries = await withSpinner(
      'Loading query log...',
      () => this.queryLogRepo.getRecentLog(24, 50),
    );

    if (entries.length === 0) {
      showNoItems('queries');
      return;
    }

    const table = createTable(['Time', 'Domain', 'Type', 'Status', 'Device']);
    entries.forEach((e) => {
      const status = e.filtering_info?.filtering_status || 'NONE';
      const statusDisplay = status === FilteringActionStatus.REQUEST_BLOCKED
        ? '🚫 Blocked'
        : '✓ Allowed';
      table.push([
        formatRelativeTime(e.time_millis),
        truncate(e.domain, 40),
        e.query_type,
        statusDisplay,
        truncate(e.device_id, 10),
      ]);
    });
    displayTable(table);
  }

  private async viewBlocked(): Promise<void> {
    const entries = await withSpinner(
      'Loading blocked queries...',
      () => this.queryLogRepo.getBlockedQueries(50),
    );

    if (entries.length === 0) {
      showNoItems('blocked queries');
      return;
    }

    const table = createTable(['Time', 'Domain', 'Type', 'Blocked By', 'Device']);
    entries.forEach((e) => {
      const blockedBy = e.filtering_info?.filtering_type || 'Unknown';
      table.push([
        formatRelativeTime(e.time_millis),
        truncate(e.domain, 40),
        e.query_type,
        blockedBy,
        truncate(e.device_id, 10),
      ]);
    });
    displayTable(table);
  }

  private async searchQueries(): Promise<void> {
    const search = await this.getInput('Enter search term (domain, IP, etc.):');
    if (!search.trim()) return;

    const entries = await withSpinner(
      'Searching...',
      () => this.queryLogRepo.search(search.trim(), 50),
    );

    if (entries.length === 0) {
      showNoItems('matching queries');
      return;
    }

    const table = createTable(['Time', 'Domain', 'Type', 'Status', 'Device']);
    entries.forEach((e) => {
      const status = e.filtering_info?.filtering_status || 'NONE';
      const statusDisplay = status === FilteringActionStatus.REQUEST_BLOCKED
        ? '🚫 Blocked'
        : '✓ Allowed';
      table.push([
        formatRelativeTime(e.time_millis),
        truncate(e.domain, 40),
        e.query_type,
        statusDisplay,
        truncate(e.device_id, 10),
      ]);
    });
    displayTable(table);
  }

  private async viewStats(): Promise<void> {
    const entries = await withSpinner(
      'Loading query log...',
      () => this.queryLogRepo.getRecentLog(24, 1000),
    );

    if (entries.length === 0) {
      showNoItems('queries');
      return;
    }

    const stats = this.queryLogRepo.getStatistics(entries);

    showPanel('Query Log Statistics (Last 24h)', {
      'Total Queries': stats.total,
      'Blocked Queries': stats.blocked,
      'Allowed Queries': stats.allowed,
      'Block Rate': `${stats.blockedPercentage.toFixed(1)}%`,
    });

    if (stats.topDomains.length > 0) {
      console.log('\nTop Domains:');
      const domTable = createTable(['Domain', 'Queries']);
      stats.topDomains.forEach((d) => {
        domTable.push([truncate(d.domain, 50), d.count.toString()]);
      });
      displayTable(domTable);
    }

    if (stats.byDevice.length > 0) {
      console.log('\nQueries by Device:');
      const devTable = createTable(['Device ID', 'Queries']);
      stats.byDevice.forEach((d) => {
        devTable.push([d.deviceId, d.count.toString()]);
      });
      displayTable(devTable);
    }

    if (stats.byFilteringType.length > 0) {
      console.log('\nBlocked by Type:');
      const typeTable = createTable(['Type', 'Count']);
      stats.byFilteringType.forEach((t) => {
        typeTable.push([t.type, t.count.toString()]);
      });
      displayTable(typeTable);
    }
  }

  private async clearLog(): Promise<void> {
    const confirmed = await this.confirm(
      'Are you sure you want to clear the entire query log?',
    );

    if (!confirmed) return;

    await withSpinner('Clearing query log...', () => this.queryLogRepo.clear());

    showSuccess('Query log cleared');
  }
}
