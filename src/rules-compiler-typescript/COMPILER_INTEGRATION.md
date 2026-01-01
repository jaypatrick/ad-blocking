# Compiler Integration

## Overview

The TypeScript rules compiler now uses the modern JSR-based `@jk-com/adblock-compiler` package for all compilation tasks.

## Architecture

### Compiler Adapter (`src/lib/compiler-adapter.ts`)

The `compiler-adapter.ts` module provides automatic fallback logic with lazy initialization:

1. **Lazy Loading**: Compiler is loaded on first use, not at module import time
2. **Primary**: Attempts to load `@jk-com/adblock-compiler` from JSR
3. **Fallback**: Falls back to `@adguard/hostlist-compiler` from npm if JSR fails
4. **Logging**: Logs which compiler is being used when initialized

```typescript
import { compile, getFilterCompiler, getCompilerInfo } from './lib/compiler-adapter.ts';

// Check which compiler is active (triggers lazy initialization if not yet loaded)
const info = await getCompilerInfo();
console.log(`Using ${info.source} package: ${info.package}`);
```

**Key Design Decision**: The adapter uses lazy initialization instead of top-level await to prevent blocking module imports. This ensures fast startup and deferred loading until the compiler is actually needed.

## Benefits of JSR Package

The `@jk-com/adblock-compiler@^0.6.0` package includes:

✅ **SOLID Principles**: Refactored codebase following Single Responsibility, Open/Closed, etc.
✅ **Dependency Injection**: All components use DI for testability
✅ **Modern TypeScript**: Full type safety and modern patterns
✅ **Comprehensive Error Handling**: Descriptive errors with context
✅ **Performance Optimizations**: Improved pattern matching and processing
✅ **Better Documentation**: Full JSDoc coverage
✅ **Trace Logging**: Additional debug level for troubleshooting

## API Compatibility

- ✅ Standard `IConfiguration` interface
- ✅ Standard `compile()` function signature
- ✅ Drop-in replacement for filter compilation needs

## Usage

### Basic Compilation

```typescript
import { compile } from './lib/compiler-adapter.ts';
import type { IConfiguration } from './lib/compiler-adapter.ts';

const config: IConfiguration = {
  name: 'My Filter List',
  sources: [
    {
      source: 'https://example.com/rules.txt',
      type: 'adblock',
    },
  ],
  transformations: ['RemoveComments', 'Deduplicate'],
};

const rules = await compile(config);
console.log(`Compiled ${rules.length} rules`);
```

### Using FilterCompiler Class

```typescript
import { getFilterCompiler } from './lib/compiler-adapter.ts';

// Get the FilterCompiler class (lazy loads on first call)
const FilterCompiler = await getFilterCompiler();
const compiler = new FilterCompiler(logger);
const result = await compiler.compile(config);
```

### Check Active Compiler

```typescript
import { getCompilerInfo } from './lib/compiler-adapter.ts';

// This will trigger lazy initialization if compiler not yet loaded
const { source, package: pkg } = await getCompilerInfo();
console.log(`Active compiler: ${pkg} (${source})`);
```

## Migration Notes

### For Developers

Import from `./lib/compiler-adapter.ts` to use the compiler:

```typescript
import { compile } from './lib/compiler-adapter.ts';
```

### For CI/CD

The JSR package is used for all compilations. Ensure network access to jsr.io is available.

## Troubleshooting

### JSR Package Not Loading

If the package fails to load:

1. Check internet connectivity to jsr.io
2. Verify `deno.json` has the import configured
3. Run `deno cache --reload src/compiler.ts`
4. Check Deno cache status: `deno cache --reload`

## Version History

- **v0.6.0** (2026-01-01): Initial JSR integration with Phase 1 SOLID refactoring
  - SOLID-compliant interfaces
  - 8 new classes following SRP
  - Full dependency injection support
  - Comprehensive error handling

## Links

- JSR Package: https://jsr.io/@jk-com/adblock-compiler
- Source Code: https://github.com/jaypatrick/hostlistcompiler
- Issue Tracker: https://github.com/jaypatrick/hostlistcompiler/issues

## Future Enhancements

Phase 2 refactoring (planned):
- Factory pattern implementations
- Complete DI integration
- Remove duplicate code
- Enhanced testing infrastructure
