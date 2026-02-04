#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Build script for Python projects only

.DESCRIPTION
    Builds all Python projects in the repository.

.EXAMPLE
    .\build-python.ps1
    Build all Python projects
#>

[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot = Split-Path -Parent $ScriptDir
Set-Location $RepoRoot

Write-Host "Building Python projects..." -ForegroundColor Blue
Write-Host ""

# Check if Python is installed
if (-not (Get-Command python -ErrorAction SilentlyContinue) -and -not (Get-Command python3 -ErrorAction SilentlyContinue)) {
    Write-Host "✗ Python 3 is not installed. Please install Python 3 to build Python projects." -ForegroundColor Red
    exit 1
}

$pythonCmd = if (Get-Command python3 -ErrorAction SilentlyContinue) { "python3" } else { "python" }

$BuildFailed = $false

# Build Rules Compiler Python
Write-Host "→ Building Rules Compiler (Python)..."
try {
    Push-Location src/rules-compiler-python
    try {
        & $pythonCmd -m pip install --quiet -e ".[dev]"
        & $pythonCmd -m mypy rules_compiler/
        Write-Host "✓ Rules Compiler (Python) built successfully" -ForegroundColor Green
    }
    finally {
        Pop-Location
    }
}
catch {
    Write-Host "✗ Rules Compiler (Python) build failed" -ForegroundColor Red
    $BuildFailed = $true
}

Write-Host ""

if ($BuildFailed) {
    Write-Host "✗ Some builds failed." -ForegroundColor Red
    exit 1
}
else {
    Write-Host "✓ All Python projects built successfully!" -ForegroundColor Green
    exit 0
}
