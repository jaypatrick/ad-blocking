# Rules Compiler Comparison

This guide helps you choose the right rules compiler for your use case. All compilers produce identical output and support the same configuration schema.

## Quick Comparison

| Feature | TypeScript | .NET | Python | Rust | PowerShell | Shell |
|---------|------------|------|--------|------|------------|-------|
| Language | TypeScript | C# | Python | Rust | PowerShell | Bash/PS1 |
| Runtime | Node.js 18+ | .NET 10 | Python 3.9+ | None | PowerShell 7+ | Bash/PowerShell |
| Config Formats | JSON, YAML, TOML | JSON, YAML, TOML | JSON, YAML, TOML | JSON, YAML, TOML | JSON | JSON, YAML, TOML |
| Library API | No | Yes | Yes | Yes | Yes | No |
| CLI | Yes | Yes | Yes | Yes | Yes | Yes |
| Interactive Mode | No | Yes | No | No | Yes | No |
| Tests | Jest | xUnit | pytest | cargo test | Pester | No |
| Binary Distribution | No | No | No | Yes | No | No |
| Deno Support | Yes | No | No | No | No | No |

## Detailed Comparison

### TypeScript Compiler

**Best for**: Node.js/JavaScript developers, Deno users, CI/CD pipelines

```bash
cd src/rules-compiler-typescript
npm install
npm run compile
```

**Pros**:
- Native JavaScript ecosystem integration
- Deno support for TypeScript execution
- Optional Rust CLI frontend for performance
- Familiar tooling (npm, ESLint, Jest)

**Cons**:
- Requires Node.js runtime
- No library API for programmatic use
- Slower startup than compiled languages

**Features**:
- CLI with argument parsing
- YAML, JSON, TOML configuration
- Debug output mode
- Copy to rules directory option

### .NET Compiler

**Best for**: C# developers, enterprise environments, interactive use

```bash
cd src/rules-compiler-dotnet
dotnet run --project src/RulesCompiler.Console
```

**Pros**:
- Full library with dependency injection
- Interactive menu-driven mode
- Configuration validation before compilation
- Verbose mode for debugging
- Strong typing and comprehensive API

**Cons**:
- Requires .NET 10 runtime
- Larger deployment footprint

**Features**:
- Interactive Spectre.Console UI
- CLI mode with all options
- Configuration validation (`--validate`)
- Verbose output (`--verbose`)
- Library API for embedding

**Library Usage**:

```csharp
using RulesCompiler.Extensions;
using RulesCompiler.Abstractions;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddLogging();
services.AddRulesCompiler();
var provider = services.BuildServiceProvider();

var compiler = provider.GetRequiredService<IRulesCompilerService>();
var result = await compiler.RunAsync(new CompilerOptions
{
    ConfigPath = "config.yaml",
    OutputPath = "output.txt"
});
```

### Python Compiler

**Best for**: Python developers, data scientists, scripting

```bash
cd src/rules-compiler-python
pip install -e .
rules-compiler -c config.yaml
```

**Pros**:
- Easy installation via pip
- Python API for integration
- Type hints for IDE support
- Familiar Python tooling (pytest, mypy, ruff)

**Cons**:
- Requires Python 3.9+ runtime
- Slightly slower than compiled languages

**Features**:
- CLI with argparse
- Python API for programmatic use
- Type annotations
- PyPI-ready packaging

**Library Usage**:

```python
from rules_compiler import RulesCompiler, compile_rules

# Simple function
result = compile_rules("config.yaml")
print(f"Compiled {result.rule_count} rules")

# Class-based
compiler = RulesCompiler()
result = compiler.compile("config.yaml", output_path="output.txt")
```

### Rust Compiler

**Best for**: Performance-critical use, single-binary deployment, systems integration

```bash
cd src/rules-compiler-rust
cargo build --release
./target/release/rules-compiler -c config.yaml
```

**Pros**:
- Single statically-linked binary
- Zero runtime dependencies (except Node.js for hostlist-compiler)
- Fastest startup time
- Small binary size with LTO
- Memory safe

**Cons**:
- Requires Rust toolchain to build
- Less familiar for non-Rust developers

**Features**:
- clap-based CLI
- Library crate for embedding
- All configuration formats
- Release builds with LTO optimization

**Library Usage**:

```rust
use rules_compiler::{RulesCompiler, CompilerConfiguration};

fn main() -> Result<(), Box<dyn std::error::Error>> {
    let compiler = RulesCompiler::new();
    let result = compiler.compile("config.yaml", None)?;
    println!("Compiled {} rules", result.rule_count);
    Ok(())
}
```

### PowerShell Module

**Best for**: Windows administrators, automation scripts, cross-platform PowerShell users

```powershell
Import-Module ./src/adguard-api-powershell/Invoke-RulesCompiler.psm1
Invoke-RulesCompiler -CopyToRules
```

**Pros**:
- Native PowerShell integration
- Cross-platform (Windows, Linux, macOS)
- Interactive harness for testing
- Pester tests included
- Pipeline-friendly output

**Cons**:
- Requires PowerShell 7+
- JSON configuration only (no YAML/TOML)

**Features**:
- Exported module functions
- Interactive test harness
- Version information
- Pipeline support

**Functions**:

```powershell
# Read configuration
$config = Read-CompilerConfiguration -Path "config.json"

# Compile rules
$result = Invoke-FilterCompiler -Config $config

# Write output
Write-CompiledOutput -Content $result.Content -Path "output.txt"

# All-in-one
Invoke-RulesCompiler -CopyToRules

# Get version info
Get-CompilerVersion | Format-List
```

### Shell Scripts

**Best for**: Simple automation, CI/CD, Unix environments

```bash
./src/rules-compiler-shell/compile-rules.sh -c config.yaml -r
```

**Pros**:
- No additional runtime (just Bash or PowerShell)
- Simple and portable
- Easy to customize

**Cons**:
- Limited error handling
- No library API
- Depends on external tools for YAML/TOML

**Scripts**:

| Script | Platform |
|--------|----------|
| `compile-rules.sh` | Linux/macOS (Bash) |
| `compile-rules.ps1` | Cross-platform (PowerShell Core) |
| `compile-rules.cmd` | Windows (Batch) |

## Performance Comparison

| Compiler | Startup Time | Memory Usage | Build Time |
|----------|--------------|--------------|------------|
| TypeScript | Medium | Medium | Fast (npm install) |
| .NET | Medium | Medium | Medium (dotnet restore) |
| Python | Medium | Low | Fast (pip install) |
| Rust | Fast | Low | Slow (cargo build) |
| PowerShell | Fast | Medium | None |
| Shell | Fast | Low | None |

*Note: Actual compilation time depends on hostlist-compiler, which is the same for all.*

## Decision Matrix

### Choose TypeScript if:
- You're already using Node.js
- You want Deno support
- Your team knows JavaScript/TypeScript
- You need CI/CD integration with npm

### Choose .NET if:
- You're in a C#/.NET environment
- You want interactive menu mode
- You need configuration validation
- You want a library with DI support

### Choose Python if:
- You're in a Python environment
- You need a pip-installable package
- You want to integrate with Python scripts
- You need type hints and mypy support

### Choose Rust if:
- You need a single binary deployment
- Performance is critical
- You want zero runtime dependencies
- You're embedding in a Rust application

### Choose PowerShell if:
- You're on Windows
- You need automation scripts
- You want interactive testing
- Your team uses PowerShell

### Choose Shell Scripts if:
- You need simplicity
- You're in a Unix environment
- You want easy customization
- You're setting up CI/CD

## Feature Matrix

| Feature | TypeScript | .NET | Python | Rust |
|---------|:----------:|:----:|:------:|:----:|
| **Configuration** |
| JSON | Yes | Yes | Yes | Yes |
| YAML | Yes | Yes | Yes | Yes |
| TOML | Yes | Yes | Yes | Yes |
| Validation | No | Yes | No | No |
| **CLI** |
| Config file | Yes | Yes | Yes | Yes |
| Output file | Yes | Yes | Yes | Yes |
| Copy to rules | Yes | Yes | Yes | Yes |
| Debug/Verbose | Yes | Yes | Yes | Yes |
| Version | Yes | Yes | Yes | Yes |
| Help | Yes | Yes | Yes | Yes |
| **Advanced** |
| Library API | No | Yes | Yes | Yes |
| Interactive | No | Yes | No | No |
| Tests | Jest | xUnit | pytest | cargo test |
| DI Support | No | Yes | No | No |
| Async | Yes | Yes | No | Planned |

## Migration Between Compilers

All compilers use the same configuration format, so you can:

1. Use the same config file with any compiler
2. Generate output that's identical across compilers
3. Switch compilers without changing configuration

Example workflow:
```bash
# Development with TypeScript
npm run compile -- -c config.yaml -o output.txt

# CI/CD with Rust for speed
./target/release/rules-compiler -c config.yaml -o output.txt

# Automation with PowerShell
Invoke-RulesCompiler -ConfigPath config.yaml
```
