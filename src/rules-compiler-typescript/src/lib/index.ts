/**
 * Rules Compiler Library API
 *
 * This module provides the main library entry points for programmatic usage.
 * Use these exports when integrating the rules compiler into your application.
 *
 * @example
 * ```typescript
 * import {
 *   RulesCompiler,
 *   ConfigurationBuilder,
 *   createRulesCompiler,
 *   createConfiguration
 * } from '@rules-compiler/typescript/lib';
 *
 * // Simple usage
 * const compiler = RulesCompiler.create();
 * const result = await compiler.compile({ configPath: 'config.yaml' });
 *
 * // With builder pattern
 * const compiler = RulesCompiler.builder()
 *   .withTimeout(60000)
 *   .withDebug(true)
 *   .build();
 *
 * // Programmatic configuration
 * const config = ConfigurationBuilder.create('My Filters')
 *   .addSource('https://example.com/filters.txt')
 *   .withPreset('basic')
 *   .build();
 *
 * const rules = await compiler.compileFromConfig(config);
 * ```
 *
 * @packageDocumentation
 */

// Main compiler service and builder
export {
  RulesCompiler,
  RulesCompilerBuilder,
  createRulesCompiler,
} from './rules-compiler.ts';

export type {
  RulesCompilerServiceOptions,
  CompileRunOptions,
  CompileProgressEvent,
} from './rules-compiler.ts';

// Configuration builder
export {
  ConfigurationBuilder,
  createConfiguration,
  AVAILABLE_TRANSFORMATIONS,
  TRANSFORMATION_DESCRIPTIONS,
} from './configuration-builder.ts';

export type {
  Transformation,
  SourceType,
  SourceConfig,
} from './configuration-builder.ts';

// Re-export core types for convenience
export type {
  CompilerResult,
  ConfigurationFormat,
  Logger,
  ExtendedConfiguration,
  VersionInfo,
  PlatformInfo,
} from '../types.ts';

// Re-export validation types
export type {
  ValidationResult,
  ResourceLimits,
} from '../validation.ts';

// Re-export commonly needed utilities
export { validateConfiguration, DEFAULT_RESOURCE_LIMITS } from '../validation.ts';
