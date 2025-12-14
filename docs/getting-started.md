# Getting Started

This guide will help you get up and running with the ad-blocking toolkit.

## Prerequisites

### Core Requirements

| Requirement | Version | Purpose | Installation |
|-------------|---------|---------|--------------|
| Node.js | 18+ | All compilers, Website | [nodejs.org](https://nodejs.org/) |
| hostlist-compiler | Latest | Filter compilation | `npm install -g @adguard/hostlist-compiler` |

### Language-Specific Requirements

| Language | Requirement | Version | Installation |
|----------|-------------|---------|--------------|
| .NET | .NET SDK | 8.0+ | [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/8.0) |
| Python | Python | 3.9+ | [python.org](https://www.python.org/) |
| Rust | Rust | 1.70+ | [rustup.rs](https://rustup.rs/) |
| PowerShell | PowerShell | 7+ | [GitHub](https://github.com/PowerShell/PowerShell) |

## Quick Installation

### 1. Clone the Repository

```bash
git clone https://github.com/jaypatrick/ad-blocking.git
cd ad-blocking
```

### 2. Install hostlist-compiler

All compilers require the underlying `@adguard/hostlist-compiler` npm package:

```bash
npm install -g @adguard/hostlist-compiler
```

Verify installation:

```bash
hostlist-compiler --version
```

### 3. Choose Your Compiler

Pick the compiler that best fits your workflow:

#### TypeScript (Node.js)

```bash
cd src/rules-compiler-typescript
npm install
npm run build
npm run compile
```

#### .NET

```bash
cd src/rules-compiler-dotnet
dotnet restore RulesCompiler.slnx
dotnet build RulesCompiler.slnx
dotnet run --project src/RulesCompiler.Console
```

#### Python

```bash
cd src/rules-compiler-python
pip install -e .
rules-compiler
```

#### Rust

```bash
cd src/rules-compiler-rust
cargo build --release
./target/release/rules-compiler
```

#### PowerShell

```powershell
Import-Module ./src/adguard-api-powershell/Invoke-RulesCompiler.psm1
Invoke-RulesCompiler
```

#### Bash/Shell

```bash
./src/shell/compile-rules.sh
```

## First Compilation

### 1. Create a Configuration File

Create `my-config.yaml`:

```yaml
name: My Ad-Blocking Filter
description: Custom filter list for blocking ads and trackers

sources:
  - name: EasyList
    source: https://easylist.to/easylist/easylist.txt
    type: adblock
    transformations:
      - Validate
      - RemoveModifiers

  - name: AdGuard Base
    source: https://raw.githubusercontent.com/AdguardTeam/FiltersRegistry/master/filters/filter_2_Base/filter.txt
    type: adblock

transformations:
  - Deduplicate
  - RemoveEmptyLines
  - TrimLines
  - InsertFinalNewLine
```

### 2. Compile Your Filter

```bash
# TypeScript
npm run compile -- -c my-config.yaml -o my-filter.txt

# .NET
dotnet run --project src/RulesCompiler.Console -- -c my-config.yaml -o my-filter.txt

# Python
rules-compiler -c my-config.yaml -o my-filter.txt

# Rust
cargo run -- -c my-config.yaml -o my-filter.txt
```

### 3. Review the Output

Your compiled filter list is now in `my-filter.txt`. You can:

- Upload it to AdGuard DNS as a custom filter
- Use it with other ad-blocking software
- Host it on a web server for subscription

## Using the AdGuard API Client

### 1. Get Your API Key

1. Log in to [AdGuard DNS](https://adguard-dns.io/)
2. Go to Account Settings
3. Generate an API key

### 2. Set Environment Variable

```bash
# Linux/macOS
export AdGuard__ApiKey="your-api-key-here"

# Windows PowerShell
$env:AdGuard__ApiKey = "your-api-key-here"

# Windows CMD
set AdGuard:ApiKey=your-api-key-here
```

### 3. Run the Console UI

```bash
cd src/adguard-api-client
dotnet run --project src/AdGuard.ConsoleUI
```

The interactive menu lets you:
- View account limits
- Manage devices
- View DNS servers
- Check query logs
- View statistics

## Using Docker

For a pre-configured development environment:

```bash
# Build the Docker image
docker build -f Dockerfile.warp -t ad-blocking-dev .

# Run interactively
docker run -it -v $(pwd):/workspace ad-blocking-dev

# Inside the container
cd /workspace/src/rules-compiler-typescript
npm install
npm run compile
```

See [Docker Guide](docker-guide.md) for more details.

## Next Steps

- [Configuration Reference](configuration-reference.md) - Learn all configuration options
- [Compiler Comparison](compiler-comparison.md) - Choose the right compiler for your needs
- [API Client Usage Guide](guides/api-client-usage.md) - Detailed API client documentation
- [API Client Examples](guides/api-client-examples.md) - Code examples

## Common Issues

### hostlist-compiler not found

Make sure Node.js is in your PATH and reinstall:

```bash
npm install -g @adguard/hostlist-compiler
```

### Permission denied on Linux/macOS

Make shell scripts executable:

```bash
chmod +x src/shell/compile-rules.sh
```

### Python package not found

Install in development mode:

```bash
cd src/rules-compiler-python
pip install -e .
```

### Rust build fails

Update Rust to the latest stable:

```bash
rustup update stable
```

## Getting Help

- [GitHub Issues](https://github.com/jaypatrick/ad-blocking/issues)
- [Security Policy](../SECURITY.md)
