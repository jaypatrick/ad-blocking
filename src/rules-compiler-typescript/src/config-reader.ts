/**
 * Configuration reader with multi-format support (JSON, YAML, TOML)
 */

import { readFileSync, existsSync } from 'node:fs';
import { extname, resolve } from 'node:path';
import { parse as parseYaml } from 'yaml';
import { parse as parseToml } from '@iarna/toml';
import type { IConfiguration } from '@adguard/hostlist-compiler';
import type { ConfigurationFormat, ExtendedConfiguration, Logger } from './types';
import { logger as defaultLogger } from './logger';

/**
 * Detects configuration format from file extension
 * @param filePath - Path to configuration file
 * @returns Detected format
 * @throws Error if extension is not recognized
 */
export function detectFormat(filePath: string): ConfigurationFormat {
  const ext = extname(filePath).toLowerCase();

  switch (ext) {
    case '.json':
      return 'json';
    case '.yaml':
    case '.yml':
      return 'yaml';
    case '.toml':
      return 'toml';
    default:
      throw new Error(`Unknown configuration file extension: ${ext}`);
  }
}

/**
 * Parses JSON configuration
 */
function parseJson(content: string): IConfiguration {
  try {
    return JSON.parse(content) as IConfiguration;
  } catch (error) {
    if (error instanceof SyntaxError) {
      throw new Error(`Invalid JSON: ${error.message}`);
    }
    throw error;
  }
}

/**
 * Parses YAML configuration
 */
function parseYamlConfig(content: string): IConfiguration {
  try {
    const parsed = parseYaml(content) as unknown;
    if (!parsed || typeof parsed !== 'object') {
      throw new Error('Invalid YAML: parsed result is not an object');
    }
    return parsed as IConfiguration;
  } catch (error) {
    const message = error instanceof Error ? error.message : 'Unknown error';
    throw new Error(`Invalid YAML: ${message}`);
  }
}

/**
 * Parses TOML configuration
 */
function parseTomlConfig(content: string): IConfiguration {
  try {
    const parsed = parseToml(content);
    return parsed as unknown as IConfiguration;
  } catch (error) {
    const message = error instanceof Error ? error.message : 'Unknown error';
    throw new Error(`Invalid TOML: ${message}`);
  }
}

/**
 * Default config search paths
 */
const DEFAULT_CONFIG_PATHS = [
  'compiler-config.json',
  'compiler-config.yaml',
  'compiler-config.yml',
  'compiler-config.toml',
  '../rules-compiler-typescript/compiler-config.json',
];

/**
 * Finds a default configuration file
 * @param basePath - Base path to search from
 * @returns Path to config file or undefined
 */
export function findDefaultConfig(basePath: string = process.cwd()): string | undefined {
  for (const configPath of DEFAULT_CONFIG_PATHS) {
    const fullPath = resolve(basePath, configPath);
    if (existsSync(fullPath)) {
      return fullPath;
    }
  }
  return undefined;
}

/**
 * Reads and parses configuration from a file
 * @param configPath - Path to configuration file
 * @param format - Optional format override
 * @param logger - Logger instance
 * @returns Parsed configuration with metadata
 * @throws Error if file doesn't exist or parsing fails
 */
export function readConfiguration(
  configPath: string,
  format?: ConfigurationFormat,
  logger: Logger = defaultLogger
): ExtendedConfiguration {
  logger.debug(`Reading configuration from: ${configPath}`);

  if (!existsSync(configPath)) {
    throw new Error(`Configuration file not found: ${configPath}`);
  }

  const content = readFileSync(configPath, 'utf8');
  const detectedFormat = format ?? detectFormat(configPath);

  logger.debug(`Configuration format: ${detectedFormat}`);

  let config: IConfiguration;

  switch (detectedFormat) {
    case 'json':
      config = parseJson(content);
      break;
    case 'yaml':
      config = parseYamlConfig(content);
      break;
    case 'toml':
      config = parseTomlConfig(content);
      break;
    default: {
      const exhaustiveCheck: never = detectedFormat;
      throw new Error(`Unsupported format: ${String(exhaustiveCheck)}`);
    }
  }

  // Add metadata
  const extendedConfig = config as ExtendedConfiguration;
  extendedConfig._sourceFormat = detectedFormat;
  extendedConfig._sourcePath = configPath;

  const configRecord = config as unknown as Record<string, unknown>;
  const versionValue = configRecord.version;
  const version = typeof versionValue === 'string' || typeof versionValue === 'number' ? String(versionValue) : 'unknown';
  logger.info(`Loaded configuration: ${config.name} v${version}`);
  return extendedConfig;
}

/**
 * Converts configuration to JSON string (removes internal metadata)
 * @param config - Configuration object
 * @returns JSON string
 */
export function toJson(config: IConfiguration): string {
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const { _sourceFormat, _sourcePath, ...cleanConfig } = config as ExtendedConfiguration;
  return JSON.stringify(cleanConfig, null, 2);
}
