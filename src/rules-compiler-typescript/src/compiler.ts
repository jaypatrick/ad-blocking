/**
 * Core compiler service for filter rules compilation
 * Production-ready with timeouts, error handling, and resource limits
 */

import compile, { type IConfiguration } from '@adguard/hostlist-compiler';
import { writeFileSync, readFileSync, existsSync, copyFileSync, mkdirSync, statSync } from 'node:fs';
import { dirname, join, resolve } from 'node:path';
import { createHash } from 'node:crypto';
import type { CompilerResult, CompileOptions, Logger } from './types';
import { readConfiguration } from './config-reader';
import { logger as defaultLogger } from './logger';
import {
  CompilationError,
  ErrorCode,
  isCompilerError,
} from './errors';
import { withTimeout } from './timeout';
import { DEFAULT_RESOURCE_LIMITS, checkFileSize } from './validation';

/**
 * Writes compiled rules to an output file
 * @param outputPath - Path to output file
 * @param rules - Array of compiled rules
 * @param logger - Logger instance
 */
export function writeOutput(
  outputPath: string,
  rules: string[],
  logger: Logger = defaultLogger
): void {
  logger.debug(`Writing ${rules.length} rules to: ${outputPath}`);

  // Ensure output directory exists
  const outputDir = dirname(outputPath);
  if (!existsSync(outputDir)) {
    mkdirSync(outputDir, { recursive: true });
    logger.debug(`Created output directory: ${outputDir}`);
  }

  const content = rules.join('\n');
  writeFileSync(outputPath, content, 'utf8');

  logger.info(`Wrote ${rules.length} lines to ${outputPath}`);
}

/**
 * Counts non-empty, non-comment lines in a file
 * @param filePath - Path to file
 * @returns Number of rules
 */
export function countRules(filePath: string): number {
  if (!existsSync(filePath)) {
    return 0;
  }

  const content = readFileSync(filePath, 'utf8');
  const lines = content.split('\n');

  return lines.filter((line) => {
    const trimmed = line.trim();
    if (!trimmed) return false;
    if (trimmed.startsWith('!')) return false;
    if (trimmed.startsWith('#')) return false;
    return true;
  }).length;
}

/**
 * Computes SHA-384 hash of a file
 * @param filePath - Path to file
 * @returns Hex-encoded hash string
 */
export function computeHash(filePath: string): string {
  const content = readFileSync(filePath);
  return createHash('sha384').update(content).digest('hex');
}

/**
 * Copies compiled output to rules directory
 * @param sourcePath - Path to source file
 * @param destPath - Path to destination file
 * @param logger - Logger instance
 */
export function copyToRulesDirectory(
  sourcePath: string,
  destPath: string,
  logger: Logger = defaultLogger
): void {
  logger.debug(`Copying ${sourcePath} to ${destPath}`);

  const destDir = dirname(destPath);
  if (!existsSync(destDir)) {
    mkdirSync(destDir, { recursive: true });
  }

  copyFileSync(sourcePath, destPath);
  logger.info(`Copied to rules directory: ${destPath}`);
}

/**
 * Compiler options with resource limits
 */
export interface CompilerOptions {
  /** Compilation timeout in milliseconds */
  timeoutMs?: number;
  /** Maximum output file size in bytes */
  maxOutputSize?: number;
}

/**
 * Default compiler options
 */
const DEFAULT_COMPILER_OPTIONS: CompilerOptions = {
  timeoutMs: DEFAULT_RESOURCE_LIMITS.compilationTimeoutMs,
  maxOutputSize: DEFAULT_RESOURCE_LIMITS.maxOutputFileSize,
};

/**
 * Compiles filter rules using the hostlist-compiler
 * @param config - Compiler configuration
 * @param logger - Logger instance
 * @param options - Compiler options
 * @returns Array of compiled rules
 */
export async function compileFilters(
  config: IConfiguration,
  logger: Logger = defaultLogger,
  options: CompilerOptions = {}
): Promise<string[]> {
  const resolvedOptions = { ...DEFAULT_COMPILER_OPTIONS, ...options };
  logger.info('Starting filter compilation...');

  try {
    // Wrap compilation with timeout
    const result = await withTimeout(
      compile(config),
      resolvedOptions.timeoutMs ?? DEFAULT_RESOURCE_LIMITS.compilationTimeoutMs,
      { configName: config.name }
    );

    logger.info(`Compilation complete. Generated ${result.length} rules.`);
    return result;
  } catch (error) {
    // Re-throw if already a CompilerError
    if (isCompilerError(error)) {
      logger.error(`Compilation failed: ${error.toLogString()}`);
      throw error;
    }

    const message = error instanceof Error ? error.message : 'Unknown error';
    logger.error(`Compilation failed: ${message}`);
    throw new CompilationError(
      `Filter compilation failed: ${message}`,
      ErrorCode.COMPILATION_FAILED,
      { configName: config.name },
      error instanceof Error ? error : undefined
    );
  }
}

/**
 * Generates a timestamped output filename
 * @returns Filename with timestamp
 */
function generateOutputFilename(): string {
  const timestamp = new Date().toISOString().replace(/[:.]/g, '-').slice(0, 19);
  return `compiled-${timestamp}.txt`;
}

/**
 * Extended compile options with resource limits
 */
export interface ExtendedCompileOptions extends CompileOptions {
  /** Compilation timeout in milliseconds */
  timeoutMs?: number;
  /** Maximum output file size in bytes */
  maxOutputSize?: number;
}

/**
 * Runs the full compilation pipeline
 * @param options - Compilation options
 * @returns Compilation result
 */
export async function runCompiler(options: ExtendedCompileOptions): Promise<CompilerResult> {
  const logger = options.logger ?? defaultLogger;
  const startTime = new Date();

  const result: CompilerResult = {
    success: false,
    configName: '',
    configVersion: '',
    ruleCount: 0,
    outputPath: '',
    outputHash: '',
    copiedToRules: false,
    elapsedMs: 0,
    startTime,
    endTime: new Date(),
  };

  try {
    // Read configuration
    logger.info(`Loading configuration from: ${options.configPath}`);
    const config = readConfiguration(options.configPath, options.format, logger);
    result.configName = config.name ?? 'unknown';
    const configRecord = config as unknown as Record<string, unknown>;
    const versionValue = configRecord.version;
    result.configVersion = typeof versionValue === 'string' ? versionValue : 'unknown';

    // Determine output path
    const outputFilename = generateOutputFilename();
    const defaultOutputPath = join(dirname(options.configPath), 'output', outputFilename);
    const outputPath = options.outputPath ?? defaultOutputPath;
    result.outputPath = resolve(outputPath);

    // Compile filters with timeout
    const rules = await compileFilters(config, logger, {
      timeoutMs: options.timeoutMs,
      maxOutputSize: options.maxOutputSize,
    });

    // Write output
    writeOutput(result.outputPath, rules, logger);

    // Check output file size
    const outputStats = statSync(result.outputPath);
    const maxOutputSize = options.maxOutputSize ?? DEFAULT_RESOURCE_LIMITS.maxOutputFileSize;
    checkFileSize(outputStats.size, maxOutputSize, 'output file');

    // Calculate statistics
    result.ruleCount = countRules(result.outputPath);
    result.outputHash = computeHash(result.outputPath);

    logger.debug(`Hash: ${result.outputHash}`);

    // Copy to rules directory if requested
    if (options.copyToRules) {
      const rulesDir =
        options.rulesDirectory ?? join(dirname(options.configPath), '..', '..', 'rules');
      const destPath = join(rulesDir, 'adguard_user_filter.txt');
      copyToRulesDirectory(result.outputPath, resolve(destPath), logger);
      result.copiedToRules = true;
      result.rulesDestination = resolve(destPath);
    }

    result.success = true;
  } catch (error) {
    // Use structured error information if available
    if (isCompilerError(error)) {
      result.errorMessage = error.toLogString();
      result.errorCode = error.code;
    } else {
      const message = error instanceof Error ? error.message : 'Unknown error';
      result.errorMessage = message;
    }
    logger.error(`Compilation failed: ${result.errorMessage}`);
  }

  result.endTime = new Date();
  result.elapsedMs = result.endTime.getTime() - startTime.getTime();

  return result;
}
