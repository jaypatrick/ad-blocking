/**
 * Client tests
 */

import { assertEquals, assertExists, assertThrows, assertInstanceOf } from '@std/assert';
import { AdGuardDnsClient, ApiClientFactory, ConfigurationBuilder } from '../src/client.ts';
import { ApiNotConfiguredError } from '../src/errors/index.ts';

// AdGuardDnsClient.withApiKey tests
Deno.test('AdGuardDnsClient.withApiKey - should create client with API key', () => {
  const client = AdGuardDnsClient.withApiKey('test-api-key');
  assertInstanceOf(client, AdGuardDnsClient);
  assertExists(client.account);
  assertExists(client.devices);
  assertExists(client.dnsServers);
});

// AdGuardDnsClient.withBearerToken tests
Deno.test('AdGuardDnsClient.withBearerToken - should create client with bearer token', () => {
  const client = AdGuardDnsClient.withBearerToken('test-token');
  assertInstanceOf(client, AdGuardDnsClient);
});

// AdGuardDnsClient.fromEnv tests
Deno.test('AdGuardDnsClient.fromEnv - should create client from custom environment variable', () => {
  Deno.env.set('TEST_API_KEY', 'test-key');
  const client = AdGuardDnsClient.fromEnv('TEST_API_KEY');
  assertInstanceOf(client, AdGuardDnsClient);
  Deno.env.delete('TEST_API_KEY');
});

Deno.test('AdGuardDnsClient.fromEnv - should prefer ADGUARD_AdGuard__ApiKey when no envVar specified', () => {
  const originalDotNet = Deno.env.get('ADGUARD_AdGuard__ApiKey');
  const originalLegacy = Deno.env.get('ADGUARD_API_KEY');

  Deno.env.set('ADGUARD_AdGuard__ApiKey', 'dotnet-format-key');
  Deno.env.set('ADGUARD_API_KEY', 'legacy-format-key');

  const client = AdGuardDnsClient.fromEnv();
  assertInstanceOf(client, AdGuardDnsClient);

  // Restore original values
  if (originalDotNet !== undefined) {
    Deno.env.set('ADGUARD_AdGuard__ApiKey', originalDotNet);
  } else {
    Deno.env.delete('ADGUARD_AdGuard__ApiKey');
  }
  if (originalLegacy !== undefined) {
    Deno.env.set('ADGUARD_API_KEY', originalLegacy);
  } else {
    Deno.env.delete('ADGUARD_API_KEY');
  }
});

Deno.test('AdGuardDnsClient.fromEnv - should fallback to ADGUARD_API_KEY when ADGUARD_AdGuard__ApiKey not set', () => {
  const originalDotNet = Deno.env.get('ADGUARD_AdGuard__ApiKey');
  const originalLegacy = Deno.env.get('ADGUARD_API_KEY');

  Deno.env.delete('ADGUARD_AdGuard__ApiKey');
  Deno.env.set('ADGUARD_API_KEY', 'legacy-format-key');

  const client = AdGuardDnsClient.fromEnv();
  assertInstanceOf(client, AdGuardDnsClient);

  // Restore original values
  if (originalDotNet !== undefined) {
    Deno.env.set('ADGUARD_AdGuard__ApiKey', originalDotNet);
  }
  if (originalLegacy !== undefined) {
    Deno.env.set('ADGUARD_API_KEY', originalLegacy);
  } else {
    Deno.env.delete('ADGUARD_API_KEY');
  }
});

Deno.test('AdGuardDnsClient.fromEnv - should throw when env var not set', () => {
  assertThrows(() => AdGuardDnsClient.fromEnv('NONEXISTENT_VAR'));
});

// AdGuardDnsClient.builder tests
Deno.test('AdGuardDnsClient.builder - should return ConfigurationBuilder', () => {
  const builder = AdGuardDnsClient.builder();
  assertInstanceOf(builder, ConfigurationBuilder);
});

// AdGuardDnsClient APIs and Repositories tests
Deno.test('AdGuardDnsClient - should have all APIs', () => {
  const client = AdGuardDnsClient.withApiKey('test-key');
  assertExists(client.account);
  assertExists(client.auth);
  assertExists(client.devices);
  assertExists(client.dnsServers);
  assertExists(client.statistics);
  assertExists(client.queryLog);
  assertExists(client.filterLists);
  assertExists(client.webServices);
  assertExists(client.dedicatedIp);
});

Deno.test('AdGuardDnsClient - should have all repositories', () => {
  const client = AdGuardDnsClient.withApiKey('test-key');
  assertExists(client.deviceRepository);
  assertExists(client.dnsServerRepository);
  assertExists(client.userRulesRepository);
  assertExists(client.statisticsRepository);
  assertExists(client.queryLogRepository);
});

// ApiClientFactory tests
Deno.test('ApiClientFactory.isConfigured - should return false when not configured', () => {
  const factory = new ApiClientFactory();
  assertEquals(factory.isConfigured, false);
});

Deno.test('ApiClientFactory.isConfigured - should return true after configuration', () => {
  const factory = new ApiClientFactory();
  factory.configure('test-key');
  assertEquals(factory.isConfigured, true);
});

Deno.test('ApiClientFactory.configure - should configure with API key', () => {
  const factory = new ApiClientFactory();
  factory.configure('test-key');
  assertEquals(factory.isConfigured, true);
  assertEquals(factory.maskedApiKey, '********');
});

Deno.test('ApiClientFactory.configureWithBearerToken - should configure with bearer token', () => {
  const factory = new ApiClientFactory();
  factory.configureWithBearerToken('test-token');
  assertEquals(factory.isConfigured, true);
});

Deno.test('ApiClientFactory.configureFromEnv - should configure from environment variable', () => {
  Deno.env.set('TEST_KEY', 'env-key');
  const factory = new ApiClientFactory();
  factory.configureFromEnv('TEST_KEY');
  assertEquals(factory.isConfigured, true);
  Deno.env.delete('TEST_KEY');
});

Deno.test('ApiClientFactory.configureFromEnv - should throw when env var not set', () => {
  const factory = new ApiClientFactory();
  assertThrows(() => factory.configureFromEnv('NONEXISTENT'));
});

// ApiClientFactory API factories tests
Deno.test('ApiClientFactory.createAccountApi - should create AccountApi', () => {
  const factory = new ApiClientFactory();
  factory.configure('test-key');
  const api = factory.createAccountApi();
  assertExists(api);
});

Deno.test('ApiClientFactory.createDevicesApi - should create DevicesApi', () => {
  const factory = new ApiClientFactory();
  factory.configure('test-key');
  const api = factory.createDevicesApi();
  assertExists(api);
});

Deno.test('ApiClientFactory.createDnsServersApi - should create DnsServersApi', () => {
  const factory = new ApiClientFactory();
  factory.configure('test-key');
  const api = factory.createDnsServersApi();
  assertExists(api);
});

Deno.test('ApiClientFactory - should throw when not configured', () => {
  const unconfigured = new ApiClientFactory();
  assertThrows(() => unconfigured.createAccountApi(), ApiNotConfiguredError);
});

// ApiClientFactory Repository factories tests
Deno.test('ApiClientFactory.createDeviceRepository - should create DeviceRepository', () => {
  const factory = new ApiClientFactory();
  factory.configure('test-key');
  const repo = factory.createDeviceRepository();
  assertExists(repo);
});

Deno.test('ApiClientFactory.createDnsServerRepository - should create DnsServerRepository', () => {
  const factory = new ApiClientFactory();
  factory.configure('test-key');
  const repo = factory.createDnsServerRepository();
  assertExists(repo);
});

Deno.test('ApiClientFactory.createUserRulesRepository - should create UserRulesRepository', () => {
  const factory = new ApiClientFactory();
  factory.configure('test-key');
  const repo = factory.createUserRulesRepository();
  assertExists(repo);
});

Deno.test('ApiClientFactory.createStatisticsRepository - should create StatisticsRepository', () => {
  const factory = new ApiClientFactory();
  factory.configure('test-key');
  const repo = factory.createStatisticsRepository();
  assertExists(repo);
});

Deno.test('ApiClientFactory.createQueryLogRepository - should create QueryLogRepository', () => {
  const factory = new ApiClientFactory();
  factory.configure('test-key');
  const repo = factory.createQueryLogRepository();
  assertExists(repo);
});

// maskedApiKey tests
Deno.test('ApiClientFactory.maskedApiKey - should return undefined when not configured', () => {
  const factory = new ApiClientFactory();
  assertEquals(factory.maskedApiKey, undefined);
});

Deno.test('ApiClientFactory.maskedApiKey - should return masked key when configured', () => {
  const factory = new ApiClientFactory();
  factory.configure('abcdefghijklmnop');
  assertEquals(factory.maskedApiKey, 'abcd...mnop');
});
