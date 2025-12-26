/**
 * Type definitions for the Rules Compiler TypeScript Frontend
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
  /** Custom rules directory path */
  rulesDirectory?: string;
  /** Force configuration format */
  format?: ConfigurationFormat;
  /** Show version information */
  version: boolean;
  /** Show help */
  help: boolean;
  /** Enable debug output */
  debug: boolean;
  /** Show config only (don't compile) */
  showConfig: boolean;
  /** Run in interactive mode */
  interactive: boolean;
  /** Run in compile mode (non-interactive) */
  compile: boolean;
  /** Validate configuration only */
  validate: boolean;
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
  /** Error code if failed (from CompilerError) */
  errorCode?: string;
}

/**
 * Platform information
 */
export interface PlatformInfo {
  /** Operating system name */
  os: string;
  /** CPU architecture */
  arch: string;
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
  platform: PlatformInfo;
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

/**
 * Options for running the compiler
 */
export interface CompileOptions {
  /** Path to configuration file */
  configPath: string;
  /** Path to output file */
  outputPath?: string;
  /** Copy output to rules directory */
  copyToRules?: boolean;
  /** Custom rules directory path */
  rulesDirectory?: string;
  /** Force configuration format */
  format?: ConfigurationFormat;
  /** Logger instance */
  logger?: Logger;
}
