# Phase 2: Shell Reorganization + Environment Variables

## Status: IN PROGRESS

This document tracks the implementation of Phase 2 of the PowerShell/Shell modernization project.

## Completed ✅

### Directory Structure
- ✅ Created `src/shell-scripts/bash/`
- ✅ Created `src/shell-scripts/zsh/`
- ✅ Created `src/powershell-modules/` hierarchy
- ✅ Copied shell scripts to new locations

### Documentation
- ✅ Created comprehensive [Environment Variables Reference](docs/ENVIRONMENT_VARIABLES.md)
- ✅ Created Migration script (`Migrate-To-NewStructure.ps1`)
- ✅ Created CompilerConfiguration class (first OOP class)

### Environment Variables Defined
All environment variables are now documented:
- `ADGUARD_COMPILER_*` (6 variables)
- `ADGUARD_WEBHOOK_*` (6 variables)
- `ADGUARD_AdGuard__*` (2 variables for .NET)
- `DEBUG` (global)

## Remaining Work 🔨

### 1. Add Environment Variable Support to PowerShell Modules

**File**: `src/adguard-api-powershell/Invoke-RulesCompiler.psm1`

Add to the BEGIN block of `Invoke-RulesCompiler`:
```powershell
# Load from environment variables if not specified
if (-not $ConfigPath -and $env:ADGUARD_COMPILER_CONFIG) {
    $ConfigPath = $env:ADGUARD_COMPILER_CONFIG
}

if (-not $OutputPath -and $env:ADGUARD_COMPILER_OUTPUT) {
    $OutputPath = Join-Path $env:ADGUARD_COMPILER_OUTPUT $DefaultOutputFile
}

if (-not $RulesPath -and $env:ADGUARD_COMPILER_RULES_DIR) {
    $RulesPath = $env:ADGUARD_COMPILER_RULES_DIR
}

if ($env:ADGUARD_COMPILER_VERBOSE -eq 'true' -or $env:ADGUARD_COMPILER_VERBOSE -eq '1') {
    $VerbosePreference = 'Continue'
}
```

**File**: `src/adguard-api-powershell/Invoke-WebHook.psm1`

Add to BEGIN block of `Invoke-AdGuardWebhook`:
```powershell
# Load from environment variables
if (-not $WebhookUrl -and $env:ADGUARD_WEBHOOK_URL) {
    $WebhookUrl = $env:ADGUARD_WEBHOOK_URL
}

if (-not $PSBoundParameters.ContainsKey('WaitTime') -and $env:ADGUARD_WEBHOOK_WAIT_TIME) {
    $WaitTime = [int]$env:ADGUARD_WEBHOOK_WAIT_TIME
}

if (-not $PSBoundParameters.ContainsKey('RetryCount') -and $env:ADGUARD_WEBHOOK_RETRY_COUNT) {
    $RetryCount = [int]$env:ADGUARD_WEBHOOK_RETRY_COUNT
}

if (-not $PSBoundParameters.ContainsKey('RetryInterval') -and $env:ADGUARD_WEBHOOK_RETRY_INTERVAL) {
    $RetryInterval = [int]$env:ADGUARD_WEBHOOK_RETRY_INTERVAL
}

if (-not $PSBoundParameters.ContainsKey('Format') -and $env:ADGUARD_WEBHOOK_FORMAT) {
    $Format = $env:ADGUARD_WEBHOOK_FORMAT
}
```

### 2. Add Environment Variable Support to Bash Script

**File**: `src/shell-scripts/bash/compile-rules.sh`

Add after the "Configuration" section (around line 52):
```bash
# Load from environment variables
CONFIG_PATH="${ADGUARD_COMPILER_CONFIG:-}"
OUTPUT_PATH="${ADGUARD_COMPILER_OUTPUT:-}"
COPY_TO_RULES="${ADGUARD_COMPILER_COPY_TO_RULES:-false}"
FORMAT="${ADGUARD_COMPILER_FORMAT:-}"

# Check for DEBUG environment variable
if [[ "${DEBUG}" == "1" ]] || [[ "${DEBUG,,}" == "true" ]]; then
    DEBUG=true
fi

# Check for verbose
if [[ "${ADGUARD_COMPILER_VERBOSE}" == "1" ]] || [[ "${ADGUARD_COMPILER_VERBOSE,,}" == "true" ]]; then
    set -x
fi
```

### 3. Add Environment Variable Support to Zsh Script

**File**: `src/shell-scripts/zsh/compile-rules.zsh`

Add after configuration variables (around line 53):
```zsh
# Load from environment variables
CONFIG_PATH="${ADGUARD_COMPILER_CONFIG:-}"
OUTPUT_PATH="${ADGUARD_COMPILER_OUTPUT:-}"
(( ${+ADGUARD_COMPILER_COPY_TO_RULES} )) && COPY_TO_RULES=1
FORMAT="${ADGUARD_COMPILER_FORMAT:-}"

# Check for DEBUG
if [[ "${DEBUG}" == "1" ]] || [[ "${DEBUG:l}" == "true" ]]; then
    DEBUG=1
fi

# Check for verbose
if [[ "${ADGUARD_COMPILER_VERBOSE}" == "1" ]] || [[ "${ADGUARD_COMPILER_VERBOSE:l}" == "true" ]]; then
    setopt XTRACE
fi
```

### 4. Create Unit Tests

**File**: `src/powershell-modules/RulesCompiler/Tests/EnvironmentVariables.Tests.ps1`
```powershell
Describe "Environment Variables - RulesCompiler" {
    BeforeEach {
        # Save current environment
        $script:savedEnv = @{}
        @('ADGUARD_COMPILER_CONFIG', 'ADGUARD_COMPILER_OUTPUT', 'ADGUARD_COMPILER_RULES_DIR') | ForEach-Object {
            $script:savedEnv[$_] = Get-Item "env:$_" -ErrorAction SilentlyContinue
        }
    }
    
    AfterEach {
        # Restore environment
        $script:savedEnv.Keys | ForEach-Object {
            if ($script:savedEnv[$_]) {
                Set-Item "env:$_" -Value $script:savedEnv[$_].Value
            } else {
                Remove-Item "env:$_" -ErrorAction SilentlyContinue
            }
        }
    }
    
    It "Should load config path from ADGUARD_COMPILER_CONFIG" {
        $env:ADGUARD_COMPILER_CONFIG = "test-config.yaml"
        # Test implementation
    }
    
    It "Should load output path from ADGUARD_COMPILER_OUTPUT" {
        $env:ADGUARD_COMPILER_OUTPUT = "/tmp/output"
        # Test implementation
    }
    
    # Add more tests...
}
```

**File**: `src/powershell-modules/AdGuardWebhook/Tests/EnvironmentVariables.Tests.ps1`
```powershell
Describe "Environment Variables - Webhook" {
    BeforeEach {
        # Save and clear environment
        $script:savedEnv = @{}
        @('ADGUARD_WEBHOOK_URL', 'ADGUARD_WEBHOOK_WAIT_TIME') | ForEach-Object {
            $script:savedEnv[$_] = Get-Item "env:$_" -ErrorAction SilentlyContinue
            Remove-Item "env:$_" -ErrorAction SilentlyContinue
        }
    }
    
    AfterEach {
        # Restore environment
        $script:savedEnv.Keys | ForEach-Object {
            if ($script:savedEnv[$_]) {
                Set-Item "env:$_" -Value $script:savedEnv[$_].Value
            }
        }
    }
    
    It "Should load webhook URL from ADGUARD_WEBHOOK_URL" {
        $env:ADGUARD_WEBHOOK_URL = "https://example.com/webhook"
        # Test implementation
    }
    
    It "Should load wait time from ADGUARD_WEBHOOK_WAIT_TIME" {
        $env:ADGUARD_WEBHOOK_WAIT_TIME = "500"
        # Test implementation
    }
    
    # Add more tests...
}
```

### 5. Create Shell Script Tests

**File**: `src/shell-scripts/bash/test-compile-rules.sh`
```bash
#!/usr/bin/env bash
# Basic tests for compile-rules.sh

test_env_var_config() {
    export ADGUARD_COMPILER_CONFIG="test-config.json"
    result=$(./compile-rules.sh --help 2>&1 | grep -c "test-config.json")
    if [[ $result -gt 0 ]]; then
        echo "✓ Environment variable CONFIG test passed"
        return 0
    else
        echo "✗ Environment variable CONFIG test failed"
        return 1
    fi
}

# Run tests
test_env_var_config
```

### 6. Create READMEs

**File**: `src/shell-scripts/README.md` - Overview of shell scripts
**File**: `src/shell-scripts/bash/README.md` - Bash-specific documentation (already created)
**File**: `src/shell-scripts/zsh/README.md` - Zsh-specific documentation
**File**: `src/powershell-modules/README.md` - PowerShell modules overview

### 7. Update Existing PowerShell Tests

Add environment variable tests to:
- `src/adguard-api-powershell/Tests/RulesCompiler-Tests.ps1`
- `src/adguard-api-powershell/Tests/Webhook-Tests.ps1`

## Implementation Steps

1. **Add env var support to PowerShell** (30 minutes)
   - Edit Invoke-RulesCompiler.psm1
   - Edit Invoke-WebHook.psm1
   - Test manually

2. **Add env var support to shell scripts** (20 minutes)
   - Edit bash/compile-rules.sh
   - Edit zsh/compile-rules.zsh
   - Test manually

3. **Create unit tests** (1 hour)
   - Create new test files
   - Update existing test files
   - Run all tests: `Invoke-Pester`

4. **Create READMEs** (30 minutes)
   - Shell scripts overview
   - Zsh-specific README
   - PowerShell modules README

5. **Test everything** (30 minutes)
   - Test env vars on PowerShell
   - Test env vars on bash (WSL if on Windows)
   - Run all Pester tests

6. **Commit and create PR** (15 minutes)
   - Commit with descriptive message
   - Push to branch
   - Create PR #2

## Testing Checklist

- [ ] PowerShell: Config path from env var
- [ ] PowerShell: Output path from env var
- [ ] PowerShell: Rules dir from env var
- [ ] PowerShell: Verbose from env var
- [ ] PowerShell: DEBUG mode
- [ ] Webhook: URL from env var
- [ ] Webhook: All timing params from env vars
- [ ] Webhook: Format from env var
- [ ] Bash: All env vars work
- [ ] Zsh: All env vars work
- [ ] Priority order correct (env < config < CLI)
- [ ] Pester tests pass
- [ ] PSScriptAnalyzer passes

## Notes

- Maintain 100% backward compatibility
- CLI parameters always override environment variables
- Document breaking changes (if any)
- Update CHANGELOG.md

## Next Phase (Phase 3)

Phase 3 will focus on:
- OOP refactoring of PowerShell modules
- Splitting into Public/Private/Classes structure
- Advanced class implementations
- Enhanced test coverage
