# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This repository houses multiple sub-projects for ad-blocking, network protection, and AdGuard DNS management:
- **Rules Compiler** (TypeScript) - Compiles filter rules using @adguard/hostlist-compiler with JSON/YAML/TOML support, Deno support, and optional Rust frontend
- **Rules Compiler** (C#/.NET 8) - .NET library and console app for filter rule compilation
- **Rules Compiler** (Python) - Python package with pip distribution for filter rule compilation
- **Rules Compiler** (Rust) - High-performance Rust library and CLI for filter rule compilation
- **Shell Scripts** - Cross-platform Bash and PowerShell Core scripts for compilation
- **API Client** (C#/.NET 8) - Auto-generated SDK for AdGuard DNS API v1.11
- **Console UI** (C#/.NET 8) - Spectre.Console menu-driven wrapper for the API client
- **Webhook App** (C#/.NET 8) - Triggers AdGuard DNS webhooks with rate limiting
- **Website** (Gatsby) - Static portfolio site deployed to GitHub Pages
- **PowerShell Modules** - Automation modules including:
  - **RulesCompiler Module** - Cross-platform PowerShell API for filter rule compilation
  - **Webhook Module** - AdGuard DNS webhook automation

## Common Commands

### TypeScript Rules Compiler (`src/rules-compiler-typescript/`)
```bash
cd src/rules-compiler-typescript
npm ci                    # Install dependencies
npm run build             # Build TypeScript
npm test                  # Run Jest tests
npm run test:coverage     # Run tests with coverage
npm run lint              # Lint
npm run compile           # Compile filter rules
npm run compile:yaml      # Compile using YAML config
npm run compile:toml      # Compile using TOML config
npm run dev               # Run with ts-node

# CLI with options
npm run compile -- -c config.yaml -r -d
npm run compile -- --help
npm run compile -- --version
```

### Shell Scripts (`scripts/shell/`)
```bash
# Bash (Linux/macOS)
./scripts/shell/compile-rules.sh                    # Use default config
./scripts/shell/compile-rules.sh -c config.yaml -r  # YAML config, copy to rules
./scripts/shell/compile-rules.sh -v                 # Show version

# PowerShell Core (all platforms)
./scripts/shell/compile-rules.ps1
./scripts/shell/compile-rules.ps1 -ConfigPath config.yaml -CopyToRules
./scripts/shell/compile-rules.ps1 -Version

# Windows Batch
scripts\shell\compile-rules.cmd -c config.json -r
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

### .NET API Client + Console UI (`src/api-client/`)
```bash
cd src/api-client
dotnet restore AdGuard.ApiClient.sln
dotnet build AdGuard.ApiClient.sln
dotnet test AdGuard.ApiClient.sln
dotnet run --project src/AdGuard.ConsoleUI/AdGuard.ConsoleUI.csproj
```

### .NET Webhook App (`src/webhook-app/`)
```bash
cd src/webhook-app
dotnet restore Code.sln
dotnet build Code.sln
dotnet test Code.sln
dotnet run    # Requires ADGUARD_WEBHOOK_URL env var
```

### Gatsby Website (`src/website/`)
```bash
cd src/website
npm ci
npm run develop    # Dev server
npm run build      # Production build
npm run serve      # Serve local build
```

### PowerShell RulesCompiler Module (`scripts/powershell/`)
```powershell
# Import the module
Import-Module ./scripts/powershell/Invoke-RulesCompiler.psm1

# Check versions and platform info
Get-CompilerVersion | Format-List

# Compile filter rules
Invoke-RulesCompiler

# Compile and copy to rules directory
Invoke-RulesCompiler -CopyToRules

# Run interactive harness
./scripts/powershell/RulesCompiler-Harness.ps1

# Run Pester tests
Invoke-Pester -Path ./scripts/powershell/Tests/RulesCompiler-Tests.ps1

# Lint with PSScriptAnalyzer
Invoke-ScriptAnalyzer -Path scripts/powershell -Recurse
```

## Running Individual Tests

### TypeScript (Jest)
```bash
cd src/rules-compiler-typescript
npx jest cli.test.ts                       # By file
npx jest -t "should compile rules"         # By test name
```

### .NET (xUnit)
```bash
cd src/api-client
dotnet test AdGuard.ApiClient.sln --filter "FullyQualifiedName~DevicesApiTests"   # By class
dotnet test AdGuard.ApiClient.sln --filter "Name~GetAccountLimits"                # By method
```

### PowerShell (Pester)
```powershell
# Run all PowerShell tests
Invoke-Pester -Path ./scripts/powershell/Tests/

# Run specific test file
Invoke-Pester -Path ./scripts/powershell/Tests/RulesCompiler-Tests.ps1

# Run with detailed output
Invoke-Pester -Path ./scripts/powershell/Tests/ -Output Detailed
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
- `rules/Api/cli.ts` - Minimal CLI for compilation
- `rules/Config/compiler-config.json` - Compiler configuration

### Rules Compiler - TypeScript (`src/rules-compiler-typescript/`)
- TypeScript wrapper around @adguard/hostlist-compiler
- Supports JSON, YAML, and TOML configuration formats
- `src/cli.ts` - Command-line interface with argument parsing
- `src/config-reader.ts` - Multi-format configuration reader
- `src/compiler.ts` - Core compilation logic
- `src/mod.ts` - Deno entry point
- `frontend-rust/` - Optional Rust CLI frontend
- Deno support via `deno.json`
- ESLint and Jest for testing

### Shell Scripts (`scripts/shell/`)
- Cross-platform shell scripts for filter compilation
- `compile-rules.sh` - Bash script for Linux/macOS
- `compile-rules.ps1` - PowerShell Core script (all platforms)
- `compile-rules.cmd` - Windows batch wrapper
- Supports JSON, YAML, TOML via external tools (yq, Python)

### Rules Compiler - .NET (`src/rules-compiler-dotnet/`)
- .NET 8 library wrapping @adguard/hostlist-compiler
- Supports JSON, YAML, and TOML configuration formats
- `RulesCompiler` - Core library with abstractions, models, and services
- `RulesCompiler.Console` - Spectre.Console interactive and CLI frontend
- `RulesCompiler.Tests` - xUnit tests
- Key interfaces: `IRulesCompilerService`, `IConfigurationReader`, `IFilterCompiler`

### Rules Compiler - Python (`src/rules-compiler-python/`)
- Python 3.9+ package wrapping @adguard/hostlist-compiler
- Supports JSON, YAML, and TOML configuration formats
- `rules_compiler/config.py` - Multi-format configuration reader
- `rules_compiler/compiler.py` - Core `RulesCompiler` class and `compile_rules()` function
- `rules_compiler/cli.py` - argparse-based CLI
- Install via `pip install -e .` for development
- Key classes: `RulesCompiler`, `CompilerConfiguration`, `CompilerResult`

### Rules Compiler - Rust (`src/rules-compiler-rust/`)
- High-performance Rust library and CLI wrapping @adguard/hostlist-compiler
- Supports JSON, YAML, and TOML configuration formats
- `src/config.rs` - Configuration structs and parsing
- `src/compiler.rs` - `RulesCompiler` struct and `compile_rules()` function
- `src/main.rs` - clap-based CLI with argument parsing
- `src/error.rs` - `CompilerError` enum with thiserror
- Single binary distribution with zero runtime dependencies (except Node.js for hostlist-compiler)
- Key structs: `RulesCompiler`, `CompilerConfiguration`, `CompilerResult`, `VersionInfo`

### API Client (`src/api-client/`)
- Auto-generated from `api/openapi.yaml` (AdGuard DNS API v1.11)
- `Helpers/ConfigurationHelper.cs` - Fluent auth, timeouts, user agent
- `Helpers/RetryPolicyHelper.cs` - Polly-based retry for 408/429/5xx
- Uses Newtonsoft.Json and JsonSubTypes

### Console UI (`src/AdGuard.ConsoleUI/`)
- Spectre.Console menu-driven interface
- `ApiClientFactory` configures SDK from settings or interactive prompt

### Webhook App (`src/webhook-app/`)
- `Program.cs` - Main entry, wires logging + configuration
- `Infrastructure/ClientSideRateLimitedHandler.cs` - Rate limiting via FixedWindowRateLimiter
- `Extensions/HttpResponseMessage.cs` - Structured logging helpers

### PowerShell Modules (`scripts/powershell/`)
- **RulesCompiler Module** - Cross-platform PowerShell API mirroring TypeScript compiler
  - `Invoke-RulesCompiler.psm1` - Main module with exported functions
  - `RulesCompiler.psd1` - Module manifest
  - `RulesCompiler-Harness.ps1` - Interactive test harness
  - Functions: `Read-CompilerConfiguration`, `Invoke-FilterCompiler`, `Write-CompiledOutput`, `Invoke-RulesCompiler`, `Get-CompilerVersion`
- **Webhook Module** - AdGuard DNS webhook automation
  - `Invoke-WebHook.psm1` - Webhook invocation with retry logic
  - `Webhook-Manifest.psd1` - Module manifest

## Environment Variables

- `ADGUARD_WEBHOOK_URL` - Required by webhook app (format: `https://linkip.adguard-dns.com/linkip/<DEVICE_ID>/<AUTH_TOKEN>`)
- `AdGuard:ApiKey` - API credential for console UI (can also prompt interactively)
- `LINEAR_API_KEY` - For Linear import scripts (`scripts/linear/`)
- `DEBUG` - Set to any value to enable debug logging in PowerShell modules

## CI/CD Alignment

GitHub Actions workflows validate:
- `.github/workflows/dotnet.yml` - Builds/tests both .NET solutions with .NET 8
- `.github/workflows/typescript.yml` - Node 20, tsc --noEmit, eslint for rules-compiler-typescript
- `.github/workflows/gatsby.yml` - Builds website and deploys to GitHub Pages
- `.github/workflows/powershell.yml` - PSScriptAnalyzer on PowerShell scripts

## Prerequisites

- .NET 8.0 SDK
- Node.js 18+ (CI uses Node 20)
- PowerShell 7+ (for scripts)
- Python 3.9+ (for Python rules compiler)
- Rust 1.70+ (for Rust rules compiler, install via rustup)
- hostlist-compiler: `npm install -g @adguard/hostlist-compiler`
