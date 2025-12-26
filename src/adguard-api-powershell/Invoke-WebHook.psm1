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
    Version: 1.0.0
#>

#region Private Functions

function Write-WebhookLog {
    <#
    .SYNOPSIS
    Internal logging function with colored output.
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [string]$Message,
        
        [ValidateSet('Info', 'Success', 'Warning', 'Error', 'Debug')]
        [string]$Level = 'Info',
        
        [switch]$NoNewline
    )

    if ($script:QuietMode) { return }

    $timestamp = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'
    $icon = switch ($Level) {
        'Info'    { '🔵' }
        'Success' { '✅' }
        'Warning' { '⚠️' }
        'Error'   { '❌' }
        'Debug'   { '🔍' }
    }

    $color = switch ($Level) {
        'Info'    { 'Cyan' }
        'Success' { 'Green' }
        'Warning' { 'Yellow' }
        'Error'   { 'Red' }
        'Debug'   { 'Gray' }
    }

    $output = "[$timestamp] $icon $Message"
    
    if ($NoNewline) {
        Write-Host $output -ForegroundColor $color -NoNewline
    }
    else {
        Write-Host $output -ForegroundColor $color
    }
}

function Show-Banner {
    <#
    .SYNOPSIS
    Displays a welcome banner.
    #>
    [CmdletBinding()]
    param()

    if ($script:QuietMode -or -not $script:ShowBanner) { return }

    Write-Host ""
    Write-Host "╔═══════════════════════════════════════════╗" -ForegroundColor Cyan
    Write-Host "║   AdGuard DNS Webhook Invoker v1.0.0     ║" -ForegroundColor Cyan
    Write-Host "║   Dynamic IP Address Updater              ║" -ForegroundColor Cyan
    Write-Host "╚═══════════════════════════════════════════╝" -ForegroundColor Cyan
    Write-Host ""
}

function Read-WebhookConfig {
    <#
    .SYNOPSIS
    Reads configuration from a JSON or YAML file.
    #>
    [CmdletBinding()]
    [OutputType([hashtable])]
    param(
        [Parameter(Mandatory)]
        [string]$Path
    )

    if (-not (Test-Path $Path)) {
        throw "Configuration file not found: $Path"
    }

    $extension = [System.IO.Path]::GetExtension($Path).ToLower()
    $content = Get-Content -Path $Path -Raw -Encoding UTF8

    $config = switch ($extension) {
        '.json' {
            $content | ConvertFrom-Json -AsHashtable
        }
        { $_ -in '.yaml', '.yml' } {
            # Try powershell-yaml module if available
            if (Get-Module -ListAvailable -Name 'powershell-yaml') {
                Import-Module powershell-yaml -ErrorAction SilentlyContinue
                $content | ConvertFrom-Yaml
            }
            else {
                throw "YAML files require powershell-yaml module. Install with: Install-Module powershell-yaml"
            }
        }
        default {
            throw "Unsupported configuration file format: $extension"
        }
    }

    return $config
}

function Save-WebhookConfig {
    <#
    .SYNOPSIS
    Saves current configuration to a JSON file.
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [hashtable]$Config,
        
        [Parameter(Mandatory)]
        [string]$Path
    )

    $directory = Split-Path -Path $Path -Parent
    if ($directory -and -not (Test-Path $directory)) {
        New-Item -ItemType Directory -Path $directory -Force | Out-Null
    }

    $Config | ConvertTo-Json -Depth 10 | Set-Content -Path $Path -Encoding UTF8
    Write-WebhookLog "Configuration saved to: $Path" -Level Success
}

function Format-WebhookStatistics {
    <#
    .SYNOPSIS
    Formats statistics output.
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [hashtable]$Stats,
        
        [ValidateSet('Table', 'List', 'Json')]
        [string]$Format = 'Table'
    )

    switch ($Format) {
        'Json' {
            return $Stats | ConvertTo-Json
        }
        'List' {
            Write-Host ""
            Write-Host "Webhook Statistics" -ForegroundColor Cyan
            Write-Host "==================" -ForegroundColor Cyan
            foreach ($key in $Stats.Keys) {
                $value = $Stats[$key]
                Write-Host "${key}: " -NoNewline -ForegroundColor Gray
                Write-Host $value -ForegroundColor White
            }
        }
        'Table' {
            $Stats.GetEnumerator() | ForEach-Object {
                [PSCustomObject]@{
                    Metric = $_.Key
                    Value  = $_.Value
                }
            } | Format-Table -AutoSize
        }
    }
}

#endregion

#region Public Functions

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

    .PARAMETER ConfigFile
    Path to a configuration file (JSON or YAML) containing webhook settings.

    .PARAMETER SaveConfig
    Save current configuration to the specified file path.

    .PARAMETER ShowStatistics
    Display detailed statistics at the end of execution.

    .PARAMETER Quiet
    Suppress all output except errors.

    .PARAMETER Format
    Output format for statistics: Table, List, or Json. Defaults to Table.

    .PARAMETER ShowBanner
    Display welcome banner on startup. Defaults to true.

    .EXAMPLE
    Invoke-AdGuardWebhook -WebhookUrl "https://example.com/webhook" -Continuous

    .EXAMPLE
    Invoke-AdGuardWebhook -ConfigFile "webhook-config.json" -ShowStatistics

    .EXAMPLE
    Invoke-AdGuardWebhook -WebhookUrl "https://example.com/webhook" -SaveConfig "config.json"

    .EXAMPLE
    Invoke-AdGuardWebhook -ConfigFile "config.json" -Quiet -Format Json

    .INPUTS
    System.Uri, System.Int32, System.Boolean

    .OUTPUTS
    Microsoft.PowerShell.Commands.WebResponseObject or PSCustomObject (with -ShowStatistics)

    .NOTES
    Author:  Jayson Knight
    Website: https://jaysonknight.com
    GitHub:  jaypatrick
    Version: 1.0.0
    #>
    
    [CmdletBinding(DefaultParameterSetName = 'Direct')]
    [OutputType([Microsoft.PowerShell.Commands.WebResponseObject], [PSCustomObject])]
    param(
        [Parameter(Mandatory, Position = 0, ValueFromPipeline, ValueFromPipelineByPropertyName, ParameterSetName = 'Direct')]
        [Parameter(ParameterSetName = 'Config')]
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

        [Parameter()]
        [Alias('c', 'Continous')]  # Keep old alias for backward compatibility
        [switch]$Continuous,

        [Parameter(ParameterSetName = 'Config')]
        [Alias('Config')]
        [ValidateScript({ Test-Path $_ -PathType Leaf })]
        [string]$ConfigFile,

        [Parameter()]
        [Alias('Save')]
        [string]$SaveConfig,

        [Parameter()]
        [Alias('Stats')]
        [switch]$ShowStatistics,

        [Parameter()]
        [Alias('q')]
        [switch]$Quiet,

        [Parameter()]
        [ValidateSet('Table', 'List', 'Json')]
        [string]$Format = 'Table',

        [Parameter()]
        [switch]$ShowBanner = $true
    )

    BEGIN {
        # Set script-level variables
        $script:QuietMode = $Quiet
        $script:ShowBanner = $ShowBanner
        
        # Load configuration from file if specified
        if ($ConfigFile) {
            try {
                $config = Read-WebhookConfig -Path $ConfigFile
                
                # Override with config values if not explicitly provided
                if (-not $PSBoundParameters.ContainsKey('WebhookUrl') -and $config.WebhookUrl) {
                    $WebhookUrl = $config.WebhookUrl
                }
                if (-not $PSBoundParameters.ContainsKey('WaitTime') -and $config.WaitTime) {
                    $WaitTime = $config.WaitTime
                }
                if (-not $PSBoundParameters.ContainsKey('RetryCount') -and $config.RetryCount) {
                    $RetryCount = $config.RetryCount
                }
                if (-not $PSBoundParameters.ContainsKey('RetryInterval') -and $config.RetryInterval) {
                    $RetryInterval = $config.RetryInterval
                }
                if (-not $PSBoundParameters.ContainsKey('Continuous') -and $config.Continuous) {
                    $Continuous = $config.Continuous
                }
                
                Write-WebhookLog "Configuration loaded from: $ConfigFile" -Level Success
            }
            catch {
                Write-WebhookLog "Failed to load configuration: $_" -Level Error
                throw
            }
        }
        
        # Validate webhook URL
        if (-not $WebhookUrl) {
            throw "WebhookUrl is required. Provide it via parameter or configuration file."
        }
        
        # Show banner
        Show-Banner
        
        # Initialize statistics
        $script:Stats = @{
            'Total Requests'     = 0
            'Successful Requests' = 0
            'Failed Requests'    = 0
            'Start Time'         = Get-Date
            'Webhook URL'        = $WebhookUrl.ToString()
            'Wait Time (ms)'     = $WaitTime
            'Retry Count'        = $RetryCount
            'Retry Interval (s)' = $RetryInterval
        }
        
        $Stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
        
        Write-WebhookLog "Starting webhook invocation..." -Level Info
        Write-WebhookLog "Webhook URL: $WebhookUrl" -Level Info
        Write-WebhookLog "Wait Time: $WaitTime ms" -Level Info
        Write-WebhookLog "Continuous Mode: $Continuous" -Level Info
    }

    PROCESS {
        $iteration = 0
        $lastResponse = $null
        
        do {
            $iteration++
            $script:Stats['Total Requests']++
            
            try {
                # Show progress for continuous mode
                if ($Continuous -and -not $Quiet) {
                    Write-Progress -Activity "Webhook Invocation" `
                        -Status "Iteration $iteration - Success: $($script:Stats['Successful Requests']), Failed: $($script:Stats['Failed Requests'])" `
                        -PercentComplete -1
                }
                
                Write-WebhookLog "Sending request #$iteration..." -Level Info
                
                # Invoke the webhook
                $Response = Invoke-WebRequest -Uri $WebhookUrl `
                    -MaximumRetryCount $RetryCount `
                    -RetryIntervalSec $RetryInterval `
                    -ErrorAction Stop
                
                $StatusCode = $Response.StatusCode
                
                if ($StatusCode -ge 200 -and $StatusCode -lt 300) {
                    $script:Stats['Successful Requests']++
                    Write-WebhookLog "Request #$iteration succeeded (Status: $StatusCode)" -Level Success
                    
                    if ($Response.Content -and -not $Quiet) {
                        Write-WebhookLog "Response: $($Response.Content)" -Level Info
                    }
                }
                else {
                    $script:Stats['Failed Requests']++
                    Write-WebhookLog "Request #$iteration returned status: $StatusCode" -Level Warning
                }
                
                $lastResponse = $Response
            }
            catch {
                $script:Stats['Failed Requests']++
                Write-WebhookLog "Request #$iteration failed: $($_.Exception.Message)" -Level Error
                
                if ($VerbosePreference -ne 'SilentlyContinue') {
                    Write-WebhookLog "Stack trace: $($_.ScriptStackTrace)" -Level Debug
                }
            }
            finally {
                # Wait before next iteration
                if ($Continuous) {
                    Start-Sleep -Milliseconds $WaitTime
                }
            }
            
        } while ($Continuous)
        
        # Clear progress
        if ($Continuous -and -not $Quiet) {
            Write-Progress -Activity "Webhook Invocation" -Completed
        }
        
        return $lastResponse
    }

    END {
        $Stopwatch.Stop()
        $script:Stats['End Time'] = Get-Date
        $script:Stats['Total Duration'] = $Stopwatch.Elapsed.ToString('hh\:mm\:ss\.fff')
        $script:Stats['Success Rate (%)'] = if ($script:Stats['Total Requests'] -gt 0) {
            [math]::Round(($script:Stats['Successful Requests'] / $script:Stats['Total Requests']) * 100, 2)
        } else { 0 }
        
        Write-WebhookLog "Webhook invocation completed" -Level Success
        
        # Save configuration if requested
        if ($SaveConfig) {
            $configToSave = @{
                WebhookUrl    = $WebhookUrl.ToString()
                WaitTime      = $WaitTime
                RetryCount    = $RetryCount
                RetryInterval = $RetryInterval
                Continuous    = $Continuous.IsPresent
            }
            
            try {
                Save-WebhookConfig -Config $configToSave -Path $SaveConfig
            }
            catch {
                Write-WebhookLog "Failed to save configuration: $_" -Level Error
            }
        }
        
        # Display statistics if requested
        if ($ShowStatistics) {
            Write-Host ""
            Format-WebhookStatistics -Stats $script:Stats -Format $Format
        }
        
        # Return statistics object for programmatic use
        if ($ShowStatistics -or $Format -eq 'Json') {
            return [PSCustomObject]$script:Stats
        }
    }
}

#endregion

# Export functions
Export-ModuleMember -Function Invoke-AdGuardWebhook

# Create alias for backward compatibility
Set-Alias -Name Invoke-Webhook -Value Invoke-AdGuardWebhook
Export-ModuleMember -Alias Invoke-Webhook
