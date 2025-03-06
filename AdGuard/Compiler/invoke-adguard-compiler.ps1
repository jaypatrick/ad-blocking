# prior to running this script, use npm to install hostlist-compiler globally (npm install -g hostlist-compiler)
$workingDirectory = D:\source\repos\ad-blocking\AdGuard\Compiler\
$outputFileName = adguard_user_filter.txt
$configFileName = config.json

Set-Location -Path $workingDirectory
hostlist-compiler --config $configFileName --output $outputFileName
$outputFileHash = get-filehash $outputFileName -Algorithm SHA384
write-host $outputFileHash

# Copy to Rules directory
Copy-Item -Path $outputFileName -Destination D:\source\repos\ad-blocking\AdGuard\Rules\adguard_user_filter.txt -Force

pause 