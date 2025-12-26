#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Test harness for the AdGuard Webhook module.

.DESCRIPTION
    Interactive script to test and demonstrate the Webhook module functionality.
    Provides options to invoke webhooks with different configurations.

.NOTES
    Author:  Jayson Knight
    Website: https://jaysonknight.com
    GitHub:  jaypatrick

.EXAMPLE
    .\Webhook-Harness.ps1

.EXAMPLE
    .\Webhook-Harness.ps1 -Continuous
#>

[CmdletBinding()]
param(
    [Parameter()]
    [uri]$WebhookUrl,
    
    [Parameter()]
    [switch]$Continuous,
    
    [Parameter()]
    [switch]$ShowStatistics,
    
    [Parameter()]
    [string]$ConfigFile
)

# Import the module
$modulePath = Join-Path $PSScriptRoot 'Invoke-WebHook.psm1'
if (Test-Path $modulePath) {
    Import-Module $modulePath -Force
    Write-Host "AdGuard Webhook module loaded successfully." -ForegroundColor Green
}
else {
    Write-Error "Module not found at: $modulePath"
    exit 1
}

# Get webhook URL from parameter or environment variable
if (-not $WebhookUrl) {
    $webhookUrl = $env:ADGUARD_WEBHOOK_URL
    if ([string]::IsNullOrEmpty($webhookUrl)) {
        Write-Host ""
        Write-Host "No webhook URL provided." -ForegroundColor Yellow
        Write-Host "Set ADGUARD_WEBHOOK_URL environment variable or use -WebhookUrl parameter." -ForegroundColor Yellow
        Write-Host ""
        exit 1
    }
    $WebhookUrl = [uri]$webhookUrl
}

# Default values
$Wait = 500
$Count = 10
$Interval = 5

function Get-YesNoResponse {
    $title = "Post to Webhook Continuously?"
    $message = "Are you sure you want to perform this action continuously?"
    
    $yes = New-Object System.Management.Automation.Host.ChoiceDescription "&Yes", "Invoke Webhook Continuously."
    $no = New-Object System.Management.Automation.Host.ChoiceDescription "&No", "Invoke Webhook Once."
    $suspend = New-Object System.Management.Automation.Host.ChoiceDescription "&Suspend", "Pause and return to command prompt."
    
    $options = [System.Management.Automation.Host.ChoiceDescription[]]($yes, $no, $suspend)
    $response = $host.UI.PromptForChoice($title, $message, $options, 0)
    
    return $response -eq 0
}

try {
    Write-Host ""
    Write-Host "AdGuard Webhook Test Harness" -ForegroundColor Cyan
    Write-Host "============================" -ForegroundColor Cyan
    Write-Host ""
    
    # Prompt for continuous mode if not specified
    if (-not $PSBoundParameters.ContainsKey('Continuous')) {
        $Continuous = Get-YesNoResponse
    }
    
    # Prepare parameters
    $params = @{
        WebhookUrl      = $WebhookUrl
        WaitTime        = $Wait
        RetryCount      = $Count
        RetryInterval   = $Interval
        ShowStatistics  = $true
    }
    
    if ($Continuous) {
        $params['Continuous'] = $true
    }
    
    if ($ConfigFile) {
        $params['ConfigFile'] = $ConfigFile
    }
    
    # Invoke the webhook
    Write-Host "Starting webhook invocation..." -ForegroundColor Green
    Write-Host ""
    
    $result = Invoke-AdGuardWebhook @params
    
    Write-Host ""
    Write-Host "Test harness completed successfully." -ForegroundColor Green
}
catch {
    Write-Host ""
    Write-Host "Error occurred: $($_.Exception.Message)" -ForegroundColor Red
    Write-Error $_
    exit 1
}
