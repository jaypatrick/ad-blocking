/**
 * Error classes tests
 */

import { assertEquals, assert } from '@std/assert';
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

Deno.test('Error classes', async (t) => {
  await t.step('ApiError', async (t) => {
    await t.step('should create with message and status code', () => {
      const error = new ApiError('Test error', 400);
      assertEquals(error.message, 'Test error');
      assertEquals(error.statusCode, 400);
      assertEquals(error.name, 'ApiError');
    });

    await t.step('should include response', () => {
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

    await t.step('should be instanceof Error', () => {
      const error = new ApiError('Test', 400);
      assert(error instanceof Error);
      assert(error instanceof ApiError);
    });
  });

  await t.step('EntityNotFoundError', async (t) => {
    await t.step('should create with entity type', () => {
      const error = new EntityNotFoundError('Device');
      assertEquals(error.message, 'Device not found');
      assertEquals(error.statusCode, 404);
      assertEquals(error.entityType, 'Device');
      assertEquals(error.entityId, undefined);
    });

    await t.step('should create with entity type and ID', () => {
      const error = new EntityNotFoundError('Device', 'abc123');
      assertEquals(error.message, "Device with ID 'abc123' not found");
      assertEquals(error.entityId, 'abc123');
    });

    await t.step('should be instanceof ApiError', () => {
      const error = new EntityNotFoundError('Device');
      assert(error instanceof ApiError);
      assert(error instanceof EntityNotFoundError);
    });
  });

  await t.step('ValidationError', async (t) => {
    await t.step('should create with message', () => {
      const error = new ValidationError('Invalid input');
      assertEquals(error.message, 'Invalid input');
      assertEquals(error.statusCode, 400);
      assertEquals(error.name, 'ValidationError');
    });

    await t.step('should include response', () => {
      const response = {
        error_code: ErrorCodes.FIELD_REQUIRED,
        message: 'Name is required',
        fields: [{ field: 'name', error_code: ErrorCodes.FIELD_REQUIRED }],
      };
      const error = new ValidationError('Validation failed', response);
      assertEquals(error.fieldErrors.length, 1);
    });
  });

  await t.step('RateLimitError', async (t) => {
    await t.step('should create with message', () => {
      const error = new RateLimitError('Too many requests');
      assertEquals(error.message, 'Too many requests');
      assertEquals(error.statusCode, 429);
      assertEquals(error.retryAfter, undefined);
    });

    await t.step('should include retry after', () => {
      const error = new RateLimitError('Too many requests', 60);
      assertEquals(error.retryAfter, 60);
    });
  });

  await t.step('AuthenticationError', async (t) => {
    await t.step('should create with default message', () => {
      const error = new AuthenticationError();
      assertEquals(error.message, 'Authentication failed');
      assertEquals(error.statusCode, 401);
    });

    await t.step('should create with custom message', () => {
      const error = new AuthenticationError('Invalid API key');
      assertEquals(error.message, 'Invalid API key');
    });
  });

  await t.step('ApiNotConfiguredError', async (t) => {
    await t.step('should create with default message', () => {
      const error = new ApiNotConfiguredError();
      assertEquals(error.message, 'API client is not configured. Call configure() first.');
      assertEquals(error.name, 'ApiNotConfiguredError');
    });

    await t.step('should create with custom message', () => {
      const error = new ApiNotConfiguredError('Custom message');
      assertEquals(error.message, 'Custom message');
    });
  });

  await t.step('RepositoryError', async (t) => {
    await t.step('should create with operation', () => {
      const error = new RepositoryError('Get device');
      assertEquals(error.message, 'Get device failed');
      assertEquals(error.operation, 'Get device');
      assertEquals(error.innerCause, undefined);
    });

    await t.step('should include cause', () => {
      const cause = new Error('Network error');
      const error = new RepositoryError('Get device', cause);
      assertEquals(error.message, 'Get device failed: Network error');
      assertEquals(error.innerCause, cause);
    });
  });
});
