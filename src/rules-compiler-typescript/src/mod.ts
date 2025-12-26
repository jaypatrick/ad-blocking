/**
 * Rules Compiler TypeScript Frontend - Deno Entry Point
 *
 * This module provides Deno-compatible exports and CLI entry point.
 * Uses Deno's Node.js compatibility layer for file system operations.
 *
 * @module
 */

// Re-export everything from the main index
export * from './index.ts';

// Deno CLI entry point
import { main } from './cli.ts';

// Run if executed directly
if (import.meta.main) {
  const exitCode = await main(Deno.args);
  Deno.exit(exitCode);
}
