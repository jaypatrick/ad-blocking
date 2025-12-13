/**
 * Configuration reader with multi-format support (JSON, YAML, TOML)
 */
import type { IConfiguration } from '@adguard/hostlist-compiler';
import type { ConfigurationFormat, ExtendedConfiguration, Logger } from './types';
/**
 * Detects configuration format from file extension
 * @param filePath - Path to configuration file
 * @returns Detected format
 * @throws Error if extension is not recognized
 */
export declare function detectFormat(filePath: string): ConfigurationFormat;
/**
 * Finds a default configuration file
 * @param basePath - Base path to search from
 * @returns Path to config file or undefined
 */
export declare function findDefaultConfig(basePath?: string): string | undefined;
/**
 * Reads and parses configuration from a file
 * @param configPath - Path to configuration file
 * @param format - Optional format override
 * @param logger - Logger instance
 * @returns Parsed configuration with metadata
 * @throws Error if file doesn't exist or parsing fails
 */
export declare function readConfiguration(configPath: string, format?: ConfigurationFormat, logger?: Logger): ExtendedConfiguration;
/**
 * Converts configuration to JSON string (removes internal metadata)
 * @param config - Configuration object
 * @returns JSON string
 */
export declare function toJson(config: IConfiguration): string;
//# sourceMappingURL=config-reader.d.ts.map