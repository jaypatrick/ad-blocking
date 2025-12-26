/**
 * Retry policy helper for HTTP requests
 * Matches .NET RetryPolicyHelper functionality
 */

import { AxiosError } from 'axios';

/** Retry policy options */
export interface RetryOptions {
  /** Maximum number of retries */
  maxRetries?: number;
  /** Initial delay in milliseconds */
  initialDelayMs?: number;
  /** Maximum delay in milliseconds */
  maxDelayMs?: number;
  /** Backoff strategy */
  backoff?: 'exponential' | 'linear';
  /** Custom retry condition */
  shouldRetry?: (error: Error, attempt: number) => boolean;
  /** Callback on retry */
  onRetry?: (error: Error, attempt: number, delayMs: number) => void;
}

/** Default retry options */
export const DEFAULT_RETRY_OPTIONS: Required<Omit<RetryOptions, 'shouldRetry' | 'onRetry'>> = {
  maxRetries: 3,
  initialDelayMs: 2000,
  maxDelayMs: 30000,
  backoff: 'exponential',
};

/** Status codes that should trigger a retry */
const RETRYABLE_STATUS_CODES = [408, 429, 500, 502, 503, 504];

/** Check if an error is retryable */
export function isRetryableError(error: Error): boolean {
  if (error instanceof AxiosError) {
    const status = error.response?.status;
    if (status && RETRYABLE_STATUS_CODES.includes(status)) {
      return true;
    }
    // Network errors
    if (error.code === 'ECONNRESET' || error.code === 'ETIMEDOUT' || error.code === 'ENOTFOUND') {
      return true;
    }
  }
  return false;
}

/** Calculate delay for a retry attempt */
export function calculateDelay(
  attempt: number,
  options: Required<Omit<RetryOptions, 'shouldRetry' | 'onRetry'>>,
): number {
  let delay: number;

  if (options.backoff === 'exponential') {
    // Exponential backoff: delay = initialDelay * 2^(attempt-1)
    delay = options.initialDelayMs * Math.pow(2, attempt - 1);
  } else {
    // Linear backoff: delay = initialDelay * attempt
    delay = options.initialDelayMs * attempt;
  }

  // Add jitter (10% randomization)
  const jitter = delay * 0.1 * Math.random();
  delay = delay + jitter;

  // Cap at max delay
  return Math.min(delay, options.maxDelayMs);
}

/** Sleep for a given number of milliseconds */
function sleep(ms: number): Promise<void> {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

/** Execute an async function with retry logic */
export async function executeWithRetry<T>(
  operation: () => Promise<T>,
  options: RetryOptions = {},
): Promise<T> {
  const opts = {
    ...DEFAULT_RETRY_OPTIONS,
    ...options,
  };

  let lastError: Error | undefined;

  for (let attempt = 0; attempt <= opts.maxRetries; attempt++) {
    try {
      return await operation();
    } catch (error) {
      lastError = error instanceof Error ? error : new Error(String(error));

      // Check if we should retry
      const shouldRetry = opts.shouldRetry
        ? opts.shouldRetry(lastError, attempt + 1)
        : isRetryableError(lastError);

      if (attempt >= opts.maxRetries || !shouldRetry) {
        throw lastError;
      }

      // Calculate delay and wait
      const delay = calculateDelay(attempt + 1, opts);

      if (opts.onRetry) {
        opts.onRetry(lastError, attempt + 1, delay);
      }

      await sleep(delay);
    }
  }

  throw lastError;
}

/** Create a retry-enabled wrapper for an async function */
export function withRetry<TArgs extends unknown[], TResult>(
  fn: (...args: TArgs) => Promise<TResult>,
  options: RetryOptions = {},
): (...args: TArgs) => Promise<TResult> {
  return (...args: TArgs) => executeWithRetry(() => fn(...args), options);
}

/** Create a rate limit specific retry policy */
export function createRateLimitRetryPolicy(
  maxRetries: number = 3,
  baseDelayMs: number = 5000,
): RetryOptions {
  return {
    maxRetries,
    initialDelayMs: baseDelayMs,
    backoff: 'linear',
    shouldRetry: (error: Error) => {
      if (error instanceof AxiosError) {
        return error.response?.status === 429;
      }
      return false;
    },
  };
}

/** Retry policy helper object */
export const RetryPolicy = {
  executeWithRetry,
  withRetry,
  isRetryableError,
  calculateDelay,
  createRateLimitRetryPolicy,
  DEFAULT_OPTIONS: DEFAULT_RETRY_OPTIONS,
};

export default RetryPolicy;
