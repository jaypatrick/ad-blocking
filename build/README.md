# Build Scripts Directory

This directory contains build scripts for the ad-blocking repository. All scripts support both **Bash** (Linux/macOS) and **PowerShell** (cross-platform) environments.

## Table of Contents

- [Overview](#overview)
- [Main Build Scripts](#main-build-scripts)
- [Component-Specific Build Scripts](#component-specific-build-scripts)
- [Usage Examples](#usage-examples)
- [Build Profiles](#build-profiles)
- [Prerequisites](#prerequisites)
- [CI/CD Integration](#cicd-integration)
- [Troubleshooting](#troubleshooting)

## Overview

The build system provides two types of scripts:

1. **Main build script** (`build.sh`/`build.ps1`) - Builds all or selected projects
2. **Component-specific scripts** - Builds individual language ecosystems

All scripts:
- Support both debug and release builds
- Provide colored output for better readability
- Include comprehensive error handling
- Exit with proper status codes for CI/CD integration
- Include built-in help documentation

## Main Build Scripts

### `build.sh` / `build.ps1`

The primary build scripts that can build all projects or specific ones.

**Features:**
- Build all projects at once
- Select specific language ecosystems to build
- Choose debug or release profiles
- Parallel builds where applicable
- Comprehensive error reporting

**Bash Usage:**
```bash
./build/build.sh [OPTIONS]

OPTIONS:
    --all               Build all projects (default if no specific project selected)
    --rust              Build Rust projects
    --dotnet            Build .NET projects
    --typescript        Build TypeScript/Deno projects
    --python            Build Python projects
    --debug             Use debug profile (default)
    --release           Use release profile
    -h, --help          Show help message
```

**PowerShell Usage:**
```powershell
.\build\build.ps1 [OPTIONS]

PARAMETERS:
    -All                Build all projects
    -Rust               Build Rust projects
    -DotNet             Build .NET projects
    -TypeScript         Build TypeScript/Deno projects
    -Python             Build Python projects
    -Profile            'debug' (default) or 'release'
```

## Component-Specific Build Scripts

### Rust Projects (`build-rust.sh` / `build-rust.ps1`)

Builds the Rust workspace including:
- AdGuard API Rust SDK
- Rules compiler (Rust implementation)
- Validation library

**Bash Usage:**
```bash
./build/build-rust.sh [--debug|--release]
```

**PowerShell Usage:**
```powershell
.\build\build-rust.ps1 [-Profile {debug|release}]
```

**Output:**
- Debug builds: `target/debug/`
- Release builds: `target/release/`

**Build Time:**
- Debug: ~2-5 minutes (first build)
- Release: ~5-10 minutes (with optimizations)

### .NET Projects (`build-dotnet.sh` / `build-dotnet.ps1`)

Builds .NET 10 projects including:
- AdGuard API Client library
- AdGuard Console UI (Spectre.Console)
- Rules Compiler .NET library and console

**Bash Usage:**
```bash
./build/build-dotnet.sh [--debug|--release]
```

**PowerShell Usage:**
```powershell
.\build\build-dotnet.ps1 [-Configuration {Debug|Release}]
```

**Output:**
- Debug builds: `bin/Debug/net10.0/`
- Release builds: `bin/Release/net10.0/`

**Build Time:**
- Debug: ~30-60 seconds
- Release: ~45-90 seconds

### TypeScript Projects (`build-typescript.sh` / `build-typescript.ps1`)

Builds TypeScript/Deno projects including:
- Rules Compiler (TypeScript/Deno)
- AdGuard API TypeScript Client
- Linear Import Tool

**Bash Usage:**
```bash
./build/build-typescript.sh
```

**PowerShell Usage:**
```powershell
.\build\build-typescript.ps1
```

**Requirements:**
- Deno 2.0 or later

**Build Time:**
- ~20-40 seconds per project

### Python Projects (`build-python.sh` / `build-python.ps1`)

Builds Python projects including:
- Rules Compiler Python implementation

**Bash Usage:**
```bash
./build/build-python.sh
```

**PowerShell Usage:**
```powershell
.\build\build-python.ps1
```

**Requirements:**
- Python 3.9 or later
- pip package manager

**Build Time:**
- ~15-30 seconds

## Usage Examples

### Build Everything

```bash
# Bash - Debug mode (default)
./build/build.sh

# Bash - Release mode
./build/build.sh --all --release

# PowerShell - Debug mode (default)
.\build\build.ps1

# PowerShell - Release mode
.\build\build.ps1 -All -Profile release
```

### Build Specific Projects

```bash
# Build only Rust projects (Bash)
./build/build.sh --rust

# Build only .NET projects (Bash)
./build/build.sh --dotnet --release

# Build Rust and .NET (PowerShell)
.\build\build.ps1 -Rust -DotNet

# Build TypeScript projects (PowerShell)
.\build\build.ps1 -TypeScript
```

### Build Individual Components

```bash
# Build only Rust workspace (Bash)
./build/build-rust.sh --release

# Build only .NET projects (PowerShell)
.\build\build-dotnet.ps1 -Configuration Release

# Build only TypeScript projects (Bash)
./build/build-typescript.sh

# Build only Python projects (PowerShell)
.\build\build-python.ps1
```

### Common Development Workflows

**Quick iteration (debug builds):**
```bash
# Fast debug builds for development
./build/build.sh --rust --dotnet --debug
```

**Preparing for release:**
```bash
# Full optimized release build
./build/build.sh --all --release
```

**CI/CD simulation:**
```bash
# Test each component individually as CI does
./build/build-rust.sh --release
./build/build-dotnet.sh --release
./build/build-typescript.sh
./build/build-python.sh
```

## Build Profiles

### Debug Profile

**Characteristics:**
- Faster compilation
- Includes debug symbols
- No optimizations
- Larger binary sizes
- Better for development and debugging

**When to use:**
- Local development
- Quick iterations
- Debugging issues
- Testing changes

### Release Profile

**Characteristics:**
- Slower compilation (optimizations applied)
- Stripped debug symbols (smaller binaries)
- Full optimizations (LTO, inlining)
- Better runtime performance
- Suitable for production

**When to use:**
- Production builds
- Performance testing
- Final releases
- Benchmarking

## Prerequisites

### Required Tools

| Language/Project | Required Tools | Version |
|-----------------|----------------|---------|
| **Rust** | Rust toolchain (rustc, cargo) | 1.85+ |
| **.NET** | .NET SDK | 10.0+ |
| **TypeScript** | Deno runtime | 2.0+ |
| **Python** | Python and pip | 3.9+ |

### Platform-Specific Requirements

**Linux/macOS:**
- Bash shell
- Standard build tools (gcc, make for native dependencies)
- git

**Windows:**
- PowerShell 7+ (recommended) or Windows PowerShell 5.1
- Visual Studio Build Tools (for .NET native compilation)
- git

### Installation Commands

**Rust:**
```bash
# Install via rustup
curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh
```

**.NET 10:**
```bash
# Linux (Ubuntu/Debian)
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 10.0

# macOS
brew install dotnet@10

# Windows
winget install Microsoft.DotNet.SDK.10
```

**Deno:**
```bash
# Linux/macOS
curl -fsSL https://deno.land/install.sh | sh

# Windows
irm https://deno.land/install.ps1 | iex
```

**Python:**
```bash
# Linux (Ubuntu/Debian)
sudo apt-get install python3 python3-pip

# macOS
brew install python@3.12

# Windows
winget install Python.Python.3.12
```

## CI/CD Integration

### GitHub Actions Example

```yaml
name: Build All Projects
on: [push, pull_request]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
      
      - name: Setup Rust
        uses: actions-rust-lang/setup-rust-toolchain@v1
      
      - name: Setup Deno
        uses: denoland/setup-deno@v1
        with:
          deno-version: v2.x
      
      - name: Build all projects
        run: ./build/build.sh --all --release
```

### Build Status Codes

All build scripts follow standard Unix exit codes:

- `0` - Success (all builds completed)
- `1` - Failure (one or more builds failed)

This allows for proper CI/CD integration:

```bash
# Fail-fast approach
./build/build.sh --all || exit 1

# Continue on error
./build/build.sh --rust || true
./build/build.sh --dotnet || true
```

## Troubleshooting

### Common Issues

**Issue: "command not found: cargo"**
```
Solution: Install Rust toolchain via rustup
```

**Issue: ".NET SDK not found"**
```
Solution: Install .NET 10 SDK
Verify: dotnet --version
```

**Issue: "Deno is not installed"**
```
Solution: Install Deno 2.0+
Verify: deno --version
```

**Issue: "Build failed with exit code 1"**
```
Solution: 
1. Check build output for specific errors
2. Ensure all prerequisites are installed
3. Try building individual components separately
4. Clean build artifacts: cargo clean, dotnet clean
```

### Debug Mode

For verbose output, you can modify the scripts or use:

```bash
# Bash - Enable debug output
bash -x ./build/build.sh --rust

# PowerShell - Enable verbose output
.\build\build.ps1 -Rust -Verbose
```

### Clean Builds

To ensure a clean build state:

```bash
# Rust - Clean workspace
cargo clean

# .NET - Clean all projects
dotnet clean src/adguard-api-dotnet/AdGuard.ApiClient.slnx
dotnet clean src/rules-compiler-dotnet/RulesCompiler.slnx

# Deno - Clear cache
deno cache --reload
```

## Build Artifacts

### Rust Build Artifacts

```
target/
├── debug/              # Debug builds
│   ├── adguard-api-rust
│   ├── rules-compiler
│   └── libadguard_validation.*
└── release/            # Release builds
    ├── adguard-api-rust
    ├── rules-compiler
    └── libadguard_validation.*
```

### .NET Build Artifacts

```
src/adguard-api-dotnet/
├── src/AdGuard.ApiClient/bin/Debug/net10.0/
├── src/AdGuard.ConsoleUI/bin/Debug/net10.0/
└── ...

src/rules-compiler-dotnet/
├── src/RulesCompiler/bin/Debug/net10.0/
├── src/RulesCompiler.Console/bin/Debug/net10.0/
└── ...
```

### TypeScript Build Artifacts

TypeScript/Deno projects use JIT compilation, so no build artifacts are generated. Type checking and validation occur during the build process.

### Python Build Artifacts

Python projects install in development mode, creating:
```
src/rules-compiler-python/
├── rules_compiler.egg-info/
└── build/
```

## Performance Tips

### Parallel Builds

Cargo automatically uses parallel compilation. To control:

```bash
# Limit parallel jobs
cargo build -j 4
```

### Incremental Builds

Most build systems support incremental builds by default:

- **Rust**: Incremental compilation enabled by default in debug mode
- **.NET**: Incremental builds automatic
- **Deno**: Module caching automatic

### Caching

For CI/CD, consider caching:

```yaml
# GitHub Actions - Cache Cargo
- uses: actions/cache@v4
  with:
    path: |
      ~/.cargo/registry
      ~/.cargo/git
      target
    key: ${{ runner.os }}-cargo-${{ hashFiles('**/Cargo.lock') }}
```

## Related Documentation

- [Main README](../README.md) - Project overview
- [CLAUDE.md](../CLAUDE.md) - Developer guide
- [Tools README](../tools/README.md) - Build script tests
- [CI/CD Workflows](../.github/workflows/) - Automated builds

## Support

For build-related issues:
1. Check the script's help text (`--help` or `-h`)
2. Review error messages carefully
3. Verify all prerequisites are installed
4. Check CI workflow logs for examples
5. Open an issue with the complete build output

---

**Last Updated:** 2026-02-04  
**Maintained By:** ad-blocking repository contributors
