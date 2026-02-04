#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Build script for .NET projects only

.DESCRIPTION
    Builds all .NET projects in the repository.

.PARAMETER Configuration
    Build configuration: 'Debug' (default) or 'Release'

.EXAMPLE
    .\build-dotnet.ps1
    Build in Debug mode

.EXAMPLE
    .\build-dotnet.ps1 -Configuration Release
    Build in Release mode
#>

[CmdletBinding()]
param(
    [Parameter(HelpMessage = "Build configuration: 'Debug' (default) or 'Release'")]
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Debug'
)

$ErrorActionPreference = 'Stop'
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot = Split-Path -Parent $ScriptDir
Set-Location $RepoRoot

Write-Host "Building .NET projects..." -ForegroundColor Blue
Write-Host "Configuration: $Configuration" -ForegroundColor Blue
Write-Host ""

$BuildFailed = $false

# Build AdGuard API Client
Write-Host "→ Building AdGuard API Client (.NET)..."
try {
    Push-Location src/adguard-api-dotnet
    try {
        dotnet restore AdGuard.ApiClient.slnx
        dotnet build AdGuard.ApiClient.slnx --no-restore --configuration $Configuration
        Write-Host "✓ AdGuard API Client built successfully" -ForegroundColor Green
    }
    finally {
        Pop-Location
    }
}
catch {
    Write-Host "✗ AdGuard API Client build failed" -ForegroundColor Red
    $BuildFailed = $true
}

# Build Rules Compiler .NET
Write-Host "→ Building Rules Compiler (.NET)..."
try {
    Push-Location src/rules-compiler-dotnet
    try {
        dotnet restore RulesCompiler.slnx
        dotnet build RulesCompiler.slnx --no-restore --configuration $Configuration
        Write-Host "✓ Rules Compiler (.NET) built successfully" -ForegroundColor Green
    }
    finally {
        Pop-Location
    }
}
catch {
    Write-Host "✗ Rules Compiler (.NET) build failed" -ForegroundColor Red
    $BuildFailed = $true
}

Write-Host ""

if ($BuildFailed) {
    Write-Host "✗ Some builds failed." -ForegroundColor Red
    exit 1
}
else {
    Write-Host "✓ All .NET projects built successfully!" -ForegroundColor Green
    exit 0
}
