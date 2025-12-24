#!/usr/bin/env -S deno run --allow-read --allow-write --allow-env --allow-net

/**
 * AdGuard DNS CLI
 * Interactive console UI for managing AdGuard DNS
 * Deno-compatible implementation
 */

import { Command } from 'commander';
import inquirer from 'inquirer';
import chalk from 'chalk';
import { AdGuardDnsClient } from '../client.ts';
import { RulesCompilerIntegration } from '../rules-compiler-integration.ts';
import { consoleLogger, maskApiKey } from '../helpers/configuration.ts';
import { showHeader, showSuccess, showError, showInfo, withSpinner } from './utils.ts';
import { DevicesMenu } from './menus/devices.ts';
import { DnsServersMenu } from './menus/dns-servers.ts';
import { UserRulesMenu } from './menus/user-rules.ts';
import { StatisticsMenu } from './menus/statistics.ts';
import { QueryLogMenu } from './menus/query-log.ts';
import { AccountMenu } from './menus/account.ts';
import { VERSION, API_VERSION } from '../index.ts';

const program = new Command();

program
  .name('adguard-dns')
  .description('AdGuard DNS API CLI - Interactive console for managing AdGuard DNS')
  .version(`${VERSION} (API v${API_VERSION})`)
  .option('-k, --api-key <key>', 'AdGuard DNS API key')
  .option('-e, --env-var <name>', 'Environment variable containing API key', 'ADGUARD_API_KEY')
  .option('-v, --verbose', 'Enable verbose logging')
  .action(async (options) => {
    await runInteractive(options);
  });

// Sync command for non-interactive use
program
  .command('sync')
  .description('Sync rules from file to AdGuard DNS')
  .requiredOption('-f, --file <path>', 'Path to rules file')
  .option('-s, --server <id>', 'DNS server ID (uses default if not specified)')
  .option('-k, --api-key <key>', 'AdGuard DNS API key')
  .option('-e, --env-var <name>', 'Environment variable containing API key', 'ADGUARD_API_KEY')
  .option('-a, --append', 'Append to existing rules instead of replacing')
  .option('-v, --verbose', 'Enable verbose logging')
  .action(async (options) => {
    await runSync(options);
  });

async function getApiKey(options: { apiKey?: string; envVar?: string }): Promise<string> {
  // Check command line option
  if (options.apiKey) {
    return options.apiKey;
  }

  // Check environment variable
  const envVar = options.envVar || 'ADGUARD_API_KEY';
  const envKey = Deno.env.get(envVar);
  if (envKey) {
    return envKey;
  }

  // Prompt for API key
  const { apiKey } = await inquirer.prompt([
    {
      type: 'password',
      name: 'apiKey',
      message: 'Enter your AdGuard DNS API key:',
      validate: (input: string) => input.trim().length > 0 || 'API key is required',
    },
  ]);

  return apiKey;
}

async function runInteractive(options: { apiKey?: string; envVar?: string; verbose?: boolean }): Promise<void> {
  console.log();
  console.log(chalk.bold.blue('━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━'));
  console.log(chalk.bold.blue('  AdGuard DNS CLI'));
  console.log(chalk.bold.blue(`  Version ${VERSION} | API v${API_VERSION}`));
  console.log(chalk.bold.blue('━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━'));
  console.log();

  try {
    const apiKey = await getApiKey(options);
    const logger = options.verbose ? consoleLogger : undefined;

    showInfo(`Connecting with API key: ${maskApiKey(apiKey)}`);

    const client = AdGuardDnsClient.withApiKey(apiKey, logger);

    // Test connection
    const connected = await withSpinner('Testing connection...', () =>
      client.testConnection()
    );

    if (!connected) {
      showError('Failed to connect to AdGuard DNS API. Please check your API key.');
      Deno.exit(1);
    }

    showSuccess('Connected to AdGuard DNS API');

    // Create menus
    const rulesIntegration = new RulesCompilerIntegration(
      client.userRulesRepository,
      client.dnsServerRepository,
      logger
    );

    const menus = {
      devices: new DevicesMenu(client.deviceRepository, client.dnsServerRepository),
      dnsServers: new DnsServersMenu(client.dnsServerRepository),
      userRules: new UserRulesMenu(
        client.userRulesRepository,
        client.dnsServerRepository,
        rulesIntegration
      ),
      statistics: new StatisticsMenu(client.statisticsRepository),
      queryLog: new QueryLogMenu(client.queryLogRepository),
      account: new AccountMenu(client.account, client.filterLists, client.webServices),
    };

    // Main menu loop
    while (true) {
      showHeader('Main Menu');

      const { choice } = await inquirer.prompt([
        {
          type: 'list',
          name: 'choice',
          message: 'Select an option:',
          choices: [
            { name: '📱 Devices', value: 'devices' },
            { name: '🖥️  DNS Servers', value: 'dnsServers' },
            { name: '📝 User Rules', value: 'userRules' },
            { name: '📊 Statistics', value: 'statistics' },
            { name: '📋 Query Log', value: 'queryLog' },
            { name: '👤 Account & Info', value: 'account' },
            new inquirer.Separator(),
            { name: '🚪 Exit', value: 'exit' },
          ],
        },
      ]);

      if (choice === 'exit') {
        console.log();
        showInfo('Goodbye!');
        Deno.exit(0);
      }

      const menu = menus[choice as keyof typeof menus];
      if (menu) {
        try {
          await menu.show();
        } catch (error) {
          showError(error instanceof Error ? error.message : String(error));
        }
      }
    }
  } catch (error) {
    showError(error instanceof Error ? error.message : String(error));
    Deno.exit(1);
  }
}

async function runSync(options: {
  file: string;
  server?: string;
  apiKey?: string;
  envVar?: string;
  append?: boolean;
  verbose?: boolean;
}): Promise<void> {
  try {
    const apiKey = await getApiKey(options);
    const logger = options.verbose ? consoleLogger : undefined;

    const client = AdGuardDnsClient.withApiKey(apiKey, logger);

    const rulesIntegration = new RulesCompilerIntegration(
      client.userRulesRepository,
      client.dnsServerRepository,
      logger
    );

    let result;
    if (options.server) {
      result = await rulesIntegration.syncRules(options.server, {
        rulesPath: options.file,
        append: options.append,
        enable: true,
      });
    } else {
      result = await rulesIntegration.syncRulesToDefault({
        rulesPath: options.file,
        append: options.append,
        enable: true,
      });
    }

    if (result.success) {
      showSuccess(`Synced ${result.rulesCount} rules to ${result.dnsServerId}`);
    } else {
      showError(`Failed to sync rules: ${result.error}`);
      Deno.exit(1);
    }
  } catch (error) {
    showError(error instanceof Error ? error.message : String(error));
    Deno.exit(1);
  }
}

program.parse();
