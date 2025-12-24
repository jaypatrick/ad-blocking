/**
 * Retry policy helper tests
 */

import { assertEquals, assertGreaterOrEqual, assertLess, assertLessOrEqual, assertRejects } from '@std/assert';
import { spy, assertSpyCall, assertSpyCalls } from '@std/testing/mock';
import { AxiosError, type AxiosResponse } from 'npm:axios';
import {
  executeWithRetry,
  withRetry,
  isRetryableError,
  calculateDelay,
  createRateLimitRetryPolicy,
  DEFAULT_RETRY_OPTIONS,
} from '../../src/helpers/retry.ts';

// isRetryableError tests
Deno.test('isRetryableError - should return true for 408 status', () => {
  const error = new AxiosError('Timeout');
  error.response = { status: 408 } as AxiosResponse;
  assertEquals(isRetryableError(error), true);
});

Deno.test('isRetryableError - should return true for 429 status', () => {
  const error = new AxiosError('Too Many Requests');
  error.response = { status: 429 } as AxiosResponse;
  assertEquals(isRetryableError(error), true);
});

Deno.test('isRetryableError - should return true for 5xx status', () => {
  for (const status of [500, 502, 503, 504]) {
    const error = new AxiosError('Server Error');
    error.response = { status } as AxiosResponse;
    assertEquals(isRetryableError(error), true);
  }
});

Deno.test('isRetryableError - should return true for network errors', () => {
  const error = new AxiosError('Network Error');
  error.code = 'ECONNRESET';
  assertEquals(isRetryableError(error), true);
});

Deno.test('isRetryableError - should return false for 400 status', () => {
  const error = new AxiosError('Bad Request');
  error.response = { status: 400 } as AxiosResponse;
  assertEquals(isRetryableError(error), false);
});

Deno.test('isRetryableError - should return false for 401 status', () => {
  const error = new AxiosError('Unauthorized');
  error.response = { status: 401 } as AxiosResponse;
  assertEquals(isRetryableError(error), false);
});

Deno.test('isRetryableError - should return false for non-Axios errors', () => {
  const error = new Error('Generic error');
  assertEquals(isRetryableError(error), false);
});

// calculateDelay tests
Deno.test('calculateDelay - should calculate exponential delay', () => {
  const baseOptions = {
    maxRetries: 3,
    initialDelayMs: 1000,
    maxDelayMs: 30000,
    backoff: 'exponential' as const,
  };

  // First retry: 1000ms
  const delay1 = calculateDelay(1, baseOptions);
  assertGreaterOrEqual(delay1, 1000);
  assertLess(delay1, 1200); // Allow for jitter

  // Second retry: 2000ms
  const delay2 = calculateDelay(2, baseOptions);
  assertGreaterOrEqual(delay2, 2000);
  assertLess(delay2, 2400);

  // Third retry: 4000ms
  const delay3 = calculateDelay(3, baseOptions);
  assertGreaterOrEqual(delay3, 4000);
  assertLess(delay3, 4800);
});

Deno.test('calculateDelay - should calculate linear delay', () => {
  const baseOptions = {
    maxRetries: 3,
    initialDelayMs: 1000,
    maxDelayMs: 30000,
    backoff: 'exponential' as const,
  };
  const linearOptions = { ...baseOptions, backoff: 'linear' as const };

  const delay1 = calculateDelay(1, linearOptions);
  assertGreaterOrEqual(delay1, 1000);
  assertLess(delay1, 1200);

  const delay2 = calculateDelay(2, linearOptions);
  assertGreaterOrEqual(delay2, 2000);
  assertLess(delay2, 2400);
});

Deno.test('calculateDelay - should cap at max delay', () => {
  const baseOptions = {
    maxRetries: 3,
    initialDelayMs: 1000,
    maxDelayMs: 30000,
    backoff: 'exponential' as const,
  };
  const delay = calculateDelay(10, baseOptions);
  assertLessOrEqual(delay, baseOptions.maxDelayMs);
});

// executeWithRetry tests
Deno.test('executeWithRetry - should return result on success', async () => {
  const result = await executeWithRetry(() => Promise.resolve('success'));
  assertEquals(result, 'success');
});

Deno.test('executeWithRetry - should retry on retryable error', async () => {
  let attempts = 0;
  const operation = () => {
    attempts++;
    if (attempts < 3) {
      const error = new AxiosError('Server Error');
      error.response = { status: 500 } as AxiosResponse;
      throw error;
    }
    return Promise.resolve('success');
  };
  const operationSpy = spy(operation);

  const result = await executeWithRetry(operationSpy, {
    maxRetries: 3,
    initialDelayMs: 10,
  });

  assertEquals(result, 'success');
  assertSpyCalls(operationSpy, 3);
});

Deno.test('executeWithRetry - should throw after max retries', async () => {
  const error = new AxiosError('Server Error');
  error.response = { status: 500 } as AxiosResponse;

  await assertRejects(
    () => executeWithRetry(() => Promise.reject(error), {
      maxRetries: 2,
      initialDelayMs: 10,
    }),
    AxiosError
  );
});

Deno.test('executeWithRetry - should not retry non-retryable errors', async () => {
  const error = new AxiosError('Bad Request');
  error.response = { status: 400 } as AxiosResponse;
  const operation = () => Promise.reject(error);
  const operationSpy = spy(operation);

  await assertRejects(
    () => executeWithRetry(operationSpy, { maxRetries: 3, initialDelayMs: 10 }),
    AxiosError
  );

  assertSpyCalls(operationSpy, 1);
});

Deno.test('executeWithRetry - should call onRetry callback', async () => {
  const error = new AxiosError('Server Error');
  error.response = { status: 500 } as AxiosResponse;

  let attempts = 0;
  const operation = () => {
    attempts++;
    if (attempts < 2) throw error;
    return Promise.resolve('success');
  };

  const onRetryFn = (_err: Error, _attempt: number, _delay: number) => {};
  const onRetrySpy = spy(onRetryFn);

  await executeWithRetry(operation, {
    maxRetries: 3,
    initialDelayMs: 10,
    onRetry: onRetrySpy,
  });

  assertSpyCalls(onRetrySpy, 1);
  assertSpyCall(onRetrySpy, 0, {
    args: [error, 1, onRetrySpy.calls[0].args[2]],
  });
});

Deno.test('executeWithRetry - should use custom shouldRetry function', async () => {
  const error = new Error('Custom error');
  let attempts = 0;

  const operation = () => {
    attempts++;
    if (attempts < 2) throw error;
    return Promise.resolve('success');
  };

  const result = await executeWithRetry(operation, {
    maxRetries: 3,
    initialDelayMs: 10,
    shouldRetry: () => true,
  });

  assertEquals(result, 'success');
});

// withRetry tests
Deno.test('withRetry - should create a retry-wrapped function', async () => {
  const fn = (_arg1: string, _arg2: string) => Promise.resolve('result');
  const fnSpy = spy(fn);
  const wrapped = withRetry(fnSpy);

  const result = await wrapped('arg1', 'arg2');

  assertEquals(result, 'result');
  assertSpyCall(fnSpy, 0, { args: ['arg1', 'arg2'] });
});

// createRateLimitRetryPolicy tests
Deno.test('createRateLimitRetryPolicy - should create rate limit policy', () => {
  const policy = createRateLimitRetryPolicy(5, 10000);
  assertEquals(policy.maxRetries, 5);
  assertEquals(policy.initialDelayMs, 10000);
  assertEquals(policy.backoff, 'linear');
});

Deno.test('createRateLimitRetryPolicy - should only retry 429 errors', () => {
  const policy = createRateLimitRetryPolicy();

  const error429 = new AxiosError('Too Many Requests');
  error429.response = { status: 429 } as AxiosResponse;
  assertEquals(policy.shouldRetry!(error429, 1), true);

  const error500 = new AxiosError('Server Error');
  error500.response = { status: 500 } as AxiosResponse;
  assertEquals(policy.shouldRetry!(error500, 1), false);
});

// DEFAULT_RETRY_OPTIONS tests
Deno.test('DEFAULT_RETRY_OPTIONS - should have correct defaults', () => {
  assertEquals(DEFAULT_RETRY_OPTIONS.maxRetries, 3);
  assertEquals(DEFAULT_RETRY_OPTIONS.initialDelayMs, 2000);
  assertEquals(DEFAULT_RETRY_OPTIONS.maxDelayMs, 30000);
  assertEquals(DEFAULT_RETRY_OPTIONS.backoff, 'exponential');
});
