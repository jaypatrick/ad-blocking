/**
 * Core compiler service for filter rules compilation
 */
import { type IConfiguration } from '@adguard/hostlist-compiler';
import type { CompilerResult, CompileOptions, Logger } from './types';
/**
 * Writes compiled rules to an output file
 * @param outputPath - Path to output file
 * @param rules - Array of compiled rules
 * @param logger - Logger instance
 */
export declare function writeOutput(outputPath: string, rules: string[], logger?: Logger): void;
/**
 * Counts non-empty, non-comment lines in a file
 * @param filePath - Path to file
 * @returns Number of rules
 */
export declare function countRules(filePath: string): number;
/**
 * Computes SHA-384 hash of a file
 * @param filePath - Path to file
 * @returns Hex-encoded hash string
 */
export declare function computeHash(filePath: string): string;
/**
 * Copies compiled output to rules directory
 * @param sourcePath - Path to source file
 * @param destPath - Path to destination file
 * @param logger - Logger instance
 */
export declare function copyToRulesDirectory(sourcePath: string, destPath: string, logger?: Logger): void;
/**
 * Compiles filter rules using the hostlist-compiler
 * @param config - Compiler configuration
 * @param logger - Logger instance
 * @returns Array of compiled rules
 */
export declare function compileFilters(config: IConfiguration, logger?: Logger): Promise<string[]>;
/**
 * Runs the full compilation pipeline
 * @param options - Compilation options
 * @returns Compilation result
 */
export declare function runCompiler(options: CompileOptions): Promise<CompilerResult>;
//# sourceMappingURL=compiler.d.ts.map