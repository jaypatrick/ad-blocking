Clear-Host
$NewUri = "https://linkip.adguard-dns.com/linkip/db94e3e9/8AdnEQlPCjyMaX74vTDZkraUDUYpCFiZ1tcH8dSk9VH"
$CurrentDate = Get-Date -DisplayHint Time
[int]$Counter
# CLI arguments

([Parameter(Mandatory, HelpMessage = "This value represents the amount of time between iterations.")]
# [ValidateNotNullOrEmpty(ErrorMessage = "{0} Must have a valid value")]
# [ValidateCount(0, 1, ErrorMessage = "{0} Must occur exactly zero or none.")]
# [ValidateRange(200, [int]::MaxValue, ErrorMessage = "[0] Must be at least 200ms")]
[int]$WaitTime = 200)
if ($WaitTime -isnot [int]) {
    $WaitTime = Read-Host -Prompt "Wait time must be numeric value..."
}
if ($WaitTime -lt 200) {
    $WaitTime = Read-Host -Prompt "Wait time between must be less than 200ms"
}
Write-Host "Allocated wait time is: $WaitTime"
# add support for command line arguments, test for validity, and also recieve input from the CLI. Download some sort of famework.
do {
    $NewResponse = Invoke-WebRequest -Uri $NewUri # -PassThru # -OutFile $OutputFile
    $elapsedTime = New-TimeSpan -Start($CurrentDate)
    Write-Host $elapsedTime "TOTAL elapsed time since invocation"
    Write-Host $NewResponse.Content -ForegroundColor Green
    Write-Host $elapsedTime.TotalSeconds "seconds elapsed since $currentDate" -ForegroundColor Magenta
    Write-Host "Local IP address has been updated $counter times." -ForegroundColor Blue
    Write-Host 
    Write-Host
    Start-Sleep -Milliseconds $WaitTime
    $counter++
    break
} until ($infinity)
