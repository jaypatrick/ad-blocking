/**
 * DNS Server repository
 */

import { BaseRepository } from './base.ts';
import { DnsServersApi } from '../api/dns-servers.ts';
import { Logger } from '../helpers/configuration.ts';
import {
  DNSServer,
  DNSServerCreate,
  DNSServerUpdate,
  DNSServerSettingsUpdate,
} from '../models/index.ts';

/** DNS Server repository for managing DNS servers/profiles */
export class DnsServerRepository extends BaseRepository {
  private readonly api: DnsServersApi;

  constructor(api: DnsServersApi, logger?: Logger) {
    super(logger);
    this.api = api;
  }

  /**
   * Get all DNS servers
   * @returns List of DNS servers
   */
  async getAll(): Promise<DNSServer[]> {
    return this.execute('Get all DNS servers', () => this.api.listDnsServers());
  }

  /**
   * Get a DNS server by ID
   * @param id - DNS server ID
   * @returns DNS server
   * @throws EntityNotFoundError if DNS server not found
   */
  async getById(id: string): Promise<DNSServer> {
    return this.executeWithEntityCheck(
      `Get DNS server ${id}`,
      () => this.api.getDnsServer(id),
      'DNSServer',
      id
    );
  }

  /**
   * Create a new DNS server
   * @param data - DNS server creation data
   * @returns Created DNS server
   */
  async create(data: DNSServerCreate): Promise<DNSServer> {
    return this.execute(`Create DNS server ${data.name}`, () => this.api.createDnsServer(data));
  }

  /**
   * Update a DNS server
   * @param id - DNS server ID
   * @param data - DNS server update data
   * @throws EntityNotFoundError if DNS server not found
   */
  async update(id: string, data: DNSServerUpdate): Promise<void> {
    return this.executeWithEntityCheck(
      `Update DNS server ${id}`,
      () => this.api.updateDnsServer(id, data),
      'DNSServer',
      id
    );
  }

  /**
   * Delete a DNS server
   * @param id - DNS server ID
   * @throws EntityNotFoundError if DNS server not found
   */
  async delete(id: string): Promise<void> {
    return this.executeWithEntityCheck(
      `Delete DNS server ${id}`,
      () => this.api.removeDnsServer(id),
      'DNSServer',
      id
    );
  }

  /**
   * Update DNS server settings
   * @param id - DNS server ID
   * @param settings - Settings update data
   * @throws EntityNotFoundError if DNS server not found
   */
  async updateSettings(id: string, settings: DNSServerSettingsUpdate): Promise<void> {
    return this.executeWithEntityCheck(
      `Update DNS server settings ${id}`,
      () => this.api.updateDnsServerSettings(id, settings),
      'DNSServer',
      id
    );
  }

  /**
   * Get the default DNS server
   * @returns Default DNS server or undefined if none
   */
  async getDefault(): Promise<DNSServer | undefined> {
    const servers = await this.getAll();
    return servers.find(s => s.default);
  }
}
