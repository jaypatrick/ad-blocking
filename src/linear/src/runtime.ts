/**
 * Runtime abstraction layer for Deno and Bun compatibility
 *
 * This module provides a unified API that works across both runtimes.
 */

/** Detected runtime environment */
export type Runtime = 'deno' | 'bun' | 'node';

/** Detect the current runtime */
export function detectRuntime(): Runtime {
  // Check for Deno
  if (typeof globalThis !== 'undefined' && 'Deno' in globalThis) {
    return 'deno';
  }
  // Check for Bun
  if (typeof globalThis !== 'undefined' && 'Bun' in globalThis) {
    return 'bun';
  }
  // Default to Node.js
  return 'node';
}

/** Current runtime */
export const runtime: Runtime = detectRuntime();

/** Check if running in Deno */
export const isDeno = runtime === 'deno';

/** Check if running in Bun */
export const isBun = runtime === 'bun';

/** Check if running in Node.js */
export const isNode = runtime === 'node';

/**
 * Get environment variable value
 */
export function getEnv(key: string): string | undefined {
  if (isDeno) {
    try {
      // deno-lint-ignore no-explicit-any
      return (globalThis as any).Deno.env.get(key);
    } catch {
      return undefined;
    }
  }
  // Bun and Node.js use process.env
  return process.env[key];
}

/**
 * Get command line arguments (excluding runtime and script path)
 */
export function getArgs(): string[] {
  if (isDeno) {
    // deno-lint-ignore no-explicit-any
    return (globalThis as any).Deno.args;
  }
  // Bun and Node.js use process.argv, skip first two elements
  return process.argv.slice(2);
}

/**
 * Exit the process with a code
 */
export function exit(code: number): never {
  if (isDeno) {
    // deno-lint-ignore no-explicit-any
    (globalThis as any).Deno.exit(code);
  }
  process.exit(code);
}

/**
 * Check if this is the main module (entry point)
 */
export function isMainModule(importMeta: ImportMeta): boolean {
  if (isDeno) {
    // deno-lint-ignore no-explicit-any
    return (importMeta as any).main === true;
  }
  if (isBun) {
    // Bun uses import.meta.main
    // deno-lint-ignore no-explicit-any
    return (importMeta as any).main === true;
  }
  // Node.js - check if the module URL matches the entry point
  return false;
}
