
function Update-ExternalIPAddress {
    <#
    .SYNOPSIS
    Returns a list of services that are set to start automatically, are not
    currently running, excluding the services that are set to delayed start.

    .DESCRIPTION
    Get-MrAutoStoppedService is a function that returns a list of services from
    the specified remote computer(s) that are set to start automatically, are not
    currently running, and it excludes the services that are set to start automatically
    with a delayed startup.

    .PARAMETER ComputerName
    The remote computer(s) to check the status of the services on.

    .PARAMETER Credential
    Specifies a user account that has permission to perform this action. The default
    is the current user.

    .EXAMPLE
    Get-MrAutoStoppedService -ComputerName 'Server1', 'Server2'

    .EXAMPLE
    'Server1', 'Server2' | Get-MrAutoStoppedService

    .EXAMPLE
    Get-MrAutoStoppedService -ComputerName 'Server1' -Credential (Get-Credential)

    .INPUTS
    String

    .OUTPUTS
    PSCustomObject

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

    }
    PROCESS {
        
        Write-Output $WebHookUrl
        Write-Output $WaitTime

        Write-Verbose("Web hook url and wait time are mandatory. The default for wait time is 200ms")
    
        # LOCAL VARIABLES
        Write-Verbose("Local variables include a datetime")
        $CurrentDate = Get-Date
        $Counter
        while ($infinity) {
            try {
                Write-Host "Allocated wait time is: $WaitTime ms"
                $NewResponse = Invoke-WebRequest -Uri $WebHookUrl -MaximumRetryCount $RetryCount -RetryIntervalSec $RetryInterval
                $StatusCode = $Response.StatusCode
                $elapsedTime = New-TimeSpan -Start($CurrentDate)
                Write-Host $elapsedTime "TOTAL elapsed time since invocation"
                Write-Host $NewResponse.Content -ForegroundColor Green
                Write-Host $elapsedTime.TotalSeconds "seconds elapsed since $currentDate" -ForegroundColor Magenta
                Write-Host "Public IP address has been updated $counter times." -ForegroundColor Blue
                Write-Host 
                Write-Host
            }
            catch {
                $StatusCode = $_.Exception.Response.StatusCode.value__
                Write-Warning "An error occurred"
            }
            finally {
                Write-Verbose "Global counter is incremented to track request #'s"
                $Counter++
                Start-Sleep -Milliseconds $WaitTime
            }
        }
    }
    END {
        
    }
}