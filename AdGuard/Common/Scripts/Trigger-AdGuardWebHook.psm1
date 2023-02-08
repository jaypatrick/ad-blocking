﻿function Update-ExternalIPAddress {
    <#
    .SYNOPSIS
    Returns a list of services that are set to start automatically, are not
    currently running, excluding the services that are set to delayed start.

    .DESCRIPTION
    Get-MrAutoStoppedService is a function that returns a list of services from
    the specified remote computer(s) that are set to start automatically, are not
    currently running, and it excludes the services that are set to start automatically
    with a delayed startup.

    .PARAMETER WebHookUrl
    The remote webhook endpoint to trigger

    .PARAMETER WaitTime
    How much time to wait between standard invocations. This defaults to 200ms

    .PARAMETER RetryCount
    In the event the remote endpoint isn't available, how many times to retry invocation

    .PARAMETER RetryInterval
    In the event the remote endpoint isn't available, how much time to wait to retry invocation

    .EXAMPLE
    Get-MrAutoStoppedService -ComputerName 'Server1', 'Server2'

    .INPUTS
    String, Int, Int, Int

    .OUTPUTS
    None

    .NOTES
    Author:  Jayson Knight
    Website: https://jaysonknight.com
    GitHub: jaypatrick
    #>
    
    [CmdletBinding()]
    param(
        [Parameter(Mandatory, Position = 0, ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [Alias("u, url")]
        [ValidateCount(1, 1)]
        [ValidatePattern("((?:(?:1\d\d|2[0-5][0-5]|2[0-4]\d|0?[1-9]\d|0?0?\d)\.){3}(?:1\d\d|2[0-5][0-5]|2[0-4]\d|0?[1-9]\d|0?0?\d))")]
        [string]$WebHookUrl,

        [Parameter(ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [Alias("w, wait")]
        [ValidateRange(200, [int]::MaxValue)]
        [int]$WaitTime = 200,

        [Parameter(ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [Alias("rc, count")]
        [ValidateRange(0, 100)]
        [int]$RetryCount = 10,

        [Parameter(ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [Alias("ri, interval")]
        [ValidateRange(1, 60)]
        [int]$RetryInterval = 5
    )
    BEGIN {
        $DefaultUri = "https://linkip.adguard-dns.com/linkip/db94e3e9/8AdnEQlPCjyMaX74vTDZkraUDUYpCFiZ1tcH8dSk9VH"
        Write-Output $WebHookUrl
        Write-Output $WaitTime
        Write-Output $RetryCount
        Write-Output $RetryInterval
        $CurrentDate = Get-Date -DisplayHint Time
        [int]$Counter = 0
        $Stopwatch = [system.diagnostics.stopwatch]::StartNew()
    }
    PROCESS {

        $Counter
        while ($infinity) {
            try {
                Write-Host "Allocated wait time is: $WaitTime ms"
                $NewResponse = Invoke-WebRequest -Uri $WebHookUrl -MaximumRetryCount $RetryCount -RetryIntervalSec $RetryInterval
                $StatusCode = $Response.StatusCode
                $ElapsedTime = New-TimeSpan -Start($CurrentDate)
                Write-Host $ElapsedTime "TOTAL elapsed time since invocation"
                Write-Host $NewResponse.Content -ForegroundColor Green
                Write-Host $ElapsedTime.TotalSeconds "seconds elapsed since $currentDate" -ForegroundColor Magenta
                Write-Host "Public IP address has been updated $counter times." -ForegroundColor Blue
                Write-Host 
                Write-Host
            }
            catch {
                $StatusCode = $_.Exception.Response.StatusCode.value__
                Write-Warning "An error occurred" -ErrorAction Continue
            }
            finally {
                Write-Verbose "Global counter is incremented to track request #'s: $Counter invocations"
                $Counter++
                Start-Sleep -Milliseconds $WaitTime
            }
        }
    }
    END {
        $stopwatch.Stop()
        $stopwatch
    }
}