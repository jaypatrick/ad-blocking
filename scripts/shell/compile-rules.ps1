#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Cross-platform PowerShell Core script for compiling AdGuard filter rules.

.DESCRIPTION
    This script provides a PowerShell Core interface to the hostlist-compiler,
    supporting JSON, YAML, and TOML configuration formats. Works on Windows,
    macOS, and Linux.

.PARAMETER ConfigPath
    Path to the configuration file. Default: compiler-config.json

.PARAMETER OutputPath
    Path to the output file. Default: output/compiled-TIMESTAMP.txt

.PARAMETER CopyToRules
    Copy the compiled output to the rules directory.

.PARAMETER Format
    Force configuration format (json, yaml, toml). Auto-detected if not specified.

.PARAMETER Version
    Show version information and exit.

.PARAMETER Help
    Show help message and exit.

.PARAMETER Debug
    Enable debug output.

.EXAMPLE
    ./compile-rules.ps1

.EXAMPLE
    ./compile-rules.ps1 -ConfigPath config.yaml -CopyToRules

.EXAMPLE
    ./compile-rules.ps1 -Version

.NOTES
    Author: jaypatrick
    License: GPLv3
    Requires: PowerShell 7.0+, Node.js 18+
#>

[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [Alias('c', 'Config')]
    [string]$ConfigPath,

    [Parameter()]
    [Alias('o', 'Output')]
    [string]$OutputPath,

    [Parameter()]
    [Alias('r', 'Copy')]
    [switch]$CopyToRules,

    [Parameter()]
    [Alias('f')]
    [ValidateSet('json', 'yaml', 'toml')]
    [string]$Format,

    [Parameter()]
    [Alias('v')]
    [switch]$Version,

    [Parameter()]
    [Alias('h', '?')]
    [switch]$Help
)

# Script configuration
$ErrorActionPreference = 'Stop'
$ScriptVersion = '1.0.0'

# Paths
$ScriptDir = $PSScriptRoot
$ProjectRoot = Split-Path -Parent (Split-Path -Parent $ScriptDir)
$DefaultConfig = Join-Path $ProjectRoot 'src' 'filter-compiler' 'compiler-config.json'
$DefaultRulesDir = Join-Path $ProjectRoot 'rules'
$DefaultOutputFile = 'adguard_user_filter.txt'

#region Logging Functions

function Write-Log {
    param(
        [Parameter(Mandatory)]
        [string]$Message,
        [ValidateSet('Info', 'Warn', 'Error', 'Debug')]
        [string]$Level = 'Info'
    )

    $timestamp = (Get-Date).ToUniversalTime().ToString('yyyy-MM-ddTHH:mm:ssZ')
    $prefix = switch ($Level) {
        'Info'  { Write-Host "[$Level] $timestamp - $Message" -ForegroundColor Green }
        'Warn'  { Write-Host "[$Level] $timestamp - $Message" -ForegroundColor Yellow }
        'Error' { Write-Host "[$Level] $timestamp - $Message" -ForegroundColor Red }
        'Debug' {
            if ($DebugPreference -ne 'SilentlyContinue' -or $env:DEBUG) {
                Write-Host "[$Level] $timestamp - $Message" -ForegroundColor Cyan
            }
        }
    }
}

#endregion

#region Helper Functions

function Show-Help {
    @"
AdGuard Filter Rules Compiler (PowerShell Core API)

Usage: ./compile-rules.ps1 [OPTIONS]

Options:
  -ConfigPath, -c PATH    Path to configuration file (default: compiler-config.json)
  -OutputPath, -o PATH    Path to output file (default: output/compiled-TIMESTAMP.txt)
  -CopyToRules, -r        Copy output to rules directory
  -Format, -f FORMAT      Force configuration format (json, yaml, toml)
  -Version, -v            Show version information
  -Help, -h               Show this help message
  -Debug                  Enable debug output

Supported Configuration Formats:
  - JSON (.json)
  - YAML (.yaml, .yml)
  - TOML (.toml)

Examples:
  ./compile-rules.ps1                           # Use default config
  ./compile-rules.ps1 -c config.yaml -r         # Use YAML config, copy to rules
  ./compile-rules.ps1 -ConfigPath config.toml   # Use TOML config
  ./compile-rules.ps1 -Version                  # Show version info

"@
}

function Show-VersionInfo {
    $platformInfo = @{
        OS = if ($IsWindows) { 'Windows' } elseif ($IsLinux) { 'Linux' } elseif ($IsMacOS) { 'macOS' } else { $PSVersionTable.OS }
        Architecture = [System.Runtime.InteropServices.RuntimeInformation]::ProcessArchitecture
        PowerShell = $PSVersionTable.PSVersion.ToString()
    }

    Write-Output "AdGuard Filter Rules Compiler (PowerShell Core API)"
    Write-Output "Version: $ScriptVersion"
    Write-Output ""
    Write-Output "Platform Information:"
    Write-Output "  OS: $($platformInfo.OS)"
    Write-Output "  Architecture: $($platformInfo.Architecture)"
    Write-Output "  PowerShell: $($platformInfo.PowerShell)"
    Write-Output ""

    # Check Node.js
    try {
        $nodeVersion = & node --version 2>$null
        Write-Output "  Node.js: $nodeVersion"
    }
    catch {
        Write-Output "  Node.js: Not found"
    }

    # Check hostlist-compiler
    $compilerPath = Get-CompilerPath
    if ($compilerPath) {
        Write-Output "  hostlist-compiler: $compilerPath"
    }
    else {
        Write-Output "  hostlist-compiler: Not found"
    }
}

function Get-ConfigurationFormat {
    param([string]$FilePath)

    $extension = [System.IO.Path]::GetExtension($FilePath).ToLower()

    switch ($extension) {
        '.json' { return 'json' }
        '.yaml' { return 'yaml' }
        '.yml'  { return 'yaml' }
        '.toml' { return 'toml' }
        default {
            throw "Unknown configuration file extension: $extension"
        }
    }
}

function Get-CompilerPath {
    # Try global hostlist-compiler
    $extensions = if ($IsWindows) { @('.cmd', '.exe', '.bat', '') } else { @('') }

    foreach ($ext in $extensions) {
        $cmd = "hostlist-compiler$ext"
        $path = Get-Command $cmd -ErrorAction SilentlyContinue
        if ($path) {
            return $path.Source
        }
    }

    # Check for npx
    foreach ($ext in $extensions) {
        $cmd = "npx$ext"
        $path = Get-Command $cmd -ErrorAction SilentlyContinue
        if ($path) {
            return "npx @adguard/hostlist-compiler"
        }
    }

    return $null
}

function ConvertFrom-Yaml {
    param([string]$Content)

    # Try powershell-yaml module first
    if (Get-Module -ListAvailable -Name powershell-yaml) {
        Import-Module powershell-yaml -ErrorAction SilentlyContinue
        return $Content | ConvertFrom-Yaml
    }

    # Fallback to Python
    $pythonScript = @"
import sys, json, yaml
data = yaml.safe_load(sys.stdin.read())
print(json.dumps(data))
"@

    $result = $Content | python3 -c $pythonScript 2>$null
    if ($LASTEXITCODE -eq 0) {
        return $result | ConvertFrom-Json
    }

    throw "No YAML parser available. Install powershell-yaml or Python with PyYAML."
}

function ConvertFrom-Toml {
    param([string]$Content)

    # Use Python for TOML parsing
    $pythonScript = @"
import sys, json
try:
    import tomllib
    data = tomllib.loads(sys.stdin.read())
except ImportError:
    import toml
    data = toml.loads(sys.stdin.read())
print(json.dumps(data))
"@

    $result = $Content | python3 -c $pythonScript 2>$null
    if ($LASTEXITCODE -eq 0) {
        return $result | ConvertFrom-Json
    }

    throw "No TOML parser available. Requires Python 3.11+ or toml package."
}

function Read-Configuration {
    param(
        [string]$Path,
        [string]$ForceFormat
    )

    if (-not (Test-Path $Path)) {
        throw "Configuration file not found: $Path"
    }

    $format = if ($ForceFormat) { $ForceFormat } else { Get-ConfigurationFormat -FilePath $Path }
    $content = Get-Content -Path $Path -Raw -Encoding UTF8

    Write-Log "Reading configuration format: $format" -Level Debug

    switch ($format) {
        'json' {
            return $content | ConvertFrom-Json
        }
        'yaml' {
            return ConvertFrom-Yaml -Content $content
        }
        'toml' {
            return ConvertFrom-Toml -Content $content
        }
    }
}

function Get-RuleCount {
    param([string]$FilePath)

    $lines = Get-Content -Path $FilePath -Encoding UTF8
    $count = 0

    foreach ($line in $lines) {
        $trimmed = $line.Trim()
        if ($trimmed -and -not $trimmed.StartsWith('!') -and -not $trimmed.StartsWith('#')) {
            $count++
        }
    }

    return $count
}

function Get-FileHash384 {
    param([string]$FilePath)

    $hash = Get-FileHash -Path $FilePath -Algorithm SHA384
    return $hash.Hash.ToLower()
}

#endregion

#region Main Functions

function Invoke-Compilation {
    param(
        [string]$ConfigPath,
        [string]$OutputPath,
        [string]$Format
    )

    $startTime = Get-Date

    Write-Log "Starting filter compilation..."
    Write-Log "Config: $ConfigPath" -Level Debug
    Write-Log "Output: $OutputPath" -Level Debug

    # Verify config exists
    if (-not (Test-Path $ConfigPath)) {
        throw "Configuration file not found: $ConfigPath"
    }

    # Detect format
    $actualFormat = if ($Format) { $Format } else { Get-ConfigurationFormat -FilePath $ConfigPath }
    Write-Log "Configuration format: $actualFormat" -Level Debug

    # Convert to JSON if needed
    $jsonConfigPath = $ConfigPath
    $tempConfig = $null

    if ($actualFormat -ne 'json') {
        $config = Read-Configuration -Path $ConfigPath -ForceFormat $actualFormat
        $tempConfig = [System.IO.Path]::GetTempFileName()
        $tempConfig = [System.IO.Path]::ChangeExtension($tempConfig, '.json')
        $config | ConvertTo-Json -Depth 10 | Set-Content -Path $tempConfig -Encoding UTF8
        $jsonConfigPath = $tempConfig
        Write-Log "Created temporary JSON config: $tempConfig" -Level Debug
    }

    # Ensure output directory exists
    $outputDir = Split-Path -Parent $OutputPath
    if (-not (Test-Path $outputDir)) {
        New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
    }

    # Get compiler command
    $compilerCmd = Get-CompilerPath
    if (-not $compilerCmd) {
        throw "hostlist-compiler not found. Install with: npm install -g @adguard/hostlist-compiler"
    }

    Write-Log "Using compiler: $compilerCmd" -Level Debug
    Write-Log "Running hostlist-compiler..."

    try {
        # Execute compiler
        if ($compilerCmd -like 'npx*') {
            $result = & npx @adguard/hostlist-compiler --config $jsonConfigPath --output $OutputPath 2>&1
        }
        else {
            $result = & $compilerCmd --config $jsonConfigPath --output $OutputPath 2>&1
        }

        if ($LASTEXITCODE -ne 0) {
            throw "Compilation failed with exit code $LASTEXITCODE`n$result"
        }

        $endTime = Get-Date
        $elapsed = [int]($endTime - $startTime).TotalMilliseconds

        # Get statistics
        $ruleCount = Get-RuleCount -FilePath $OutputPath
        $outputHash = Get-FileHash384 -FilePath $OutputPath

        Write-Log "Compilation successful!"
        Write-Output ""
        Write-Output "Results:"
        Write-Output "  Rule Count:  $ruleCount"
        Write-Output "  Output Path: $OutputPath"
        Write-Output "  Hash:        $($outputHash.Substring(0, 32))..."
        Write-Output "  Elapsed:     ${elapsed}ms"

        return @{
            Success = $true
            RuleCount = $ruleCount
            OutputPath = $OutputPath
            OutputHash = $outputHash
            ElapsedMs = $elapsed
        }
    }
    finally {
        # Clean up temp file
        if ($tempConfig -and (Test-Path $tempConfig)) {
            Remove-Item -Path $tempConfig -Force -ErrorAction SilentlyContinue
        }
    }
}

function Copy-ToRulesDirectory {
    param([string]$SourcePath)

    $destPath = Join-Path $DefaultRulesDir $DefaultOutputFile

    Write-Log "Copying output to rules directory..."

    if (-not (Test-Path $DefaultRulesDir)) {
        New-Item -ItemType Directory -Path $DefaultRulesDir -Force | Out-Null
    }

    Copy-Item -Path $SourcePath -Destination $destPath -Force
    Write-Log "Copied to: $destPath"

    return $destPath
}

#endregion

#region Main Entry Point

function Main {
    # Handle help and version
    if ($Help) {
        Show-Help
        return
    }

    if ($Version) {
        Show-VersionInfo
        return
    }

    # Enable debug if requested
    if ($PSBoundParameters.ContainsKey('Debug') -or $env:DEBUG) {
        $DebugPreference = 'Continue'
    }

    # Set defaults
    if (-not $ConfigPath) {
        $ConfigPath = $DefaultConfig
    }

    if (-not $OutputPath) {
        $timestamp = (Get-Date).ToUniversalTime().ToString('yyyyMMdd-HHmmss')
        $OutputPath = Join-Path $ProjectRoot 'src' 'rules-compiler-typescript' 'output' "compiled-$timestamp.txt"
    }

    try {
        # Run compilation
        $result = Invoke-Compilation -ConfigPath $ConfigPath -OutputPath $OutputPath -Format $Format

        # Copy to rules if requested
        if ($CopyToRules -and $result.Success) {
            Copy-ToRulesDirectory -SourcePath $result.OutputPath
        }

        Write-Log "Done!"
    }
    catch {
        Write-Log $_.Exception.Message -Level Error
        exit 1
    }
}

# Run main
Main

#endregion
