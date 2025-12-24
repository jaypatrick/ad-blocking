/**
 * Integration with rules-compiler-typescript for compiling user rules
 *
 * This module provides utilities to compile filter rules from configuration
 * files and push them to AdGuard DNS as user rules.
 * Deno-only implementation
 */

import { isAbsolute, resolve, join } from '@std/path';
import { UserRulesRepository } from './repositories/user-rules.ts';
import { DnsServerRepository } from './repositories/dns-server.ts';
import { Logger, silentLogger } from './helpers/configuration.ts';

/** Result of a rules compilation and sync operation */
export interface RulesSyncResult {
  /** Whether the operation was successful */
  success: boolean;
  /** Number of rules compiled */
  rulesCount: number;
  /** DNS server ID that was updated */
  dnsServerId: string;
  /** Error message if failed */
  error?: string;
  /** Rules that were synced */
  rules?: string[];
}

/** Options for rules sync */
export interface RulesSyncOptions {
  /** Enable compiled rules (default: true) */
  enable?: boolean;
  /** Path to compiled rules file */
  rulesPath?: string;
  /** Inline rules to sync (alternative to rulesPath) */
  rules?: string[];
  /** Whether to append to existing rules (default: false, replaces) */
  append?: boolean;
  /** Filter out comments and empty lines (default: true) */
  filterComments?: boolean;
}

/** Rules compiler integration service */
export class RulesCompilerIntegration {
  private readonly userRulesRepo: UserRulesRepository;
  private readonly dnsServerRepo: DnsServerRepository;
  private readonly logger: Logger;

  constructor(
    userRulesRepo: UserRulesRepository,
    dnsServerRepo: DnsServerRepository,
    logger?: Logger
  ) {
    this.userRulesRepo = userRulesRepo;
    this.dnsServerRepo = dnsServerRepo;
    this.logger = logger ?? silentLogger;
  }

  /**
   * Read rules from a file
   * @param filePath - Path to rules file
   * @param filterComments - Whether to filter comments
   * @returns Array of rules
   */
  async readRulesFromFile(
    filePath: string,
    filterComments: boolean = true
  ): Promise<string[]> {
    this.logger.debug(`Reading rules from: ${filePath}`);

    const absolutePath = isAbsolute(filePath)
      ? filePath
      : resolve(Deno.cwd(), filePath);

    const content = await Deno.readTextFile(absolutePath);
    let lines = content.split('\n');

    if (filterComments) {
      lines = lines.filter((line: string) => {
        const trimmed = line.trim();
        // Keep non-empty lines that aren't comments
        // AdGuard rules starting with ! are comments, but lines starting with || are rules
        return trimmed.length > 0 && !trimmed.startsWith('!') && !trimmed.startsWith('#');
      });
    }

    this.logger.info(`Read ${lines.length} rules from ${filePath}`);
    return lines;
  }

  /**
   * Parse rules from string content
   * @param content - Rules content as string
   * @param filterComments - Whether to filter comments
   * @returns Array of rules
   */
  parseRules(content: string, filterComments: boolean = true): string[] {
    let lines = content.split('\n');

    if (filterComments) {
      lines = lines.filter(line => {
        const trimmed = line.trim();
        return trimmed.length > 0 && !trimmed.startsWith('!') && !trimmed.startsWith('#');
      });
    }

    return lines;
  }

  /**
   * Sync rules to a DNS server
   * @param dnsServerId - DNS server ID
   * @param options - Sync options
   * @returns Sync result
   */
  async syncRules(
    dnsServerId: string,
    options: RulesSyncOptions
  ): Promise<RulesSyncResult> {
    try {
      this.logger.info(`Syncing rules to DNS server: ${dnsServerId}`);

      // Get rules from file or inline
      let rules: string[];
      if (options.rulesPath) {
        rules = await this.readRulesFromFile(
          options.rulesPath,
          options.filterComments !== false
        );
      } else if (options.rules) {
        rules = options.filterComments !== false
          ? options.rules.filter(r => r.trim().length > 0 && !r.trim().startsWith('!'))
          : options.rules;
      } else {
        throw new Error('Either rulesPath or rules must be provided');
      }

      // If appending, get existing rules first
      if (options.append) {
        const existing = await this.userRulesRepo.getRules(dnsServerId);
        rules = [...existing.rules, ...rules];
        // Deduplicate
        rules = [...new Set(rules)];
      }

      // Update rules
      await this.userRulesRepo.setRules(dnsServerId, rules);

      // Enable if requested
      if (options.enable !== false) {
        await this.userRulesRepo.enableRules(dnsServerId);
      }

      this.logger.info(`Successfully synced ${rules.length} rules to ${dnsServerId}`);

      return {
        success: true,
        rulesCount: rules.length,
        dnsServerId,
        rules,
      };
    } catch (error) {
      const message = error instanceof Error ? error.message : String(error);
      this.logger.error(`Failed to sync rules: ${message}`);

      return {
        success: false,
        rulesCount: 0,
        dnsServerId,
        error: message,
      };
    }
  }

  /**
   * Sync rules to the default DNS server
   * @param options - Sync options
   * @returns Sync result
   */
  async syncRulesToDefault(options: RulesSyncOptions): Promise<RulesSyncResult> {
    try {
      const defaultServer = await this.dnsServerRepo.getDefault();
      if (!defaultServer) {
        throw new Error('No default DNS server found');
      }
      return this.syncRules(defaultServer.id, options);
    } catch (error) {
      const message = error instanceof Error ? error.message : String(error);
      return {
        success: false,
        rulesCount: 0,
        dnsServerId: '',
        error: message,
      };
    }
  }

  /**
   * Sync rules from the rules-compiler output to a DNS server
   * This is designed to work with the compiled output from rules-compiler-typescript
   *
   * @param dnsServerId - DNS server ID
   * @param rulesDirectory - Path to rules directory (default: rules/)
   * @param rulesFile - Name of rules file (default: adguard_user_filter.txt)
   */
  async syncCompiledRules(
    dnsServerId: string,
    rulesDirectory: string = 'rules',
    rulesFile: string = 'adguard_user_filter.txt'
  ): Promise<RulesSyncResult> {
    const rulesPath = join(rulesDirectory, rulesFile);
    return this.syncRules(dnsServerId, {
      rulesPath,
      enable: true,
      filterComments: true,
      append: false,
    });
  }

  /**
   * Get the diff between current rules and new rules
   * @param dnsServerId - DNS server ID
   * @param newRules - New rules to compare
   */
  async getRulesDiff(
    dnsServerId: string,
    newRules: string[]
  ): Promise<{
    added: string[];
    removed: string[];
    unchanged: string[];
  }> {
    const current = await this.userRulesRepo.getRules(dnsServerId);
    const currentSet = new Set(current.rules);
    const newSet = new Set(newRules);

    const added = newRules.filter(r => !currentSet.has(r));
    const removed = current.rules.filter(r => !newSet.has(r));
    const unchanged = newRules.filter(r => currentSet.has(r));

    return { added, removed, unchanged };
  }

  /**
   * Validate rules format
   * @param rules - Rules to validate
   * @returns Validation result
   */
  validateRules(rules: string[]): {
    valid: boolean;
    errors: { line: number; rule: string; error: string }[];
  } {
    const errors: { line: number; rule: string; error: string }[] = [];

    rules.forEach((rule, index) => {
      const trimmed = rule.trim();

      // Skip empty lines and comments
      if (trimmed.length === 0 || trimmed.startsWith('!') || trimmed.startsWith('#')) {
        return;
      }

      // Basic validation - check for common patterns
      // This is a simplified validation, real validation would be more complex
      if (trimmed.length > 1024) {
        errors.push({
          line: index + 1,
          rule: trimmed.substring(0, 50) + '...',
          error: 'Rule exceeds maximum length (1024 characters)',
        });
      }

      // Check for invalid characters in domain rules
      if (trimmed.startsWith('||') && trimmed.includes(' ')) {
        errors.push({
          line: index + 1,
          rule: trimmed,
          error: 'Domain rule contains invalid space character',
        });
      }
    });

    return {
      valid: errors.length === 0,
      errors,
    };
  }
}

export default RulesCompilerIntegration;
