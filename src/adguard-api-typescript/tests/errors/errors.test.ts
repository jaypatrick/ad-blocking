/**
 * Error classes tests
 * Deno-native testing implementation
 */

import { assertEquals, assertInstanceOf } from '@std/assert';
import {
  ApiError,
  EntityNotFoundError,
  ValidationError,
  RateLimitError,
  AuthenticationError,
  ApiNotConfiguredError,
  RepositoryError,
} from '../../src/errors/index.ts';
import { ErrorCodes } from '../../src/models/index.ts';

// ApiError tests
Deno.test('ApiError - creates with message and status code', () => {
  const error = new ApiError('Test error', 400);
  assertEquals(error.message, 'Test error');
  assertEquals(error.statusCode, 400);
  assertEquals(error.name, 'ApiError');
});

Deno.test('ApiError - includes response', () => {
  const response = {
    error_code: ErrorCodes.BAD_REQUEST,
    message: 'Bad request',
    fields: [],
  };
  const error = new ApiError('Test error', 400, response);
  assertEquals(error.response, response);
  assertEquals(error.errorCode, ErrorCodes.BAD_REQUEST);
  assertEquals(error.fieldErrors, []);
});

Deno.test('ApiError - is instanceof Error', () => {
  const error = new ApiError('Test', 400);
  assertInstanceOf(error, Error);
  assertInstanceOf(error, ApiError);
});

// EntityNotFoundError tests
Deno.test('EntityNotFoundError - creates with entity type', () => {
  const error = new EntityNotFoundError('Device');
  assertEquals(error.message, 'Device not found');
  assertEquals(error.statusCode, 404);
  assertEquals(error.entityType, 'Device');
  assertEquals(error.entityId, undefined);
});

Deno.test('EntityNotFoundError - creates with entity type and ID', () => {
  const error = new EntityNotFoundError('Device', 'abc123');
  assertEquals(error.message, "Device with ID 'abc123' not found");
  assertEquals(error.entityId, 'abc123');
});

Deno.test('EntityNotFoundError - is instanceof ApiError', () => {
  const error = new EntityNotFoundError('Device');
  assertInstanceOf(error, ApiError);
  assertInstanceOf(error, EntityNotFoundError);
});

// ValidationError tests
Deno.test('ValidationError - creates with message', () => {
  const error = new ValidationError('Invalid input');
  assertEquals(error.message, 'Invalid input');
  assertEquals(error.statusCode, 400);
  assertEquals(error.name, 'ValidationError');
});

Deno.test('ValidationError - includes response', () => {
  const response = {
    error_code: ErrorCodes.FIELD_REQUIRED,
    message: 'Name is required',
    fields: [{ field: 'name', error_code: ErrorCodes.FIELD_REQUIRED }],
  };
  const error = new ValidationError('Validation failed', response);
  assertEquals(error.fieldErrors.length, 1);
});

// RateLimitError tests
Deno.test('RateLimitError - creates with message', () => {
  const error = new RateLimitError('Too many requests');
  assertEquals(error.message, 'Too many requests');
  assertEquals(error.statusCode, 429);
  assertEquals(error.retryAfter, undefined);
});

Deno.test('RateLimitError - includes retry after', () => {
  const error = new RateLimitError('Too many requests', 60);
  assertEquals(error.retryAfter, 60);
});

// AuthenticationError tests
Deno.test('AuthenticationError - creates with default message', () => {
  const error = new AuthenticationError();
  assertEquals(error.message, 'Authentication failed');
  assertEquals(error.statusCode, 401);
});

Deno.test('AuthenticationError - creates with custom message', () => {
  const error = new AuthenticationError('Invalid API key');
  assertEquals(error.message, 'Invalid API key');
});

// ApiNotConfiguredError tests
Deno.test('ApiNotConfiguredError - creates with default message', () => {
  const error = new ApiNotConfiguredError();
  assertEquals(error.message, 'API client is not configured. Call configure() first.');
  assertEquals(error.name, 'ApiNotConfiguredError');
});

Deno.test('ApiNotConfiguredError - creates with custom message', () => {
  const error = new ApiNotConfiguredError('Custom message');
  assertEquals(error.message, 'Custom message');
});

// RepositoryError tests
Deno.test('RepositoryError - creates with operation', () => {
  const error = new RepositoryError('Get device');
  assertEquals(error.message, 'Get device failed');
  assertEquals(error.operation, 'Get device');
  assertEquals(error.innerCause, undefined);
});

Deno.test('RepositoryError - includes cause', () => {
  const cause = new Error('Network error');
  const error = new RepositoryError('Get device', cause);
  assertEquals(error.message, 'Get device failed: Network error');
  assertEquals(error.innerCause, cause);
});
