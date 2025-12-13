#!/usr/bin/env node
/**
 * Command-line interface for the Rules Compiler TypeScript Frontend
 */
import type { CliOptions, VersionInfo } from './types';
/**
 * Parses command line arguments
 * @param args - Command line arguments (process.argv.slice(2))
 * @returns Parsed CLI options
 */
export declare function parseArgs(args: string[]): CliOptions;
/**
 * Shows help message
 */
export declare function showHelp(): void;
/**
 * Gets version information
 * @returns Version info object
 */
export declare function getVersionInfo(): VersionInfo;
/**
 * Shows version information
 */
export declare function showVersion(): void;
/**
 * Main CLI entry point
 * @param args - Command line arguments
 * @returns Exit code
 */
export declare function main(args?: string[]): Promise<number>;
//# sourceMappingURL=cli.d.ts.map