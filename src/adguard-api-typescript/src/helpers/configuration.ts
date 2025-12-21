/**
 * Configuration helper for AdGuard DNS API client
 * Matches .NET ConfigurationHelper functionality
 */

import axios, { AxiosInstance, AxiosRequestConfig, InternalAxiosRequestConfig } from 'axios';
import { RetryOptions, executeWithRetry } from './retry';

/** API base path */
export const DEFAULT_BASE_PATH = 'https://api.adguard-dns.io';

/** Default timeout in milliseconds */
export const DEFAULT_TIMEOUT = 30000;

/** Minimum timeout in milliseconds */
export const MIN_TIMEOUT = 1000;

/** Maximum timeout in milliseconds */
export const MAX_TIMEOUT = 300000;

/** Authentication type */
export type AuthType = 'api-key' | 'bearer';

/** Logger interface */
export interface Logger {
  debug(message: string, ...args: unknown[]): void;
  info(message: string, ...args: unknown[]): void;
  warn(message: string, ...args: unknown[]): void;
  error(message: string, ...args: unknown[]): void;
}

/** Console logger implementation */
export const consoleLogger: Logger = {
  debug: (message, ...args) => console.debug(`[DEBUG] ${message}`, ...args),
  info: (message, ...args) => console.info(`[INFO] ${message}`, ...args),
  warn: (message, ...args) => console.warn(`[WARN] ${message}`, ...args),
  error: (message, ...args) => console.error(`[ERROR] ${message}`, ...args),
};

/** Silent logger implementation */
export const silentLogger: Logger = {
  debug: () => {},
  info: () => {},
  warn: () => {},
  error: () => {},
};

/** API client configuration */
export interface ApiConfiguration {
  /** Base path URL */
  basePath: string;
  /** Request timeout in milliseconds */
  timeout: number;
  /** User agent string */
  userAgent: string;
  /** Authentication type */
  authType?: AuthType;
  /** API key (if using API key auth) */
  apiKey?: string;
  /** Bearer token (if using bearer auth) */
  accessToken?: string;
  /** Retry options */
  retryOptions?: RetryOptions;
  /** Logger instance */
  logger?: Logger;
}

/** Default configuration */
export const DEFAULT_CONFIG: ApiConfiguration = {
  basePath: DEFAULT_BASE_PATH,
  timeout: DEFAULT_TIMEOUT,
  userAgent: 'adguard-api-typescript/1.0.0',
};

/** Create an Axios instance with the given configuration */
export function createAxiosInstance(config: ApiConfiguration): AxiosInstance {
  const logger = config.logger ?? silentLogger;

  const axiosConfig: AxiosRequestConfig = {
    baseURL: config.basePath,
    timeout: config.timeout,
    headers: {
      'Content-Type': 'application/json',
      'User-Agent': config.userAgent,
    },
  };

  const instance = axios.create(axiosConfig);

  // Add authentication interceptor
  instance.interceptors.request.use((request: InternalAxiosRequestConfig) => {
    if (config.authType === 'api-key' && config.apiKey) {
      request.headers.set('Authorization', `ApiKey ${config.apiKey}`);
    } else if (config.authType === 'bearer' && config.accessToken) {
      request.headers.set('Authorization', `Bearer ${config.accessToken}`);
    }

    logger.debug(`Request: ${request.method?.toUpperCase()} ${request.url}`);
    return request;
  });

  // Add response logging interceptor
  instance.interceptors.response.use(
    response => {
      logger.debug(`Response: ${response.status} ${response.statusText}`);
      return response;
    },
    error => {
      if (axios.isAxiosError(error)) {
        logger.error(`Request failed: ${error.response?.status} ${error.message}`);
      }
      return Promise.reject(error);
    }
  );

  return instance;
}

/** Configuration builder for fluent API */
export class ConfigurationBuilder {
  private config: ApiConfiguration = { ...DEFAULT_CONFIG };

  /** Set API key authentication */
  withApiKey(apiKey: string): ConfigurationBuilder {
    if (!apiKey || apiKey.trim().length === 0) {
      throw new Error('API key cannot be empty');
    }
    this.config.authType = 'api-key';
    this.config.apiKey = apiKey.trim();
    return this;
  }

  /** Set bearer token authentication */
  withBearerToken(accessToken: string): ConfigurationBuilder {
    if (!accessToken || accessToken.trim().length === 0) {
      throw new Error('Access token cannot be empty');
    }
    this.config.authType = 'bearer';
    this.config.accessToken = accessToken.trim();
    return this;
  }

  /** Set base path */
  withBasePath(basePath: string): ConfigurationBuilder {
    if (!basePath || basePath.trim().length === 0) {
      throw new Error('Base path cannot be empty');
    }
    this.config.basePath = basePath.trim();
    return this;
  }

  /** Set request timeout */
  withTimeout(timeoutMs: number): ConfigurationBuilder {
    if (timeoutMs < MIN_TIMEOUT || timeoutMs > MAX_TIMEOUT) {
      throw new Error(`Timeout must be between ${MIN_TIMEOUT} and ${MAX_TIMEOUT} milliseconds`);
    }
    this.config.timeout = timeoutMs;
    return this;
  }

  /** Set user agent */
  withUserAgent(userAgent: string): ConfigurationBuilder {
    if (!userAgent || userAgent.trim().length === 0) {
      throw new Error('User agent cannot be empty');
    }
    this.config.userAgent = userAgent.trim();
    return this;
  }

  /** Set retry options */
  withRetryOptions(options: RetryOptions): ConfigurationBuilder {
    this.config.retryOptions = options;
    return this;
  }

  /** Set logger */
  withLogger(logger: Logger): ConfigurationBuilder {
    this.config.logger = logger;
    return this;
  }

  /** Enable console logging */
  withConsoleLogging(): ConfigurationBuilder {
    this.config.logger = consoleLogger;
    return this;
  }

  /** Build the configuration */
  build(): ApiConfiguration {
    return { ...this.config };
  }

  /** Build and create an Axios instance */
  buildClient(): AxiosInstance {
    return createAxiosInstance(this.build());
  }
}

/** Create a configuration with API key authentication */
export function createWithApiKey(
  apiKey: string,
  basePath: string = DEFAULT_BASE_PATH,
  logger?: Logger
): ApiConfiguration {
  if (!apiKey || apiKey.trim().length === 0) {
    throw new Error('API key cannot be empty');
  }

  return {
    ...DEFAULT_CONFIG,
    basePath,
    authType: 'api-key',
    apiKey: apiKey.trim(),
    logger,
  };
}

/** Create a configuration with bearer token authentication */
export function createWithBearerToken(
  accessToken: string,
  basePath: string = DEFAULT_BASE_PATH,
  logger?: Logger
): ApiConfiguration {
  if (!accessToken || accessToken.trim().length === 0) {
    throw new Error('Access token cannot be empty');
  }

  return {
    ...DEFAULT_CONFIG,
    basePath,
    authType: 'bearer',
    accessToken: accessToken.trim(),
    logger,
  };
}

/** Create a custom configuration */
export function createCustom(
  basePath?: string,
  timeout?: number,
  userAgent?: string,
  logger?: Logger
): ApiConfiguration {
  const config: ApiConfiguration = { ...DEFAULT_CONFIG };

  if (basePath) {
    config.basePath = basePath;
  }
  if (timeout !== undefined) {
    if (timeout < MIN_TIMEOUT || timeout > MAX_TIMEOUT) {
      throw new Error(`Timeout must be between ${MIN_TIMEOUT} and ${MAX_TIMEOUT} milliseconds`);
    }
    config.timeout = timeout;
  }
  if (userAgent) {
    config.userAgent = userAgent;
  }
  if (logger) {
    config.logger = logger;
  }

  return config;
}

/** Validate that configuration has authentication */
export function validateAuthentication(config: ApiConfiguration): boolean {
  if (config.authType === 'api-key') {
    return !!config.apiKey && config.apiKey.trim().length > 0;
  }
  if (config.authType === 'bearer') {
    return !!config.accessToken && config.accessToken.trim().length > 0;
  }
  return false;
}

/** Mask an API key for display (show first 4 and last 4 chars) */
export function maskApiKey(apiKey: string): string {
  if (apiKey.length <= 8) {
    return '********';
  }
  return `${apiKey.slice(0, 4)}...${apiKey.slice(-4)}`;
}

/** Create a request wrapper with retry logic */
export function createRetryableClient(
  config: ApiConfiguration
): <T>(request: () => Promise<T>) => Promise<T> {
  const retryOptions = config.retryOptions ?? {};

  return <T>(request: () => Promise<T>) => {
    return executeWithRetry(request, {
      ...retryOptions,
      onRetry: (error, attempt, delay) => {
        config.logger?.warn(
          `Request failed, retrying (attempt ${attempt}): ${error.message}. Waiting ${delay}ms...`
        );
        retryOptions.onRetry?.(error, attempt, delay);
      },
    });
  };
}

/** Configuration helper object */
export const ConfigurationHelper = {
  createWithApiKey,
  createWithBearerToken,
  createCustom,
  validateAuthentication,
  maskApiKey,
  createAxiosInstance,
  createRetryableClient,
  ConfigurationBuilder,
  DEFAULT_BASE_PATH,
  DEFAULT_TIMEOUT,
  MIN_TIMEOUT,
  MAX_TIMEOUT,
  consoleLogger,
  silentLogger,
};

export default ConfigurationHelper;
