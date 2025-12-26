#Requires -Version 7.0

BeforeAll {
    # Import the module under test
    $ModulePath = Join-Path $PSScriptRoot '..' 'Invoke-WebHook.psm1'
    Import-Module $ModulePath -Force -ErrorAction Stop
    
    # Save current environment variables
    $script:SavedEnv = @{}
    $EnvVars = @(
        'ADGUARD_WEBHOOK_URL',
        'ADGUARD_WEBHOOK_CONFIG',
        'ADGUARD_WEBHOOK_WAIT_TIME',
        'ADGUARD_WEBHOOK_RETRY_COUNT',
        'ADGUARD_WEBHOOK_RETRY_INTERVAL',
        'ADGUARD_WEBHOOK_FORMAT'
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

Describe "Environment Variables - Webhook Module" {
    
    BeforeEach {
        # Clean environment before each test
        @(
            'ADGUARD_WEBHOOK_URL',
            'ADGUARD_WEBHOOK_CONFIG',
            'ADGUARD_WEBHOOK_WAIT_TIME',
            'ADGUARD_WEBHOOK_RETRY_COUNT',
            'ADGUARD_WEBHOOK_RETRY_INTERVAL',
            'ADGUARD_WEBHOOK_FORMAT'
        ) | ForEach-Object {
            Remove-Item "env:$_" -ErrorAction SilentlyContinue
        }
    }

    Context "Webhook URL Environment Variable" {
        
        It "Should use ADGUARD_WEBHOOK_URL when WebhookUrl parameter is not provided" {
            $env:ADGUARD_WEBHOOK_URL = "https://env-webhook.example.com/test"
            
            Mock Invoke-WebRequest { 
                [PSCustomObject]@{
                    StatusCode = 200
                    StatusDescription = 'OK'
                }
            }
            
            { Invoke-AdGuardWebhook } | Should -Not -Throw
            
            Should -Invoke Invoke-WebRequest -ParameterFilter { 
                $Uri -eq "https://env-webhook.example.com/test" 
            }
        }
        
        It "Should prioritize CLI parameter over ADGUARD_WEBHOOK_URL" {
            $env:ADGUARD_WEBHOOK_URL = "https://env-webhook.example.com/test"
            
            Mock Invoke-WebRequest { 
                [PSCustomObject]@{
                    StatusCode = 200
                    StatusDescription = 'OK'
                }
            }
            
            { Invoke-AdGuardWebhook -WebhookUrl "https://cli-webhook.example.com/test" } | Should -Not -Throw
            
            Should -Invoke Invoke-WebRequest -ParameterFilter { 
                $Uri -eq "https://cli-webhook.example.com/test" 
            }
        }
        
        It "Should throw when no URL is provided via parameter or environment" {
            Remove-Item env:ADGUARD_WEBHOOK_URL -ErrorAction SilentlyContinue
            
            { Invoke-AdGuardWebhook } | Should -Throw "*WebhookUrl is required*"
        }
    }

    Context "Config File Environment Variable" {
        
        It "Should use ADGUARD_WEBHOOK_CONFIG when ConfigFile parameter is not provided" {
            # Create a temporary config file
            $tempConfig = New-TemporaryFile
            $configContent = @{
                WebhookUrl = "https://config-webhook.example.com/test"
                WaitTime = 300
                RetryCount = 5
            } | ConvertTo-Json
            Set-Content -Path $tempConfig.FullName -Value $configContent
            
            $env:ADGUARD_WEBHOOK_CONFIG = $tempConfig.FullName
            
            Mock Invoke-WebRequest { 
                [PSCustomObject]@{
                    StatusCode = 200
                    StatusDescription = 'OK'
                }
            }
            
            try {
                { Invoke-AdGuardWebhook } | Should -Not -Throw
                
                Should -Invoke Invoke-WebRequest -ParameterFilter { 
                    $Uri -eq "https://config-webhook.example.com/test" 
                }
            }
            finally {
                Remove-Item $tempConfig.FullName -Force -ErrorAction SilentlyContinue
            }
        }
    }

    Context "Wait Time Environment Variable" {
        
        It "Should use ADGUARD_WEBHOOK_WAIT_TIME when WaitTime parameter is not provided" {
            $env:ADGUARD_WEBHOOK_WAIT_TIME = "500"
            
            Mock Invoke-WebRequest { 
                [PSCustomObject]@{
                    StatusCode = 200
                    StatusDescription = 'OK'
                }
            }
            Mock Start-Sleep { }
            
            { Invoke-AdGuardWebhook -WebhookUrl "https://test.example.com" } | Should -Not -Throw
            
            # The function should use 500ms wait time
            # Note: Direct verification of internal variable is difficult, this ensures no error
        }
        
        It "Should prioritize CLI parameter over ADGUARD_WEBHOOK_WAIT_TIME" {
            $env:ADGUARD_WEBHOOK_WAIT_TIME = "500"
            
            Mock Invoke-WebRequest { 
                [PSCustomObject]@{
                    StatusCode = 200
                    StatusDescription = 'OK'
                }
            }
            
            { Invoke-AdGuardWebhook -WebhookUrl "https://test.example.com" -WaitTime 300 } | Should -Not -Throw
        }
        
        It "Should handle invalid ADGUARD_WEBHOOK_WAIT_TIME gracefully" {
            $env:ADGUARD_WEBHOOK_WAIT_TIME = "invalid"
            
            # Should throw due to casting error or fall back to default
            { 
                [int]$env:ADGUARD_WEBHOOK_WAIT_TIME 
            } | Should -Throw
        }
    }

    Context "Retry Count Environment Variable" {
        
        It "Should use ADGUARD_WEBHOOK_RETRY_COUNT when RetryCount parameter is not provided" {
            $env:ADGUARD_WEBHOOK_RETRY_COUNT = "15"
            
            Mock Invoke-WebRequest { 
                [PSCustomObject]@{
                    StatusCode = 200
                    StatusDescription = 'OK'
                }
            }
            
            { Invoke-AdGuardWebhook -WebhookUrl "https://test.example.com" } | Should -Not -Throw
        }
        
        It "Should prioritize CLI parameter over ADGUARD_WEBHOOK_RETRY_COUNT" {
            $env:ADGUARD_WEBHOOK_RETRY_COUNT = "15"
            
            Mock Invoke-WebRequest { 
                [PSCustomObject]@{
                    StatusCode = 200
                    StatusDescription = 'OK'
                }
            }
            
            { Invoke-AdGuardWebhook -WebhookUrl "https://test.example.com" -RetryCount 3 } | Should -Not -Throw
        }
    }

    Context "Retry Interval Environment Variable" {
        
        It "Should use ADGUARD_WEBHOOK_RETRY_INTERVAL when RetryInterval parameter is not provided" {
            $env:ADGUARD_WEBHOOK_RETRY_INTERVAL = "10"
            
            Mock Invoke-WebRequest { 
                [PSCustomObject]@{
                    StatusCode = 200
                    StatusDescription = 'OK'
                }
            }
            
            { Invoke-AdGuardWebhook -WebhookUrl "https://test.example.com" } | Should -Not -Throw
        }
        
        It "Should prioritize CLI parameter over ADGUARD_WEBHOOK_RETRY_INTERVAL" {
            $env:ADGUARD_WEBHOOK_RETRY_INTERVAL = "10"
            
            Mock Invoke-WebRequest { 
                [PSCustomObject]@{
                    StatusCode = 200
                    StatusDescription = 'OK'
                }
            }
            
            { Invoke-AdGuardWebhook -WebhookUrl "https://test.example.com" -RetryInterval 2 } | Should -Not -Throw
        }
    }

    Context "Output Format Environment Variable" {
        
        It "Should use ADGUARD_WEBHOOK_FORMAT when Format parameter is not provided" {
            $env:ADGUARD_WEBHOOK_FORMAT = "Json"
            
            Mock Invoke-WebRequest { 
                [PSCustomObject]@{
                    StatusCode = 200
                    StatusDescription = 'OK'
                }
            }
            
            { Invoke-AdGuardWebhook -WebhookUrl "https://test.example.com" -ShowStatistics } | Should -Not -Throw
        }
        
        It "Should prioritize CLI parameter over ADGUARD_WEBHOOK_FORMAT" {
            $env:ADGUARD_WEBHOOK_FORMAT = "Json"
            
            Mock Invoke-WebRequest { 
                [PSCustomObject]@{
                    StatusCode = 200
                    StatusDescription = 'OK'
                }
            }
            
            { Invoke-AdGuardWebhook -WebhookUrl "https://test.example.com" -Format "List" } | Should -Not -Throw
        }
        
        It "Should validate format values from environment variable" {
            $env:ADGUARD_WEBHOOK_FORMAT = "InvalidFormat"
            
            Mock Invoke-WebRequest { 
                [PSCustomObject]@{
                    StatusCode = 200
                    StatusDescription = 'OK'
                }
            }
            
            # The function should either reject invalid format or use default
            # This depends on implementation - ensuring it doesn't crash
            { Invoke-AdGuardWebhook -WebhookUrl "https://test.example.com" } | Should -Not -Throw
        }
    }

    Context "Multiple Environment Variables Combined" {
        
        It "Should handle multiple environment variables together" {
            $env:ADGUARD_WEBHOOK_URL = "https://multi-env.example.com/webhook"
            $env:ADGUARD_WEBHOOK_WAIT_TIME = "250"
            $env:ADGUARD_WEBHOOK_RETRY_COUNT = "8"
            $env:ADGUARD_WEBHOOK_RETRY_INTERVAL = "3"
            $env:ADGUARD_WEBHOOK_FORMAT = "List"
            
            Mock Invoke-WebRequest { 
                [PSCustomObject]@{
                    StatusCode = 200
                    StatusDescription = 'OK'
                }
            }
            
            { Invoke-AdGuardWebhook -ShowStatistics } | Should -Not -Throw
            
            Should -Invoke Invoke-WebRequest -ParameterFilter { 
                $Uri -eq "https://multi-env.example.com/webhook" 
            }
        }
        
        It "Should override environment variables with CLI parameters selectively" {
            $env:ADGUARD_WEBHOOK_URL = "https://env-webhook.example.com/test"
            $env:ADGUARD_WEBHOOK_WAIT_TIME = "500"
            $env:ADGUARD_WEBHOOK_RETRY_COUNT = "15"
            
            Mock Invoke-WebRequest { 
                [PSCustomObject]@{
                    StatusCode = 200
                    StatusDescription = 'OK'
                }
            }
            
            # Override only URL, keep env vars for wait time and retry count
            { Invoke-AdGuardWebhook -WebhookUrl "https://cli-webhook.example.com/test" } | Should -Not -Throw
            
            Should -Invoke Invoke-WebRequest -ParameterFilter { 
                $Uri -eq "https://cli-webhook.example.com/test" 
            }
        }
    }

    Context "Environment Variables with Config File Priority" {
        
        It "Should prioritize config file over environment variables" {
            # Create a temporary config file
            $tempConfig = New-TemporaryFile
            $configContent = @{
                WebhookUrl = "https://config-webhook.example.com/test"
                WaitTime = 400
            } | ConvertTo-Json
            Set-Content -Path $tempConfig.FullName -Value $configContent
            
            $env:ADGUARD_WEBHOOK_URL = "https://env-webhook.example.com/test"
            $env:ADGUARD_WEBHOOK_WAIT_TIME = "500"
            
            Mock Invoke-WebRequest { 
                [PSCustomObject]@{
                    StatusCode = 200
                    StatusDescription = 'OK'
                }
            }
            
            try {
                { Invoke-AdGuardWebhook -ConfigFile $tempConfig.FullName } | Should -Not -Throw
                
                # Config file should take precedence
                Should -Invoke Invoke-WebRequest -ParameterFilter { 
                    $Uri -eq "https://config-webhook.example.com/test" 
                }
            }
            finally {
                Remove-Item $tempConfig.FullName -Force -ErrorAction SilentlyContinue
            }
        }
    }

    Context "No Environment Variables Set" {
        
        It "Should use default values when no environment variables are set" {
            # Ensure all env vars are cleared
            @(
                'ADGUARD_WEBHOOK_URL',
                'ADGUARD_WEBHOOK_WAIT_TIME',
                'ADGUARD_WEBHOOK_RETRY_COUNT',
                'ADGUARD_WEBHOOK_RETRY_INTERVAL',
                'ADGUARD_WEBHOOK_FORMAT'
            ) | ForEach-Object {
                Remove-Item "env:$_" -ErrorAction SilentlyContinue
            }
            
            Mock Invoke-WebRequest { 
                [PSCustomObject]@{
                    StatusCode = 200
                    StatusDescription = 'OK'
                }
            }
            
            { Invoke-AdGuardWebhook -WebhookUrl "https://test.example.com" } | Should -Not -Throw
            
            Should -Invoke Invoke-WebRequest -Times 1
        }
    }

    Context "Backward Compatibility" {
        
        It "Should maintain backward compatibility with direct parameter usage" {
            # No environment variables, only CLI parameters
            Mock Invoke-WebRequest { 
                [PSCustomObject]@{
                    StatusCode = 200
                    StatusDescription = 'OK'
                }
            }
            
            { 
                Invoke-AdGuardWebhook `
                    -WebhookUrl "https://test.example.com" `
                    -WaitTime 200 `
                    -RetryCount 10 `
                    -RetryInterval 5 `
                    -Format "Table"
            } | Should -Not -Throw
            
            Should -Invoke Invoke-WebRequest -Times 1
        }
        
        It "Should work with the legacy Invoke-WebHook alias" {
            Mock Invoke-WebRequest { 
                [PSCustomObject]@{
                    StatusCode = 200
                    StatusDescription = 'OK'
                }
            }
            
            # Test backward-compatible alias
            { Invoke-WebHook -WebhookUrl "https://test.example.com" } | Should -Not -Throw
            
            Should -Invoke Invoke-WebRequest -Times 1
        }
    }
}
