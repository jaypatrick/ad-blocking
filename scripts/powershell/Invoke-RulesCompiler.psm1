#Requires -Version 7.0

<#
.SYNOPSIS
    PowerShell module for compiling AdGuard filter rules.

.DESCRIPTION
    This module provides a PowerShell API for the AdGuard filter rules compiler.
    It wraps the @adguard/hostlist-compiler CLI tool and provides functions for:
    - Reading compiler configuration
    - Compiling filter rules
    - Writing compiled output

    This module mirrors the TypeScript API in invoke-compiler.ts.

    CROSS-PLATFORM SUPPORT:
    This module is designed to work on Windows, macOS, and Linux with PowerShell 7.0+.
    - Uses platform-agnostic path handling (Join-Path, [System.IO.Path])
    - Detects platform via $PSVersionTable.Platform
    - Handles command invocation consistently across platforms
    - Uses UTF-8 encoding for all file operations

.NOTES
    Author:  Jayson Knight
    Website: https://jaysonknight.com
    GitHub:  jaypatrick

    Prerequisites:
    - PowerShell 7.0+ (cross-platform)
    - Node.js 18+ installed
    - @adguard/hostlist-compiler installed globally: npm install -g @adguard/hostlist-compiler

    Supported Platforms:
    - Windows 10/11 with PowerShell 7+
    - macOS 10.15+ with PowerShell 7+
    - Linux (Ubuntu 18.04+, Debian 10+, RHEL 8+, etc.) with PowerShell 7+
#>

#region Private Functions

function Get-PlatformInfo {
    <#
    .SYNOPSIS
    Gets information about the current platform.

    .DESCRIPTION
    Returns platform-specific information for cross-platform compatibility.
    This is used internally to handle platform differences.
    #>
    [CmdletBinding()]
    [OutputType([PSCustomObject])]
    param()

    $platform = $PSVersionTable.Platform
    $isWindows = $IsWindows -or ($null -eq $platform) -or ($platform -eq 'Win32NT')
    $isLinux = $IsLinux -or ($platform -eq 'Unix' -and $PSVersionTable.OS -match 'Linux')
    $isMacOS = $IsMacOS -or ($platform -eq 'Unix' -and $PSVersionTable.OS -match 'Darwin')

    return [PSCustomObject]@{
        Platform    = if ($isWindows) { 'Windows' } elseif ($isMacOS) { 'macOS' } elseif ($isLinux) { 'Linux' } else { 'Unknown' }
        IsWindows   = $isWindows
        IsLinux     = $isLinux
        IsMacOS     = $isMacOS
        PathSeparator = [System.IO.Path]::DirectorySeparatorChar
        OS          = $PSVersionTable.OS
    }
}

function Get-CommandPath {
    <#
    .SYNOPSIS
    Gets the path to a command, handling cross-platform differences.

    .DESCRIPTION
    Locates an executable command across different platforms, checking for
    platform-specific extensions on Windows (.cmd, .exe, .bat).

    .PARAMETER CommandName
    The name of the command to locate.
    #>
    [CmdletBinding()]
    [OutputType([string])]
    param(
        [Parameter(Mandatory)]
        [string]$CommandName
    )

    # Try to find the command directly
    $command = Get-Command $CommandName -ErrorAction SilentlyContinue

    if ($command) {
        return $command.Source
    }

    # On Windows, try with common extensions
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

function Write-CompilerLog {
    <#
    .SYNOPSIS
    Internal logging function for consistent console output.

    .PARAMETER Level
    The log level (INFO, WARN, ERROR, DEBUG).

    .PARAMETER Message
    The message to log.

    .PARAMETER Arguments
    Additional arguments to include in the log message.
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [ValidateSet('INFO', 'WARN', 'ERROR', 'DEBUG')]
        [string]$Level,

        [Parameter(Mandatory)]
        [string]$Message,

        [Parameter()]
        [object[]]$Arguments
    )

    # Use UTC time for consistent cross-platform logging
    $timestamp = (Get-Date).ToUniversalTime().ToString('yyyy-MM-ddTHH:mm:ss.fffZ')
    $formattedMessage = "[$Level] $timestamp - $Message"

    if ($Arguments) {
        $formattedMessage += " " + ($Arguments -join ' ')
    }

    switch ($Level) {
        'INFO' { Write-Host $formattedMessage -ForegroundColor Cyan }
        'WARN' { Write-Warning $formattedMessage }
        'ERROR' { Write-Error $formattedMessage -ErrorAction Continue }
        'DEBUG' {
            if ($env:DEBUG) {
                Write-Host $formattedMessage -ForegroundColor Gray
            }
        }
    }
}

#endregion

#region Public Functions

function Read-CompilerConfiguration {
    <#
    .SYNOPSIS
    Reads and parses the compiler configuration from a JSON file.

    .DESCRIPTION
    Reads the compiler configuration JSON file and returns it as a PowerShell object.
    This function mirrors the TypeScript readConfiguration() function.

    .PARAMETER ConfigPath
    Path to the configuration JSON file. Defaults to 'compiler-config.json' in the filter-compiler directory.

    .OUTPUTS
    [PSCustomObject] The parsed configuration object containing:
    - name: Filter list name
    - description: Filter list description
    - version: Version string
    - sources: Array of source configurations
    - transformations: Array of transformation names

    .EXAMPLE
    $config = Read-CompilerConfiguration -ConfigPath './compiler-config.json'

    .EXAMPLE
    $config = Read-CompilerConfiguration
    # Uses default path

    .NOTES
    Throws an error if the file doesn't exist or contains invalid JSON.
    #>
    [CmdletBinding()]
    [OutputType([PSCustomObject])]
    param(
        [Parameter(Position = 0, ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [Alias('Path', 'Config')]
        [string]$ConfigPath
    )

    BEGIN {
        # Set default config path if not provided
        if (-not $ConfigPath) {
            $ConfigPath = Join-Path $PSScriptRoot '..' '..' 'src' 'filter-compiler' 'compiler-config.json'
            $ConfigPath = [System.IO.Path]::GetFullPath($ConfigPath)
        }
    }

    PROCESS {
        Write-CompilerLog -Level DEBUG -Message "Reading configuration from: $ConfigPath"

        if (-not (Test-Path $ConfigPath)) {
            $errorMessage = "Configuration file not found: $ConfigPath"
            Write-CompilerLog -Level ERROR -Message $errorMessage
            throw [System.IO.FileNotFoundException]::new($errorMessage)
        }

        try {
            $fileContent = Get-Content -Path $ConfigPath -Raw -Encoding UTF8
            $config = $fileContent | ConvertFrom-Json

            Write-CompilerLog -Level DEBUG -Message "Configuration parsed successfully"
            Write-CompilerLog -Level INFO -Message "Loaded configuration: $($config.name) v$($config.version)"

            return $config
        }
        catch [System.ArgumentException] {
            $errorMessage = "Invalid JSON in configuration file: $($_.Exception.Message)"
            Write-CompilerLog -Level ERROR -Message $errorMessage
            throw [System.FormatException]::new($errorMessage)
        }
    }
}

function Invoke-FilterCompiler {
    <#
    .SYNOPSIS
    Compiles filter rules using the AdGuard hostlist compiler.

    .DESCRIPTION
    Invokes the @adguard/hostlist-compiler CLI to compile filter rules based on
    the provided configuration. This function mirrors the TypeScript compileFilters() function.

    .PARAMETER ConfigPath
    Path to the configuration JSON file.

    .PARAMETER OutputPath
    Path where the compiled output will be written.

    .PARAMETER WorkingDirectory
    Working directory for the compiler. Defaults to the filter-compiler directory.

    .OUTPUTS
    [PSCustomObject] An object containing:
    - Success: Boolean indicating if compilation succeeded
    - RuleCount: Number of rules in the output (if successful)
    - OutputPath: Path to the output file
    - Hash: SHA384 hash of the output file
    - ElapsedMs: Time taken in milliseconds

    .EXAMPLE
    $result = Invoke-FilterCompiler -ConfigPath './config.json' -OutputPath './output.txt'

    .EXAMPLE
    Invoke-FilterCompiler | Format-List

    .NOTES
    Requires @adguard/hostlist-compiler to be installed globally via npm.
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
        [string]$WorkingDirectory
    )

    BEGIN {
        $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()

        # Set defaults
        if (-not $WorkingDirectory) {
            $WorkingDirectory = Join-Path $PSScriptRoot '..' '..' 'src' 'filter-compiler'
            $WorkingDirectory = [System.IO.Path]::GetFullPath($WorkingDirectory)
        }

        if (-not $ConfigPath) {
            $ConfigPath = 'compiler-config.json'
        }

        if (-not $OutputPath) {
            $OutputPath = 'adguard_user_filter.txt'
        }
    }

    PROCESS {
        Write-CompilerLog -Level INFO -Message "Starting filter compilation..."
        Write-CompilerLog -Level DEBUG -Message "Working directory: $WorkingDirectory"
        Write-CompilerLog -Level DEBUG -Message "Config file: $ConfigPath"
        Write-CompilerLog -Level DEBUG -Message "Output file: $OutputPath"

        # Verify hostlist-compiler is available (cross-platform)
        $compilerPath = Get-CommandPath -CommandName 'hostlist-compiler'
        if (-not $compilerPath) {
            # Try using npx as fallback (works if hostlist-compiler is installed locally)
            $npxPath = Get-CommandPath -CommandName 'npx'
            if ($npxPath) {
                Write-CompilerLog -Level DEBUG -Message "hostlist-compiler not found globally, will try npx"
            }
            else {
                $errorMessage = "hostlist-compiler not found. Install with: npm install -g @adguard/hostlist-compiler"
                Write-CompilerLog -Level ERROR -Message $errorMessage
                throw [System.IO.FileNotFoundException]::new($errorMessage)
            }
        }

        # Log platform info for debugging
        $platformInfo = Get-PlatformInfo
        Write-CompilerLog -Level DEBUG -Message "Platform: $($platformInfo.Platform), OS: $($platformInfo.OS)"

        # Save current location and change to working directory
        $originalLocation = Get-Location
        try {
            Set-Location -Path $WorkingDirectory

            # Build the full output path for the result object
            $fullOutputPath = Join-Path $WorkingDirectory $OutputPath

            # Run the compiler (try global install first, then npx)
            Write-CompilerLog -Level INFO -Message "Invoking hostlist-compiler..."
            if ($compilerPath) {
                $compilerOutput = & hostlist-compiler --config $ConfigPath --output $OutputPath 2>&1
            }
            else {
                # Use npx to run the compiler
                Write-CompilerLog -Level DEBUG -Message "Using npx to invoke hostlist-compiler"
                $compilerOutput = & npx hostlist-compiler --config $ConfigPath --output $OutputPath 2>&1
            }

            if ($LASTEXITCODE -ne 0) {
                $errorMessage = "Compilation failed with exit code $LASTEXITCODE"
                Write-CompilerLog -Level ERROR -Message $errorMessage
                Write-CompilerLog -Level ERROR -Message "Compiler output: $compilerOutput"

                return [PSCustomObject]@{
                    Success      = $false
                    RuleCount    = 0
                    OutputPath   = $fullOutputPath
                    Hash         = $null
                    ElapsedMs    = $stopwatch.ElapsedMilliseconds
                    ErrorMessage = $errorMessage
                    CompilerOutput = $compilerOutput
                }
            }

            # Get rule count and file hash
            $ruleCount = 0
            $fileHash = $null

            if (Test-Path $OutputPath) {
                # Use UTF8 encoding explicitly for cross-platform consistency
                $content = Get-Content -Path $OutputPath -Encoding UTF8
                $ruleCount = ($content | Where-Object { $_ -and -not $_.StartsWith('!') }).Count
                $fileHash = (Get-FileHash -Path $OutputPath -Algorithm SHA384).Hash
            }

            $stopwatch.Stop()
            Write-CompilerLog -Level INFO -Message "Compilation complete. Generated $ruleCount rules in $($stopwatch.ElapsedMilliseconds)ms"

            return [PSCustomObject]@{
                Success      = $true
                RuleCount    = $ruleCount
                OutputPath   = $fullOutputPath
                Hash         = $fileHash
                ElapsedMs    = $stopwatch.ElapsedMilliseconds
                ErrorMessage = $null
                CompilerOutput = $compilerOutput
            }
        }
        finally {
            Set-Location -Path $originalLocation
        }
    }
}

function Write-CompiledOutput {
    <#
    .SYNOPSIS
    Writes or copies compiled filter rules to a destination.

    .DESCRIPTION
    Copies the compiled output file to a specified destination, typically the rules directory.
    This function mirrors the TypeScript writeOutput() function but operates on existing files.

    .PARAMETER SourcePath
    Path to the source compiled file.

    .PARAMETER DestinationPath
    Path where the file should be copied.

    .PARAMETER Force
    Overwrite the destination file if it exists.

    .OUTPUTS
    [PSCustomObject] An object containing:
    - Success: Boolean indicating if the operation succeeded
    - SourcePath: Path to the source file
    - DestinationPath: Path to the destination file
    - BytesCopied: Size of the copied file in bytes

    .EXAMPLE
    Write-CompiledOutput -SourcePath './output.txt' -DestinationPath '../rules/adguard_user_filter.txt'

    .EXAMPLE
    $result = Invoke-FilterCompiler
    $result | Write-CompiledOutput -DestinationPath '../rules/adguard_user_filter.txt'
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
        # Set default destination path if not provided
        if (-not $DestinationPath) {
            $DestinationPath = Join-Path $PSScriptRoot '..' '..' 'rules' 'adguard_user_filter.txt'
            $DestinationPath = [System.IO.Path]::GetFullPath($DestinationPath)
        }
    }

    PROCESS {
        Write-CompilerLog -Level DEBUG -Message "Copying output from: $SourcePath"
        Write-CompilerLog -Level DEBUG -Message "Copying output to: $DestinationPath"

        if (-not (Test-Path $SourcePath)) {
            $errorMessage = "Source file not found: $SourcePath"
            Write-CompilerLog -Level ERROR -Message $errorMessage
            throw [System.IO.FileNotFoundException]::new($errorMessage)
        }

        try {
            # Ensure destination directory exists
            $destinationDir = Split-Path -Path $DestinationPath -Parent
            if (-not (Test-Path $destinationDir)) {
                New-Item -ItemType Directory -Path $destinationDir -Force | Out-Null
            }

            # Copy the file
            Copy-Item -Path $SourcePath -Destination $DestinationPath -Force:$Force

            $fileInfo = Get-Item $DestinationPath
            # Use UTF8 encoding explicitly for cross-platform consistency
            $lineCount = (Get-Content $DestinationPath -Encoding UTF8).Count

            Write-CompilerLog -Level INFO -Message "Successfully wrote $lineCount lines to $DestinationPath"

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
            Write-CompilerLog -Level ERROR -Message $errorMessage
            throw
        }
    }
}

function Invoke-RulesCompiler {
    <#
    .SYNOPSIS
    Main entry point for the filter compiler.

    .DESCRIPTION
    Orchestrates the complete compilation pipeline:
    1. Reads the configuration from compiler-config.json
    2. Compiles the filters using @adguard/hostlist-compiler
    3. Optionally copies the output to the rules directory

    This function mirrors the TypeScript main() function.

    .PARAMETER ConfigPath
    Path to the configuration JSON file. Defaults to the standard location.

    .PARAMETER OutputPath
    Path for the compiled output file. Defaults to 'adguard_user_filter.txt'.

    .PARAMETER CopyToRules
    If specified, copies the compiled output to the rules directory.

    .PARAMETER RulesPath
    Custom path for the rules directory. Only used if -CopyToRules is specified.

    .OUTPUTS
    [PSCustomObject] An object containing the compilation results.

    .EXAMPLE
    Invoke-RulesCompiler

    .EXAMPLE
    Invoke-RulesCompiler -CopyToRules

    .EXAMPLE
    Invoke-RulesCompiler -ConfigPath './custom-config.json' -CopyToRules -Verbose

    .NOTES
    This is the primary function to use when compiling filter rules.
    It combines Read-CompilerConfiguration, Invoke-FilterCompiler, and Write-CompiledOutput.
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
    }

    PROCESS {
        Write-CompilerLog -Level INFO -Message "AdGuard Filter Compiler starting..."

        try {
            # Step 1: Read configuration
            Write-CompilerLog -Level INFO -Message "Step 1/3: Reading configuration..."
            $config = Read-CompilerConfiguration -ConfigPath $ConfigPath
            Write-CompilerLog -Level INFO -Message "Configuration loaded: $($config.name)"

            # Step 2: Compile filters
            Write-CompilerLog -Level INFO -Message "Step 2/3: Compiling filters..."
            $compileResult = Invoke-FilterCompiler -ConfigPath $ConfigPath -OutputPath $OutputPath

            if (-not $compileResult.Success) {
                throw "Compilation failed: $($compileResult.ErrorMessage)"
            }

            # Step 3: Copy to rules directory (if requested)
            $copyResult = $null
            if ($CopyToRules) {
                Write-CompilerLog -Level INFO -Message "Step 3/3: Copying to rules directory..."
                $copyParams = @{
                    SourcePath = $compileResult.OutputPath
                    Force      = $true
                }
                if ($RulesPath) {
                    $copyParams['DestinationPath'] = $RulesPath
                }
                $copyResult = Write-CompiledOutput @copyParams
            }
            else {
                Write-CompilerLog -Level INFO -Message "Step 3/3: Skipping copy to rules (use -CopyToRules to enable)"
            }

            $stopwatch.Stop()
            $elapsed = $stopwatch.ElapsedMilliseconds

            Write-CompilerLog -Level INFO -Message "Compilation completed successfully in ${elapsed}ms"

            return [PSCustomObject]@{
                Success           = $true
                ConfigName        = $config.name
                ConfigVersion     = $config.version
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
            Write-CompilerLog -Level ERROR -Message "Compilation failed: $($_.Exception.Message)"

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

    .DESCRIPTION
    Returns version information for the PowerShell module and the underlying
    hostlist-compiler CLI tool.

    .OUTPUTS
    [PSCustomObject] An object containing version information.

    .EXAMPLE
    Get-CompilerVersion

    .EXAMPLE
    Get-CompilerVersion | Format-List
    #>
    [CmdletBinding()]
    [OutputType([PSCustomObject])]
    param()

    PROCESS {
        $moduleVersion = '1.0.0'
        $hostlistVersion = $null
        $nodeVersion = $null
        $npmVersion = $null

        # Get platform information
        $platformInfo = Get-PlatformInfo

        # Get hostlist-compiler version
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

        # Get Node.js version
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

        # Get npm version
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
