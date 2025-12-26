/**
 * Configuration helper tests - Basic validation
 * Full test coverage to be added in follow-up
 */

import { assertEquals, assertThrows } from '@std/assert';
import {
  ConfigurationBuilder,
  createWithApiKey,
  createWithBearerToken,
  maskApiKey,
  DEFAULT_BASE_PATH,
  DEFAULT_TIMEOUT,
  MIN_TIMEOUT,
  MAX_TIMEOUT,
  consoleLogger,
  silentLogger,
  validateAuthentication,
} from '../../src/helpers/configuration.ts';

Deno.test('ConfigurationHelper', async (t) => {
  await t.step('createWithApiKey - should create configuration with API key', () => {
    const config = createWithApiKey('test-api-key');
    assertEquals(config.apiKey, 'test-api-key');
    assertEquals(config.authType, 'api-key');
    assertEquals(config.basePath, DEFAULT_BASE_PATH);
    assertEquals(config.timeout, DEFAULT_TIMEOUT);
  });

  await t.step('createWithApiKey - should throw for empty API key', () => {
    assertThrows(() => createWithApiKey(''));
    assertThrows(() => createWithApiKey('   '));
  });

  await t.step('createWithBearerToken - should create configuration with bearer token', () => {
    const config = createWithBearerToken('test-token');
    assertEquals(config.accessToken, 'test-token');
    assertEquals(config.authType, 'bearer');
  });

  await t.step('maskApiKey - should mask API key', () => {
    assertEquals(maskApiKey('abcdefghijklmnop'), 'abcd...mnop');
    assertEquals(maskApiKey('short'), '********');
  });

  await t.step('validateAuthentication - should validate config', () => {
    const validConfig = createWithApiKey('test-key');
    assertEquals(validateAuthentication(validConfig), true);
  });

  await t.step('ConfigurationBuilder - should build configuration', () => {
    const config = new ConfigurationBuilder()
      .withApiKey('test-key')
      .withTimeout(60000)
      .build();
    assertEquals(config.apiKey, 'test-key');
    assertEquals(config.timeout, 60000);
  });

  await t.step('ConfigurationBuilder - should validate timeout range', () => {
    assertThrows(() => new ConfigurationBuilder().withTimeout(500));
    assertThrows(() => new ConfigurationBuilder().withTimeout(500000));
  });

  await t.step('Constants - should export correct constants', () => {
    assertEquals(DEFAULT_BASE_PATH, 'https://api.adguard-dns.io');
    assertEquals(DEFAULT_TIMEOUT, 30000);
    assertEquals(MIN_TIMEOUT, 1000);
    assertEquals(MAX_TIMEOUT, 300000);
  });

  await t.step('Loggers - should have console and silent loggers', () => {
    assertEquals(typeof consoleLogger.debug, 'function');
    assertEquals(typeof silentLogger.debug, 'function');
  });
});
