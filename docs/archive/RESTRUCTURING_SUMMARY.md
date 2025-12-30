# Repository Restructuring Summary

## Overview

This document summarizes the repository structure improvements made to consolidate and organize the ad-blocking multi-language monorepo according to 2024 GitHub and monorepo best practices.

## Changes Made

### 1. Centralized API Specifications

**Before:**
- `src/adguard-api-dotnet/api/openapi.json`
- `src/adguard-api-typescript/api/openapi.json`
- (Duplicate files in multiple locations)

**After:**
```
api/
├── README.md
├── openapi.json    # Single source of truth
└── openapi.yaml
```

**Benefits:**
- Single source of truth for OpenAPI specs
- Easier to update across all SDKs
- Follows GitHub repository standards

### 2. Consolidated Tools Directory

**Before:**
- `scripts/check-validation-compliance.sh`
- `test-build-scripts.sh` (root)
- `test-build-scripts.ps1` (root)
- `test-modules.ps1` (root)
- `Migrate-To-NewStructure.ps1` (root)

**After:**
```
tools/
├── README.md
├── check-validation-compliance.sh
├── test-build-scripts.sh
├── test-build-scripts.ps1
├── test-modules.ps1
└── Migrate-To-NewStructure.ps1
```

**Benefits:**
- All utility scripts in one place
- Cleaner root directory
- Standard monorepo practice

### 3. Organized Documentation

**Before:**
- `AGENTS.md` (root)
- `PHASE2_IMPLEMENTATION.md` (root)
- `RUST_WORKSPACE.md` (root)
- `RUST_MODERNIZATION_SUMMARY.md` (root)
- `TEST_UPDATES_SUMMARY.md` (root)
- `WARP.md` (root)

**After:**
```
docs/
├── api/                            # Auto-generated API docs
├── guides/                         # Usage guides
├── AGENTS.md                       # Moved from root
├── PHASE2_IMPLEMENTATION.md        # Moved from root
├── RUST_WORKSPACE.md               # Moved from root
├── RUST_MODERNIZATION_SUMMARY.md   # Moved from root
├── TEST_UPDATES_SUMMARY.md         # Moved from root
├── WARP.md                         # Moved from root
└── VALIDATION_ENFORCEMENT.md       # Updated paths
```

**Benefits:**
- All documentation in one location
- Cleaner root directory
- Easier to navigate

### 4. Consolidated Scripting Projects

**Before:**
```
src/
├── rules-compiler-shell/    # Old location
├── shell-scripts/           # Interim location
│   ├── bash/
│   └── zsh/
└── powershell-modules/      # Modern modules
    ├── Common/
    ├── RulesCompiler/
    └── AdGuardWebhook/
```

**After:**
```
src/
├── shell/                   # Consolidated shell scripts ✅
│   ├── README.md
│   ├── bash/
│   │   └── compile-rules.sh
│   └── zsh/
│       └── compile-rules.zsh
├── powershell/              # Consolidated PowerShell modules ✅
│   ├── README.md
│   ├── Common/
│   ├── RulesCompiler/
│   └── AdGuardWebhook/
└── adguard-api-powershell/  # Auto-generated API client (kept separate)
```

**Benefits:**
- Clear organization by language
- No duplication
- Easy to find all scripts of a type
- Follows multi-language monorepo standards

### 5. Added Standard GitHub Files

**New files:**
- `CONTRIBUTING.md` - Contribution guidelines (GitHub standard)
- `api/README.md` - API specification documentation
- `tools/README.md` - Tools directory documentation
- `src/shell/README.md` - Shell scripts documentation
- `src/powershell/README.md` - PowerShell modules documentation

## Directory Structure (Final)

```
ad-blocking/
├── api/                     # OpenAPI specifications (centralized) ✅
├── docs/                    # All documentation ✅
├── tools/                   # Utility and build scripts ✅
├── data/                    # Filter rules and data
├── src/                     # Source code
│   ├── shell/              # Shell scripts (consolidated) ✅
│   ├── powershell/         # PowerShell modules (consolidated) ✅
│   ├── adguard-api-dotnet/
│   ├── adguard-api-rust/
│   ├── adguard-api-typescript/
│   ├── adguard-api-powershell/  # Auto-generated API client
│   ├── adguard-validation/
│   ├── rules-compiler-dotnet/
│   ├── rules-compiler-python/
│   ├── rules-compiler-rust/
│   ├── rules-compiler-typescript/
│   └── linear/
├── .github/                # GitHub workflows and config
├── CONTRIBUTING.md         # Contribution guidelines ✅
├── README.md               # Project overview
├── SECURITY.md             # Security policy
├── LICENSE                 # GPL-3.0 license
├── build.sh                # Build script (Bash)
├── build.ps1               # Build script (PowerShell)
├── launcher.sh             # Interactive launcher (Bash)
└── launcher.ps1            # Interactive launcher (PowerShell)
```

## Updated References

All file references have been updated in:

1. **GitHub Actions Workflows:**
   - `.github/workflows/validation-compliance.yml`
   - `.github/workflows/build-scripts-tests.yml`

2. **Scripts:**
   - `launcher.sh`
   - `tools/test-modules.ps1`

3. **Documentation:**
   - `README.md`
   - `docs/VALIDATION_ENFORCEMENT.md`

## Deprecated Locations

The following directories are now deprecated and should not be used:

❌ `src/rules-compiler-shell/` → Use `src/shell/` instead  
❌ `src/shell-scripts/` → Use `src/shell/` instead  
❌ `src/powershell-modules/` → Use `src/powershell/` instead  
❌ `scripts/` (old location) → Use `tools/` instead

**Note:** These directories have been copied to new locations. Old directories can be removed after verification that all integrations work correctly.

## Migration Guide for Users

### For Shell Script Users

**Old command:**
```bash
./src/rules-compiler-shell/compile-rules.sh
```

**New command:**
```bash
./src/shell/bash/compile-rules.sh
```

### For PowerShell Users

**Old imports:**
```powershell
Import-Module ./src/powershell-modules/RulesCompiler/RulesCompiler.psd1
```

**New imports:**
```powershell
Import-Module ./src/powershell/RulesCompiler/RulesCompiler.psd1
```

### For SDK Developers

**Old OpenAPI location:**
```bash
cd src/adguard-api-dotnet/api
curl -o openapi.json https://api.adguard-dns.io/swagger/openapi.json
```

**New centralized location:**
```bash
cd api
curl -o openapi.json https://api.adguard-dns.io/swagger/openapi.json
```

## Standards Applied

### GitHub Repository Standards (2024)

✅ Clean root directory with only essential files  
✅ Centralized `docs/` for documentation  
✅ Centralized `tools/` for utilities  
✅ `CONTRIBUTING.md` for contribution guidelines  
✅ Clear separation of source code, docs, tools  

### Multi-language Monorepo Standards

✅ Organization by language/technology in `src/`  
✅ Clear naming conventions  
✅ No duplication of resources  
✅ Consolidated scripting projects  
✅ Single source of truth for shared resources  

## Benefits

1. **Easier Navigation**: Logical organization makes it easy to find files
2. **Reduced Duplication**: Single source of truth for shared resources
3. **Better Onboarding**: New contributors can quickly understand structure
4. **Standards Compliance**: Follows industry best practices
5. **Cleaner Root**: Only essential files in root directory
6. **Improved Documentation**: All docs in one place with clear README files

## Next Steps

1. ✅ Centralize API specifications
2. ✅ Consolidate tools directory
3. ✅ Organize documentation
4. ✅ Consolidate scripting projects
5. ✅ Update all references
6. ✅ Create comprehensive documentation
7. ⏳ Remove deprecated directories (after verification)
8. ⏳ Update CI/CD to verify new structure
9. ⏳ Announce changes to users

## Questions or Issues?

See:
- [CONTRIBUTING.md](../CONTRIBUTING.md) for contribution guidelines
- [README.md](../README.md) for general documentation
- Open an issue if you encounter problems with the new structure
