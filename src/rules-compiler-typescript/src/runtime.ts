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
 * Get current working directory
 */
export function cwd(): string {
  if (isDeno) {
    // deno-lint-ignore no-explicit-any
    return (globalThis as any).Deno.cwd();
  }
  return process.cwd();
}

/**
 * Get runtime version information
 */
export function getVersionInfo(): { runtime: Runtime; version: string; os: string; arch: string } {
  if (isDeno) {
    // deno-lint-ignore no-explicit-any
    const deno = (globalThis as any).Deno;
    return {
      runtime: 'deno',
      version: deno.version.deno,
      os: deno.build.os,
      arch: deno.build.arch,
    };
  }
  if (isBun) {
    // deno-lint-ignore no-explicit-any
    const bun = (globalThis as any).Bun;
    return {
      runtime: 'bun',
      version: bun.version,
      os: process.platform,
      arch: process.arch,
    };
  }
  return {
    runtime: 'node',
    version: process.version,
    os: process.platform,
    arch: process.arch,
  };
}

/** Signal handler type */
export type SignalHandler = () => void;

/** Supported signals */
export type Signal = 'SIGINT' | 'SIGTERM' | 'SIGHUP';

/**
 * Add a signal listener
 */
export function addSignalListener(signal: Signal, handler: SignalHandler): void {
  if (isDeno) {
    // deno-lint-ignore no-explicit-any
    (globalThis as any).Deno.addSignalListener(signal, handler);
  } else {
    process.on(signal, handler);
  }
}

/**
 * Remove a signal listener
 */
export function removeSignalListener(signal: Signal, handler: SignalHandler): void {
  if (isDeno) {
    // deno-lint-ignore no-explicit-any
    (globalThis as any).Deno.removeSignalListener(signal, handler);
  } else {
    process.off(signal, handler);
  }
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
  // This is a simplified check; in practice, Node.js entry detection is more complex
  return false;
}
