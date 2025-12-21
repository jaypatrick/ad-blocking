/**
 * Jest test setup file
 */

/// <reference types="jest" />

// Increase timeout for integration tests
jest.setTimeout(30000);

// Mock environment variables for tests
process.env.ADGUARD_API_KEY = process.env.ADGUARD_API_KEY || 'test-api-key';
