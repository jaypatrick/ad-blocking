/**
 * Custom error classes for production-ready error handling
 * Provides error codes, classification, and proper error hierarchy
 */

/**
 * Error codes for the rules compiler
 */
export enum ErrorCode {
  // Configuration errors (1xxx)
  CONFIG_NOT_FOUND = 'E1001',
  CONFIG_INVALID_FORMAT = 'E1002',
  CONFIG_PARSE_ERROR = 'E1003',
  CONFIG_VALIDATION_ERROR = 'E1004',
  CONFIG_MISSING_REQUIRED = 'E1005',

  // Compilation errors (2xxx)
  COMPILATION_FAILED = 'E2001',
  COMPILATION_TIMEOUT = 'E2002',
  SOURCE_FETCH_FAILED = 'E2003',

  // File system errors (3xxx)
  FILE_READ_ERROR = 'E3001',
  FILE_WRITE_ERROR = 'E3002',
  DIRECTORY_CREATE_ERROR = 'E3003',
  PATH_TRAVERSAL_ERROR = 'E3004',
  FILE_TOO_LARGE = 'E3005',

  // Input validation errors (4xxx)
  INVALID_PATH = 'E4001',
  INVALID_URL = 'E4002',
  INVALID_ARGUMENT = 'E4003',

  // System errors (5xxx)
  SHUTDOWN_REQUESTED = 'E5001',
  TIMEOUT = 'E5002',
  RESOURCE_LIMIT_EXCEEDED = 'E5003',
}

/**
 * Error severity levels
 */
export enum ErrorSeverity {
  /** Recoverable error, operation can continue */
  WARNING = 'warning',
  /** Non-recoverable error, operation failed */
  ERROR = 'error',
  /** Critical error, application should terminate */
  CRITICAL = 'critical',
}

/**
 * Error context for debugging and logging
 */
export interface ErrorContext {
  /** File path involved in the error */
  filePath?: string;
  /** URL involved in the error */
  url?: string;
  /** Configuration name */
  configName?: string;
  /** Additional metadata */
  metadata?: Record<string, unknown>;
}

/**
 * Base error class for all compiler errors
 */
export class CompilerError extends Error {
  public readonly code: ErrorCode;
  public readonly severity: ErrorSeverity;
  public readonly context: ErrorContext;
  public readonly timestamp: Date;
  public override readonly cause?: Error;
  public override name: string;

  constructor(
    message: string,
    code: ErrorCode,
    severity: ErrorSeverity = ErrorSeverity.ERROR,
    context: ErrorContext = {},
    cause?: Error,
  ) {
    super(message);
    this.name = 'CompilerError';
    this.code = code;
    this.severity = severity;
    this.context = context;
    this.timestamp = new Date();
    this.cause = cause;

    // Maintains proper stack trace for where error was thrown
    if (Error.captureStackTrace) {
      Error.captureStackTrace(this, CompilerError);
    }
  }

  /**
   * Converts error to a JSON-serializable object
   */
  toJSON(): Record<string, unknown> {
    return {
      name: this.name,
      code: this.code,
      message: this.message,
      severity: this.severity,
      context: this.context,
      timestamp: this.timestamp.toISOString(),
      cause: this.cause?.message,
      stack: this.stack,
    };
  }

  /**
   * Creates a formatted string for logging
   */
  toLogString(): string {
    const contextStr = Object.keys(this.context).length > 0
      ? ` (${JSON.stringify(this.context)})`
      : '';
    return `[${this.code}] ${this.message}${contextStr}`;
  }
}

/**
 * Configuration-related errors
 */
export class ConfigurationError extends CompilerError {
  constructor(
    message: string,
    code: ErrorCode = ErrorCode.CONFIG_VALIDATION_ERROR,
    context: ErrorContext = {},
    cause?: Error,
  ) {
    super(message, code, ErrorSeverity.ERROR, context, cause);
    this.name = 'ConfigurationError';
  }
}

/**
 * Configuration not found error
 */
export class ConfigNotFoundError extends ConfigurationError {
  constructor(filePath: string, cause?: Error) {
    super(
      `Configuration file not found: ${filePath}`,
      ErrorCode.CONFIG_NOT_FOUND,
      { filePath },
      cause,
    );
    this.name = 'ConfigNotFoundError';
  }
}

/**
 * Configuration parse error
 */
export class ConfigParseError extends ConfigurationError {
  constructor(filePath: string, format: string, parseError: string, cause?: Error) {
    super(
      `Failed to parse ${format.toUpperCase()} configuration: ${parseError}`,
      ErrorCode.CONFIG_PARSE_ERROR,
      { filePath, metadata: { format } },
      cause,
    );
    this.name = 'ConfigParseError';
  }
}

/**
 * Compilation-related errors
 */
export class CompilationError extends CompilerError {
  constructor(
    message: string,
    code: ErrorCode = ErrorCode.COMPILATION_FAILED,
    context: ErrorContext = {},
    cause?: Error,
  ) {
    super(message, code, ErrorSeverity.ERROR, context, cause);
    this.name = 'CompilationError';
  }
}

/**
 * Compilation timeout error
 */
export class CompilationTimeoutError extends CompilationError {
  public readonly timeoutMs: number;

  constructor(timeoutMs: number, context: ErrorContext = {}) {
    super(
      `Compilation timed out after ${timeoutMs}ms`,
      ErrorCode.COMPILATION_TIMEOUT,
      context,
    );
    this.name = 'CompilationTimeoutError';
    this.timeoutMs = timeoutMs;
  }
}

/**
 * File system errors
 */
export class FileSystemError extends CompilerError {
  constructor(
    message: string,
    code: ErrorCode = ErrorCode.FILE_READ_ERROR,
    context: ErrorContext = {},
    cause?: Error,
  ) {
    super(message, code, ErrorSeverity.ERROR, context, cause);
    this.name = 'FileSystemError';
  }
}

/**
 * Path traversal security error
 */
export class PathTraversalError extends CompilerError {
  constructor(path: string) {
    super(
      `Path traversal detected in path: ${path}`,
      ErrorCode.PATH_TRAVERSAL_ERROR,
      ErrorSeverity.CRITICAL,
      { filePath: path },
    );
    this.name = 'PathTraversalError';
  }
}

/**
 * Input validation errors
 */
export class ValidationError extends CompilerError {
  constructor(
    message: string,
    code: ErrorCode = ErrorCode.INVALID_ARGUMENT,
    context: ErrorContext = {},
    cause?: Error,
  ) {
    super(message, code, ErrorSeverity.ERROR, context, cause);
    this.name = 'ValidationError';
  }
}

/**
 * Shutdown requested error (for graceful shutdown)
 */
export class ShutdownError extends CompilerError {
  public readonly signal: string;

  constructor(signal: string) {
    super(
      `Shutdown requested via ${signal}`,
      ErrorCode.SHUTDOWN_REQUESTED,
      ErrorSeverity.WARNING,
      {},
    );
    this.name = 'ShutdownError';
    this.signal = signal;
  }
}

/**
 * Resource limit exceeded error
 */
export class ResourceLimitError extends CompilerError {
  public readonly limit: number;
  public readonly actual: number;
  public readonly resource: string;

  constructor(resource: string, limit: number, actual: number) {
    super(
      `Resource limit exceeded for ${resource}: ${actual} > ${limit}`,
      ErrorCode.RESOURCE_LIMIT_EXCEEDED,
      ErrorSeverity.ERROR,
      { metadata: { resource, limit, actual } },
    );
    this.name = 'ResourceLimitError';
    this.limit = limit;
    this.actual = actual;
    this.resource = resource;
  }
}

/**
 * Wraps an unknown error into a CompilerError
 */
export function wrapError(
  error: unknown,
  defaultCode: ErrorCode = ErrorCode.COMPILATION_FAILED,
): CompilerError {
  if (error instanceof CompilerError) {
    return error;
  }

  if (error instanceof Error) {
    return new CompilerError(
      error.message,
      defaultCode,
      ErrorSeverity.ERROR,
      {},
      error,
    );
  }

  return new CompilerError(
    String(error),
    defaultCode,
    ErrorSeverity.ERROR,
  );
}

/**
 * Type guard to check if an error is a CompilerError
 */
export function isCompilerError(error: unknown): error is CompilerError {
  return error instanceof CompilerError;
}

/**
 * Type guard to check if error is recoverable
 */
export function isRecoverable(error: unknown): boolean {
  if (!isCompilerError(error)) {
    return false;
  }
  return error.severity === ErrorSeverity.WARNING;
}
