# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This repository is a comprehensive multi-language toolkit for ad-blocking, network protection, and AdGuard DNS management:

### Rules Compilers (4 languages)
- **TypeScript** (`src/rules-compiler-typescript/`) - Deno 2.0+ with npm compatibility
- **C#/.NET 10** (`src/rules-compiler-dotnet/`) - Library and Spectre.Console CLI with DI support
- **Python 3.9+** (`src/rules-compiler-python/`) - pip-installable package with CLI and API
- **Rust** (`src/rules-compiler-rust/`) - High-performance single binary with zero runtime deps

### Shell Scripts
- **Bash** (`src/rules-compiler-shell/compile-rules.sh`) - Linux/macOS
- **Zsh** (`src/rules-compiler-shell/compile-rules.zsh`) - macOS/Linux with zsh-specific features
- **PowerShell Core** (`src/rules-compiler-shell/compile-rules.ps1`) - Cross-platform
- **Windows Batch** (`src/rules-compiler-shell/compile-rules.cmd`) - Windows wrapper

### PowerShell Module
- **RulesCompiler Module** (`src/adguard-api-powershell/`) - Full-featured PowerShell API with Pester tests

### API Client & Tools
- **AdGuard API Client - .NET** (`src/adguard-api-dotnet/`) - C# SDK for AdGuard DNS API v1.11
- **AdGuard API Client - TypeScript** (`src/adguard-api-typescript/`) - TypeScript SDK with Deno support
- **Console UI** (`src/adguard-api-dotnet/src/AdGuard.ConsoleUI/`) - Spectre.Console interactive interface
- **Linear Import Tool** (`src/linear/`) - TypeScript tool with Deno support

### Website
- **Gatsby Site** (`src/website/`) - Portfolio site deployed to GitHub Pages (Node.js)

### Configuration Support
All compilers support JSON, YAML, and TOML configuration formats with full @adguard/hostlist-compiler compatibility.

## Docker Development Environment

A fully-featured Docker environment with all compilers and tools:

```dockerfile
# Dockerfile.warp
FROM mcr.microsoft.com/dotnet/sdk:10.0-noble
# Includes: .NET 10 SDK, Deno 2.x, Python 3.12, Rust stable, PowerShell 7
# Pre-installed: hostlist-compiler, yq, pytest, ruff, clippy, Pester
```

Build and run:
```bash
docker build -f Dockerfile.warp -t ad-blocking-dev .
docker run -it -v $(pwd):/workspace ad-blocking-dev
```

Docker Compose (recommended):
```bash
docker compose up -d dev           # Start dev environment
docker compose exec dev bash       # Enter container
docker compose --profile test run --rm test  # Run all tests
docker compose --profile website up website  # Start website dev server
```

Warp Environment: `jaysonknight/warp-env:ad-blocking` (ID: `Egji4sZU4TNIOwNasFU73A`)

## Common Commands

### TypeScript Rules Compiler (`src/rules-compiler-typescript/`)
```bash
cd src/rules-compiler-typescript

# Deno tasks
deno task compile              # Compile filter rules
deno task compile:yaml         # Compile using YAML config
deno task compile:toml         # Compile using TOML config
deno task dev                  # Run with watch mode
deno task test                 # Run Deno tests
deno task test:coverage        # Run tests with coverage
deno task lint                 # Lint source files
deno task fmt                  # Format source files
deno task check                # Type check
deno task version              # Show version

# CLI with options
deno task compile -- -c config.yaml -r -d
deno run --allow-read --allow-write --allow-env --allow-run src/mod.ts --help
deno run --allow-read --allow-write --allow-env --allow-run src/mod.ts --version
```

### Shell Scripts (`src/rules-compiler-shell/`)
```bash
# Bash (Linux/macOS)
./src/rules-compiler-shell/compile-rules.sh                    # Use default config
./src/rules-compiler-shell/compile-rules.sh -c config.yaml -r  # YAML config, copy to rules
./src/rules-compiler-shell/compile-rules.sh -v                 # Show version

# Zsh (macOS/Linux)
./src/rules-compiler-shell/compile-rules.zsh                   # Use default config
./src/rules-compiler-shell/compile-rules.zsh -c config.yaml -r # YAML config, copy to rules
./src/rules-compiler-shell/compile-rules.zsh -v                # Show version

# PowerShell Core (all platforms)
./src/rules-compiler-shell/compile-rules.ps1
./src/rules-compiler-shell/compile-rules.ps1 -ConfigPath config.yaml -CopyToRules
./src/rules-compiler-shell/compile-rules.ps1 -Version

# Windows Batch
src\rules-compiler-shell\compile-rules.cmd -c config.json -r
```

### .NET Rules Compiler (`src/rules-compiler-dotnet/`)
```bash
cd src/rules-compiler-dotnet
dotnet restore RulesCompiler.slnx
dotnet build RulesCompiler.slnx
dotnet test RulesCompiler.slnx
dotnet run --project src/RulesCompiler.Console/RulesCompiler.Console.csproj

# Command-line options
dotnet run --project src/RulesCompiler.Console -- --config path/to/config.yaml
dotnet run --project src/RulesCompiler.Console -- --config config.json --copy
dotnet run --project src/RulesCompiler.Console -- --config config.yaml --verbose
dotnet run --project src/RulesCompiler.Console -- --config config.yaml --validate
dotnet run --project src/RulesCompiler.Console -- --version
```

### Python Rules Compiler (`src/rules-compiler-python/`)
```bash
cd src/rules-compiler-python

# Install in development mode
pip install -e .

# Install with dev dependencies
pip install -e ".[dev]"

# Run tests
pytest
pytest -v                    # Verbose output
pytest --cov=rules_compiler  # With coverage

# CLI usage
rules-compiler                           # Use default config
rules-compiler -c config.yaml            # Specific config
rules-compiler -c config.json -r         # Compile and copy to rules
rules-compiler -c config.toml -o out.txt # Custom output
rules-compiler -V                        # Show version info
rules-compiler -d                        # Debug output
rules-compiler --help                    # Show help
```

### Rust Rules Compiler (`src/rules-compiler-rust/`)
```bash
cd src/rules-compiler-rust

# Build
cargo build              # Debug build
cargo build --release    # Release build (optimized)

# Run tests
cargo test
cargo test -- --nocapture  # With output

# CLI usage
cargo run -- -c config.yaml              # Specific config
cargo run -- -c config.json -r           # Compile and copy to rules
cargo run -- -c config.toml -o out.txt   # Custom output
cargo run -- -V                          # Show version info
cargo run -- -d                          # Debug output
cargo run -- --help                      # Show help

# Release binary
./target/release/rules-compiler -c config.yaml
```

### .NET API Client + Console UI (`src/adguard-api-dotnet/`)
```bash
cd src/adguard-api-dotnet
dotnet restore src/AdGuard.ApiClient.sln
dotnet build src/AdGuard.ApiClient.sln
dotnet test src/AdGuard.ApiClient.sln
dotnet run --project src/AdGuard.ConsoleUI/AdGuard.ConsoleUI.csproj

# Run benchmarks
dotnet run --project src/AdGuard.ApiClient.Benchmarks -c Release
```

### TypeScript API Client (`src/adguard-api-typescript/`)
```bash
cd src/adguard-api-typescript

# Deno tasks
deno task start            # Start CLI
deno task sync             # Sync rules
deno task cli              # Run CLI directly
deno task test             # Run tests
deno task test:coverage    # Run tests with coverage
deno task lint             # Lint source files
deno task fmt              # Format source files
deno task check            # Type check
```

### Linear Import Tool (`src/linear/`)
```bash
cd src/linear

# Deno tasks
deno task import           # Run import tool
deno task import:docs      # Import documentation
deno task import:dry-run   # Preview import
deno task cli              # Run CLI directly
deno task test             # Run tests
deno task lint             # Lint source files
deno task fmt              # Format source files
deno task check            # Type check
```

### Gatsby Website (`src/website/`)
```bash
cd src/website
npm ci
npm run develop    # Dev server at localhost:8000
npm run build      # Production build
npm run serve      # Serve local build
```

### PowerShell RulesCompiler Module (`src/adguard-api-powershell/`)
```powershell
# Import the module
Import-Module ./src/adguard-api-powershell/Invoke-RulesCompiler.psm1

# Check versions and platform info
Get-CompilerVersion | Format-List

# Compile filter rules
Invoke-RulesCompiler

# Compile and copy to rules directory
Invoke-RulesCompiler -CopyToRules

# Run interactive harness
./src/adguard-api-powershell/RulesCompiler-Harness.ps1

# Run Pester tests
Invoke-Pester -Path ./src/adguard-api-powershell/Tests/RulesCompiler-Tests.ps1

# Lint with PSScriptAnalyzer
Invoke-ScriptAnalyzer -Path src/adguard-api-powershell -Recurse
```

## Running Individual Tests

### TypeScript (Deno)
```bash
cd src/rules-compiler-typescript
deno test src/cli.test.ts                  # By file
deno test --filter "parseArgs"             # By test name
deno task test:coverage                    # With coverage
```

### .NET (xUnit)
```bash
cd src/adguard-api-dotnet
dotnet test src/AdGuard.ApiClient.sln --filter "FullyQualifiedName~DevicesApiTests"   # By class
dotnet test src/AdGuard.ApiClient.sln --filter "Name~GetAccountLimits"                # By method

cd ../rules-compiler-dotnet
dotnet test RulesCompiler.slnx --filter "FullyQualifiedName~ConfigurationValidatorTests"
dotnet test RulesCompiler.slnx --filter "FullyQualifiedName~TransformationTests"
```

### PowerShell (Pester)
```powershell
# Run all PowerShell tests
Invoke-Pester -Path ./src/adguard-api-powershell/Tests/

# Run specific test file
Invoke-Pester -Path ./src/adguard-api-powershell/Tests/RulesCompiler-Tests.ps1

# Run with detailed output
Invoke-Pester -Path ./src/adguard-api-powershell/Tests/ -Output Detailed
```

### Python (pytest)
```bash
cd src/rules-compiler-python
pytest                                    # Run all tests
pytest -v                                 # Verbose output
pytest tests/test_config.py               # Specific file
pytest -k "test_read_yaml"                # By test name
pytest --cov=rules_compiler               # With coverage
```

### Rust (cargo test)
```bash
cd src/rules-compiler-rust
cargo test                                # Run all tests
cargo test -- --nocapture                 # With output
cargo test test_count_rules               # Specific test
cargo test config::                       # Tests in module
```

## Architecture

### Filter Rules (`rules/`)
- `rules/adguard_user_filter.txt` - Main tracked filter list consumed by AdGuard DNS

### Rules Compiler - TypeScript (`src/rules-compiler-typescript/`)
- TypeScript wrapper around @adguard/hostlist-compiler
- Deno 2.0+ runtime with npm compatibility
- Supports JSON, YAML, and TOML configuration formats
- `src/cli.ts` - Command-line interface with argument parsing
- `src/config-reader.ts` - Multi-format configuration reader
- `src/compiler.ts` - Core compilation logic
- `src/mod.ts` - Deno entry point
- `frontend-rust/` - Optional Rust CLI frontend
- `deno.json` - Deno configuration and tasks
- Uses Deno's built-in testing framework

### Shell Scripts (`src/rules-compiler-shell/`)
- Cross-platform shell scripts for filter compilation
- `compile-rules.sh` - Bash script for Linux/macOS
- `compile-rules.zsh` - Zsh script with native zsh features (zparseopts, EPOCHREALTIME)
- `compile-rules.ps1` - PowerShell Core script (all platforms)
- `compile-rules.cmd` - Windows batch wrapper
- Supports JSON, YAML, TOML via external tools (yq, Python)

### Rules Compiler - .NET (`src/rules-compiler-dotnet/`)
- .NET 10 library wrapping @adguard/hostlist-compiler
- Supports JSON, YAML, and TOML configuration formats
- `RulesCompiler` - Core library with abstractions, models, and services
- `RulesCompiler.Console` - Spectre.Console interactive and CLI frontend
- `RulesCompiler.Tests` - xUnit tests
- Key interfaces: `IRulesCompilerService`, `IConfigurationReader`, `IFilterCompiler`
- Features: Configuration validation, verbose mode, dependency injection

### Rules Compiler - Python (`src/rules-compiler-python/`)
- Python 3.9+ package wrapping @adguard/hostlist-compiler
- Supports JSON, YAML, and TOML configuration formats
- `rules_compiler/config.py` - Multi-format configuration reader
- `rules_compiler/compiler.py` - Core `RulesCompiler` class and `compile_rules()` function
- `rules_compiler/cli.py` - argparse-based CLI
- Install via `pip install -e .` for development
- Key classes: `RulesCompiler`, `CompilerConfiguration`, `CompilerResult`
- Tools: pytest, mypy, ruff

### Rules Compiler - Rust (`src/rules-compiler-rust/`)
- High-performance Rust library and CLI wrapping @adguard/hostlist-compiler
- Supports JSON, YAML, and TOML configuration formats
- `src/config.rs` - Configuration structs and parsing
- `src/compiler.rs` - `RulesCompiler` struct and `compile_rules()` function
- `src/main.rs` - clap-based CLI with argument parsing
- `src/error.rs` - `CompilerError` enum with thiserror
- Single binary distribution with zero runtime dependencies (except hostlist-compiler)
- Key structs: `RulesCompiler`, `CompilerConfiguration`, `CompilerResult`, `VersionInfo`
- LTO optimization enabled for small binary size

### API Client - .NET (`src/adguard-api-dotnet/`)
- Auto-generated from `api/openapi.json` (primary) and `api/openapi.yaml` (optional) - AdGuard DNS API v1.11
- `Helpers/ConfigurationHelper.cs` - Fluent auth, timeouts, user agent
- `Helpers/RetryPolicyHelper.cs` - Polly-based retry for 408/429/5xx
- Uses Newtonsoft.Json and JsonSubTypes
- Benchmarks project for performance testing

### API Client - TypeScript (`src/adguard-api-typescript/`)
- TypeScript SDK for AdGuard DNS API v1.11 with feature parity to .NET version
- Deno 2.0+ runtime with npm compatibility
- `src/client.ts` - Main `AdGuardDnsClient` class with fluent API
- `src/api/` - API endpoint implementations (account, devices, DNS servers, statistics, etc.)
- `src/repositories/` - Higher-level repository pattern abstractions
- `src/cli/` - Interactive CLI with menu-driven interface
- `src/mod.ts` - Deno entry point
- Key classes: `AdGuardDnsClient`, `DeviceRepository`, `DnsServerRepository`, `UserRulesRepository`
- Dependencies (via npm:): axios, commander, inquirer, chalk

### Linear Import Tool (`src/linear/`)
- TypeScript tool for importing documentation into Linear project management
- Deno 2.0+ runtime with npm compatibility
- `src/linear-import.ts` - Main CLI entry point
- `src/mod.ts` - Deno entry point
- `src/parser.ts` - Markdown documentation parser
- `src/linear-client.ts` - Linear API client wrapper
- `src/types.ts` - TypeScript type definitions
- Dependencies (via npm:): @linear/sdk, commander, marked, dotenv

### Console UI (`src/adguard-api-dotnet/src/AdGuard.ConsoleUI/`)
- Spectre.Console menu-driven interface
- `ApiClientFactory` configures SDK from settings or interactive prompt
- Features: Device management, DNS servers, statistics, query logs, filter lists

### PowerShell Modules (`src/adguard-api-powershell/`)
- **RulesCompiler Module** - Cross-platform PowerShell API mirroring TypeScript compiler
  - `Invoke-RulesCompiler.psm1` - Main module with exported functions
  - `RulesCompiler.psd1` - Module manifest
  - `RulesCompiler-Harness.ps1` - Interactive test harness
  - `Tests/` - Pester test suite
  - Functions: `Read-CompilerConfiguration`, `Invoke-FilterCompiler`, `Write-CompiledOutput`, `Invoke-RulesCompiler`, `Get-CompilerVersion`

## Configuration Schema

All compilers support the same @adguard/hostlist-compiler configuration schema:

### Root-Level Properties
| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `name` | string | Yes | Filter list name |
| `description` | string | No | Description |
| `homepage` | string | No | Homepage URL |
| `license` | string | No | License identifier |
| `version` | string | No | Version number |
| `sources` | array | Yes | List of filter sources |
| `transformations` | array | No | Global transformations |
| `inclusions` | array | No | Global include patterns |
| `exclusions` | array | No | Global exclude patterns |

### Source Properties
| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `source` | string | Yes | URL or file path |
| `name` | string | No | Source identifier |
| `type` | string | No | `adblock` or `hosts` |
| `transformations` | array | No | Source-specific transforms |
| `inclusions` | array | No | Source-specific includes |
| `exclusions` | array | No | Source-specific excludes |

### Available Transformations
RemoveComments, Compress, RemoveModifiers, Validate, ValidateAllowIp, Deduplicate, InvertAllow, RemoveEmptyLines, TrimLines, InsertFinalNewLine, ConvertToAscii

## Environment Variables

| Variable | Description |
|----------|-------------|
| `AdGuard:ApiKey` | API credential for console UI (can also prompt interactively) |
| `ADGUARD_API_KEY` | API credential for TypeScript client |
| `ADGUARD_AdGuard__ApiKey` | .NET-compatible API credential format |
| `LINEAR_API_KEY` | For Linear import scripts (`src/linear/`) |
| `LINEAR_TEAM_ID` | Optional Linear team ID |
| `LINEAR_PROJECT_NAME` | Optional Linear project name |
| `DEBUG` | Set to any value to enable debug logging |
| `LOG_LEVEL` | Log level (DEBUG, INFO, WARN, ERROR, SILENT) |
| `LOG_FORMAT` | Set to `json` for structured logging |
| `RULESCOMPILER_config` | Default configuration file path (.NET compiler) |
| `RULESCOMPILER_Logging__LogLevel__Default` | Log level for .NET compiler |

## CI/CD Alignment

GitHub Actions workflows validate:
- `.github/workflows/dotnet.yml` - Builds/tests .NET projects (API client and rules compiler) with .NET 10
- `.github/workflows/typescript.yml` - Deno 2.x for TypeScript projects, Node.js 22 for website
- `.github/workflows/gatsby.yml` - Builds website and deploys to GitHub Pages
- `.github/workflows/security.yml` - Consolidated security scanning (CodeQL, DevSkim, PSScriptAnalyzer)
- `.github/workflows/release.yml` - Builds and publishes release binaries (.NET, Rust, Python)
- `.github/workflows/claude.yml` - Claude AI integration for @claude mentions
- `.github/workflows/claude-code-review.yml` - Automated PR code review

## Prerequisites

| Requirement | Version | Required For |
|-------------|---------|--------------|
| .NET SDK | 10.0+ | .NET compiler, API client |
| Deno | 2.0+ | TypeScript projects (rules compiler, API client, linear) |
| Node.js | 22.x LTS | Website (Gatsby) |
| PowerShell | 7+ | PowerShell scripts |
| Python | 3.9+ | Python compiler |
| Rust | 1.85+ | Rust compiler (install via rustup) |
| hostlist-compiler | Latest | All compilers (`npm install -g @adguard/hostlist-compiler`) |
| Docker | 24.0+ | Container development (optional but recommended) |

## Key File Locations

- **Main filter list**: `rules/adguard_user_filter.txt`
- **Compiler configs**: `src/rules-compiler-*/`
- **Deno configs**: `src/*/deno.json`
- **OpenAPI spec**: `api/openapi.yaml`
- **Docker config**: `Dockerfile.warp`, `docker-compose.yml`, `.dockerignore`
- **Documentation**: `docs/`
- **Environment template**: `.env.example`
