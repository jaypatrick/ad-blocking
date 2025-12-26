#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Generates the AdGuard API PowerShell Client from OpenAPI specification.

.DESCRIPTION
    This script uses OpenAPI Generator to generate a PowerShell client module from
    the OpenAPI specification file. It supports both npm-based openapi-generator-cli
    and Docker-based generation.

.PARAMETER SpecPath
    Path to the OpenAPI specification file. Defaults to ../adguard-api-dotnet/api/openapi.json.

.PARAMETER GeneratorVersion
    OpenAPI Generator version to use. Defaults to 7.16.0.

.PARAMETER OutputDir
    Output directory for generated files. Defaults to current directory.

.PARAMETER PackageName
    Name of the PowerShell module. Defaults to PSAdGuardDNS.

.PARAMETER PackageVersion
    Version of the PowerShell module. Defaults to 1.0.0.

.PARAMETER UseDocker
    Use Docker to run OpenAPI Generator instead of npm installation.

.PARAMETER SkipBackup
    Skip creating a backup of existing generated files.

.PARAMETER Clean
    Remove all backup files after successful generation.

.PARAMETER DryRun
    Preview changes without actually generating new files.

.EXAMPLE
    .\Generate-PowerShellClient.ps1

.EXAMPLE
    .\Generate-PowerShellClient.ps1 -UseDocker

.EXAMPLE
    .\Generate-PowerShellClient.ps1 -PackageName "PSAdGuard" -PackageVersion "1.0.1"

.EXAMPLE
    .\Generate-PowerShellClient.ps1 -SpecPath "path/to/custom/openapi.json"
#>

[CmdletBinding(SupportsShouldProcess)]
param(
    [Parameter()]
    [string]$SpecPath = "../adguard-api-dotnet/api/openapi.json",

    [Parameter()]
    [string]$GeneratorVersion = "7.16.0",

    [Parameter()]
    [string]$OutputDir = ".",

    [Parameter()]
    [string]$PackageName = "PSAdGuardDNS",

    [Parameter()]
    [string]$PackageVersion = "1.0.0",

    [Parameter()]
    [switch]$UseDocker,

    [Parameter()]
    [switch]$SkipBackup,

    [Parameter()]
    [switch]$Clean,

    [Parameter()]
    [switch]$DryRun
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# Script variables
$scriptDir = $PSScriptRoot
$openApiSpec = Join-Path $scriptDir $SpecPath
$outputPath = if ($OutputDir -eq ".") { $scriptDir } else { $OutputDir }
$startTime = Get-Date

# Logging function
function Write-Log {
    param(
        [string]$Message,
        [ValidateSet('Info', 'Success', 'Warning', 'Error')]
        [string]$Level = 'Info'
    )
    
    $icon = switch ($Level) {
        'Info'    { '🔵' }
        'Success' { '✅' }
        'Warning' { '⚠️' }
        'Error'   { '❌' }
    }
    
    $color = switch ($Level) {
        'Info'    { 'Cyan' }
        'Success' { 'Green' }
        'Warning' { 'Yellow' }
        'Error'   { 'Red' }
    }
    
    Write-Host "$icon $Message" -ForegroundColor $color
}

# Banner
Write-Host ""
Write-Host "╔═══════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║   AdGuard API PowerShell Client Generator         ║" -ForegroundColor Cyan
Write-Host "║   OpenAPI Generator v$GeneratorVersion                ║" -ForegroundColor Cyan
Write-Host "╚═══════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

if ($DryRun) {
    Write-Log "Running in DRY RUN mode - no files will be modified" -Level Warning
}

Write-Log "OpenAPI Spec: $openApiSpec" -Level Info
Write-Log "Output Directory: $outputPath" -Level Info
Write-Log "Generator Version: $GeneratorVersion" -Level Info
Write-Log "Package Name: $PackageName" -Level Info
Write-Log "Package Version: $PackageVersion" -Level Info
Write-Host ""

# Check if OpenAPI spec exists
if (-not (Test-Path $openApiSpec)) {
    Write-Log "OpenAPI specification not found at: $openApiSpec" -Level Error
    Write-Host ""
    Write-Host "Please ensure the OpenAPI spec is available."
    Write-Host "You can generate it from the adguard-api-dotnet directory first."
    Write-Host ""
    exit 1
}

# Check if we should use Docker or npm
if ($UseDocker) {
    # Check if Docker is available
    $dockerCmd = Get-Command docker -ErrorAction SilentlyContinue
    if (-not $dockerCmd) {
        Write-Log "Docker not found" -Level Error
        Write-Host ""
        Write-Host "Please install Docker or run without -UseDocker to use npm."
        Write-Host ""
        exit 1
    }
    Write-Log "Using Docker to run OpenAPI Generator" -Level Info
} else {
    # Check if openapi-generator-cli is installed
    $generatorCmd = Get-Command openapi-generator-cli -ErrorAction SilentlyContinue
    if (-not $generatorCmd) {
        Write-Log "openapi-generator-cli not found" -Level Warning
        Write-Host ""
        Write-Host "OpenAPI Generator CLI is not installed."
        Write-Host ""
        Write-Host "Option 1 - Install via npm (recommended):"
        Write-Host "  npm install -g @openapitools/openapi-generator-cli"
        Write-Host ""
        Write-Host "Option 2 - Use Docker:"
        Write-Host "  Run this script with -UseDocker flag"
        Write-Host ""
        
        # Offer to switch to Docker
        if (Get-Command docker -ErrorAction SilentlyContinue) {
            Write-Host "Docker is available on your system." -ForegroundColor Cyan
            $response = Read-Host "Would you like to use Docker instead? (y/n)"
            if ($response -eq 'y' -or $response -eq 'Y') {
                $UseDocker = $true
                Write-Log "Switching to Docker mode" -Level Info
            } else {
                exit 1
            }
        } else {
            exit 1
        }
    } else {
        Write-Log "Using npm-based openapi-generator-cli" -Level Info
    }
}
Write-Host ""

# Backup existing generated files
if (-not $SkipBackup -and -not $DryRun) {
    $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
    $backupDir = Join-Path $scriptDir ".backup-$timestamp"
    
    $modulePath = Join-Path $outputPath $PackageName
    if (Test-Path $modulePath) {
        Write-Log "Creating backup at: $backupDir" -Level Warning
        New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
        Copy-Item -Path $modulePath -Destination $backupDir -Recurse -Force
        Write-Log "Backup created successfully" -Level Success
    }
    Write-Host ""
}

# Generate the PowerShell client
Write-Log "Generating PowerShell client module..." -Level Info
Write-Host ""

# Build additional properties based on documentation
$additionalProps = @(
    "packageName=$PackageName",
    "packageVersion=$PackageVersion",
    "apiNamePrefix=",
    "discardReadOnly=false"
) -join ","

try {
    if ($DryRun) {
        Write-Host "Would execute the following command:" -ForegroundColor Yellow
        Write-Host ""
        if ($UseDocker) {
            $absSpecPath = Resolve-Path $openApiSpec
            $absOutputPath = Resolve-Path $outputPath
            Write-Host "docker run --rm -v `"${absOutputPath}:/local`" openapitools/openapi-generator-cli:v$GeneratorVersion generate \"
            Write-Host "  -i /local/$(Split-Path $absSpecPath -Leaf) \"
            Write-Host "  -g powershell \"
            Write-Host "  -o /local \"
            Write-Host "  --additional-properties=$additionalProps"
        } else {
            Write-Host "openapi-generator-cli generate \"
            Write-Host "  -i $openApiSpec \"
            Write-Host "  -g powershell \"
            Write-Host "  -o $outputPath \"
            Write-Host "  --additional-properties=$additionalProps"
        }
        Write-Host ""
        Write-Log "Dry run complete - no files were modified" -Level Success
        exit 0
    }

    if ($UseDocker) {
        # Convert to absolute paths for Docker volume mounting
        $absSpecPath = Resolve-Path $openApiSpec
        $absOutputPath = Resolve-Path $outputPath
        
        # Copy spec to output directory temporarily for Docker access
        $tempSpec = Join-Path $absOutputPath "openapi-temp.json"
        Copy-Item -Path $absSpecPath -Destination $tempSpec -Force
        
        docker run --rm `
            -v "${absOutputPath}:/local" `
            openapitools/openapi-generator-cli:v$GeneratorVersion generate `
            -i /local/openapi-temp.json `
            -g powershell `
            -o /local `
            --additional-properties=$additionalProps
        
        # Clean up temp spec
        Remove-Item -Path $tempSpec -Force -ErrorAction SilentlyContinue
    } else {
        openapi-generator-cli generate `
            -i $openApiSpec `
            -g powershell `
            -o $outputPath `
            --additional-properties=$additionalProps
    }

    Write-Host ""
    Write-Log "Generation Complete!" -Level Success
    Write-Host ""
    Write-Host "╔═══════════════════════════════════════════════════╗" -ForegroundColor Green
    Write-Host "║              Next Steps                            ║" -ForegroundColor Green
    Write-Host "╚═══════════════════════════════════════════════════╝" -ForegroundColor Green
    Write-Host ""
    Write-Host "1. Review generated module in:" -ForegroundColor Cyan
    Write-Host "   $(Join-Path $outputPath $PackageName)" -ForegroundColor White
    Write-Host ""
    Write-Host "2. Import the module:" -ForegroundColor Cyan
    Write-Host "   Import-Module $(Join-Path $outputPath $PackageName)" -ForegroundColor White
    Write-Host ""
    Write-Host "3. Explore available cmdlets:" -ForegroundColor Cyan
    Write-Host "   Get-Command -Module $PackageName" -ForegroundColor White
    Write-Host ""
    Write-Host "4. Build/publish (if applicable):" -ForegroundColor Cyan
    Write-Host "   Build-Module $(Join-Path $outputPath $PackageName)" -ForegroundColor White
    Write-Host ""
    
    if (-not $SkipBackup) {
        $backupDir = Join-Path $scriptDir ".backup-*"
        Write-Host "Backup location: " -NoNewline -ForegroundColor Cyan
        Write-Host $backupDir -ForegroundColor White
        Write-Host ""
    }
    
    $duration = (Get-Date) - $startTime
    Write-Log "Total time: $($duration.TotalSeconds.ToString('F2')) seconds" -Level Info
    Write-Host ""
}
catch {
    Write-Host ""
    Write-Log "Failed to generate PowerShell client" -Level Error
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
    Write-Host "Common issues:" -ForegroundColor Yellow
    Write-Host "  - Ensure OpenAPI spec is valid JSON/YAML"
    Write-Host "  - Check openapi-generator-cli version compatibility"
    Write-Host "  - Verify Docker is running (if using -UseDocker)"
    Write-Host "  - Check disk space and permissions"
    Write-Host ""
    exit 1
}

# Clean up old backups if requested
if ($Clean -and -not $DryRun) {
    Write-Host ""
    Write-Log "Cleaning up old backups..." -Level Info
    Get-ChildItem -Path $scriptDir -Filter ".backup-*" -Directory | ForEach-Object {
        Remove-Item -Path $_.FullName -Recurse -Force
        Write-Log "Removed: $($_.Name)" -Level Info
    }
    Write-Log "Cleanup complete" -Level Success
}
