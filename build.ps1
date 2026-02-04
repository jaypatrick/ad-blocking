#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Root-level build script wrapper for ad-blocking repository

.DESCRIPTION
    Delegates to build/build.ps1 for all build operations.

.PARAMETER All
    Build all projects (default if no specific project selected)

.PARAMETER Rust
    Build Rust projects

.PARAMETER DotNet
    Build .NET projects

.PARAMETER TypeScript
    Build TypeScript/Deno projects

.PARAMETER Python
    Build Python projects

.PARAMETER Profile
    Build profile: 'debug' (default) or 'release'

.EXAMPLE
    .\build.ps1
    Build all projects in debug mode

.EXAMPLE
    .\build.ps1 -Rust
    Build only Rust projects in debug mode

.EXAMPLE
    .\build.ps1 -DotNet -Profile release
    Build only .NET projects in release mode

.EXAMPLE
    .\build.ps1 -All -Profile release
    Build all projects in release mode
#>

[CmdletBinding()]
param(
    [Parameter(HelpMessage = "Build all projects")]
    [switch]$All,
    
    [Parameter(HelpMessage = "Build Rust projects")]
    [switch]$Rust,
    
    [Parameter(HelpMessage = "Build .NET projects")]
    [switch]$DotNet,
    
    [Parameter(HelpMessage = "Build TypeScript/Deno projects")]
    [switch]$TypeScript,
    
    [Parameter(HelpMessage = "Build Python projects")]
    [switch]$Python,
    
    [Parameter(HelpMessage = "Build profile: 'debug' (default) or 'release'")]
    [ValidateSet('debug', 'release')]
    [string]$Profile = 'debug'
)

$ErrorActionPreference = 'Stop'
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# Delegate to the build directory script
& "$ScriptDir\build\build.ps1" @PSBoundParameters
exit $LASTEXITCODE
