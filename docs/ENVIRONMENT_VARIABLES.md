# Environment Variables Reference

Comprehensive guide to environment variables supported by AdGuard scripts and modules.

## Overview

All AdGuard scripts and modules support environment variables for configuration. This allows for:
- Easy CI/CD integration
- Containerized deployments
- User-specific defaults
- Cross-platform consistency

## Rules Compiler

### ADGUARD_COMPILER_CONFIG
**Description**: Default configuration file path  
**Type**: String (file path)  
**Default**: `compiler-config.json`  
**Example**:
```bash
export ADGUARD_COMPILER_CONFIG="$HOME/.adguard/my-config.yaml"
```

### ADGUARD_COMPILER_OUTPUT
**Description**: Default output directory  
**Type**: String (directory path)  
**Default**: `./output`  
**Example**:
```bash
export ADGUARD_COMPILER_OUTPUT="/var/adguard/rules"
```

### ADGUARD_COMPILER_RULES_DIR
**Description**: Default rules directory for copying compiled output  
**Type**: String (directory path)  
**Default**: `./rules`  
**Example**:
```bash
export ADGUARD_COMPILER_RULES_DIR="/etc/adguard/rules"
```

### ADGUARD_COMPILER_FORMAT
**Description**: Default configuration format  
**Type**: String (`json`, `yaml`, `toml`)  
**Default**: Auto-detected from file extension  
**Example**:
```bash
export ADGUARD_COMPILER_FORMAT="yaml"
```

### ADGUARD_COMPILER_VERBOSE
**Description**: Enable verbose logging  
**Type**: Boolean (`true`, `false`, `1`, `0`)  
**Default**: `false`  
**Example**:
```bash
export ADGUARD_COMPILER_VERBOSE=true
```

### DEBUG
**Description**: Enable debug mode (affects all modules)  
**Type**: Boolean (`true`, `false`, `1`, `0`)  
**Default**: `false`  
**Example**:
```bash
export DEBUG=1
```

## Webhook Module

### ADGUARD_WEBHOOK_URL
**Description**: Default webhook endpoint URL  
**Type**: String (URL)  
**Default**: None (required if not specified via parameter)  
**Example**:
```bash
export ADGUARD_WEBHOOK_URL="https://api.adguard-dns.io/webhook/xxx"
```

### ADGUARD_WEBHOOK_CONFIG
**Description**: Path to webhook configuration file  
**Type**: String (file path)  
**Default**: None  
**Example**:
```bash
export ADGUARD_WEBHOOK_CONFIG="$HOME/.adguard/webhook-config.json"
```

### ADGUARD_WEBHOOK_WAIT_TIME
**Description**: Default wait time between invocations (milliseconds)  
**Type**: Integer (200-∞)  
**Default**: `200`  
**Example**:
```bash
export ADGUARD_WEBHOOK_WAIT_TIME=500
```

### ADGUARD_WEBHOOK_RETRY_COUNT
**Description**: Default number of retry attempts  
**Type**: Integer (0-100)  
**Default**: `10`  
**Example**:
```bash
export ADGUARD_WEBHOOK_RETRY_COUNT=5
```

### ADGUARD_WEBHOOK_RETRY_INTERVAL
**Description**: Default retry interval (seconds)  
**Type**: Integer (1-60)  
**Default**: `5`  
**Example**:
```bash
export ADGUARD_WEBHOOK_RETRY_INTERVAL=10
```

### ADGUARD_WEBHOOK_FORMAT
**Description**: Default output format for statistics  
**Type**: String (`Table`, `List`, `Json`)  
**Default**: `Table`  
**Example**:
```bash
export ADGUARD_WEBHOOK_FORMAT="Json"
```

## API Client (C#/.NET)

### ADGUARD_AdGuard__ApiKey
**Description**: AdGuard DNS API key (.NET configuration format)  
**Type**: String  
**Default**: None  
**Example**:
```bash
export ADGUARD_AdGuard__ApiKey="your-api-key-here"
```

### ADGUARD_AdGuard__BaseUrl
**Description**: API base URL (.NET configuration format)  
**Type**: String (URL)  
**Default**: `https://api.adguard-dns.io`  
**Example**:
```bash
export ADGUARD_AdGuard__BaseUrl="https://api.adguard-dns.io"
```

## Cross-Platform Usage

### PowerShell (Windows/Linux/macOS)
```powershell
# Set for current session
$env:ADGUARD_COMPILER_CONFIG = "config.yaml"
$env:ADGUARD_WEBHOOK_URL = "https://example.com/webhook"

# Set permanently (Windows)
[System.Environment]::SetEnvironmentVariable('ADGUARD_COMPILER_CONFIG', 'config.yaml', 'User')

# Set permanently (Linux/macOS) - add to ~/.profile or ~/.zshrc
```

### Bash/Zsh (Linux/macOS)
```bash
# Set for current session
export ADGUARD_COMPILER_CONFIG="config.yaml"
export ADGUARD_WEBHOOK_URL="https://example.com/webhook"

# Set permanently - add to ~/.bashrc or ~/.zshrc
echo 'export ADGUARD_COMPILER_CONFIG="config.yaml"' >> ~/.bashrc
```

### Docker
```dockerfile
ENV ADGUARD_COMPILER_CONFIG=/app/config.yaml \
    ADGUARD_WEBHOOK_URL=https://api.adguard-dns.io/webhook/xxx \
    DEBUG=1
```

### Docker Compose
```yaml
services:
  adguard-compiler:
    image: adguard-compiler
    environment:
      - ADGUARD_COMPILER_CONFIG=/app/config.yaml
      - ADGUARD_COMPILER_OUTPUT=/app/output
      - ADGUARD_COMPILER_VERBOSE=true
```

### CI/CD (GitHub Actions)
```yaml
env:
  ADGUARD_COMPILER_CONFIG: ${{ secrets.COMPILER_CONFIG }}
  ADGUARD_WEBHOOK_URL: ${{ secrets.WEBHOOK_URL }}
  DEBUG: true

steps:
  - name: Compile rules
    run: |
      ./src/shell-scripts/bash/compile-rules.sh -r
```

## Priority Order

When multiple configuration sources are available, they are applied in this order (later overrides earlier):

1. **Default values** (hardcoded in scripts)
2. **Environment variables** (system or session)
3. **Configuration files** (if specified)
4. **Command-line parameters** (highest priority)

## Best Practices

### Development
- Use configuration files for complex setups
- Use environment variables for simple overrides
- Use command-line parameters for one-off changes

### Production
- Store sensitive values in environment variables (not in config files)
- Use a `.env` file or secrets manager
- Document required environment variables in deployment guides

### CI/CD
- Use secrets management for sensitive values
- Set non-sensitive defaults in workflow files
- Validate required environment variables before execution

## Validation

Most scripts will validate environment variables and provide helpful error messages:

```bash
# Missing required variable
$ ./compile-rules.sh
Error: ADGUARD_COMPILER_CONFIG not set and no config file found

# Invalid value
$ ADGUARD_WEBHOOK_WAIT_TIME=50 ./webhook.ps1
Error: ADGUARD_WEBHOOK_WAIT_TIME must be >= 200 milliseconds

# Type mismatch
$ ADGUARD_COMPILER_VERBOSE=maybe ./compile-rules.sh
Warning: Invalid boolean value 'maybe', using default 'false'
```

## Troubleshooting

### Check if variable is set
```bash
# Bash/Zsh
echo $ADGUARD_COMPILER_CONFIG

# PowerShell
$env:ADGUARD_COMPILER_CONFIG

# Show all AdGuard variables
env | grep ADGUARD  # Bash/Zsh
Get-ChildItem env: | Where-Object Name -like "ADGUARD*"  # PowerShell
```

### Clear variable
```bash
# Bash/Zsh
unset ADGUARD_COMPILER_CONFIG

# PowerShell
Remove-Item env:ADGUARD_COMPILER_CONFIG
```

### Debug mode
Enable debug mode to see which environment variables are being used:

```bash
DEBUG=1 ./compile-rules.sh

# Output will show:
# [DEBUG] Loading environment variables...
# [DEBUG] ADGUARD_COMPILER_CONFIG=/home/user/config.yaml
# [DEBUG] ADGUARD_COMPILER_VERBOSE=true
```

## See Also

- [PowerShell Modules README](../src/powershell-modules/README.md)
- [Shell Scripts README](../src/shell-scripts/README.md)
- [Configuration Reference](./configuration-reference.md)
