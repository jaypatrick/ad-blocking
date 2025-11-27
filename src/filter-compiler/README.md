# Filter Compiler

A TypeScript-based AdGuard filter list compiler that aggregates and transforms ad-blocking rules from multiple sources into a single, optimized filter list.

## Overview

The filter-compiler uses the `@adguard/hostlist-compiler` library to:

- Aggregate filter rules from local files and remote URLs
- Apply transformations (deduplication, compression, validation, etc.)
- Output optimized filter lists compatible with AdGuard

## Directory Structure

```
filter-compiler/
├── invoke-compiler.ts      # Main TypeScript compiler entry point
├── invoke-compiler.test.ts # Jest unit tests
├── invoke-compiler.ps1     # PowerShell wrapper script (Windows alternative)
├── compiler-config.json    # Configuration for filter compilation
├── package.json            # npm dependencies and scripts
├── tsconfig.json           # TypeScript compiler configuration
├── jest.config.js          # Jest testing framework configuration
├── eslint.config.mjs       # ESLint linting rules
└── output/                 # Output directory for compiled filters
```

## Installation

```bash
cd src/filter-compiler
npm install
```

## Usage

### Running the Compiler

**Using npm (recommended):**

```bash
npm run compile
```

**Using ts-node directly:**

```bash
npx ts-node invoke-compiler.ts
```

**Using PowerShell (Windows):**

```powershell
./invoke-compiler.ps1
```

> **Note:** The PowerShell script requires a global installation of the hostlist-compiler:
> ```bash
> npm install -g @adguard/hostlist-compiler
> ```

### Build TypeScript

```bash
npm run build
```

This compiles TypeScript to JavaScript in the output directory.

## Configuration

The compiler reads from `compiler-config.json`:

```json
{
  "name": "JK.com AdGuard Rules",
  "description": "Base AdGuard blocking list for JK.com",
  "version": "4.0.1.35",
  "sources": [
    {
      "name": "Local Rules",
      "source": "../../rules/adguard_user_filter.txt",
      "type": "adblock",
      "inclusions": ["*"]
    },
    {
      "name": "Remote rules",
      "source": "https://raw.githubusercontent.com/...",
      "type": "adblock",
      "inclusions": ["*"]
    }
  ],
  "transformations": [
    "Deduplicate",
    "Compress",
    "Validate",
    "RemoveEmptyLines",
    "TrimLines",
    "InsertFinalNewLine",
    "ConvertToAscii"
  ]
}
```

### Configuration Options

| Field | Description |
|-------|-------------|
| `name` | Name of the compiled filter list |
| `description` | Description for the filter list header |
| `version` | Version number for tracking changes |
| `sources` | Array of filter sources (local or remote) |
| `transformations` | Processing steps applied to the compiled rules |

### Available Transformations

- `Deduplicate` - Remove duplicate rules
- `Compress` - Optimize rule format
- `Validate` - Validate rule syntax
- `RemoveEmptyLines` - Strip blank lines
- `TrimLines` - Remove leading/trailing whitespace
- `InsertFinalNewLine` - Ensure file ends with newline
- `ConvertToAscii` - Convert Unicode to ASCII equivalents

## API

The module exports four main functions:

### `readConfiguration(configPath: string): IConfiguration`

Reads and parses the JSON configuration file.

```typescript
import { readConfiguration } from './invoke-compiler';

const config = readConfiguration('./compiler-config.json');
```

### `compileFilters(config: IConfiguration): Promise<string[]>`

Compiles filter rules using the hostlist-compiler library.

```typescript
import { compileFilters } from './invoke-compiler';

const rules = await compileFilters(config);
// Returns: ['||ads.example.com^', '||tracker.example.com^', ...]
```

### `writeOutput(outputPath: string, rules: string[]): Promise<void>`

Writes compiled rules to the output file.

```typescript
import { writeOutput } from './invoke-compiler';

await writeOutput('./output/filters.txt', rules);
```

### `main(): Promise<void>`

Orchestrates the entire compilation workflow with logging and timing.

```typescript
import { main } from './invoke-compiler';

await main();
```

## Logging

The compiler includes a built-in logger with four levels:

| Level | Description |
|-------|-------------|
| `info` | General information messages |
| `warn` | Warning messages |
| `error` | Error messages |
| `debug` | Debug output (only when `DEBUG` env var is set) |

All logs include ISO timestamps in format: `[LEVEL] YYYY-MM-DDTHH:MM:SS.sssZ - message`

### Enable Debug Logging

```bash
DEBUG=true npm run compile
```

## Testing

### Run Tests

```bash
npm test
```

### Run Tests with Coverage

```bash
npm run test:coverage
```

Coverage reports are generated in multiple formats:
- Terminal output (text)
- HTML report in `coverage/` directory
- LCOV format for CI integration

### Test Coverage

The test suite covers:
- Configuration reading (valid files, missing files, invalid JSON)
- Output writing (normal rules, empty arrays, special characters)
- Logger functionality (timestamp formatting)
- Edge cases (very long rules, Unicode characters, rule order preservation)

## Linting

```bash
npm run lint
```

ESLint is configured with:
- Recommended JavaScript rules
- TypeScript ESLint recommended rules
- Covers `.js`, `.mjs`, `.cjs`, `.ts` files

## Output

The compiler generates the compiled filter list at:

```
../../rules/adguard_user_filter.txt
```

This output file contains the aggregated and transformed filter rules ready for use with AdGuard.

## Dependencies

### Runtime

| Package | Version | Description |
|---------|---------|-------------|
| `@adguard/hostlist-compiler` | ^1.0.39 | Core compilation engine |

### Development

| Package | Version | Description |
|---------|---------|-------------|
| `typescript` | 5.4.5 | TypeScript compiler |
| `jest` | 29.7.0 | Testing framework |
| `ts-jest` | 29.1.2 | TypeScript support for Jest |
| `ts-node` | 10.9.2 | TypeScript execution engine |
| `eslint` | 9.39.1 | Code linting |
| `@types/node` | ^20.12.7 | Node.js type definitions |

## CI/CD Integration

The filter-compiler integrates with GitHub Actions workflows for:
- Type checking (`npx tsc --noEmit`)
- Linting checks
- Test execution
- Dependency caching

## Troubleshooting

### Common Issues

**"Configuration file not found"**
- Ensure `compiler-config.json` exists in the filter-compiler directory
- Check that the path in the configuration is correct

**"Failed to compile filters"**
- Verify network connectivity for remote sources
- Check that source URLs are accessible
- Validate the configuration JSON format

**"Permission denied" on output**
- Ensure the output directory exists and is writable
- Check file system permissions

### Debug Mode

Enable verbose logging to troubleshoot issues:

```bash
DEBUG=true npm run compile
```
