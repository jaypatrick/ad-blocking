/**
 * Compiler adapter that tries the new JSR package first, with fallback to AdGuard npm package
 * 
 * This adapter uses lazy initialization to avoid blocking module imports with top-level await.
 * The compiler is loaded on first use, not at module load time.
 */

/**
 * Type definitions for compiler source
 */
type CompilerSource = 'jsr' | 'npm';

/**
 * Compiler function signature (compatible with both packages)
 */
type CompilerFunction = (config: unknown) => Promise<string[]>;

/**
 * FilterCompiler class constructor signature (compatible with both packages)
 */
type FilterCompilerConstructor = new (...args: unknown[]) => unknown;

/**
 * Compiler module state
 */
interface CompilerModule {
  FilterCompiler: FilterCompilerConstructor;
  compile: CompilerFunction;
  source: CompilerSource;
}

/**
 * Cached compiler module instance
 */
let compilerModule: CompilerModule | null = null;

/**
 * Promise for ongoing initialization (prevents multiple simultaneous loads)
 */
let initPromise: Promise<CompilerModule> | null = null;

/**
 * Initialize the compiler by attempting to load JSR package first, then npm fallback
 * This function is called lazily on first use, not at module load time
 */
async function initializeCompiler(): Promise<CompilerModule> {
  // Return cached module if already initialized
  if (compilerModule) {
    return compilerModule;
  }

  // If initialization is in progress, wait for it
  if (initPromise) {
    return initPromise;
  }

  // Start initialization
  initPromise = (async () => {
    try {
      // Try JSR package first
      const jsrModule = await import('@jk-com/adblock-compiler');
      const module: CompilerModule = {
        FilterCompiler: jsrModule.FilterCompiler as FilterCompilerConstructor,
        compile: jsrModule.compile as CompilerFunction,
        source: 'jsr',
      };
      console.log('[Compiler] Using JSR package: @jk-com/adblock-compiler@^0.6.0');
      compilerModule = module;
      return module;
    } catch (jsrError) {
      console.warn('[Compiler] JSR package failed, falling back to npm:', jsrError);
      
      try {
        // Fallback to npm package
        const npmModule = await import('@adguard/hostlist-compiler');
        const module: CompilerModule = {
          FilterCompiler: npmModule.FilterCompiler as FilterCompilerConstructor,
          compile: npmModule.compile as CompilerFunction,
          source: 'npm',
        };
        console.log('[Compiler] Using npm package: @adguard/hostlist-compiler');
        compilerModule = module;
        return module;
      } catch (npmError) {
        throw new Error(
          `Failed to load compiler from both sources:\nJSR: ${jsrError}\nnpm: ${npmError}`,
        );
      }
    } finally {
      // Clear the promise once initialization completes
      initPromise = null;
    }
  })();

  return initPromise;
}

/**
 * Get information about which compiler is being used
 * If the compiler hasn't been initialized yet, this will trigger initialization
 */
export async function getCompilerInfo(): Promise<{ source: CompilerSource; package: string }> {
  const module = await initializeCompiler();
  return {
    source: module.source,
    package: module.source === 'jsr'
      ? '@jk-com/adblock-compiler@^0.6.0'
      : '@adguard/hostlist-compiler',
  };
}

/**
 * Compile function with lazy initialization
 * This ensures the compiler is loaded only when first used
 */
export async function compile(config: unknown): Promise<string[]> {
  const module = await initializeCompiler();
  return module.compile(config);
}

/**
 * Get FilterCompiler class with lazy initialization
 * This ensures the compiler is loaded only when first used
 */
export async function getFilterCompiler(): Promise<FilterCompilerConstructor> {
  const module = await initializeCompiler();
  return module.FilterCompiler;
}

/**
 * Re-export types conditionally based on which package is loaded
 * 
 * NOTE: We export types from JSR package as the canonical source since it has
 * the most complete type definitions. The npm package (@adguard/hostlist-compiler)
 * is fully compatible with these types as it shares the same API surface.
 * 
 * If you need to ensure type compatibility at runtime, use the getCompilerInfo()
 * function to determine which package is loaded and handle accordingly.
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
