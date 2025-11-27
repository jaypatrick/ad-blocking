/**
 * @fileoverview AdGuard Filter List Compiler
 *
 * This module compiles AdGuard filter lists using the @adguard/hostlist-compiler.
 * It reads configuration from a JSON file, compiles the filter rules, and outputs
 * the result to a text file.
 *
 * @module invoke-compiler
 * @author jaypatrick
 * @license MIT
 */

import compile, { IConfiguration } from '@adguard/hostlist-compiler';
import { readFileSync, writeFile, existsSync } from 'fs';
import { dirname } from 'path';

/**
 * Logger utility for consistent console output
 */
const logger = {
    /**
     * Logs an informational message
     * @param message - The message to log
     * @param args - Additional arguments to log
     */
    info: (message: string, ...args: unknown[]): void => {
        console.log(`[INFO] ${new Date().toISOString()} - ${message}`, ...args);
    },

    /**
     * Logs a warning message
     * @param message - The message to log
     * @param args - Additional arguments to log
     */
    warn: (message: string, ...args: unknown[]): void => {
        console.warn(`[WARN] ${new Date().toISOString()} - ${message}`, ...args);
    },

    /**
     * Logs an error message
     * @param message - The message to log
     * @param args - Additional arguments to log
     */
    error: (message: string, ...args: unknown[]): void => {
        console.error(`[ERROR] ${new Date().toISOString()} - ${message}`, ...args);
    },

    /**
     * Logs a debug message
     * @param message - The message to log
     * @param args - Additional arguments to log
     */
    debug: (message: string, ...args: unknown[]): void => {
        if (process.env.DEBUG) {
            console.debug(`[DEBUG] ${new Date().toISOString()} - ${message}`, ...args);
        }
    }
};

/**
 * Configuration file name
 */
const CONFIG_FILE = 'compiler-config.json';

/**
 * Output file name
 */
const OUTPUT_FILE = 'adguard_user_filter.txt';

/**
 * Reads and parses the compiler configuration from a JSON file.
 *
 * @param configPath - Path to the configuration file
 * @returns The parsed configuration object
 * @throws {Error} If the file doesn't exist or contains invalid JSON
 *
 * @example
 * ```typescript
 * const config = readConfiguration('./compiler-config.json');
 * ```
 */
export function readConfiguration(configPath: string): IConfiguration {
    logger.debug(`Reading configuration from: ${configPath}`);

    if (!existsSync(configPath)) {
        throw new Error(`Configuration file not found: ${configPath}`);
    }

    try {
        const fileContent = readFileSync(configPath, 'utf8');
        const config = JSON.parse(fileContent) as IConfiguration;

        logger.debug('Configuration parsed successfully');
        return config;
    } catch (error) {
        if (error instanceof SyntaxError) {
            throw new Error(`Invalid JSON in configuration file: ${error.message}`);
        }
        throw error;
    }
}

/**
 * Writes compiled filter rules to an output file.
 *
 * @param outputPath - Path to the output file
 * @param rules - Array of compiled filter rules
 * @returns A promise that resolves when the file is written
 * @throws {Error} If there's an error writing the file
 *
 * @example
 * ```typescript
 * await writeOutput('./output.txt', ['rule1', 'rule2']);
 * ```
 */
export function writeOutput(outputPath: string, rules: string[]): Promise<void> {
    return new Promise((resolve, reject) => {
        logger.debug(`Writing ${rules.length} rules to: ${outputPath}`);

        const content = rules.join('\n');
        writeFile(outputPath, content, (err) => {
            if (err) {
                logger.error(`Error writing to file: ${err.message}`);
                reject(new Error(`Failed to write output file: ${err.message}`));
            } else {
                logger.info(`Successfully wrote ${rules.length} lines to ${outputPath}`);
                resolve();
            }
        });
    });
}

/**
 * Compiles filter rules using the AdGuard hostlist compiler.
 *
 * @param config - The compiler configuration
 * @returns A promise that resolves with an array of compiled rules
 * @throws {Error} If compilation fails
 *
 * @example
 * ```typescript
 * const config = readConfiguration('./config.json');
 * const rules = await compileFilters(config);
 * console.log(`Compiled ${rules.length} rules`);
 * ```
 */
export async function compileFilters(config: IConfiguration): Promise<string[]> {
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
 * Main entry point for the filter compiler.
 *
 * This function:
 * 1. Reads the configuration from compiler-config.json
 * 2. Compiles the filters using @adguard/hostlist-compiler
 * 3. Writes the output to adguard_user_filter.txt
 *
 * @returns A promise that resolves when compilation is complete
 * @throws {Error} If any step fails
 *
 * @example
 * ```typescript
 * await main();
 * ```
 */
export async function main(): Promise<void> {
    const startTime = Date.now();
    logger.info('AdGuard Filter Compiler starting...');

    try {
        // Read configuration
        const config = readConfiguration(CONFIG_FILE);

        // Compile filters
        const rules = await compileFilters(config);

        // Write output
        await writeOutput(OUTPUT_FILE, rules);

        const elapsed = Date.now() - startTime;
        logger.info(`Compilation completed successfully in ${elapsed}ms`);
    } catch (error) {
        const message = error instanceof Error ? error.message : 'Unknown error';
        logger.error(`Compilation failed: ${message}`);
        throw error;
    }
}

// Run the compiler when executed directly
if (require.main === module) {
    main().catch((error) => {
        logger.error('Fatal error:', error);
        process.exit(1);
    });
}
