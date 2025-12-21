/**
 * Rules Compiler TypeScript Frontend
 *
 * A TypeScript API and CLI for compiling AdGuard filter rules
 * using @adguard/hostlist-compiler.
 *
 * Production-ready with:
 * - Custom error classes with error codes
 * - Configuration schema validation
 * - Input validation and path sanitization
 * - Graceful shutdown handling
 * - Structured JSON logging
 * - Resource limits and timeouts
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
export type { ReadConfigurationOptions } from './config-reader';

// Compiler
export {
  writeOutput,
  countRules,
  computeHash,
  copyToRulesDirectory,
  compileFilters,
  runCompiler,
} from './compiler';
export type { CompilerOptions, ExtendedCompileOptions } from './compiler';

// CLI
export { parseArgs, showHelp, showVersion, getVersionInfo, main } from './cli';

// Logger
export {
  createLogger,
  createProductionLogger,
  createDevelopmentLogger,
  parseLogLevel,
  logger,
  LogLevel,
} from './logger';
export type { LoggerConfig, ExtendedLogger } from './logger';

// Errors
export {
  CompilerError,
  ConfigurationError,
  ConfigNotFoundError,
  ConfigParseError,
  CompilationError,
  CompilationTimeoutError,
  FileSystemError,
  PathTraversalError,
  ValidationError,
  ShutdownError,
  ResourceLimitError,
  ErrorCode,
  ErrorSeverity,
  wrapError,
  isCompilerError,
  isRecoverable,
} from './errors';
export type { ErrorContext } from './errors';

// Validation
export {
  validateConfiguration,
  assertValidConfiguration,
  containsPathTraversal,
  sanitizePath,
  validateUrl,
  validateSourcePath,
  checkFileSize,
  checkSourceCount,
  DEFAULT_RESOURCE_LIMITS,
} from './validation';
export type { ValidationResult, ResourceLimits } from './validation';

// Shutdown
export {
  ShutdownHandler,
  getShutdownHandler,
  initializeShutdownHandler,
  createShutdownAwareAbortController,
  withShutdownCheck,
} from './shutdown';
export type { CleanupFn, ShutdownConfig } from './shutdown';

// Timeout utilities
export {
  withTimeout,
  createTimeoutController,
  withRetry,
  debounce,
  throttle,
} from './timeout';
export type { TimeoutController, RetryConfig } from './timeout';
