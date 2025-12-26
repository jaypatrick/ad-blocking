#Requires -Version 7.0

<#
.SYNOPSIS
    PowerShell module for invoking AdGuard DNS webhook endpoints.

.DESCRIPTION
    This module provides functions for triggering AdGuard DNS webhook endpoints to update
    dynamic IP addresses. It includes automatic retry logic, continuous operation mode,
    statistics tracking, and multiple output formats.

.NOTES
    Author:  Jayson Knight
    Website: https://jaysonknight.com
    GitHub:  jaypatrick

    Prerequisites:
    - PowerShell 7.0+ (cross-platform)

    Supported Platforms:
    - Windows 10/11 with PowerShell 7+
    - macOS 10.15+ with PowerShell 7+
    - Linux (Ubuntu 18.04+, Debian 10+, RHEL 8+, etc.) with PowerShell 7+
#>

# Import the OOP modules from src/powershell-modules/
using module ..\powershell-modules\Common\Common.psm1
using module ..\powershell-modules\AdGuardWebhook\AdGuardWebhook.psm1

function Invoke-AdGuardWebhook {
    <#
    .SYNOPSIS
    Invokes an AdGuard DNS webhook endpoint to update the dynamic IP address.

    .DESCRIPTION
    Invoke-AdGuardWebhook triggers an AdGuard DNS webhook endpoint to update
    the linked IP address for the device. It supports automatic retries, configurable wait
    times, continuous operation mode, statistics tracking, and multiple output formats.

    .PARAMETER WebhookUrl
    The remote webhook endpoint to trigger. Can be loaded from config file.

    .PARAMETER WaitTime
    Time to wait between invocations in milliseconds. Defaults to 200ms.

    .PARAMETER RetryCount
    Number of retry attempts if the endpoint is unavailable. Defaults to 10.

    .PARAMETER RetryInterval
    Time to wait between retry attempts in seconds. Defaults to 5 seconds.

    .PARAMETER Continuous
    Run continuously in a loop until manually stopped. Defaults to false.

    .EXAMPLE
    Invoke-AdGuardWebhook -WebhookUrl "https://example.com/webhook" -Continuous

    .EXAMPLE
    Invoke-AdGuardWebhook -WebhookUrl "https://example.com/webhook" -RetryCount 5

    .INPUTS
    System.Uri, System.Int32, System.Boolean

    .OUTPUTS
    PSCustomObject

    .NOTES
    Author:  Jayson Knight
    Website: https://jaysonknight.com
    GitHub:  jaypatrick
    Version: 2.0.0
    #>
    
    [CmdletBinding()]
    [OutputType([PSCustomObject])]
    param(
        [Parameter(Mandatory, Position = 0, ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [Alias('u', 'Url')]
        [ValidateNotNullOrEmpty()]
        [uri]$WebhookUrl,

        [Parameter(ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [Alias('w', 'Wait')]
        [ValidateRange(200, [int]::MaxValue)]
        [int]$WaitTime = 200,

        [Parameter(ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [Alias('rc', 'Count')]
        [ValidateRange(0, 100)]
        [int]$RetryCount = 10,

        [Parameter(ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [Alias('ri', 'Interval')]
        [ValidateRange(1, 60)]
        [int]$RetryInterval = 5,

        [Alias('c', 'Continous')]
        [bool]$Continuous = $false
    )

    BEGIN {
        # Display banner
        Write-Host ""
        Write-Host "╔═══════════════════════════════════════════════════════╗" -ForegroundColor Cyan
        Write-Host "║       AdGuard Webhook Invocation Service v2.0        ║" -ForegroundColor Cyan
        Write-Host "╚═══════════════════════════════════════════════════════╝" -ForegroundColor Cyan
        Write-Host ""

        # Create WebhookConfiguration using the new OOP class
        $config = [WebhookConfiguration]::new()
        $config.WebhookUrl = $WebhookUrl.ToString()
        $config.WaitTime = $WaitTime
        $config.RetryCount = $RetryCount
        $config.RetryInterval = $RetryInterval
        $config.Continuous = $Continuous
        $config.Quiet = $false
        $config.ShowStatistics = $true
        $config.Format = 'List'

        # Create WebhookInvoker
        $invoker = [WebhookInvoker]::new($config)

        # Display configuration with colors
        Write-Host "Configuration:" -ForegroundColor Yellow
        Write-Host "  └─ URL:            " -NoNewline -ForegroundColor Gray
        Write-Host $WebhookUrl.Host -ForegroundColor White
        Write-Host "  └─ Wait Time:      " -NoNewline -ForegroundColor Gray
        Write-Host "${WaitTime}ms" -ForegroundColor White
        Write-Host "  └─ Retry Count:    " -NoNewline -ForegroundColor Gray
        Write-Host $RetryCount -ForegroundColor White
        Write-Host "  └─ Retry Interval: " -NoNewline -ForegroundColor Gray
        Write-Host "${RetryInterval}s" -ForegroundColor White
        Write-Host "  └─ Mode:           " -NoNewline -ForegroundColor Gray
        Write-Host $(if ($Continuous) { "Continuous" } else { "Single" }) -ForegroundColor $(if ($Continuous) { "Magenta" } else { "Cyan" })
        Write-Host ""

        $currentDate = Get-Date -DisplayHint Time
    }

    PROCESS {
        try {
            if ($Continuous) {
                # Run in continuous mode
                $invoker.InvokeContinuous()
                return $invoker.Statistics
            }
            else {
                # Single invocation with retry
                $success = $invoker.InvokeWithRetry()
                
                if ($success) {
                    $summary = $invoker.Statistics.GetDetailedSummary()
                    
                    # Display success with visual feedback
                    $elapsedTime = New-TimeSpan -Start $currentDate
                    Write-Host ""
                    Write-Host "╔═══════════════════════════════════════════════════════╗" -ForegroundColor Green
                    Write-Host "║             ✓ WEBHOOK INVOKED SUCCESSFULLY            ║" -ForegroundColor Green
                    Write-Host "╚═══════════════════════════════════════════════════════╝" -ForegroundColor Green
                    Write-Host ""
                    Write-Host "Results:" -ForegroundColor Yellow
                    Write-Host "  └─ Status:         " -NoNewline -ForegroundColor Gray
                    Write-Host "SUCCESS" -ForegroundColor Green
                    Write-Host "  └─ Attempts:       " -NoNewline -ForegroundColor Gray
                    Write-Host "$($summary.SuccessfulAttempts) successful" -ForegroundColor Green
                    Write-Host "  └─ Elapsed Time:   " -NoNewline -ForegroundColor Gray
                    Write-Host "$($elapsedTime.TotalSeconds.ToString('F2'))s" -ForegroundColor White
                    Write-Host "  └─ IP Updated:     " -NoNewline -ForegroundColor Gray
                    Write-Host "Public IP address has been updated" -ForegroundColor Cyan
                    Write-Host ""
                    
                    # Show statistics
                    $invoker.ShowFinalStatistics()
                    
                    return [PSCustomObject]@{
                        Success = $true
                        Statistics = $invoker.Statistics
                        ElapsedTime = $elapsedTime
                    }
                }
                else {
                    Write-Host ""
                    Write-Host "╔═══════════════════════════════════════════════════════╗" -ForegroundColor Red
                    Write-Host "║               ✗ WEBHOOK INVOCATION FAILED            ║" -ForegroundColor Red
                    Write-Host "╚═══════════════════════════════════════════════════════╝" -ForegroundColor Red
                    Write-Host ""
                    Write-Host "Failed after $($config.RetryCount + 1) attempts" -ForegroundColor Red
                    Write-Host ""
                    return [PSCustomObject]@{
                        Success = $false
                        Statistics = $invoker.Statistics
                    }
                }
            }
        }
        catch {
            Write-Host ""
            Write-Host "╔═══════════════════════════════════════════════════════╗" -ForegroundColor Red
            Write-Host "║                    ✗ ERROR OCCURRED                   ║" -ForegroundColor Red
            Write-Host "╚═══════════════════════════════════════════════════════╝" -ForegroundColor Red
            Write-Host ""
            Write-Host "Error Details:" -ForegroundColor Yellow
            Write-Host "  └─ Message: " -NoNewline -ForegroundColor Gray
            Write-Host $_.Exception.Message -ForegroundColor Red
            Write-Host ""
            Write-Host $_.ScriptStackTrace -ForegroundColor DarkGray
            Write-Host ""
            return [PSCustomObject]@{
                Success = $false
                Error = $_
            }
        }
    }

    END {
        # Cleanup if needed
    }
}

# Export functions
Export-ModuleMember -Function Invoke-AdGuardWebhook

# Create alias for backward compatibility
Set-Alias -Name Invoke-Webhook -Value Invoke-AdGuardWebhook
Export-ModuleMember -Alias Invoke-Webhook
