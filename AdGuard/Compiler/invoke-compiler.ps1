# prior to running this script, use npm to install hostlist-compiler globally (npm install -g hostlist-compiler)
# I am working on a .psm to allow more flexibility, but for now change the paths to match your environment
$workingDirectory = D:\source\repos\ad-blocking\AdGuard\Compiler\
$outputFileName = adguard_user_filter.txt
$configFileName = compiler-config.json # this file should be in the same directory as this script (for now) and should be a valid hostlist-compiler config file

Set-Location -Path $workingDirectory
hostlist-compiler --config $configFileName --output $outputFileName
$outputFileHash = get-filehash $outputFileName -Algorithm SHA384
write-host $outputFileHash

# Copy to Rules directory
Copy-Item -Path $outputFileName -Destination D:\source\repos\ad-blocking\AdGuard\Rules\adguard_user_filter.txt -Force

pause 