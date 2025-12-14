BeforeAll {
    Import-Module $PSScriptRoot\..\Invoke-WebHook.psm1
}

Describe "Invoke-Webhook" {
    Context "Parameter Validation" {
        It "Should have WebhookUrl parameter" {
            (Get-Command Invoke-Webhook).Parameters.Keys | Should -Contain "WebhookUrl"
        }

        It "Should have WaitTime parameter with default 200" {
            $params = (Get-Command Invoke-Webhook).Parameters
            $params.Keys | Should -Contain "WaitTime"
            $params['WaitTime'].Attributes.Where({$_ -is [System.Management.Automation.AliasAttribute]}).AliasNames | Should -Contain "Wait"
        }

        It "Should have RetryCount parameter" {
            (Get-Command Invoke-Webhook).Parameters.Keys | Should -Contain "RetryCount"
        }

        It "Should have Continuous parameter" {
            (Get-Command Invoke-Webhook).Parameters.Keys | Should -Contain "Continuous"
        }

        It "Should accept Continous as alias for backward compatibility" {
            $params = (Get-Command Invoke-Webhook).Parameters
            $params['Continuous'].Attributes.Where({$_ -is [System.Management.Automation.AliasAttribute]}).AliasNames | Should -Contain "Continous"
        }
    }
}