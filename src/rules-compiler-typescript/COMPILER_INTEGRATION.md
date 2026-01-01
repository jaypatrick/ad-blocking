# Compiler Integration

## Overview

The TypeScript rules compiler now uses the modern JSR-based `@jk-com/adblock-compiler` package as the primary compiler, with automatic fallback to the npm-based `@adguard/hostlist-compiler` if needed.

## Architecture

### Compiler Adapter (`src/lib/compiler-adapter.ts`)

The `compiler-adapter.ts` module provides automatic fallback logic:

1. **Primary**: Attempts to load `@jk-com/adblock-compiler` from JSR
2. **Fallback**: Falls back to `@adguard/hostlist-compiler` from npm if JSR fails
3. **Logging**: Logs which compiler is being used at startup

```typescript
import { compile, FilterCompiler, getCompilerInfo } from './lib/compiler-adapter.ts';

// Check which compiler is active
const info = getCompilerInfo();
console.log(`Using ${info.source} package: ${info.package}`);
```

## Benefits of JSR Package

The `@jk-com/adblock-compiler@^0.6.0` package includes:

✅ **SOLID Principles**: Refactored codebase following Single Responsibility, Open/Closed, etc.
✅ **Dependency Injection**: All components use DI for testability
✅ **Modern TypeScript**: Full type safety and modern patterns
✅ **Comprehensive Error Handling**: Descriptive errors with context
✅ **Performance Optimizations**: Improved pattern matching and processing
✅ **Better Documentation**: Full JSDoc coverage
✅ **Trace Logging**: Additional debug level for troubleshooting

## Backward Compatibility

- ✅ 100% API compatible with `@adguard/hostlist-compiler`
- ✅ Same `IConfiguration` interface
- ✅ Same `compile()` function signature
- ✅ No breaking changes to existing code

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
import { FilterCompiler } from './lib/compiler-adapter.ts';

const compiler = new FilterCompiler(logger);
const result = await compiler.compile(config);
```

### Check Active Compiler

```typescript
import { getCompilerInfo } from './lib/compiler-adapter.ts';

const { source, package: pkg } = getCompilerInfo();
console.log(`Active compiler: ${pkg} (${source})`);
```

## Migration Notes

### For Developers

No code changes required! The adapter handles everything automatically. Just import from `./lib/compiler-adapter.ts` instead of directly from `@adguard/hostlist-compiler`.

**Before:**
```typescript
import compile from '@adguard/hostlist-compiler';
```

**After:**
```typescript
import { compile } from './lib/compiler-adapter.ts';
```

### For CI/CD

No changes needed. The fallback mechanism ensures compilation works even if JSR registry is unavailable.

## Troubleshooting

### JSR Package Not Loading

If you see:
```
[Compiler] JSR package failed, falling back to npm
```

This is normal and automatic. The npm package will be used instead. Check:
- Network connectivity to jsr.io
- Deno cache status (`deno cache --reload`)

### Both Packages Fail

If both fail:
```
Failed to load compiler from both sources:
JSR: <error>
npm: <error>
```

1. Check internet connectivity
2. Verify `deno.json` has both imports configured
3. Run `deno cache --reload src/compiler.ts`

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
