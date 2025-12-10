/**
 * @fileoverview Command-line interface for the AdGuard Filter Compiler
 * @module cli
 */

import { resolve, join, dirname } from 'path';
import type { CliOptions, ConfigurationFormat, VersionInfo } from './types';
import { runCompiler } from './compiler';
import { createLogger } from './logger';

// Package version (loaded dynamically)
const VERSION = '2.0.0';

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
        debug: false
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
            case '-f':
            case '--format':
                if (!['json', 'yaml', 'toml'].includes(nextArg)) {
                    throw new Error(`Invalid format: ${nextArg}. Must be json, yaml, or toml.`);
                }
                options.format = nextArg as ConfigurationFormat;
                i++;
                break;
            case '-v':
            case '--version':
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
            default:
                // Allow positional config path
                if (!arg.startsWith('-') && !options.configPath) {
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
AdGuard Filter Rules Compiler (TypeScript API)

Usage: npx ts-node src/cli.ts [OPTIONS]

Options:
  -c, --config PATH    Path to configuration file (default: compiler-config.json)
  -o, --output PATH    Path to output file (default: output/compiled-TIMESTAMP.txt)
  -r, --copy-to-rules  Copy output to rules directory
  -f, --format FORMAT  Force configuration format (json, yaml, toml)
  -v, --version        Show version information
  -h, --help           Show this help message
  -d, --debug          Enable debug output

Supported Configuration Formats:
  - JSON (.json)
  - YAML (.yaml, .yml)
  - TOML (.toml)

Examples:
  npx ts-node src/cli.ts
  npx ts-node src/cli.ts -c config.yaml -r
  npx ts-node src/cli.ts --config config.toml --output my-rules.txt
  npm run compile -- -c config.yaml -r

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
            arch: process.arch
        }
    };
}

/**
 * Shows version information
 */
export function showVersion(): void {
    const info = getVersionInfo();

    console.log('AdGuard Filter Rules Compiler (TypeScript API)');
    console.log(`Version: ${info.moduleVersion}`);
    console.log('');
    console.log('Platform Information:');
    console.log(`  OS: ${info.platform.os}`);
    console.log(`  Architecture: ${info.platform.arch}`);
    console.log(`  Node.js: ${info.nodeVersion}`);
}

/**
 * Main CLI entry point
 * @param args - Command line arguments
 * @returns Exit code
 */
export async function main(args: string[] = process.argv.slice(2)): Promise<number> {
    try {
        const options = parseArgs(args);

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

        // Create logger
        const logger = createLogger(options.debug);

        // Determine config path
        const configPath = options.configPath
            ? resolve(options.configPath)
            : resolve(process.cwd(), 'compiler-config.json');

        logger.info('AdGuard Filter Compiler starting...');

        // Run compilation
        const result = await runCompiler({
            configPath,
            outputPath: options.outputPath ? resolve(options.outputPath) : undefined,
            copyToRules: options.copyToRules,
            format: options.format,
            logger
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

            logger.info('Done!');
            return 0;
        } else {
            logger.error(`Compilation failed: ${result.errorMessage}`);
            return 1;
        }
    } catch (error) {
        const message = error instanceof Error ? error.message : 'Unknown error';
        console.error(`[ERROR] ${message}`);
        return 1;
    }
}

// Run if executed directly
if (require.main === module) {
    main().then(code => process.exit(code));
}
