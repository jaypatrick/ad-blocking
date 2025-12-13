# Ad-Blocking Repository

[![.NET](https://github.com/jaypatrick/ad-blocking/actions/workflows/dotnet.yml/badge.svg)](https://github.com/jaypatrick/ad-blocking/actions/workflows/dotnet.yml)
[![TypeScript](https://github.com/jaypatrick/ad-blocking/actions/workflows/typescript.yml/badge.svg)](https://github.com/jaypatrick/ad-blocking/actions/workflows/typescript.yml)
[![PowerShell](https://github.com/jaypatrick/ad-blocking/actions/workflows/powershell.yml/badge.svg)](https://github.com/jaypatrick/ad-blocking/actions/workflows/powershell.yml)
[![Gatsby](https://github.com/jaypatrick/ad-blocking/actions/workflows/gatsby.yml/badge.svg)](https://github.com/jaypatrick/ad-blocking/actions/workflows/gatsby.yml)

A comprehensive multi-language toolkit for ad-blocking, network protection, and AdGuard DNS management. Includes filter rule compilers in TypeScript, .NET, Python, and Rust, plus a complete C# SDK for the AdGuard DNS API.

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
- [AdGuard API Client](#adguard-api-client)
- [Console UI](#console-ui)
- [Website](#website)
- [Configuration](#configuration)
- [Testing](#testing)
- [CI/CD](#cicd)
- [Documentation](#documentation)
- [Contributing](#contributing)
- [License](#license)

## Features

- **Multi-Language Rules Compilers**: Compile AdGuard filter rules using your preferred language
  - TypeScript/Node.js with Deno support
  - .NET 8 with library and CLI
  - Python 3.9+ with pip distribution
  - Rust with high-performance single binary
  - Cross-platform shell scripts (Bash, PowerShell, Batch)
- **Full @adguard/hostlist-compiler Support**: All 11 transformations, source-specific configs, pattern matching
- **Multi-Format Configuration**: JSON, YAML, and TOML configuration file support
- **AdGuard DNS API SDK**: Complete C# client for AdGuard DNS API v1.11 with Polly resilience
- **Interactive Console UI**: Spectre.Console-powered menu interface for DNS management
- **Docker Development Environment**: Pre-configured container with all dependencies
- **Comprehensive Testing**: Unit tests for all compilers with CI/CD integration

## Project Structure

```
ad-blocking/
├── .github/                           # GitHub configuration
│   ├── workflows/                     # CI/CD pipelines
│   │   ├── dotnet.yml                 # .NET build and test
│   │   ├── typescript.yml             # TypeScript lint and build
│   │   ├── powershell.yml             # PowerShell linting
│   │   ├── gatsby.yml                 # Website deployment
│   │   ├── codeql.yml                 # Security scanning
│   │   └── devskim.yml                # Security analysis
│   ├── ISSUE_TEMPLATE/                # Issue templates
│   └── copilot-instructions.md        # Development guidelines
├── api/                               # OpenAPI specifications
│   └── openapi.yaml                   # AdGuard DNS API v1.11 spec
├── docs/                              # Documentation
│   ├── api/                           # API reference docs
│   ├── guides/                        # Usage guides
│   └── README.md                      # Documentation index
├── rules/                             # Filter rules
│   ├── adguard_user_filter.txt        # Main filter list
│   ├── Api/                           # Rules API utilities
│   └── Config/                        # Compiler configurations
├── scripts/                           # Automation scripts
│   ├── powershell/                    # PowerShell modules
│   │   ├── Invoke-RulesCompiler.psm1  # Main module
│   │   ├── RulesCompiler.psd1         # Module manifest
│   │   ├── RulesCompiler-Harness.ps1  # Interactive harness
│   │   └── Tests/                     # Pester tests
│   ├── shell/                         # Shell scripts
│   │   ├── compile-rules.sh           # Bash (Linux/macOS)
│   │   ├── compile-rules.ps1          # PowerShell Core
│   │   └── compile-rules.cmd          # Windows batch
│   └── linear/                        # Linear integration
├── src/                               # Source code
│   ├── adguard-api-client/            # AdGuard DNS API C# client
│   ├── rules-compiler-typescript/     # TypeScript rules compiler
│   ├── rules-compiler-dotnet/         # .NET rules compiler
│   ├── rules-compiler-python/         # Python rules compiler
│   ├── rules-compiler-rust/           # Rust rules compiler
│   └── website/                       # Gatsby portfolio website
├── Dockerfile.warp                    # Docker development environment
├── CLAUDE.md                          # AI assistant instructions
├── SECURITY.md                        # Security policy
└── LICENSE                            # License file
```

## Quick Start

### Prerequisites

| Requirement | Version | Required For |
|-------------|---------|--------------|
| [Node.js](https://nodejs.org/) | 18+ | All compilers, Website |
| [hostlist-compiler](https://github.com/AdguardTeam/HostlistCompiler) | Latest | All compilers |
| [.NET SDK](https://dotnet.microsoft.com/download/dotnet/8.0) | 8.0+ | .NET compiler, API client |
| [Python](https://www.python.org/) | 3.9+ | Python compiler |
| [Rust](https://rustup.rs/) | 1.70+ | Rust compiler |
| [PowerShell](https://github.com/PowerShell/PowerShell) | 7+ | PowerShell scripts |

### Install hostlist-compiler

```bash
npm install -g @adguard/hostlist-compiler
```

### Clone and Setup

```bash
git clone https://github.com/jaypatrick/ad-blocking.git
cd ad-blocking

# TypeScript compiler
cd src/rules-compiler-typescript && npm install

# .NET projects
cd ../rules-compiler-dotnet && dotnet restore RulesCompiler.slnx
cd ../adguard-api-client && dotnet restore src/AdGuard.ApiClient.sln

# Python compiler
cd ../rules-compiler-python && pip install -e ".[dev]"

# Rust compiler
cd ../rules-compiler-rust && cargo build --release
```

### Compile Filter Rules (Any Language)

```bash
# TypeScript
cd src/rules-compiler-typescript && npm run compile

# .NET
cd src/rules-compiler-dotnet && dotnet run --project src/RulesCompiler.Console

# Python
cd src/rules-compiler-python && rules-compiler

# Rust
cd src/rules-compiler-rust && cargo run --release

# PowerShell
Import-Module ./scripts/powershell/Invoke-RulesCompiler.psm1
Invoke-RulesCompiler

# Bash
./scripts/shell/compile-rules.sh
```

## Docker Development Environment

A pre-configured Docker environment is available with all dependencies installed.

### Dockerfile.warp

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy

# Includes:
# - .NET 8.0 SDK
# - Node.js 20.x LTS
# - PowerShell 7
# - Git

WORKDIR /workspace
```

### Build and Run

```bash
# Build the image
docker build -f Dockerfile.warp -t ad-blocking-dev .

# Run interactive container
docker run -it -v $(pwd):/workspace ad-blocking-dev

# Inside container, install dependencies
cd /workspace/src/rules-compiler-typescript && npm install
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

# Install
npm install

# Build
npm run build

# Compile rules
npm run compile                     # Default config
npm run compile:yaml                # YAML config
npm run compile:toml                # TOML config

# CLI options
npm run compile -- -c config.yaml   # Specific config
npm run compile -- -r               # Copy to rules/
npm run compile -- -d               # Debug output
npm run compile -- --help           # Show help
npm run compile -- --version        # Show version

# Development
npm run dev                         # Run with ts-node
npm run lint                        # ESLint
npm test                            # Jest tests
npm run test:coverage               # With coverage
```

**Features**:
- Node.js 18+ with cross-runtime compatibility
- Deno support via `deno.json` and `src/mod.ts`
- Optional Rust CLI frontend (`frontend-rust/`)
- ESLint and Jest testing

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

**Location**: `scripts/shell/`

#### Bash (Linux/macOS)

```bash
./scripts/shell/compile-rules.sh                    # Default config
./scripts/shell/compile-rules.sh -c config.yaml     # YAML config
./scripts/shell/compile-rules.sh -c config.yaml -r  # Copy to rules
./scripts/shell/compile-rules.sh -v                 # Show version
```

#### PowerShell Core (Cross-platform)

```powershell
./scripts/shell/compile-rules.ps1
./scripts/shell/compile-rules.ps1 -ConfigPath config.yaml
./scripts/shell/compile-rules.ps1 -ConfigPath config.yaml -CopyToRules
./scripts/shell/compile-rules.ps1 -Version
```

#### Windows Batch

```cmd
scripts\shell\compile-rules.cmd -c config.json -r
```

### PowerShell Module

**Location**: `scripts/powershell/`

```powershell
# Import module
Import-Module ./scripts/powershell/Invoke-RulesCompiler.psm1

# Available functions
Get-CompilerVersion | Format-List           # Version info
Invoke-RulesCompiler                         # Compile rules
Invoke-RulesCompiler -CopyToRules            # Compile and copy

# Interactive harness
./scripts/powershell/RulesCompiler-Harness.ps1

# Run Pester tests
Invoke-Pester -Path ./scripts/powershell/Tests/

# Lint with PSScriptAnalyzer
Invoke-ScriptAnalyzer -Path scripts/powershell -Recurse
```

**Exported Functions**:

| Function | Description |
|----------|-------------|
| `Read-CompilerConfiguration` | Parse config file |
| `Invoke-FilterCompiler` | Run hostlist-compiler |
| `Write-CompiledOutput` | Write output file |
| `Invoke-RulesCompiler` | Full compilation pipeline |
| `Get-CompilerVersion` | Get version info |

## AdGuard API Client

**Location**: `src/adguard-api-client/`

A comprehensive C# SDK for the [AdGuard DNS API v1.11](https://api.adguard-dns.io/static/swagger/swagger.json).

```bash
cd src/adguard-api-client

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
- Full async/await support
- Polly resilience policies (retry on 408/429/5xx)
- Dependency injection with ILogger support
- Fluent configuration helpers

**API Coverage**:

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

**Usage Example**:

```csharp
using AdGuard.ApiClient;
using AdGuard.ApiClient.Helpers;

// Configure client
var config = new Configuration()
    .WithApiKey("your-api-key")
    .WithTimeout(TimeSpan.FromSeconds(30))
    .WithUserAgent("MyApp/1.0");

var apiClient = new ApiClient(config);
var devicesApi = new DevicesApi(apiClient);

// List devices
var devices = await devicesApi.ListDevicesAsync();
foreach (var device in devices)
{
    Console.WriteLine($"{device.Name}: {device.Id}");
}
```

See [API Client Usage Guide](docs/guides/api-client-usage.md) for detailed examples.

## Console UI

**Location**: `src/adguard-api-client/src/AdGuard.ConsoleUI/`

Interactive terminal application for managing AdGuard DNS.

```bash
cd src/adguard-api-client
dotnet run --project src/AdGuard.ConsoleUI
```

**Features**:
- Menu-driven Spectre.Console interface
- Device and DNS server management
- Query statistics and log viewing
- Filter list management
- Account limits with visual progress bars
- API key configuration (environment or interactive)

**Environment Variables**:

| Variable | Description |
|----------|-------------|
| `AdGuard:ApiKey` | API credential (or prompt interactively) |

## Website

**Location**: `src/website/`

Gatsby-based portfolio website deployed to GitHub Pages.

```bash
cd src/website

# Install dependencies
npm install

# Development
npm run develop              # Dev server at localhost:8000

# Production
npm run build                # Build static site
npm run serve                # Serve build locally
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
    source: rules/local.txt
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

### TypeScript

```bash
cd src/rules-compiler-typescript
npm test                            # Run all tests
npx jest cli.test.ts                # Specific file
npx jest -t "should compile"        # By test name
npm run test:coverage               # With coverage
```

### .NET

```bash
cd src/rules-compiler-dotnet
dotnet test RulesCompiler.slnx
dotnet test --filter "FullyQualifiedName~ConfigurationValidatorTests"

cd ../adguard-api-client
dotnet test src/AdGuard.ApiClient.sln
dotnet test --filter "Name~GetAccountLimits"
```

### Python

```bash
cd src/rules-compiler-python
pytest                              # All tests
pytest -v                           # Verbose
pytest tests/test_config.py         # Specific file
pytest -k "test_read_yaml"          # By name
pytest --cov=rules_compiler         # Coverage
```

### Rust

```bash
cd src/rules-compiler-rust
cargo test                          # All tests
cargo test -- --nocapture           # With output
cargo test test_count_rules         # Specific test
cargo test config::                 # Module tests
```

### PowerShell

```powershell
Invoke-Pester -Path ./scripts/powershell/Tests/
Invoke-Pester -Path ./scripts/powershell/Tests/ -Output Detailed
Invoke-ScriptAnalyzer -Path scripts/powershell -Recurse
```

## CI/CD

GitHub Actions workflows:

| Workflow | Description |
|----------|-------------|
| `dotnet.yml` | Build and test .NET projects with .NET 8 |
| `typescript.yml` | TypeScript build, lint, and test |
| `powershell.yml` | PSScriptAnalyzer linting |
| `gatsby.yml` | Build and deploy website to GitHub Pages |
| `codeql.yml` | CodeQL security scanning |
| `devskim.yml` | DevSkim security analysis |
| `claude.yml` | Claude AI integration |
| `claude-code-review.yml` | Automated code review |

## Documentation

### API Reference

- [API Client README](src/adguard-api-client/README.md)
- [API Client Usage Guide](docs/guides/api-client-usage.md)
- [API Client Examples](docs/guides/api-client-examples.md)
- [API Reference](docs/api/)
- [ConsoleUI README](src/adguard-api-client/src/AdGuard.ConsoleUI/README.md)

### Rules Compilers

- [.NET Compiler README](src/rules-compiler-dotnet/README.md)
- [Python Compiler README](src/rules-compiler-python/README.md)
- [Rust Compiler README](src/rules-compiler-rust/README.md)
- [Shell Scripts README](scripts/shell/README.md)

### Development

- [Copilot Instructions](.github/copilot-instructions.md)
- [Claude Instructions](CLAUDE.md)
- [Security Policy](SECURITY.md)

### Test Your Ad Blocking

- [AdBlock Tester](https://adblock-tester.com/)
- [AdGuard Tester](https://d3ward.github.io/toolz/adblock.html)

## Environment Variables

| Variable | Description |
|----------|-------------|
| `AdGuard:ApiKey` | AdGuard DNS API credential |
| `LINEAR_API_KEY` | Linear integration |
| `DEBUG` | Enable debug logging |
| `RULESCOMPILER_config` | Default config path (.NET) |

## Contributing

Please see [SECURITY.md](SECURITY.md) for security policy and vulnerability reporting.

## License

See [LICENSE](LICENSE) for details.
