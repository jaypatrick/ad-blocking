#Requires -Version 7.0

<#
.SYNOPSIS
    PowerShell class for managing compiler configuration.

.DESCRIPTION
    The CompilerConfiguration class encapsulates compiler settings including name,
    version, sources, and transformations. Supports loading from JSON, YAML, and TOML
    files as well as environment variables.

.NOTES
    Author: Jayson Knight
    GitHub: jaypatrick
#>

class CompilerConfiguration {
    # Properties
    [string]$Name
    [string]$Version
    [string]$Description
    [object[]]$Sources
    [string[]]$Transformations
    [string]$SourceFormat
    [string]$SourcePath

    # Default Constructor
    CompilerConfiguration() {
        $this.Name = ''
        $this.Version = '1.0.0'
        $this.Description = ''
        $this.Sources = @()
        $this.Transformations = @()
        $this.SourceFormat = 'json'
        $this.SourcePath = $null
    }

    # Constructor with file path
    CompilerConfiguration([string]$filePath) {
        $this.Name = ''
        $this.Version = '1.0.0'
        $this.Description = ''
        $this.Sources = @()
        $this.Transformations = @()
        $this.SourceFormat = 'json'
        $this.SourcePath = $null
        
        $this.LoadFromFile($filePath)
    }

    # Load from file
    [void] LoadFromFile([string]$path) {
        if (-not (Test-Path $path)) {
            throw "Configuration file not found: $path"
        }

        $this.SourcePath = [System.IO.Path]::GetFullPath($path)
        $extension = [System.IO.Path]::GetExtension($path).ToLower()

        # Determine format
        $format = switch ($extension) {
            '.json' { 'json' }
            { $_ -in '.yaml', '.yml' } { 'yaml' }
            '.toml' { 'toml' }
            default { throw "Unsupported configuration file format: $extension" }
        }

        $this.SourceFormat = $format
        $content = Get-Content -Path $path -Raw -Encoding UTF8

        # Parse based on format
        $config = $null
        switch ($format) {
            'json' {
                $config = $content | ConvertFrom-Json
            }
            'yaml' {
                $config = $this.ParseYaml($content)
            }
            'toml' {
                $config = $this.ParseToml($content)
            }
        }

        # Apply configuration
        if ($config.name) { $this.Name = $config.name }
        if ($config.version) { $this.Version = $config.version }
        if ($config.description) { $this.Description = $config.description }
        if ($config.sources) { $this.Sources = $config.sources }
        if ($config.transformations) { $this.Transformations = $config.transformations }
    }

    # Load from environment variables
    [void] LoadFromEnvironment() {
        if ($env:ADGUARD_COMPILER_CONFIG) {
            $this.LoadFromFile($env:ADGUARD_COMPILER_CONFIG)
        }
    }

    # Parse YAML content
    hidden [object] ParseYaml([string]$content) {
        # Try to use powershell-yaml module if available
        if (Get-Module -ListAvailable -Name 'powershell-yaml') {
            Import-Module 'powershell-yaml' -ErrorAction SilentlyContinue
            if (Get-Command 'ConvertFrom-Yaml' -Module 'powershell-yaml' -ErrorAction SilentlyContinue) {
                return powershell-yaml\ConvertFrom-Yaml -Yaml $content
            }
        }

        # Fallback: Basic YAML parser
        $result = [ordered]@{}
        $currentArray = $null
        $currentArrayName = $null

        $lines = $content -split "`n" | ForEach-Object { $_.TrimEnd("`r") }

        foreach ($line in $lines) {
            if ($line -match '^\s*$' -or $line -match '^\s*#') { continue }

            if ($line -match '^(\w+):\s*(.*)$') {
                $key = $Matches[1]
                $value = $Matches[2].Trim()

                if ($value -eq '' -or $null -eq $value) {
                    $currentArrayName = $key
                    $currentArray = @()
                }
                elseif ($value -match '^\[(.+)\]$') {
                    $result[$key] = $Matches[1] -split ',\s*' | ForEach-Object { $_.Trim(' "''') }
                }
                else {
                    $result[$key] = $value.Trim(' "''')
                }
            }
            elseif ($line -match '^\s*-\s*(.*)$' -and $currentArrayName) {
                $currentArray += $Matches[1].Trim()
            }
        }

        if ($currentArrayName -and $currentArray) {
            $result[$currentArrayName] = $currentArray
        }

        return [PSCustomObject]$result
    }

    # Parse TOML content
    hidden [object] ParseToml([string]$content) {
        # Try using Python's tomllib
        if (Get-Command python3 -ErrorAction SilentlyContinue) {
            $tempFile = [System.IO.Path]::GetTempFileName()
            try {
                Set-Content -Path $tempFile -Value $content -Encoding UTF8
                $jsonOutput = python3 -c @"
import sys, json
try:
    import tomllib
except ImportError:
    import toml as tomllib
with open('$($tempFile.Replace('\', '\\'))', 'r') as f:
    data = tomllib.load(f) if hasattr(tomllib, 'load') else tomllib.loads(f.read())
print(json.dumps(data))
"@
                return $jsonOutput | ConvertFrom-Json
            }
            finally {
                Remove-Item $tempFile -Force -ErrorAction SilentlyContinue
            }
        }

        throw "TOML parsing requires Python 3.11+ with tomllib or the toml package"
    }

    # Validate configuration
    [bool] Validate() {
        if ([string]::IsNullOrEmpty($this.Name)) {
            throw "Configuration must have a name"
        }

        if ([string]::IsNullOrEmpty($this.Version)) {
            throw "Configuration must have a version"
        }

        if ($null -eq $this.Sources -or $this.Sources.Count -eq 0) {
            throw "Configuration must have at least one source"
        }

        # Validate each source
        foreach ($source in $this.Sources) {
            if (-not $source.name) {
                throw "Each source must have a name"
            }
        }

        return $true
    }

    # Convert to hashtable
    [hashtable] ToHashtable() {
        return @{
            name            = $this.Name
            version         = $this.Version
            description     = $this.Description
            sources         = $this.Sources
            transformations = $this.Transformations
            _sourceFormat   = $this.SourceFormat
            _sourcePath     = $this.SourcePath
        }
    }

    # Convert to JSON
    [string] ToJson() {
        $hash = $this.ToHashtable()
        # Remove metadata properties
        $hash.Remove('_sourceFormat')
        $hash.Remove('_sourcePath')
        return $hash | ConvertTo-Json -Depth 10
    }

    # String representation
    [string] ToString() {
        return "CompilerConfiguration: $($this.Name) v$($this.Version) ($($this.Sources.Count) sources)"
    }

    # Save to file
    [void] SaveToFile([string]$path) {
        $directory = Split-Path -Path $path -Parent
        if ($directory -and -not (Test-Path $directory)) {
            New-Item -ItemType Directory -Path $directory -Force | Out-Null
        }

        $extension = [System.IO.Path]::GetExtension($path).ToLower()
        
        switch ($extension) {
            '.json' {
                $this.ToJson() | Set-Content -Path $path -Encoding UTF8
            }
            '.yaml' {
                if (Get-Module -ListAvailable -Name 'powershell-yaml') {
                    Import-Module 'powershell-yaml' -ErrorAction SilentlyContinue
                    $hash = $this.ToHashtable()
                    $hash.Remove('_sourceFormat')
                    $hash.Remove('_sourcePath')
                    $hash | ConvertTo-Yaml | Set-Content -Path $path -Encoding UTF8
                }
                else {
                    throw "Saving to YAML requires powershell-yaml module"
                }
            }
            default {
                throw "Can only save to .json or .yaml formats"
            }
        }
    }

    # Clone configuration
    [CompilerConfiguration] Clone() {
        $clone = [CompilerConfiguration]::new()
        $clone.Name = $this.Name
        $clone.Version = $this.Version
        $clone.Description = $this.Description
        $clone.Sources = $this.Sources.Clone()
        $clone.Transformations = $this.Transformations.Clone()
        $clone.SourceFormat = $this.SourceFormat
        $clone.SourcePath = $this.SourcePath
        return $clone
    }

    # Get source by name
    [object] GetSource([string]$name) {
        foreach ($source in $this.Sources) {
            if ($source.name -eq $name) {
                return $source
            }
        }
        return $null
    }

    # Add source
    [void] AddSource([object]$source) {
        if (-not $source.name) {
            throw "Source must have a name"
        }
        
        # Check if source already exists
        $existing = $this.GetSource($source.name)
        if ($existing) {
            throw "Source with name '$($source.name)' already exists"
        }
        
        $this.Sources += $source
    }

    # Remove source by name
    [bool] RemoveSource([string]$name) {
        $newSources = @()
        $found = $false
        
        foreach ($source in $this.Sources) {
            if ($source.name -eq $name) {
                $found = $true
            }
            else {
                $newSources += $source
            }
        }
        
        $this.Sources = $newSources
        return $found
    }

    # Static factory methods
    static [CompilerConfiguration] FromFile([string]$path) {
        return [CompilerConfiguration]::new($path)
    }

    static [CompilerConfiguration] FromEnvironment() {
        $config = [CompilerConfiguration]::new()
        $config.LoadFromEnvironment()
        return $config
    }

    static [CompilerConfiguration] FromHashtable([hashtable]$hash) {
        $config = [CompilerConfiguration]::new()
        
        if ($hash.name) { $config.Name = $hash.name }
        if ($hash.version) { $config.Version = $hash.version }
        if ($hash.description) { $config.Description = $hash.description }
        if ($hash.sources) { $config.Sources = $hash.sources }
        if ($hash.transformations) { $config.Transformations = $hash.transformations }
        
        return $config
    }
}

# Export the class
Export-ModuleMember -Variable CompilerConfiguration
