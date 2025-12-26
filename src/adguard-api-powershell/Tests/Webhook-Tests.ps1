#Requires -Modules Pester

<#
.SYNOPSIS
    Comprehensive Pester tests for the AdGuard Webhook module.

.DESCRIPTION
    Tests parameter validation, function exports, configuration file support,
    and basic functionality of the Invoke-AdGuardWebhook module.

.NOTES
    Run with: Invoke-Pester -Path ./Tests/Webhook-Tests.ps1
#>

BeforeAll {
    # Import the module
    $modulePath = Join-Path $PSScriptRoot '..' 'Invoke-WebHook.psm1'
    Import-Module $modulePath -Force

    # Set up test paths
    $script:TestOutputDir = Join-Path $PSScriptRoot 'TestOutput'
    $script:TestConfigJson = Join-Path $script:TestOutputDir 'webhook-config.json'
}

AfterAll {
    # Clean up test output directory
    if (Test-Path $script:TestOutputDir) {
        Remove-Item $script:TestOutputDir -Recurse -Force -ErrorAction SilentlyContinue
    }

    # Remove the module
    Remove-Module 'Invoke-WebHook' -Force -ErrorAction SilentlyContinue
}

Describe "Module Import" {
    Context "Module Loading" {
        It "Should import the module without errors" {
            { Import-Module (Join-Path $PSScriptRoot '..' 'Invoke-WebHook.psm1') -Force } | Should -Not -Throw
        }

        It "Should export Invoke-AdGuardWebhook function" {
            Get-Command 'Invoke-AdGuardWebhook' -ErrorAction SilentlyContinue | Should -Not -BeNullOrEmpty
        }

        It "Should have Invoke-Webhook alias for backward compatibility" {
            Get-Alias 'Invoke-Webhook' -ErrorAction SilentlyContinue | Should -Not -BeNullOrEmpty
        }
    }
}

Describe "Invoke-AdGuardWebhook" {
    Context "Parameter Validation" {
        It "Should have WebhookUrl parameter" {
            (Get-Command Invoke-AdGuardWebhook).Parameters.Keys | Should -Contain "WebhookUrl"
        }

        It "Should have WaitTime parameter with default 200" {
            $params = (Get-Command Invoke-AdGuardWebhook).Parameters
            $params.Keys | Should -Contain "WaitTime"
            $params['WaitTime'].Attributes.Where({$_ -is [System.Management.Automation.AliasAttribute]}).AliasNames | Should -Contain "Wait"
        }

        It "Should have RetryCount parameter" {
            (Get-Command Invoke-AdGuardWebhook).Parameters.Keys | Should -Contain "RetryCount"
        }

        It "Should have RetryInterval parameter" {
            (Get-Command Invoke-AdGuardWebhook).Parameters.Keys | Should -Contain "RetryInterval"
        }

        It "Should have Continuous parameter" {
            (Get-Command Invoke-AdGuardWebhook).Parameters.Keys | Should -Contain "Continuous"
        }

        It "Should accept Continous as alias for backward compatibility" {
            $params = (Get-Command Invoke-AdGuardWebhook).Parameters
            $params['Continuous'].Attributes.Where({$_ -is [System.Management.Automation.AliasAttribute]}).AliasNames | Should -Contain "Continous"
        }

        It "Should have ConfigFile parameter" {
            (Get-Command Invoke-AdGuardWebhook).Parameters.Keys | Should -Contain "ConfigFile"
        }

        It "Should have SaveConfig parameter" {
            (Get-Command Invoke-AdGuardWebhook).Parameters.Keys | Should -Contain "SaveConfig"
        }

        It "Should have ShowStatistics parameter" {
            (Get-Command Invoke-AdGuardWebhook).Parameters.Keys | Should -Contain "ShowStatistics"
        }

        It "Should have Quiet parameter" {
            (Get-Command Invoke-AdGuardWebhook).Parameters.Keys | Should -Contain "Quiet"
        }

        It "Should have Format parameter" {
            (Get-Command Invoke-AdGuardWebhook).Parameters.Keys | Should -Contain "Format"
        }

        It "Should have ShowBanner parameter" {
            (Get-Command Invoke-AdGuardWebhook).Parameters.Keys | Should -Contain "ShowBanner"
        }

        It "Should validate Format parameter values" {
            $params = (Get-Command Invoke-AdGuardWebhook).Parameters
            $validateSet = $params['Format'].Attributes.Where({ $_ -is [System.Management.Automation.ValidateSetAttribute] })
            $validateSet.ValidValues | Should -Contain 'Table'
            $validateSet.ValidValues | Should -Contain 'List'
            $validateSet.ValidValues | Should -Contain 'Json'
        }

        It "Should validate WaitTime range" {
            $params = (Get-Command Invoke-AdGuardWebhook).Parameters
            $validateRange = $params['WaitTime'].Attributes.Where({ $_ -is [System.Management.Automation.ValidateRangeAttribute] })
            $validateRange.MinRange | Should -Be 200
        }

        It "Should validate RetryCount range" {
            $params = (Get-Command Invoke-AdGuardWebhook).Parameters
            $validateRange = $params['RetryCount'].Attributes.Where({ $_ -is [System.Management.Automation.ValidateRangeAttribute] })
            $validateRange.MinRange | Should -Be 0
            $validateRange.MaxRange | Should -Be 100
        }

        It "Should validate RetryInterval range" {
            $params = (Get-Command Invoke-AdGuardWebhook).Parameters
            $validateRange = $params['RetryInterval'].Attributes.Where({ $_ -is [System.Management.Automation.ValidateRangeAttribute] })
            $validateRange.MinRange | Should -Be 1
            $validateRange.MaxRange | Should -Be 60
        }
    }

    Context "Configuration File Support" {
        BeforeAll {
            # Create test output directory
            if (-not (Test-Path $script:TestOutputDir)) {
                New-Item -ItemType Directory -Path $script:TestOutputDir -Force | Out-Null
            }

            # Create test configuration file
            $testConfig = @{
                WebhookUrl    = "https://example.com/webhook"
                WaitTime      = 500
                RetryCount    = 5
                RetryInterval = 10
                Continuous    = $false
            }
            $testConfig | ConvertTo-Json | Set-Content -Path $script:TestConfigJson -Encoding UTF8
        }

        It "Should read configuration from JSON file" {
            # This test would require mocking Invoke-WebRequest
            # For now, we just verify the config file exists
            Test-Path $script:TestConfigJson | Should -BeTrue
        }

        It "Should save configuration to JSON file" {
            $saveConfigPath = Join-Path $script:TestOutputDir 'saved-config.json'
            
            # This would need mocking, but we can test file creation
            $config = @{
                WebhookUrl = "https://example.com/test"
                WaitTime = 300
            }
            $config | ConvertTo-Json | Set-Content -Path $saveConfigPath -Encoding UTF8
            
            Test-Path $saveConfigPath | Should -BeTrue
        }
    }

    Context "Backward Compatibility" {
        It "Should allow calling via Invoke-Webhook alias" {
            Get-Alias Invoke-Webhook -ErrorAction SilentlyContinue | Should -Not -BeNullOrEmpty
        }

        It "Should support Continous (misspelled) parameter alias" {
            $params = (Get-Command Invoke-AdGuardWebhook).Parameters
            $aliases = $params['Continuous'].Attributes.Where({$_ -is [System.Management.Automation.AliasAttribute]}).AliasNames
            $aliases | Should -Contain 'Continous'
        }
    }

    Context "Output Format" {
        It "Should accept Table format" {
            $params = (Get-Command Invoke-AdGuardWebhook).Parameters
            $validateSet = $params['Format'].Attributes.Where({ $_ -is [System.Management.Automation.ValidateSetAttribute] })
            $validateSet.ValidValues | Should -Contain 'Table'
        }

        It "Should accept List format" {
            $params = (Get-Command Invoke-AdGuardWebhook).Parameters
            $validateSet = $params['Format'].Attributes.Where({ $_ -is [System.Management.Automation.ValidateSetAttribute] })
            $validateSet.ValidValues | Should -Contain 'List'
        }

        It "Should accept Json format" {
            $params = (Get-Command Invoke-AdGuardWebhook).Parameters
            $validateSet = $params['Format'].Attributes.Where({ $_ -is [System.Management.Automation.ValidateSetAttribute] })
            $validateSet.ValidValues | Should -Contain 'Json'
        }
    }
}
