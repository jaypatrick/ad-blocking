# @jk-com/adblock-compiler Guide

A comprehensive guide to the modern, JSR-based filter compilation package that powers all compilers in this repository.

## Table of Contents

- [What is @jk-com/adblock-compiler?](#what-is-jk-comadblock-compiler)
- [Why Use @jk-com/adblock-compiler?](#why-use-jk-comadblock-compiler)
- [Key Advantages Over AdGuard's hostlist-compiler](#key-advantages-over-adguards-hostlist-compiler)
- [Installation and Usage](#installation-and-usage)
- [CI/CD Integration](#cicd-integration)
- [Architecture and Design](#architecture-and-design)
- [Migration from hostlist-compiler](#migration-from-hostlist-compiler)
- [API Reference](#api-reference)

## What is @jk-com/adblock-compiler?

`@jk-com/adblock-compiler` is a **modern, SOLID-compliant TypeScript package** for compiling ad-blocking filter lists from multiple sources. It's distributed via [JSR (JavaScript Registry)](https://jsr.io/@jk-com/adblock-compiler) and serves as the core compilation engine for all rules compilers in this repository.

**Key Features:**
- ✅ **Multi-source compilation**: Combine local and remote filter lists
- ✅ **11 transformations**: Deduplicate, Validate, RemoveComments, Compress, and more
- ✅ **Multi-format support**: Adblock syntax and hosts file formats
- ✅ **Pattern matching**: Wildcards, regex, file-based inclusion/exclusion
- ✅ **TypeScript-first**: Full type safety with comprehensive interfaces
- ✅ **JSR distribution**: Modern package registry with better dependency management

**Package Information:**
- **Registry**: [jsr.io/@jk-com/adblock-compiler](https://jsr.io/@jk-com/adblock-compiler)
- **Source**: [github.com/jaypatrick/hostlistcompiler](https://github.com/jaypatrick/hostlistcompiler)
- **Current Version**: 0.6.0
- **License**: GPL-3.0

## Why Use @jk-com/adblock-compiler?

### Modern Architecture

Built from the ground up following **SOLID principles** for maintainability and extensibility:

1. **Single Responsibility Principle**: Each class has one clear purpose
   - `ConfigurationValidator`: Validates configuration
   - `SourceProcessor`: Processes filter sources
   - `TransformationEngine`: Applies transformations
   - `RuleDeduplicator`: Removes duplicate rules

2. **Open/Closed Principle**: Extensible without modification
   - Add new transformations without changing core code
   - Plugin-style architecture for custom processors

3. **Dependency Injection**: All components use DI
   - Easy testing with mock dependencies
   - Configurable logging, file system, HTTP clients
   - Swap implementations without code changes

### Enhanced Developer Experience

- **Comprehensive Type Safety**: Full TypeScript types for all APIs
- **Better Error Messages**: Descriptive errors with context and suggestions
- **JSDoc Documentation**: Every function, interface, and class documented
- **Trace Logging**: Debug-level logging for troubleshooting compilation issues

### Production-Ready Features

- **Performance Optimizations**: Improved pattern matching and rule processing
- **Memory Efficiency**: Optimized for large filter lists (100k+ rules)
- **Validation**: Input validation prevents invalid configurations
- **Backward Compatibility**: Drop-in replacement for `@adguard/hostlist-compiler`

## Key Advantages Over AdGuard's hostlist-compiler

| Feature | @jk-com/adblock-compiler | @adguard/hostlist-compiler |
|---------|--------------------------|----------------------------|
| **Architecture** | SOLID-compliant, 8+ specialized classes | Monolithic design |
| **Dependency Injection** | Full DI support for all components | Limited/no DI |
| **Type Safety** | Complete TypeScript interfaces | Partial typing |
| **Error Handling** | Descriptive errors with context | Generic error messages |
| **Documentation** | Full JSDoc coverage | Minimal documentation |
| **Testing** | DI-friendly, mockable components | Harder to test |
| **Distribution** | JSR (modern registry) | npm (legacy) |
| **Performance** | Optimized pattern matching | Standard implementation |
| **Logging** | Configurable with trace level | Basic logging |
| **Maintenance** | Active development | Slower updates |

### Specific Improvements

#### 1. Better Error Handling

**Before (hostlist-compiler):**
```typescript
// Generic error
Error: Invalid configuration
```

**After (@jk-com/adblock-compiler):**
```typescript
// Descriptive error with context
ValidationError: Configuration validation failed
  - sources[0].source: Must be a valid URL or file path
  - transformations[2]: 'InvalidTransform' is not a valid transformation
  Valid transformations: RemoveComments, Deduplicate, Validate, ...
```

#### 2. Type Safety

**Before:**
```typescript
// No type hints
const rules = await compile(config);
```

**After:**
```typescript
// Full type inference
import type { IConfiguration } from '@jk-com/adblock-compiler';

const config: IConfiguration = {
  name: 'My Filter',
  sources: [/* TypeScript knows what goes here */],
  transformations: [/* Autocomplete available */]
};
```

#### 3. Dependency Injection

**Before:**
```typescript
// Hard-coded dependencies
const compiler = new FilterCompiler();
// Can't customize HTTP client, logger, etc.
```

**After:**
```typescript
// Inject custom dependencies
import { FilterCompiler } from '@jk-com/adblock-compiler';

const compiler = new FilterCompiler(
  customLogger,      // Your logger implementation
  customFileSystem,  // Your file system abstraction
  customHttpClient   // Your HTTP client with retry logic
);
```

## Installation and Usage

### Deno (Recommended)

```bash
# Add to deno.json imports
deno add @jk-com/adblock-compiler
```

**deno.json:**
```json
{
  "imports": {
    "@jk-com/adblock-compiler": "jsr:@jk-com/adblock-compiler@^0.6.0"
  }
}
```

### Node.js (via JSR)

```bash
# Install with npm using JSR proxy
npx jsr add @jk-com/adblock-compiler
```

### Basic Usage

```typescript
import { compile } from '@jk-com/adblock-compiler';
import type { IConfiguration } from '@jk-com/adblock-compiler';

// Define configuration
const config: IConfiguration = {
  name: 'My Filter List',
  description: 'Custom ad-blocking filters',
  version: '1.0.0',
  sources: [
    {
      name: 'EasyList',
      source: 'https://easylist.to/easylist/easylist.txt',
      type: 'adblock',
    },
    {
      name: 'Local Rules',
      source: './custom-rules.txt',
      type: 'adblock',
    },
  ],
  transformations: [
    'RemoveComments',
    'Deduplicate',
    'Validate',
    'InsertFinalNewLine',
  ],
};

// Compile rules
const rules: string[] = await compile(config);
console.log(`Compiled ${rules.length} rules`);
```

### Advanced Usage with Dependency Injection

```typescript
import { FilterCompiler } from '@jk-com/adblock-compiler';
import type { ILogger, IConfiguration } from '@jk-com/adblock-compiler';

// Custom logger implementation
class CustomLogger implements ILogger {
  info(message: string): void {
    console.log(`[INFO] ${message}`);
  }
  
  warn(message: string): void {
    console.warn(`[WARN] ${message}`);
  }
  
  error(message: string): void {
    console.error(`[ERROR] ${message}`);
  }
  
  debug(message: string): void {
    if (process.env.DEBUG) {
      console.debug(`[DEBUG] ${message}`);
    }
  }
}

// Create compiler with custom logger
const logger = new CustomLogger();
const compiler = new FilterCompiler(logger);

// Compile with detailed logging
const result = await compiler.compile(config);
```

## CI/CD Integration

The `@jk-com/adblock-compiler` package is designed for seamless integration into CI/CD pipelines.

### GitHub Actions

#### Example 1: Compile and Deploy Filters

```yaml
name: Compile and Deploy Filters

on:
  push:
    branches: [main]
    paths:
      - 'data/input/**'
      - 'compiler-config.json'
  schedule:
    # Run daily to fetch updated remote lists
    - cron: '0 0 * * *'

jobs:
  compile-filters:
    runs-on: ubuntu-latest
    
    steps:
      - name: Checkout repository
        uses: actions/checkout@v4
      
      - name: Setup Deno
        uses: denoland/setup-deno@v2
        with:
          deno-version: v2.x
      
      - name: Cache Deno dependencies
        uses: actions/cache@v4
        with:
          path: ~/.cache/deno
          key: ${{ runner.os }}-deno-${{ hashFiles('**/deno.lock') }}
      
      - name: Compile filter rules
        run: |
          deno run --allow-read --allow-write --allow-env --allow-net \
            jsr:@jk-com/adblock-compiler/cli \
            --config compiler-config.json \
            --output data/output/filters.txt
      
      - name: Verify compilation
        run: |
          if [ ! -f data/output/filters.txt ]; then
            echo "Compilation failed - output file not created"
            exit 1
          fi
          
          RULE_COUNT=$(grep -v '^!' data/output/filters.txt | grep -v '^#' | grep -v '^$' | wc -l)
          echo "Compiled $RULE_COUNT rules"
          
          if [ $RULE_COUNT -lt 1000 ]; then
            echo "Warning: Rule count seems low ($RULE_COUNT)"
          fi
      
      - name: Commit updated filters
        run: |
          git config user.name "GitHub Actions"
          git config user.email "actions@github.com"
          git add data/output/filters.txt
          git diff --staged --quiet || git commit -m "Update compiled filters [skip ci]"
          git push
```

#### Example 2: Multi-Compiler Validation

Ensure all compilers produce identical output:

```yaml
name: Validate Compiler Equivalence

on:
  pull_request:
    paths:
      - 'src/rules-compiler-*/**'
      - 'compiler-config.json'

jobs:
  test-equivalence:
    runs-on: ubuntu-latest
    
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup Deno
        uses: denoland/setup-deno@v2
        with:
          deno-version: v2.x
      
      - name: Compile with TypeScript
        run: |
          cd src/rules-compiler-typescript
          deno task compile
          cp ../../data/output/adguard_user_filter.txt /tmp/output-ts.txt
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
      
      - name: Compile with .NET
        run: |
          cd src/rules-compiler-dotnet
          dotnet run --project src/RulesCompiler.Console
          cp ../../data/output/adguard_user_filter.txt /tmp/output-dotnet.txt
      
      - name: Compare outputs
        run: |
          # Files should be identical
          if ! diff /tmp/output-ts.txt /tmp/output-dotnet.txt; then
            echo "ERROR: Compiler outputs differ!"
            exit 1
          fi
          
          echo "✓ All compilers produce identical output"
```

### GitLab CI

```yaml
# .gitlab-ci.yml
stages:
  - compile
  - validate
  - deploy

compile-filters:
  stage: compile
  image: denoland/deno:latest
  
  cache:
    paths:
      - .deno_cache/
  
  before_script:
    - export DENO_DIR=.deno_cache
  
  script:
    - deno run --allow-read --allow-write --allow-env --allow-net
        jsr:@jk-com/adblock-compiler/cli
        --config compiler-config.json
        --output data/output/filters.txt
  
  artifacts:
    paths:
      - data/output/filters.txt
    expire_in: 30 days
  
  rules:
    - if: $CI_COMMIT_BRANCH == "main"
    - if: $CI_PIPELINE_SOURCE == "schedule"

validate-output:
  stage: validate
  image: alpine:latest
  
  dependencies:
    - compile-filters
  
  script:
    - RULE_COUNT=$(grep -c '^||' data/output/filters.txt || true)
    - echo "Compiled $RULE_COUNT adblock rules"
    - |
      if [ $RULE_COUNT -lt 100 ]; then
        echo "ERROR: Too few rules compiled"
        exit 1
      fi
```

### Jenkins Pipeline

```groovy
// Jenkinsfile
pipeline {
    agent any
    
    triggers {
        // Run daily at midnight
        cron('H 0 * * *')
    }
    
    environment {
        DENO_DIR = "${WORKSPACE}/.deno_cache"
    }
    
    stages {
        stage('Setup') {
            steps {
                sh 'curl -fsSL https://deno.land/install.sh | sh'
                sh 'export PATH="$HOME/.deno/bin:$PATH"'
            }
        }
        
        stage('Compile Filters') {
            steps {
                sh '''
                    deno run --allow-read --allow-write --allow-env --allow-net \
                        jsr:@jk-com/adblock-compiler/cli \
                        --config compiler-config.json \
                        --output data/output/filters.txt
                '''
            }
        }
        
        stage('Validate') {
            steps {
                script {
                    def ruleCount = sh(
                        script: "grep -v '^[!#]' data/output/filters.txt | grep -v '^\$' | wc -l",
                        returnStdout: true
                    ).trim()
                    
                    echo "Compiled ${ruleCount} rules"
                    
                    if (ruleCount.toInteger() < 1000) {
                        error("Rule count too low: ${ruleCount}")
                    }
                }
            }
        }
        
        stage('Archive') {
            steps {
                archiveArtifacts artifacts: 'data/output/filters.txt'
            }
        }
    }
    
    post {
        success {
            echo 'Filter compilation succeeded!'
        }
        failure {
            emailext(
                subject: "Filter Compilation Failed - ${env.JOB_NAME}",
                body: "Build ${env.BUILD_NUMBER} failed. Check console output.",
                to: 'team@example.com'
            )
        }
    }
}
```

### Direct CLI Usage in Scripts

#### Bash Script

```bash
#!/bin/bash
# compile-filters.sh

set -euo pipefail

CONFIG_FILE="${1:-compiler-config.json}"
OUTPUT_FILE="${2:-data/output/filters.txt}"

echo "Compiling filters using @jk-com/adblock-compiler..."
echo "  Config: $CONFIG_FILE"
echo "  Output: $OUTPUT_FILE"

# Compile filters
deno run --allow-read --allow-write --allow-env --allow-net \
  jsr:@jk-com/adblock-compiler/cli \
  --config "$CONFIG_FILE" \
  --output "$OUTPUT_FILE"

# Verify output
if [ -f "$OUTPUT_FILE" ]; then
  RULE_COUNT=$(grep -cv '^[!#]' "$OUTPUT_FILE" || true)
  echo "✓ Successfully compiled $RULE_COUNT rules"
else
  echo "✗ Compilation failed - output file not created"
  exit 1
fi

# Compute hash for verification
HASH=$(sha384sum "$OUTPUT_FILE" | awk '{print $1}')
echo "  SHA-384: $HASH"
```

#### PowerShell Script

```powershell
# Compile-Filters.ps1
[CmdletBinding()]
param(
    [string]$ConfigFile = "compiler-config.json",
    [string]$OutputFile = "data/output/filters.txt"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Write-Host "Compiling filters using @jk-com/adblock-compiler..." -ForegroundColor Cyan
Write-Host "  Config: $ConfigFile" -ForegroundColor Gray
Write-Host "  Output: $OutputFile" -ForegroundColor Gray

# Compile filters
deno run --allow-read --allow-write --allow-env --allow-net `
  jsr:@jk-com/adblock-compiler/cli `
  --config $ConfigFile `
  --output $OutputFile

# Verify output
if (Test-Path $OutputFile) {
    $ruleCount = (Get-Content $OutputFile | Where-Object { 
        $_ -notmatch '^[!#]' -and $_ -ne '' 
    }).Count
    
    Write-Host "✓ Successfully compiled $ruleCount rules" -ForegroundColor Green
    
    # Compute hash
    $hash = (Get-FileHash -Path $OutputFile -Algorithm SHA384).Hash
    Write-Host "  SHA-384: $hash" -ForegroundColor Gray
} else {
    Write-Error "Compilation failed - output file not created"
    exit 1
}
```

### Docker Integration

```dockerfile
# Dockerfile for filter compilation service
FROM denoland/deno:latest

WORKDIR /app

# Copy configuration and source files
COPY compiler-config.json .
COPY data/ data/

# Install dependencies (cached layer)
RUN deno cache jsr:@jk-com/adblock-compiler/cli

# Compile filters on container start
CMD ["deno", "run", \
     "--allow-read", "--allow-write", "--allow-env", "--allow-net", \
     "jsr:@jk-com/adblock-compiler/cli", \
     "--config", "compiler-config.json", \
     "--output", "data/output/filters.txt"]
```

**docker-compose.yml:**
```yaml
version: '3.8'

services:
  filter-compiler:
    build: .
    volumes:
      - ./data/input:/app/data/input:ro
      - ./data/output:/app/data/output:rw
      - ./compiler-config.json:/app/compiler-config.json:ro
    environment:
      - DEBUG=true
```

## Architecture and Design

### SOLID Principles in Action

The `@jk-com/adblock-compiler` package is built on **8 core classes**, each with a single responsibility:

```
┌─────────────────────────────────────────────────────────┐
│ FilterCompiler (Orchestrator)                           │
│ - Coordinates the compilation process                   │
│ - Manages dependencies via DI                           │
└─────────────────────────────────────────────────────────┘
                          │
          ┌───────────────┼───────────────┐
          │               │               │
          ▼               ▼               ▼
┌──────────────┐  ┌──────────────┐  ┌──────────────┐
│ Configuration│  │    Source    │  │Transformation│
│  Validator   │  │  Processor   │  │   Engine     │
└──────────────┘  └──────────────┘  └──────────────┘
          │               │               │
          │               │               │
          ▼               ▼               ▼
┌──────────────┐  ┌──────────────┐  ┌──────────────┐
│  Validation  │  │  Downloader  │  │     Rule     │
│   Service    │  │   Service    │  │ Deduplicator │
└──────────────┘  └──────────────┘  └──────────────┘
          │               │               │
          │               │               │
          ▼               ▼               ▼
        ┌─────────────────────────────────┐
        │   Dependency Injection Layer    │
        │  (Logger, FileSystem, HTTP)     │
        └─────────────────────────────────┘
```

### Key Interfaces

```typescript
// Core compilation interface
export interface IConfiguration {
  name: string;
  description?: string;
  version?: string;
  homepage?: string;
  license?: string;
  sources: ISource[];
  transformations?: TransformationType[];
  inclusions?: string[];
  exclusions?: string[];
}

// Source configuration
export interface ISource {
  name?: string;
  source: string;  // URL or file path
  type?: SourceType;
  transformations?: TransformationType[];
  inclusions?: string[];
  exclusions?: string[];
}

// Logger interface for DI
export interface ILogger {
  info(message: string): void;
  warn(message: string): void;
  error(message: string): void;
  debug(message: string): void;
}

// Validation result
export interface IValidationResult {
  valid: boolean;
  errors: string[];
  warnings: string[];
}
```

### Available Transformations

All 11 transformations from the original package, with improved implementations:

| Transformation | Description | Performance |
|---------------|-------------|-------------|
| `RemoveComments` | Remove comment lines (! and #) | O(n) |
| `Compress` | Convert hosts format to adblock | O(n) |
| `RemoveModifiers` | Remove unsupported modifiers | O(n) |
| `Validate` | Remove dangerous rules | O(n) |
| `ValidateAllowIp` | Validate with IP rules allowed | O(n) |
| `Deduplicate` | Remove duplicate rules | O(n log n) |
| `InvertAllow` | Convert exceptions to blocking | O(n) |
| `RemoveEmptyLines` | Remove blank lines | O(n) |
| `TrimLines` | Trim whitespace | O(n) |
| `InsertFinalNewLine` | Add final newline | O(1) |
| `ConvertToAscii` | Convert IDN to punycode | O(n) |

## Migration from hostlist-compiler

### Step 1: Update Package Reference

**Before:**
```json
{
  "imports": {
    "@adguard/hostlist-compiler": "npm:@adguard/hostlist-compiler@^1.0.0"
  }
}
```

**After:**
```json
{
  "imports": {
    "@jk-com/adblock-compiler": "jsr:@jk-com/adblock-compiler@^0.6.0"
  }
}
```

### Step 2: Update Imports

**Before:**
```typescript
import { compile } from '@adguard/hostlist-compiler';
```

**After:**
```typescript
import { compile } from '@jk-com/adblock-compiler';
```

### Step 3: No Code Changes Required!

The API is **100% backward compatible**. Your existing code will work without modifications:

```typescript
// This code works with both packages
const config = {
  name: 'My Filter',
  sources: [{ source: 'https://example.com/rules.txt' }],
  transformations: ['Deduplicate', 'Validate']
};

const rules = await compile(config);
```

### Step 4: Verify Compilation

Both packages should produce **identical output**:

```bash
# Compile with old package
deno run --allow-all old-compiler.ts > output-old.txt

# Compile with new package
deno run --allow-all new-compiler.ts > output-new.txt

# Verify identical output
diff output-old.txt output-new.txt
# Should show no differences
```

### Gradual Migration Strategy

The TypeScript compiler in this repository uses an **adapter pattern** with automatic fallback:

```typescript
// Tries JSR first, falls back to npm if unavailable
import { compile } from './lib/compiler-adapter.ts';

// No changes needed in your code
const rules = await compile(config);
```

This allows:
- ✅ Zero-downtime migration
- ✅ Gradual rollout across environments
- ✅ Automatic failover if JSR is unavailable
- ✅ Testing both versions in parallel

## API Reference

### Core Functions

#### `compile(config: IConfiguration): Promise<string[]>`

Compiles filter rules from the provided configuration.

**Parameters:**
- `config`: Configuration object defining sources and transformations

**Returns:**
- Promise resolving to array of compiled filter rules (strings)

**Example:**
```typescript
const rules = await compile({
  name: 'My Filter',
  sources: [{ source: 'https://example.com/rules.txt' }],
  transformations: ['Deduplicate']
});
```

### Classes

#### `FilterCompiler`

Main compiler class with dependency injection support.

**Constructor:**
```typescript
constructor(
  logger?: ILogger,
  fileSystem?: IFileSystem,
  httpClient?: IHttpClient
)
```

**Methods:**

- `compile(config: IConfiguration): Promise<string[]>`
  - Compiles rules from configuration
  
- `validate(config: IConfiguration): IValidationResult`
  - Validates configuration without compiling

**Example:**
```typescript
import { FilterCompiler } from '@jk-com/adblock-compiler';

const compiler = new FilterCompiler(customLogger);
const rules = await compiler.compile(config);
```

### Type Exports

All TypeScript interfaces are exported for type safety:

```typescript
import type {
  IConfiguration,
  ISource,
  ILogger,
  IValidationResult,
  TransformationType,
  SourceType
} from '@jk-com/adblock-compiler';
```

## Best Practices

### 1. Use Type Imports

```typescript
// Separate type imports from value imports
import { compile } from '@jk-com/adblock-compiler';
import type { IConfiguration } from '@jk-com/adblock-compiler';
```

### 2. Validate Configuration Early

```typescript
import { FilterCompiler } from '@jk-com/adblock-compiler';

const compiler = new FilterCompiler();
const validation = compiler.validate(config);

if (!validation.valid) {
  console.error('Configuration errors:', validation.errors);
  process.exit(1);
}

const rules = await compiler.compile(config);
```

### 3. Use Custom Logger for Production

```typescript
import { FilterCompiler } from '@jk-com/adblock-compiler';
import type { ILogger } from '@jk-com/adblock-compiler';

class ProductionLogger implements ILogger {
  info(msg: string) { /* Send to logging service */ }
  warn(msg: string) { /* Alert monitoring */ }
  error(msg: string) { /* Create incident */ }
  debug(msg: string) { /* Disable in production */ }
}

const compiler = new FilterCompiler(new ProductionLogger());
```

### 4. Cache Compiled Results

```typescript
// Cache compilation results to avoid redundant downloads
const cacheKey = JSON.stringify(config);
const cached = await cache.get(cacheKey);

if (cached) {
  return cached;
}

const rules = await compile(config);
await cache.set(cacheKey, rules, { ttl: 3600 });
```

## Troubleshooting

### Common Issues

#### 1. JSR Registry Unreachable

**Symptom:** `Failed to load jsr:@jk-com/adblock-compiler`

**Solutions:**
- Check internet connectivity to jsr.io
- Verify firewall/proxy settings allow JSR access
- Use npm fallback: `import from '@adguard/hostlist-compiler'`

#### 2. Type Errors After Migration

**Symptom:** TypeScript errors about incompatible types

**Solution:**
```bash
# Clear Deno cache and reload
deno cache --reload src/mod.ts
```

#### 3. Different Output After Migration

**Symptom:** Compiled output differs from previous version

**Solution:**
```typescript
// Check which compiler is active
import { getCompilerInfo } from './lib/compiler-adapter.ts';

const info = await getCompilerInfo();
console.log('Active compiler:', info);
```

## Resources

- **JSR Package**: https://jsr.io/@jk-com/adblock-compiler
- **Source Code**: https://github.com/jaypatrick/hostlistcompiler
- **Issue Tracker**: https://github.com/jaypatrick/hostlistcompiler/issues
- **This Repository**: https://github.com/jaypatrick/ad-blocking
- **Documentation Website**: https://jaypatrick.github.io/ad-blocking

## Version History

- **v0.6.0** (2026-01-01): Initial JSR release
  - SOLID-compliant architecture
  - 8 specialized classes following Single Responsibility Principle
  - Full dependency injection support
  - Comprehensive error handling with context
  - Performance optimizations
  - Complete JSDoc documentation
  - Trace-level logging for debugging

## Contributing

Contributions to `@jk-com/adblock-compiler` are welcome! Please:

1. Visit the [source repository](https://github.com/jaypatrick/hostlistcompiler)
2. Review the contribution guidelines
3. Submit issues or pull requests

## License

GPL-3.0 - See [LICENSE](../../LICENSE) for details.
