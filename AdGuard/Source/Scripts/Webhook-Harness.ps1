Import-Module -Name .\Invoke-WebHook.psm1 -Force

$DefaultUri = "https://linkip.adguard-dns.com/linkip/db94e3e9/8AdnEQlPCjyMaX74vTDZkraUDUYpCFiZ1tcH8dSk9VH"
[int]$Wait = 500
[int]$Count = 10
[int]$Interval = 5

function Get-YesNoResponse {
    # Create prompt body
    $title = "Post to Webhook Continuously?"
    $message = "Are you sure you want to perform this action continuously?"
    
    # Create answers
    $yes = New-Object System.Management.Automation.Host.ChoiceDescription "&Yes", "Invoke Webhook Continously."
    $no = New-Object System.Management.Automation.Host.ChoiceDescription "&No", "Invoke Webhook Once."
    $suspend = New-Object System.Management.Automation.Host.ChoiceDescription "&Suspend", "Pause the current pipeline and return to the command prompt."
    
    # Create ChoiceDescription with answers
    $options = [System.Management.Automation.Host.ChoiceDescription[]]($yes, $no, $suspend)
    $response = $host.UI.PromptForChoice($title, $message, $options, 0)
    return $response
}

$YesNoResponse = Get-YesNoResponse
$ResponseMessage = Invoke-Webhook -WebhookUrl $DefaultUri -WaitTime $Wait -RetryCount $Count -RetryInterval $Interval -Continous $YesNoResponse

Write-Host "The response message was: $ResponseMessage" -ForegroundColor Yellow
# Remove-Module -Name .\Invoke-WebHook.psm1