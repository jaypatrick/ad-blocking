/**
 * Retry policy helper tests - Basic validation
 * Full test coverage with mocks to be added in follow-up
 */

import { assertEquals, assert } from '@std/assert';
import { AxiosError } from 'axios';
import {
  executeWithRetry,
  isRetryableError,
  calculateDelay,
  DEFAULT_RETRY_OPTIONS,
} from '../../src/helpers/retry.ts';

Deno.test('RetryPolicy', async (t) => {
  await t.step('isRetryableError - should return true for 429 status', () => {
    const error = new AxiosError('Too Many Requests');
    error.response = { status: 429 } as any;
    assertEquals(isRetryableError(error), true);
  });

  await t.step('isRetryableError - should return true for 5xx status', () => {
    const error = new AxiosError('Server Error');
    error.response = { status: 500 } as any;
    assertEquals(isRetryableError(error), true);
  });

  await t.step('isRetryableError - should return false for 400 status', () => {
    const error = new AxiosError('Bad Request');
    error.response = { status: 400 } as any;
    assertEquals(isRetryableError(error), false);
  });

  await t.step('isRetryableError - should return false for non-Axios errors', () => {
    const error = new Error('Generic error');
    assertEquals(isRetryableError(error), false);
  });

  await t.step('calculateDelay - should calculate exponential delay', () => {
    const options = {
      maxRetries: 3,
      initialDelayMs: 1000,
      maxDelayMs: 30000,
      backoff: 'exponential' as const,
    };
    
    const delay1 = calculateDelay(1, options);
    assert(delay1 >= 1000);
    assert(delay1 < 1200);
  });

  await t.step('calculateDelay - should cap at max delay', () => {
    const options = {
      maxRetries: 10,
      initialDelayMs: 1000,
      maxDelayMs: 30000,
      backoff: 'exponential' as const,
    };
    
    const delay = calculateDelay(10, options);
    assert(delay <= options.maxDelayMs);
  });

  await t.step('executeWithRetry - should return result on success', async () => {
    const result = await executeWithRetry(() => Promise.resolve('success'));
    assertEquals(result, 'success');
  });

  await t.step('DEFAULT_RETRY_OPTIONS - should have correct defaults', () => {
    assertEquals(DEFAULT_RETRY_OPTIONS.maxRetries, 3);
    assertEquals(DEFAULT_RETRY_OPTIONS.initialDelayMs, 2000);
    assertEquals(DEFAULT_RETRY_OPTIONS.maxDelayMs, 30000);
    assertEquals(DEFAULT_RETRY_OPTIONS.backoff, 'exponential');
  });
});
