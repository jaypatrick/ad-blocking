"use strict";
/**
 * Core compiler service for filter rules compilation
 */
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.writeOutput = writeOutput;
exports.countRules = countRules;
exports.computeHash = computeHash;
exports.copyToRulesDirectory = copyToRulesDirectory;
exports.compileFilters = compileFilters;
exports.runCompiler = runCompiler;
const hostlist_compiler_1 = __importDefault(require("@adguard/hostlist-compiler"));
const node_fs_1 = require("node:fs");
const node_path_1 = require("node:path");
const node_crypto_1 = require("node:crypto");
const config_reader_1 = require("./config-reader");
const logger_1 = require("./logger");
/**
 * Writes compiled rules to an output file
 * @param outputPath - Path to output file
 * @param rules - Array of compiled rules
 * @param logger - Logger instance
 */
function writeOutput(outputPath, rules, logger = logger_1.logger) {
    logger.debug(`Writing ${rules.length} rules to: ${outputPath}`);
    // Ensure output directory exists
    const outputDir = (0, node_path_1.dirname)(outputPath);
    if (!(0, node_fs_1.existsSync)(outputDir)) {
        (0, node_fs_1.mkdirSync)(outputDir, { recursive: true });
        logger.debug(`Created output directory: ${outputDir}`);
    }
    const content = rules.join('\n');
    (0, node_fs_1.writeFileSync)(outputPath, content, 'utf8');
    logger.info(`Wrote ${rules.length} lines to ${outputPath}`);
}
/**
 * Counts non-empty, non-comment lines in a file
 * @param filePath - Path to file
 * @returns Number of rules
 */
function countRules(filePath) {
    if (!(0, node_fs_1.existsSync)(filePath)) {
        return 0;
    }
    const content = (0, node_fs_1.readFileSync)(filePath, 'utf8');
    const lines = content.split('\n');
    return lines.filter((line) => {
        const trimmed = line.trim();
        if (!trimmed)
            return false;
        if (trimmed.startsWith('!'))
            return false;
        if (trimmed.startsWith('#'))
            return false;
        return true;
    }).length;
}
/**
 * Computes SHA-384 hash of a file
 * @param filePath - Path to file
 * @returns Hex-encoded hash string
 */
function computeHash(filePath) {
    const content = (0, node_fs_1.readFileSync)(filePath);
    return (0, node_crypto_1.createHash)('sha384').update(content).digest('hex');
}
/**
 * Copies compiled output to rules directory
 * @param sourcePath - Path to source file
 * @param destPath - Path to destination file
 * @param logger - Logger instance
 */
function copyToRulesDirectory(sourcePath, destPath, logger = logger_1.logger) {
    logger.debug(`Copying ${sourcePath} to ${destPath}`);
    const destDir = (0, node_path_1.dirname)(destPath);
    if (!(0, node_fs_1.existsSync)(destDir)) {
        (0, node_fs_1.mkdirSync)(destDir, { recursive: true });
    }
    (0, node_fs_1.copyFileSync)(sourcePath, destPath);
    logger.info(`Copied to rules directory: ${destPath}`);
}
/**
 * Compiles filter rules using the hostlist-compiler
 * @param config - Compiler configuration
 * @param logger - Logger instance
 * @returns Array of compiled rules
 */
async function compileFilters(config, logger = logger_1.logger) {
    logger.info('Starting filter compilation...');
    try {
        const result = await (0, hostlist_compiler_1.default)(config);
        logger.info(`Compilation complete. Generated ${result.length} rules.`);
        return result;
    }
    catch (error) {
        const message = error instanceof Error ? error.message : 'Unknown error';
        logger.error(`Compilation failed: ${message}`);
        throw new Error(`Filter compilation failed: ${message}`);
    }
}
/**
 * Generates a timestamped output filename
 * @returns Filename with timestamp
 */
function generateOutputFilename() {
    const timestamp = new Date().toISOString().replace(/[:.]/g, '-').slice(0, 19);
    return `compiled-${timestamp}.txt`;
}
/**
 * Runs the full compilation pipeline
 * @param options - Compilation options
 * @returns Compilation result
 */
async function runCompiler(options) {
    const logger = options.logger ?? logger_1.logger;
    const startTime = new Date();
    const result = {
        success: false,
        configName: '',
        configVersion: '',
        ruleCount: 0,
        outputPath: '',
        outputHash: '',
        copiedToRules: false,
        elapsedMs: 0,
        startTime,
        endTime: new Date(),
    };
    try {
        // Read configuration
        logger.info(`Loading configuration from: ${options.configPath}`);
        const config = (0, config_reader_1.readConfiguration)(options.configPath, options.format, logger);
        result.configName = config.name ?? 'unknown';
        const configRecord = config;
        const versionValue = configRecord.version;
        result.configVersion = typeof versionValue === 'string' ? versionValue : 'unknown';
        // Determine output path
        const outputFilename = generateOutputFilename();
        const defaultOutputPath = (0, node_path_1.join)((0, node_path_1.dirname)(options.configPath), 'output', outputFilename);
        const outputPath = options.outputPath ?? defaultOutputPath;
        result.outputPath = (0, node_path_1.resolve)(outputPath);
        // Compile filters
        const rules = await compileFilters(config, logger);
        // Write output
        writeOutput(result.outputPath, rules, logger);
        // Calculate statistics
        result.ruleCount = countRules(result.outputPath);
        result.outputHash = computeHash(result.outputPath);
        logger.debug(`Hash: ${result.outputHash}`);
        // Copy to rules directory if requested
        if (options.copyToRules) {
            const rulesDir = options.rulesDirectory ?? (0, node_path_1.join)((0, node_path_1.dirname)(options.configPath), '..', '..', 'rules');
            const destPath = (0, node_path_1.join)(rulesDir, 'adguard_user_filter.txt');
            copyToRulesDirectory(result.outputPath, (0, node_path_1.resolve)(destPath), logger);
            result.copiedToRules = true;
            result.rulesDestination = (0, node_path_1.resolve)(destPath);
        }
        result.success = true;
    }
    catch (error) {
        const message = error instanceof Error ? error.message : 'Unknown error';
        result.errorMessage = message;
        logger.error(`Compilation failed: ${message}`);
    }
    result.endTime = new Date();
    result.elapsedMs = result.endTime.getTime() - startTime.getTime();
    return result;
}
//# sourceMappingURL=compiler.js.map