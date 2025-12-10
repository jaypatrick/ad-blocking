/**
 * @fileoverview Main entry point for the AdGuard Filter Compiler API
 * @module index
 *
 * This module exports all public functions and types for the filter compiler.
 *
 * @example
 * ```typescript
 * import { readConfiguration, runCompiler } from './src';
 *
 * const config = readConfiguration('./compiler-config.yaml');
 * const result = await runCompiler({ configPath: './config.json' });
 * ```
 */

// Types
export type {
    ConfigurationFormat,
    CliOptions,
    CompilerResult,
    VersionInfo,
    ExtendedConfiguration,
    Logger
} from './types';

// Logger
export { createLogger, logger } from './logger';

// Configuration reader
export {
    detectFormat,
    readConfiguration,
    toJson
} from './config-reader';

// Compiler
export {
    writeOutput,
    countRules,
    computeHash,
    copyToRulesDirectory,
    compileFilters,
    runCompiler,
    type CompileOptions
} from './compiler';

// CLI
export {
    parseArgs,
    showHelp,
    showVersion,
    getVersionInfo,
    main
} from './cli';
