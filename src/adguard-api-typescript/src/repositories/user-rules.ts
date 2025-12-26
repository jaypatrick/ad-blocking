/**
 * User Rules repository
 */

import { BaseRepository } from './base.ts';
import { DnsServersApi } from '../api/dns-servers.ts';
import { Logger } from '../helpers/configuration.ts';
import { UserRulesSettings, UserRulesSettingsUpdate } from '../models/index.ts';

/** User Rules repository for managing DNS server user rules */
export class UserRulesRepository extends BaseRepository {
  private readonly api: DnsServersApi;

  constructor(api: DnsServersApi, logger?: Logger) {
    super(logger);
    this.api = api;
  }

  /**
   * Get user rules for a DNS server
   * @param dnsServerId - DNS server ID
   * @returns User rules settings
   * @throws EntityNotFoundError if DNS server not found
   */
  async getRules(dnsServerId: string): Promise<UserRulesSettings> {
    const server = await this.executeWithEntityCheck(
      `Get user rules for DNS server ${dnsServerId}`,
      async () => {
        const servers = await this.api.listDnsServers();
        const server = servers.find((s) => s.id === dnsServerId);
        if (!server) {
          throw new Error(`DNS server ${dnsServerId} not found`);
        }
        return server;
      },
      'DNSServer',
      dnsServerId,
    );

    return server.settings.user_rules_settings;
  }

  /**
   * Update user rules for a DNS server
   * @param dnsServerId - DNS server ID
   * @param settings - User rules settings update
   * @throws EntityNotFoundError if DNS server not found
   */
  async updateRules(dnsServerId: string, settings: UserRulesSettingsUpdate): Promise<void> {
    return this.executeWithEntityCheck(
      `Update user rules for DNS server ${dnsServerId}`,
      () =>
        this.api.updateDnsServerSettings(dnsServerId, {
          user_rules_settings: settings,
        }),
      'DNSServer',
      dnsServerId,
    );
  }

  /**
   * Enable user rules for a DNS server
   * @param dnsServerId - DNS server ID
   */
  async enableRules(dnsServerId: string): Promise<void> {
    return this.updateRules(dnsServerId, { enabled: true });
  }

  /**
   * Disable user rules for a DNS server
   * @param dnsServerId - DNS server ID
   */
  async disableRules(dnsServerId: string): Promise<void> {
    return this.updateRules(dnsServerId, { enabled: false });
  }

  /**
   * Set user rules (replaces all existing rules)
   * @param dnsServerId - DNS server ID
   * @param rules - Array of rules
   */
  async setRules(dnsServerId: string, rules: string[]): Promise<void> {
    return this.updateRules(dnsServerId, { rules });
  }

  /**
   * Add a rule to the DNS server
   * @param dnsServerId - DNS server ID
   * @param rule - Rule to add
   */
  async addRule(dnsServerId: string, rule: string): Promise<void> {
    const current = await this.getRules(dnsServerId);
    const rules = [...current.rules, rule];
    return this.setRules(dnsServerId, rules);
  }

  /**
   * Remove a rule from the DNS server
   * @param dnsServerId - DNS server ID
   * @param rule - Rule to remove
   */
  async removeRule(dnsServerId: string, rule: string): Promise<void> {
    const current = await this.getRules(dnsServerId);
    const rules = current.rules.filter((r) => r !== rule);
    return this.setRules(dnsServerId, rules);
  }

  /**
   * Clear all user rules for a DNS server
   * @param dnsServerId - DNS server ID
   */
  async clearRules(dnsServerId: string): Promise<void> {
    return this.setRules(dnsServerId, []);
  }
}
