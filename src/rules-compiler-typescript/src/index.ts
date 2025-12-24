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
} from './types.ts';

// Configuration reader
export {
  detectFormat,
  findDefaultConfig,
  readConfiguration,
  toJson,
} from './config-reader.ts';
export type { ReadConfigurationOptions } from './config-reader.ts';

// Compiler
export {
  writeOutput,
  countRules,
  computeHash,
  copyToRulesDirectory,
  compileFilters,
  runCompiler,
} from './compiler.ts';
export type { CompilerOptions, ExtendedCompileOptions } from './compiler.ts';

// CLI
export { parseArgs, showHelp, showVersion, getVersionInfo, main } from './cli.ts';

// Logger
export {
  createLogger,
  createProductionLogger,
  createDevelopmentLogger,
  parseLogLevel,
  logger,
  LogLevel,
} from './logger.ts';
export type { LoggerConfig, ExtendedLogger } from './logger.ts';

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
} from './errors.ts';
export type { ErrorContext } from './errors.ts';

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
} from './validation.ts';
export type { ValidationResult, ResourceLimits } from './validation.ts';

// Shutdown
export {
  ShutdownHandler,
  getShutdownHandler,
  initializeShutdownHandler,
  createShutdownAwareAbortController,
  withShutdownCheck,
} from './shutdown.ts';
export type { CleanupFn, ShutdownConfig } from './shutdown.ts';

// Timeout utilities
export {
  withTimeout,
  createTimeoutController,
  withRetry,
  debounce,
  throttle,
} from './timeout.ts';
export type { TimeoutController, RetryConfig } from './timeout.ts';
