#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Complete workflow to download OpenAPI spec and update the API client.

.DESCRIPTION
    This script downloads the latest AdGuard DNS API OpenAPI specification,
    validates it, regenerates the C# API client, and runs tests.

.PARAMETER SpecUrl
    URL to download the OpenAPI specification from.
    Defaults to https://api.adguard-dns.io/swagger/openapi.json

.PARAMETER SkipValidation
    Skip OpenAPI specification validation.

.PARAMETER SkipBuild
    Skip building the solution after regeneration.

.PARAMETER SkipTests
    Skip running tests after build.

.EXAMPLE
    .\Update-ApiClient.ps1

.EXAMPLE
    .\Update-ApiClient.ps1 -SkipTests

.EXAMPLE
    .\Update-ApiClient.ps1 -SpecUrl "https://custom-url/openapi.json"
#>

[CmdletBinding()]
param(
    [Parameter()]
    [string]$SpecUrl = "https://api.adguard-dns.io/swagger/openapi.json",

    [Parameter()]
    [switch]$SkipValidation,

    [Parameter()]
    [switch]$SkipBuild,

    [Parameter()]
    [switch]$SkipTests
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$scriptDir = $PSScriptRoot
$apiDir = Join-Path $scriptDir "api"
$openApiJson = Join-Path $apiDir "openapi.json"
$openApiYaml = Join-Path $apiDir "openapi.yaml"

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "AdGuard API Client Update Workflow" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

# Ensure API directory exists
New-Item -ItemType Directory -Path $apiDir -Force | Out-Null

# Step 1: Download the latest OpenAPI spec
Write-Host "Step 1: Downloading latest OpenAPI specification..." -ForegroundColor Yellow
Write-Host "  URL: $SpecUrl"
Write-Host ""

# Backup existing specs
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
if (Test-Path $openApiJson) {
    Write-Host "  Backing up existing JSON spec..."
    Copy-Item $openApiJson "$openApiJson.backup-$timestamp"
}

if (Test-Path $openApiYaml) {
    Write-Host "  Backing up existing YAML spec..."
    Copy-Item $openApiYaml "$openApiYaml.backup-$timestamp"
}

# Download the spec
try {
    Write-Host "  Downloading..."
    $response = Invoke-WebRequest -Uri $SpecUrl -UseBasicParsing
    
    # Verify it's valid JSON
    try {
        $json = $response.Content | ConvertFrom-Json
        $response.Content | Out-File -FilePath $openApiJson -Encoding utf8
        Write-Host "  ✓ Successfully downloaded OpenAPI JSON spec" -ForegroundColor Green
        
        # Convert JSON to YAML for easier editing/viewing (optional)
        if (Get-Command yq -ErrorAction SilentlyContinue) {
            Write-Host "  Converting JSON to YAML for readability..."
            yq eval -P $openApiJson | Out-File -FilePath $openApiYaml -Encoding utf8
            Write-Host "  ✓ Converted to YAML format" -ForegroundColor Green
        } else {
            Write-Host "  ℹ yq not found, skipping YAML conversion" -ForegroundColor Yellow
            Write-Host "  Install yq: pip install yq"
        }
    }
    catch {
        Write-Host "  ✗ Downloaded file is not valid JSON" -ForegroundColor Red
        throw
    }
}
catch {
    Write-Host "  ✗ Failed to download OpenAPI spec" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please check:"
    Write-Host "  1. Internet connectivity"
    Write-Host "  2. The URL is accessible: $SpecUrl"
    Write-Host "  3. No firewall blocking the request"
    Write-Host ""
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Step 2: Show changes
Write-Host "Step 2: Reviewing changes..." -ForegroundColor Yellow
Write-Host ""

if (Get-Command git -ErrorAction SilentlyContinue) {
    try {
        $gitRoot = git rev-parse --git-dir 2>$null
        if ($gitRoot) {
            Write-Host "Changes in OpenAPI spec:"
            git diff --stat $openApiJson 2>$null
            Write-Host ""
        }
    }
    catch {
        # Not in a git repository
    }
}

# Step 3: Validate the spec (optional but recommended)
if (-not $SkipValidation) {
    Write-Host "Step 3: Validating OpenAPI specification..." -ForegroundColor Yellow
    Write-Host ""

    if (Get-Command spectral -ErrorAction SilentlyContinue) {
        Write-Host "Running Spectral validation..."
        try {
            spectral lint $openApiJson --quiet
            Write-Host "  ✓ Specification is valid" -ForegroundColor Green
        }
        catch {
            Write-Host "  ⚠ Specification has some issues" -ForegroundColor Yellow
            $response = Read-Host "Continue anyway? (y/N)"
            if ($response -notmatch '^[Yy]$') {
                Write-Host "Aborted by user"
                exit 1
            }
        }
    } else {
        Write-Host "  ℹ spectral not found, skipping validation" -ForegroundColor Yellow
        Write-Host "  Install spectral: npm install -g @stoplight/spectral-cli"
    }

    Write-Host ""
}

# Step 4: Regenerate the API client
Write-Host "Step 4: Regenerating API client..." -ForegroundColor Yellow
Write-Host ""

$regenScript = Join-Path $scriptDir "Regenerate-Client.ps1"
if (Test-Path $regenScript) {
    Write-Host "Running regeneration script..."
    try {
        & $regenScript
        Write-Host "  ✓ API client regenerated successfully" -ForegroundColor Green
    }
    catch {
        Write-Host "  ✗ Failed to regenerate API client" -ForegroundColor Red
        throw
    }
} else {
    Write-Host "  ℹ Regeneration script not found at: $regenScript" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "To regenerate the API client manually:"
    Write-Host "  1. Install OpenAPI Generator: npm install -g @openapitools/openapi-generator-cli"
    Write-Host "  2. Run: openapi-generator-cli generate -i $openApiJson -g csharp -o $scriptDir"
}

Write-Host ""

# Step 5: Build and test
if (-not $SkipBuild) {
    Write-Host "Step 5: Building and testing..." -ForegroundColor Yellow
    Write-Host ""

    $slnFile = Join-Path $scriptDir "AdGuard.ApiClient.slnx"
    if (Test-Path $slnFile) {
        Write-Host "Building solution..."
        try {
            dotnet build $slnFile --no-restore | Out-Null
            Write-Host "  ✓ Build successful" -ForegroundColor Green
        }
        catch {
            Write-Host "  ✗ Build failed" -ForegroundColor Red
            Write-Host "Please review and fix compilation errors"
            throw
        }
        
        if (-not $SkipTests) {
            Write-Host ""
            Write-Host "Running tests..."
            try {
                dotnet test $slnFile --no-build --verbosity quiet | Out-Null
                Write-Host "  ✓ All tests passed" -ForegroundColor Green
            }
            catch {
                Write-Host "  ✗ Some tests failed" -ForegroundColor Red
                Write-Host "Please review and fix failing tests"
                throw
            }
        }
    } else {
        Write-Host "  ℹ Solution file not found, skipping build" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "==========================================" -ForegroundColor Green
Write-Host "Update Complete!" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Summary:"
Write-Host "  • Downloaded latest OpenAPI spec from AdGuard DNS API"
if (-not $SkipValidation) {
    Write-Host "  • Validated specification"
}
Write-Host "  • Regenerated C# API client"
if (-not $SkipBuild) {
    Write-Host "  • Built and tested the solution"
}
Write-Host ""
Write-Host "Next steps:"
Write-Host "  1. Review changes: git diff"
Write-Host "  2. Test the updated client with your application"
Write-Host "  3. Commit the changes: git add . && git commit -m 'Update API client from latest OpenAPI spec'"
Write-Host ""
