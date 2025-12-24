/**
 * AdGuard DNS API Library
 *
 * High-level library API for programmatic usage.
 *
 * @example
 * ```typescript
 * import { AdGuardDnsClientBuilder, createPagedList } from '@adguard/api-typescript/lib';
 *
 * // Create client with builder
 * const client = AdGuardDnsClientBuilder.create()
 *   .withApiKey('your-api-key')
 *   .withTimeout(60000)
 *   .withConsoleLogging()
 *   .build();
 *
 * // Test connection
 * const connected = await client.testConnection();
 *
 * // Use repositories for high-level operations
 * const devices = await client.deviceRepository.getAll();
 *
 * // With pagination
 * const pagedDevices = createPagedList(() => client.deviceRepository.getAll())
 *   .filter(d => d.settings?.protectionEnabled)
 *   .take(10)
 *   .toArray();
 * ```
 *
 * @packageDocumentation
 */

// Client builder
export {
  AdGuardDnsClientBuilder,
  createClientBuilder,
  Logger,
  consoleLogger,
  silentLogger,
  DEFAULT_TIMEOUT,
  MIN_TIMEOUT,
  MAX_TIMEOUT,
} from './client-builder.ts';
export type { RetryOptions } from './client-builder.ts';

// Pagination support
export {
  PagedListBuilder,
  createPagedList,
  paginate,
  createPageResult,
} from './pagination.ts';
export type {
  PagedList,
  PaginationOptions,
  PageResult,
} from './pagination.ts';

// Re-export main client
export { AdGuardDnsClient, ApiClientFactory } from '../client.ts';
export { ConfigurationBuilder } from '../helpers/configuration.ts';
export type { ApiConfiguration, AuthType } from '../helpers/configuration.ts';

// Re-export repositories for convenience
export {
  DeviceRepository,
  DnsServerRepository,
  UserRulesRepository,
  StatisticsRepository,
  QueryLogRepository,
} from '../repositories/index.ts';

// Re-export common types
export type { Device, DeviceCreate, DeviceUpdate } from '../models/index.ts';
