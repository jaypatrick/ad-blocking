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
 * ## Library API
 *
 * For programmatic usage, use the high-level library API:
 *
 * ```typescript
 * import { RulesCompiler, ConfigurationBuilder } from '@rules-compiler/typescript/lib';
 *
 * // Create a compiler
 * const compiler = RulesCompiler.builder()
 *   .withTimeout(60000)
 *   .build();
 *
 * // Compile from config file
 * const result = await compiler.compile({ configPath: 'config.yaml' });
 *
 * // Or build config programmatically
 * const config = ConfigurationBuilder.create('My Filters')
 *   .addSource('https://example.com/filters.txt')
 *   .withPreset('basic')
 *   .build();
 * const rules = await compiler.compileFromConfig(config);
 * ```
 *
 * @packageDocumentation
 */

// Types
export type {
  CliOptions,
  CompileOptions,
  CompilerResult,
  ConfigurationFormat,
  ExtendedConfiguration,
  Logger,
  PlatformInfo,
  VersionInfo,
} from './types.ts';

// Configuration reader
export { detectFormat, findDefaultConfig, readConfiguration, toJson } from './config-reader.ts';
export type { ReadConfigurationOptions } from './config-reader.ts';

// Compiler
export {
  compileFilters,
  computeHash,
  copyToRulesDirectory,
  countRules,
  runCompiler,
  writeOutput,
} from './compiler.ts';
export type { CompilerOptions, ExtendedCompileOptions } from './compiler.ts';

// CLI
export { getVersionInfo, main, parseArgs, showHelp, showVersion } from './cli.ts';

// Console (Interactive Mode)
export {
  bold,
  chalk,
  colorStatus,
  // Application
  ConsoleApplication,
  createKeyValueTable,
  createSpinner,
  createTable,
  dim,
  displayTable,
  formatBytes,
  formatElapsed,
  // Utilities
  formatValue,
  generateBanner,
  runInteractive,
  showError,
  showHeader,
  showInfo,
  showNoItems,
  showPanel,
  showRule,
  showSuccess,
  showWarning,
  showWelcomeBanner,
  truncate,
  withSpinner,
} from './console/index.ts';
export type { ConsoleAppOptions } from './console/index.ts';

// Logger
export {
  createDevelopmentLogger,
  createLogger,
  createProductionLogger,
  logger,
  LogLevel,
  parseLogLevel,
} from './logger.ts';
export type { ExtendedLogger, LoggerConfig } from './logger.ts';

// Errors
export {
  CompilationError,
  CompilationTimeoutError,
  CompilerError,
  ConfigNotFoundError,
  ConfigParseError,
  ConfigurationError,
  ErrorCode,
  ErrorSeverity,
  FileSystemError,
  isCompilerError,
  isRecoverable,
  PathTraversalError,
  ResourceLimitError,
  ShutdownError,
  ValidationError,
  wrapError,
} from './errors.ts';
export type { ErrorContext } from './errors.ts';

// Validation
export {
  assertValidConfiguration,
  checkFileSize,
  checkSourceCount,
  containsPathTraversal,
  DEFAULT_RESOURCE_LIMITS,
  sanitizePath,
  validateConfiguration,
  validateSourcePath,
  validateUrl,
} from './validation.ts';
export type { ResourceLimits, ValidationResult } from './validation.ts';

// Shutdown
export {
  createShutdownAwareAbortController,
  getShutdownHandler,
  initializeShutdownHandler,
  ShutdownHandler,
  withShutdownCheck,
} from './shutdown.ts';
export type { CleanupFn, ShutdownConfig } from './shutdown.ts';

// Timeout utilities
export { createTimeoutController, debounce, throttle, withRetry, withTimeout } from './timeout.ts';
export type { RetryConfig, TimeoutController } from './timeout.ts';

// Library API (high-level)
export {
  AVAILABLE_TRANSFORMATIONS,
  ConfigurationBuilder,
  createConfiguration,
  createRulesCompiler,
  RulesCompiler,
  RulesCompilerBuilder,
  TRANSFORMATION_DESCRIPTIONS,
} from './lib/index.ts';
export type {
  CompileProgressEvent,
  CompileRunOptions,
  RulesCompilerServiceOptions,
  SourceConfig,
  SourceType,
  Transformation,
} from './lib/index.ts';
