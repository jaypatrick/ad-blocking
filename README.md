# Ad-Blocking Repository

[![.NET](https://github.com/jaypatrick/ad-blocking/actions/workflows/dotnet.yml/badge.svg)](https://github.com/jaypatrick/ad-blocking/actions/workflows/dotnet.yml)
[![Rust](https://github.com/jaypatrick/ad-blocking/actions/workflows/rust-clippy.yml/badge.svg)](https://github.com/jaypatrick/ad-blocking/actions/workflows/rust-clippy.yml)
[![Docker](https://github.com/jaypatrick/ad-blocking/actions/workflows/docker-image.yml/badge.svg)](https://github.com/jaypatrick/ad-blocking/actions/workflows/docker-image.yml)
[![TypeScript](https://github.com/jaypatrick/ad-blocking/actions/workflows/typescript.yml/badge.svg)](https://github.com/jaypatrick/ad-blocking/actions/workflows/typescript.yml)
[![Powershell](https://github.com/jaypatrick/ad-blocking/actions/workflows/powershell.yml/badge.svg)](https://github.com/jaypatrick/ad-blocking/actions/workflows/powershell.yml)
[![CodeQL](https://github.com/jaypatrick/ad-blocking/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/jaypatrick/ad-blocking/actions/workflows/github-code-scanning/codeql)
[![Dependabot](https://github.com/jaypatrick/ad-blocking/actions/workflows/dependabot/dependabot-updates/badge.svg)](https://github.com/jaypatrick/ad-blocking/actions/workflows/dependabot/dependabot-updates)
[![Copilot](https://github.com/jaypatrick/ad-blocking/actions/workflows/copilot-pull-request-reviewer/copilot-pull-request-reviewer/badge.svg)](https://github.com/jaypatrick/ad-blocking/actions/workflows/copilot-pull-request-reviewer/copilot-pull-request-reviewer)
[![Binaries](https://github.com/jaypatrick/ad-blocking/actions/workflows/release.yml/badge.svg)](https://github.com/jaypatrick/ad-blocking/actions/workflows/release.yml)

A comprehensive multi-language toolkit for ad-blocking, network protection, and AdGuard DNS management. Features filter rule compilers in **5 languages** (TypeScript, .NET, Python, Rust, PowerShell), plus complete **API SDKs for AdGuard DNS** in both C# and Rust with interactive console interfaces.

## Table of Contents

- [Features](#features)
- [Project Structure](#project-structure)
- [Quick Start](#quick-start)
- [Docker Development Environment](#docker-development-environment)
- [Rules Compilers](#rules-compilers)
  - [TypeScript](#typescript-compiler)
  - [.NET](#net-compiler)
  - [Python](#python-compiler)
  - [Rust](#rust-compiler)
  - [Shell Scripts](#shell-scripts)
  - [PowerShell Module](#powershell-module)
- [AdGuard API Clients](#adguard-api-clients)
  - [C# SDK](#c-sdk)
  - [Rust SDK](#rust-sdk)
- [Console Applications](#console-applications)
  - [.NET Console UI](#net-console-ui)
  - [Rust CLI](#rust-cli)
  - [TypeScript CLI](#typescript-cli)
- [Configuration](#configuration)
- [Testing](#testing)
- [CI/CD](#cicd)
- [Documentation](#documentation)
- [Contributing](#contributing)
- [License](#license)

## Features

### Rules Compilers (5 Languages)

| Language | Runtime | Distribution | Key Features |
|----------|---------|--------------|--------------|
| **TypeScript** | Deno 2.0+ | Deno | Secure by default, built-in TypeScript, optional Rust frontend |
| **C#/.NET** | .NET 10 | NuGet/Binary | Interactive CLI, config validation, DI support |
| **Python** | Python 3.9+ | pip | Type hints, PyPI-ready packaging |
| **Rust** | Native binary | Cargo/Binary | Zero-runtime deps, LTO optimization |
| **PowerShell** | PowerShell 7+ | Module | Pipeline-friendly, Pester tests |

All compilers wrap [@adguard/hostlist-compiler](https://github.com/AdguardTeam/HostlistCompiler) and support:
- **All 11 transformations**: Deduplicate, Validate, RemoveComments, Compress, RemoveModifiers, etc.
- **Multi-format config**: JSON, YAML, and TOML configuration files
- **Source-specific settings**: Per-source transformations, inclusions, exclusions
- **Pattern matching**: Wildcards, regex, file-based patterns

### AdGuard DNS API SDKs

| SDK | Language | Features |
|-----|----------|----------|
| **C# SDK** | .NET 10 | Full async/await, Polly resilience (retry on 408/429/5xx), DI support |
| **TypeScript SDK** | Deno 2.0+ | Full API coverage, repository pattern, retry policies, interactive CLI |
| **Rust SDK** | Rust 2024 | Auto-generated from OpenAPI, Tokio async runtime, single binary |

Both SDKs provide complete coverage of AdGuard DNS API v1.11 including devices, DNS servers, query logs, statistics, filter lists, web services, and dedicated IP management.

### Interactive Console Applications

- **C# Console UI** - Spectre.Console menu-driven interface with rich formatting
- **Rust CLI** - dialoguer-based interactive menus with TOML config persistence

### Additional Features

- **Shell Scripts**: Bash, Zsh, PowerShell Core, and Windows Batch wrappers
- **Docker Environment**: Pre-configured container with .NET 10, Deno, PowerShell 7
- **Comprehensive Testing**: Deno test, xUnit, pytest, cargo test, Pester across all components
- **CI/CD Integration**: GitHub Actions for build, test, security scanning, and releases

## Project Structure

```
ad-blocking/
├── .github/                           # GitHub configuration
│   ├── workflows/                     # CI/CD pipelines
│   │   ├── dotnet.yml                 # .NET build and test
│   │   ├── typescript.yml             # TypeScript lint and build
│   │   ├── powershell.yml             # PowerShell linting
│   │   ├── release.yml                # Build and publish binaries
│   │   ├── codeql.yml                 # CodeQL security scanning
│   │   ├── devskim.yml                # DevSkim security analysis
│   │   └── claude*.yml                # Claude AI integration
│   └── ISSUE_TEMPLATE/                # Issue templates
├── api/                               # OpenAPI specifications
│   ├── openapi.json                   # AdGuard DNS API v1.11 (primary)
│   └── openapi.yaml                   # AdGuard DNS API v1.11 (optional)
├── docs/                              # Documentation
│   ├── api/                           # Auto-generated API reference
│   ├── guides/                        # Usage guides and tutorials
│   ├── getting-started.md             # Quick start guide
│   ├── compiler-comparison.md         # Compiler comparison matrix
│   ├── configuration-reference.md     # Configuration schema reference
│   └── docker-guide.md                # Docker development guide
├── data/                              # Filter rules and data
│   ├── input/                         # Source filter lists (local & remote refs)
│   │   ├── README.md                  # Input directory documentation
│   │   ├── example-custom-rules.txt   # Example local rules
│   │   ├── internet-sources.txt.example # Example remote sources config
│   │   └── .gitignore                 # Ignore large/sensitive files
│   ├── output/                        # Compiled filter output
│   │   └── adguard_user_filter.txt    # Main tracked filter list (adblock format)
│   └── Config/                        # Compiler configurations (optional)
├── src/                               # Source code
│   ├── rules-compiler-typescript/     # TypeScript/Node.js compiler
│   ├── rules-compiler-dotnet/         # C#/.NET 10 compiler
│   ├── rules-compiler-python/         # Python 3.9+ compiler
│   ├── rules-compiler-rust/           # Rust compiler (single binary)
│   ├── rules-compiler-shell/          # Shell scripts
│   │   ├── compile-rules.sh           # Bash (Linux/macOS)
│   │   ├── compile-rules.zsh          # Zsh (macOS/Linux)
│   │   ├── compile-rules.ps1          # PowerShell Core (all platforms)
│   │   └── compile-rules.cmd          # Windows batch wrapper
│   ├── adguard-api-dotnet/            # C# API SDK + Console UI
│   │   ├── src/AdGuard.ApiClient/     # C# SDK library
│   │   ├── src/AdGuard.ConsoleUI/     # Spectre.Console interface
│   │   └── src/AdGuard.ApiClient.Tests/ # xUnit tests
│   ├── adguard-api-rust/              # Rust API SDK + CLI
│   │   ├── adguard-api-lib/           # Rust SDK library
│   │   └── adguard-api-cli/           # Interactive CLI application
│   ├── adguard-api-typescript/        # TypeScript API SDK + CLI
│   │   ├── src/api/                   # API client implementations
│   │   ├── src/cli/                   # Interactive CLI application
│   │   └── tests/                     # Deno test suite
│   ├── adguard-api-powershell/        # PowerShell modules
│   │   ├── Invoke-RulesCompiler.psm1  # Rules compiler module
│   │   ├── RulesCompiler.psd1         # Module manifest
│   │   └── Tests/                     # Pester test suite
│   └── linear/                        # Linear integration scripts
├── Dockerfile.warp                    # Docker dev environment
├── CLAUDE.md                          # AI assistant instructions
├── SECURITY.md                        # Security policy
└── LICENSE                            # GPL-3.0 license
```

## Quick Start

### Prerequisites

| Requirement | Version | Required For |
|-------------|---------|--------------|
| [Deno](https://deno.land/) | 2.0+ | TypeScript compiler, TypeScript API client |
| [hostlist-compiler](https://github.com/AdguardTeam/HostlistCompiler) | Latest | All compilers |
| [.NET SDK](https://dotnet.microsoft.com/download/dotnet/10.0) | 10.0+ | .NET compiler, API client |
| [Python](https://www.python.org/) | 3.9+ | Python compiler |
| [Rust](https://rustup.rs/) | 1.70+ | Rust compiler |
| [PowerShell](https://github.com/PowerShell/PowerShell) | 7+ | PowerShell scripts |

### Install Deno

```bash
# macOS/Linux
curl -fsSL https://deno.land/install.sh | sh

# Windows (PowerShell)
irm https://deno.land/install.ps1 | iex
```

The `@adguard/hostlist-compiler` package is accessed via Deno's npm compatibility.

### Clone and Setup

```bash
git clone https://github.com/jaypatrick/ad-blocking.git
cd ad-blocking

# TypeScript compiler
cd src/rules-compiler-typescript && deno cache src/mod.ts

# .NET projects
cd ../rules-compiler-dotnet && dotnet restore RulesCompiler.slnx
cd ../adguard-api-dotnet && dotnet restore src/AdGuard.ApiClient.sln

# Python compiler
cd ../rules-compiler-python && pip install -e ".[dev]"

# Rust compiler
cd ../rules-compiler-rust && cargo build --release
```

### Compile Filter Rules (Any Language)

```bash
# TypeScript
cd src/rules-compiler-typescript && deno task compile

# .NET
cd src/rules-compiler-dotnet && dotnet run --project src/RulesCompiler.Console

# Python
cd src/rules-compiler-python && rules-compiler

# Rust
cd src/rules-compiler-rust && cargo run --release

# PowerShell
Import-Module ./src/adguard-api-powershell/Invoke-RulesCompiler.psm1
Invoke-RulesCompiler

# Bash
./src/rules-compiler-shell/compile-rules.sh
```

## Docker Development Environment

A pre-configured Docker environment is available with all dependencies installed.

### Dockerfile.warp

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0-noble

# Includes:
# - .NET 10 SDK
# - Deno 2.0+
# - PowerShell 7
# - Git

WORKSPACE /workspace
```

### Build and Run

```bash
# Build the image
docker build -f Dockerfile.warp -t ad-blocking-dev .

# Run interactive container
docker run -it -v $(pwd):/workspace ad-blocking-dev

# Inside container, cache Deno dependencies
cd /workspace/src/rules-compiler-typescript && deno cache src/mod.ts
cd /workspace/src/rules-compiler-dotnet && dotnet restore RulesCompiler.slnx
```

### Warp Environment

For Warp terminal users, a pre-built environment is available:

| Property | Value |
|----------|-------|
| Docker Image | `jaysonknight/warp-env:ad-blocking` |
| Environment ID | `Egji4sZU4TNIOwNasFU73A` |

```bash
# Create Warp integrations
warp integration create slack --environment Egji4sZU4TNIOwNasFU73A
warp integration create linear --environment Egji4sZU4TNIOwNasFU73A
```

## Data Directory Structure

The `data/` directory organizes all filter-related files with a clear separation between inputs and outputs:

### Input Directory (`data/input/`)

Source location for filter rules to be compiled:

- **Local rule files**: Place custom filter lists in adblock or hosts format
  - Examples: `custom-rules.txt`, `company-blocklist.txt`
  - Supports `.txt`, `.hosts` extensions
  - Automatic format detection (adblock vs hosts)

- **Internet source references**: File containing URLs to remote filter lists
  - Create `internet-sources.txt` with one URL per line
  - Example sources: EasyList, StevenBlack hosts, AdGuard filters
  - Lines starting with `#` are comments

**Features:**
- ✅ **Hash verification**: SHA-384 integrity checking for tampering detection
- ✅ **Syntax validation**: Automatic linting before compilation
- ✅ **Multi-format support**: Both adblock and hosts file formats
- ✅ **Remote list fetching**: Download and verify internet sources
- ✅ **Error reporting**: Clear messages with line numbers for invalid rules

**Example structure:**
```
data/input/
├── README.md                    # Documentation
├── custom-rules.txt             # Your custom adblock rules
├── internet-sources.txt         # URLs to remote lists
└── .gitignore                   # Ignore large/sensitive files
```

See [`data/input/README.md`](data/input/README.md) for detailed usage instructions.

### Output Directory (`data/output/`)

Contains the final compiled filter list:

- **`adguard_user_filter.txt`**: Main filter list in **adblock format**
  - Merged from all input sources
  - Deduplicated and validated
  - Ready for use with AdGuard DNS or other blockers
  - Tracked in version control

**Compilation guarantees:**
- ✅ Output is always in adblock syntax (not hosts format)
- ✅ Comments and metadata preserved from sources
- ✅ SHA-384 hash computed for verification
- ✅ Rule count validation

### Compilation Workflow

```
┌─────────────────────────────────────────────────────┐
│ 1. Discover all files in data/input/               │
│    - Scan for .txt, .hosts files                   │
│    - Parse internet-sources.txt                    │
└─────────────────────────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────┐
│ 2. Validate & Hash Check                           │
│    - Syntax validation for each file               │
│    - Compute SHA-384 hashes                        │
│    - Detect tampering/modifications                │
└─────────────────────────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────┐
│ 3. Fetch Internet Sources (if configured)          │
│    - Download remote lists                         │
│    - Verify with hashes                            │
│    - Cache for performance                         │
└─────────────────────────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────┐
│ 4. Compile with @adguard/hostlist-compiler         │
│    - Merge all sources                             │
│    - Apply transformations (dedupe, validate, etc) │
│    - Convert hosts format to adblock if needed     │
└─────────────────────────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────┐
│ 5. Output to data/output/adguard_user_filter.txt   │
│    - Write final adblock-format list               │
│    - Compute output hash                           │
│    - Log statistics (rule count, hash)             │
└─────────────────────────────────────────────────────┘
```

## Rules Compilers

All compilers wrap [@adguard/hostlist-compiler](https://github.com/AdguardTeam/HostlistCompiler) and support:

- **Multi-format config**: JSON, YAML, TOML
- **All 11 transformations**: Deduplicate, Validate, RemoveComments, Compress, etc.
- **Source-specific settings**: Per-source transformations, inclusions, exclusions
- **Pattern matching**: Wildcards, regex, file-based patterns

### TypeScript Compiler

**Location**: `src/rules-compiler-typescript/`

```bash
cd src/rules-compiler-typescript

# Compile rules
deno task compile                   # Default config
deno task compile:yaml              # YAML config
deno task compile:toml              # TOML config

# CLI options
deno task compile -- -c config.yaml # Specific config
deno task compile -- -r             # Copy to data/
deno task compile -- -d             # Debug output
deno task compile -- --help         # Show help
deno task compile -- --version      # Show version

# Interactive console mode
deno task interactive               # Interactive menu

# Development
deno task dev                       # Run with watch mode
deno task lint                      # Deno lint
deno task test                      # Deno tests
deno task test:coverage             # With coverage
```

**Features**:
- Deno 2.0+ runtime with secure-by-default permissions
- Built-in TypeScript support, no build step required
- Optional Rust CLI frontend (`frontend-rust/`)
- Deno native testing and linting

### .NET Compiler

**Location**: `src/rules-compiler-dotnet/`

```bash
cd src/rules-compiler-dotnet

# Build
dotnet restore RulesCompiler.slnx
dotnet build RulesCompiler.slnx

# Interactive mode (menu-driven)
dotnet run --project src/RulesCompiler.Console

# CLI mode
dotnet run --project src/RulesCompiler.Console -- --config config.yaml
dotnet run --project src/RulesCompiler.Console -- -c config.json --copy
dotnet run --project src/RulesCompiler.Console -- -c config.yaml --verbose
dotnet run --project src/RulesCompiler.Console -- -c config.yaml --validate
dotnet run --project src/RulesCompiler.Console -- --version

# Tests
dotnet test RulesCompiler.slnx
```

**CLI Options**:

| Option | Short | Description |
|--------|-------|-------------|
| `--config` | `-c` | Configuration file path |
| `--output` | `-o` | Output file path |
| `--copy` | | Copy to rules directory |
| `--verbose` | | Detailed compiler output |
| `--validate` | | Validate config only |
| `--version` | `-v` | Show version info |

**Features**:
- Interactive Spectre.Console menu
- Configuration validation with error/warning reporting
- Dependency injection support
- Cross-platform (Windows, Linux, macOS)

See [.NET Compiler README](src/rules-compiler-dotnet/README.md) for library usage.

### Python Compiler

**Location**: `src/rules-compiler-python/`

```bash
cd src/rules-compiler-python

# Install
pip install -e .                    # Basic install
pip install -e ".[dev]"             # With dev dependencies

# CLI usage
rules-compiler                      # Default config
rules-compiler -c config.yaml       # Specific config
rules-compiler -c config.json -r    # Compile and copy
rules-compiler -o output.txt        # Custom output
rules-compiler -V                   # Version info
rules-compiler -d                   # Debug output
rules-compiler --help               # Show help

# Tests
pytest                              # Run tests
pytest -v                           # Verbose
pytest --cov=rules_compiler         # Coverage
```

**Features**:
- Python 3.9-3.12 support
- Type hints with mypy checking
- Ruff linting
- PyPI-ready packaging

**Python API**:

```python
from rules_compiler import RulesCompiler, compile_rules

# Simple usage
result = compile_rules("config.yaml")
print(f"Compiled {result.rule_count} rules")

# Class-based usage
compiler = RulesCompiler()
result = compiler.compile("config.yaml", output_path="output.txt")
```

### Rust Compiler

**Location**: `src/rules-compiler-rust/`

```bash
cd src/rules-compiler-rust

# Build
cargo build                         # Debug build
cargo build --release               # Release build (optimized)

# CLI usage
cargo run -- -c config.yaml         # Specific config
cargo run -- -c config.json -r      # Compile and copy
cargo run -- -o output.txt          # Custom output
cargo run -- -V                     # Version info
cargo run -- -d                     # Debug output
cargo run -- --help                 # Show help

# Release binary
./target/release/rules-compiler -c config.yaml

# Tests
cargo test                          # Run tests
cargo test -- --nocapture           # With output
```

**Features**:
- Single statically-linked binary
- LTO optimization for small binary size
- Zero runtime dependencies (except Node.js for hostlist-compiler)
- Cross-platform support

**Rust API**:

```rust
use rules_compiler::{RulesCompiler, CompilerConfiguration};

let compiler = RulesCompiler::new();
let result = compiler.compile("config.yaml", None)?;
println!("Compiled {} rules", result.rule_count);
```

### Shell Scripts

**Location**: `src/rules-compiler-shell/`

Cross-platform shell scripts that wrap `@adguard/hostlist-compiler` for simple automation and CI/CD pipelines.

| Script | Platform | Shell | Features |
|--------|----------|-------|----------|
| `compile-rules.sh` | Linux, macOS | Bash | Full feature support, YAML/TOML via yq/Python |
| `compile-rules.zsh` | Linux, macOS | Zsh | Native zsh features (zparseopts, EPOCHREALTIME) |
| `compile-rules.ps1` | All platforms | PowerShell 7+ | Cross-platform, PowerShell pipeline support |
| `compile-rules.cmd` | Windows | Batch | Simple wrapper for Windows users |

#### Bash (Linux/macOS)

```bash
# Make executable (first time)
chmod +x src/rules-compiler-shell/compile-rules.sh

# Run with defaults
./src/rules-compiler-shell/compile-rules.sh

# Use specific configuration
./src/rules-compiler-shell/compile-rules.sh -c config.yaml

# Compile and copy to rules directory
./src/rules-compiler-shell/compile-rules.sh -c config.yaml -r

# Show version/help
./src/rules-compiler-shell/compile-rules.sh -v
./src/rules-compiler-shell/compile-rules.sh -h
```

#### Zsh (macOS/Linux)

```zsh
# Make executable (first time)
chmod +x src/rules-compiler-shell/compile-rules.zsh

# Run with defaults
./src/rules-compiler-shell/compile-rules.zsh

# Use YAML configuration
./src/rules-compiler-shell/compile-rules.zsh -c config.yaml

# Compile and copy to rules directory
./src/rules-compiler-shell/compile-rules.zsh -c config.yaml -r

# Debug mode
./src/rules-compiler-shell/compile-rules.zsh -c config.yaml -d
```

#### PowerShell Core (Cross-platform)

```powershell
# Run with defaults
./src/rules-compiler-shell/compile-rules.ps1

# Use YAML configuration
./src/rules-compiler-shell/compile-rules.ps1 -ConfigPath config.yaml

# Compile and copy to rules directory
./src/rules-compiler-shell/compile-rules.ps1 -ConfigPath config.yaml -CopyToRules

# Show version
./src/rules-compiler-shell/compile-rules.ps1 -Version
```

#### Windows Batch

```cmd
src\rules-compiler-shell\compile-rules.cmd -c config.json -r
```

**CLI Options** (all scripts):

| Option | Short | Description |
|--------|-------|-------------|
| `--config PATH` | `-c` | Path to configuration file |
| `--output PATH` | `-o` | Path to output file |
| `--copy-to-rules` | `-r` | Copy output to rules directory |
| `--format FORMAT` | `-f` | Force format (json, yaml, toml) |
| `--version` | `-v` | Show version information |
| `--help` | `-h` | Show help message |
| `--debug` | `-d` | Enable debug output |

See [Shell Scripts README](src/rules-compiler-shell/README.md) for detailed documentation.

### PowerShell Modules

**Location**: `src/adguard-api-powershell/`

#### RulesCompiler Module

PowerShell API for compiling AdGuard filter rules.

```powershell
# Import module
Import-Module ./src/adguard-api-powershell/Invoke-RulesCompiler.psm1

# Available functions
Get-CompilerVersion | Format-List           # Version info
Invoke-RulesCompiler                         # Compile rules
Invoke-RulesCompiler -CopyToRules            # Compile and copy

# Interactive harness
./src/adguard-api-powershell/RulesCompiler-Harness.ps1

# Run Pester tests
Invoke-Pester -Path ./src/adguard-api-powershell/Tests/
```

**Exported Functions**:

| Function | Description |
|----------|-------------|
| `Read-CompilerConfiguration` | Parse config file (JSON, YAML, TOML) |
| `Invoke-FilterCompiler` | Run hostlist-compiler |
| `Write-CompiledOutput` | Write output file |
| `Invoke-RulesCompiler` | Full compilation pipeline |
| `Get-CompilerVersion` | Get version info |

#### Webhook Module (v1.0.0 - NEW!)

Modernized PowerShell module for invoking AdGuard DNS webhooks with rich features.

```powershell
# Import module
Import-Module ./src/adguard-api-powershell/Invoke-WebHook.psm1

# Basic invocation
Invoke-AdGuardWebhook -WebhookUrl "https://api.adguard-dns.io/webhook/xxx"

# Continuous mode with statistics
Invoke-AdGuardWebhook -WebhookUrl $url -Continuous -ShowStatistics

# Load from configuration file
Invoke-AdGuardWebhook -ConfigFile webhook-config.json -ShowStatistics

# Save configuration
Invoke-AdGuardWebhook -WebhookUrl $url -SaveConfig config.json

# Quiet mode with JSON output
Invoke-AdGuardWebhook -WebhookUrl $url -Quiet -Format Json
```

**New Features (v1.0.0)**:
- ✅ Rich console output with colored icons and emojis
- 📊 Progress bars for continuous operations
- 📊 Statistics tracking (success/failure rates, elapsed time)
- 💾 Configuration file support (JSON/YAML)
- 💾 Multiple output formats (Table, List, JSON)
- ⚙️ Quiet mode for scripting
- 🎯 Parameter validation with ranges
- 🔙 Backward compatible alias (`Invoke-Webhook`)

**Configuration File Example**:
```json
{
  "WebhookUrl": "https://api.adguard-dns.io/webhook/xxx",
  "WaitTime": 500,
  "RetryCount": 10,
  "RetryInterval": 5,
  "Continuous": false
}
```

#### Utility Scripts (Enhanced)

All utility scripts have been enhanced with modern features:

**Regenerate-Client.ps1** - New options:
- `-DryRun`: Preview changes without modification
- `-Compare`: Show detailed diffs
- `-Clean`: Remove backup files
- `-LogFile`: Persistent logging
- `-OutputFormat`: Text, Json, or Markdown

**Update-ApiClient.ps1** - New options:
- `-Force`: Skip confirmations
- `-CI`: Non-interactive mode
- Step progress indicators with colored icons

See [PowerShell Modules README](src/adguard-api-powershell/README.md) for complete documentation

## AdGuard API Clients

Complete SDK implementations for the [AdGuard DNS API v1.11](https://api.adguard-dns.io/static/swagger/swagger.json) in C#, TypeScript, and Rust.

### C# SDK

**Location**: `src/adguard-api-dotnet/`

```bash
cd src/adguard-api-dotnet

# Build
dotnet restore src/AdGuard.ApiClient.sln
dotnet build src/AdGuard.ApiClient.sln

# Test
dotnet test src/AdGuard.ApiClient.sln

# Run benchmarks
dotnet run --project src/AdGuard.ApiClient.Benchmarks -c Release
```

**Features**:
- Auto-generated from OpenAPI specification
- Full async/await support with cancellation tokens
- Polly resilience policies (automatic retry on 408/429/5xx with exponential backoff)
- Dependency injection with `ILogger` support
- Fluent configuration helpers for easy setup
- Newtonsoft.Json serialization with JsonSubTypes support

**Usage Example**:

```csharp
using AdGuard.ApiClient;
using AdGuard.ApiClient.Helpers;

// Configure client with fluent API
var config = new Configuration()
    .WithApiKey("your-api-key")
    .WithTimeout(TimeSpan.FromSeconds(30))
    .WithUserAgent("MyApp/1.0");

var apiClient = new ApiClient(config);
var devicesApi = new DevicesApi(apiClient);

// List all devices
var devices = await devicesApi.ListDevicesAsync();
foreach (var device in devices)
{
    Console.WriteLine($"{device.Name}: {device.Id}");
}

// Get account limits
var accountApi = new AccountApi(apiClient);
var limits = await accountApi.GetAccountLimitsAsync();
Console.WriteLine($"Devices: {limits.DevicesCount}/{limits.DevicesLimit}");
```

### Rust SDK

**Location**: `src/adguard-api-rust/`

```bash
cd src/adguard-api-rust

# Build
cargo build --release

# Run tests
cargo test
```

**Features**:
- Auto-generated from OpenAPI specification using OpenAPI Generator
- Async/await support with Tokio runtime
- Single statically-linked binary distribution
- Configurable TLS: rustls (default) or native-tls
- Memory-safe with zero-cost abstractions

**Usage Example**:

```rust
use adguard_api_lib::apis::configuration::Configuration;
use adguard_api_lib::apis::devices_api;

#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error>> {
    // Configure the API client
    let mut config = Configuration::new();
    config.base_path = "https://api.adguard-dns.io".to_string();
    config.bearer_access_token = Some("your-api-token".to_string());

    // List devices
    let devices = devices_api::list_devices(&config).await?;
    for device in devices {
        println!("{}: {}", device.name, device.id);
    }

    Ok(())
}
```

### TypeScript SDK

**Location**: `src/adguard-api-typescript/`

```bash
cd src/adguard-api-typescript

# Run tests
deno task test

# Run CLI
deno task start
```

**Features**:
- Auto-generated types from OpenAPI specification
- Repository pattern with high-level abstractions
- Automatic retry with exponential backoff using axios-retry
- Interactive CLI with inquirer prompts
- Deno 2.0+ runtime with secure-by-default permissions
- Full test coverage with Deno test

**Usage Example**:

```typescript
import { AdGuardDnsClient } from './src/index.ts';

// Configure client
const client = AdGuardDnsClient.withApiKey('your-api-key');

// Or from environment variable
const client = AdGuardDnsClient.fromEnv('ADGUARD_API_KEY');

// List all devices
const devices = await client.devices.listDevices();
for (const device of devices) {
    console.log(`${device.name}: ${device.id}`);
}

// Get account limits
const limits = await client.account.getAccountLimits();
console.log(`Devices: ${limits.devices_count}/${limits.devices_limit}`);

// Use repositories for higher-level operations
const stats = await client.statisticsRepository.getSummary();
```

### API Coverage (All SDKs)

| API | Description |
|-----|-------------|
| `AccountApi` | Account limits and information |
| `AuthenticationApi` | OAuth token generation |
| `DevicesApi` | Device CRUD operations |
| `DNSServersApi` | DNS server profile management |
| `DedicatedIPAddressesApi` | Dedicated IPv4 management |
| `FilterListsApi` | Filter list retrieval |
| `QueryLogApi` | Query log operations |
| `StatisticsApi` | DNS query statistics |
| `WebServicesApi` | Web services for blocking |

See [API Client Usage Guide](docs/guides/api-client-usage.md) for detailed C# examples.

## Console Applications

Interactive terminal applications for managing AdGuard DNS.

### .NET Console UI

**Location**: `src/adguard-api-dotnet/src/AdGuard.ConsoleUI/`

```bash
cd src/adguard-api-dotnet
dotnet run --project src/AdGuard.ConsoleUI
```

**Features**:
- Menu-driven Spectre.Console interface with rich formatting
- Device and DNS server management with detailed views
- Query statistics and log viewing with filtering
- Filter list browsing and management
- Account limits with visual progress bars
- API key configuration (environment variable or interactive prompt)

### Rust CLI

**Location**: `src/adguard-api-rust/adguard-api-cli/`

```bash
cd src/adguard-api-rust

# Run directly
cargo run --bin adguard-api-cli

# Or build and run release binary
cargo build --release
./target/release/adguard-api-cli
```

**Features**:
- Interactive menu-driven interface using dialoguer
- Full feature parity with .NET Console UI
- TOML configuration file persistence (`~/.config/adguard-api-cli/config.toml`)
- Single binary distribution with no runtime dependencies
- Cross-platform (Linux, macOS, Windows)

**Configuration**:

```toml
# ~/.config/adguard-api-cli/config.toml
api_url = "https://api.adguard-dns.io"
api_token = "your-api-token-here"
```

Or use environment variables (both .NET-compatible and legacy formats supported by Rust CLI):
```bash
# .NET-compatible format (recommended - works with both C# and Rust)
export ADGUARD_AdGuard__BaseUrl="https://api.adguard-dns.io"
export ADGUARD_AdGuard__ApiKey="your-token-here"

# Legacy format (Rust CLI backward compatibility)
export ADGUARD_API_URL="https://api.adguard-dns.io"
export ADGUARD_API_TOKEN="your-token-here"
```

**Menu Options** (both applications):

| Menu | Description |
|------|-------------|
| Account Info | View account limits and usage statistics |
| Devices | List and view device details |
| DNS Servers | List and view DNS server configurations |
| User Rules | View and manage user rules |
| Query Log | View recent queries with time range filters |
| Statistics | View query statistics (24h, 7d, 30d) |
| Filter Lists | Browse available filter lists |
| Web Services | List blockable web services |
| Dedicated IPs | List and allocate dedicated IPv4 addresses |
| Settings | Configure API key, test connection |

**Environment Variables**:

Both applications now support the same environment variable names for cross-compatibility:

| Variable | Description |
|----------|-------------|
| `ADGUARD_AdGuard__ApiKey` | API credential (recommended - works with both C# and Rust) |
| `ADGUARD_AdGuard__BaseUrl` | API base URL (optional, works with both C# and Rust) |
| `ADGUARD_API_TOKEN` | Legacy API credential (Rust backward compatibility) |
| `ADGUARD_API_URL` | Legacy API base URL (Rust backward compatibility) |

**Note**: The Rust CLI now prioritizes .NET-compatible format (`ADGUARD_AdGuard__ApiKey`) over legacy format (`ADGUARD_API_TOKEN`) for maximum cross-compatibility.

**C# Console UI Configuration Example**:
```bash
# Linux/macOS
export ADGUARD_AdGuard__ApiKey="your-api-key-here"

# Windows PowerShell
$env:ADGUARD_AdGuard__ApiKey="your-api-key-here"
```

### TypeScript CLI

**Location**: `src/adguard-api-typescript/`

```bash
cd src/adguard-api-typescript

# Run interactively
deno task start

# Or with API key
deno task start -- --api-key your-key

# Sync rules from file
deno task start -- sync --file data/output/adguard_user_filter.txt
```

**Features**:
- Interactive menu-driven interface using inquirer and ora
- Full feature parity with .NET Console UI
- Repository pattern with high-level abstractions
- Automatic retry with exponential backoff
- TypeScript types from OpenAPI specification
- Deno's secure-by-default permission model

**Configuration**:

Set environment variable:
```bash
# Linux/macOS
export ADGUARD_API_KEY="your-api-key-here"

# Windows PowerShell
$env:ADGUARD_API_KEY="your-api-key-here"
```

## Configuration

All compilers support the same configuration schema with JSON, YAML, or TOML syntax.

### Configuration Properties

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `name` | string | Yes | Filter list name |
| `description` | string | No | Description |
| `homepage` | string | No | Homepage URL |
| `license` | string | No | License identifier |
| `version` | string | No | Version number |
| `sources` | array | Yes | Filter sources |
| `transformations` | array | No | Global transformations |
| `inclusions` | array | No | Include patterns |
| `exclusions` | array | No | Exclude patterns |

### Source Properties

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `source` | string | Yes | URL or file path |
| `name` | string | No | Source identifier |
| `type` | string | No | `adblock` or `hosts` |
| `transformations` | array | No | Source transformations |
| `inclusions` | array | No | Source include patterns |
| `exclusions` | array | No | Source exclude patterns |

### Available Transformations

| Transformation | Description |
|---------------|-------------|
| `RemoveComments` | Remove comment lines |
| `Compress` | Convert hosts to adblock syntax |
| `RemoveModifiers` | Remove unsupported modifiers |
| `Validate` | Remove dangerous rules |
| `ValidateAllowIp` | Validate with IP rules allowed |
| `Deduplicate` | Remove duplicates |
| `InvertAllow` | Convert exceptions to blocking |
| `RemoveEmptyLines` | Remove blank lines |
| `TrimLines` | Trim whitespace |
| `InsertFinalNewLine` | Add final newline |
| `ConvertToAscii` | Convert IDN to punycode |

### Example Configurations

#### YAML

```yaml
name: My Filter List
description: Custom ad-blocking filter
version: "1.0.0"

sources:
  - name: Local Rules
    source: data/local.txt
    type: adblock

  - name: EasyList
    source: https://easylist.to/easylist/easylist.txt
    transformations:
      - RemoveModifiers
      - Validate

transformations:
  - Deduplicate
  - RemoveEmptyLines
  - InsertFinalNewLine

exclusions:
  - "*.google.com"
  - "/analytics/"
```

#### JSON

```json
{
  "name": "My Filter List",
  "sources": [
    {
      "name": "EasyList",
      "source": "https://easylist.to/easylist/easylist.txt"
    }
  ],
  "transformations": ["Deduplicate", "InsertFinalNewLine"]
}
```

#### TOML

```toml
name = "My Filter List"
transformations = ["Deduplicate", "InsertFinalNewLine"]

[[sources]]
name = "EasyList"
source = "https://easylist.to/easylist/easylist.txt"
```

## Testing

### TypeScript (Deno)

```bash
cd src/rules-compiler-typescript
deno task test                      # Run all tests
deno test src/cli.test.ts           # Specific file
deno task test:coverage             # With coverage
```

### .NET (xUnit)

```bash
# Rules Compiler
cd src/rules-compiler-dotnet
dotnet test RulesCompiler.slnx
dotnet test --filter "FullyQualifiedName~ConfigurationValidatorTests"
dotnet test --filter "FullyQualifiedName~TransformationTests"

# API Client
cd ../adguard-api-dotnet
dotnet test src/AdGuard.ApiClient.sln
dotnet test --filter "FullyQualifiedName~DevicesApiTests"
dotnet test --filter "Name~GetAccountLimits"
```

### Python (pytest)

```bash
cd src/rules-compiler-python
pytest                              # All tests
pytest -v                           # Verbose
pytest tests/test_config.py         # Specific file
pytest -k "test_read_yaml"          # By name
pytest --cov=rules_compiler         # Coverage
```

### Rust (cargo test)

```bash
# Rules Compiler
cd src/rules-compiler-rust
cargo test                          # All tests
cargo test -- --nocapture           # With output
cargo test test_count_rules         # Specific test
cargo test config::                 # Module tests

# API Client
cd ../adguard-api-rust
cargo test                          # All workspace tests
cargo test --package adguard-api-lib    # Library tests only
cargo test --package adguard-api-cli    # CLI tests only
```

### PowerShell (Pester)

```powershell
# Run all tests
Invoke-Pester -Path ./src/adguard-api-powershell/Tests/

# Run with detailed output
Invoke-Pester -Path ./src/adguard-api-powershell/Tests/ -Output Detailed

# Lint with PSScriptAnalyzer
Invoke-ScriptAnalyzer -Path src/adguard-api-powershell -Recurse
```

### All Tests Summary

| Component | Framework | Command |
|-----------|-----------|---------|
| TypeScript Compiler | Deno test | `deno task test` |
| TypeScript API Client | Deno test | `deno task test` |
| .NET Compiler | xUnit | `dotnet test RulesCompiler.slnx` |
| .NET API Client | xUnit | `dotnet test src/AdGuard.ApiClient.sln` |
| Python Compiler | pytest | `pytest` |
| Rust Compiler | cargo test | `cargo test` |
| Rust API Client | cargo test | `cargo test` |
| PowerShell Module | Pester | `Invoke-Pester` |

## CI/CD

GitHub Actions workflows:

| Workflow | Description |
|----------|-------------|
| `dotnet.yml` | Build and test .NET projects with .NET 10 |
| `typescript.yml` | TypeScript build, lint, and test |
| `powershell.yml` | PSScriptAnalyzer linting |
| `release.yml` | Build and publish binaries on version tags |
| `codeql.yml` | CodeQL security scanning |
| `devskim.yml` | DevSkim security analysis |
| `claude.yml` | Claude AI integration |
| `claude-code-review.yml` | Automated code review |

### Releases

The repository automatically builds and publishes binaries when a new version tag is pushed. See the [Release Guide](docs/release-guide.md) for details.

Pre-built binaries are available for:
- **AdGuard.ConsoleUI** (Windows, Linux, macOS)
- **RulesCompiler.Console** (Windows, Linux, macOS)
- **rules-compiler** Rust binary (Windows, Linux, macOS)
- **rules-compiler** Python wheel (cross-platform)

Download the latest release from the [Releases page](https://github.com/jaypatrick/ad-blocking/releases).

## Documentation

### Getting Started

- [Getting Started Guide](docs/getting-started.md) - Quick installation and first compilation
- [Compiler Comparison](docs/compiler-comparison.md) - Choose the right compiler for your needs
- [Configuration Reference](docs/configuration-reference.md) - Complete configuration schema
- [Docker Guide](docs/docker-guide.md) - Development with Docker containers

### API Reference

- [C# API Client README](src/adguard-api-dotnet/README.md)
- [TypeScript API Client README](src/adguard-api-typescript/README.md)
- [Rust API Client README](src/adguard-api-rust/README.md)
- [API Client Usage Guide](docs/guides/api-client-usage.md)
- [API Client Examples](docs/guides/api-client-examples.md)
- [API Reference](docs/api/)
- [Console UI Architecture](docs/guides/consoleui-architecture.md)

### Rules Compilers

- [TypeScript Compiler](src/rules-compiler-typescript/) - Node.js/Deno compiler
- [.NET Compiler README](src/rules-compiler-dotnet/README.md) - C# library and CLI
- [Python Compiler README](src/rules-compiler-python/README.md) - pip-installable package
- [Rust Compiler README](src/rules-compiler-rust/README.md) - Single binary distribution
- [Shell Scripts README](src/rules-compiler-shell/README.md) - Bash, Zsh, PowerShell, Batch
- [PowerShell Module](src/adguard-api-powershell/README.md) - Full-featured PowerShell API

### Development

- [Claude Instructions](CLAUDE.md) - AI assistant development guidelines
- [Security Policy](SECURITY.md) - Vulnerability reporting
- [Release Guide](docs/release-guide.md) - Release process and binary publishing
- [Centralized Package Management](docs/centralized-package-management.md) - NuGet package management

### Test Your Ad Blocking

- [AdBlock Tester](https://adblock-tester.com/)
- [AdGuard Tester](https://d3ward.github.io/toolz/adblock.html)

## Environment Variables

### API Clients

Both C# and Rust implementations now support the same environment variable names:

| Variable | Description |
|----------|-------------|
| `ADGUARD_AdGuard__ApiKey` | AdGuard DNS API credential (recommended - works with both C# and Rust) |
| `ADGUARD_AdGuard__BaseUrl` | API base URL (optional, works with both C# and Rust) |
| `ADGUARD_API_TOKEN` | Legacy API credential (Rust backward compatibility only) |
| `ADGUARD_API_URL` | Legacy API base URL (Rust backward compatibility only) |

**Cross-Compatibility**: You can now use the same environment variables across both C# Console UI and Rust CLI implementations. The Rust CLI prioritizes the .NET-compatible format (`ADGUARD_AdGuard__ApiKey`) over legacy format (`ADGUARD_API_TOKEN`).

**Note for .NET format**: The `ADGUARD_` prefix is required, and double underscore (`__`) represents colon (`:`) in configuration keys. Example: `ADGUARD_AdGuard__ApiKey` maps to `AdGuard:ApiKey` in configuration.

### Rules Compilers

| Variable | Application | Description |
|----------|-------------|-------------|
| `DEBUG` | All compilers | Enable debug logging |
| `RULESCOMPILER_config` | .NET compiler | Default config file path |
| `RULESCOMPILER_Logging__LogLevel__Default` | .NET compiler | Log level (Debug, Information, Warning, Error) |

### Other

| Variable | Description |
|----------|-------------|
| `LINEAR_API_KEY` | Linear integration scripts |

## Contributing

Please see [SECURITY.md](SECURITY.md) for security policy and vulnerability reporting.

## License

See [LICENSE](LICENSE) for details.

