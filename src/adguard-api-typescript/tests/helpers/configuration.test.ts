/**
 * Configuration helper tests
 */

/// <reference types="jest" />

import {
  ConfigurationBuilder,
  createWithApiKey,
  createWithBearerToken,
  createCustom,
  validateAuthentication,
  maskApiKey,
  DEFAULT_BASE_PATH,
  DEFAULT_TIMEOUT,
  MIN_TIMEOUT,
  MAX_TIMEOUT,
  consoleLogger,
  silentLogger,
} from '../../src/helpers/configuration';

describe('ConfigurationHelper', () => {
  describe('createWithApiKey', () => {
    it('should create configuration with API key', () => {
      const config = createWithApiKey('test-api-key');
      expect(config.apiKey).toBe('test-api-key');
      expect(config.authType).toBe('api-key');
      expect(config.basePath).toBe(DEFAULT_BASE_PATH);
      expect(config.timeout).toBe(DEFAULT_TIMEOUT);
    });

    it('should throw for empty API key', () => {
      expect(() => createWithApiKey('')).toThrow();
      expect(() => createWithApiKey('   ')).toThrow();
    });

    it('should trim API key', () => {
      const config = createWithApiKey('  test-key  ');
      expect(config.apiKey).toBe('test-key');
    });

    it('should accept custom base path', () => {
      const config = createWithApiKey('test-key', 'https://custom.api.com');
      expect(config.basePath).toBe('https://custom.api.com');
    });

    it('should accept logger', () => {
      const config = createWithApiKey('test-key', undefined, consoleLogger);
      expect(config.logger).toBe(consoleLogger);
    });
  });

  describe('createWithBearerToken', () => {
    it('should create configuration with bearer token', () => {
      const config = createWithBearerToken('test-token');
      expect(config.accessToken).toBe('test-token');
      expect(config.authType).toBe('bearer');
    });

    it('should throw for empty token', () => {
      expect(() => createWithBearerToken('')).toThrow();
    });
  });

  describe('createCustom', () => {
    it('should create default configuration', () => {
      const config = createCustom();
      expect(config.basePath).toBe(DEFAULT_BASE_PATH);
      expect(config.timeout).toBe(DEFAULT_TIMEOUT);
    });

    it('should accept custom values', () => {
      const config = createCustom(
        'https://custom.api.com',
        60000,
        'Custom-Agent/1.0',
        consoleLogger
      );
      expect(config.basePath).toBe('https://custom.api.com');
      expect(config.timeout).toBe(60000);
      expect(config.userAgent).toBe('Custom-Agent/1.0');
      expect(config.logger).toBe(consoleLogger);
    });

    it('should validate timeout range', () => {
      expect(() => createCustom(undefined, 500)).toThrow();
      expect(() => createCustom(undefined, 500000)).toThrow();
    });
  });

  describe('validateAuthentication', () => {
    it('should return true for valid API key config', () => {
      const config = createWithApiKey('test-key');
      expect(validateAuthentication(config)).toBe(true);
    });

    it('should return true for valid bearer token config', () => {
      const config = createWithBearerToken('test-token');
      expect(validateAuthentication(config)).toBe(true);
    });

    it('should return false for config without auth', () => {
      const config = createCustom();
      expect(validateAuthentication(config)).toBe(false);
    });
  });

  describe('maskApiKey', () => {
    it('should mask API key showing first and last 4 chars', () => {
      const result = maskApiKey('abcdefghijklmnop');
      expect(result).toBe('abcd...mnop');
    });

    it('should fully mask short API keys', () => {
      const result = maskApiKey('short');
      expect(result).toBe('********');
    });
  });

  describe('ConfigurationBuilder', () => {
    it('should build configuration with API key', () => {
      const config = new ConfigurationBuilder()
        .withApiKey('test-key')
        .build();
      expect(config.apiKey).toBe('test-key');
      expect(config.authType).toBe('api-key');
    });

    it('should build configuration with bearer token', () => {
      const config = new ConfigurationBuilder()
        .withBearerToken('test-token')
        .build();
      expect(config.accessToken).toBe('test-token');
      expect(config.authType).toBe('bearer');
    });

    it('should support method chaining', () => {
      const config = new ConfigurationBuilder()
        .withApiKey('test-key')
        .withBasePath('https://custom.api.com')
        .withTimeout(60000)
        .withUserAgent('Custom-Agent/1.0')
        .withConsoleLogging()
        .build();

      expect(config.apiKey).toBe('test-key');
      expect(config.basePath).toBe('https://custom.api.com');
      expect(config.timeout).toBe(60000);
      expect(config.userAgent).toBe('Custom-Agent/1.0');
      expect(config.logger).toBe(consoleLogger);
    });

    it('should validate timeout range', () => {
      expect(() =>
        new ConfigurationBuilder().withTimeout(500)
      ).toThrow();
      expect(() =>
        new ConfigurationBuilder().withTimeout(500000)
      ).toThrow();
    });

    it('should throw for empty values', () => {
      expect(() => new ConfigurationBuilder().withApiKey('')).toThrow();
      expect(() => new ConfigurationBuilder().withBearerToken('')).toThrow();
      expect(() => new ConfigurationBuilder().withBasePath('')).toThrow();
      expect(() => new ConfigurationBuilder().withUserAgent('')).toThrow();
    });
  });

  describe('Constants', () => {
    it('should export correct constants', () => {
      expect(DEFAULT_BASE_PATH).toBe('https://api.adguard-dns.io');
      expect(DEFAULT_TIMEOUT).toBe(30000);
      expect(MIN_TIMEOUT).toBe(1000);
      expect(MAX_TIMEOUT).toBe(300000);
    });
  });

  describe('Loggers', () => {
    it('should have console logger', () => {
      expect(consoleLogger.debug).toBeDefined();
      expect(consoleLogger.info).toBeDefined();
      expect(consoleLogger.warn).toBeDefined();
      expect(consoleLogger.error).toBeDefined();
    });

    it('should have silent logger', () => {
      expect(silentLogger.debug).toBeDefined();
      expect(silentLogger.info).toBeDefined();
      expect(silentLogger.warn).toBeDefined();
      expect(silentLogger.error).toBeDefined();
      // Silent logger should not throw
      expect(() => silentLogger.debug('test')).not.toThrow();
    });
  });
});
