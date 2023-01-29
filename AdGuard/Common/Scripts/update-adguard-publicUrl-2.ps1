$NewUri = "https://linkip.adguard-dns.com/linkip/db94e3e9/8AdnEQlPCjyMaX74vTDZkraUDUYpCFiZ1tcH8dSk9VH"
$waitTime = 500
$stopwatch = [system.diagnostics.stopwatch]::StartNew()
$currentDate = Get-Date -DisplayHint Time
$counter
Clear-Host
do {
    $NewResponse = Invoke-WebRequest -Uri $newUri # -PassThru # -OutFile $OutputFile
    $elapsedTime = New-TimeSpan -Start($currentDate)
    $elapsedMinutes = $elapsedTime.TotalMinutes
    $elapsedSeconds = $elapsedTime.TotalSeconds
    $elapsedHours = $elapsedTime.TotalHours
    Write-Host $elapsedTime "TOTAL elapsed time since invocation"
    Write-Host $NewResponse.Content -ForegroundColor Green
    Write-Host $elapsedTime.TotalSeconds "seconds elapsed since $currentDate" -ForegroundColor Magenta
    Write-Host "Local IP address has been updated $counter times." -ForegroundColor Blue
    Write-Host 
    Write-Host
    Start-Sleep -Milliseconds $waitTime
    $counter++
} until ($infinity)