# Environment Variable Migration Guide

## Overview

The ad-blocking repository has standardized on a single, universal environment variable format that works consistently across all programming languages (TypeScript, Rust, C#, Python, PowerShell).

## What Changed?

### New Standard Format (Recommended)

One format that works everywhere:

```bash
ADGUARD_API_KEY=your-api-key-here
ADGUARD_API_BASE_URL=https://api.adguard-dns.io
ADGUARD_WEBHOOK_URL=https://your-webhook-url
ADGUARD_LINEAR_API_KEY=lin_api_xxxxx
ADGUARD_LINEAR_TEAM_ID=team_xxxxx
ADGUARD_LINEAR_PROJECT_NAME=My Project
DEBUG=1
```

### Legacy Formats (Still Supported)

For backward compatibility, these legacy formats still work:

```bash
# Legacy .NET hierarchical format
ADGUARD_AdGuard__ApiKey=your-api-key-here
ADGUARD_AdGuard__BaseUrl=https://api.adguard-dns.io

# Legacy Rust format
ADGUARD_API_TOKEN=your-api-key-here
ADGUARD_API_URL=https://api.adguard-dns.io

# Legacy Linear format
LINEAR_API_KEY=lin_api_xxxxx
LINEAR_TEAM_ID=team_xxxxx
LINEAR_PROJECT_NAME=My Project
```

## Why Standardize?

### Before (Inconsistent)
Different variable names required for different languages:
- TypeScript: `ADGUARD_API_KEY`
- Rust: `ADGUARD_API_TOKEN` 
- .NET: `ADGUARD_AdGuard__ApiKey`
- Linear: `LINEAR_API_KEY`

### After (Consistent)
One variable name works for all languages:
- TypeScript: `ADGUARD_API_KEY` ✅
- Rust: `ADGUARD_API_KEY` ✅
- .NET: `ADGUARD_API_KEY` ✅
- Linear: `ADGUARD_LINEAR_API_KEY` ✅

## Migration Steps

### 1. Update Your `.env` File

**Before:**
```bash
ADGUARD_AdGuard__ApiKey=abc123
LINEAR_API_KEY=lin_api_xyz
```

**After:**
```bash
ADGUARD_API_KEY=abc123
ADGUARD_LINEAR_API_KEY=lin_api_xyz
```

### 2. Update Shell Scripts and CI/CD

**Before:**
```bash
export ADGUARD_AdGuard__ApiKey="your-key"
export LINEAR_API_KEY="lin_api_key"
```

**After:**
```bash
export ADGUARD_API_KEY="your-key"
export ADGUARD_LINEAR_API_KEY="lin_api_key"
```

### 3. Update Docker Compose / Kubernetes

**Before:**
```yaml
environment:
  - ADGUARD_AdGuard__ApiKey=${API_KEY}
  - LINEAR_API_KEY=${LINEAR_KEY}
```

**After:**
```yaml
environment:
  - ADGUARD_API_KEY=${API_KEY}
  - ADGUARD_LINEAR_API_KEY=${LINEAR_KEY}
```

### 4. Update GitHub Actions Secrets

No changes needed - just reference them with new names:

```yaml
env:
  ADGUARD_API_KEY: ${{ secrets.ADGUARD_API_KEY }}
  ADGUARD_LINEAR_API_KEY: ${{ secrets.LINEAR_API_KEY }}
```

## Backward Compatibility

**Don't worry!** All code supports both old and new formats. The system will:

1. Try the new standardized format first
2. Fall back to legacy formats if not found
3. Work seamlessly during migration

Priority order for API keys:
1. `ADGUARD_API_KEY` (new standard - highest priority)
2. `ADGUARD_AdGuard__ApiKey` (legacy .NET format)
3. `ADGUARD_API_TOKEN` (legacy Rust format)

Priority order for Linear:
1. `ADGUARD_LINEAR_API_KEY` (new standard - highest priority)
2. `LINEAR_API_KEY` (legacy format)

## Testing Your Migration

### Quick Test

```bash
# Set the new format
export ADGUARD_API_KEY="test-key"

# All these commands should work
cd src/adguard-api-typescript && deno task start
cd src/adguard-api-rust && cargo run
cd src/adguard-api-dotnet/src/AdGuard.ConsoleUI && dotnet run
```

### Verify Environment Variables

```bash
# Check what's set
env | grep ADGUARD

# Should show new format:
# ADGUARD_API_KEY=...
# ADGUARD_LINEAR_API_KEY=...
```

## Language-Specific Notes

### TypeScript (Deno)
- Reads `ADGUARD_API_KEY` directly
- Falls back to `ADGUARD_AdGuard__ApiKey`, then `ADGUARD_API_TOKEN`
- No code changes needed

### Rust
- Reads `ADGUARD_API_KEY` directly
- Falls back to `ADGUARD_AdGuard__ApiKey`, then `ADGUARD_API_TOKEN`
- Configuration file in `~/.config/adguard-api-cli/config.toml`

### C# / .NET
- Reads `ADGUARD_API_KEY` and maps it to `AdGuard:ApiKey`
- Still supports `ADGUARD_AdGuard__ApiKey` (hierarchical format)
- Uses standard .NET configuration system

### PowerShell
- Reads `ADGUARD_API_KEY` from `$env:ADGUARD_API_KEY`
- Works identically across Windows, Linux, macOS

## Benefits

✅ **Simplicity**: One format to remember  
✅ **Consistency**: Same variables across all languages  
✅ **Clarity**: Clear naming convention (ADGUARD_ prefix)  
✅ **Compatibility**: Works with existing 12-factor app tooling  
✅ **Safety**: Backward compatibility ensures smooth migration  

## Support

If you encounter issues during migration:

1. Check the [Environment Variables Reference](./ENVIRONMENT_VARIABLES.md)
2. Verify your `.env.example` matches the template
3. Check [troubleshooting guide](./guides/troubleshooting-guide.md)
4. Open an issue on GitHub

## Timeline

- **Current**: Both old and new formats supported
- **Recommended**: Migrate to new format when convenient
- **Future**: Legacy formats may be deprecated in a future major version (with advance notice)

---

**Last Updated**: 2025-12-27  
**Status**: Active - migration recommended but not required
