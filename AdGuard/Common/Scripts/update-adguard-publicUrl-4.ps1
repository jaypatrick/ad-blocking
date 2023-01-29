Clear-Host
$Results = @()
$Hosts = @()
$waitTime = 1000
$stopwatch = [system.diagnostics.stopwatch]::StartNew()
$NewUri = "https://linkip.adguard-dns.com/linkip/db94e3e9/8AdnEQlPCjyMaX74vTDZkraUDUYpCFiZ1tcH8dSk9VH"
$IPRegex = "((?:(?:1\d\d|2[0-5][0-5]|2[0-4]\d|0?[1-9]\d|0?0?\d)\.){3}(?:1\d\d|2[0-5][0-5]|2[0-4]\d|0?[1-9]\d|0?0?\d))"

#Checking log file
$UriRequest = Invoke-WebRequest -Uri $NewUri
$Lines = $UriRequest.Content
$Lines

do {
    #Getting IP Addresses
    Foreach ($Line in $Lines) {
        $IP = $Object1 = $null
        $IP = ($Line  |  Select-String -Pattern $IPRegex -AllMatches).Matches.Value
        IF ($IP -notmatch "0.0.0.0") {
            $Object1 = New-Object PSObject -Property @{ 
 
                IPAddress = $IP
            }
            $Results += $Object1   
        }
    }

    $Results

    #Selecting unique IPs
    $NewIP = $Results | Select-Object IPAddress -Unique
    $NewIP

    #Checking hostname
    Foreach ($Item in $IPUnique) {
        $HostName = $Object2 = $null
        $HostName = (Resolve-DnsName $Item.IPAddress -ErrorAction SilentlyContinue).NAMEHOST
        If (!$HostName) { $Hostname = "None" }
        $Object2 = New-Object PSObject -Property @{ 
 
            IPAddress = $item.ipaddress
            NameHost  = $HostName
 
        }
        $Hosts += $Object2   
    }
    $Hosts.IPAddress #| Out-GridView -Title "Hostnames"
    $Hosts.Clear()
    Start-Sleep -Milliseconds $waitTime
} while ($true)
$stopwatch.Stop()
$stopwatch