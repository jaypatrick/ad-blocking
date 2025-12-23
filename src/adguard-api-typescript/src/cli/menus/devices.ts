/**
 * Devices menu
 */

import { BaseMenu, MenuItem } from './base.js';
import { DeviceRepository } from '../../repositories/device.js';
import { DnsServerRepository } from '../../repositories/dns-server.js';
import { DeviceType } from '../../models/index.js';
import {
  createTable,
  displayTable,
  showPanel,
  showSuccess,
  showNoItems,
  withSpinner,
} from '../utils.js';

export class DevicesMenu extends BaseMenu {
  protected get title(): string {
    return 'Devices';
  }

  constructor(
    private readonly deviceRepo: DeviceRepository,
    private readonly dnsServerRepo: DnsServerRepository
  ) {
    super();
  }

  protected getMenuItems(): MenuItem[] {
    return [
      { name: 'List all devices', action: () => this.listDevices() },
      { name: 'View device details', action: () => this.viewDevice() },
      { name: 'Create device', action: () => this.createDevice() },
      { name: 'Delete device', action: () => this.deleteDevice() },
      { name: 'Toggle device protection', action: () => this.toggleProtection() },
    ];
  }

  private async listDevices(): Promise<void> {
    const devices = await withSpinner('Loading devices...', () =>
      this.deviceRepo.getAll()
    );

    if (devices.length === 0) {
      showNoItems('devices');
      return;
    }

    const table = createTable(['ID', 'Name', 'Type', 'DNS Server', 'Protected']);
    for (const device of devices) {
      table.push([
        device.id,
        device.name,
        device.device_type,
        device.dns_server_id,
        device.settings.protection_enabled ? 'Yes' : 'No',
      ]);
    }
    displayTable(table);
  }

  private async viewDevice(): Promise<void> {
    const devices = await withSpinner('Loading devices...', () =>
      this.deviceRepo.getAll()
    );

    const device = await this.selectItem(
      'Select a device:',
      devices,
      d => `${d.name} (${d.id})`
    );

    if (!device) return;

    showPanel(`Device: ${device.name}`, {
      ID: device.id,
      Name: device.name,
      Type: device.device_type,
      'DNS Server ID': device.dns_server_id,
      'Protection Enabled': device.settings.protection_enabled,
      'DoH Auth Only': device.settings.detect_doh_auth_only,
      'DoH URL': device.dns_addresses.dns_over_https_url,
      'DoT URL': device.dns_addresses.dns_over_tls_url,
      'DoQ URL': device.dns_addresses.dns_over_quic_url,
    });
  }

  private async createDevice(): Promise<void> {
    // Get DNS servers for selection
    const dnsServers = await withSpinner('Loading DNS servers...', () =>
      this.dnsServerRepo.getAll()
    );

    if (dnsServers.length === 0) {
      showNoItems('DNS servers');
      return;
    }

    const name = await this.getInput('Device name:');
    if (!name.trim()) return;

    const deviceTypes = Object.values(DeviceType);
    const { deviceType } = await require('inquirer').prompt([
      {
        type: 'list',
        name: 'deviceType',
        message: 'Select device type:',
        choices: deviceTypes,
      },
    ]);

    const dnsServer = await this.selectItem(
      'Select DNS server:',
      dnsServers,
      s => `${s.name} (${s.id})`
    );

    if (!dnsServer) return;

    const device = await withSpinner('Creating device...', () =>
      this.deviceRepo.create({
        name: name.trim(),
        device_type: deviceType,
        dns_server_id: dnsServer.id,
      })
    );

    showSuccess(`Device created: ${device.name} (${device.id})`);
  }

  private async deleteDevice(): Promise<void> {
    const devices = await withSpinner('Loading devices...', () =>
      this.deviceRepo.getAll()
    );

    const device = await this.selectItem(
      'Select a device to delete:',
      devices,
      d => `${d.name} (${d.id})`
    );

    if (!device) return;

    const confirmed = await this.confirm(
      `Are you sure you want to delete "${device.name}"?`
    );

    if (!confirmed) return;

    await withSpinner('Deleting device...', () =>
      this.deviceRepo.delete(device.id)
    );

    showSuccess(`Device deleted: ${device.name}`);
  }

  private async toggleProtection(): Promise<void> {
    const devices = await withSpinner('Loading devices...', () =>
      this.deviceRepo.getAll()
    );

    const device = await this.selectItem(
      'Select a device:',
      devices,
      d => `${d.name} - Protection: ${d.settings.protection_enabled ? 'ON' : 'OFF'}`
    );

    if (!device) return;

    const newState = !device.settings.protection_enabled;

    await withSpinner(
      `${newState ? 'Enabling' : 'Disabling'} protection...`,
      () =>
        this.deviceRepo.updateSettings(device.id, {
          protection_enabled: newState,
        })
    );

    showSuccess(
      `Protection ${newState ? 'enabled' : 'disabled'} for ${device.name}`
    );
  }
}
