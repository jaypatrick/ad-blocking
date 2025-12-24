/**
 * Configuration helper tests
 * Deno-native testing implementation
 */

import { assertEquals, assertThrows } from '@std/assert';
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
} from '../../src/helpers/configuration.ts';

// createWithApiKey tests
Deno.test('createWithApiKey - creates configuration with API key', () => {
  const config = createWithApiKey('test-api-key');
  assertEquals(config.apiKey, 'test-api-key');
  assertEquals(config.authType, 'api-key');
  assertEquals(config.basePath, DEFAULT_BASE_PATH);
  assertEquals(config.timeout, DEFAULT_TIMEOUT);
});

Deno.test('createWithApiKey - throws for empty API key', () => {
  assertThrows(() => createWithApiKey(''));
  assertThrows(() => createWithApiKey('   '));
});

Deno.test('createWithApiKey - trims API key', () => {
  const config = createWithApiKey('  test-key  ');
  assertEquals(config.apiKey, 'test-key');
});

Deno.test('createWithApiKey - accepts custom base path', () => {
  const config = createWithApiKey('test-key', 'https://custom.api.com');
  assertEquals(config.basePath, 'https://custom.api.com');
});

Deno.test('createWithApiKey - accepts logger', () => {
  const config = createWithApiKey('test-key', undefined, consoleLogger);
  assertEquals(config.logger, consoleLogger);
});

// createWithBearerToken tests
Deno.test('createWithBearerToken - creates configuration with bearer token', () => {
  const config = createWithBearerToken('test-token');
  assertEquals(config.accessToken, 'test-token');
  assertEquals(config.authType, 'bearer');
});

Deno.test('createWithBearerToken - throws for empty token', () => {
  assertThrows(() => createWithBearerToken(''));
});

// createCustom tests
Deno.test('createCustom - creates default configuration', () => {
  const config = createCustom();
  assertEquals(config.basePath, DEFAULT_BASE_PATH);
  assertEquals(config.timeout, DEFAULT_TIMEOUT);
});

Deno.test('createCustom - accepts custom values', () => {
  const config = createCustom(
    'https://custom.api.com',
    60000,
    'Custom-Agent/1.0',
    consoleLogger,
  );
  assertEquals(config.basePath, 'https://custom.api.com');
  assertEquals(config.timeout, 60000);
  assertEquals(config.userAgent, 'Custom-Agent/1.0');
  assertEquals(config.logger, consoleLogger);
});

Deno.test('createCustom - validates timeout range', () => {
  assertThrows(() => createCustom(undefined, 500));
  assertThrows(() => createCustom(undefined, 500000));
});

// validateAuthentication tests
Deno.test('validateAuthentication - returns true for valid API key config', () => {
  const config = createWithApiKey('test-key');
  assertEquals(validateAuthentication(config), true);
});

Deno.test('validateAuthentication - returns true for valid bearer token config', () => {
  const config = createWithBearerToken('test-token');
  assertEquals(validateAuthentication(config), true);
});

Deno.test('validateAuthentication - returns false for config without auth', () => {
  const config = createCustom();
  assertEquals(validateAuthentication(config), false);
});

// maskApiKey tests
Deno.test('maskApiKey - masks API key showing first and last 4 chars', () => {
  const result = maskApiKey('abcdefghijklmnop');
  assertEquals(result, 'abcd...mnop');
});

Deno.test('maskApiKey - fully masks short API keys', () => {
  const result = maskApiKey('short');
  assertEquals(result, '********');
});

// ConfigurationBuilder tests
Deno.test('ConfigurationBuilder - builds configuration with API key', () => {
  const config = new ConfigurationBuilder()
    .withApiKey('test-key')
    .build();
  assertEquals(config.apiKey, 'test-key');
  assertEquals(config.authType, 'api-key');
});

Deno.test('ConfigurationBuilder - builds configuration with bearer token', () => {
  const config = new ConfigurationBuilder()
    .withBearerToken('test-token')
    .build();
  assertEquals(config.accessToken, 'test-token');
  assertEquals(config.authType, 'bearer');
});

Deno.test('ConfigurationBuilder - supports method chaining', () => {
  const config = new ConfigurationBuilder()
    .withApiKey('test-key')
    .withBasePath('https://custom.api.com')
    .withTimeout(60000)
    .withUserAgent('Custom-Agent/1.0')
    .withConsoleLogging()
    .build();

  assertEquals(config.apiKey, 'test-key');
  assertEquals(config.basePath, 'https://custom.api.com');
  assertEquals(config.timeout, 60000);
  assertEquals(config.userAgent, 'Custom-Agent/1.0');
  assertEquals(config.logger, consoleLogger);
});

Deno.test('ConfigurationBuilder - validates timeout range', () => {
  assertThrows(() => new ConfigurationBuilder().withTimeout(500));
  assertThrows(() => new ConfigurationBuilder().withTimeout(500000));
});

Deno.test('ConfigurationBuilder - throws for empty values', () => {
  assertThrows(() => new ConfigurationBuilder().withApiKey(''));
  assertThrows(() => new ConfigurationBuilder().withBearerToken(''));
  assertThrows(() => new ConfigurationBuilder().withBasePath(''));
  assertThrows(() => new ConfigurationBuilder().withUserAgent(''));
});

// Constants tests
Deno.test('Constants - exports correct values', () => {
  assertEquals(DEFAULT_BASE_PATH, 'https://api.adguard-dns.io');
  assertEquals(DEFAULT_TIMEOUT, 30000);
  assertEquals(MIN_TIMEOUT, 1000);
  assertEquals(MAX_TIMEOUT, 300000);
});

// Loggers tests
Deno.test('Loggers - console logger has all methods', () => {
  assertEquals(typeof consoleLogger.debug, 'function');
  assertEquals(typeof consoleLogger.info, 'function');
  assertEquals(typeof consoleLogger.warn, 'function');
  assertEquals(typeof consoleLogger.error, 'function');
});

Deno.test('Loggers - silent logger has all methods', () => {
  assertEquals(typeof silentLogger.debug, 'function');
  assertEquals(typeof silentLogger.info, 'function');
  assertEquals(typeof silentLogger.warn, 'function');
  assertEquals(typeof silentLogger.error, 'function');
  // Silent logger should not throw
  silentLogger.debug('test');
  silentLogger.info('test');
  silentLogger.warn('test');
  silentLogger.error('test');
});
