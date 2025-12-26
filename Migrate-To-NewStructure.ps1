#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Migration script to reorganize shell scripts and PowerShell modules.

.DESCRIPTION
    This script automates the reorganization of shell scripts into separate
    directories and restructures PowerShell modules with OOP paradigms.

.NOTES
    Author: Jayson Knight
    Version: 1.0.0
    
.EXAMPLE
    .\Migrate-To-NewStructure.ps1
    
.EXAMPLE
    .\Migrate-To-NewStructure.ps1 -WhatIf
#>

[CmdletBinding(SupportsShouldProcess)]
param()

$ErrorActionPreference = 'Stop'

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Shell Scripts & PowerShell Migration  " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Define paths
$projectRoot = $PSScriptRoot
$oldShellDir = Join-Path $projectRoot "src" "rules-compiler-shell"
$oldPowerShellDir = Join-Path $projectRoot "src" "adguard-api-powershell"
$newShellDir = Join-Path $projectRoot "src" "shell-scripts"
$newPowerShellDir = Join-Path $projectRoot "src" "powershell-modules"

# Step 1: Copy shell scripts to new locations
Write-Host "Step 1: Reorganizing shell scripts..." -ForegroundColor Yellow

if (Test-Path (Join-Path $oldShellDir "compile-rules.sh")) {
    $bashDir = Join-Path $newShellDir "bash"
    New-Item -ItemType Directory -Path $bashDir -Force | Out-Null
    
    if ($PSCmdlet.ShouldProcess("compile-rules.sh", "Copy to $bashDir")) {
        Copy-Item (Join-Path $oldShellDir "compile-rules.sh") -Destination $bashDir
        Write-Host "  ✓ Copied compile-rules.sh to bash/" -ForegroundColor Green
    }
}

if (Test-Path (Join-Path $oldShellDir "compile-rules.zsh")) {
    $zshDir = Join-Path $newShellDir "zsh"
    New-Item -ItemType Directory -Path $zshDir -Force | Out-Null
    
    if ($PSCmdlet.ShouldProcess("compile-rules.zsh", "Copy to $zshDir")) {
        Copy-Item (Join-Path $oldShellDir "compile-rules.zsh") -Destination $zshDir
        Write-Host "  ✓ Copied compile-rules.zsh to zsh/" -ForegroundColor Green
    }
}

# Step 2: Note about manual work needed
Write-Host ""
Write-Host "Step 2: Manual steps required..." -ForegroundColor Yellow
Write-Host ""
Write-Host "The following tasks need to be completed manually:" -ForegroundColor Cyan
Write-Host ""
Write-Host "  1. Refactor PowerShell modules to use OOP structure:" -ForegroundColor White
Write-Host "     - Split Invoke-RulesCompiler.psm1 into Public/Private/Classes" -ForegroundColor Gray
Write-Host "     - Split Invoke-WebHook.psm1 into Public/Private/Classes" -ForegroundColor Gray
Write-Host "     - Create class files (CompilerConfiguration, CompilerResult, etc.)" -ForegroundColor Gray
Write-Host ""
Write-Host "  2. Add environment variable support to all modules" -ForegroundColor White
Write-Host ""
Write-Host "  3. Modernize bash and zsh scripts with:" -ForegroundColor White
Write-Host "     - Environment variable support" -ForegroundColor Gray
Write-Host "     - Enhanced error handling" -ForegroundColor Gray
Write-Host "     - Progress indicators" -ForegroundColor Gray
Write-Host "     - Statistics output" -ForegroundColor Gray
Write-Host ""
Write-Host "  4. Update tests for new structure" -ForegroundColor White
Write-Host ""
Write-Host "  5. Update documentation and create migration guide" -ForegroundColor White
Write-Host ""

# Step 3: Create placeholder READMEs
Write-Host "Step 3: Creating placeholder READMEs..." -ForegroundColor Yellow

$bashReadme = @"
# Bash Scripts

Bash shell scripts for AdGuard DNS operations.

## Scripts

- **compile-rules.sh**: Compile AdGuard filter rules using hostlist-compiler

## Environment Variables

- `ADGUARD_COMPILER_CONFIG`: Default config file path
- `ADGUARD_COMPILER_OUTPUT`: Default output directory
- `ADGUARD_COMPILER_RULES_DIR`: Default rules directory
- `ADGUARD_COMPILER_FORMAT`: Default config format (json, yaml, toml)
- `DEBUG`: Enable debug output

## Usage

```bash
# Basic usage
./compile-rules.sh

# Use custom config
./compile-rules.sh -c config.yaml

# Copy to rules directory
./compile-rules.sh -c config.yaml -r

# Enable debug mode
DEBUG=1 ./compile-rules.sh
```

## Requirements

- Bash 4.0+
- Node.js 18+
- hostlist-compiler: `npm install -g @adguard/hostlist-compiler`

## See Also

- [Main README](../../../README.md)
- [PowerShell Modules](../../powershell-modules/README.md)
"@

$bashReadmePath = Join-Path $newShellDir "bash" "README.md"
if ($PSCmdlet.ShouldProcess($bashReadmePath, "Create README")) {
    $bashReadme | Out-File -FilePath $bashReadmePath -Encoding UTF8
    Write-Host "  ✓ Created bash/README.md" -ForegroundColor Green
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  Migration Script Complete" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "  1. Review the copied files in src/shell-scripts/" -ForegroundColor White
Write-Host "  2. Complete the manual refactoring tasks listed above" -ForegroundColor White
Write-Host "  3. Test all modules and scripts" -ForegroundColor White
Write-Host "  4. Update documentation" -ForegroundColor White
Write-Host "  5. Commit changes with descriptive message" -ForegroundColor White
Write-Host ""
