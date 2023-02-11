Import-Module -Name .\Invoke-WebHook.psm1 -Force
$DefaultUri = "https://linkip.adguard-dns.com/linkip/db94e3e9/8AdnEQlPCjyMaX74vTDZkraUDUYpCFiZ1tcH8dSk9VH"
[int]$Wait = 500
[int]$Count = 10
[int]$Interval = 5
[bool]$Continuous = $false

$ResponseCode = Invoke-Webhook -WebhookUrl $DefaultUri -WaitTime $Wait -RetryCount $Count -RetryInterval $Interval -Continous $Continuous

Write-Host "The response code is: $ResponseCode"