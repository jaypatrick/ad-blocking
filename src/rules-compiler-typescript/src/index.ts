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

// Console (Interactive Mode)
export {
  // Utilities
  formatValue,
  createTable,
  createKeyValueTable,
  displayTable,
  showSuccess,
  showError,
  showWarning,
  showInfo,
  showHeader,
  showPanel,
  showRule,
  showNoItems,
  createSpinner,
  withSpinner,
  generateBanner,
  showWelcomeBanner,
  truncate,
  formatElapsed,
  formatBytes,
  colorStatus,
  dim,
  bold,
  chalk,
  // Application
  ConsoleApplication,
  runInteractive,
} from './console/index.ts';
export type { ConsoleAppOptions } from './console/index.ts';

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

// Library API (high-level)
export {
  RulesCompiler,
  RulesCompilerBuilder,
  createRulesCompiler,
  ConfigurationBuilder,
  createConfiguration,
  AVAILABLE_TRANSFORMATIONS,
  TRANSFORMATION_DESCRIPTIONS,
} from './lib/index.ts';
export type {
  RulesCompilerServiceOptions,
  CompileRunOptions,
  CompileProgressEvent,
  Transformation,
  SourceType,
  SourceConfig,
} from './lib/index.ts';
