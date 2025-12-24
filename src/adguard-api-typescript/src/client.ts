/**
 * AdGuard DNS API Client
 * Main entry point for the SDK
 */

import {
  ApiConfiguration,
  ConfigurationBuilder,
  createWithApiKey,
  createWithBearerToken,
  Logger,
  maskApiKey,
  validateAuthentication,
} from './helpers/configuration.js';
import {
  AccountApi,
  AuthApi,
  DevicesApi,
  DnsServersApi,
  StatisticsApi,
  QueryLogApi,
  FilterListsApi,
  WebServicesApi,
  DedicatedIpApi,
} from './api/index.js';
import {
  DeviceRepository,
  DnsServerRepository,
  UserRulesRepository,
  StatisticsRepository,
  QueryLogRepository,
} from './repositories/index.js';
import { ApiNotConfiguredError } from './errors/index.js';

/** AdGuard DNS API Client Factory */
export class ApiClientFactory {
  private config?: ApiConfiguration;
  private logger?: Logger;

  /** Check if client is configured */
  get isConfigured(): boolean {
    return !!this.config && validateAuthentication(this.config);
  }

  /** Get masked API key for display */
  get maskedApiKey(): string | undefined {
    if (this.config?.apiKey) {
      return maskApiKey(this.config.apiKey);
    }
    return undefined;
  }

  /**
   * Configure with API key
   * @param apiKey - AdGuard DNS API key
   * @param logger - Optional logger
   */
  configure(apiKey: string, logger?: Logger): void {
    this.config = createWithApiKey(apiKey, undefined, logger);
    this.logger = logger;
  }

  /**
   * Configure with bearer token
   * @param accessToken - OAuth access token
   * @param logger - Optional logger
   */
  configureWithBearerToken(accessToken: string, logger?: Logger): void {
    this.config = createWithBearerToken(accessToken, undefined, logger);
    this.logger = logger;
  }

  /**
   * Configure with custom configuration
   * @param config - API configuration
   */
  configureCustom(config: ApiConfiguration): void {
    this.config = config;
    this.logger = config.logger;
  }

  /**
   * Configure from environment variable
   *
   * Supports multiple environment variable naming conventions:
   * - .NET-compatible format: ADGUARD_AdGuard__ApiKey (recommended, tried first)
   * - Legacy/alternative format: ADGUARD_API_KEY (fallback)
   *
   * @param envVar - Custom environment variable name (overrides default lookup)
   * @param logger - Optional logger
   */
  configureFromEnv(envVar?: string, logger?: Logger): void {
    let apiKey: string | undefined;

    if (envVar) {
      // Use explicitly provided environment variable name
      apiKey = process.env[envVar];
      if (!apiKey) {
        throw new Error(`Environment variable ${envVar} is not set`);
      }
    } else {
      // Try .NET-compatible format first, then fallback to legacy format
      apiKey = process.env['ADGUARD_AdGuard__ApiKey'] ?? process.env['ADGUARD_API_KEY'];
      if (!apiKey) {
        throw new Error(
          'API key not configured. Set ADGUARD_AdGuard__ApiKey (recommended) or ADGUARD_API_KEY environment variable.'
        );
      }
    }

    this.configure(apiKey, logger);
  }

  /**
   * Get configuration or throw if not configured
   */
  private getConfig(): ApiConfiguration {
    if (!this.config || !validateAuthentication(this.config)) {
      throw new ApiNotConfiguredError();
    }
    return this.config;
  }

  /**
   * Test connection by getting account limits
   */
  async testConnection(): Promise<boolean> {
    try {
      const accountApi = this.createAccountApi();
      await accountApi.getAccountLimits();
      return true;
    } catch {
      return false;
    }
  }

  // API factories
  createAccountApi(): AccountApi {
    return new AccountApi(this.getConfig());
  }

  createAuthApi(): AuthApi {
    return new AuthApi(this.getConfig());
  }

  createDevicesApi(): DevicesApi {
    return new DevicesApi(this.getConfig());
  }

  createDnsServersApi(): DnsServersApi {
    return new DnsServersApi(this.getConfig());
  }

  createStatisticsApi(): StatisticsApi {
    return new StatisticsApi(this.getConfig());
  }

  createQueryLogApi(): QueryLogApi {
    return new QueryLogApi(this.getConfig());
  }

  createFilterListsApi(): FilterListsApi {
    return new FilterListsApi(this.getConfig());
  }

  createWebServicesApi(): WebServicesApi {
    return new WebServicesApi(this.getConfig());
  }

  createDedicatedIpApi(): DedicatedIpApi {
    return new DedicatedIpApi(this.getConfig());
  }

  // Repository factories
  createDeviceRepository(): DeviceRepository {
    return new DeviceRepository(this.createDevicesApi(), this.logger);
  }

  createDnsServerRepository(): DnsServerRepository {
    return new DnsServerRepository(this.createDnsServersApi(), this.logger);
  }

  createUserRulesRepository(): UserRulesRepository {
    return new UserRulesRepository(this.createDnsServersApi(), this.logger);
  }

  createStatisticsRepository(): StatisticsRepository {
    return new StatisticsRepository(this.createStatisticsApi(), this.logger);
  }

  createQueryLogRepository(): QueryLogRepository {
    return new QueryLogRepository(this.createQueryLogApi(), this.logger);
  }
}

/** Main AdGuard DNS API Client */
export class AdGuardDnsClient {
  private readonly factory: ApiClientFactory;

  // APIs
  readonly account: AccountApi;
  readonly auth: AuthApi;
  readonly devices: DevicesApi;
  readonly dnsServers: DnsServersApi;
  readonly statistics: StatisticsApi;
  readonly queryLog: QueryLogApi;
  readonly filterLists: FilterListsApi;
  readonly webServices: WebServicesApi;
  readonly dedicatedIp: DedicatedIpApi;

  // Repositories
  readonly deviceRepository: DeviceRepository;
  readonly dnsServerRepository: DnsServerRepository;
  readonly userRulesRepository: UserRulesRepository;
  readonly statisticsRepository: StatisticsRepository;
  readonly queryLogRepository: QueryLogRepository;

  constructor(config: ApiConfiguration) {
    this.factory = new ApiClientFactory();
    this.factory.configureCustom(config);

    // Initialize APIs
    this.account = this.factory.createAccountApi();
    this.auth = this.factory.createAuthApi();
    this.devices = this.factory.createDevicesApi();
    this.dnsServers = this.factory.createDnsServersApi();
    this.statistics = this.factory.createStatisticsApi();
    this.queryLog = this.factory.createQueryLogApi();
    this.filterLists = this.factory.createFilterListsApi();
    this.webServices = this.factory.createWebServicesApi();
    this.dedicatedIp = this.factory.createDedicatedIpApi();

    // Initialize repositories
    this.deviceRepository = this.factory.createDeviceRepository();
    this.dnsServerRepository = this.factory.createDnsServerRepository();
    this.userRulesRepository = this.factory.createUserRulesRepository();
    this.statisticsRepository = this.factory.createStatisticsRepository();
    this.queryLogRepository = this.factory.createQueryLogRepository();
  }

  /**
   * Create client with API key
   * @param apiKey - AdGuard DNS API key
   * @param logger - Optional logger
   */
  static withApiKey(apiKey: string, logger?: Logger): AdGuardDnsClient {
    return new AdGuardDnsClient(createWithApiKey(apiKey, undefined, logger));
  }

  /**
   * Create client with bearer token
   * @param accessToken - OAuth access token
   * @param logger - Optional logger
   */
  static withBearerToken(accessToken: string, logger?: Logger): AdGuardDnsClient {
    return new AdGuardDnsClient(createWithBearerToken(accessToken, undefined, logger));
  }

  /**
   * Create client from environment variable
   *
   * Supports multiple environment variable naming conventions:
   * - .NET-compatible format: ADGUARD_AdGuard__ApiKey (recommended, tried first)
   * - TypeScript format: ADGUARD_API_KEY (fallback)
   *
   * @param envVar - Custom environment variable name (overrides default lookup)
   * @param logger - Optional logger
   */
  static fromEnv(envVar?: string, logger?: Logger): AdGuardDnsClient {
    let apiKey: string | undefined;

    if (envVar) {
      // Use explicitly provided environment variable name
      apiKey = process.env[envVar];
      if (!apiKey) {
        throw new Error(`Environment variable ${envVar} is not set`);
      }
    } else {
      // Try .NET-compatible format first, then fallback to legacy format
      apiKey = process.env['ADGUARD_AdGuard__ApiKey'] ?? process.env['ADGUARD_API_KEY'];
      if (!apiKey) {
        throw new Error(
          'API key not configured. Set ADGUARD_AdGuard__ApiKey (recommended) or ADGUARD_API_KEY environment variable.'
        );
      }
    }

    return AdGuardDnsClient.withApiKey(apiKey, logger);
  }

  /**
   * Create client with configuration builder
   */
  static builder(): ConfigurationBuilder {
    return new ConfigurationBuilder();
  }

  /**
   * Test connection by getting account limits
   */
  async testConnection(): Promise<boolean> {
    try {
      await this.account.getAccountLimits();
      return true;
    } catch {
      return false;
    }
  }
}

export { ConfigurationBuilder };
