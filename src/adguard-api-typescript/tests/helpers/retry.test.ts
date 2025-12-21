/**
 * Retry policy helper tests
 */

/// <reference types="jest" />

import { AxiosError } from 'axios';
import {
  executeWithRetry,
  withRetry,
  isRetryableError,
  calculateDelay,
  createRateLimitRetryPolicy,
  DEFAULT_RETRY_OPTIONS,
} from '../../src/helpers/retry';

describe('RetryPolicy', () => {
  describe('isRetryableError', () => {
    it('should return true for 408 status', () => {
      const error = new AxiosError('Timeout');
      error.response = { status: 408 } as any;
      expect(isRetryableError(error)).toBe(true);
    });

    it('should return true for 429 status', () => {
      const error = new AxiosError('Too Many Requests');
      error.response = { status: 429 } as any;
      expect(isRetryableError(error)).toBe(true);
    });

    it('should return true for 5xx status', () => {
      for (const status of [500, 502, 503, 504]) {
        const error = new AxiosError('Server Error');
        error.response = { status } as any;
        expect(isRetryableError(error)).toBe(true);
      }
    });

    it('should return true for network errors', () => {
      const error = new AxiosError('Network Error');
      error.code = 'ECONNRESET';
      expect(isRetryableError(error)).toBe(true);
    });

    it('should return false for 400 status', () => {
      const error = new AxiosError('Bad Request');
      error.response = { status: 400 } as any;
      expect(isRetryableError(error)).toBe(false);
    });

    it('should return false for 401 status', () => {
      const error = new AxiosError('Unauthorized');
      error.response = { status: 401 } as any;
      expect(isRetryableError(error)).toBe(false);
    });

    it('should return false for non-Axios errors', () => {
      const error = new Error('Generic error');
      expect(isRetryableError(error)).toBe(false);
    });
  });

  describe('calculateDelay', () => {
    const baseOptions = {
      maxRetries: 3,
      initialDelayMs: 1000,
      maxDelayMs: 30000,
      backoff: 'exponential' as const,
    };

    it('should calculate exponential delay', () => {
      // First retry: 1000ms
      const delay1 = calculateDelay(1, baseOptions);
      expect(delay1).toBeGreaterThanOrEqual(1000);
      expect(delay1).toBeLessThan(1200); // Allow for jitter

      // Second retry: 2000ms
      const delay2 = calculateDelay(2, baseOptions);
      expect(delay2).toBeGreaterThanOrEqual(2000);
      expect(delay2).toBeLessThan(2400);

      // Third retry: 4000ms
      const delay3 = calculateDelay(3, baseOptions);
      expect(delay3).toBeGreaterThanOrEqual(4000);
      expect(delay3).toBeLessThan(4800);
    });

    it('should calculate linear delay', () => {
      const linearOptions = { ...baseOptions, backoff: 'linear' as const };

      const delay1 = calculateDelay(1, linearOptions);
      expect(delay1).toBeGreaterThanOrEqual(1000);
      expect(delay1).toBeLessThan(1200);

      const delay2 = calculateDelay(2, linearOptions);
      expect(delay2).toBeGreaterThanOrEqual(2000);
      expect(delay2).toBeLessThan(2400);
    });

    it('should cap at max delay', () => {
      const delay = calculateDelay(10, baseOptions);
      expect(delay).toBeLessThanOrEqual(baseOptions.maxDelayMs);
    });
  });

  describe('executeWithRetry', () => {
    it('should return result on success', async () => {
      const result = await executeWithRetry(() => Promise.resolve('success'));
      expect(result).toBe('success');
    });

    it('should retry on retryable error', async () => {
      let attempts = 0;
      const operation = jest.fn().mockImplementation(() => {
        attempts++;
        if (attempts < 3) {
          const error = new AxiosError('Server Error');
          error.response = { status: 500 } as any;
          throw error;
        }
        return Promise.resolve('success');
      });

      const result = await executeWithRetry(operation, {
        maxRetries: 3,
        initialDelayMs: 10,
      });

      expect(result).toBe('success');
      expect(operation).toHaveBeenCalledTimes(3);
    });

    it('should throw after max retries', async () => {
      const error = new AxiosError('Server Error');
      error.response = { status: 500 } as any;

      await expect(
        executeWithRetry(() => Promise.reject(error), {
          maxRetries: 2,
          initialDelayMs: 10,
        })
      ).rejects.toThrow();
    });

    it('should not retry non-retryable errors', async () => {
      const error = new AxiosError('Bad Request');
      error.response = { status: 400 } as any;
      const operation = jest.fn().mockRejectedValue(error);

      await expect(
        executeWithRetry(operation, { maxRetries: 3, initialDelayMs: 10 })
      ).rejects.toThrow();

      expect(operation).toHaveBeenCalledTimes(1);
    });

    it('should call onRetry callback', async () => {
      const error = new AxiosError('Server Error');
      error.response = { status: 500 } as any;

      let attempts = 0;
      const operation = jest.fn().mockImplementation(() => {
        attempts++;
        if (attempts < 2) throw error;
        return Promise.resolve('success');
      });

      const onRetry = jest.fn();

      await executeWithRetry(operation, {
        maxRetries: 3,
        initialDelayMs: 10,
        onRetry,
      });

      expect(onRetry).toHaveBeenCalledTimes(1);
      expect(onRetry).toHaveBeenCalledWith(error, 1, expect.any(Number));
    });

    it('should use custom shouldRetry function', async () => {
      const error = new Error('Custom error');
      let attempts = 0;

      const operation = jest.fn().mockImplementation(() => {
        attempts++;
        if (attempts < 2) throw error;
        return Promise.resolve('success');
      });

      const result = await executeWithRetry(operation, {
        maxRetries: 3,
        initialDelayMs: 10,
        shouldRetry: () => true,
      });

      expect(result).toBe('success');
    });
  });

  describe('withRetry', () => {
    it('should create a retry-wrapped function', async () => {
      const fn = jest.fn().mockResolvedValue('result');
      const wrapped = withRetry(fn);

      const result = await wrapped('arg1', 'arg2');

      expect(result).toBe('result');
      expect(fn).toHaveBeenCalledWith('arg1', 'arg2');
    });
  });

  describe('createRateLimitRetryPolicy', () => {
    it('should create rate limit policy', () => {
      const policy = createRateLimitRetryPolicy(5, 10000);
      expect(policy.maxRetries).toBe(5);
      expect(policy.initialDelayMs).toBe(10000);
      expect(policy.backoff).toBe('linear');
    });

    it('should only retry 429 errors', () => {
      const policy = createRateLimitRetryPolicy();

      const error429 = new AxiosError('Too Many Requests');
      error429.response = { status: 429 } as any;
      expect(policy.shouldRetry!(error429, 1)).toBe(true);

      const error500 = new AxiosError('Server Error');
      error500.response = { status: 500 } as any;
      expect(policy.shouldRetry!(error500, 1)).toBe(false);
    });
  });

  describe('DEFAULT_RETRY_OPTIONS', () => {
    it('should have correct defaults', () => {
      expect(DEFAULT_RETRY_OPTIONS.maxRetries).toBe(3);
      expect(DEFAULT_RETRY_OPTIONS.initialDelayMs).toBe(2000);
      expect(DEFAULT_RETRY_OPTIONS.maxDelayMs).toBe(30000);
      expect(DEFAULT_RETRY_OPTIONS.backoff).toBe('exponential');
    });
  });
});
