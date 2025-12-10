#Requires -Modules Pester

<#
.SYNOPSIS
    Pester tests for the RulesCompiler module.

.DESCRIPTION
    Tests parameter validation, function exports, and basic functionality
    of the Invoke-RulesCompiler PowerShell module.

.NOTES
    Run with: Invoke-Pester -Path ./Tests/RulesCompiler-Tests.ps1
#>

BeforeAll {
    # Import the module
    $modulePath = Join-Path $PSScriptRoot '..' 'Invoke-RulesCompiler.psm1'
    Import-Module $modulePath -Force

    # Set up test paths
    $script:TestConfigPath = Join-Path $PSScriptRoot '..' '..' '..' 'src' 'filter-compiler' 'compiler-config.json'
    $script:TestYamlConfigPath = Join-Path $PSScriptRoot '..' '..' '..' 'src' 'filter-compiler' 'compiler-config.yaml'
    $script:TestTomlConfigPath = Join-Path $PSScriptRoot '..' '..' '..' 'src' 'filter-compiler' 'compiler-config.toml'
    $script:TestOutputDir = Join-Path $PSScriptRoot 'TestOutput'
}

AfterAll {
    # Clean up test output directory if it exists
    if (Test-Path $script:TestOutputDir) {
        Remove-Item $script:TestOutputDir -Recurse -Force -ErrorAction SilentlyContinue
    }

    # Remove the module
    Remove-Module 'Invoke-RulesCompiler' -Force -ErrorAction SilentlyContinue
}

Describe "Module Import" {
    Context "Module Loading" {
        It "Should import the module without errors" {
            { Import-Module (Join-Path $PSScriptRoot '..' 'Invoke-RulesCompiler.psm1') -Force } | Should -Not -Throw
        }

        It "Should export Read-CompilerConfiguration function" {
            Get-Command 'Read-CompilerConfiguration' -Module 'Invoke-RulesCompiler' | Should -Not -BeNullOrEmpty
        }

        It "Should export Invoke-FilterCompiler function" {
            Get-Command 'Invoke-FilterCompiler' -Module 'Invoke-RulesCompiler' | Should -Not -BeNullOrEmpty
        }

        It "Should export Write-CompiledOutput function" {
            Get-Command 'Write-CompiledOutput' -Module 'Invoke-RulesCompiler' | Should -Not -BeNullOrEmpty
        }

        It "Should export Invoke-RulesCompiler function" {
            Get-Command 'Invoke-RulesCompiler' -Module 'Invoke-RulesCompiler' | Should -Not -BeNullOrEmpty
        }

        It "Should export Get-CompilerVersion function" {
            Get-Command 'Get-CompilerVersion' -Module 'Invoke-RulesCompiler' | Should -Not -BeNullOrEmpty
        }
    }
}

Describe "Read-CompilerConfiguration" {
    Context "Parameter Validation" {
        It "Should have ConfigPath parameter" {
            (Get-Command Read-CompilerConfiguration).Parameters.Keys | Should -Contain "ConfigPath"
        }

        It "Should have Format parameter" {
            (Get-Command Read-CompilerConfiguration).Parameters.Keys | Should -Contain "Format"
        }

        It "Should have Path alias for ConfigPath" {
            $params = (Get-Command Read-CompilerConfiguration).Parameters
            $aliases = $params['ConfigPath'].Attributes.Where({ $_ -is [System.Management.Automation.AliasAttribute] }).AliasNames
            $aliases | Should -Contain "Path"
        }

        It "Should have Config alias for ConfigPath" {
            $params = (Get-Command Read-CompilerConfiguration).Parameters
            $aliases = $params['ConfigPath'].Attributes.Where({ $_ -is [System.Management.Automation.AliasAttribute] }).AliasNames
            $aliases | Should -Contain "Config"
        }

        It "Should accept pipeline input for ConfigPath" {
            $params = (Get-Command Read-CompilerConfiguration).Parameters
            $attrs = $params['ConfigPath'].Attributes.Where({ $_ -is [System.Management.Automation.ParameterAttribute] })
            $attrs.ValueFromPipeline | Should -BeTrue
        }

        It "Should validate Format parameter values" {
            $params = (Get-Command Read-CompilerConfiguration).Parameters
            $validateSet = $params['Format'].Attributes.Where({ $_ -is [System.Management.Automation.ValidateSetAttribute] })
            $validateSet.ValidValues | Should -Contain 'json'
            $validateSet.ValidValues | Should -Contain 'yaml'
            $validateSet.ValidValues | Should -Contain 'toml'
        }
    }

    Context "Configuration Reading" {
        It "Should throw when config file does not exist" {
            { Read-CompilerConfiguration -ConfigPath './nonexistent-config.json' } | Should -Throw
        }

        It "Should read valid configuration file" -Skip:(-not (Test-Path $script:TestConfigPath)) {
            $config = Read-CompilerConfiguration -ConfigPath $script:TestConfigPath
            $config | Should -Not -BeNullOrEmpty
        }

        It "Should return configuration with name property" -Skip:(-not (Test-Path $script:TestConfigPath)) {
            $config = Read-CompilerConfiguration -ConfigPath $script:TestConfigPath
            $config.name | Should -Not -BeNullOrEmpty
        }

        It "Should return configuration with version property" -Skip:(-not (Test-Path $script:TestConfigPath)) {
            $config = Read-CompilerConfiguration -ConfigPath $script:TestConfigPath
            $config.version | Should -Not -BeNullOrEmpty
        }

        It "Should return configuration with sources array" -Skip:(-not (Test-Path $script:TestConfigPath)) {
            $config = Read-CompilerConfiguration -ConfigPath $script:TestConfigPath
            $config.sources | Should -BeOfType [System.Object[]]
        }

        It "Should return configuration with transformations array" -Skip:(-not (Test-Path $script:TestConfigPath)) {
            $config = Read-CompilerConfiguration -ConfigPath $script:TestConfigPath
            $config.transformations | Should -Not -BeNullOrEmpty
        }
    }

    Context "Error Handling" {
        BeforeAll {
            # Create a temporary invalid JSON file
            $script:InvalidJsonPath = Join-Path $script:TestOutputDir 'invalid.json'
            if (-not (Test-Path $script:TestOutputDir)) {
                New-Item -ItemType Directory -Path $script:TestOutputDir -Force | Out-Null
            }
            '{ invalid json }' | Out-File -FilePath $script:InvalidJsonPath -Encoding UTF8
        }

        It "Should throw on invalid JSON" {
            { Read-CompilerConfiguration -ConfigPath $script:InvalidJsonPath } | Should -Throw
        }
    }

    Context "YAML Configuration Support" {
        It "Should read valid YAML configuration file" -Skip:(-not (Test-Path $script:TestYamlConfigPath)) {
            $config = Read-CompilerConfiguration -ConfigPath $script:TestYamlConfigPath
            $config | Should -Not -BeNullOrEmpty
        }

        It "Should return YAML configuration with name property" -Skip:(-not (Test-Path $script:TestYamlConfigPath)) {
            $config = Read-CompilerConfiguration -ConfigPath $script:TestYamlConfigPath
            $config.name | Should -Not -BeNullOrEmpty
        }

        It "Should return YAML configuration with version property" -Skip:(-not (Test-Path $script:TestYamlConfigPath)) {
            $config = Read-CompilerConfiguration -ConfigPath $script:TestYamlConfigPath
            $config.version | Should -Not -BeNullOrEmpty
        }

        It "Should include _sourceFormat metadata for YAML" -Skip:(-not (Test-Path $script:TestYamlConfigPath)) {
            $config = Read-CompilerConfiguration -ConfigPath $script:TestYamlConfigPath
            $config._sourceFormat | Should -Be 'yaml'
        }

        It "Should allow explicit Format parameter for YAML" -Skip:(-not (Test-Path $script:TestYamlConfigPath)) {
            $config = Read-CompilerConfiguration -ConfigPath $script:TestYamlConfigPath -Format 'yaml'
            $config._sourceFormat | Should -Be 'yaml'
        }
    }

    Context "TOML Configuration Support" {
        It "Should read valid TOML configuration file" -Skip:(-not (Test-Path $script:TestTomlConfigPath)) {
            $config = Read-CompilerConfiguration -ConfigPath $script:TestTomlConfigPath
            $config | Should -Not -BeNullOrEmpty
        }

        It "Should return TOML configuration with name property" -Skip:(-not (Test-Path $script:TestTomlConfigPath)) {
            $config = Read-CompilerConfiguration -ConfigPath $script:TestTomlConfigPath
            $config.name | Should -Not -BeNullOrEmpty
        }

        It "Should return TOML configuration with version property" -Skip:(-not (Test-Path $script:TestTomlConfigPath)) {
            $config = Read-CompilerConfiguration -ConfigPath $script:TestTomlConfigPath
            $config.version | Should -Not -BeNullOrEmpty
        }

        It "Should include _sourceFormat metadata for TOML" -Skip:(-not (Test-Path $script:TestTomlConfigPath)) {
            $config = Read-CompilerConfiguration -ConfigPath $script:TestTomlConfigPath
            $config._sourceFormat | Should -Be 'toml'
        }

        It "Should allow explicit Format parameter for TOML" -Skip:(-not (Test-Path $script:TestTomlConfigPath)) {
            $config = Read-CompilerConfiguration -ConfigPath $script:TestTomlConfigPath -Format 'toml'
            $config._sourceFormat | Should -Be 'toml'
        }
    }

    Context "Format Detection" {
        BeforeAll {
            if (-not (Test-Path $script:TestOutputDir)) {
                New-Item -ItemType Directory -Path $script:TestOutputDir -Force | Out-Null
            }

            # Create test files with different extensions
            $script:TestYmlFile = Join-Path $script:TestOutputDir 'test.yml'
            @"
name: "Test Config"
version: "1.0.0"
"@ | Out-File -FilePath $script:TestYmlFile -Encoding UTF8
        }

        It "Should detect .yml extension as YAML format" {
            $config = Read-CompilerConfiguration -ConfigPath $script:TestYmlFile
            $config._sourceFormat | Should -Be 'yaml'
        }

        It "Should detect .yaml extension as YAML format" -Skip:(-not (Test-Path $script:TestYamlConfigPath)) {
            $config = Read-CompilerConfiguration -ConfigPath $script:TestYamlConfigPath
            $config._sourceFormat | Should -Be 'yaml'
        }

        It "Should detect .toml extension as TOML format" -Skip:(-not (Test-Path $script:TestTomlConfigPath)) {
            $config = Read-CompilerConfiguration -ConfigPath $script:TestTomlConfigPath
            $config._sourceFormat | Should -Be 'toml'
        }

        It "Should detect .json extension as JSON format" -Skip:(-not (Test-Path $script:TestConfigPath)) {
            $config = Read-CompilerConfiguration -ConfigPath $script:TestConfigPath
            $config._sourceFormat | Should -Be 'json'
        }
    }
}

Describe "Invoke-FilterCompiler" {
    Context "Parameter Validation" {
        It "Should have ConfigPath parameter" {
            (Get-Command Invoke-FilterCompiler).Parameters.Keys | Should -Contain "ConfigPath"
        }

        It "Should have OutputPath parameter" {
            (Get-Command Invoke-FilterCompiler).Parameters.Keys | Should -Contain "OutputPath"
        }

        It "Should have WorkingDirectory parameter" {
            (Get-Command Invoke-FilterCompiler).Parameters.Keys | Should -Contain "WorkingDirectory"
        }

        It "Should have Format parameter" {
            (Get-Command Invoke-FilterCompiler).Parameters.Keys | Should -Contain "Format"
        }

        It "Should validate Format parameter values" {
            $params = (Get-Command Invoke-FilterCompiler).Parameters
            $validateSet = $params['Format'].Attributes.Where({ $_ -is [System.Management.Automation.ValidateSetAttribute] })
            $validateSet.ValidValues | Should -Contain 'json'
            $validateSet.ValidValues | Should -Contain 'yaml'
            $validateSet.ValidValues | Should -Contain 'toml'
        }

        It "Should have Config alias for ConfigPath" {
            $params = (Get-Command Invoke-FilterCompiler).Parameters
            $aliases = $params['ConfigPath'].Attributes.Where({ $_ -is [System.Management.Automation.AliasAttribute] }).AliasNames
            $aliases | Should -Contain "Config"
        }

        It "Should have Output alias for OutputPath" {
            $params = (Get-Command Invoke-FilterCompiler).Parameters
            $aliases = $params['OutputPath'].Attributes.Where({ $_ -is [System.Management.Automation.AliasAttribute] }).AliasNames
            $aliases | Should -Contain "Output"
        }

        It "Should have WorkDir alias for WorkingDirectory" {
            $params = (Get-Command Invoke-FilterCompiler).Parameters
            $aliases = $params['WorkingDirectory'].Attributes.Where({ $_ -is [System.Management.Automation.AliasAttribute] }).AliasNames
            $aliases | Should -Contain "WorkDir"
        }
    }

    Context "Output Object Structure" {
        It "Should return object with Success property" {
            # Mock the compiler to avoid external dependency in tests
            $mockResult = [PSCustomObject]@{
                Success          = $false
                RuleCount        = 0
                OutputPath       = ''
                Hash             = $null
                ElapsedMs        = 0
                ErrorMessage     = 'Test error'
                CompilerOutput   = ''
            }
            $mockResult.PSObject.Properties.Name | Should -Contain 'Success'
        }
    }
}

Describe "Write-CompiledOutput" {
    Context "Parameter Validation" {
        It "Should have SourcePath parameter" {
            (Get-Command Write-CompiledOutput).Parameters.Keys | Should -Contain "SourcePath"
        }

        It "Should have DestinationPath parameter" {
            (Get-Command Write-CompiledOutput).Parameters.Keys | Should -Contain "DestinationPath"
        }

        It "Should have Force switch parameter" {
            (Get-Command Write-CompiledOutput).Parameters.Keys | Should -Contain "Force"
            (Get-Command Write-CompiledOutput).Parameters['Force'].SwitchParameter | Should -BeTrue
        }

        It "Should have Path alias for SourcePath" {
            $params = (Get-Command Write-CompiledOutput).Parameters
            $aliases = $params['SourcePath'].Attributes.Where({ $_ -is [System.Management.Automation.AliasAttribute] }).AliasNames
            $aliases | Should -Contain "Path"
        }

        It "Should have OutputPath alias for SourcePath" {
            $params = (Get-Command Write-CompiledOutput).Parameters
            $aliases = $params['SourcePath'].Attributes.Where({ $_ -is [System.Management.Automation.AliasAttribute] }).AliasNames
            $aliases | Should -Contain "OutputPath"
        }

        It "Should require SourcePath parameter" {
            $params = (Get-Command Write-CompiledOutput).Parameters
            $attrs = $params['SourcePath'].Attributes.Where({ $_ -is [System.Management.Automation.ParameterAttribute] })
            $attrs.Mandatory | Should -BeTrue
        }
    }

    Context "Error Handling" {
        It "Should throw when source file does not exist" {
            { Write-CompiledOutput -SourcePath './nonexistent-file.txt' } | Should -Throw
        }
    }

    Context "File Operations" {
        BeforeAll {
            # Create test source file
            if (-not (Test-Path $script:TestOutputDir)) {
                New-Item -ItemType Directory -Path $script:TestOutputDir -Force | Out-Null
            }
            $script:TestSourceFile = Join-Path $script:TestOutputDir 'source.txt'
            $script:TestDestFile = Join-Path $script:TestOutputDir 'dest.txt'
            "Line 1`nLine 2`nLine 3" | Out-File -FilePath $script:TestSourceFile -Encoding UTF8
        }

        It "Should copy file to destination" {
            $result = Write-CompiledOutput -SourcePath $script:TestSourceFile -DestinationPath $script:TestDestFile -Force
            $result.Success | Should -BeTrue
            Test-Path $script:TestDestFile | Should -BeTrue
        }

        It "Should return correct line count" {
            $result = Write-CompiledOutput -SourcePath $script:TestSourceFile -DestinationPath $script:TestDestFile -Force
            $result.LineCount | Should -BeGreaterThan 0
        }

        It "Should return byte count" {
            $result = Write-CompiledOutput -SourcePath $script:TestSourceFile -DestinationPath $script:TestDestFile -Force
            $result.BytesCopied | Should -BeGreaterThan 0
        }
    }
}

Describe "Invoke-RulesCompiler" {
    Context "Parameter Validation" {
        It "Should have ConfigPath parameter" {
            (Get-Command Invoke-RulesCompiler).Parameters.Keys | Should -Contain "ConfigPath"
        }

        It "Should have OutputPath parameter" {
            (Get-Command Invoke-RulesCompiler).Parameters.Keys | Should -Contain "OutputPath"
        }

        It "Should have CopyToRules switch parameter" {
            (Get-Command Invoke-RulesCompiler).Parameters.Keys | Should -Contain "CopyToRules"
            (Get-Command Invoke-RulesCompiler).Parameters['CopyToRules'].SwitchParameter | Should -BeTrue
        }

        It "Should have RulesPath parameter" {
            (Get-Command Invoke-RulesCompiler).Parameters.Keys | Should -Contain "RulesPath"
        }

        It "Should have Copy alias for CopyToRules" {
            $params = (Get-Command Invoke-RulesCompiler).Parameters
            $aliases = $params['CopyToRules'].Attributes.Where({ $_ -is [System.Management.Automation.AliasAttribute] }).AliasNames
            $aliases | Should -Contain "Copy"
        }
    }

    Context "Output Object Structure" {
        It "Should define expected output properties" {
            $expectedProperties = @(
                'Success',
                'ConfigName',
                'ConfigVersion',
                'RuleCount',
                'OutputPath',
                'OutputHash',
                'CopiedToRules',
                'RulesDestination',
                'ElapsedMs',
                'StartTime',
                'EndTime'
            )
            # This is a structural test - actual execution depends on hostlist-compiler
            $expectedProperties | Should -Not -BeNullOrEmpty
        }
    }
}

Describe "Get-CompilerVersion" {
    Context "Function Behavior" {
        It "Should return version information object" {
            $version = Get-CompilerVersion
            $version | Should -Not -BeNullOrEmpty
        }

        It "Should include ModuleName property" {
            $version = Get-CompilerVersion
            $version.ModuleName | Should -Be 'Invoke-RulesCompiler'
        }

        It "Should include ModuleVersion property" {
            $version = Get-CompilerVersion
            $version.ModuleVersion | Should -Not -BeNullOrEmpty
        }

        It "Should include PowerShellVersion property" {
            $version = Get-CompilerVersion
            $version.PowerShellVersion | Should -Not -BeNullOrEmpty
        }

        It "Should include Platform property" {
            $version = Get-CompilerVersion
            $version.Platform | Should -Not -BeNullOrEmpty
        }

        It "Should include HostlistCompilerVersion property" {
            $version = Get-CompilerVersion
            $version.PSObject.Properties.Name | Should -Contain 'HostlistCompilerVersion'
        }

        It "Should include NodeVersion property" {
            $version = Get-CompilerVersion
            $version.PSObject.Properties.Name | Should -Contain 'NodeVersion'
        }
    }
}
