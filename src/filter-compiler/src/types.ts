/**
 * @fileoverview Type definitions for the AdGuard Filter Compiler
 * @module types
 */

import type { IConfiguration } from '@adguard/hostlist-compiler';

/**
 * Supported configuration file formats
 */
export type ConfigurationFormat = 'json' | 'yaml' | 'toml';

/**
 * CLI options parsed from command line arguments
 */
export interface CliOptions {
    /** Path to configuration file */
    configPath?: string;
    /** Path to output file */
    outputPath?: string;
    /** Copy output to rules directory */
    copyToRules: boolean;
    /** Force configuration format */
    format?: ConfigurationFormat;
    /** Show version information */
    version: boolean;
    /** Show help */
    help: boolean;
    /** Enable debug output */
    debug: boolean;
}

/**
 * Result of a compilation operation
 */
export interface CompilerResult {
    /** Whether compilation was successful */
    success: boolean;
    /** Name from configuration */
    configName: string;
    /** Version from configuration */
    configVersion: string;
    /** Number of rules in output */
    ruleCount: number;
    /** Path to output file */
    outputPath: string;
    /** SHA-384 hash of output file */
    outputHash: string;
    /** Whether output was copied to rules */
    copiedToRules: boolean;
    /** Destination path if copied */
    rulesDestination?: string;
    /** Elapsed time in milliseconds */
    elapsedMs: number;
    /** Start time */
    startTime: Date;
    /** End time */
    endTime: Date;
    /** Error message if failed */
    errorMessage?: string;
}

/**
 * Version information for all components
 */
export interface VersionInfo {
    /** Module version */
    moduleVersion: string;
    /** Node.js version */
    nodeVersion: string;
    /** Platform information */
    platform: {
        os: string;
        arch: string;
    };
    /** hostlist-compiler version (if available) */
    hostlistCompilerVersion?: string;
}

/**
 * Extended configuration with source format tracking
 */
export interface ExtendedConfiguration extends IConfiguration {
    /** Original file format */
    _sourceFormat?: ConfigurationFormat;
    /** Original file path */
    _sourcePath?: string;
}

/**
 * Logger interface for consistent logging
 */
export interface Logger {
    info(message: string, ...args: unknown[]): void;
    warn(message: string, ...args: unknown[]): void;
    error(message: string, ...args: unknown[]): void;
    debug(message: string, ...args: unknown[]): void;
}
