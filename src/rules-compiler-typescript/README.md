# Rules Compiler TypeScript

TypeScript implementation of the AdGuard filter rules compiler using Deno.

## Features

- Compiles AdGuard filter rules from multiple sources
- Supports JSON, YAML, and TOML configuration formats
- SHA-384 hashing for output verification
- Interactive and CLI modes
- Comprehensive error handling and logging

## Requirements

- Deno 2.x or later

## Usage

### Interactive Mode

```bash
deno task interactive
```

### Compile from Config

```bash
deno task compile
```

### Command-Line Options

```bash
# Show help
deno task start -- --help

# Compile with specific config
deno task start -- -c config.yaml

# Compile with validation disabled
deno task start -- -c config.yaml --no-validate-config

# Fail on validation warnings
deno task start -- -c config.yaml --fail-on-warnings

# Validate configuration only
deno task start -- --validate -c config.yaml

# Show version information
deno task start -- --version
```

| Option | Description |
|--------|-------------|
| `-c, --config PATH` | Path to configuration file |
| `-o, --output PATH` | Path to output file |
| `-r, --copy-to-rules` | Copy output to rules directory |
| `--rules-dir PATH` | Custom rules directory path |
| `-f, --format FORMAT` | Force configuration format (json, yaml, toml) |
| `-v, --version` | Show version information |
| `-h, --help` | Show help message |
| `-d, --debug` | Enable debug output |
| `--show-config` | Show parsed configuration (don't compile) |
| `--validate-config` | Enable configuration validation before compilation (default: true) |
| `--no-validate-config` | Disable configuration validation before compilation |
| `--fail-on-warnings` | Fail compilation if configuration has validation warnings |
| `-i, --interactive` | Run in interactive menu mode |
| `--compile` | Run in CLI mode (compile and exit) |
| `--validate` | Validate configuration only |
| `--enable-chunking` | Enable chunked parallel compilation for large rule lists |
| `--chunk-size N` | Number of sources per chunk (when using source-based chunking) |
| `--max-parallel N` | Maximum number of chunks to compile in parallel (default: CPU count) |

### Chunked Parallel Compilation

For large rule lists (e.g., 10+ million entries), the single-threaded `@adguard/hostlist-compiler` can be slow. The chunking feature splits compilation into parallel chunks:

**Command-line usage:**
```bash
# Enable chunking with default settings (CPU count parallel workers)
deno task start -- --enable-chunking

# Custom parallel workers and chunk settings
deno task start -- --enable-chunking --max-parallel 8

# Fine-tune chunk size (sources per chunk)
deno task start -- --enable-chunking --chunk-size 50000 --max-parallel 4
```

**Configuration file:**
```json
{
  "name": "My Filter List",
  "chunking": {
    "enabled": true,
    "strategy": "source",
    "maxParallel": 4
  },
  "sources": [
    { "source": "https://example.com/list1.txt" },
    { "source": "https://example.com/list2.txt" }
  ]
}
```

**How it works:**
1. Splits sources into N chunks (based on `maxParallel`)
2. Compiles each chunk in parallel using Promise.all batching
3. Merges and deduplicates results
4. Preserves SHA-384 hash consistency

**When to use:**
- Large number of sources (10+)
- Very large individual sources (1M+ rules)
- Multi-core systems with available CPU resources

### Generate Type Definitions

To generate TypeScript declaration files (`.d.ts`):

```bash
deno task generate:types
```

This will create `.d.ts` files in the `dist/` directory that re-export types from the source files. These files provide type information for consumers of this library.

**Note**: The `.d.ts` files are automatically generated and should not be edited manually. They are excluded from version control (see `.gitignore`).

## Available Tasks

- `deno task start` - Run the compiler
- `deno task interactive` - Run in interactive mode
- `deno task compile` - Compile from default config
- `deno task test` - Run tests
- `deno task check` - Type check the code
- `deno task lint` - Lint the code
- `deno task fmt` - Format the code
- `deno task generate:types` - Generate `.d.ts` type definition files

## Type Definitions

This project uses Deno, which works with TypeScript natively. For compatibility with other tools or for publishing, type definition files (`.d.ts`) can be generated using:

```bash
deno task generate:types
```

The generated files are placed in the `dist/` directory and re-export all types from the source files.

## Development

1. Make changes to the TypeScript source files in `src/`
2. Run tests: `deno task test`
3. Type check: `deno task check`
4. Generate type definitions: `deno task generate:types`
