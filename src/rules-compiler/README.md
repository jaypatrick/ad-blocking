# Rules Compiler (.NET)

A .NET 8 library and console application for compiling AdGuard filter rules using `@adguard/hostlist-compiler`.

## Overview

This project provides a C# API and command-line interface that mirrors the functionality of the PowerShell `Invoke-RulesCompiler` module, offering:

- Cross-platform support (Windows, macOS, Linux)
- Multiple configuration formats (JSON, YAML, TOML)
- Integration with AdGuard's hostlist-compiler CLI
- Spectre.Console-based interactive UI

## Projects

| Project | Description |
|---------|-------------|
| `RulesCompiler` | Core class library with abstractions, models, and services |
| `RulesCompiler.Console` | Console application with interactive and CLI modes |
| `RulesCompiler.Tests` | xUnit tests for the library |

## Prerequisites

| Requirement | Version | Notes |
|-------------|---------|-------|
| .NET SDK | 8.0+ | Cross-platform runtime |
| Node.js | 18+ | Required for hostlist-compiler |
| hostlist-compiler | Latest | `npm install -g @adguard/hostlist-compiler` |

## Installation

```bash
cd src/rules-compiler
dotnet restore RulesCompiler.slnx
dotnet build RulesCompiler.slnx
```

## Usage

### Command Line

```bash
# Run interactively
dotnet run --project src/RulesCompiler.Console

# Compile with specific config
dotnet run --project src/RulesCompiler.Console -- --config path/to/config.json

# Compile and copy to rules directory
dotnet run --project src/RulesCompiler.Console -- --config config.yaml --copy

# Show version info
dotnet run --project src/RulesCompiler.Console -- --version
```

### Programmatic API

```csharp
using Microsoft.Extensions.DependencyInjection;
using RulesCompiler.Extensions;
using RulesCompiler.Abstractions;

// Setup DI
var services = new ServiceCollection();
services.AddLogging();
services.AddRulesCompiler();
var provider = services.BuildServiceProvider();

// Use the service
var compiler = provider.GetRequiredService<IRulesCompilerService>();

// Read configuration
var config = await compiler.ReadConfigurationAsync("compiler-config.yaml");
Console.WriteLine($"Name: {config.Name}, Version: {config.Version}");

// Compile rules
var result = await compiler.RunAsync(
    configPath: "compiler-config.json",
    copyToRules: true);

if (result.Success)
{
    Console.WriteLine($"Compiled {result.RuleCount} rules");
    Console.WriteLine($"Output: {result.OutputPath}");
}
```

## Configuration Formats

### JSON (Default)

```json
{
  "name": "My Filter Rules",
  "version": "1.0.0",
  "sources": [
    {
      "name": "Local Rules",
      "source": "./rules.txt",
      "type": "adblock"
    }
  ],
  "transformations": ["Deduplicate", "Validate"]
}
```

### YAML

```yaml
name: My Filter Rules
version: 1.0.0
sources:
  - name: Local Rules
    source: ./rules.txt
    type: adblock
transformations:
  - Deduplicate
  - Validate
```

### TOML

```toml
name = "My Filter Rules"
version = "1.0.0"
transformations = ["Deduplicate", "Validate"]

[[sources]]
name = "Local Rules"
source = "./rules.txt"
type = "adblock"
```

## Library Architecture

### Abstractions

| Interface | Description |
|-----------|-------------|
| `IConfigurationReader` | Reads and parses configuration files |
| `IFilterCompiler` | Compiles filter rules via hostlist-compiler |
| `IOutputWriter` | Handles output file operations |
| `IRulesCompilerService` | Main orchestration service |

### Models

| Model | Description |
|-------|-------------|
| `CompilerConfiguration` | Configuration file model |
| `FilterSource` | Source filter list definition |
| `CompilerResult` | Compilation result with metrics |
| `VersionInfo` | Component version information |
| `ConfigurationFormat` | Enum for JSON/YAML/TOML formats |

### Services

| Service | Description |
|---------|-------------|
| `ConfigurationReader` | Parses JSON, YAML, and TOML configs |
| `FilterCompiler` | Executes hostlist-compiler CLI |
| `OutputWriter` | Copies output, computes hashes, counts rules |
| `RulesCompilerService` | Orchestrates the full pipeline |

## Dependency Injection

Register all services with a single extension method:

```csharp
services.AddRulesCompiler();
```

This registers:
- `CommandHelper`
- `IConfigurationReader` -> `ConfigurationReader`
- `IFilterCompiler` -> `FilterCompiler`
- `IOutputWriter` -> `OutputWriter`
- `IRulesCompilerService` -> `RulesCompilerService`

## Running Tests

```bash
cd src/rules-compiler
dotnet test RulesCompiler.slnx
dotnet test RulesCompiler.slnx --verbosity detailed
```

## Environment Variables

| Variable | Description |
|----------|-------------|
| `RULESCOMPILER_config` | Default configuration file path |
| `RULESCOMPILER_Logging__LogLevel__Default` | Log level (Debug, Information, Warning, Error) |

## Cross-Platform Notes

- Uses `System.Runtime.InteropServices.RuntimeInformation` for platform detection
- Path handling via `Path.Combine` and `Path.GetFullPath`
- UTF-8 encoding for all file operations
- Falls back to `npx` if hostlist-compiler not globally installed

## Project Structure

```
src/rules-compiler/
├── RulesCompiler.slnx              # Solution file
├── README.md                        # This file
└── src/
    ├── RulesCompiler/               # Core library
    │   ├── Abstractions/            # Interfaces
    │   ├── Configuration/           # Config readers
    │   ├── Extensions/              # DI extensions
    │   ├── Helpers/                 # Utility classes
    │   ├── Models/                  # Data models
    │   └── Services/                # Implementation
    ├── RulesCompiler.Console/       # Console app
    │   ├── Helpers/                 # Console helpers
    │   └── Services/                # App services
    └── RulesCompiler.Tests/         # Unit tests
```

## License

GPLv3 - See [LICENSE](../../LICENSE) for details.
