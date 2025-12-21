/**
 * Account menu
 */

import { BaseMenu, MenuItem } from './base';
import { AccountApi } from '../../api/account';
import { FilterListsApi } from '../../api/filter-lists';
import { WebServicesApi } from '../../api/web-services';
import {
  createTable,
  displayTable,
  showPanel,
  withSpinner,
  truncate,
} from '../utils';

export class AccountMenu extends BaseMenu {
  protected get title(): string {
    return 'Account & Info';
  }

  constructor(
    private readonly accountApi: AccountApi,
    private readonly filterListsApi: FilterListsApi,
    private readonly webServicesApi: WebServicesApi
  ) {
    super();
  }

  protected getMenuItems(): MenuItem[] {
    return [
      { name: 'View account limits', action: () => this.viewLimits() },
      { name: 'View filter lists', action: () => this.viewFilterLists() },
      { name: 'View web services', action: () => this.viewWebServices() },
    ];
  }

  private async viewLimits(): Promise<void> {
    const limits = await withSpinner('Loading account limits...', () =>
      this.accountApi.getAccountLimits()
    );

    showPanel('Account Limits', {
      Devices: `${limits.devices.current} / ${limits.devices.max}`,
      'DNS Servers': `${limits.dns_servers.current} / ${limits.dns_servers.max}`,
      'User Rules': `${limits.user_rules.current} / ${limits.user_rules.max}`,
      'Access Rules': `${limits.access_rules.current} / ${limits.access_rules.max}`,
      'Dedicated IPv4': `${limits.dedicated_ipv4.current} / ${limits.dedicated_ipv4.max}`,
      Requests: `${limits.requests.current} / ${limits.requests.max}`,
    });
  }

  private async viewFilterLists(): Promise<void> {
    const lists = await withSpinner('Loading filter lists...', () =>
      this.filterListsApi.listFilterLists()
    );

    const table = createTable(['ID', 'Name', 'Rules', 'Categories']);
    lists.forEach(list => {
      const categories = list.categories.map(c => c.category).join(', ');
      table.push([
        list.filter_id,
        truncate(list.name, 40),
        list.rules_count.toString(),
        truncate(categories, 20),
      ]);
    });
    displayTable(table);
  }

  private async viewWebServices(): Promise<void> {
    const services = await withSpinner('Loading web services...', () =>
      this.webServicesApi.listWebServices()
    );

    const table = createTable(['ID', 'Name']);
    services.forEach(service => {
      table.push([service.id, service.name]);
    });
    displayTable(table);
  }
}
