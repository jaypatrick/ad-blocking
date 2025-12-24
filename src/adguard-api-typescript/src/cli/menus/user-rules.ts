/**
 * User Rules menu
 */

import { BaseMenu, MenuItem } from './base.ts';
import { UserRulesRepository } from '../../repositories/user-rules.ts';
import { DnsServerRepository } from '../../repositories/dns-server.ts';
import { RulesCompilerIntegration } from '../../rules-compiler-integration.ts';
import {
  createTable,
  displayTable,
  showPanel,
  showSuccess,
  showNoItems,
  showInfo,
  withSpinner,
} from '../utils.ts';

export class UserRulesMenu extends BaseMenu {
  protected get title(): string {
    return 'User Rules';
  }

  constructor(
    private readonly userRulesRepo: UserRulesRepository,
    private readonly dnsServerRepo: DnsServerRepository,
    private readonly rulesIntegration: RulesCompilerIntegration
  ) {
    super();
  }

  protected getMenuItems(): MenuItem[] {
    return [
      { name: 'View user rules', action: () => this.viewRules() },
      { name: 'Add a rule', action: () => this.addRule() },
      { name: 'Remove a rule', action: () => this.removeRule() },
      { name: 'Clear all rules', action: () => this.clearRules() },
      { name: 'Toggle user rules', action: () => this.toggleRules() },
      { name: 'Sync from file', action: () => this.syncFromFile() },
      { name: 'Sync compiled rules', action: () => this.syncCompiledRules() },
    ];
  }

  private async selectDnsServer(): Promise<{ id: string; name: string } | undefined> {
    const servers = await withSpinner('Loading DNS servers...', () =>
      this.dnsServerRepo.getAll()
    );

    return this.selectItem(
      'Select a DNS server:',
      servers,
      s => `${s.name} (${s.id})`
    );
  }

  private async viewRules(): Promise<void> {
    const server = await this.selectDnsServer();
    if (!server) return;

    const rules = await withSpinner('Loading user rules...', () =>
      this.userRulesRepo.getRules(server.id)
    );

    showPanel(`User Rules for: ${server.name}`, {
      Enabled: rules.enabled,
      'Rule Count': rules.rules.length,
    });

    if (rules.rules.length === 0) {
      showNoItems('rules');
      return;
    }

    const table = createTable(['#', 'Rule']);
    rules.rules.forEach((rule, index) => {
      table.push([(index + 1).toString(), rule]);
    });
    displayTable(table);
  }

  private async addRule(): Promise<void> {
    const server = await this.selectDnsServer();
    if (!server) return;

    const rule = await this.getInput('Enter rule (e.g., ||example.com^):');
    if (!rule.trim()) return;

    await withSpinner('Adding rule...', () =>
      this.userRulesRepo.addRule(server.id, rule.trim())
    );

    showSuccess(`Rule added to ${server.name}`);
  }

  private async removeRule(): Promise<void> {
    const server = await this.selectDnsServer();
    if (!server) return;

    const rules = await withSpinner('Loading rules...', () =>
      this.userRulesRepo.getRules(server.id)
    );

    if (rules.rules.length === 0) {
      showNoItems('rules');
      return;
    }

    const rule = await this.selectItem(
      'Select a rule to remove:',
      rules.rules,
      r => r
    );

    if (!rule) return;

    await withSpinner('Removing rule...', () =>
      this.userRulesRepo.removeRule(server.id, rule)
    );

    showSuccess(`Rule removed from ${server.name}`);
  }

  private async clearRules(): Promise<void> {
    const server = await this.selectDnsServer();
    if (!server) return;

    const confirmed = await this.confirm(
      `Are you sure you want to clear all rules for "${server.name}"?`
    );

    if (!confirmed) return;

    await withSpinner('Clearing rules...', () =>
      this.userRulesRepo.clearRules(server.id)
    );

    showSuccess(`All rules cleared for ${server.name}`);
  }

  private async toggleRules(): Promise<void> {
    const server = await this.selectDnsServer();
    if (!server) return;

    const rules = await withSpinner('Loading rules...', () =>
      this.userRulesRepo.getRules(server.id)
    );

    const newState = !rules.enabled;

    await withSpinner(
      `${newState ? 'Enabling' : 'Disabling'} user rules...`,
      () =>
        newState
          ? this.userRulesRepo.enableRules(server.id)
          : this.userRulesRepo.disableRules(server.id)
    );

    showSuccess(`User rules ${newState ? 'enabled' : 'disabled'} for ${server.name}`);
  }

  private async syncFromFile(): Promise<void> {
    const server = await this.selectDnsServer();
    if (!server) return;

    const filePath = await this.getInput('Enter path to rules file:');
    if (!filePath.trim()) return;

    const result = await withSpinner('Syncing rules from file...', () =>
      this.rulesIntegration.syncRules(server.id, {
        rulesPath: filePath.trim(),
        enable: true,
      })
    );

    if (result.success) {
      showSuccess(`Synced ${result.rulesCount} rules to ${server.name}`);
    } else {
      throw new Error(result.error);
    }
  }

  private async syncCompiledRules(): Promise<void> {
    const server = await this.selectDnsServer();
    if (!server) return;

    showInfo('Syncing from rules/adguard_user_filter.txt...');

    const result = await withSpinner('Syncing compiled rules...', () =>
      this.rulesIntegration.syncCompiledRules(server.id)
    );

    if (result.success) {
      showSuccess(`Synced ${result.rulesCount} compiled rules to ${server.name}`);
    } else {
      throw new Error(result.error);
    }
  }
}
