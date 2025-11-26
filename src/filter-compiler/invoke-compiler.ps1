# Prior to running this script, use npm to install hostlist-compiler globally (npm install -g @adguard/hostlist-compiler)
# This script uses relative paths and should be run from the Compiler directory or will navigate there automatically

$workingDirectory = $PSScriptRoot
$outputFileName = "adguard_user_filter.txt"
$configFileName = "compiler-config.json"
$rulesDirectory = Join-Path $workingDirectory ".." "Rules"

Write-Host "Working directory: $workingDirectory"
Write-Host "Output file: $outputFileName"
Write-Host "Config file: $configFileName"

Set-Location -Path $workingDirectory

# Compile the filters
Write-Host "Compiling filters..."
hostlist-compiler --config $configFileName --output $outputFileName

if ($LASTEXITCODE -ne 0) {
    Write-Error "Compilation failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

# Get file hash
$outputFileHash = Get-FileHash $outputFileName -Algorithm SHA384
Write-Host "Output file hash: $($outputFileHash.Hash)"

# Copy to Rules directory
$rulesOutputPath = Join-Path $rulesDirectory $outputFileName
Write-Host "Copying to Rules directory: $rulesOutputPath"
Copy-Item -Path $outputFileName -Destination $rulesOutputPath -Force

Write-Host "Compilation complete!" 