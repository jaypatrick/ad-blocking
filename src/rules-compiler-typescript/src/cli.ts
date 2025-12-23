#!/usr/bin/env node
/**
 * Command-line interface for the Rules Compiler TypeScript Frontend
 * Production-ready with graceful shutdown and structured error handling
 */

import { resolve } from 'node:path';
import type { CliOptions, ConfigurationFormat, VersionInfo } from './types.js';
import { runCompiler } from './compiler.js';
import { findDefaultConfig, readConfiguration, toJson } from './config-reader.js';
import { createLogger, createProductionLogger } from './logger.js';
import { initializeShutdownHandler, ShutdownHandler } from './shutdown.js';
import { isCompilerError } from './errors.js';

/** Package version */
const VERSION = '1.0.0';

/**
 * Parses command line arguments
 * @param args - Command line arguments (process.argv.slice(2))
 * @returns Parsed CLI options
 */
export function parseArgs(args: string[]): CliOptions {
  const options: CliOptions = {
    copyToRules: false,
    version: false,
    help: false,
    debug: false,
    showConfig: false,
  };

  for (let i = 0; i < args.length; i++) {
    const arg = args[i];
    const nextArg = args[i + 1];

    switch (arg) {
      case '-c':
      case '--config':
        options.configPath = nextArg;
        i++;
        break;
      case '-o':
      case '--output':
        options.outputPath = nextArg;
        i++;
        break;
      case '-r':
      case '--copy-to-rules':
        options.copyToRules = true;
        break;
      case '--rules-dir':
        options.rulesDirectory = nextArg;
        i++;
        break;
      case '-f':
      case '--format':
        if (!nextArg || !['json', 'yaml', 'toml'].includes(nextArg)) {
          throw new Error(`Invalid format: ${nextArg}. Must be json, yaml, or toml.`);
        }
        options.format = nextArg as ConfigurationFormat;
        i++;
        break;
      case '-v':
      case '--version':
        options.version = true;
        break;
      case '-V':
      case '--version-info':
        options.version = true;
        break;
      case '-h':
      case '--help':
        options.help = true;
        break;
      case '-d':
      case '--debug':
        options.debug = true;
        break;
      case '--show-config':
        options.showConfig = true;
        break;
      default:
        // Allow positional config path
        if (arg && !arg.startsWith('-') && !options.configPath) {
          options.configPath = arg;
        }
    }
  }

  return options;
}

/**
 * Shows help message
 */
export function showHelp(): void {
  console.log(`
AdGuard Filter Rules Compiler (TypeScript Frontend)

Usage: rules-compiler [OPTIONS] [CONFIG_PATH]

Options:
  -c, --config PATH     Path to configuration file
  -o, --output PATH     Path to output file (default: output/compiled-TIMESTAMP.txt)
  -r, --copy-to-rules   Copy output to rules directory
  --rules-dir PATH      Custom rules directory path
  -f, --format FORMAT   Force configuration format (json, yaml, toml)
  -v, --version         Show version information
  -h, --help            Show this help message
  -d, --debug           Enable debug output
  --show-config         Show parsed configuration (don't compile)

Production Options:
  --json-logs           Use JSON format for log output (structured logging)
  --timeout MS          Compilation timeout in milliseconds (default: 300000)

Environment Variables:
  DEBUG                 Enable debug logging
  LOG_FORMAT=json       Enable JSON log format
  LOG_LEVEL             Set log level (DEBUG, INFO, WARN, ERROR, SILENT)

Supported Configuration Formats:
  - JSON  (.json)
  - YAML  (.yaml, .yml)
  - TOML  (.toml)

Examples:
  rules-compiler                          # Use default config
  rules-compiler -c config.yaml           # Specific config file
  rules-compiler -c config.json -r        # Compile and copy to rules
  rules-compiler -c config.toml -o out.txt
  rules-compiler --show-config -c config.yaml
  rules-compiler --json-logs -c config.yaml  # Production mode with JSON logs
  rules-compiler --timeout 60000 -c config.yaml  # 60 second timeout
`);
}

/**
 * Gets version information
 * @returns Version info object
 */
export function getVersionInfo(): VersionInfo {
  return {
    moduleVersion: VERSION,
    nodeVersion: process.version,
    platform: {
      os: process.platform,
      arch: process.arch,
    },
  };
}

/**
 * Shows version information
 */
export function showVersion(): void {
  const info = getVersionInfo();

  console.log('AdGuard Filter Rules Compiler (TypeScript Frontend)');
  console.log(`Version: ${info.moduleVersion}`);
  console.log('');
  console.log('Platform Information:');
  console.log(`  OS: ${info.platform.os}`);
  console.log(`  Architecture: ${info.platform.arch}`);
  console.log(`  Node.js: ${info.nodeVersion}`);
}

/**
 * Formats transformations value for display
 * @param value - Transformations value from config
 * @returns Formatted string
 */
function formatTransformations(value: unknown): string {
  if (Array.isArray(value)) {
    return value.join(', ');
  }
  if (typeof value === 'string') {
    return value;
  }
  return 'none';
}

/**
 * Shows configuration details
 * @param configPath - Path to configuration file
 * @param format - Optional format override
 */
function showConfig(configPath: string, format?: ConfigurationFormat): void {
  const logger = createLogger(false);
  const config = readConfiguration(configPath, format, logger);

  console.log(`Configuration: ${configPath}`);
  console.log('');
  console.log(`  Name: ${config.name}`);

  const configRecord = config as unknown as Record<string, unknown>;
  const versionValue = configRecord['version'];
  const version = typeof versionValue === 'string' || typeof versionValue === 'number' ? String(versionValue) : 'N/A';
  console.log(`  Version: ${version}`);

  const licenseValue = configRecord['license'];
  const license = typeof licenseValue === 'string' ? licenseValue : 'N/A';
  console.log(`  License: ${license}`);

  console.log(`  Sources: ${config.sources?.length || 0}`);
  console.log(`  Transformations: ${formatTransformations(configRecord['transformations'])}`);
  
  console.log('');
  console.log('JSON representation:');
  console.log(toJson(config));
}

/**
 * Extended CLI options with production features
 */
interface ExtendedCliOptions extends CliOptions {
  /** Use JSON format for logging */
  jsonLogs: boolean;
  /** Compilation timeout in milliseconds */
  timeout?: number;
}

/**
 * Parses extended command line arguments
 */
function parseExtendedArgs(args: string[]): ExtendedCliOptions {
  const baseOptions = parseArgs(args);
  const extendedOptions: ExtendedCliOptions = {
    ...baseOptions,
    jsonLogs: false,
  };

  for (let i = 0; i < args.length; i++) {
    const arg = args[i];
    const nextArg = args[i + 1];

    switch (arg) {
      case '--json-logs':
        extendedOptions.jsonLogs = true;
        break;
      case '--timeout':
        if (!nextArg) {
          throw new Error('Timeout value is required');
        }
        extendedOptions.timeout = parseInt(nextArg, 10);
        if (isNaN(extendedOptions.timeout)) {
          throw new Error(`Invalid timeout value: ${nextArg}`);
        }
        i++;
        break;
    }
  }

  return extendedOptions;
}

/**
 * Main CLI entry point
 * @param args - Command line arguments
 * @returns Exit code
 */
export async function main(args: string[] = process.argv.slice(2)): Promise<number> {
  let shutdownHandler: ShutdownHandler | undefined;

  try {
    const options = parseExtendedArgs(args);

    // Handle help
    if (options.help) {
      showHelp();
      return 0;
    }

    // Handle version
    if (options.version) {
      showVersion();
      return 0;
    }

    // Create logger (JSON format for production, human-readable for development)
    const logger = options.jsonLogs
      ? createProductionLogger()
      : createLogger(options.debug);

    // Initialize graceful shutdown handler
    shutdownHandler = initializeShutdownHandler({ logger });

    // Determine config path
    let configPath: string;
    if (options.configPath) {
      configPath = resolve(options.configPath);
    } else {
      const defaultConfig = findDefaultConfig();
      if (!defaultConfig) {
        logger.error('Configuration file not found.');
        console.error('Searched:');
        console.error('  - compiler-config.json');
        console.error('  - compiler-config.yaml');
        console.error('  - compiler-config.yml');
        console.error('  - compiler-config.toml');
        console.error('');
        console.error('Specify config path with -c/--config');
        return 1;
      }
      configPath = defaultConfig;
    }

    // Show config only
    if (options.showConfig) {
      showConfig(configPath, options.format);
      return 0;
    }

    // Check for shutdown before starting
    shutdownHandler.assertNotShuttingDown();

    logger.info('AdGuard Filter Rules Compiler starting...');
    logger.info(`Configuration: ${configPath}`);

    // Run compilation
    const result = await runCompiler({
      configPath,
      outputPath: options.outputPath ? resolve(options.outputPath) : undefined,
      copyToRules: options.copyToRules,
      rulesDirectory: options.rulesDirectory,
      format: options.format,
      logger,
      timeoutMs: options.timeout,
    });

    if (result.success) {
      console.log('');
      console.log('Results:');
      console.log(`  Config Name:  ${result.configName}`);
      console.log(`  Config Ver:   ${result.configVersion}`);
      console.log(`  Rule Count:   ${result.ruleCount.toLocaleString()}`);
      console.log(`  Output Path:  ${result.outputPath}`);
      console.log(`  Hash:         ${result.outputHash.slice(0, 32)}...`);
      console.log(`  Elapsed:      ${result.elapsedMs}ms`);

      if (result.copiedToRules) {
        console.log(`  Copied To:    ${result.rulesDestination}`);
      }

      console.log('');
      logger.info('Done!');
      return 0;
    } else {
      if (result.errorCode) {
        logger.error(`[${result.errorCode}] ${result.errorMessage}`);
      } else {
        logger.error(`Compilation failed: ${result.errorMessage}`);
      }
      return 1;
    }
  } catch (error) {
    if (isCompilerError(error)) {
      console.error(`[ERROR] ${error.toLogString()}`);
    } else {
      const message = error instanceof Error ? error.message : 'Unknown error';
      console.error(`[ERROR] ${message}`);
    }
    return 1;
  } finally {
    // Clean up shutdown handler
    if (shutdownHandler) {
      shutdownHandler.unlisten();
    }
  }
}

// Run if executed directly
if (require.main === module) {
  void main().then((code) => process.exit(code));
}
