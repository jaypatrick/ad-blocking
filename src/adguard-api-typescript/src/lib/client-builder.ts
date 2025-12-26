/**
 * AdGuardDnsClientBuilder - Fluent builder for creating AdGuard DNS clients
 *
 * Provides a clean, fluent API for configuring and building the client.
 *
 * @example
 * ```typescript
 * // Basic usage
 * const client = AdGuardDnsClientBuilder.create()
 *   .withApiKey('your-api-key')
 *   .build();
 *
 * // With all options
 * const client = AdGuardDnsClientBuilder.create()
 *   .withApiKey('your-api-key')
 *   .withTimeout(60000)
 *   .withRetry({ maxRetries: 3, retryDelay: 1000 })
 *   .withConsoleLogging()
 *   .build();
 *
 * // From environment
 * const client = AdGuardDnsClientBuilder.create()
 *   .fromEnv('ADGUARD_API_KEY')
 *   .withTimeout(30000)
 *   .build();
 * ```
 */

import { AdGuardDnsClient } from '../client.ts';
import {
  ConfigurationBuilder,
  consoleLogger,
  DEFAULT_TIMEOUT,
  Logger,
  MAX_TIMEOUT,
  MIN_TIMEOUT,
  silentLogger,
} from '../helpers/configuration.ts';
import { RetryOptions } from '../helpers/retry.ts';

/**
 * Builder for creating AdGuard DNS clients with fluent API
 */
export class AdGuardDnsClientBuilder {
  private configBuilder: ConfigurationBuilder;

  constructor() {
    this.configBuilder = new ConfigurationBuilder();
  }

  /**
   * Create a new builder instance
   */
  static create(): AdGuardDnsClientBuilder {
    return new AdGuardDnsClientBuilder();
  }

  /**
   * Set API key authentication
   * @param apiKey AdGuard DNS API key
   */
  withApiKey(apiKey: string): this {
    this.configBuilder.withApiKey(apiKey);
    return this;
  }

  /**
   * Set bearer token authentication
   * @param accessToken OAuth access token
   */
  withBearerToken(accessToken: string): this {
    this.configBuilder.withBearerToken(accessToken);
    return this;
  }

  /**
   * Configure from environment variable
   * @param envVar Environment variable name (default: ADGUARD_API_KEY or ADGUARD_AdGuard__ApiKey)
   */
  fromEnv(envVar?: string): this {
    let apiKey: string | undefined;

    if (envVar) {
      apiKey = Deno.env.get(envVar);
      if (!apiKey) {
        throw new Error(`Environment variable ${envVar} is not set`);
      }
    } else {
      // Try .NET-compatible format first, then fallback to legacy format
      apiKey = Deno.env.get('ADGUARD_AdGuard__ApiKey') ?? Deno.env.get('ADGUARD_API_KEY');
      if (!apiKey) {
        throw new Error(
          'API key not configured. Set ADGUARD_AdGuard__ApiKey (recommended) or ADGUARD_API_KEY environment variable.',
        );
      }
    }

    this.configBuilder.withApiKey(apiKey);
    return this;
  }

  /**
   * Set custom base URL
   * @param basePath Base URL for the API (default: https://api.adguard-dns.io)
   */
  withBasePath(basePath: string): this {
    this.configBuilder.withBasePath(basePath);
    return this;
  }

  /**
   * Set request timeout
   * @param timeoutMs Timeout in milliseconds (1000-300000)
   */
  withTimeout(timeoutMs: number): this {
    this.configBuilder.withTimeout(timeoutMs);
    return this;
  }

  /**
   * Set custom user agent
   * @param userAgent User agent string
   */
  withUserAgent(userAgent: string): this {
    this.configBuilder.withUserAgent(userAgent);
    return this;
  }

  /**
   * Set retry options for failed requests
   * @param options Retry configuration
   */
  withRetry(options: RetryOptions): this {
    this.configBuilder.withRetryOptions(options);
    return this;
  }

  /**
   * Set a custom logger
   * @param logger Logger instance
   */
  withLogger(logger: Logger): this {
    this.configBuilder.withLogger(logger);
    return this;
  }

  /**
   * Enable console logging
   */
  withConsoleLogging(): this {
    this.configBuilder.withConsoleLogging();
    return this;
  }

  /**
   * Disable all logging
   */
  withSilentLogging(): this {
    this.configBuilder.withLogger(silentLogger);
    return this;
  }

  /**
   * Build the AdGuardDnsClient instance
   */
  build(): AdGuardDnsClient {
    const config = this.configBuilder.build();
    return new AdGuardDnsClient(config);
  }

  /**
   * Build and test the connection
   * @returns Client if connection successful
   * @throws Error if connection fails
   */
  async buildAndTest(): Promise<AdGuardDnsClient> {
    const client = this.build();
    const connected = await client.testConnection();
    if (!connected) {
      throw new Error('Failed to connect to AdGuard DNS API');
    }
    return client;
  }
}

/**
 * Create a client builder
 */
export function createClientBuilder(): AdGuardDnsClientBuilder {
  return new AdGuardDnsClientBuilder();
}

// Re-export useful types and constants
export type { Logger };
export { consoleLogger, DEFAULT_TIMEOUT, MAX_TIMEOUT, MIN_TIMEOUT, silentLogger };
export type { RetryOptions };
