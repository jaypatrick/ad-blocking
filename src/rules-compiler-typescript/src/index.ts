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
} from './types.js';

// Configuration reader
export {
  detectFormat,
  findDefaultConfig,
  readConfiguration,
  toJson,
} from './config-reader.js';
export type { ReadConfigurationOptions } from './config-reader.js';

// Compiler
export {
  writeOutput,
  countRules,
  computeHash,
  copyToRulesDirectory,
  compileFilters,
  runCompiler,
} from './compiler.js';
export type { CompilerOptions, ExtendedCompileOptions } from './compiler.js';

// CLI
export { parseArgs, showHelp, showVersion, getVersionInfo, main } from './cli.js';

// Logger
export {
  createLogger,
  createProductionLogger,
  createDevelopmentLogger,
  parseLogLevel,
  logger,
  LogLevel,
} from './logger.js';
export type { LoggerConfig, ExtendedLogger } from './logger.js';

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
} from './errors.js';
export type { ErrorContext } from './errors.js';

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
} from './validation.js';
export type { ValidationResult, ResourceLimits } from './validation.js';

// Shutdown
export {
  ShutdownHandler,
  getShutdownHandler,
  initializeShutdownHandler,
  createShutdownAwareAbortController,
  withShutdownCheck,
} from './shutdown.js';
export type { CleanupFn, ShutdownConfig } from './shutdown.js';

// Timeout utilities
export {
  withTimeout,
  createTimeoutController,
  withRetry,
  debounce,
  throttle,
} from './timeout.js';
export type { TimeoutController, RetryConfig } from './timeout.js';
