/**
 * Rules Compiler TypeScript Frontend - Deno Entry Point
 *
 * This module provides Deno-compatible exports and CLI entry point.
 * It uses Node.js compatibility layer for file system operations.
 *
 * @module
 */

// Re-export everything from the main index
export * from './index.ts';

// Deno CLI entry point
import { main } from './cli.ts';

// Check if running as main module in Deno
const isDeno = typeof Deno !== 'undefined';
const isMainModule = isDeno && Deno.mainModule === import.meta.url;

if (isMainModule) {
  const exitCode = await main(Deno.args);
  Deno.exit(exitCode);
}
