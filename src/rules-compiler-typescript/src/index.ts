/**
 * Rules Compiler TypeScript Frontend
 *
 * A TypeScript API and CLI for compiling AdGuard filter rules
 * using @adguard/hostlist-compiler.
 *
 * @packageDocumentation
 */

// Types
export type {
  ConfigurationFormat,
  CliOptions,
  CompilerResult,
  VersionInfo,
  PlatformInfo,
  ExtendedConfiguration,
  Logger,
  CompileOptions,
} from './types';

// Configuration reader
export {
  detectFormat,
  findDefaultConfig,
  readConfiguration,
  toJson,
} from './config-reader';

// Compiler
export {
  writeOutput,
  countRules,
  computeHash,
  copyToRulesDirectory,
  compileFilters,
  runCompiler,
} from './compiler';

// CLI
export { parseArgs, showHelp, showVersion, getVersionInfo, main } from './cli';

// Logger
export { createLogger, logger } from './logger';
