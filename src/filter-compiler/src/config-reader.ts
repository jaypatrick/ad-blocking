/**
 * @fileoverview Configuration reader with multi-format support
 * @module config-reader
 */

import { readFileSync, existsSync } from 'fs';
import { extname } from 'path';
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
 * @param content - JSON content string
 * @returns Parsed configuration
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
 * @param content - YAML content string
 * @returns Parsed configuration
 */
function parseYamlConfig(content: string): IConfiguration {
    try {
        const parsed = parseYaml(content);
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
 * @param content - TOML content string
 * @returns Parsed configuration
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
        default:
            throw new Error(`Unsupported format: ${detectedFormat}`);
    }

    // Add metadata
    const extendedConfig = config as ExtendedConfiguration;
    extendedConfig._sourceFormat = detectedFormat;
    extendedConfig._sourcePath = configPath;

    logger.debug('Configuration parsed successfully');
    return extendedConfig;
}

/**
 * Converts configuration to JSON string
 * @param config - Configuration object
 * @returns JSON string
 */
export function toJson(config: IConfiguration): string {
    // Remove metadata before serializing
    const { _sourceFormat, _sourcePath, ...cleanConfig } = config as ExtendedConfiguration;
    return JSON.stringify(cleanConfig, null, 2);
}
