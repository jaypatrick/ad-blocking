#Requires -Version 7.0

<#
.SYNOPSIS
    Test harness for the RulesCompiler PowerShell module.

.DESCRIPTION
    Interactive script to test and demonstrate the RulesCompiler module functionality.
    Provides a menu-driven interface to:
    - Read and display compiler configuration
    - Compile filter rules
    - Copy output to rules directory
    - Show version information

.NOTES
    Author:  Jayson Knight
    Website: https://jaysonknight.com
    GitHub:  jaypatrick

.EXAMPLE
    .\RulesCompiler-Harness.ps1

.EXAMPLE
    .\RulesCompiler-Harness.ps1 -CompileOnly
#>

[CmdletBinding()]
param(
    [Parameter()]
    [switch]$CompileOnly,

    [Parameter()]
    [switch]$CopyToRules,

    [Parameter()]
    [string]$ConfigPath
)

# Import the module
$modulePath = Join-Path $PSScriptRoot 'Invoke-RulesCompiler.psm1'
if (Test-Path $modulePath) {
    Import-Module $modulePath -Force
    Write-Host "RulesCompiler module loaded successfully." -ForegroundColor Green
}
else {
    Write-Error "Module not found at: $modulePath"
    exit 1
}

function Show-Menu {
    <#
    .SYNOPSIS
    Displays the main menu and returns the user's choice.
    #>
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "   AdGuard Rules Compiler - PowerShell" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  1. Show Version Information"
    Write-Host "  2. Read Configuration"
    Write-Host "  3. Compile Filter Rules"
    Write-Host "  4. Compile and Copy to Rules Directory"
    Write-Host "  5. Run Full Pipeline"
    Write-Host "  6. Exit"
    Write-Host ""

    $choice = Read-Host "Enter your choice (1-6)"
    return $choice
}

function Show-VersionInfo {
    <#
    .SYNOPSIS
    Displays version information for the compiler components.
    #>
    Write-Host ""
    Write-Host "Version Information:" -ForegroundColor Yellow
    Write-Host "-------------------"

    $version = Get-CompilerVersion
    $version | Format-List
}

function Show-Configuration {
    <#
    .SYNOPSIS
    Reads and displays the compiler configuration.
    #>
    param([string]$Path)

    Write-Host ""
    Write-Host "Configuration:" -ForegroundColor Yellow
    Write-Host "--------------"

    try {
        if ($Path) {
            $config = Read-CompilerConfiguration -ConfigPath $Path
        }
        else {
            $config = Read-CompilerConfiguration
        }

        Write-Host "Name:        $($config.name)" -ForegroundColor White
        Write-Host "Version:     $($config.version)" -ForegroundColor White
        Write-Host "Description: $($config.description)" -ForegroundColor White
        Write-Host "Homepage:    $($config.homepage)" -ForegroundColor White
        Write-Host "License:     $($config.license)" -ForegroundColor White
        Write-Host ""
        Write-Host "Sources:" -ForegroundColor Cyan
        foreach ($source in $config.sources) {
            Write-Host "  - $($source.name): $($source.source)" -ForegroundColor Gray
        }
        Write-Host ""
        Write-Host "Transformations:" -ForegroundColor Cyan
        Write-Host "  $($config.transformations -join ', ')" -ForegroundColor Gray
    }
    catch {
        Write-Error "Failed to read configuration: $($_.Exception.Message)"
    }
}

function Invoke-Compilation {
    <#
    .SYNOPSIS
    Runs the filter compilation.
    #>
    param(
        [string]$Path,
        [switch]$Copy
    )

    Write-Host ""
    Write-Host "Starting compilation..." -ForegroundColor Yellow
    Write-Host ""

    try {
        $params = @{}
        if ($Path) {
            $params['ConfigPath'] = $Path
        }
        if ($Copy) {
            $params['CopyToRules'] = $true
        }

        $result = Invoke-RulesCompiler @params

        Write-Host ""
        Write-Host "Compilation Results:" -ForegroundColor Yellow
        Write-Host "-------------------"

        if ($result.Success) {
            Write-Host "Status:       SUCCESS" -ForegroundColor Green
            Write-Host "Config:       $($result.ConfigName) v$($result.ConfigVersion)" -ForegroundColor White
            Write-Host "Rules Count:  $($result.RuleCount)" -ForegroundColor White
            Write-Host "Output File:  $($result.OutputPath)" -ForegroundColor White
            Write-Host "Output Hash:  $($result.OutputHash)" -ForegroundColor Gray
            Write-Host "Elapsed Time: $($result.ElapsedMs)ms" -ForegroundColor White

            if ($result.CopiedToRules) {
                Write-Host "Copied To:    $($result.RulesDestination)" -ForegroundColor Cyan
            }
        }
        else {
            Write-Host "Status:       FAILED" -ForegroundColor Red
            Write-Host "Error:        $($result.ErrorMessage)" -ForegroundColor Red
            Write-Host "Elapsed Time: $($result.ElapsedMs)ms" -ForegroundColor White
        }

        return $result
    }
    catch {
        Write-Error "Compilation failed: $($_.Exception.Message)"
        return $null
    }
}

# Main execution
if ($CompileOnly) {
    # Non-interactive mode: just compile
    $result = Invoke-Compilation -Path $ConfigPath -Copy:$CopyToRules
    if ($result -and $result.Success) {
        exit 0
    }
    else {
        exit 1
    }
}

# Interactive mode
do {
    $choice = Show-Menu

    switch ($choice) {
        '1' {
            Show-VersionInfo
        }
        '2' {
            Show-Configuration -Path $ConfigPath
        }
        '3' {
            Invoke-Compilation -Path $ConfigPath
        }
        '4' {
            Invoke-Compilation -Path $ConfigPath -Copy
        }
        '5' {
            Write-Host ""
            Write-Host "Running Full Pipeline..." -ForegroundColor Magenta
            Write-Host "========================" -ForegroundColor Magenta
            Show-VersionInfo
            Show-Configuration -Path $ConfigPath
            Invoke-Compilation -Path $ConfigPath -Copy
        }
        '6' {
            Write-Host ""
            Write-Host "Goodbye!" -ForegroundColor Green
            exit 0
        }
        default {
            Write-Host "Invalid choice. Please enter 1-6." -ForegroundColor Red
        }
    }

    if ($choice -ne '6') {
        Write-Host ""
        Read-Host "Press Enter to continue..."
    }
} while ($choice -ne '6')
