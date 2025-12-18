# PowerShell Modules for Ad-Blocking

This directory contains PowerShell modules for automating AdGuard DNS operations and filter rule compilation.

## Modules

### RulesCompiler Module

A cross-platform PowerShell API for compiling AdGuard filter rules using `@adguard/hostlist-compiler`.

**Files:**
- `Invoke-RulesCompiler.psm1` - Main module with all functions
- `RulesCompiler.psd1` - Module manifest
- `RulesCompiler-Harness.ps1` - Interactive test harness

#### Installation

```powershell
# Import the module
Import-Module ./src/adguard-api-powershell/Invoke-RulesCompiler.psm1

# Or import using the manifest
Import-Module ./src/adguard-api-powershell/RulesCompiler.psd1
```

#### Prerequisites

| Requirement | Version | Notes |
|-------------|---------|-------|
| PowerShell | 7.0+ | Cross-platform (Windows, macOS, Linux) |
| Node.js | 18+ | Required for hostlist-compiler |
| hostlist-compiler | Latest | `npm install -g @adguard/hostlist-compiler` |

#### Exported Functions

| Function | Description |
|----------|-------------|
| `Read-CompilerConfiguration` | Reads and parses configuration files (JSON, YAML, TOML) |
| `Invoke-FilterCompiler` | Compiles filter rules using hostlist-compiler CLI |
| `Write-CompiledOutput` | Copies compiled output to a destination directory |
| `Invoke-RulesCompiler` | Main orchestration function for the full compilation pipeline |
| `Get-CompilerVersion` | Returns version information for all components |

#### Supported Configuration Formats

The module supports multiple configuration file formats:

| Format | Extensions | Notes |
|--------|------------|-------|
| JSON | `.json` | Native PowerShell support, default format |
| YAML | `.yaml`, `.yml` | Built-in parser, or install `powershell-yaml` for full support |
| TOML | `.toml` | Built-in parser |

**Format Detection:**
- Format is automatically detected from file extension
- Use `-Format` parameter to override: `-Format 'yaml'`
- YAML/TOML configs are converted to temporary JSON for hostlist-compiler

#### Usage Examples

```powershell
# Check installed versions and platform info
Get-CompilerVersion | Format-List

# Read and display configuration (JSON - default)
$config = Read-CompilerConfiguration
$config | Format-List

# Read YAML configuration
$config = Read-CompilerConfiguration -ConfigPath './compiler-config.yaml'

# Read TOML configuration
$config = Read-CompilerConfiguration -ConfigPath './compiler-config.toml'

# Explicitly specify format (useful when extension doesn't match)
$config = Read-CompilerConfiguration -ConfigPath './config.txt' -Format 'yaml'

# Compile filter rules (uses default JSON config)
Invoke-RulesCompiler

# Compile using YAML configuration
Invoke-RulesCompiler -ConfigPath 'compiler-config.yaml'

# Compile using TOML configuration
Invoke-RulesCompiler -ConfigPath 'compiler-config.toml'

# Compile and copy to rules directory
Invoke-RulesCompiler -CopyToRules

# Use custom config file
Invoke-RulesCompiler -ConfigPath './custom-config.json' -CopyToRules

# Run interactively with the harness
./RulesCompiler-Harness.ps1

# Run non-interactively
./RulesCompiler-Harness.ps1 -CompileOnly -CopyToRules
```

#### Cross-Platform Support

This module is designed to work identically on all platforms supported by PowerShell 7+:

| Platform | Status | Notes |
|----------|--------|-------|
| Windows 10/11 | ✅ Supported | PowerShell 7+ required |
| macOS 10.15+ | ✅ Supported | Install via Homebrew: `brew install powershell` |
| Linux (Ubuntu/Debian) | ✅ Supported | Install via package manager |
| Linux (RHEL/CentOS) | ✅ Supported | Install via package manager |

**Cross-Platform Features:**
- Uses `Join-Path` and `[System.IO.Path]` for path handling
- Platform detection via `$PSVersionTable.Platform`
- UTF-8 encoding for all file operations
- Automatic command detection with platform-specific extensions
- Falls back to `npx` if hostlist-compiler not globally installed

#### Output Structure

`Invoke-RulesCompiler` returns a structured object:

```powershell
@{
    Success          = $true
    ConfigName       = 'JK.com AdGuard Rules'
    ConfigVersion    = '4.0.1.35'
    RuleCount        = 11707
    OutputPath       = '/path/to/output.txt'
    OutputHash       = 'SHA384_HASH...'
    CopiedToRules    = $true
    RulesDestination = '/path/to/rules/adguard_user_filter.txt'
    ElapsedMs        = 1234
    StartTime        = [DateTime]
    EndTime          = [DateTime]
}
```

---

### Webhook Module

A PowerShell module for triggering AdGuard DNS webhooks to update device IP addresses.

**Files:**
- `Invoke-WebHook.psm1` - Main module
- `Webhook-Manifest.psd1` - Module manifest
- `Webhook-Harness.ps1` - Interactive test harness

#### Usage

```powershell
# Import the module
Import-Module ./src/adguard-api-powershell/Invoke-WebHook.psm1

# Set the webhook URL (from AdGuard DNS dashboard)
$webhookUrl = $env:ADGUARD_WEBHOOK_URL

# Invoke once
Invoke-Webhook -WebhookUrl $webhookUrl

# Invoke with retry settings
Invoke-Webhook -WebhookUrl $webhookUrl -RetryCount 5 -RetryInterval 10

# Run continuously
Invoke-Webhook -WebhookUrl $webhookUrl -Continuous $true -WaitTime 500
```

---

## Testing

### Running Pester Tests

```powershell
# Install Pester if needed
Install-Module -Name Pester -Force -SkipPublisherCheck

# Run all tests
Invoke-Pester -Path ./src/adguard-api-powershell/Tests/

# Run specific test file
Invoke-Pester -Path ./src/adguard-api-powershell/Tests/RulesCompiler-Tests.ps1

# Run with verbose output
Invoke-Pester -Path ./src/adguard-api-powershell/Tests/ -Output Detailed
```

### Running PSScriptAnalyzer

```powershell
# Install PSScriptAnalyzer if needed
Install-Module -Name PSScriptAnalyzer -Force

# Analyze all scripts
Invoke-ScriptAnalyzer -Path ./src/adguard-api-powershell -Recurse

# Analyze specific file
Invoke-ScriptAnalyzer -Path ./src/adguard-api-powershell/Invoke-RulesCompiler.psm1
```

---

## Directory Structure

```
src/adguard-api-powershell/
├── README.md                    # This file
├── Invoke-RulesCompiler.psm1    # Rules compiler module
├── RulesCompiler.psd1           # Rules compiler manifest
├── RulesCompiler-Harness.ps1    # Rules compiler test harness
├── Invoke-WebHook.psm1          # Webhook module
├── Webhook-Manifest.psd1        # Webhook manifest
├── Webhook-Harness.ps1          # Webhook test harness
├── Tests/
│   ├── RulesCompiler-Tests.ps1  # Pester tests for rules compiler
│   └── Webhook-Tests.ps1        # Pester tests for webhook module
└── Modules/
    ├── Module-Stub.psm1         # Placeholder
    └── global.json              # .NET SDK config
```

---

## Environment Variables

| Variable | Required | Description |
|----------|----------|-------------|
| `ADGUARD_WEBHOOK_URL` | For webhook | Full webhook URL from AdGuard DNS dashboard |
| `DEBUG` | Optional | Set to any value to enable debug logging |

---

## Troubleshooting

### hostlist-compiler not found

```powershell
# Install globally
npm install -g @adguard/hostlist-compiler

# Or the module will fall back to npx (slower but works)
```

### Permission denied on Linux/macOS

```bash
# Make sure scripts are executable
chmod +x ./src/adguard-api-powershell/*.ps1
```

### Module not loading

```powershell
# Check PowerShell version (must be 7.0+)
$PSVersionTable.PSVersion

# Force reimport
Import-Module ./src/adguard-api-powershell/Invoke-RulesCompiler.psm1 -Force
```

### Get diagnostic information

```powershell
# Check all versions and platform info
Get-CompilerVersion | Format-List

# Enable debug output
$env:DEBUG = '1'
Invoke-RulesCompiler -Verbose
```

---

## Contributing

1. Follow existing code patterns and conventions
2. Add Pester tests for new functionality
3. Ensure cross-platform compatibility
4. Run PSScriptAnalyzer before committing
5. Update this README for new features

---

## License

GPLv3 - See [LICENSE](../../LICENSE) for details.
