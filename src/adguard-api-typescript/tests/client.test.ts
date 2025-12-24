/**
 * Client tests
 */

/// <reference types="jest" />

import { AdGuardDnsClient, ApiClientFactory, ConfigurationBuilder } from '../src/client';
import { ApiNotConfiguredError } from '../src/errors';

describe('AdGuardDnsClient', () => {
  describe('withApiKey', () => {
    it('should create client with API key', () => {
      const client = AdGuardDnsClient.withApiKey('test-api-key');
      expect(client).toBeInstanceOf(AdGuardDnsClient);
      expect(client.account).toBeDefined();
      expect(client.devices).toBeDefined();
      expect(client.dnsServers).toBeDefined();
    });
  });

  describe('withBearerToken', () => {
    it('should create client with bearer token', () => {
      const client = AdGuardDnsClient.withBearerToken('test-token');
      expect(client).toBeInstanceOf(AdGuardDnsClient);
    });
  });

  describe('fromEnv', () => {
    it('should create client from custom environment variable', () => {
      process.env.TEST_API_KEY = 'test-key';
      const client = AdGuardDnsClient.fromEnv('TEST_API_KEY');
      expect(client).toBeInstanceOf(AdGuardDnsClient);
      delete process.env.TEST_API_KEY;
    });

    it('should prefer ADGUARD_AdGuard__ApiKey when no envVar specified', () => {
      const originalDotNet = process.env.ADGUARD_AdGuard__ApiKey;
      const originalLegacy = process.env.ADGUARD_API_KEY;

      process.env.ADGUARD_AdGuard__ApiKey = 'dotnet-format-key';
      process.env.ADGUARD_API_KEY = 'legacy-format-key';

      const client = AdGuardDnsClient.fromEnv();
      expect(client).toBeInstanceOf(AdGuardDnsClient);

      // Restore original values
      if (originalDotNet !== undefined) {
        process.env.ADGUARD_AdGuard__ApiKey = originalDotNet;
      } else {
        delete process.env.ADGUARD_AdGuard__ApiKey;
      }
      if (originalLegacy !== undefined) {
        process.env.ADGUARD_API_KEY = originalLegacy;
      } else {
        delete process.env.ADGUARD_API_KEY;
      }
    });

    it('should fallback to ADGUARD_API_KEY when ADGUARD_AdGuard__ApiKey not set', () => {
      const originalDotNet = process.env.ADGUARD_AdGuard__ApiKey;
      const originalLegacy = process.env.ADGUARD_API_KEY;

      delete process.env.ADGUARD_AdGuard__ApiKey;
      process.env.ADGUARD_API_KEY = 'legacy-format-key';

      const client = AdGuardDnsClient.fromEnv();
      expect(client).toBeInstanceOf(AdGuardDnsClient);

      // Restore original values
      if (originalDotNet !== undefined) {
        process.env.ADGUARD_AdGuard__ApiKey = originalDotNet;
      }
      if (originalLegacy !== undefined) {
        process.env.ADGUARD_API_KEY = originalLegacy;
      } else {
        delete process.env.ADGUARD_API_KEY;
      }
    });

    it('should throw when env var not set', () => {
      expect(() => AdGuardDnsClient.fromEnv('NONEXISTENT_VAR')).toThrow();
    });
  });

  describe('builder', () => {
    it('should return ConfigurationBuilder', () => {
      const builder = AdGuardDnsClient.builder();
      expect(builder).toBeInstanceOf(ConfigurationBuilder);
    });
  });

  describe('APIs and Repositories', () => {
    let client: AdGuardDnsClient;

    beforeAll(() => {
      client = AdGuardDnsClient.withApiKey('test-key');
    });

    it('should have all APIs', () => {
      expect(client.account).toBeDefined();
      expect(client.auth).toBeDefined();
      expect(client.devices).toBeDefined();
      expect(client.dnsServers).toBeDefined();
      expect(client.statistics).toBeDefined();
      expect(client.queryLog).toBeDefined();
      expect(client.filterLists).toBeDefined();
      expect(client.webServices).toBeDefined();
      expect(client.dedicatedIp).toBeDefined();
    });

    it('should have all repositories', () => {
      expect(client.deviceRepository).toBeDefined();
      expect(client.dnsServerRepository).toBeDefined();
      expect(client.userRulesRepository).toBeDefined();
      expect(client.statisticsRepository).toBeDefined();
      expect(client.queryLogRepository).toBeDefined();
    });
  });
});

describe('ApiClientFactory', () => {
  let factory: ApiClientFactory;

  beforeEach(() => {
    factory = new ApiClientFactory();
  });

  describe('isConfigured', () => {
    it('should return false when not configured', () => {
      expect(factory.isConfigured).toBe(false);
    });

    it('should return true after configuration', () => {
      factory.configure('test-key');
      expect(factory.isConfigured).toBe(true);
    });
  });

  describe('configure', () => {
    it('should configure with API key', () => {
      factory.configure('test-key');
      expect(factory.isConfigured).toBe(true);
      expect(factory.maskedApiKey).toBe('********');
    });
  });

  describe('configureWithBearerToken', () => {
    it('should configure with bearer token', () => {
      factory.configureWithBearerToken('test-token');
      expect(factory.isConfigured).toBe(true);
    });
  });

  describe('configureFromEnv', () => {
    it('should configure from environment variable', () => {
      process.env.TEST_KEY = 'env-key';
      factory.configureFromEnv('TEST_KEY');
      expect(factory.isConfigured).toBe(true);
      delete process.env.TEST_KEY;
    });

    it('should throw when env var not set', () => {
      expect(() => factory.configureFromEnv('NONEXISTENT')).toThrow();
    });
  });

  describe('API factories', () => {
    beforeEach(() => {
      factory.configure('test-key');
    });

    it('should create AccountApi', () => {
      const api = factory.createAccountApi();
      expect(api).toBeDefined();
    });

    it('should create DevicesApi', () => {
      const api = factory.createDevicesApi();
      expect(api).toBeDefined();
    });

    it('should create DnsServersApi', () => {
      const api = factory.createDnsServersApi();
      expect(api).toBeDefined();
    });

    it('should throw when not configured', () => {
      const unconfigured = new ApiClientFactory();
      expect(() => unconfigured.createAccountApi()).toThrow(ApiNotConfiguredError);
    });
  });

  describe('Repository factories', () => {
    beforeEach(() => {
      factory.configure('test-key');
    });

    it('should create DeviceRepository', () => {
      const repo = factory.createDeviceRepository();
      expect(repo).toBeDefined();
    });

    it('should create DnsServerRepository', () => {
      const repo = factory.createDnsServerRepository();
      expect(repo).toBeDefined();
    });

    it('should create UserRulesRepository', () => {
      const repo = factory.createUserRulesRepository();
      expect(repo).toBeDefined();
    });

    it('should create StatisticsRepository', () => {
      const repo = factory.createStatisticsRepository();
      expect(repo).toBeDefined();
    });

    it('should create QueryLogRepository', () => {
      const repo = factory.createQueryLogRepository();
      expect(repo).toBeDefined();
    });
  });

  describe('maskedApiKey', () => {
    it('should return undefined when not configured', () => {
      expect(factory.maskedApiKey).toBeUndefined();
    });

    it('should return masked key when configured', () => {
      factory.configure('abcdefghijklmnop');
      expect(factory.maskedApiKey).toBe('abcd...mnop');
    });
  });
});
