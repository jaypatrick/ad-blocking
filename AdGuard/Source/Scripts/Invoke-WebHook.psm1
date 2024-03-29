﻿function Invoke-Webhook {
    <#
    .SYNOPSIS
    Returns a list of services that are set to start automatically, are not
    currently running, excluding the services that are set to delayed start.

    .DESCRIPTION
    Get-MrAutoStoppedService is a function that returns a list of services from
    the specified remote computer(s) that are set to start automatically, are not
    currently running, and it excludes the services that are set to start automatically
    with a delayed startup.

    .PARAMETER WebhookUrl
    The remote webhook endpoint to trigger

    .PARAMETER WaitTime
    How much time to wait between standard invocations. This defaults to 200ms.

    .PARAMETER RetryCount
    In the event the remote endpoint isn't available, how many times to retry invocation. This defaults to 10 retries.

    .PARAMETER RetryInterval
    In the event the remote endpoint isn't available, how much time to wait to retry invocation. This defaults to 5 seconds between retries.

    .PARAMETER Continous
    Should this script be run continuously, or until the user specifies it to stop. This defaults to false. True will run in a loop.

    .EXAMPLE
    Invoke-Webhook -WebhookUrl <url> -Wait 200 -Count 10 -Interval 5 -Continous $True

    .INPUTS
    Uri, Int, Int, Int

    .OUTPUTS
    The status code of the webhook invocation

    .NOTES
    Author:  Jayson Knight
    Website: https://jaysonknight.com
    GitHub: jaypatrick
    #>
    
    [CmdletBinding()]
    param(
        [Parameter(Mandatory, Position = 0, ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [Alias("u, Url")]
        # [ValidatePattern("(http[s]?|[s]?ftp[s]?)(:\/\/)([^\s,]+)")]
        [uri]$WebhookUrl,

        [Parameter(ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [Alias("w, Wait")]
        [ValidateRange(200, [int]::MaxValue)]
        [int]$WaitTime = 200,

        [Parameter(ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [Alias("rc, Count")]
        [ValidateRange(0, 100)]
        [int]$RetryCount = 10,

        [Parameter(ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [Alias("ri, Interval")]
        [ValidateRange(1, 60)]
        [int]$RetryInterval = 5,

        [Alias("c, Continuous")]
        [bool]$Continous = $false
    )

    BEGIN {
        Write-Output $WebhookUrl
        Write-Output $WaitTime
        Write-Output $RetryCount
        Write-Output $RetryInterval
        $CurrentDate = Get-Date -DisplayHint Time
        [int]$RequestsSucceeded = 1
        [int]$RequestsFailed = 0
        [int]$TotalRequests = 1
        $Stopwatch = [system.diagnostics.stopwatch]::StartNew()
    }

    PROCESS {

        do {
            try {
                Write-Host "Allocated wait time is: $WaitTime ms"
                $Response = Invoke-WebRequest -Uri $WebhookUrl -MaximumRetryCount $RetryCount -RetryIntervalSec $RetryInterval
                $StatusCode = $Response.StatusCode
                if ($StatusCode -lt 300) { [void]$RequestsSucceeded++ }
                $ElapsedTime = New-TimeSpan -Start($CurrentDate)
                Write-Host $ElapsedTime "TOTAL elapsed time since invocation"
                Write-Host $Response.Content -ForegroundColor Green
                Write-Host $ElapsedTime.TotalSeconds "seconds elapsed since $CurrentDate" -ForegroundColor Magenta
                Write-Host "Public IP address has been updated $RequestsSucceeded times." -ForegroundColor Blue
            }
            catch {
                $StatusCode = $_.Exception.Response.StatusCode.value__
                [void]$RequestsFailed++
                Write-Error $_ -ErrorAction Continue
                Write-Host $_.ScriptStackTrace
            }
            finally {
                [void]$TotalRequests++
                Write-Verbose "Global counter is incremented to track request #'s: $TotalRequests invocations"
                Start-Sleep -Milliseconds $WaitTime
            }
            return $Response
        }until ($Continous)
    }

    END {
        [void]$stopwatch.Stop()
        [void]$stopwatch
    }
}
Export-ModuleMember -Function Invoke-Webhook