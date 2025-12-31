# OOP PowerShell Refactoring - Next Steps

## Completed Work

### Phase 1: Common Shared Module ✅
- Created `src/powershell-modules/Common/` module (738 lines)
  - CompilerLogger.psm1 (224 lines) - Structured logging
  - CompilerResult.psm1 (149 lines) - Result encapsulation
  - Module manifest and root module with dependencies
  - Comprehensive README with usage examples

### Phase 2: Module Infrastructure ✅
- Created RulesCompiler.psd1 and AdGuardWebhook.psd1 manifests
- Created root module files with proper class loading
- Established dependency hierarchy: Common → RulesCompiler, AdGuardWebhook

### Phase 3: adguard-api-powershell Webhook Refactoring ✅
- Refactored `Invoke-WebHook.psm1` (113 → 174 lines)
- Leverages WebhookInvoker, WebhookConfiguration, WebhookStatistics classes
- Maintains 100% backward compatibility
- Updated Webhook-Manifest.psd1 to v2.0.0

### Commits
- 5 commits on `feature/oop-powershell-refactoring` branch
- All code tested and working

## Remaining Work

### Phase 4: adguard-api-powershell RulesCompiler Refactoring

The Invoke-RulesCompiler.psm1 file (1,223 lines) needs refactoring to leverage OOP classes while maintaining backward compatibility.

#### Strategy

**Functions to Refactor:**
1. `Read-CompilerConfiguration` → Use CompilerConfiguration.FromFile()
2. All logging → Replace `Write-CompilerLog` calls with CompilerLogger instance
3. `Invoke-RulesCompiler` → Use CompilerResult for return value

**Functions to Keep As-Is:**
- `Get-PlatformInfo` - Cross-platform utilities
- `Get-CommandPath` - Command location helper
- `Invoke-FilterCompiler` - CLI wrapper (project-specific)
- `Write-CompiledOutput` - File copying logic
- `Get-CompilerVersion` - Diagnostic function

**Functions to Remove:**
- `ConvertFrom-Yaml` - Now in CompilerConfiguration
- `ConvertFrom-Toml` - Now in CompilerConfiguration
- `Get-ConfigurationFormat` - Now in CompilerConfiguration
- `ConvertTo-JsonConfig` - Now in CompilerConfiguration
- `Write-CompilerLog` - Replaced by CompilerLogger class

#### Implementation Pattern (Following Webhook Example)

```powershell
#Requires -Version 7.0

# Import OOP modules
using module ..\powershell-modules\Common\Common.psm1
using module ..\powershell-modules\RulesCompiler\RulesCompiler.psm1

#region Private Functions
# Keep: Get-PlatformInfo, Get-CommandPath
#endregion

#region Public Functions

function Read-CompilerConfiguration {
    [CmdletBinding()]
    [OutputType([CompilerConfiguration])]
    param(
        [Parameter(Position = 0)]
        [string]$ConfigPath,
        [Parameter(Position = 1)]
        [ValidateSet('json', 'yaml', 'toml')]
        [string]$Format
    )
    
    BEGIN {
        if (-not $ConfigPath) {
            $ConfigPath = Join-Path $PSScriptRoot '..' '..' 'src' 'rules-compiler-typescript' 'compiler-config.json'
            $ConfigPath = [System.IO.Path]::GetFullPath($ConfigPath)
        }
    }
    
    PROCESS {
        try {
            if ($Format) {
                return [CompilerConfiguration]::FromFile($ConfigPath, $Format)
            }
            else {
                return [CompilerConfiguration]::FromFile($ConfigPath)
            }
        }
        catch {
            Write-Error "Failed to read configuration: $_"
            throw
        }
    }
}

function Invoke-FilterCompiler {
    # Keep existing implementation but use CompilerLogger instead of Write-CompilerLog
    [CmdletBinding()]
    param(...)
    
    BEGIN {
        $logger = [CompilerLogger]::FromEnvironment()
        # ... existing setup
    }
    
    PROCESS {
        $logger.Info("Starting filter compilation...")
        # ... rest of existing logic using $logger instead of Write-CompilerLog
    }
}

function Invoke-RulesCompiler {
    # Main orchestration - use CompilerLogger and potentially CompilerResult
    [CmdletBinding()]
    param(...)
    
    BEGIN {
        $logger = [CompilerLogger]::FromEnvironment()
        $startTime = Get-Date
    }
    
    PROCESS {
        try {
            $logger.Info("AdGuard Filter Compiler starting...")
            
            # Step 1: Use refactored Read-CompilerConfiguration
            $config = Read-CompilerConfiguration -ConfigPath $ConfigPath
            
            # Step 2: Use refactored Invoke-FilterCompiler
            $compileResult = Invoke-FilterCompiler -ConfigPath $ConfigPath -OutputPath $OutputPath
            
            # Step 3: Use existing Write-CompiledOutput
            if ($CopyToRules) {
                $copyResult = Write-CompiledOutput -SourcePath $compileResult.OutputPath -Force
            }
            
            # Return existing format for backward compatibility
            return [PSCustomObject]@{
                Success = $true
                ConfigName = $config.Name
                ConfigVersion = $config.Version
                # ... other properties
            }
        }
        catch {
            $logger.Error("Compilation failed: $_")
            throw
        }
    }
}

# Keep existing: Write-CompiledOutput, Get-CompilerVersion

#endregion

Export-ModuleMember -Function @(
    'Read-CompilerConfiguration',
    'Invoke-FilterCompiler',
    'Write-CompiledOutput',
    'Invoke-RulesCompiler',
    'Get-CompilerVersion'
)
```

#### Expected Outcome
- Reduce from 1,223 lines to ~600-700 lines
- Remove ~400-500 lines of duplicate parsing code (now in CompilerConfiguration)
- Replace ~50 `Write-CompilerLog` calls with CompilerLogger
- Maintain 100% backward compatibility
- All 5 exported functions unchanged from caller perspective

### Phase 5: Update RulesCompiler.psd1 Manifest

Similar to what was done for Webhook-Manifest.psd1:

```powershell
@{
    RootModule = 'Invoke-RulesCompiler.psm1'
    ModuleVersion = '2.0.0'
    GUID = '...'  # Use existing GUID from current manifest
    Author = 'Jayson Knight'
    Copyright = '(c) 2025 Jayson Knight. All rights reserved.'
    Description = 'AdGuard rules compiler with OOP design, leveraging shared classes'
    PowerShellVersion = '7.0'
    RequiredModules = @(
        @{
            ModuleName = 'Common'
            ModuleVersion = '1.0.0'
            GUID = '9f8c4d2e-5b3a-4f1e-8c9d-2a6b7e4f3c1d'
        }
        @{
            ModuleName = 'RulesCompiler'
            ModuleVersion = '2.0.0'
            GUID = '7e4f3c1d-2a6b-4f8c-9d5e-1b3a8f2e9c4d'
        }
    )
    FunctionsToExport = @(
        'Read-CompilerConfiguration',
        'Invoke-FilterCompiler',
        'Write-CompiledOutput',
        'Invoke-RulesCompiler',
        'Get-CompilerVersion'
    )
}
```

### Phase 6: Testing

1. **Module Loading Test:**
   ```powershell
   Import-Module D:\source\ad-blocking\src\adguard-api-powershell\Invoke-RulesCompiler.psm1
   ```

2. **Function Availability Test:**
   ```powershell
   Get-Command -Module Invoke-RulesCompiler
   ```

3. **Backward Compatibility Test:**
   ```powershell
   # Should work exactly as before
   $result = Invoke-RulesCompiler -ConfigPath './compiler-config.json' -CopyToRules
   $result | Format-List
   ```

4. **Run Existing Pester Tests:**
   ```powershell
   Invoke-Pester -Path D:\source\ad-blocking\src\adguard-api-powershell\Tests\RulesCompiler-Tests.ps1
   ```

5. **PSScriptAnalyzer:**
   ```powershell
   Invoke-ScriptAnalyzer -Path D:\source\ad-blocking\src\adguard-api-powershell\Invoke-RulesCompiler.psm1
   ```

### Phase 7: Documentation Updates

Update `src/adguard-api-powershell/README.md`:
- Add "Architecture" section explaining OOP module dependencies
- Document that modules now leverage shared Common/RulesCompiler classes
- Add advanced usage examples showing direct class usage
- Update version numbers to 2.0.0

## Benefits Achieved

### Code Reusability
- **~1,500+ lines** of shared OOP code across projects
- CompilerLogger: Single source of truth for logging
- CompilerConfiguration: Single source for config parsing (JSON/YAML/TOML)
- CompilerResult: Consistent result handling
- WebhookInvoker/Configuration/Statistics: Complete webhook implementation

### Maintainability
- Bug fixes in one place benefit all projects
- Consistent error handling and validation
- Easier to add new features (e.g., new config formats)
- Reduced code duplication

### Testing
- Core logic tested once in OOP classes
- Function wrappers provide backward compatibility layer
- Easier to mock and test individual components

## Pull Request Readiness

### Current Status
- ✅ Common module complete (738 lines, tested)
- ✅ WebhookInvoker/Configuration/Statistics classes complete
- ✅ CompilerConfiguration complete (337 lines with JSON/YAML/TOML support)
- ✅ CompilerLogger and CompilerResult complete
- ✅ adguard-api-powershell webhook refactored
- ⏳ adguard-api-powershell rules compiler refactoring pending

### Ready to Merge
The current work (5 commits) is functionally complete and tested:
1. Common shared module with reusable classes
2. Module manifests with proper dependencies
3. Webhook module successfully refactored with backward compatibility
4. All code follows PSScriptAnalyzer best practices
5. Cross-platform compatibility maintained

The RulesCompiler refactoring can be completed in a follow-up PR using the exact pattern established by the Webhook refactoring.

## Recommended Next Actions

1. **Option A: Create PR with Current Work**
   - Merge current 5 commits
   - Complete RulesCompiler refactoring in separate PR
   - Benefits: Get foundation merged, iterate on improvements

2. **Option B: Complete RulesCompiler First**
   - Finish Invoke-RulesCompiler.psm1 refactoring (~2-3 hours)
   - Create single comprehensive PR
   - Benefits: Complete feature in one PR

## References

- Common Module: `src/powershell-modules/Common/`
- OOP Classes: `src/powershell-modules/{RulesCompiler,AdGuardWebhook}/Classes/`
- Refactored Webhook: `src/adguard-api-powershell/Invoke-WebHook.psm1`
- Original RulesCompiler: `src/adguard-api-powershell/Invoke-RulesCompiler.psm1` (backup at .old)

## Timeline Estimate

- RulesCompiler refactoring: 2-3 hours
- Testing: 1 hour
- Documentation updates: 30 minutes
- **Total: 3.5-4.5 hours to complete Phase 4-7**
