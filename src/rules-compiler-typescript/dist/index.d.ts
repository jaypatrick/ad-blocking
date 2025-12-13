/**
 * Rules Compiler TypeScript Frontend
 *
 * A TypeScript API and CLI for compiling AdGuard filter rules
 * using @adguard/hostlist-compiler.
 *
 * @packageDocumentation
 */
export type { ConfigurationFormat, CliOptions, CompilerResult, VersionInfo, PlatformInfo, ExtendedConfiguration, Logger, CompileOptions, } from './types';
export { detectFormat, findDefaultConfig, readConfiguration, toJson, } from './config-reader';
export { writeOutput, countRules, computeHash, copyToRulesDirectory, compileFilters, runCompiler, } from './compiler';
export { parseArgs, showHelp, showVersion, getVersionInfo, main } from './cli';
export { createLogger, logger } from './logger';
//# sourceMappingURL=index.d.ts.map