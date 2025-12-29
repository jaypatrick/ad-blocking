/**
 * DNS Servers API client
 */

import { BaseApi } from './base.ts';
import type { ApiConfiguration } from '../helpers/configuration.ts';
import type {
  DNSServer,
  DNSServerCreate,
  DNSServerSettingsUpdate,
  DNSServerUpdate,
} from '../models/index.ts';

/** DNS Servers API endpoints */
export class DnsServersApi extends BaseApi {
  constructor(config: ApiConfiguration) {
    super(config);
  }

  /**
   * List all DNS servers
   * @returns Array of DNS servers
   */
  async listDnsServers(): Promise<DNSServer[]> {
    this.logger.debug('Listing DNS servers');
    return this.get<DNSServer[]>('/oapi/v1/dns_servers');
  }

  /**
   * Get a DNS server by ID
   * @param dnsServerId - DNS server ID
   * @returns DNS server information
   */
  async getDnsServer(dnsServerId: string): Promise<DNSServer> {
    this.logger.debug(`Getting DNS server: ${dnsServerId}`);
    try {
      return await this.get<DNSServer>(`/oapi/v1/dns_servers/${dnsServerId}`);
    } catch (error) {
      this.handleError(error, 'DNSServer', dnsServerId);
    }
  }

  /**
   * Create a new DNS server
   * @param dnsServer - DNS server creation data
   * @returns Created DNS server
   */
  async createDnsServer(dnsServer: DNSServerCreate): Promise<DNSServer> {
    this.logger.debug(`Creating DNS server: ${dnsServer.name}`);
    return this.post<DNSServer>('/oapi/v1/dns_servers', dnsServer);
  }

  /**
   * Update a DNS server
   * @param dnsServerId - DNS server ID
   * @param dnsServer - DNS server update data
   */
  async updateDnsServer(dnsServerId: string, dnsServer: DNSServerUpdate): Promise<void> {
    this.logger.debug(`Updating DNS server: ${dnsServerId}`);
    try {
      await this.put<void>(`/oapi/v1/dns_servers/${dnsServerId}`, dnsServer);
    } catch (error) {
      this.handleError(error, 'DNSServer', dnsServerId);
    }
  }

  /**
   * Delete a DNS server
   * @param dnsServerId - DNS server ID
   */
  async removeDnsServer(dnsServerId: string): Promise<void> {
    this.logger.debug(`Removing DNS server: ${dnsServerId}`);
    try {
      await this.delete<void>(`/oapi/v1/dns_servers/${dnsServerId}`);
    } catch (error) {
      this.handleError(error, 'DNSServer', dnsServerId);
    }
  }

  /**
   * Update DNS server settings
   * @param dnsServerId - DNS server ID
   * @param settings - Settings update data
   */
  async updateDnsServerSettings(
    dnsServerId: string,
    settings: DNSServerSettingsUpdate,
  ): Promise<void> {
    this.logger.debug(`Updating DNS server settings: ${dnsServerId}`);
    try {
      await this.put<void>(`/oapi/v1/dns_servers/${dnsServerId}/settings`, settings);
    } catch (error) {
      this.handleError(error, 'DNSServer', dnsServerId);
    }
  }
}
