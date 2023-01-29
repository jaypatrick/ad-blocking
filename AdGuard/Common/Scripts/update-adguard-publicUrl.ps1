$newUri = "https://linkip.adguard-dns.com/linkip/db94e3e9/8AdnEQlPCjyMaX74vTDZkraUDUYpCFiZ1tcH8dSk9VH"
$waitTime = 1000
$stopwatch = [system.diagnostics.stopwatch]::StartNew()
$NewResponse = "New Response"
$OldResponse = "Old Response"
$OutputFile = ".\Output.log"
$RequestsSucceeded = 0
$RequestsFailed = 0
$TotalRequests = 0
Clear-Host
# New-Item -Path $OutputFile -ItemType File
$IPRegex = '((?:(?:1\d\d|2[0-5][0-5]|2[0-4]\d|0?[1-9]\d|0?0?\d)\.){3}(?:1\d\d|2[0-5][0-5]|2[0-4]\d|0?[1-9]\d|0?0?\d))'
do {
    try {
        $OldResponse = Invoke-WebRequest -Uri $newUri -PassThru -OutFile $OutputFile
        $NewMatch = $ParsedNewUrl.Matches.Value 
        $OldMatch = $ParsedOldUrl.Matches.Value
        $OldResponse.Content
        if ($NewMatch -ieq $OldMatch) {
            Write-Host $NewMatch "is equal to " $OldMatch `r`n "Processing next request..." -ForegroundColor Red
            continue 
        }
        else {
            Write-Host $NewMatch "is NOT equal to " $OldMatch `r`n "Processing next request..." -ForegroundColor Green
            Write-Host "Local Count: " $RequestsSucceeded $Response
            $RequestsSucceeded++
            $OldResponse = $NewResponse
            $NewResponse
        }
    } 
    catch {
        $StatusCode = = $_.Exception.Response.StatusCode.value__
        Write-Host "Failed Count: " $StatusCode
        $RequestsFailed++
        Write-Debug "This is the debug stream, status code is $StatusCode"
    }
    finally {
        $NewResponse = Invoke-WebRequest -Uri $newUri -PassThru -OutFile $OutputFile
        $TotalRequests++
        # $OldResponse = $NewResponse
        Write-Host "Global Count: $TotalRequests" `r`n
        # $ParsedNewUrl, $ParsedOldUrl | Select-String -Pattern $IPRegex -InputObject $NewResponse.Content
        $ParsedOldUrl = Select-String -Pattern $IPRegex -InputObject $OldResponse.Content -AllMatches
        $ParsedNewUrl = Select-String -Pattern $IPRegex -InputObject $NewResponse.Content -AllMatches
        Write-Verbose "Finally block entered. $TotalRequests"
        Get-Content -Path $OutputFile
        Start-Sleep -Milliseconds $waitTime
    }
    
}#  while ($NewResponse -ine $OldResponse)
until ($infinity)

$stopwatch.Stop()
$stopwatch