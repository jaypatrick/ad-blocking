# PowerShell Modules for AdGuard

Modern, object-oriented PowerShell modules for AdGuard filter compilation and webhook management.

## Overview

This directory contains PowerShell modules designed with object-oriented principles, providing a rich API for AdGuard operations across Windows, macOS, and Linux.

## Directory Structure

```
powershell-modules/
├── RulesCompiler/          # Filter rules compilation module
│   ├── Classes/            # PowerShell class definitions
│   ├── Public/             # Exported public functions
│   ├── Private/            # Internal helper functions
│   ├── Tests/              # Pester tests
│   └── RulesCompiler.psd1  # Module manifest
├── AdGuardWebhook/         # Webhook invocation module
│   ├── Classes/            # PowerShell class definitions
│   ├── Public/             # Exported public functions
│   ├── Private/            # Internal helper functions
│   ├── Tests/              # Pester tests
│   └── AdGuardWebhook.psd1 # Module manifest
└── Scripts/                # Standalone utility scripts
```

## Available Modules

### RulesCompiler
Compiles AdGuard filter rules using the `@adguard/hostlist-compiler` CLI.

**Location**: Currently at `src/adguard-api-powershell/Invoke-RulesCompiler.psm1` (to be migrated)

**Features**:
- Cross-platform support (Windows, macOS, Linux)
- Multiple config formats (JSON, YAML, TOML)
- Environment variable configuration
- Pipeline support
- Comprehensive error handling
- Statistics and reporting

**Key Functions**:
- `Invoke-RulesCompiler` - Main compilation function
- `Read-CompilerConfiguration` - Read config files
- `Invoke-FilterCompiler` - Execute compiler
- `Write-CompiledOutput` - Write results
- `Get-CompilerVersion` - Version information

### AdGuardWebhook
Invokes AdGuard DNS webhook endpoints for dynamic IP updates.

**Location**: Currently at `src/adguard-api-powershell/Invoke-WebHook.psm1` (to be migrated)

**Features**:
- Automatic retry logic
- Continuous operation mode
- Statistics tracking
- Multiple output formats (Table, List, JSON)
- Configuration file support
- Visual progress indicators
- Environment variable configuration

**Key Functions**:
- `Invoke-AdGuardWebhook` - Invoke webhook endpoint
- Backward-compatible alias: `Invoke-WebHook`

## Environment Variables

Both modules support comprehensive environment variable configuration:

### RulesCompiler Module

| Variable | Description | Default |
|----------|-------------|---------|
| `ADGUARD_COMPILER_CONFIG` | Configuration file path | `compiler-config.json` |
| `ADGUARD_COMPILER_OUTPUT` | Output file path | `adguard_user_filter.txt` |
| `ADGUARD_COMPILER_RULES_DIR` | Rules directory | `../../data` |
| `ADGUARD_COMPILER_VERBOSE` | Enable verbose mode | `false` |
| `ADGUARD_COMPILER_COPY_TO_RULES` | Auto-copy to rules | `false` |
| `DEBUG` | Enable debug logging | `false` |

### AdGuardWebhook Module

| Variable | Description | Default |
|----------|-------------|---------|
| `ADGUARD_WEBHOOK_URL` | Webhook endpoint URL | *(required)* |
| `ADGUARD_WEBHOOK_CONFIG` | Configuration file path | None |
| `ADGUARD_WEBHOOK_WAIT_TIME` | Wait time between calls (ms) | `200` |
| `ADGUARD_WEBHOOK_RETRY_COUNT` | Number of retry attempts | `10` |
| `ADGUARD_WEBHOOK_RETRY_INTERVAL` | Retry delay (seconds) | `5` |
| `ADGUARD_WEBHOOK_FORMAT` | Output format (Table/List/Json) | `Table` |

### Priority Order
```
CLI Parameters > Config Files > Environment Variables > Defaults
```

## Installation

### Prerequisites
- PowerShell 7.0 or later (cross-platform)
- Node.js 18+ 
- `@adguard/hostlist-compiler`: `npm install -g @adguard/hostlist-compiler`
- Pester 5.0+ for running tests: `Install-Module -Name Pester -Force`

### Import Modules

```powershell
# Import RulesCompiler module
Import-Module ./src/adguard-api-powershell/Invoke-RulesCompiler.psm1

# Import Webhook module
Import-Module ./src/adguard-api-powershell/Invoke-WebHook.psm1
```

## Usage Examples

### RulesCompiler Module

#### Basic Compilation
```powershell
# Use default configuration
Invoke-RulesCompiler

# With custom config
Invoke-RulesCompiler -ConfigPath "config.yaml"

# Compile and copy to rules directory
Invoke-RulesCompiler -ConfigPath "config.json" -CopyToRules
```

#### With Environment Variables
```powershell
# Set environment variables
$env:ADGUARD_COMPILER_CONFIG = "production-config.yaml"
$env:ADGUARD_COMPILER_COPY_TO_RULES = "true"
$env:ADGUARD_COMPILER_VERBOSE = "1"

# Run compilation
Invoke-RulesCompiler
```

#### Pipeline Usage
```powershell
# Get configuration, compile, and write output
Read-CompilerConfiguration -ConfigPath "config.yaml" |
    Invoke-FilterCompiler |
    Write-CompiledOutput -DestinationPath "../rules/filter.txt"
```

#### Check Version Information
```powershell
Get-CompilerVersion | Format-List
```

### AdGuardWebhook Module

#### Basic Webhook Invocation
```powershell
# Single invocation
Invoke-AdGuardWebhook -WebhookUrl "https://example.com/webhook"

# With statistics
Invoke-AdGuardWebhook -WebhookUrl "https://example.com/webhook" -ShowStatistics

# Continuous mode
Invoke-AdGuardWebhook -WebhookUrl "https://example.com/webhook" -Continuous
```

#### Using Configuration File
```powershell
# Create config file
@{
    WebhookUrl = "https://example.com/webhook"
    WaitTime = 300
    RetryCount = 15
    RetryInterval = 10
} | ConvertTo-Json | Set-Content "webhook-config.json"

# Use config file
Invoke-AdGuardWebhook -ConfigFile "webhook-config.json"
```

#### With Environment Variables
```powershell
# Set environment variables
$env:ADGUARD_WEBHOOK_URL = "https://example.com/webhook"
$env:ADGUARD_WEBHOOK_WAIT_TIME = "500"
$env:ADGUARD_WEBHOOK_RETRY_COUNT = "20"

# Run webhook
Invoke-AdGuardWebhook -ShowStatistics -Format Json
```

#### Advanced Options
```powershell
# Custom retry behavior
Invoke-AdGuardWebhook `
    -WebhookUrl "https://example.com/webhook" `
    -WaitTime 400 `
    -RetryCount 20 `
    -RetryInterval 3 `
    -ShowStatistics `
    -Format List

# Save configuration
Invoke-AdGuardWebhook `
    -WebhookUrl "https://example.com/webhook" `
    -SaveConfig "my-webhook.json"

# Quiet mode (no output except errors)
Invoke-AdGuardWebhook `
    -WebhookUrl "https://example.com/webhook" `
    -Quiet
```

## Object-Oriented Design (Phase 3)

The modules are being refactored to use PowerShell classes for better encapsulation and reusability:

### Planned Classes

#### RulesCompiler Module
- `CompilerConfiguration` - Configuration management
- `CompilerResult` - Compilation results
- `CompilerLogger` - Logging functionality

#### AdGuardWebhook Module
- `WebhookConfiguration` - Webhook settings
- `WebhookStatistics` - Statistics tracking
- `WebhookInvoker` - Core webhook functionality

### Class Example
```powershell
# Using CompilerConfiguration class
$config = [CompilerConfiguration]::new()
$config.LoadFromEnvironment()
$config.Validate()

# Or load from file
$config = [CompilerConfiguration]::new("config.yaml")
```

## Testing

### Run All Tests
```powershell
# Run all Pester tests
Invoke-Pester -Path ./src/adguard-api-powershell/Tests/

# Run specific test file
Invoke-Pester -Path ./src/adguard-api-powershell/Tests/EnvironmentVariables-RulesCompiler.Tests.ps1

# Run with coverage
Invoke-Pester -Path ./src/adguard-api-powershell/Tests/ -CodeCoverage ./src/adguard-api-powershell/*.psm1
```

### Test Files
- `Webhook-Tests.ps1` - Webhook module tests
- `RulesCompiler-Tests.ps1` - RulesCompiler module tests
- `EnvironmentVariables-RulesCompiler.Tests.ps1` - Env var tests for compiler
- `EnvironmentVariables-Webhook.Tests.ps1` - Env var tests for webhook

## Code Quality

### PSScriptAnalyzer
```powershell
# Install PSScriptAnalyzer
Install-Module -Name PSScriptAnalyzer -Force

# Run analysis
Invoke-ScriptAnalyzer -Path ./src/adguard-api-powershell/ -Recurse

# Run with specific severity
Invoke-ScriptAnalyzer -Path ./src/adguard-api-powershell/ -Severity Error,Warning
```

## Cross-Platform Support

All modules are designed to work across platforms:

| Platform | Status | Notes |
|----------|--------|-------|
| Windows 10/11 | ✅ Excellent | Native support |
| macOS 10.15+ | ✅ Excellent | PowerShell 7+ required |
| Linux (Ubuntu) | ✅ Excellent | PowerShell 7+ required |
| Linux (Debian) | ✅ Excellent | PowerShell 7+ required |
| Linux (RHEL/Fedora) | ✅ Good | PowerShell 7+ required |

### Platform-Specific Considerations
- Path handling uses `Join-Path` and `[System.IO.Path]` for compatibility
- Command detection works across platforms
- UTF-8 encoding used consistently
- Platform detection built-in

## Best Practices

### Module Development
1. Use approved PowerShell verbs (`Get-`, `Set-`, `Invoke-`, etc.)
2. Include comment-based help for all functions
3. Support pipeline input where appropriate
4. Use `[CmdletBinding()]` for advanced function features
5. Implement `-WhatIf` and `-Confirm` for destructive operations
6. Include comprehensive error handling

### Environment Variables
1. Always check CLI parameters first
2. Fall back to environment variables if not specified
3. Provide sensible defaults
4. Document all supported variables
5. Use consistent naming conventions

### Testing
1. Write tests for all public functions
2. Mock external dependencies
3. Test environment variable handling
4. Test error conditions
5. Maintain >80% code coverage

## Migration Guide

If migrating from old script locations:

```powershell
# Old import
Import-Module ./src/rules-compiler-powershell/invoke-compiler.psm1

# New import
Import-Module ./src/powershell-modules/RulesCompiler/RulesCompiler.psd1
```

## Troubleshooting

### Module not found
```powershell
# Check PowerShell version
$PSVersionTable.PSVersion

# Verify module path
$env:PSModulePath -split [IO.Path]::PathSeparator

# Import with full path
Import-Module "D:\source\ad-blocking\src\adguard-api-powershell\Invoke-RulesCompiler.psm1" -Force
```

### Environment variables not working
```powershell
# List all ADGUARD_* variables
Get-ChildItem env:ADGUARD_*

# Set temporarily
$env:ADGUARD_COMPILER_CONFIG = "config.yaml"

# Set permanently (Windows)
[System.Environment]::SetEnvironmentVariable('ADGUARD_COMPILER_CONFIG', 'config.yaml', 'User')
```

### Permission errors
```powershell
# Set execution policy (Windows)
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

# macOS/Linux - no execution policy needed
```

## Related Documentation

- [Environment Variables Reference](../../docs/ENVIRONMENT_VARIABLES.md)
- [Shell Scripts](../shell-scripts/README.md)
- [Main Project README](../../README.md)
- [CHANGELOG](../../src/adguard-api-powershell/CHANGELOG.md)

## Contributing

When contributing to PowerShell modules:
1. Follow PowerShell best practices and style guidelines
2. Write Pester tests for new functionality
3. Run PSScriptAnalyzer before committing
4. Update comment-based help
5. Test on Windows, macOS, and Linux if possible
6. Maintain backward compatibility
7. Update relevant documentation

## License

GPLv3 - See [LICENSE](../../LICENSE) for details.
