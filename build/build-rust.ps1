#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Build script for Rust projects only

.DESCRIPTION
    Builds all Rust projects in the repository workspace.

.PARAMETER Profile
    Build profile: 'debug' (default) or 'release'

.EXAMPLE
    .\build-rust.ps1
    Build in debug mode

.EXAMPLE
    .\build-rust.ps1 -Profile release
    Build in release mode
#>

[CmdletBinding()]
param(
    [Parameter(HelpMessage = "Build profile: 'debug' (default) or 'release'")]
    [ValidateSet('debug', 'release')]
    [string]$Profile = 'debug'
)

$ErrorActionPreference = 'Stop'
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot = Split-Path -Parent $ScriptDir
Set-Location $RepoRoot

Write-Host "Building Rust projects..." -ForegroundColor Blue
Write-Host "Build Profile: $Profile" -ForegroundColor Blue
Write-Host ""

# Build the entire workspace
Write-Host "→ Building Rust workspace..."
try {
    if ($Profile -eq "release") {
        cargo build --release --workspace
    } else {
        cargo build --workspace
    }
    Write-Host "✓ Rust workspace built successfully" -ForegroundColor Green
    exit 0
}
catch {
    Write-Host "✗ Rust workspace build failed" -ForegroundColor Red
    exit 1
}
