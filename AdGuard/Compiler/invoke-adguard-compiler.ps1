$workingDirectory = D:\source\repos\ad-blocking\AdGuard\Compiler\
$outputFileName = adguard_user_filter.txt
$configFileName = config.json

cd $workingDirectory
hostlist-compiler --config config.json --output $outputFileName
$outputFileHash = get-filehash $outputFileName -Algorithm SHA384
write-host $outputFileHash

# Copy to Rules directory
Copy-Item -Path $outputFileName -Destination D:\source\repos\ad-blocking\AdGuard\Rules\adguard_user_filter.txt -Force

pause 