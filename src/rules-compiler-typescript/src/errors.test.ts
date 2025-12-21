/**
 * Tests for custom error classes
 */

import {
  CompilerError,
  ConfigurationError,
  ConfigNotFoundError,
  ConfigParseError,
  CompilationError,
  CompilationTimeoutError,
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

describe('CompilerError', () => {
  it('should create error with all properties', () => {
    const error = new CompilerError(
      'Test error',
      ErrorCode.COMPILATION_FAILED,
      ErrorSeverity.ERROR,
      { filePath: '/test/path' }
    );

    expect(error.message).toBe('Test error');
    expect(error.code).toBe(ErrorCode.COMPILATION_FAILED);
    expect(error.severity).toBe(ErrorSeverity.ERROR);
    expect(error.context.filePath).toBe('/test/path');
    expect(error.timestamp).toBeInstanceOf(Date);
    expect(error.name).toBe('CompilerError');
  });

  it('should use default severity', () => {
    const error = new CompilerError('Test', ErrorCode.COMPILATION_FAILED);
    expect(error.severity).toBe(ErrorSeverity.ERROR);
  });

  it('should convert to JSON', () => {
    const error = new CompilerError('Test error', ErrorCode.CONFIG_NOT_FOUND);
    const json = error.toJSON();

    expect(json.name).toBe('CompilerError');
    expect(json.code).toBe(ErrorCode.CONFIG_NOT_FOUND);
    expect(json.message).toBe('Test error');
    expect(typeof json.timestamp).toBe('string');
  });

  it('should create log string', () => {
    const error = new CompilerError(
      'Test error',
      ErrorCode.COMPILATION_FAILED,
      ErrorSeverity.ERROR,
      { configName: 'test-config' }
    );

    const logStr = error.toLogString();
    expect(logStr).toContain('[E2001]');
    expect(logStr).toContain('Test error');
    expect(logStr).toContain('test-config');
  });

  it('should preserve cause error', () => {
    const cause = new Error('Original error');
    const error = new CompilerError(
      'Wrapped error',
      ErrorCode.COMPILATION_FAILED,
      ErrorSeverity.ERROR,
      {},
      cause
    );

    expect(error.cause).toBe(cause);
    expect(error.toJSON().cause).toBe('Original error');
  });
});

describe('ConfigNotFoundError', () => {
  it('should create with file path', () => {
    const error = new ConfigNotFoundError('/path/to/config.yaml');

    expect(error.code).toBe(ErrorCode.CONFIG_NOT_FOUND);
    expect(error.context.filePath).toBe('/path/to/config.yaml');
    expect(error.message).toContain('/path/to/config.yaml');
    expect(error.name).toBe('ConfigNotFoundError');
  });
});

describe('ConfigParseError', () => {
  it('should create with format details', () => {
    const error = new ConfigParseError('/path/config.json', 'json', 'Unexpected token');

    expect(error.code).toBe(ErrorCode.CONFIG_PARSE_ERROR);
    expect(error.context.filePath).toBe('/path/config.json');
    expect(error.context.metadata?.format).toBe('json');
    expect(error.message).toContain('JSON');
    expect(error.message).toContain('Unexpected token');
  });
});

describe('CompilationTimeoutError', () => {
  it('should include timeout duration', () => {
    const error = new CompilationTimeoutError(30000);

    expect(error.code).toBe(ErrorCode.COMPILATION_TIMEOUT);
    expect(error.timeoutMs).toBe(30000);
    expect(error.message).toContain('30000ms');
  });
});

describe('PathTraversalError', () => {
  it('should be critical severity', () => {
    const error = new PathTraversalError('../../../etc/passwd');

    expect(error.code).toBe(ErrorCode.PATH_TRAVERSAL_ERROR);
    expect(error.severity).toBe(ErrorSeverity.CRITICAL);
    expect(error.context.filePath).toBe('../../../etc/passwd');
  });
});

describe('ShutdownError', () => {
  it('should be warning severity', () => {
    const error = new ShutdownError('SIGTERM');

    expect(error.code).toBe(ErrorCode.SHUTDOWN_REQUESTED);
    expect(error.severity).toBe(ErrorSeverity.WARNING);
    expect(error.signal).toBe('SIGTERM');
    expect(error.message).toContain('SIGTERM');
  });
});

describe('ResourceLimitError', () => {
  it('should include limit details', () => {
    const error = new ResourceLimitError('file size', 1000, 2000);

    expect(error.code).toBe(ErrorCode.RESOURCE_LIMIT_EXCEEDED);
    expect(error.resource).toBe('file size');
    expect(error.limit).toBe(1000);
    expect(error.actual).toBe(2000);
    expect(error.message).toContain('file size');
    expect(error.message).toContain('2000 > 1000');
  });
});

describe('wrapError', () => {
  it('should return CompilerError as-is', () => {
    const original = new CompilerError('Test', ErrorCode.COMPILATION_FAILED);
    const wrapped = wrapError(original);

    expect(wrapped).toBe(original);
  });

  it('should wrap Error instance', () => {
    const original = new Error('Standard error');
    const wrapped = wrapError(original);

    expect(wrapped).toBeInstanceOf(CompilerError);
    expect(wrapped.message).toBe('Standard error');
    expect(wrapped.cause).toBe(original);
  });

  it('should wrap non-Error values', () => {
    const wrapped = wrapError('string error');

    expect(wrapped).toBeInstanceOf(CompilerError);
    expect(wrapped.message).toBe('string error');
  });

  it('should use provided error code', () => {
    const wrapped = wrapError('error', ErrorCode.CONFIG_NOT_FOUND);
    expect(wrapped.code).toBe(ErrorCode.CONFIG_NOT_FOUND);
  });
});

describe('isCompilerError', () => {
  it('should return true for CompilerError', () => {
    const error = new CompilerError('Test', ErrorCode.COMPILATION_FAILED);
    expect(isCompilerError(error)).toBe(true);
  });

  it('should return true for subclasses', () => {
    expect(isCompilerError(new ConfigNotFoundError('/path'))).toBe(true);
    expect(isCompilerError(new CompilationError('test'))).toBe(true);
  });

  it('should return false for standard Error', () => {
    expect(isCompilerError(new Error('test'))).toBe(false);
  });

  it('should return false for non-errors', () => {
    expect(isCompilerError('string')).toBe(false);
    expect(isCompilerError(null)).toBe(false);
    expect(isCompilerError(undefined)).toBe(false);
  });
});

describe('isRecoverable', () => {
  it('should return true for warning severity', () => {
    const error = new ShutdownError('SIGTERM');
    expect(isRecoverable(error)).toBe(true);
  });

  it('should return false for error severity', () => {
    const error = new CompilationError('Failed');
    expect(isRecoverable(error)).toBe(false);
  });

  it('should return false for critical severity', () => {
    const error = new PathTraversalError('../etc/passwd');
    expect(isRecoverable(error)).toBe(false);
  });

  it('should return false for non-CompilerError', () => {
    expect(isRecoverable(new Error('test'))).toBe(false);
  });
});
