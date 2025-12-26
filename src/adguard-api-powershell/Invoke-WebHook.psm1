#Requires -Version 7.0

<#
.SYNOPSIS
    PowerShell module for invoking AdGuard webhooks.

.DESCRIPTION
    This module provides a PowerShell API for invoking AdGuard DNS webhooks.
    It wraps the WebhookInvoker OOP class from the AdGuardWebhook module and
    provides a backward-compatible function API.

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

function Invoke-Webhook {
    <#
    .SYNOPSIS
    Invokes an AdGuard webhook endpoint to update the dynamic IP address for the device.

    .DESCRIPTION
    Invoke-Webhook is a function that triggers an AdGuard DNS webhook endpoint to update
    the linked IP address for the device. It supports automatic retries, configurable wait
    times, and continuous operation mode for keeping the IP address updated.

    .PARAMETER WebhookUrl
    The remote webhook endpoint to trigger

    .PARAMETER WaitTime
    How much time to wait between standard invocations. This defaults to 200ms.

    .PARAMETER RetryCount
    In the event the remote endpoint isn't available, how many times to retry invocation. This defaults to 10 retries.

    .PARAMETER RetryInterval
    In the event the remote endpoint isn't available, how much time to wait to retry invocation. This defaults to 5 seconds between retries.

    .PARAMETER Continuous
    Should this script be run continuously, or until the user specifies it to stop. This defaults to false. True will run in a loop.

    .EXAMPLE
    Invoke-Webhook -WebhookUrl <url> -Wait 200 -Count 10 -Interval 5 -Continuous $True

    .INPUTS
    Uri, Int, Int, Int

    .OUTPUTS
    The status code of the webhook invocation or WebhookStatistics object

    .NOTES
    Author:  Jayson Knight
    Website: https://jaysonknight.com
    GitHub: jaypatrick
    #>
    
    [CmdletBinding()]
    param(
        [Parameter(Mandatory, Position = 0, ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [Alias("u", "Url")]
        [uri]$WebhookUrl,

        [Parameter(ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [Alias("w", "Wait")]
        [ValidateRange(200, [int]::MaxValue)]
        [int]$WaitTime = 200,

        [Parameter(ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [Alias("rc", "Count")]
        [ValidateRange(0, 100)]
        [int]$RetryCount = 10,

        [Parameter(ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [Alias("ri", "Interval")]
        [ValidateRange(1, 60)]
        [int]$RetryInterval = 5,

        [Alias("c", "Continous")]
        [bool]$Continuous = $false
    )

    BEGIN {
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

        # Display initial parameters (backward compatibility)
        Write-Output $WebhookUrl
        Write-Output $WaitTime
        Write-Output $RetryCount
        Write-Output $RetryInterval

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
                    
                    # Display backward-compatible output
                    $elapsedTime = New-TimeSpan -Start $currentDate
                    Write-Host $elapsedTime "TOTAL elapsed time since invocation"
                    Write-Host "Public IP address has been updated." -ForegroundColor Green
                    Write-Host $elapsedTime.TotalSeconds "seconds elapsed since $currentDate" -ForegroundColor Magenta
                    Write-Host "Webhook invoked successfully $($summary.SuccessfulAttempts) time(s)." -ForegroundColor Blue
                    
                    # Show statistics
                    $invoker.ShowFinalStatistics()
                    
                    return [PSCustomObject]@{
                        Success = $true
                        Statistics = $invoker.Statistics
                        ElapsedTime = $elapsedTime
                    }
                }
                else {
                    Write-Error "Webhook invocation failed after $($config.RetryCount + 1) attempts"
                    return [PSCustomObject]@{
                        Success = $false
                        Statistics = $invoker.Statistics
                    }
                }
            }
        }
        catch {
            Write-Error $_ -ErrorAction Continue
            Write-Host $_.ScriptStackTrace
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

# Export the function
Export-ModuleMember -Function Invoke-Webhook
