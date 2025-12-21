/**
 * Error classes tests
 */

import {
  ApiError,
  EntityNotFoundError,
  ValidationError,
  RateLimitError,
  AuthenticationError,
  ApiNotConfiguredError,
  RepositoryError,
} from '../../src/errors';
import { ErrorCodes } from '../../src/models';

describe('Error classes', () => {
  describe('ApiError', () => {
    it('should create with message and status code', () => {
      const error = new ApiError('Test error', 400);
      expect(error.message).toBe('Test error');
      expect(error.statusCode).toBe(400);
      expect(error.name).toBe('ApiError');
    });

    it('should include response', () => {
      const response = {
        error_code: ErrorCodes.BAD_REQUEST,
        message: 'Bad request',
        fields: [],
      };
      const error = new ApiError('Test error', 400, response);
      expect(error.response).toEqual(response);
      expect(error.errorCode).toBe(ErrorCodes.BAD_REQUEST);
      expect(error.fieldErrors).toEqual([]);
    });

    it('should be instanceof Error', () => {
      const error = new ApiError('Test', 400);
      expect(error instanceof Error).toBe(true);
      expect(error instanceof ApiError).toBe(true);
    });
  });

  describe('EntityNotFoundError', () => {
    it('should create with entity type', () => {
      const error = new EntityNotFoundError('Device');
      expect(error.message).toBe('Device not found');
      expect(error.statusCode).toBe(404);
      expect(error.entityType).toBe('Device');
      expect(error.entityId).toBeUndefined();
    });

    it('should create with entity type and ID', () => {
      const error = new EntityNotFoundError('Device', 'abc123');
      expect(error.message).toBe("Device with ID 'abc123' not found");
      expect(error.entityId).toBe('abc123');
    });

    it('should be instanceof ApiError', () => {
      const error = new EntityNotFoundError('Device');
      expect(error instanceof ApiError).toBe(true);
      expect(error instanceof EntityNotFoundError).toBe(true);
    });
  });

  describe('ValidationError', () => {
    it('should create with message', () => {
      const error = new ValidationError('Invalid input');
      expect(error.message).toBe('Invalid input');
      expect(error.statusCode).toBe(400);
      expect(error.name).toBe('ValidationError');
    });

    it('should include response', () => {
      const response = {
        error_code: ErrorCodes.FIELD_REQUIRED,
        message: 'Name is required',
        fields: [{ field: 'name', error_code: ErrorCodes.FIELD_REQUIRED }],
      };
      const error = new ValidationError('Validation failed', response);
      expect(error.fieldErrors).toHaveLength(1);
    });
  });

  describe('RateLimitError', () => {
    it('should create with message', () => {
      const error = new RateLimitError('Too many requests');
      expect(error.message).toBe('Too many requests');
      expect(error.statusCode).toBe(429);
      expect(error.retryAfter).toBeUndefined();
    });

    it('should include retry after', () => {
      const error = new RateLimitError('Too many requests', 60);
      expect(error.retryAfter).toBe(60);
    });
  });

  describe('AuthenticationError', () => {
    it('should create with default message', () => {
      const error = new AuthenticationError();
      expect(error.message).toBe('Authentication failed');
      expect(error.statusCode).toBe(401);
    });

    it('should create with custom message', () => {
      const error = new AuthenticationError('Invalid API key');
      expect(error.message).toBe('Invalid API key');
    });
  });

  describe('ApiNotConfiguredError', () => {
    it('should create with default message', () => {
      const error = new ApiNotConfiguredError();
      expect(error.message).toBe('API client is not configured. Call configure() first.');
      expect(error.name).toBe('ApiNotConfiguredError');
    });

    it('should create with custom message', () => {
      const error = new ApiNotConfiguredError('Custom message');
      expect(error.message).toBe('Custom message');
    });
  });

  describe('RepositoryError', () => {
    it('should create with operation', () => {
      const error = new RepositoryError('Get device');
      expect(error.message).toBe('Get device failed');
      expect(error.operation).toBe('Get device');
      expect(error.innerCause).toBeUndefined();
    });

    it('should include cause', () => {
      const cause = new Error('Network error');
      const error = new RepositoryError('Get device', cause);
      expect(error.message).toBe('Get device failed: Network error');
      expect(error.innerCause).toBe(cause);
    });
  });
});
