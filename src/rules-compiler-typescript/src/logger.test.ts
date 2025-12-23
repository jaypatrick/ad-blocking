/**
 * Tests for enhanced logger
 */

import { jest } from '@jest/globals';
import {
  createLogger,
  createProductionLogger,
  createDevelopmentLogger,
  parseLogLevel,
  LogLevel,
} from './logger';

describe('createLogger', () => {
  let consoleSpy: {
    log: jest.SpiedFunction<typeof console.log>;
    warn: jest.SpiedFunction<typeof console.warn>;
    error: jest.SpiedFunction<typeof console.error>;
    debug: jest.SpiedFunction<typeof console.debug>;
  };

  beforeEach(() => {
    consoleSpy = {
      log: jest.spyOn(console, 'log').mockImplementation(),
      warn: jest.spyOn(console, 'warn').mockImplementation(),
      error: jest.spyOn(console, 'error').mockImplementation(),
      debug: jest.spyOn(console, 'debug').mockImplementation(),
    };
  });

  afterEach(() => {
    jest.restoreAllMocks();
  });

  it('should create logger with default config', () => {
    const logger = createLogger();
    expect(logger.config).toBeDefined();
    expect(logger.config.debugEnabled).toBe(false);
    expect(logger.config.jsonFormat).toBe(false);
  });

  it('should support boolean parameter for backwards compatibility', () => {
    const logger = createLogger(true);
    expect(logger.config.debugEnabled).toBe(true);
  });

  it('should log info messages', () => {
    const logger = createLogger();
    logger.info('Test message');

    expect(consoleSpy.log).toHaveBeenCalled();
    const call = consoleSpy.log.mock.calls[0][0];
    expect(call).toContain('[INFO]');
    expect(call).toContain('Test message');
  });

  it('should log warn messages', () => {
    const logger = createLogger();
    logger.warn('Warning message');

    expect(consoleSpy.warn).toHaveBeenCalled();
    const call = consoleSpy.warn.mock.calls[0][0];
    expect(call).toContain('[WARN]');
    expect(call).toContain('Warning message');
  });

  it('should log error messages', () => {
    const logger = createLogger();
    logger.error('Error message');

    expect(consoleSpy.error).toHaveBeenCalled();
    const call = consoleSpy.error.mock.calls[0][0];
    expect(call).toContain('[ERROR]');
    expect(call).toContain('Error message');
  });

  it('should not log debug when disabled', () => {
    const logger = createLogger({ debugEnabled: false });
    logger.debug('Debug message');

    expect(consoleSpy.debug).not.toHaveBeenCalled();
  });

  it('should log debug when enabled', () => {
    const logger = createLogger({ debugEnabled: true, minLevel: LogLevel.DEBUG });
    logger.debug('Debug message');

    expect(consoleSpy.debug).toHaveBeenCalled();
    const call = consoleSpy.debug.mock.calls[0][0];
    expect(call).toContain('[DEBUG]');
    expect(call).toContain('Debug message');
  });

  it('should output JSON format when configured', () => {
    const logger = createLogger({ jsonFormat: true });
    logger.info('Test message');

    expect(consoleSpy.log).toHaveBeenCalled();
    const output = consoleSpy.log.mock.calls[0][0];

    // Should be valid JSON
    const parsed = JSON.parse(output);
    expect(parsed.level).toBe('INFO');
    expect(parsed.message).toBe('Test message');
    expect(parsed.timestamp).toBeDefined();
  });

  it('should include app name in JSON output', () => {
    const logger = createLogger({ jsonFormat: true, appName: 'my-app' });
    logger.info('Test');

    const output = consoleSpy.log.mock.calls[0][0];
    const parsed = JSON.parse(output);
    expect(parsed.app).toBe('my-app');
  });

  it('should respect minimum log level', () => {
    const logger = createLogger({ minLevel: LogLevel.WARN });

    logger.info('Info');
    logger.warn('Warn');

    expect(consoleSpy.log).not.toHaveBeenCalled();
    expect(consoleSpy.warn).toHaveBeenCalled();
  });

  it('should create child logger with additional context', () => {
    const logger = createLogger({ jsonFormat: true });
    const child = logger.child({ requestId: '123' });

    child.info('Test');

    const output = consoleSpy.log.mock.calls[0][0];
    const parsed = JSON.parse(output);
    expect(parsed.context.requestId).toBe('123');
  });

  it('should check if log level is enabled', () => {
    const logger = createLogger({ minLevel: LogLevel.WARN });

    expect(logger.isLevelEnabled(LogLevel.INFO)).toBe(false);
    expect(logger.isLevelEnabled(LogLevel.WARN)).toBe(true);
    expect(logger.isLevelEnabled(LogLevel.ERROR)).toBe(true);
  });

  it('should include extra args in output', () => {
    const logger = createLogger({ jsonFormat: true });
    logger.info('Test', { extra: 'data' });

    const output = consoleSpy.log.mock.calls[0][0];
    const parsed = JSON.parse(output);
    expect(parsed.args).toBeDefined();
    expect(parsed.args[0]).toEqual({ extra: 'data' });
  });
});

describe('createProductionLogger', () => {
  it('should create JSON-formatted logger', () => {
    const logger = createProductionLogger();
    expect(logger.config.jsonFormat).toBe(true);
    expect(logger.config.debugEnabled).toBe(false);
  });

  it('should use custom app name', () => {
    const logger = createProductionLogger('custom-app');
    expect(logger.config.appName).toBe('custom-app');
  });
});

describe('createDevelopmentLogger', () => {
  it('should enable debug logging', () => {
    const logger = createDevelopmentLogger();
    expect(logger.config.debugEnabled).toBe(true);
    expect(logger.config.jsonFormat).toBe(false);
    expect(logger.config.minLevel).toBe(LogLevel.DEBUG);
  });
});

describe('parseLogLevel', () => {
  it('should parse valid log levels', () => {
    expect(parseLogLevel('DEBUG')).toBe(LogLevel.DEBUG);
    expect(parseLogLevel('INFO')).toBe(LogLevel.INFO);
    expect(parseLogLevel('WARN')).toBe(LogLevel.WARN);
    expect(parseLogLevel('ERROR')).toBe(LogLevel.ERROR);
    expect(parseLogLevel('SILENT')).toBe(LogLevel.SILENT);
  });

  it('should be case-insensitive', () => {
    expect(parseLogLevel('debug')).toBe(LogLevel.DEBUG);
    expect(parseLogLevel('Info')).toBe(LogLevel.INFO);
    expect(parseLogLevel('WARNING')).toBe(LogLevel.WARN);
  });

  it('should default to INFO for unknown levels', () => {
    expect(parseLogLevel('unknown')).toBe(LogLevel.INFO);
    expect(parseLogLevel('')).toBe(LogLevel.INFO);
  });
});

describe('LogLevel', () => {
  it('should have correct ordering', () => {
    expect(LogLevel.DEBUG).toBeLessThan(LogLevel.INFO);
    expect(LogLevel.INFO).toBeLessThan(LogLevel.WARN);
    expect(LogLevel.WARN).toBeLessThan(LogLevel.ERROR);
    expect(LogLevel.ERROR).toBeLessThan(LogLevel.SILENT);
  });
});
