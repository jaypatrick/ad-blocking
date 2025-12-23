/**
 * DNS Servers menu
 */

import { BaseMenu, MenuItem } from './base.js';
import { DnsServerRepository } from '../../repositories/dns-server.js';
// eslint-disable-next-line @typescript-eslint/no-unused-vars
import {
  createTable,
  displayTable,
  showPanel,
  showSuccess,
  showNoItems,
  withSpinner,
} from '../utils.js';

export class DnsServersMenu extends BaseMenu {
  protected get title(): string {
    return 'DNS Servers';
  }

  constructor(private readonly dnsServerRepo: DnsServerRepository) {
    super();
  }

  protected getMenuItems(): MenuItem[] {
    return [
      { name: 'List all DNS servers', action: () => this.listServers() },
      { name: 'View server details', action: () => this.viewServer() },
      { name: 'Create DNS server', action: () => this.createServer() },
      { name: 'Delete DNS server', action: () => this.deleteServer() },
      { name: 'Toggle protection', action: () => this.toggleProtection() },
    ];
  }

  private async listServers(): Promise<void> {
    const servers = await withSpinner('Loading DNS servers...', () =>
      this.dnsServerRepo.getAll()
    );

    if (servers.length === 0) {
      showNoItems('DNS servers');
      return;
    }

    const table = createTable(['ID', 'Name', 'Default', 'Devices', 'Protected']);
    for (const server of servers) {
      table.push([
        server.id,
        server.name,
        server.default ? 'Yes' : 'No',
        server.device_ids.length.toString(),
        server.settings.protection_enabled ? 'Yes' : 'No',
      ]);
    }
    displayTable(table);
  }

  private async viewServer(): Promise<void> {
    const servers = await withSpinner('Loading DNS servers...', () =>
      this.dnsServerRepo.getAll()
    );

    const server = await this.selectItem(
      'Select a DNS server:',
      servers,
      s => `${s.name} (${s.id})`
    );

    if (!server) return;

    showPanel(`DNS Server: ${server.name}`, {
      ID: server.id,
      Name: server.name,
      Default: server.default,
      'Connected Devices': server.device_ids.length,
      'Protection Enabled': server.settings.protection_enabled,
      'IP Logging': server.settings.ip_log_enabled,
      'Block Chrome Prefetch': server.settings.block_chrome_prefetch,
      'Block Firefox Canary': server.settings.block_firefox_canary,
      'Block Private Relay': server.settings.block_private_relay,
      'Block TTL (seconds)': server.settings.block_ttl_seconds,
      'Blocking Mode': server.settings.blocking_mode_settings.blocking_mode,
      'User Rules Enabled': server.settings.user_rules_settings.enabled,
      'User Rules Count': server.settings.user_rules_settings.rules.length,
      'Filter Lists Enabled': server.settings.filter_lists_settings.enabled,
      'Safebrowsing Enabled': server.settings.safebrowsing_settings.enabled,
      'Parental Control': server.settings.parental_control_settings.enabled,
    });
  }

  private async createServer(): Promise<void> {
    const name = await this.getInput('DNS server name:');
    if (!name.trim()) return;

    const server = await withSpinner('Creating DNS server...', () =>
      this.dnsServerRepo.create({ name: name.trim() })
    );

    showSuccess(`DNS server created: ${server.name} (${server.id})`);
  }

  private async deleteServer(): Promise<void> {
    const servers = await withSpinner('Loading DNS servers...', () =>
      this.dnsServerRepo.getAll()
    );

    // Filter out default server
    const deletableServers = servers.filter(s => !s.default);

    if (deletableServers.length === 0) {
      showNoItems('deletable DNS servers (cannot delete default)');
      return;
    }

    const server = await this.selectItem(
      'Select a DNS server to delete:',
      deletableServers,
      s => `${s.name} (${s.id})`
    );

    if (!server) return;

    const confirmed = await this.confirm(
      `Are you sure you want to delete "${server.name}"?`
    );

    if (!confirmed) return;

    await withSpinner('Deleting DNS server...', () =>
      this.dnsServerRepo.delete(server.id)
    );

    showSuccess(`DNS server deleted: ${server.name}`);
  }

  private async toggleProtection(): Promise<void> {
    const servers = await withSpinner('Loading DNS servers...', () =>
      this.dnsServerRepo.getAll()
    );

    const server = await this.selectItem(
      'Select a DNS server:',
      servers,
      s => `${s.name} - Protection: ${s.settings.protection_enabled ? 'ON' : 'OFF'}`
    );

    if (!server) return;

    const newState = !server.settings.protection_enabled;

    await withSpinner(
      `${newState ? 'Enabling' : 'Disabling'} protection...`,
      () =>
        this.dnsServerRepo.updateSettings(server.id, {
          protection_enabled: newState,
        })
    );

    showSuccess(
      `Protection ${newState ? 'enabled' : 'disabled'} for ${server.name}`
    );
  }
}
