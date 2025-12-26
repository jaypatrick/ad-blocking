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
  consoleLogger,
  createClientBuilder,
  DEFAULT_TIMEOUT,
  MAX_TIMEOUT,
  MIN_TIMEOUT,
  silentLogger,
} from './client-builder.ts';
export type { Logger, RetryOptions } from './client-builder.ts';

// Pagination support
export { createPagedList, createPageResult, PagedListBuilder, paginate } from './pagination.ts';
export type { PagedList, PageResult, PaginationOptions } from './pagination.ts';

// Re-export main client
export { AdGuardDnsClient, ApiClientFactory } from '../client.ts';
export { ConfigurationBuilder } from '../helpers/configuration.ts';
export type { ApiConfiguration, AuthType } from '../helpers/configuration.ts';

// Re-export repositories for convenience
export {
  DeviceRepository,
  DnsServerRepository,
  QueryLogRepository,
  StatisticsRepository,
  UserRulesRepository,
} from '../repositories/index.ts';

// Re-export common types
export type { Device, DeviceCreate, DeviceUpdate } from '../models/index.ts';
