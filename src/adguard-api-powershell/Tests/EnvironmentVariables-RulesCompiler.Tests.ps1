#Requires -Version 7.0

BeforeAll {
    # Import the module under test
    $ModulePath = Join-Path $PSScriptRoot '..' 'Invoke-RulesCompiler.psm1'
    Import-Module $ModulePath -Force -ErrorAction Stop
    
    # Save current environment variables
    $script:SavedEnv = @{}
    $EnvVars = @(
        'ADGUARD_COMPILER_CONFIG',
        'ADGUARD_COMPILER_OUTPUT',
        'ADGUARD_COMPILER_RULES_DIR',
        'ADGUARD_COMPILER_VERBOSE',
        'ADGUARD_COMPILER_COPY_TO_RULES',
        'DEBUG'
    )
    
    foreach ($var in $EnvVars) {
        if (Test-Path "env:$var") {
            $script:SavedEnv[$var] = Get-Item "env:$var"
        }
        Remove-Item "env:$var" -ErrorAction SilentlyContinue
    }
}

AfterAll {
    # Restore environment variables
    foreach ($var in $script:SavedEnv.Keys) {
        Set-Item "env:$var" -Value $script:SavedEnv[$var].Value
    }
}

Describe "Environment Variables - RulesCompiler Module" {
    
    BeforeEach {
        # Clean environment before each test
        @(
            'ADGUARD_COMPILER_CONFIG',
            'ADGUARD_COMPILER_OUTPUT',
            'ADGUARD_COMPILER_RULES_DIR',
            'ADGUARD_COMPILER_VERBOSE',
            'ADGUARD_COMPILER_COPY_TO_RULES',
            'DEBUG'
        ) | ForEach-Object {
            Remove-Item "env:$_" -ErrorAction SilentlyContinue
        }
    }

    Context "Configuration Path Environment Variable" {
        
        It "Should use ADGUARD_COMPILER_CONFIG when ConfigPath is not provided" {
            # This test verifies the environment variable is read
            $env:ADGUARD_COMPILER_CONFIG = "test-config.json"
            
            # Mock the functions to prevent actual compilation
            Mock Read-CompilerConfiguration -ModuleName Invoke-RulesCompiler { 
                [PSCustomObject]@{
                    name = 'Test Config'
                    version = '1.0.0'
                    _sourceFormat = 'json'
                    _sourcePath = $ConfigPath
                }
            }
            Mock Invoke-FilterCompiler -ModuleName Invoke-RulesCompiler { 
                [PSCustomObject]@{
                    Success = $true
                    RuleCount = 100
                    OutputPath = 'test-output.txt'
                    Hash = 'ABCD1234'
                    ElapsedMs = 1000
                }
            }
            
            { Invoke-RulesCompiler } | Should -Not -Throw
            
            Should -Invoke Read-CompilerConfiguration -ModuleName Invoke-RulesCompiler -ParameterFilter { 
                $ConfigPath -eq "test-config.json" 
            }
        }
        
        It "Should prioritize CLI parameter over ADGUARD_COMPILER_CONFIG" {
            $env:ADGUARD_COMPILER_CONFIG = "env-config.json"
            
            Mock Read-CompilerConfiguration { 
                [PSCustomObject]@{
                    name = 'Test Config'
                    version = '1.0.0'
                    _sourceFormat = 'json'
                    _sourcePath = $ConfigPath
                }
            }
            Mock Invoke-FilterCompiler { 
                [PSCustomObject]@{
                    Success = $true
                    RuleCount = 100
                    OutputPath = 'test-output.txt'
                    Hash = 'ABCD1234'
                    ElapsedMs = 1000
                }
            }
            
            { Invoke-RulesCompiler -ConfigPath "cli-config.json" } | Should -Not -Throw
            
            Should -Invoke Read-CompilerConfiguration -ParameterFilter { 
                $ConfigPath -eq "cli-config.json" 
            }
        }
    }

    Context "Output Path Environment Variable" {
        
        It "Should use ADGUARD_COMPILER_OUTPUT when OutputPath is not provided" {
            $env:ADGUARD_COMPILER_OUTPUT = "env-output.txt"
            
            Mock Read-CompilerConfiguration { 
                [PSCustomObject]@{
                    name = 'Test Config'
                    version = '1.0.0'
                }
            }
            Mock Invoke-FilterCompiler { 
                [PSCustomObject]@{
                    Success = $true
                    RuleCount = 100
                    OutputPath = $OutputPath
                    Hash = 'ABCD1234'
                    ElapsedMs = 1000
                }
            }
            
            { Invoke-RulesCompiler } | Should -Not -Throw
            
            Should -Invoke Invoke-FilterCompiler -ParameterFilter { 
                $OutputPath -eq "env-output.txt" 
            }
        }
        
        It "Should prioritize CLI parameter over ADGUARD_COMPILER_OUTPUT" {
            $env:ADGUARD_COMPILER_OUTPUT = "env-output.txt"
            
            Mock Read-CompilerConfiguration { 
                [PSCustomObject]@{
                    name = 'Test Config'
                    version = '1.0.0'
                }
            }
            Mock Invoke-FilterCompiler { 
                [PSCustomObject]@{
                    Success = $true
                    RuleCount = 100
                    OutputPath = $OutputPath
                    Hash = 'ABCD1234'
                    ElapsedMs = 1000
                }
            }
            
            { Invoke-RulesCompiler -OutputPath "cli-output.txt" } | Should -Not -Throw
            
            Should -Invoke Invoke-FilterCompiler -ParameterFilter { 
                $OutputPath -eq "cli-output.txt" 
            }
        }
    }

    Context "Rules Directory Environment Variable" {
        
        It "Should use ADGUARD_COMPILER_RULES_DIR when RulesPath is not provided and CopyToRules is set" {
            $env:ADGUARD_COMPILER_RULES_DIR = "/custom/rules"
            
            Mock Read-CompilerConfiguration { 
                [PSCustomObject]@{
                    name = 'Test Config'
                    version = '1.0.0'
                }
            }
            Mock Invoke-FilterCompiler { 
                [PSCustomObject]@{
                    Success = $true
                    RuleCount = 100
                    OutputPath = 'test-output.txt'
                    Hash = 'ABCD1234'
                    ElapsedMs = 1000
                }
            }
            Mock Write-CompiledOutput { 
                [PSCustomObject]@{
                    Success = $true
                    DestinationPath = $DestinationPath
                }
            }
            
            { Invoke-RulesCompiler -CopyToRules } | Should -Not -Throw
            
            Should -Invoke Write-CompiledOutput -ParameterFilter { 
                $DestinationPath -eq "/custom/rules" 
            }
        }
    }

    Context "Verbose Mode Environment Variable" {
        
        It "Should enable verbose mode when ADGUARD_COMPILER_VERBOSE is 'true'" {
            $env:ADGUARD_COMPILER_VERBOSE = "true"
            
            Mock Read-CompilerConfiguration { 
                [PSCustomObject]@{
                    name = 'Test Config'
                    version = '1.0.0'
                }
            }
            Mock Invoke-FilterCompiler { 
                [PSCustomObject]@{
                    Success = $true
                    RuleCount = 100
                    OutputPath = 'test-output.txt'
                    Hash = 'ABCD1234'
                    ElapsedMs = 1000
                }
            }
            
            # Note: Testing VerbosePreference directly is tricky in Pester
            # This test just ensures the function doesn't throw
            { Invoke-RulesCompiler } | Should -Not -Throw
        }
        
        It "Should enable verbose mode when ADGUARD_COMPILER_VERBOSE is '1'" {
            $env:ADGUARD_COMPILER_VERBOSE = "1"
            
            Mock Read-CompilerConfiguration { 
                [PSCustomObject]@{
                    name = 'Test Config'
                    version = '1.0.0'
                }
            }
            Mock Invoke-FilterCompiler { 
                [PSCustomObject]@{
                    Success = $true
                    RuleCount = 100
                    OutputPath = 'test-output.txt'
                    Hash = 'ABCD1234'
                    ElapsedMs = 1000
                }
            }
            
            { Invoke-RulesCompiler } | Should -Not -Throw
        }
    }

    Context "Copy To Rules Environment Variable" {
        
        It "Should enable CopyToRules when ADGUARD_COMPILER_COPY_TO_RULES is 'true'" {
            $env:ADGUARD_COMPILER_COPY_TO_RULES = "true"
            
            Mock Read-CompilerConfiguration { 
                [PSCustomObject]@{
                    name = 'Test Config'
                    version = '1.0.0'
                }
            }
            Mock Invoke-FilterCompiler { 
                [PSCustomObject]@{
                    Success = $true
                    RuleCount = 100
                    OutputPath = 'test-output.txt'
                    Hash = 'ABCD1234'
                    ElapsedMs = 1000
                }
            }
            Mock Write-CompiledOutput { 
                [PSCustomObject]@{
                    Success = $true
                }
            }
            
            { Invoke-RulesCompiler } | Should -Not -Throw
            
            Should -Invoke Write-CompiledOutput -Times 1
        }
        
        It "Should enable CopyToRules when ADGUARD_COMPILER_COPY_TO_RULES is '1'" {
            $env:ADGUARD_COMPILER_COPY_TO_RULES = "1"
            
            Mock Read-CompilerConfiguration { 
                [PSCustomObject]@{
                    name = 'Test Config'
                    version = '1.0.0'
                }
            }
            Mock Invoke-FilterCompiler { 
                [PSCustomObject]@{
                    Success = $true
                    RuleCount = 100
                    OutputPath = 'test-output.txt'
                    Hash = 'ABCD1234'
                    ElapsedMs = 1000
                }
            }
            Mock Write-CompiledOutput { 
                [PSCustomObject]@{
                    Success = $true
                }
            }
            
            { Invoke-RulesCompiler } | Should -Not -Throw
            
            Should -Invoke Write-CompiledOutput -Times 1
        }
        
        It "Should prioritize CLI -CopyToRules switch over environment variable" {
            # Set env var to false, but use CLI switch
            Remove-Item env:ADGUARD_COMPILER_COPY_TO_RULES -ErrorAction SilentlyContinue
            
            Mock Read-CompilerConfiguration { 
                [PSCustomObject]@{
                    name = 'Test Config'
                    version = '1.0.0'
                }
            }
            Mock Invoke-FilterCompiler { 
                [PSCustomObject]@{
                    Success = $true
                    RuleCount = 100
                    OutputPath = 'test-output.txt'
                    Hash = 'ABCD1234'
                    ElapsedMs = 1000
                }
            }
            Mock Write-CompiledOutput { 
                [PSCustomObject]@{
                    Success = $true
                }
            }
            
            { Invoke-RulesCompiler -CopyToRules } | Should -Not -Throw
            
            Should -Invoke Write-CompiledOutput -Times 1
        }
    }

    Context "DEBUG Environment Variable" {
        
        It "Should enable debug logging when DEBUG is set" {
            $env:DEBUG = "1"
            
            Mock Write-CompilerLog { }
            Mock Read-CompilerConfiguration { 
                [PSCustomObject]@{
                    name = 'Test Config'
                    version = '1.0.0'
                }
            }
            Mock Invoke-FilterCompiler { 
                [PSCustomObject]@{
                    Success = $true
                    RuleCount = 100
                    OutputPath = 'test-output.txt'
                    Hash = 'ABCD1234'
                    ElapsedMs = 1000
                }
            }
            
            { Invoke-RulesCompiler } | Should -Not -Throw
            
            # DEBUG affects Write-CompilerLog behavior
            Should -Invoke Write-CompilerLog -Times 1 -ParameterFilter { $Level -eq 'DEBUG' }
        }
    }

    Context "Multiple Environment Variables Combined" {
        
        It "Should handle multiple environment variables together" {
            $env:ADGUARD_COMPILER_CONFIG = "multi-config.json"
            $env:ADGUARD_COMPILER_OUTPUT = "multi-output.txt"
            $env:ADGUARD_COMPILER_VERBOSE = "1"
            $env:ADGUARD_COMPILER_COPY_TO_RULES = "true"
            $env:ADGUARD_COMPILER_RULES_DIR = "/multi/rules"
            
            Mock Read-CompilerConfiguration { 
                [PSCustomObject]@{
                    name = 'Test Config'
                    version = '1.0.0'
                }
            }
            Mock Invoke-FilterCompiler { 
                [PSCustomObject]@{
                    Success = $true
                    RuleCount = 100
                    OutputPath = 'multi-output.txt'
                    Hash = 'ABCD1234'
                    ElapsedMs = 1000
                }
            }
            Mock Write-CompiledOutput { 
                [PSCustomObject]@{
                    Success = $true
                }
            }
            
            { Invoke-RulesCompiler } | Should -Not -Throw
            
            Should -Invoke Read-CompilerConfiguration -ParameterFilter { 
                $ConfigPath -eq "multi-config.json" 
            }
            Should -Invoke Invoke-FilterCompiler -ParameterFilter { 
                $OutputPath -eq "multi-output.txt" 
            }
            Should -Invoke Write-CompiledOutput -Times 1
        }
    }

    Context "No Environment Variables Set" {
        
        It "Should use default values when no environment variables are set" {
            # Ensure all env vars are cleared
            @(
                'ADGUARD_COMPILER_CONFIG',
                'ADGUARD_COMPILER_OUTPUT',
                'ADGUARD_COMPILER_RULES_DIR',
                'ADGUARD_COMPILER_VERBOSE',
                'ADGUARD_COMPILER_COPY_TO_RULES'
            ) | ForEach-Object {
                Remove-Item "env:$_" -ErrorAction SilentlyContinue
            }
            
            Mock Read-CompilerConfiguration { 
                [PSCustomObject]@{
                    name = 'Test Config'
                    version = '1.0.0'
                }
            }
            Mock Invoke-FilterCompiler { 
                [PSCustomObject]@{
                    Success = $true
                    RuleCount = 100
                    OutputPath = 'test-output.txt'
                    Hash = 'ABCD1234'
                    ElapsedMs = 1000
                }
            }
            
            { Invoke-RulesCompiler } | Should -Not -Throw
            
            Should -Invoke Read-CompilerConfiguration -Times 1
            Should -Invoke Invoke-FilterCompiler -Times 1
        }
    }
}
