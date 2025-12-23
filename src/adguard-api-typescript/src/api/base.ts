/**
 * Base API client class
 */

import { AxiosInstance, AxiosError } from 'axios';
import {
  ApiConfiguration,
  createAxiosInstance,
  createRetryableClient,
  Logger,
  silentLogger,
} from '../helpers/configuration.js';
import {
  ApiError,
  ValidationError,
  EntityNotFoundError,
  RateLimitError,
  AuthenticationError,
} from '../errors/index.js';
import { ErrorResponse } from '../models/index.js';

/** Base API client */
export abstract class BaseApi {
  protected readonly client: AxiosInstance;
  protected readonly config: ApiConfiguration;
  protected readonly logger: Logger;
  protected readonly executeWithRetry: <T>(request: () => Promise<T>) => Promise<T>;

  constructor(config: ApiConfiguration) {
    this.config = config;
    this.logger = config.logger ?? silentLogger;
    this.client = createAxiosInstance(config);
    this.executeWithRetry = createRetryableClient(config);
  }

  /** Handle API errors and convert to appropriate error types */
  protected handleError(error: unknown, entityType?: string, entityId?: string): never {
    if (error instanceof AxiosError) {
      const status = error.response?.status ?? 0;
      const data = error.response?.data as ErrorResponse | undefined;
      const message = data?.message ?? error.message;

      switch (status) {
        case 400:
          throw new ValidationError(message, data);
        case 401:
          throw new AuthenticationError(message);
        case 404:
          throw new EntityNotFoundError(entityType ?? 'Resource', entityId);
        case 429:
          const retryAfter = error.response?.headers?.['retry-after'];
          throw new RateLimitError(message, retryAfter ? parseInt(retryAfter, 10) : undefined);
        default:
          throw new ApiError(message, status, data);
      }
    }

    if (error instanceof Error) {
      throw new ApiError(error.message, 0);
    }

    throw new ApiError(String(error), 0);
  }

  /** Execute a GET request */
  protected async get<T>(path: string, params?: Record<string, unknown>): Promise<T> {
    try {
      const response = await this.executeWithRetry(() =>
        this.client.get<T>(path, { params })
      );
      return response.data;
    } catch (error) {
      this.handleError(error);
    }
  }

  /** Execute a POST request */
  protected async post<T>(path: string, data?: unknown): Promise<T> {
    try {
      const response = await this.executeWithRetry(() =>
        this.client.post<T>(path, data)
      );
      return response.data;
    } catch (error) {
      this.handleError(error);
    }
  }

  /** Execute a PUT request */
  protected async put<T>(path: string, data?: unknown): Promise<T> {
    try {
      const response = await this.executeWithRetry(() =>
        this.client.put<T>(path, data)
      );
      return response.data;
    } catch (error) {
      this.handleError(error);
    }
  }

  /** Execute a DELETE request */
  protected async delete<T = void>(path: string): Promise<T> {
    try {
      const response = await this.executeWithRetry(() =>
        this.client.delete<T>(path)
      );
      return response.data;
    } catch (error) {
      this.handleError(error);
    }
  }

  /** Execute a PATCH request */
  protected async patch<T>(path: string, data?: unknown): Promise<T> {
    try {
      const response = await this.executeWithRetry(() =>
        this.client.patch<T>(path, data)
      );
      return response.data;
    } catch (error) {
      this.handleError(error);
    }
  }
}
