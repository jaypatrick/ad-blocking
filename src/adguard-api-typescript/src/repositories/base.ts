/**
 * Base repository class with common error handling
 */

import { Logger, silentLogger } from '../helpers/configuration.ts';
import { ApiError, EntityNotFoundError, RepositoryError } from '../errors/index.ts';

/** Repository operation callback */
export type RepositoryOperation<T> = () => Promise<T>;

/** Error callback for logging */
export type ErrorCallback = (error: Error, operation: string) => void;

/** Base repository class */
export abstract class BaseRepository {
  protected readonly logger: Logger;

  constructor(logger?: Logger) {
    this.logger = logger ?? silentLogger;
  }

  /**
   * Execute an operation with standard error handling
   * @param operation - Operation name for logging
   * @param action - Async action to execute
   * @param onError - Optional error callback
   * @returns Result of the operation
   */
  protected async execute<T>(
    operation: string,
    action: RepositoryOperation<T>,
    onError?: ErrorCallback,
  ): Promise<T> {
    try {
      this.logger.debug(`Executing: ${operation}`);
      const result = await action();
      this.logger.debug(`Completed: ${operation}`);
      return result;
    } catch (error) {
      const err = error instanceof Error ? error : new Error(String(error));

      if (onError) {
        onError(err, operation);
      }

      this.logger.error(`Failed: ${operation} - ${err.message}`);

      if (error instanceof ApiError) {
        throw new RepositoryError(operation, error);
      }

      throw new RepositoryError(operation, err);
    }
  }

  /**
   * Execute an operation that may return 404 (entity not found)
   * @param operation - Operation name for logging
   * @param action - Async action to execute
   * @param entityType - Entity type for error messages
   * @param entityId - Entity ID for error messages
   * @param onError - Optional error callback
   * @returns Result of the operation
   */
  protected async executeWithEntityCheck<T>(
    operation: string,
    action: RepositoryOperation<T>,
    entityType: string,
    entityId?: string,
    onError?: ErrorCallback,
  ): Promise<T> {
    try {
      this.logger.debug(`Executing: ${operation}`);
      const result = await action();
      this.logger.debug(`Completed: ${operation}`);
      return result;
    } catch (error) {
      const err = error instanceof Error ? error : new Error(String(error));

      if (onError) {
        onError(err, operation);
      }

      this.logger.error(`Failed: ${operation} - ${err.message}`);

      if (error instanceof EntityNotFoundError) {
        throw error;
      }

      if (error instanceof ApiError && error.statusCode === 404) {
        throw new EntityNotFoundError(entityType, entityId);
      }

      if (error instanceof ApiError) {
        throw new RepositoryError(operation, error);
      }

      throw new RepositoryError(operation, err);
    }
  }
}
