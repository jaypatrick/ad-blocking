/**
 * AdGuard DNS API TypeScript SDK
 *
 * A comprehensive TypeScript SDK for the AdGuard DNS API v1.11
 * with feature parity to adguard-api-dotnet.
 *
 * ## Quick Start
 *
 * @example
 * ```typescript
 * import { AdGuardDnsClient } from '@adguard/api-typescript';
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
 *
 * ## Library API (Recommended)
 *
 * For programmatic usage, use the fluent builder pattern:
 *
 * @example
 * ```typescript
 * import { AdGuardDnsClientBuilder, createPagedList } from '@adguard/api-typescript/lib';
 *
 * // Build client with full configuration
 * const client = AdGuardDnsClientBuilder.create()
 *   .withApiKey('your-api-key')
 *   .withTimeout(60000)
 *   .withConsoleLogging()
 *   .build();
 *
 * // Use pagination for list operations
 * const devices = await createPagedList(() => client.deviceRepository.getAll())
 *   .filter(d => d.settings?.protectionEnabled)
 *   .take(10)
 *   .toArray();
 * ```
 *
 * @packageDocumentation
 */

// Main client
export { AdGuardDnsClient, ApiClientFactory, ConfigurationBuilder } from './client.ts';

// Models
export * from './models/index.ts';

// Errors
export * from './errors/index.ts';

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
} from './helpers/datetime.ts';

export {
  RetryPolicy,
  executeWithRetry,
  withRetry,
  isRetryableError,
  createRateLimitRetryPolicy,
} from './helpers/retry.ts';

export {
  ConfigurationHelper,
  consoleLogger,
  silentLogger,
  createWithApiKey,
  createWithBearerToken,
  createCustom,
  validateAuthentication,
  maskApiKey,
  DEFAULT_BASE_PATH,
  DEFAULT_TIMEOUT,
} from './helpers/configuration.ts';
export type {
  ApiConfiguration,
  Logger,
} from './helpers/configuration.ts';

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
} from './api/index.ts';

// Repositories
export {
  DeviceRepository,
  DnsServerRepository,
  UserRulesRepository,
  StatisticsRepository,
  QueryLogRepository,
} from './repositories/index.ts';
export type {
  TimeRange,
} from './repositories/index.ts';

// Rules compiler integration
export {
  RulesCompilerIntegration,
} from './rules-compiler-integration.ts';
export type {
  RulesSyncResult,
  RulesSyncOptions,
} from './rules-compiler-integration.ts';

// Version info
export const VERSION = '1.0.0';
export const API_VERSION = '1.11';

// Library API (high-level)
export {
  AdGuardDnsClientBuilder,
  createClientBuilder,
  PagedListBuilder,
  createPagedList,
  paginate,
  createPageResult,
} from './lib/index.ts';
export type {
  PagedList,
  PaginationOptions,
  PageResult,
} from './lib/index.ts';
