/**
 * AdGuard DNS API TypeScript SDK
 *
 * A comprehensive TypeScript SDK for the AdGuard DNS API v1.11
 * with feature parity to adguard-api-dotnet.
 *
 * @example
 * ```typescript
 * import { AdGuardDnsClient } from 'adguard-api-typescript';
 *
 * // Create client with API key
 * const client = AdGuardDnsClient.withApiKey('your-api-key');
 *
 * // Or from environment variable
 * const client = AdGuardDnsClient.fromEnv('ADGUARD_API_KEY');
 *
 * // Use APIs directly
 * const devices = await client.devices.listDevices();
 *
 * // Or use repositories for higher-level operations
 * const device = await client.deviceRepository.getById('device-id');
 * ```
 */

// Main client
export { AdGuardDnsClient, ApiClientFactory, ConfigurationBuilder } from './client';

// Models
export * from './models';

// Errors
export * from './errors';

// Helpers
export {
  DateTime,
  toUnixMilliseconds,
  fromUnixMilliseconds,
  daysAgo,
  hoursAgo,
  minutesAgo,
  startOfToday,
  endOfToday,
  formatRelative,
} from './helpers/datetime';

export {
  RetryPolicy,
  executeWithRetry,
  withRetry,
  isRetryableError,
  createRateLimitRetryPolicy,
} from './helpers/retry';

export {
  ConfigurationHelper,
  ApiConfiguration,
  Logger,
  consoleLogger,
  silentLogger,
  createWithApiKey,
  createWithBearerToken,
  createCustom,
  validateAuthentication,
  maskApiKey,
  DEFAULT_BASE_PATH,
  DEFAULT_TIMEOUT,
} from './helpers/configuration';

// APIs
export {
  AccountApi,
  AuthApi,
  DevicesApi,
  DnsServersApi,
  StatisticsApi,
  QueryLogApi,
  FilterListsApi,
  WebServicesApi,
  DedicatedIpApi,
} from './api';

// Repositories
export {
  DeviceRepository,
  DnsServerRepository,
  UserRulesRepository,
  StatisticsRepository,
  QueryLogRepository,
  TimeRange,
} from './repositories';

// Rules compiler integration
export {
  RulesCompilerIntegration,
  RulesSyncResult,
  RulesSyncOptions,
} from './rules-compiler-integration';

// Version info
export const VERSION = '1.0.0';
export const API_VERSION = '1.11';
