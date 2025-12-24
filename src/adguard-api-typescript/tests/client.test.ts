/**
 * Client tests
 * Deno-native testing implementation
 */

import { assertEquals, assertInstanceOf, assertThrows } from '@std/assert';
import { AdGuardDnsClient, ApiClientFactory, ConfigurationBuilder } from '../src/client.ts';
import { ApiNotConfiguredError } from '../src/errors/index.ts';

// AdGuardDnsClient.withApiKey tests
Deno.test('AdGuardDnsClient.withApiKey - creates client with API key', () => {
  const client = AdGuardDnsClient.withApiKey('test-api-key');
  assertInstanceOf(client, AdGuardDnsClient);
  assertEquals(client.account !== undefined, true);
  assertEquals(client.devices !== undefined, true);
  assertEquals(client.dnsServers !== undefined, true);
});

// AdGuardDnsClient.withBearerToken tests
Deno.test('AdGuardDnsClient.withBearerToken - creates client with bearer token', () => {
  const client = AdGuardDnsClient.withBearerToken('test-token');
  assertInstanceOf(client, AdGuardDnsClient);
});

// AdGuardDnsClient.fromEnv tests
Deno.test('AdGuardDnsClient.fromEnv - creates client from custom environment variable', () => {
  Deno.env.set('TEST_API_KEY', 'test-key');
  const client = AdGuardDnsClient.fromEnv('TEST_API_KEY');
  assertInstanceOf(client, AdGuardDnsClient);
  Deno.env.delete('TEST_API_KEY');
});

Deno.test('AdGuardDnsClient.fromEnv - prefers ADGUARD_AdGuard__ApiKey when no envVar specified', () => {
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

Deno.test('AdGuardDnsClient.fromEnv - fallback to ADGUARD_API_KEY when ADGUARD_AdGuard__ApiKey not set', () => {
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

Deno.test('AdGuardDnsClient.fromEnv - throws when env var not set', () => {
  assertThrows(() => AdGuardDnsClient.fromEnv('NONEXISTENT_VAR'));
});

// AdGuardDnsClient.builder tests
Deno.test('AdGuardDnsClient.builder - returns ConfigurationBuilder', () => {
  const builder = AdGuardDnsClient.builder();
  assertInstanceOf(builder, ConfigurationBuilder);
});

// AdGuardDnsClient APIs and Repositories tests
Deno.test('AdGuardDnsClient - has all APIs', () => {
  const client = AdGuardDnsClient.withApiKey('test-key');
  assertEquals(client.account !== undefined, true);
  assertEquals(client.auth !== undefined, true);
  assertEquals(client.devices !== undefined, true);
  assertEquals(client.dnsServers !== undefined, true);
  assertEquals(client.statistics !== undefined, true);
  assertEquals(client.queryLog !== undefined, true);
  assertEquals(client.filterLists !== undefined, true);
  assertEquals(client.webServices !== undefined, true);
  assertEquals(client.dedicatedIp !== undefined, true);
});

Deno.test('AdGuardDnsClient - has all repositories', () => {
  const client = AdGuardDnsClient.withApiKey('test-key');
  assertEquals(client.deviceRepository !== undefined, true);
  assertEquals(client.dnsServerRepository !== undefined, true);
  assertEquals(client.userRulesRepository !== undefined, true);
  assertEquals(client.statisticsRepository !== undefined, true);
  assertEquals(client.queryLogRepository !== undefined, true);
});

// ApiClientFactory.isConfigured tests
Deno.test('ApiClientFactory.isConfigured - returns false when not configured', () => {
  const factory = new ApiClientFactory();
  assertEquals(factory.isConfigured, false);
});

Deno.test('ApiClientFactory.isConfigured - returns true after configuration', () => {
  const factory = new ApiClientFactory();
  factory.configure('test-key');
  assertEquals(factory.isConfigured, true);
});

// ApiClientFactory.configure tests
Deno.test('ApiClientFactory.configure - configures with API key', () => {
  const factory = new ApiClientFactory();
  factory.configure('test-key');
  assertEquals(factory.isConfigured, true);
  assertEquals(factory.maskedApiKey, '********');
});

// ApiClientFactory.configureWithBearerToken tests
Deno.test('ApiClientFactory.configureWithBearerToken - configures with bearer token', () => {
  const factory = new ApiClientFactory();
  factory.configureWithBearerToken('test-token');
  assertEquals(factory.isConfigured, true);
});

// ApiClientFactory.configureFromEnv tests
Deno.test('ApiClientFactory.configureFromEnv - configures from environment variable', () => {
  Deno.env.set('TEST_KEY', 'env-key');
  const factory = new ApiClientFactory();
  factory.configureFromEnv('TEST_KEY');
  assertEquals(factory.isConfigured, true);
  Deno.env.delete('TEST_KEY');
});

Deno.test('ApiClientFactory.configureFromEnv - throws when env var not set', () => {
  const factory = new ApiClientFactory();
  assertThrows(() => factory.configureFromEnv('NONEXISTENT'));
});

// ApiClientFactory API factories tests
Deno.test('ApiClientFactory - creates AccountApi', () => {
  const factory = new ApiClientFactory();
  factory.configure('test-key');
  const api = factory.createAccountApi();
  assertEquals(api !== undefined, true);
});

Deno.test('ApiClientFactory - creates DevicesApi', () => {
  const factory = new ApiClientFactory();
  factory.configure('test-key');
  const api = factory.createDevicesApi();
  assertEquals(api !== undefined, true);
});

Deno.test('ApiClientFactory - creates DnsServersApi', () => {
  const factory = new ApiClientFactory();
  factory.configure('test-key');
  const api = factory.createDnsServersApi();
  assertEquals(api !== undefined, true);
});

Deno.test('ApiClientFactory - throws when not configured', () => {
  const unconfigured = new ApiClientFactory();
  assertThrows(() => unconfigured.createAccountApi(), ApiNotConfiguredError);
});

// ApiClientFactory Repository factories tests
Deno.test('ApiClientFactory - creates DeviceRepository', () => {
  const factory = new ApiClientFactory();
  factory.configure('test-key');
  const repo = factory.createDeviceRepository();
  assertEquals(repo !== undefined, true);
});

Deno.test('ApiClientFactory - creates DnsServerRepository', () => {
  const factory = new ApiClientFactory();
  factory.configure('test-key');
  const repo = factory.createDnsServerRepository();
  assertEquals(repo !== undefined, true);
});

Deno.test('ApiClientFactory - creates UserRulesRepository', () => {
  const factory = new ApiClientFactory();
  factory.configure('test-key');
  const repo = factory.createUserRulesRepository();
  assertEquals(repo !== undefined, true);
});

Deno.test('ApiClientFactory - creates StatisticsRepository', () => {
  const factory = new ApiClientFactory();
  factory.configure('test-key');
  const repo = factory.createStatisticsRepository();
  assertEquals(repo !== undefined, true);
});

Deno.test('ApiClientFactory - creates QueryLogRepository', () => {
  const factory = new ApiClientFactory();
  factory.configure('test-key');
  const repo = factory.createQueryLogRepository();
  assertEquals(repo !== undefined, true);
});

// ApiClientFactory.maskedApiKey tests
Deno.test('ApiClientFactory.maskedApiKey - returns undefined when not configured', () => {
  const factory = new ApiClientFactory();
  assertEquals(factory.maskedApiKey, undefined);
});

Deno.test('ApiClientFactory.maskedApiKey - returns masked key when configured', () => {
  const factory = new ApiClientFactory();
  factory.configure('abcdefghijklmnop');
  assertEquals(factory.maskedApiKey, 'abcd...mnop');
});
