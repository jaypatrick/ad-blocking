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
