/**
 * Device repository
 */

import { BaseRepository } from './base.ts';
import { DevicesApi } from '../api/devices.ts';
import { Logger } from '../helpers/configuration.ts';
import {
  Device,
  DeviceCreate,
  DeviceUpdate,
  DeviceSettingsUpdate,
  DedicatedIps,
  LinkDedicatedIPv4,
} from '../models/index.ts';

/** Device repository for managing devices */
export class DeviceRepository extends BaseRepository {
  private readonly api: DevicesApi;

  constructor(api: DevicesApi, logger?: Logger) {
    super(logger);
    this.api = api;
  }

  /**
   * Get all devices
   * @returns List of devices
   */
  async getAll(): Promise<Device[]> {
    return this.execute('Get all devices', () => this.api.listDevices());
  }

  /**
   * Get a device by ID
   * @param id - Device ID
   * @returns Device
   * @throws EntityNotFoundError if device not found
   */
  async getById(id: string): Promise<Device> {
    return this.executeWithEntityCheck(
      `Get device ${id}`,
      () => this.api.getDevice(id),
      'Device',
      id
    );
  }

  /**
   * Create a new device
   * @param data - Device creation data
   * @returns Created device
   */
  async create(data: DeviceCreate): Promise<Device> {
    return this.execute(`Create device ${data.name}`, () => this.api.createDevice(data));
  }

  /**
   * Update a device
   * @param id - Device ID
   * @param data - Device update data
   * @throws EntityNotFoundError if device not found
   */
  async update(id: string, data: DeviceUpdate): Promise<void> {
    return this.executeWithEntityCheck(
      `Update device ${id}`,
      () => this.api.updateDevice(id, data),
      'Device',
      id
    );
  }

  /**
   * Delete a device
   * @param id - Device ID
   * @throws EntityNotFoundError if device not found
   */
  async delete(id: string): Promise<void> {
    return this.executeWithEntityCheck(
      `Delete device ${id}`,
      () => this.api.removeDevice(id),
      'Device',
      id
    );
  }

  /**
   * Update device settings
   * @param id - Device ID
   * @param settings - Settings update data
   * @throws EntityNotFoundError if device not found
   */
  async updateSettings(id: string, settings: DeviceSettingsUpdate): Promise<void> {
    return this.executeWithEntityCheck(
      `Update device settings ${id}`,
      () => this.api.updateDeviceSettings(id, settings),
      'Device',
      id
    );
  }

  /**
   * Get dedicated addresses for a device
   * @param id - Device ID
   * @returns Dedicated IPs
   */
  async getDedicatedAddresses(id: string): Promise<DedicatedIps> {
    return this.execute(`Get dedicated addresses for ${id}`, () =>
      this.api.listDedicatedAddresses(id)
    );
  }

  /**
   * Link dedicated IPv4 to a device
   * @param id - Device ID
   * @param data - Link data
   */
  async linkDedicatedIPv4(id: string, data: LinkDedicatedIPv4): Promise<void> {
    return this.executeWithEntityCheck(
      `Link IPv4 ${data.ip} to device ${id}`,
      () => this.api.linkDedicatedIPv4(id, data),
      'Device',
      id
    );
  }

  /**
   * Unlink dedicated IPv4 from a device
   * @param id - Device ID
   * @param ip - IPv4 address
   */
  async unlinkDedicatedIPv4(id: string, ip: string): Promise<void> {
    return this.executeWithEntityCheck(
      `Unlink IPv4 ${ip} from device ${id}`,
      () => this.api.unlinkDedicatedIPv4(id, ip),
      'Device',
      id
    );
  }

  /**
   * Reset DNS-over-HTTPS password
   * @param id - Device ID
   */
  async resetDOHPassword(id: string): Promise<void> {
    return this.executeWithEntityCheck(
      `Reset DoH password for device ${id}`,
      () => this.api.resetDOHPassword(id),
      'Device',
      id
    );
  }
}
