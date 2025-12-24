/**
 * Jest test setup file
 *
 * IMPORTANT: This file sets up test-only mock values.
 * These are NOT real credentials and are only used for unit testing.
 */

import { jest } from '@jest/globals';

// Increase timeout for integration tests
jest.setTimeout(30000);

// Set test-only mock API key for unit tests (NOT a real credential)
// Real integration tests should use actual credentials from environment
const TEST_MOCK_API_KEY = 'test-mock-api-key-for-unit-tests';
process.env.ADGUARD_API_KEY = process.env.ADGUARD_API_KEY || TEST_MOCK_API_KEY;
process.env.ADGUARD_AdGuard__ApiKey = process.env.ADGUARD_AdGuard__ApiKey || TEST_MOCK_API_KEY;
