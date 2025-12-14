#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Regenerates the AdGuard API Client from OpenAPI specification.

.DESCRIPTION
    This script uses OpenAPI Generator to regenerate the C# client code from
    the OpenAPI specification file. It creates a backup of existing files and
    generates new client code using the specified generator version.

.PARAMETER SpecPath
    Path to the OpenAPI specification file. Defaults to api/openapi.json.

.PARAMETER GeneratorVersion
    OpenAPI Generator version to use. Defaults to 7.16.0.

.PARAMETER SkipBackup
    Skip creating a backup of existing generated files.

.EXAMPLE
    .\Regenerate-Client.ps1

.EXAMPLE
    .\Regenerate-Client.ps1 -SpecPath "path/to/custom/openapi.json"

.EXAMPLE
    .\Regenerate-Client.ps1 -SkipBackup
#>

[CmdletBinding()]
param(
    [Parameter()]
    [string]$SpecPath = "api/openapi.json",

    [Parameter()]
    [string]$GeneratorVersion = "7.16.0",

    [Parameter()]
    [switch]$SkipBackup
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$scriptDir = $PSScriptRoot
$apiDir = Join-Path $scriptDir "api"
$srcDir = Join-Path $scriptDir "src"
$openApiSpec = Join-Path $scriptDir $SpecPath
$outputDir = $scriptDir

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "AdGuard API Client Regeneration Script" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

# Check if OpenAPI spec exists
if (-not (Test-Path $openApiSpec)) {
    Write-Host "ERROR: OpenAPI specification not found at: $openApiSpec" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please ensure the OpenAPI spec is available at:"
    Write-Host "  $openApiSpec"
    Write-Host ""
    Write-Host "You can obtain the latest spec from AdGuard DNS API documentation:"
    Write-Host "  https://adguard-dns.io/kb/private-dns/api/overview/"
    Write-Host ""
    exit 1
}

# Check if openapi-generator-cli is installed
$generatorCmd = Get-Command openapi-generator-cli -ErrorAction SilentlyContinue
if (-not $generatorCmd) {
    Write-Host "ERROR: openapi-generator-cli not found" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please install OpenAPI Generator CLI:"
    Write-Host "  npm install -g @openapitools/openapi-generator-cli"
    Write-Host ""
    Write-Host "Or use Docker:"
    Write-Host '  docker run --rm -v "${PWD}:/local" openapitools/openapi-generator-cli generate \'
    Write-Host '    -i /local/api/openapi.json \'
    Write-Host '    -g csharp \'
    Write-Host '    -o /local \'
    Write-Host '    --additional-properties=targetFramework=net10.0,packageName=AdGuard.ApiClient'
    Write-Host ""
    exit 1
}

Write-Host "OpenAPI Spec: $openApiSpec"
Write-Host "Output Directory: $outputDir"
Write-Host "Generator Version: $GeneratorVersion"
Write-Host ""

# Backup existing generated files
if (-not $SkipBackup) {
    $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
    $backupDir = Join-Path $scriptDir ".backup-$timestamp"
    Write-Host "Creating backup at: $backupDir" -ForegroundColor Yellow
    
    $apiClientSrc = Join-Path $srcDir "AdGuard.ApiClient"
    if (Test-Path $apiClientSrc) {
        New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
        Copy-Item -Path $apiClientSrc -Destination $backupDir -Recurse -Force
        Write-Host "Backup created successfully" -ForegroundColor Green
    }
    Write-Host ""
}

# Generate the client
Write-Host "Generating C# client code..." -ForegroundColor Yellow
Write-Host ""

$additionalProps = @(
    "targetFramework=net10.0",
    "packageName=AdGuard.ApiClient",
    "packageVersion=1.0.0",
    "jsonLibrary=Newtonsoft.Json",
    "validatable=false",
    "netCoreProjectFile=true",
    "nullableReferenceTypes=true"
) -join ","

try {
    openapi-generator-cli generate `
        -i $openApiSpec `
        -g csharp `
        -o $outputDir `
        --additional-properties=$additionalProps

    Write-Host ""
    Write-Host "==========================================" -ForegroundColor Green
    Write-Host "Generation Complete!" -ForegroundColor Green
    Write-Host "==========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:"
    Write-Host "  1. Review generated code in: $srcDir\AdGuard.ApiClient\"
    if (-not $SkipBackup) {
        Write-Host "  2. Apply any custom modifications from: $backupDir\"
    }
    Write-Host "  3. Build the solution: dotnet build AdGuard.ApiClient.slnx"
    Write-Host "  4. Run tests: dotnet test AdGuard.ApiClient.slnx"
    Write-Host ""
    if (-not $SkipBackup) {
        Write-Host "Backup location: $backupDir" -ForegroundColor Cyan
    }
    Write-Host ""
}
catch {
    Write-Host "ERROR: Failed to generate client code" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}
