#Requires -Version 7.0

<#
.SYNOPSIS
    PowerShell module for compiling AdGuard filter rules.

.DESCRIPTION
    This module provides a PowerShell API for the AdGuard filter rules compiler.
    It wraps the @adguard/hostlist-compiler CLI tool and leverages the RulesCompiler
    and Common OOP modules for configuration management, logging, and result handling.

    This module provides backward-compatible function APIs while using the modernized
    OOP implementation internally.

.NOTES
    Author:  Jayson Knight
    Website: https://jaysonknight.com
    GitHub:  jaypatrick

    Prerequisites:
    - PowerShell 7.0+ (cross-platform)
    - Node.js 18+ installed
    - @adguard/hostlist-compiler installed globally
#>

# Import the OOP modules from src/powershell-modules/
using module ..\powershell-modules\Common\Common.psm1
using module ..\powershell-modules\RulesCompiler\RulesCompiler.psm1

#region Private Functions

function Get-PlatformInfo {
    [CmdletBinding()]
    [OutputType([PSCustomObject])]
    param()

    $platform = $PSVersionTable.Platform
    $onWindows = $IsWindows -or ($null -eq $platform) -or ($platform -eq 'Win32NT')
    $onLinux = $IsLinux -or ($platform -eq 'Unix' -and $PSVersionTable.OS -match 'Linux')
    $onMacOS = $IsMacOS -or ($platform -eq 'Unix' -and $PSVersionTable.OS -match 'Darwin')

    return [PSCustomObject]@{
        Platform      = if ($onWindows) { 'Windows' } elseif ($onMacOS) { 'macOS' } elseif ($onLinux) { 'Linux' } else { 'Unknown' }
        IsWindows     = $onWindows
        IsLinux       = $onLinux
        IsMacOS       = $onMacOS
        PathSeparator = [System.IO.Path]::DirectorySeparatorChar
        OS            = $PSVersionTable.OS
    }
}

function Get-CommandPath {
    [CmdletBinding()]
    [OutputType([string])]
    param(
        [Parameter(Mandatory)]
        [string]$CommandName
    )

    $command = Get-Command $CommandName -ErrorAction SilentlyContinue
    if ($command) {
        return $command.Source
    }

    $platformInfo = Get-PlatformInfo
    if ($platformInfo.IsWindows) {
        foreach ($ext in @('.cmd', '.exe', '.bat', '.ps1')) {
            $command = Get-Command "$CommandName$ext" -ErrorAction SilentlyContinue
            if ($command) {
                return $command.Source
            }
        }
    }

    return $null
}

#endregion

#region Public Functions

function Read-CompilerConfiguration {
    <#
    .SYNOPSIS
    Reads and parses the compiler configuration from a JSON, YAML, or TOML file.

    .DESCRIPTION
    Reads the compiler configuration file using the CompilerConfiguration OOP class.
    Supports JSON, YAML, and TOML formats with automatic detection.

    .PARAMETER ConfigPath
    Path to the configuration file.

    .PARAMETER Format
    Explicitly specify the configuration format.

    .OUTPUTS
    [CompilerConfiguration] The parsed configuration object.
    #>
    [CmdletBinding()]
    [OutputType([CompilerConfiguration])]
    param(
        [Parameter(Position = 0, ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [Alias('Path', 'Config')]
        [string]$ConfigPath,

        [Parameter(Position = 1, ValueFromPipelineByPropertyName)]
        [ValidateSet('json', 'yaml', 'toml')]
        [string]$Format
    )

    BEGIN {
        if (-not $ConfigPath) {
            $ConfigPath = Join-Path $PSScriptRoot '..' '..' 'src' 'rules-compiler-typescript' 'compiler-config.json'
            $ConfigPath = [System.IO.Path]::GetFullPath($ConfigPath)
        }
    }

    PROCESS {
        try {
            if ($Format) {
                return [CompilerConfiguration]::FromFile($ConfigPath, $Format)
            }
            else {
                return [CompilerConfiguration]::FromFile($ConfigPath)
            }
        }
        catch {
            Write-Error "Failed to read configuration from ${ConfigPath}: $_"
            throw
        }
    }
}

function Invoke-FilterCompiler {
    <#
    .SYNOPSIS
    Compiles filter rules using the AdGuard hostlist compiler.

    .DESCRIPTION
    Invokes the @adguard/hostlist-compiler CLI to compile filter rules.

    .PARAMETER ConfigPath
    Path to the configuration file.

    .PARAMETER OutputPath
    Path where the compiled output will be written.

    .PARAMETER WorkingDirectory
    Working directory for the compiler.

    .PARAMETER Format
    Configuration format (json, yaml, toml).
    #>
    [CmdletBinding()]
    [OutputType([PSCustomObject])]
    param(
        [Parameter(Position = 0, ValueFromPipelineByPropertyName)]
        [Alias('Config')]
        [string]$ConfigPath,

        [Parameter(Position = 1, ValueFromPipelineByPropertyName)]
        [Alias('Output')]
        [string]$OutputPath,

        [Parameter(ValueFromPipelineByPropertyName)]
        [Alias('WorkDir')]
        [string]$WorkingDirectory,

        [Parameter(ValueFromPipelineByPropertyName)]
        [ValidateSet('json', 'yaml', 'toml')]
        [string]$Format
    )

    BEGIN {
        $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
        $logger = [CompilerLogger]::FromEnvironment()

        if (-not $WorkingDirectory) {
            $WorkingDirectory = Join-Path $PSScriptRoot '..' '..' 'src' 'rules-compiler-typescript'
            $WorkingDirectory = [System.IO.Path]::GetFullPath($WorkingDirectory)
        }

        if (-not $ConfigPath) { $ConfigPath = 'compiler-config.json' }
        if (-not $OutputPath) { $OutputPath = 'adguard_user_filter.txt' }
    }

    PROCESS {
        $logger.Info('Starting filter compilation...')
        $logger.Debug("Working directory: $WorkingDirectory")
        $logger.Debug("Config file: $ConfigPath")
        $logger.Debug("Output file: $OutputPath")

        $compilerPath = Get-CommandPath -CommandName 'hostlist-compiler'
        if (-not $compilerPath) {
            $npxPath = Get-CommandPath -CommandName 'npx'
            if ($npxPath) {
                $logger.Debug('hostlist-compiler not found globally, will try npx')
            }
            else {
                $errorMessage = 'hostlist-compiler not found. Install with: npm install -g @adguard/hostlist-compiler'
                $logger.Error($errorMessage)
                throw [System.IO.FileNotFoundException]::new($errorMessage)
            }
        }

        $platformInfo = Get-PlatformInfo
        $logger.Debug("Platform: $($platformInfo.Platform), OS: $($platformInfo.OS)")

        $fullConfigPath = Join-Path $WorkingDirectory $ConfigPath
        $tempJsonConfig = $null
        $effectiveConfigPath = $ConfigPath

        # Convert non-JSON configs to temporary JSON
        if ($Format -and $Format -ne 'json') {
            $logger.Info("Converting $Format configuration to JSON for hostlist-compiler...")
            try {
                $config = Read-CompilerConfiguration -ConfigPath $fullConfigPath -Format $Format
                $tempJsonPath = [System.IO.Path]::Combine([System.IO.Path]::GetTempPath(), "compiler-config-$(Get-Date -Format 'yyyyMMddHHmmss').json")
                $config | ConvertTo-Json -Depth 10 | Set-Content -Path $tempJsonPath -Encoding UTF8
                $tempJsonConfig = $tempJsonPath
                $effectiveConfigPath = $tempJsonConfig
                $logger.Debug("Temporary JSON config: $tempJsonConfig")
            }
            catch {
                $errorMessage = "Failed to convert $Format configuration: $($_.Exception.Message)"
                $logger.Error($errorMessage)
                return [PSCustomObject]@{
                    Success        = $false
                    RuleCount      = 0
                    OutputPath     = $null
                    Hash           = $null
                    ElapsedMs      = $stopwatch.ElapsedMilliseconds
                    ErrorMessage   = $errorMessage
                    CompilerOutput = $null
                    ConfigFormat   = $Format
                }
            }
        }

        $originalLocation = Get-Location
        try {
            Set-Location -Path $WorkingDirectory
            $fullOutputPath = Join-Path $WorkingDirectory $OutputPath

            $logger.Info('Invoking hostlist-compiler...')
            if ($compilerPath) {
                $compilerOutput = & hostlist-compiler --config $effectiveConfigPath --output $OutputPath 2>&1
            }
            else {
                $logger.Debug('Using npx to invoke hostlist-compiler')
                $compilerOutput = & npx hostlist-compiler --config $effectiveConfigPath --output $OutputPath 2>&1
            }

            if ($LASTEXITCODE -ne 0) {
                $errorMessage = "Compilation failed with exit code $LASTEXITCODE"
                $logger.Error($errorMessage)
                $logger.Error("Compiler output: $compilerOutput")

                return [PSCustomObject]@{
                    Success        = $false
                    RuleCount      = 0
                    OutputPath     = $fullOutputPath
                    Hash           = $null
                    ElapsedMs      = $stopwatch.ElapsedMilliseconds
                    ErrorMessage   = $errorMessage
                    CompilerOutput = $compilerOutput
                    ConfigFormat   = $Format
                }
            }

            $ruleCount = 0
            $fileHash = $null

            if (Test-Path $OutputPath) {
                $content = Get-Content -Path $OutputPath -Encoding UTF8
                $ruleCount = ($content | Where-Object { $_ -and -not $_.StartsWith('!') }).Count
                $fileHash = (Get-FileHash -Path $OutputPath -Algorithm SHA384).Hash
            }

            $stopwatch.Stop()
            $logger.Info("Compilation complete. Generated $ruleCount rules in $($stopwatch.ElapsedMilliseconds)ms")

            return [PSCustomObject]@{
                Success        = $true
                RuleCount      = $ruleCount
                OutputPath     = $fullOutputPath
                Hash           = $fileHash
                ElapsedMs      = $stopwatch.ElapsedMilliseconds
                ErrorMessage   = $null
                CompilerOutput = $compilerOutput
                ConfigFormat   = $Format
            }
        }
        finally {
            Set-Location -Path $originalLocation

            if ($tempJsonConfig -and (Test-Path $tempJsonConfig)) {
                Remove-Item -Path $tempJsonConfig -Force -ErrorAction SilentlyContinue
                $logger.Debug('Cleaned up temporary JSON config')
            }
        }
    }
}

function Write-CompiledOutput {
    <#
    .SYNOPSIS
    Writes or copies compiled filter rules to a destination.

    .PARAMETER SourcePath
    Path to the source compiled file.

    .PARAMETER DestinationPath
    Path where the file should be copied.

    .PARAMETER Force
    Overwrite the destination file if it exists.
    #>
    [CmdletBinding()]
    [OutputType([PSCustomObject])]
    param(
        [Parameter(Mandatory, Position = 0, ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [Alias('Path', 'OutputPath')]
        [string]$SourcePath,

        [Parameter(Position = 1, ValueFromPipelineByPropertyName)]
        [Alias('Destination', 'Target')]
        [string]$DestinationPath,

        [Parameter()]
        [switch]$Force
    )

    BEGIN {
        $logger = [CompilerLogger]::FromEnvironment()
        
        if (-not $DestinationPath) {
            $DestinationPath = Join-Path $PSScriptRoot '..' '..' 'rules' 'adguard_user_filter.txt'
            $DestinationPath = [System.IO.Path]::GetFullPath($DestinationPath)
        }
    }

    PROCESS {
        $logger.Debug("Copying output from: $SourcePath")
        $logger.Debug("Copying output to: $DestinationPath")

        if (-not (Test-Path $SourcePath)) {
            $errorMessage = "Source file not found: $SourcePath"
            $logger.Error($errorMessage)
            throw [System.IO.FileNotFoundException]::new($errorMessage)
        }

        try {
            $destinationDir = Split-Path -Path $DestinationPath -Parent
            if (-not (Test-Path $destinationDir)) {
                New-Item -ItemType Directory -Path $destinationDir -Force | Out-Null
            }

            Copy-Item -Path $SourcePath -Destination $DestinationPath -Force:$Force

            $fileInfo = Get-Item $DestinationPath
            $lineCount = (Get-Content $DestinationPath -Encoding UTF8).Count

            $logger.Info("Successfully wrote $lineCount lines to $DestinationPath")

            return [PSCustomObject]@{
                Success         = $true
                SourcePath      = $SourcePath
                DestinationPath = $DestinationPath
                BytesCopied     = $fileInfo.Length
                LineCount       = $lineCount
            }
        }
        catch {
            $errorMessage = "Failed to write output file: $($_.Exception.Message)"
            $logger.Error($errorMessage)
            throw
        }
    }
}

function Invoke-RulesCompiler {
    <#
    .SYNOPSIS
    Main entry point for the filter compiler.

    .DESCRIPTION
    Orchestrates the complete compilation pipeline using OOP classes.

    .PARAMETER ConfigPath
    Path to the configuration file.

    .PARAMETER OutputPath
    Path for the compiled output file.

    .PARAMETER CopyToRules
    If specified, copies the compiled output to the rules directory.

    .PARAMETER RulesPath
    Custom path for the rules directory.
    #>
    [CmdletBinding()]
    [OutputType([PSCustomObject])]
    param(
        [Parameter(Position = 0, ValueFromPipelineByPropertyName)]
        [Alias('Config')]
        [string]$ConfigPath,

        [Parameter(Position = 1, ValueFromPipelineByPropertyName)]
        [Alias('Output')]
        [string]$OutputPath,

        [Parameter()]
        [Alias('Copy')]
        [switch]$CopyToRules,

        [Parameter()]
        [string]$RulesPath
    )

    BEGIN {
        $startTime = Get-Date
        $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
        $logger = [CompilerLogger]::FromEnvironment()
        
        # Display banner
        Write-Host ""
        Write-Host "╔════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
        Write-Host "║          AdGuard Filter Rules Compiler v2.0              ║" -ForegroundColor Cyan
        Write-Host "╚════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
        Write-Host ""
    }

    PROCESS {
        Write-Host "Starting compilation pipeline..." -ForegroundColor Yellow
        Write-Host ""

        try {
            # Step 1: Read configuration
            Write-Host "├── [1/3] Reading configuration..." -ForegroundColor Cyan
            $config = Read-CompilerConfiguration -ConfigPath $ConfigPath
            Write-Host "│   └─ Loaded: " -NoNewline -ForegroundColor Gray
            Write-Host "$($config.Name) v$($config.Version)" -ForegroundColor Green
            Write-Host "│" -ForegroundColor Gray

            # Step 2: Compile filters
            Write-Host "├── [2/3] Compiling filters..." -ForegroundColor Cyan
            $compileResult = Invoke-FilterCompiler -ConfigPath $ConfigPath -OutputPath $OutputPath

            if (-not $compileResult.Success) {
                throw "Compilation failed: $($compileResult.ErrorMessage)"
            }
            
            Write-Host "│   └─ Generated: " -NoNewline -ForegroundColor Gray
            Write-Host "$($compileResult.RuleCount) rules" -ForegroundColor Green
            Write-Host "│" -ForegroundColor Gray

            # Step 3: Copy to rules directory (if requested)
            $copyResult = $null
            if ($CopyToRules) {
                Write-Host "└── [3/3] Copying to rules directory..." -ForegroundColor Cyan
                $copyParams = @{
                    SourcePath = $compileResult.OutputPath
                    Force      = $true
                }
                if ($RulesPath) {
                    $copyParams['DestinationPath'] = $RulesPath
                }
                $copyResult = Write-CompiledOutput @copyParams
                Write-Host "    └─ Destination: " -NoNewline -ForegroundColor Gray
                Write-Host $copyResult.DestinationPath -ForegroundColor Green
            }
            else {
                Write-Host "└── [3/3] Skipping copy (use -CopyToRules to enable)" -ForegroundColor DarkGray
            }

            $stopwatch.Stop()
            $elapsed = $stopwatch.ElapsedMilliseconds
            
            # Display success banner
            Write-Host ""
            Write-Host "╔════════════════════════════════════════════════════════════╗" -ForegroundColor Green
            Write-Host "║            ✓ COMPILATION COMPLETED SUCCESSFULLY           ║" -ForegroundColor Green
            Write-Host "╚════════════════════════════════════════════════════════════╝" -ForegroundColor Green
            Write-Host ""
            Write-Host "Results Summary:" -ForegroundColor Yellow
            Write-Host "  └─ Configuration:  " -NoNewline -ForegroundColor Gray
            Write-Host "$($config.Name) v$($config.Version)" -ForegroundColor White
            Write-Host "  └─ Rules Count:    " -NoNewline -ForegroundColor Gray
            Write-Host $compileResult.RuleCount -ForegroundColor Green
            Write-Host "  └─ Output File:    " -NoNewline -ForegroundColor Gray
            Write-Host $(Split-Path $compileResult.OutputPath -Leaf) -ForegroundColor Cyan
            Write-Host "  └─ Elapsed Time:   " -NoNewline -ForegroundColor Gray
            Write-Host "$($elapsed)ms" -ForegroundColor White
            if ($CopyToRules) {
                Write-Host "  └─ Copied To:      " -NoNewline -ForegroundColor Gray
                Write-Host $(Split-Path $copyResult.DestinationPath -Leaf) -ForegroundColor Magenta
            }
            Write-Host ""

            return [PSCustomObject]@{
                Success           = $true
                ConfigName        = $config.Name
                ConfigVersion     = $config.Version
                RuleCount         = $compileResult.RuleCount
                OutputPath        = $compileResult.OutputPath
                OutputHash        = $compileResult.Hash
                CopiedToRules     = $CopyToRules.IsPresent
                RulesDestination  = if ($copyResult) { $copyResult.DestinationPath } else { $null }
                ElapsedMs         = $elapsed
                StartTime         = $startTime
                EndTime           = Get-Date
            }
        }
        catch {
            $stopwatch.Stop()
            
            # Display failure banner
            Write-Host ""
            Write-Host "╔════════════════════════════════════════════════════════════╗" -ForegroundColor Red
            Write-Host "║                 ✗ COMPILATION FAILED                    ║" -ForegroundColor Red
            Write-Host "╚════════════════════════════════════════════════════════════╝" -ForegroundColor Red
            Write-Host ""
            Write-Host "Error Details:" -ForegroundColor Yellow
            Write-Host "  └─ Message: " -NoNewline -ForegroundColor Gray
            Write-Host $_.Exception.Message -ForegroundColor Red
            Write-Host "  └─ Elapsed: " -NoNewline -ForegroundColor Gray
            Write-Host "$($stopwatch.ElapsedMilliseconds)ms" -ForegroundColor White
            Write-Host ""

            return [PSCustomObject]@{
                Success           = $false
                ConfigName        = $null
                ConfigVersion     = $null
                RuleCount         = 0
                OutputPath        = $null
                OutputHash        = $null
                CopiedToRules     = $false
                RulesDestination  = $null
                ElapsedMs         = $stopwatch.ElapsedMilliseconds
                StartTime         = $startTime
                EndTime           = Get-Date
                ErrorMessage      = $_.Exception.Message
            }
        }
    }
}

function Get-CompilerVersion {
    <#
    .SYNOPSIS
    Gets the version information for the rules compiler components.

    .OUTPUTS
    [PSCustomObject] An object containing version information.
    #>
    [CmdletBinding()]
    [OutputType([PSCustomObject])]
    param()

    PROCESS {
        $moduleVersion = '2.0.0'
        $hostlistVersion = $null
        $nodeVersion = $null
        $npmVersion = $null

        $platformInfo = Get-PlatformInfo

        $compilerPath = Get-CommandPath -CommandName 'hostlist-compiler'
        if ($compilerPath) {
            try {
                $hostlistVersion = & hostlist-compiler --version 2>&1
                if ($LASTEXITCODE -ne 0) {
                    $hostlistVersion = 'Error getting version'
                }
            }
            catch {
                $hostlistVersion = 'Error: ' + $_.Exception.Message
            }
        }
        else {
            $hostlistVersion = 'Not installed (run: npm install -g @adguard/hostlist-compiler)'
        }

        $nodePath = Get-CommandPath -CommandName 'node'
        if ($nodePath) {
            try {
                $nodeVersion = & node --version 2>&1
                if ($LASTEXITCODE -ne 0) {
                    $nodeVersion = 'Error getting version'
                }
            }
            catch {
                $nodeVersion = 'Error: ' + $_.Exception.Message
            }
        }
        else {
            $nodeVersion = 'Not installed'
        }

        $npmPath = Get-CommandPath -CommandName 'npm'
        if ($npmPath) {
            try {
                $npmVersion = & npm --version 2>&1
                if ($LASTEXITCODE -ne 0) {
                    $npmVersion = 'Error getting version'
                }
            }
            catch {
                $npmVersion = 'Error: ' + $_.Exception.Message
            }
        }
        else {
            $npmVersion = 'Not installed'
        }

        return [PSCustomObject]@{
            ModuleName              = 'Invoke-RulesCompiler'
            ModuleVersion           = $moduleVersion
            HostlistCompilerVersion = $hostlistVersion
            NodeVersion             = $nodeVersion
            NpmVersion              = $npmVersion
            PowerShellVersion       = $PSVersionTable.PSVersion.ToString()
            Platform                = $platformInfo.Platform
            OS                      = $platformInfo.OS
            PathSeparator           = $platformInfo.PathSeparator
        }
    }
}

#endregion

#region Module Exports

Export-ModuleMember -Function @(
    'Read-CompilerConfiguration',
    'Invoke-FilterCompiler',
    'Write-CompiledOutput',
    'Invoke-RulesCompiler',
    'Get-CompilerVersion'
)

#endregion
