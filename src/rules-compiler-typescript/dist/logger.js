"use strict";
/**
 * Logger utility for consistent console output
 */
Object.defineProperty(exports, "__esModule", { value: true });
exports.logger = void 0;
exports.createLogger = createLogger;
/**
 * Formats timestamp for log messages
 */
function timestamp() {
    return new Date().toISOString();
}
/**
 * Creates a logger instance with timestamp prefixes
 * @param debugEnabled - Whether debug logging is enabled
 * @returns Logger instance
 */
function createLogger(debugEnabled = false) {
    return {
        info(message, ...args) {
            console.log(`[INFO] ${timestamp()} - ${message}`, ...args);
        },
        warn(message, ...args) {
            console.warn(`[WARN] ${timestamp()} - ${message}`, ...args);
        },
        error(message, ...args) {
            console.error(`[ERROR] ${timestamp()} - ${message}`, ...args);
        },
        debug(message, ...args) {
            if (debugEnabled || process.env.DEBUG) {
                console.debug(`[DEBUG] ${timestamp()} - ${message}`, ...args);
            }
        },
    };
}
/**
 * Default logger instance (debug disabled unless DEBUG env var is set)
 */
exports.logger = createLogger(!!process.env.DEBUG);
//# sourceMappingURL=logger.js.map