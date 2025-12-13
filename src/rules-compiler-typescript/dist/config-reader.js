"use strict";
/**
 * Configuration reader with multi-format support (JSON, YAML, TOML)
 */
Object.defineProperty(exports, "__esModule", { value: true });
exports.detectFormat = detectFormat;
exports.findDefaultConfig = findDefaultConfig;
exports.readConfiguration = readConfiguration;
exports.toJson = toJson;
const node_fs_1 = require("node:fs");
const node_path_1 = require("node:path");
const yaml_1 = require("yaml");
const toml_1 = require("@iarna/toml");
const logger_1 = require("./logger");
/**
 * Detects configuration format from file extension
 * @param filePath - Path to configuration file
 * @returns Detected format
 * @throws Error if extension is not recognized
 */
function detectFormat(filePath) {
    const ext = (0, node_path_1.extname)(filePath).toLowerCase();
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
function parseJson(content) {
    try {
        return JSON.parse(content);
    }
    catch (error) {
        if (error instanceof SyntaxError) {
            throw new Error(`Invalid JSON: ${error.message}`);
        }
        throw error;
    }
}
/**
 * Parses YAML configuration
 */
function parseYamlConfig(content) {
    try {
        const parsed = (0, yaml_1.parse)(content);
        if (!parsed || typeof parsed !== 'object') {
            throw new Error('Invalid YAML: parsed result is not an object');
        }
        return parsed;
    }
    catch (error) {
        const message = error instanceof Error ? error.message : 'Unknown error';
        throw new Error(`Invalid YAML: ${message}`);
    }
}
/**
 * Parses TOML configuration
 */
function parseTomlConfig(content) {
    try {
        const parsed = (0, toml_1.parse)(content);
        return parsed;
    }
    catch (error) {
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
    '../../rules/Config/compiler-config.json',
];
/**
 * Finds a default configuration file
 * @param basePath - Base path to search from
 * @returns Path to config file or undefined
 */
function findDefaultConfig(basePath = process.cwd()) {
    for (const configPath of DEFAULT_CONFIG_PATHS) {
        const fullPath = (0, node_path_1.resolve)(basePath, configPath);
        if ((0, node_fs_1.existsSync)(fullPath)) {
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
function readConfiguration(configPath, format, logger = logger_1.logger) {
    logger.debug(`Reading configuration from: ${configPath}`);
    if (!(0, node_fs_1.existsSync)(configPath)) {
        throw new Error(`Configuration file not found: ${configPath}`);
    }
    const content = (0, node_fs_1.readFileSync)(configPath, 'utf8');
    const detectedFormat = format ?? detectFormat(configPath);
    logger.debug(`Configuration format: ${detectedFormat}`);
    let config;
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
            const exhaustiveCheck = detectedFormat;
            throw new Error(`Unsupported format: ${String(exhaustiveCheck)}`);
        }
    }
    // Add metadata
    const extendedConfig = config;
    extendedConfig._sourceFormat = detectedFormat;
    extendedConfig._sourcePath = configPath;
    const configRecord = config;
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
function toJson(config) {
    const { _sourceFormat: _sf, _sourcePath: _sp, ...cleanConfig } = config;
    // Variables prefixed with _ to indicate they're intentionally unused
    void _sf;
    void _sp;
    return JSON.stringify(cleanConfig, null, 2);
}
//# sourceMappingURL=config-reader.js.map