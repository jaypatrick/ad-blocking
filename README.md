# Ad-Blocking Repository

Various rules lists, scripts, and other tools used to block nuisances from networks.

## Project Structure

```
ad-blocking/
├── .github/                    # GitHub configuration
│   ├── workflows/              # CI/CD pipelines
│   └── ISSUE_TEMPLATE/         # Issue templates
├── docs/                       # Documentation
│   ├── api/                    # API client documentation
│   └── guides/                 # Usage guides
├── rules/                      # Filter rules
│   ├── adguard_user_filter.txt # Main filter list
│   ├── Api/                    # Rules API utilities
│   └── Config/                 # Rule compilation config
├── scripts/                    # Automation scripts
│   └── powershell/             # PowerShell modules
├── src/                        # Source code
│   ├── adguard-api-client/     # AdGuard DNS API C# client
│   ├── rules-compiler-typescript/ # TypeScript rules compiler
│   ├── rules-compiler-dotnet/  # .NET rules compiler
│   ├── rules-compiler-python/  # Python rules compiler
│   ├── rules-compiler-rust/    # Rust rules compiler
│   └── website/                # Gatsby portfolio website
├── README.md                   # This file
├── SECURITY.md                 # Security policy
└── LICENSE                     # License file
```

## Quick Start

### Prerequisites

**Core Requirements:**
- [Node.js 18+](https://nodejs.org/) - Required for all rules compilers and Gatsby website
- [AdGuard HostList Compiler](https://github.com/AdguardTeam/HostlistCompiler) - For filter compilation: `npm install -g @adguard/hostlist-compiler`

**Language-Specific Requirements:**
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) - For C# API client and .NET rules compiler
- [Python 3.9+](https://www.python.org/) - For Python rules compiler
- [Rust 1.70+](https://rustup.rs/) - For Rust rules compiler
- [PowerShell 7+](https://github.com/PowerShell/PowerShell) - For automation scripts

### Warp Environment

This repository includes a pre-configured Warp environment for development and AI-assisted workflows.

**Environment Details:**
- **Docker Image:** `jaysonknight/warp-env:ad-blocking`
  - .NET 8.0 SDK on Ubuntu Jammy
  - Node.js 20.x LTS
  - PowerShell 7
  - Git
- **Environment ID:** `Egji4sZU4TNIOwNasFU73A`

**Setup Commands (automated):**
```bash
cd ad-blocking/src/rules-compiler-typescript && npm install
cd ad-blocking/src/website && npm install
cd ad-blocking/src/adguard-api-client && dotnet restore
```

**Using with Integrations:**
```bash
# Create a Slack integration
warp integration create slack --environment Egji4sZU4TNIOwNasFU73A

# Create a Linear integration
warp integration create linear --environment Egji4sZU4TNIOwNasFU73A
```

For full repository access (opening PRs, pushing changes), authorize the Warp GitHub app.

### Configuration

1. **Install Dependencies**:
   ```bash
   # Install hostlist-compiler globally
   npm install -g @adguard/hostlist-compiler
   
   # TypeScript compiler dependencies
   cd src/rules-compiler-typescript
   npm install

   # .NET compiler and API client (restore packages)
   cd ../rules-compiler-dotnet
   dotnet restore RulesCompiler.slnx
   
   cd ../adguard-api-client
   dotnet restore AdGuard.ApiClient.slnx

   # Python compiler
   cd ../rules-compiler-python
   pip install -e ".[dev]"

   # Rust compiler
   cd ../rules-compiler-rust
   cargo build

   # Website dependencies (optional)
   cd ../website
   npm install
   ```

### Usage

#### Compile Filter Rules

**TypeScript Compiler:**
```bash
cd src/rules-compiler-typescript
npm run build
npm run compile
```

**.NET Compiler:**
```bash
cd src/rules-compiler-dotnet

# Interactive mode (menu-driven)
dotnet run --project src/RulesCompiler.Console

# CLI mode
dotnet run --project src/RulesCompiler.Console -- --config Config/compiler-config.yaml

# Validate configuration only
dotnet run --project src/RulesCompiler.Console -- --config Config/compiler-config.yaml --validate

# Verbose compilation
dotnet run --project src/RulesCompiler.Console -- --config Config/compiler-config.yaml --verbose
```

**Python Compiler:**
```bash
cd src/rules-compiler-python

# Install
pip install -e .

# Use CLI
rules-compiler -c compiler-config.yaml

# Or use Python API
python -c "from rules_compiler import RulesCompiler; RulesCompiler().compile('compiler-config.yaml')"
```

**Rust Compiler:**
```bash
cd src/rules-compiler-rust

# Build and run
cargo build --release
cargo run --release -- -c compiler-config.yaml

# Or use the built binary
./target/release/rules-compiler -c compiler-config.yaml
```

**PowerShell Harness:**
```bash
cd scripts/powershell
./RulesCompiler-Harness.ps1
```

#### Use the AdGuard API Client

**Console UI:**
```bash
cd src/adguard-api-client/src/AdGuard.ConsoleUI
dotnet run
```

**Programmatic Usage:**
See the [API Client Usage Guide](docs/guides/api-client-usage.md) and [Examples](docs/guides/api-client-examples.md).

### Testing

#### Run All Tests

**TypeScript:**
```bash
cd src/rules-compiler-typescript
npm test
```

**.NET API Client:**
```bash
cd src/adguard-api-client/src
dotnet test AdGuard.ApiClient.slnx
```

**.NET Rules Compiler:**
```bash
cd src/rules-compiler-dotnet
dotnet test RulesCompiler.slnx
```

**Python:**
```bash
cd src/rules-compiler-python
pytest
```

**Rust:**
```bash
cd src/rules-compiler-rust
cargo test
```

**PowerShell:**
```bash
cd scripts/powershell
pwsh -Command "Invoke-ScriptAnalyzer -Path . -Recurse"
```

## Components

### Filter Rules (`rules/`)
The main AdGuard filter list for blocking ads, trackers, and malware. The rules list [can be found here](rules/adguard_user_filter.txt).

### Rules Compilers

#### TypeScript (`src/rules-compiler-typescript/`)
TypeScript-based compiler using [@adguard/hostlist-compiler](https://github.com/AdguardTeam/HostlistCompiler) to compile and transform filter rules. Includes Deno support and optional Rust CLI frontend.

**Features:**
- Node.js 18+ support with cross-runtime compatibility
- Deno support for TypeScript execution
- Optional Rust CLI frontend for performance
- Full hostlist-compiler configuration support

#### .NET (`src/rules-compiler-dotnet/`)
A comprehensive .NET 8 library and console application for compiling filter rules with full [@adguard/hostlist-compiler](https://github.com/AdguardTeam/HostlistCompiler) support.

**Features:**
- Multi-format configuration: JSON, YAML, and TOML support
- Configuration validation with detailed error/warning reporting
- Interactive and CLI modes with rich menu system
- Verbose mode for debugging
- All 11 transformations supported (Deduplicate, Validate, RemoveComments, etc.)
- Cross-platform: Windows, Linux, and macOS

**Key Capabilities:**
- Source-specific transformations and inclusion/exclusion patterns
- Validation-only mode for configuration checking
- Library with dependency injection support
- Comprehensive unit test coverage

See the [.NET Rules Compiler README](src/rules-compiler-dotnet/README.md) for detailed documentation.

#### Python (`src/rules-compiler-python/`)
Python 3.9+ API for compiling filter rules with CLI and programmatic interfaces.

**Features:**
- Command-line interface with multiple options
- Python API for integration into other projects
- JSON, YAML, and TOML configuration support
- Type hints and mypy type checking
- Comprehensive test suite with pytest

See the [Python Rules Compiler README](src/rules-compiler-python/README.md) for detailed documentation.

#### Rust (`src/rules-compiler-rust/`)
Fast, single-binary CLI tool and library for compiling filter rules.

**Features:**
- Zero runtime dependencies (statically linked)
- Fast performance with low memory usage
- Cross-platform support (Windows, macOS, Linux)
- JSON, YAML, and TOML configuration support
- Library for embedding in Rust projects

See the [Rust Rules Compiler README](src/rules-compiler-rust/README.md) for detailed documentation.

### API Client (`src/adguard-api-client/`)
C# SDK for the [AdGuard DNS API v1.11](https://api.adguard-dns.io/static/swagger/swagger.json). Auto-generated from OpenAPI specification with full async support and Polly resilience.

**Features:**
- Full async/await support
- Polly resilience and retry policies
- Dependency injection with ILogger support
- OpenAPI 3.0 generated client
- .NET 8.0 target framework

**Includes AdGuard.ConsoleUI:**
A console-based user interface for managing AdGuard DNS accounts with features including:
- Device and DNS server management
- Query statistics and log viewing
- Filter list management
- Account limits display with visual progress bars
- Rich terminal UI with [Spectre.Console](https://spectreconsole.net/)

See the [API Client README](src/adguard-api-client/README.md) and [ConsoleUI README](src/adguard-api-client/src/AdGuard.ConsoleUI/README.md) for detailed documentation.

### Website (`src/website/`)
Gatsby-based portfolio website deployed to GitHub Pages.

## Project Purpose

The internet is full of nuisances, and this repository helps eradicate them from networks:
1. Ads
2. Trackers
3. Malware

### How do I safeguard my network?

There are plenty of great apps that will help, but there is no one-size-fits-all solution, especially for:
- Smart devices (Echo, HomePod, etc.)
- Smart TVs
- IoT devices without installation capability

### Test Your Ad Blocking

- [AdBlock Tester](https://adblock-tester.com/)
- [AdGuard Tester](https://d3ward.github.io/toolz/adblock.html)

## Documentation

### API Client
- [API Client README](src/adguard-api-client/README.md) - API client overview
- [API Client Usage Guide](docs/guides/api-client-usage.md) - Detailed usage instructions
- [API Client Examples](docs/guides/api-client-examples.md) - Code examples
- [API Reference](docs/api/) - Full API documentation
- [ConsoleUI README](src/adguard-api-client/src/AdGuard.ConsoleUI/README.md) - Console UI documentation

### Rules Compilers
- [.NET Compiler README](src/rules-compiler-dotnet/README.md) - Comprehensive .NET implementation with all features
- [Python Compiler README](src/rules-compiler-python/README.md) - Python implementation
- [Rust Compiler README](src/rules-compiler-rust/README.md) - Rust implementation

### General
- [Copilot Instructions](.github/copilot-instructions.md) - Development guidelines for all languages
- [SECURITY.md](SECURITY.md) - Security policy and vulnerability reporting

## Contributing

Please see [SECURITY.md](SECURITY.md) for security policy and vulnerability reporting.

## License

See [LICENSE](LICENSE) for details.
