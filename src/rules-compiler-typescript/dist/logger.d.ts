/**
 * Logger utility for consistent console output
 */
import type { Logger } from './types';
/**
 * Creates a logger instance with timestamp prefixes
 * @param debugEnabled - Whether debug logging is enabled
 * @returns Logger instance
 */
export declare function createLogger(debugEnabled?: boolean): Logger;
/**
 * Default logger instance (debug disabled unless DEBUG env var is set)
 */
export declare const logger: Logger;
//# sourceMappingURL=logger.d.ts.map