/**
 * Compiler adapter that tries the new JSR package first, with fallback to AdGuard npm package
 */

import type { IConfiguration, ILogger } from '@jk-com/adblock-compiler';

// Try to import from JSR first
let FilterCompiler: any;
let compile: any;
let compilerSource: 'jsr' | 'npm' = 'jsr';

try {
  const jsrModule = await import('@jk-com/adblock-compiler');
  FilterCompiler = jsrModule.FilterCompiler;
  compile = jsrModule.compile;
  compilerSource = 'jsr';
  console.log('[Compiler] Using JSR package: @jk-com/adblock-compiler@^0.6.0');
} catch (jsrError) {
  console.warn('[Compiler] JSR package failed, falling back to npm:', jsrError);
  try {
    const npmModule = await import('@adguard/hostlist-compiler');
    FilterCompiler = npmModule.FilterCompiler;
    compile = npmModule.compile;
    compilerSource = 'npm';
    console.log('[Compiler] Using npm package: @adguard/hostlist-compiler');
  } catch (npmError) {
    throw new Error(
      `Failed to load compiler from both sources:\nJSR: ${jsrError}\nnpm: ${npmError}`,
    );
  }
}

/**
 * Get information about which compiler is being used
 */
export function getCompilerInfo(): { source: 'jsr' | 'npm'; package: string } {
  return {
    source: compilerSource,
    package: compilerSource === 'jsr'
      ? '@jk-com/adblock-compiler@^0.6.0'
      : '@adguard/hostlist-compiler',
  };
}

/**
 * Export the compiler classes and functions
 * These will be from whichever package successfully loaded
 */
export { compile, FilterCompiler };

/**
 * Export types from JSR package (they're compatible)
 */
export type {
  IBasicLogger,
  IConfiguration,
  IDetailedLogger,
  IDownloader,
  IFileSystem,
  IFilterable,
  IHttpClient,
  ILogger,
  ISource,
  ITransformable,
  IValidationResult,
  SourceType,
  TransformationType,
} from '@jk-com/adblock-compiler';
