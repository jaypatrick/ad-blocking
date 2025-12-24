/**
 * Retry policy helper tests
 * Deno-native testing implementation
 */

import {
  assertEquals,
  assertRejects,
  assertGreaterOrEqual,
  assertLess,
  assertLessOrEqual,
} from '@std/assert';
import { spy } from '@std/testing/mock';
import { AxiosError } from 'axios';
import {
  executeWithRetry,
  withRetry,
  isRetryableError,
  calculateDelay,
  createRateLimitRetryPolicy,
  DEFAULT_RETRY_OPTIONS,
} from '../../src/helpers/retry.ts';

// isRetryableError tests
Deno.test('isRetryableError - returns true for 408 status', () => {
  const error = new AxiosError('Timeout');
  error.response = { status: 408 } as AxiosError['response'];
  assertEquals(isRetryableError(error), true);
});

Deno.test('isRetryableError - returns true for 429 status', () => {
  const error = new AxiosError('Too Many Requests');
  error.response = { status: 429 } as AxiosError['response'];
  assertEquals(isRetryableError(error), true);
});

Deno.test('isRetryableError - returns true for 5xx status', () => {
  for (const status of [500, 502, 503, 504]) {
    const error = new AxiosError('Server Error');
    error.response = { status } as AxiosError['response'];
    assertEquals(isRetryableError(error), true);
  }
});

Deno.test('isRetryableError - returns true for network errors', () => {
  const error = new AxiosError('Network Error');
  error.code = 'ECONNRESET';
  assertEquals(isRetryableError(error), true);
});

Deno.test('isRetryableError - returns false for 400 status', () => {
  const error = new AxiosError('Bad Request');
  error.response = { status: 400 } as AxiosError['response'];
  assertEquals(isRetryableError(error), false);
});

Deno.test('isRetryableError - returns false for 401 status', () => {
  const error = new AxiosError('Unauthorized');
  error.response = { status: 401 } as AxiosError['response'];
  assertEquals(isRetryableError(error), false);
});

Deno.test('isRetryableError - returns false for non-Axios errors', () => {
  const error = new Error('Generic error');
  assertEquals(isRetryableError(error), false);
});

// calculateDelay tests
Deno.test('calculateDelay - calculates exponential delay', () => {
  const options = {
    maxRetries: 3,
    initialDelayMs: 1000,
    maxDelayMs: 30000,
    backoff: 'exponential' as const,
  };

  // First retry: ~1000ms (with jitter)
  const delay1 = calculateDelay(1, options);
  assertGreaterOrEqual(delay1, 1000);
  assertLess(delay1, 1200);

  // Second retry: ~2000ms (with jitter)
  const delay2 = calculateDelay(2, options);
  assertGreaterOrEqual(delay2, 2000);
  assertLess(delay2, 2400);

  // Third retry: ~4000ms (with jitter)
  const delay3 = calculateDelay(3, options);
  assertGreaterOrEqual(delay3, 4000);
  assertLess(delay3, 4800);
});

Deno.test('calculateDelay - calculates linear delay', () => {
  const options = {
    maxRetries: 3,
    initialDelayMs: 1000,
    maxDelayMs: 30000,
    backoff: 'linear' as const,
  };

  const delay1 = calculateDelay(1, options);
  assertGreaterOrEqual(delay1, 1000);
  assertLess(delay1, 1200);

  const delay2 = calculateDelay(2, options);
  assertGreaterOrEqual(delay2, 2000);
  assertLess(delay2, 2400);
});

Deno.test('calculateDelay - caps at max delay', () => {
  const options = {
    maxRetries: 3,
    initialDelayMs: 1000,
    maxDelayMs: 30000,
    backoff: 'exponential' as const,
  };
  const delay = calculateDelay(10, options);
  assertLessOrEqual(delay, options.maxDelayMs);
});

// executeWithRetry tests
Deno.test('executeWithRetry - returns result on success', async () => {
  const result = await executeWithRetry(() => Promise.resolve('success'));
  assertEquals(result, 'success');
});

Deno.test('executeWithRetry - retries on retryable error', async () => {
  let attempts = 0;
  const operation = async () => {
    attempts++;
    if (attempts < 3) {
      const error = new AxiosError('Server Error');
      error.response = { status: 500 } as AxiosError['response'];
      throw error;
    }
    return 'success';
  };

  const result = await executeWithRetry(operation, {
    maxRetries: 3,
    initialDelayMs: 10,
  });

  assertEquals(result, 'success');
  assertEquals(attempts, 3);
});

Deno.test('executeWithRetry - throws after max retries', async () => {
  const error = new AxiosError('Server Error');
  error.response = { status: 500 } as AxiosError['response'];

  await assertRejects(
    () =>
      executeWithRetry(() => Promise.reject(error), {
        maxRetries: 2,
        initialDelayMs: 10,
      }),
    AxiosError,
  );
});

Deno.test('executeWithRetry - does not retry non-retryable errors', async () => {
  const error = new AxiosError('Bad Request');
  error.response = { status: 400 } as AxiosError['response'];

  let callCount = 0;
  const operation = async () => {
    callCount++;
    throw error;
  };

  await assertRejects(
    () =>
      executeWithRetry(operation, { maxRetries: 3, initialDelayMs: 10 }),
    AxiosError,
  );

  assertEquals(callCount, 1);
});

Deno.test('executeWithRetry - calls onRetry callback', async () => {
  const error = new AxiosError('Server Error');
  error.response = { status: 500 } as AxiosError['response'];

  let attempts = 0;
  const operation = async () => {
    attempts++;
    if (attempts < 2) throw error;
    return 'success';
  };

  let onRetryCalled = false;
  let retryAttempt = 0;
  const onRetry = (_err: Error, attempt: number, _delay: number) => {
    onRetryCalled = true;
    retryAttempt = attempt;
  };

  await executeWithRetry(operation, {
    maxRetries: 3,
    initialDelayMs: 10,
    onRetry,
  });

  assertEquals(onRetryCalled, true);
  assertEquals(retryAttempt, 1);
});

Deno.test('executeWithRetry - uses custom shouldRetry function', async () => {
  const error = new Error('Custom error');
  let attempts = 0;

  const operation = async () => {
    attempts++;
    if (attempts < 2) throw error;
    return 'success';
  };

  const result = await executeWithRetry(operation, {
    maxRetries: 3,
    initialDelayMs: 10,
    shouldRetry: () => true,
  });

  assertEquals(result, 'success');
});

// withRetry tests
Deno.test('withRetry - creates a retry-wrapped function', async () => {
  const fn = spy((_arg1: string, _arg2: string) => Promise.resolve('result'));
  const wrapped = withRetry(fn);

  const result = await wrapped('arg1', 'arg2');

  assertEquals(result, 'result');
  assertEquals(fn.calls.length, 1);
  assertEquals(fn.calls[0].args, ['arg1', 'arg2']);
});

// createRateLimitRetryPolicy tests
Deno.test('createRateLimitRetryPolicy - creates rate limit policy', () => {
  const policy = createRateLimitRetryPolicy(5, 10000);
  assertEquals(policy.maxRetries, 5);
  assertEquals(policy.initialDelayMs, 10000);
  assertEquals(policy.backoff, 'linear');
});

Deno.test('createRateLimitRetryPolicy - only retries 429 errors', () => {
  const policy = createRateLimitRetryPolicy();

  const error429 = new AxiosError('Too Many Requests');
  error429.response = { status: 429 } as AxiosError['response'];
  assertEquals(policy.shouldRetry!(error429, 1), true);

  const error500 = new AxiosError('Server Error');
  error500.response = { status: 500 } as AxiosError['response'];
  assertEquals(policy.shouldRetry!(error500, 1), false);
});

// DEFAULT_RETRY_OPTIONS tests
Deno.test('DEFAULT_RETRY_OPTIONS - has correct defaults', () => {
  assertEquals(DEFAULT_RETRY_OPTIONS.maxRetries, 3);
  assertEquals(DEFAULT_RETRY_OPTIONS.initialDelayMs, 2000);
  assertEquals(DEFAULT_RETRY_OPTIONS.maxDelayMs, 30000);
  assertEquals(DEFAULT_RETRY_OPTIONS.backoff, 'exponential');
});
