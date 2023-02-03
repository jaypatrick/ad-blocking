Clear-Host
$NewUri = "https://linkip.adguard-dns.com/linkip/db94e3e9/8AdnEQlPCjyMaX74vTDZkraUDUYpCFiZ1tcH8dSk9VH"
$CurrentDate = Get-Date -DisplayHint Time
[int]$MinimumWaitTime = 1000
[int]$WaitTime = 1000
do {
     Write-Host "Allocated wait time is: $WaitTime ms"
   $NewResponse = Invoke-WebRequest -Uri $NewUri # -PassThru # -OutFile $OutputFile
    $elapsedTime = New-TimeSpan -Start($CurrentDate)
     Write-Host $elapsedTime "TOTAL elapsed time since invocation"
    Write-Host $NewResponse.Content -ForegroundColor Green
     Write-Host $elapsedTime.TotalSeconds "seconds elapsed since $currentDate" -ForegroundColor Magenta
    Write-Host "Public IP address has been updated $counter times." -ForegroundColor Blue
    Write-Host 
    Write-Host
    Start-Sleep -Milliseconds $WaitTime
   $counter++
} until ($infinity)
