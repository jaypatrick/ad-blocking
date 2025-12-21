/**
 * Custom error classes for AdGuard DNS API
 */

import { ErrorResponse, FieldError, ErrorCodes } from '../models';

/** Base API error */
export class ApiError extends Error {
  readonly statusCode: number;
  readonly response?: ErrorResponse;

  constructor(message: string, statusCode: number, response?: ErrorResponse) {
    super(message);
    this.name = 'ApiError';
    this.statusCode = statusCode;
    this.response = response;
    Object.setPrototypeOf(this, ApiError.prototype);
  }

  /** Get field errors if available */
  get fieldErrors(): FieldError[] {
    return this.response?.fields ?? [];
  }

  /** Get error code */
  get errorCode(): ErrorCodes | undefined {
    return this.response?.error_code;
  }
}

/** Entity not found error (404) */
export class EntityNotFoundError extends ApiError {
  readonly entityType: string;
  readonly entityId?: string;

  constructor(entityType: string, entityId?: string) {
    const message = entityId
      ? `${entityType} with ID '${entityId}' not found`
      : `${entityType} not found`;
    super(message, 404);
    this.name = 'EntityNotFoundError';
    this.entityType = entityType;
    this.entityId = entityId;
    Object.setPrototypeOf(this, EntityNotFoundError.prototype);
  }
}

/** Validation error (400) */
export class ValidationError extends ApiError {
  constructor(message: string, response?: ErrorResponse) {
    super(message, 400, response);
    this.name = 'ValidationError';
    Object.setPrototypeOf(this, ValidationError.prototype);
  }
}

/** Rate limit error (429) */
export class RateLimitError extends ApiError {
  readonly retryAfter?: number;

  constructor(message: string, retryAfter?: number) {
    super(message, 429);
    this.name = 'RateLimitError';
    this.retryAfter = retryAfter;
    Object.setPrototypeOf(this, RateLimitError.prototype);
  }
}

/** Authentication error (401) */
export class AuthenticationError extends ApiError {
  constructor(message: string = 'Authentication failed') {
    super(message, 401);
    this.name = 'AuthenticationError';
    Object.setPrototypeOf(this, AuthenticationError.prototype);
  }
}

/** API not configured error */
export class ApiNotConfiguredError extends Error {
  constructor(message: string = 'API client is not configured. Call configure() first.') {
    super(message);
    this.name = 'ApiNotConfiguredError';
    Object.setPrototypeOf(this, ApiNotConfiguredError.prototype);
  }
}

/** Repository error (wraps API errors) */
export class RepositoryError extends Error {
  readonly operation: string;
  readonly innerCause?: Error;

  constructor(operation: string, innerCause?: Error) {
    const message = innerCause
      ? `${operation} failed: ${innerCause.message}`
      : `${operation} failed`;
    super(message);
    this.name = 'RepositoryError';
    this.operation = operation;
    this.innerCause = innerCause;
    Object.setPrototypeOf(this, RepositoryError.prototype);
  }
}
