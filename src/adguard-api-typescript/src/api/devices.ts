/**
 * Devices API client
 */

import { BaseApi } from './base.ts';
import { ApiConfiguration } from '../helpers/configuration.ts';
import {
  DedicatedIps,
  Device,
  DeviceCreate,
  DeviceSettingsUpdate,
  DeviceUpdate,
  LinkDedicatedIPv4,
} from '../models/index.ts';

/** Devices API endpoints */
export class DevicesApi extends BaseApi {
  constructor(config: ApiConfiguration) {
    super(config);
  }

  /**
   * List all devices
   * @returns Array of devices
   */
  async listDevices(): Promise<Device[]> {
    this.logger.debug('Listing devices');
    return this.get<Device[]>('/oapi/v1/devices');
  }

  /**
   * Get a device by ID
   * @param deviceId - Device ID
   * @returns Device information
   */
  async getDevice(deviceId: string): Promise<Device> {
    this.logger.debug(`Getting device: ${deviceId}`);
    try {
      return await this.get<Device>(`/oapi/v1/devices/${deviceId}`);
    } catch (error) {
      this.handleError(error, 'Device', deviceId);
    }
  }

  /**
   * Create a new device
   * @param device - Device creation data
   * @returns Created device
   */
  async createDevice(device: DeviceCreate): Promise<Device> {
    this.logger.debug(`Creating device: ${device.name}`);
    return this.post<Device>('/oapi/v1/devices', device);
  }

  /**
   * Update a device
   * @param deviceId - Device ID
   * @param device - Device update data
   */
  async updateDevice(deviceId: string, device: DeviceUpdate): Promise<void> {
    this.logger.debug(`Updating device: ${deviceId}`);
    try {
      await this.put<void>(`/oapi/v1/devices/${deviceId}`, device);
    } catch (error) {
      this.handleError(error, 'Device', deviceId);
    }
  }

  /**
   * Delete a device
   * @param deviceId - Device ID
   */
  async removeDevice(deviceId: string): Promise<void> {
    this.logger.debug(`Removing device: ${deviceId}`);
    try {
      await this.delete<void>(`/oapi/v1/devices/${deviceId}`);
    } catch (error) {
      this.handleError(error, 'Device', deviceId);
    }
  }

  /**
   * Update device settings
   * @param deviceId - Device ID
   * @param settings - Settings update data
   */
  async updateDeviceSettings(deviceId: string, settings: DeviceSettingsUpdate): Promise<void> {
    this.logger.debug(`Updating device settings: ${deviceId}`);
    try {
      await this.put<void>(`/oapi/v1/devices/${deviceId}/settings`, settings);
    } catch (error) {
      this.handleError(error, 'Device', deviceId);
    }
  }

  /**
   * List dedicated addresses for a device
   * @param deviceId - Device ID
   * @returns Dedicated IPs information
   */
  async listDedicatedAddresses(deviceId: string): Promise<DedicatedIps> {
    this.logger.debug(`Listing dedicated addresses for device: ${deviceId}`);
    return this.get<DedicatedIps>(`/oapi/v1/devices/${deviceId}/dedicated_addresses`);
  }

  /**
   * Link dedicated IPv4 to a device
   * @param deviceId - Device ID
   * @param data - Link data with IP address
   */
  async linkDedicatedIPv4(deviceId: string, data: LinkDedicatedIPv4): Promise<void> {
    this.logger.debug(`Linking dedicated IPv4 ${data.ip} to device: ${deviceId}`);
    try {
      await this.post<void>(`/oapi/v1/devices/${deviceId}/dedicated_addresses/ipv4`, data);
    } catch (error) {
      this.handleError(error, 'Device', deviceId);
    }
  }

  /**
   * Unlink dedicated IPv4 from a device
   * @param deviceId - Device ID
   * @param ip - IPv4 address to unlink
   */
  async unlinkDedicatedIPv4(deviceId: string, ip: string): Promise<void> {
    this.logger.debug(`Unlinking dedicated IPv4 ${ip} from device: ${deviceId}`);
    try {
      await this.delete<void>(`/oapi/v1/devices/${deviceId}/dedicated_addresses/ipv4?ip=${ip}`);
    } catch (error) {
      this.handleError(error, 'Device', deviceId);
    }
  }

  /**
   * Get DNS-over-HTTPS mobile config
   * @param deviceId - Device ID
   * @param excludeWifiNetworks - SSIDs to exclude
   * @param excludeDomains - Domains to exclude
   * @returns Mobile config file content
   */
  async getDoHMobileConfig(
    deviceId: string,
    excludeWifiNetworks?: string[],
    excludeDomains?: string[],
  ): Promise<string> {
    this.logger.debug(`Getting DoH mobile config for device: ${deviceId}`);
    const params: Record<string, unknown> = {};
    if (excludeWifiNetworks) {
      params['exclude_wifi_networks'] = excludeWifiNetworks;
    }
    if (excludeDomains) {
      params['exclude_domain'] = excludeDomains;
    }
    return this.get<string>(`/oapi/v1/devices/${deviceId}/doh.mobileconfig`, params);
  }

  /**
   * Get DNS-over-TLS mobile config
   * @param deviceId - Device ID
   * @param excludeWifiNetworks - SSIDs to exclude
   * @param excludeDomains - Domains to exclude
   * @returns Mobile config file content
   */
  async getDoTMobileConfig(
    deviceId: string,
    excludeWifiNetworks?: string[],
    excludeDomains?: string[],
  ): Promise<string> {
    this.logger.debug(`Getting DoT mobile config for device: ${deviceId}`);
    const params: Record<string, unknown> = {};
    if (excludeWifiNetworks) {
      params['exclude_wifi_networks'] = excludeWifiNetworks;
    }
    if (excludeDomains) {
      params['exclude_domain'] = excludeDomains;
    }
    return this.get<string>(`/oapi/v1/devices/${deviceId}/dot.mobileconfig`, params);
  }

  /**
   * Reset DNS-over-HTTPS password for a device
   * @param deviceId - Device ID
   */
  async resetDOHPassword(deviceId: string): Promise<void> {
    this.logger.debug(`Resetting DoH password for device: ${deviceId}`);
    try {
      await this.put<void>(`/oapi/v1/devices/${deviceId}/doh_password/reset`);
    } catch (error) {
      this.handleError(error, 'Device', deviceId);
    }
  }
}
