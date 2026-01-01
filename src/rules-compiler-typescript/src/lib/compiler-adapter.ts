/**
 * Compiler adapter that loads the JSR package
 */

import type { IConfiguration, ILogger } from '@jk-com/adblock-compiler';

// Import from JSR
let FilterCompiler: any;
let compile: any;
const compilerSource: 'jsr' = 'jsr';

try {
  const jsrModule = await import('@jk-com/adblock-compiler');
  FilterCompiler = jsrModule.FilterCompiler;
  compile = jsrModule.compile;
  console.log('[Compiler] Using JSR package: @jk-com/adblock-compiler@^0.6.0');
} catch (jsrError) {
  throw new Error(
    `Failed to load compiler from JSR: ${jsrError}`,
  );
}

/**
 * Get information about which compiler is being used
 */
export function getCompilerInfo(): { source: 'jsr'; package: string } {
  return {
    source: compilerSource,
    package: '@jk-com/adblock-compiler@^0.6.0',
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
