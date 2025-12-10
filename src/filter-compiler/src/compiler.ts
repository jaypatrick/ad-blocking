/**
 * @fileoverview Filter compiler with multi-format configuration support
 * @module compiler
 */

import compile, { type IConfiguration } from '@adguard/hostlist-compiler';
import { writeFileSync, readFileSync, existsSync, copyFileSync, mkdirSync } from 'fs';
import { dirname, join, resolve } from 'path';
import { createHash } from 'crypto';
import type { CompilerResult, ConfigurationFormat, Logger } from './types';
import { readConfiguration, toJson } from './config-reader';
import { logger as defaultLogger } from './logger';

/**
 * Writes compiled rules to an output file
 * @param outputPath - Path to output file
 * @param rules - Array of compiled rules
 * @param logger - Logger instance
 */
export function writeOutput(outputPath: string, rules: string[], logger: Logger = defaultLogger): void {
    logger.debug(`Writing ${rules.length} rules to: ${outputPath}`);

    // Ensure output directory exists
    const outputDir = dirname(outputPath);
    if (!existsSync(outputDir)) {
        mkdirSync(outputDir, { recursive: true });
    }

    const content = rules.join('\n');
    writeFileSync(outputPath, content, 'utf8');

    logger.info(`Successfully wrote ${rules.length} lines to ${outputPath}`);
}

/**
 * Counts non-empty, non-comment lines in a file
 * @param filePath - Path to file
 * @returns Number of rules
 */
export function countRules(filePath: string): number {
    if (!existsSync(filePath)) {
        return 0;
    }

    const content = readFileSync(filePath, 'utf8');
    const lines = content.split('\n');

    return lines.filter(line => {
        const trimmed = line.trim();
        if (!trimmed) return false;
        if (trimmed.startsWith('!')) return false;
        if (trimmed.startsWith('#')) return false;
        return true;
    }).length;
}

/**
 * Computes SHA-384 hash of a file
 * @param filePath - Path to file
 * @returns Hex-encoded hash string
 */
export function computeHash(filePath: string): string {
    const content = readFileSync(filePath);
    return createHash('sha384').update(content).digest('hex');
}

/**
 * Copies compiled output to rules directory
 * @param sourcePath - Path to source file
 * @param destPath - Path to destination file
 * @param logger - Logger instance
 */
export function copyToRulesDirectory(
    sourcePath: string,
    destPath: string,
    logger: Logger = defaultLogger
): void {
    logger.debug(`Copying ${sourcePath} to ${destPath}`);

    const destDir = dirname(destPath);
    if (!existsSync(destDir)) {
        mkdirSync(destDir, { recursive: true });
    }

    copyFileSync(sourcePath, destPath);
    logger.info(`Copied to: ${destPath}`);
}

/**
 * Compiles filter rules using the configuration
 * @param config - Compiler configuration
 * @param logger - Logger instance
 * @returns Array of compiled rules
 */
export async function compileFilters(
    config: IConfiguration,
    logger: Logger = defaultLogger
): Promise<string[]> {
    logger.info('Starting filter compilation...');

    try {
        const result = await compile(config);
        logger.info(`Compilation complete. Generated ${result.length} rules.`);
        return result;
    } catch (error) {
        const message = error instanceof Error ? error.message : 'Unknown error';
        logger.error(`Compilation failed: ${message}`);
        throw new Error(`Filter compilation failed: ${message}`);
    }
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
    /** Rules directory path */
    rulesDirectory?: string;
    /** Force configuration format */
    format?: ConfigurationFormat;
    /** Logger instance */
    logger?: Logger;
}

/**
 * Runs the full compilation pipeline
 * @param options - Compilation options
 * @returns Compilation result
 */
export async function runCompiler(options: CompileOptions): Promise<CompilerResult> {
    const logger = options.logger ?? defaultLogger;
    const startTime = new Date();

    const result: CompilerResult = {
        success: false,
        configName: '',
        configVersion: '',
        ruleCount: 0,
        outputPath: '',
        outputHash: '',
        copiedToRules: false,
        elapsedMs: 0,
        startTime,
        endTime: new Date()
    };

    try {
        // Read configuration
        const config = readConfiguration(options.configPath, options.format, logger);
        result.configName = config.name ?? '';
        result.configVersion = (config as Record<string, unknown>).version as string ?? '';

        // Determine output path
        const timestamp = new Date().toISOString().replace(/[:.]/g, '-').slice(0, 19);
        const defaultOutputPath = join(
            dirname(options.configPath),
            'output',
            `compiled-${timestamp}.txt`
        );
        const outputPath = options.outputPath ?? defaultOutputPath;
        result.outputPath = resolve(outputPath);

        // Compile filters
        const rules = await compileFilters(config, logger);

        // Write output
        writeOutput(result.outputPath, rules, logger);

        // Calculate statistics
        result.ruleCount = countRules(result.outputPath);
        result.outputHash = computeHash(result.outputPath);

        logger.info(`Compiled ${result.ruleCount} rules, hash: ${result.outputHash.slice(0, 32)}...`);

        // Copy to rules directory if requested
        if (options.copyToRules) {
            const rulesDir = options.rulesDirectory ?? join(dirname(options.configPath), '..', '..', 'rules');
            const destPath = join(rulesDir, 'adguard_user_filter.txt');
            copyToRulesDirectory(result.outputPath, resolve(destPath), logger);
            result.copiedToRules = true;
            result.rulesDestination = resolve(destPath);
        }

        result.success = true;
    } catch (error) {
        const message = error instanceof Error ? error.message : 'Unknown error';
        result.errorMessage = message;
        logger.error(`Compilation failed: ${message}`);
    }

    result.endTime = new Date();
    result.elapsedMs = result.endTime.getTime() - startTime.getTime();

    return result;
}
