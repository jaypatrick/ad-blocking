/**
 * Client tests - Basic structure validation
 * Full test coverage to be added in follow-up
 */

import { assertEquals, assert, assertThrows } from '@std/assert';
import { AdGuardDnsClient, ApiClientFactory } from '../src/client.ts';
import { ApiNotConfiguredError } from '../src/errors/index.ts';

Deno.test('AdGuardDnsClient', async (t) => {
  await t.step('withApiKey - should create client with API key', () => {
    const client = AdGuardDnsClient.withApiKey('test-api-key');
    assert(client instanceof AdGuardDnsClient);
    assert(client.account !== undefined);
    assert(client.devices !== undefined);
    assert(client.dnsServers !== undefined);
  });

  await t.step('withBearerToken - should create client with bearer token', () => {
    const client = AdGuardDnsClient.withBearerToken('test-token');
    assert(client instanceof AdGuardDnsClient);
  });

  await t.step('fromEnv - should throw when env var not set', () => {
    assertThrows(() => AdGuardDnsClient.fromEnv('NONEXISTENT_VAR'));
  });

  await t.step('should have all APIs', () => {
    const client = AdGuardDnsClient.withApiKey('test-key');
    assert(client.account !== undefined);
    assert(client.auth !== undefined);
    assert(client.devices !== undefined);
    assert(client.dnsServers !== undefined);
    assert(client.statistics !== undefined);
    assert(client.queryLog !== undefined);
    assert(client.filterLists !== undefined);
    assert(client.webServices !== undefined);
    assert(client.dedicatedIp !== undefined);
  });

  await t.step('should have all repositories', () => {
    const client = AdGuardDnsClient.withApiKey('test-key');
    assert(client.deviceRepository !== undefined);
    assert(client.dnsServerRepository !== undefined);
    assert(client.userRulesRepository !== undefined);
    assert(client.statisticsRepository !== undefined);
    assert(client.queryLogRepository !== undefined);
  });
});

Deno.test('ApiClientFactory', async (t) => {
  await t.step('isConfigured - should return false when not configured', () => {
    const factory = new ApiClientFactory();
    assertEquals(factory.isConfigured, false);
  });

  await t.step('configure - should configure with API key', () => {
    const factory = new ApiClientFactory();
    factory.configure('test-key');
    assertEquals(factory.isConfigured, true);
    assertEquals(factory.maskedApiKey, '********');
  });

  await t.step('should throw when not configured', () => {
    const unconfigured = new ApiClientFactory();
    assertThrows(() => unconfigured.createAccountApi(), ApiNotConfiguredError);
  });
});
