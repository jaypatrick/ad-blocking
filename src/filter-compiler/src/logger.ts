/**
 * @fileoverview Logger utility for consistent console output
 * @module logger
 */

import type { Logger } from './types';

/**
 * Creates a logger instance with timestamp prefixes
 * @param debugEnabled - Whether debug logging is enabled
 * @returns Logger instance
 */
export function createLogger(debugEnabled = false): Logger {
    const timestamp = (): string => new Date().toISOString();

    return {
        info: (message: string, ...args: unknown[]): void => {
            console.log(`[INFO] ${timestamp()} - ${message}`, ...args);
        },

        warn: (message: string, ...args: unknown[]): void => {
            console.warn(`[WARN] ${timestamp()} - ${message}`, ...args);
        },

        error: (message: string, ...args: unknown[]): void => {
            console.error(`[ERROR] ${timestamp()} - ${message}`, ...args);
        },

        debug: (message: string, ...args: unknown[]): void => {
            if (debugEnabled || process.env.DEBUG) {
                console.debug(`[DEBUG] ${timestamp()} - ${message}`, ...args);
            }
        }
    };
}

/**
 * Default logger instance (debug disabled unless DEBUG env var is set)
 */
export const logger = createLogger(!!process.env.DEBUG);
