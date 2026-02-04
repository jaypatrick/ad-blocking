#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Build script for TypeScript/Deno projects only

.DESCRIPTION
    Builds all TypeScript/Deno projects in the repository.

.EXAMPLE
    .\build-typescript.ps1
    Build all TypeScript projects
#>

[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot = Split-Path -Parent $ScriptDir
Set-Location $RepoRoot

Write-Host "Building TypeScript/Deno projects..." -ForegroundColor Blue
Write-Host ""

# Check if Deno is installed
if (-not (Get-Command deno -ErrorAction SilentlyContinue)) {
    Write-Host "✗ Deno is not installed. Please install Deno to build TypeScript projects." -ForegroundColor Red
    exit 1
}

$BuildFailed = $false

# Build Rules Compiler TypeScript
Write-Host "→ Building Rules Compiler (TypeScript)..."
try {
    Push-Location src/rules-compiler-typescript
    try {
        deno task generate:types
        deno task check
        Write-Host "✓ Rules Compiler (TypeScript) built successfully" -ForegroundColor Green
    }
    finally {
        Pop-Location
    }
}
catch {
    Write-Host "✗ Rules Compiler (TypeScript) build failed" -ForegroundColor Red
    $BuildFailed = $true
}

# Build AdGuard API TypeScript
Write-Host "→ Building AdGuard API Client (TypeScript)..."
try {
    Push-Location src/adguard-api-typescript
    try {
        deno task generate:types
        deno task check
        Write-Host "✓ AdGuard API Client (TypeScript) built successfully" -ForegroundColor Green
    }
    finally {
        Pop-Location
    }
}
catch {
    Write-Host "✗ AdGuard API Client (TypeScript) build failed" -ForegroundColor Red
    $BuildFailed = $true
}

# Build Linear tool
Write-Host "→ Building Linear Import Tool (TypeScript)..."
try {
    Push-Location src/linear
    try {
        deno task generate:types
        deno task check
        Write-Host "✓ Linear Import Tool built successfully" -ForegroundColor Green
    }
    finally {
        Pop-Location
    }
}
catch {
    Write-Host "✗ Linear Import Tool build failed" -ForegroundColor Red
    $BuildFailed = $true
}

Write-Host ""

if ($BuildFailed) {
    Write-Host "✗ Some builds failed." -ForegroundColor Red
    exit 1
}
else {
    Write-Host "✓ All TypeScript projects built successfully!" -ForegroundColor Green
    exit 0
}
