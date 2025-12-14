# OpenAPI Specification Update Guide

This guide explains how to update the AdGuard DNS API OpenAPI specification and regenerate the C# API client.

## Overview

The AdGuard API Client is auto-generated from an OpenAPI (formerly Swagger) specification using [OpenAPI Generator](https://openapi-generator.tech). This ensures the client stays in sync with the AdGuard DNS API.

## Current Version

- **API Version**: 1.11
- **Generator Version**: 7.16.0
- **Target Framework**: .NET 10
- **JSON Library**: Newtonsoft.Json

## OpenAPI Specification Location

The OpenAPI specification is stored at:
```
src/adguard-api-client/api/openapi.yaml
```

## Obtaining the Latest OpenAPI Spec

### Official Public URL

The AdGuard DNS API OpenAPI specification is publicly available at:

**https://api.adguard-dns.io/swagger/openapi.json**

You can download it directly:

```bash
# Download the latest spec
curl -O https://api.adguard-dns.io/swagger/openapi.json

# Or save it to the correct location
curl -o src/adguard-api-client/api/openapi.json \
     https://api.adguard-dns.io/swagger/openapi.json
```

### Alternative Methods

If the public URL is not accessible:

1. **Visit AdGuard DNS Documentation**
   - https://adguard-dns.io/kb/private-dns/api/overview/
   - Check for links to the API specification

2. **Check Your Account Dashboard**
   - If you have an AdGuard DNS account, check for developer/API documentation

3. **AdGuard GitHub Repositories**
   - Search https://github.com/AdguardTeam for API-related repositories

## Updating the OpenAPI Specification

The latest AdGuard DNS API OpenAPI specification is publicly available at:

**https://api.adguard-dns.io/swagger/openapi.json**

### Quick Update (Automated)

Use the complete update workflow script:

```bash
# Linux/macOS
cd src/adguard-api-client
./update-api-client.sh

# Windows (PowerShell)
cd src/adguard-api-client
.\Update-ApiClient.ps1
```

This script will:
1. Download the latest OpenAPI spec
2. Validate it
3. Regenerate the API client
4. Build and test the solution

### Manual Update

Once you have obtained the latest OpenAPI specification:

1. **Download the spec**:
   ```bash
   curl -o src/adguard-api-client/api/openapi.json \
        https://api.adguard-dns.io/swagger/openapi.json
   ```

2. **Backup the current spec**:
   ```bash
   cp src/adguard-api-client/api/openapi.yaml \
      src/adguard-api-client/api/openapi.yaml.backup
   ```

3. **Convert JSON to YAML** (optional, for readability):
   ```bash
   # Install yq if needed: pip install yq
   yq eval -P src/adguard-api-client/api/openapi.json > \
            src/adguard-api-client/api/openapi.yaml
   ```

4. **Verify the specification**:
   ```bash
   # Install OpenAPI tools if needed
   npm install -g @stoplight/spectral-cli

   # Validate the spec
   spectral lint src/adguard-api-client/api/openapi.yaml
   ```

5. **Review changes**:
   ```bash
   git diff src/adguard-api-client/api/openapi.yaml
   ```

## Regenerating the API Client

### Prerequisites

Install OpenAPI Generator CLI:

```bash
# Using npm (recommended)
npm install -g @openapitools/openapi-generator-cli

# Or using Docker (no installation required)
# See Docker method below
```

### Method 1: Using the Regeneration Script (Recommended)

#### Linux/macOS:
```bash
cd src/adguard-api-client
./regenerate-client.sh
```

#### Windows (PowerShell):
```powershell
cd src/adguard-api-client
.\Regenerate-Client.ps1
```

The script will:
1. Validate the OpenAPI specification exists
2. Create a backup of existing generated files
3. Generate new client code
4. Provide instructions for next steps

### Method 2: Using OpenAPI Generator Directly

```bash
cd src/adguard-api-client

# Generate C# client
openapi-generator-cli generate \
    -i api/openapi.yaml \
    -g csharp \
    -o . \
    --additional-properties=\
targetFramework=net10.0,\
packageName=AdGuard.ApiClient,\
packageVersion=1.0.0,\
jsonLibrary=Newtonsoft.Json,\
validatable=false,\
netCoreProjectFile=true,\
nullableReferenceTypes=true
```

### Method 3: Using Docker

```bash
cd src/adguard-api-client

docker run --rm \
    -v "${PWD}:/local" \
    openapitools/openapi-generator-cli generate \
    -i /local/api/openapi.yaml \
    -g csharp \
    -o /local \
    --additional-properties=\
targetFramework=net10.0,\
packageName=AdGuard.ApiClient,\
packageVersion=1.0.0,\
jsonLibrary=Newtonsoft.Json,\
validatable=false,\
netCoreProjectFile=true,\
nullableReferenceTypes=true
```

## Post-Generation Steps

### 1. Review Generated Code

Check the generated files in `src/AdGuard.ApiClient/`:

```bash
git status src/adguard-api-client/src/AdGuard.ApiClient/
git diff src/adguard-api-client/src/AdGuard.ApiClient/
```

### 2. Apply Custom Modifications

The repository contains customizations in the following locations:
- `src/AdGuard.ApiClient/Helpers/` - Custom helper classes
- `src/AdGuard.ConsoleUI/` - Console application (not auto-generated)

Ensure these customizations are preserved or updated as needed.

### 3. Build the Solution

```bash
cd src/adguard-api-client
dotnet restore AdGuard.ApiClient.slnx
dotnet build AdGuard.ApiClient.slnx --no-restore
```

Fix any compilation errors that may arise from API changes.

### 4. Run Tests

```bash
dotnet test AdGuard.ApiClient.slnx --no-build
```

Update tests as needed to accommodate API changes.

### 5. Update Documentation

If the API has changed, update:
- `docs/api/` - API endpoint documentation
- `docs/guides/api-client-usage.md` - Usage examples
- `src/adguard-api-client/README.md` - Client library README

### 6. Test ConsoleUI

The ConsoleUI application depends on the API client. Test it manually:

```bash
cd src/adguard-api-client/src/AdGuard.ConsoleUI
dotnet run
```

Verify all menu options and API operations work correctly.

## Troubleshooting

### OpenAPI Spec Not Found

**Error**: `OpenAPI specification not found at: api/openapi.yaml`

**Solution**: Ensure you have downloaded the latest specification and placed it in the correct location.

### Generator Version Mismatch

**Error**: `Unsupported generator version`

**Solution**: Update OpenAPI Generator CLI:
```bash
npm install -g @openapitools/openapi-generator-cli@latest
```

### Compilation Errors After Generation

**Common issues**:

1. **Breaking API changes**: Review the OpenAPI spec diff to understand what changed
2. **Missing dependencies**: Run `dotnet restore`
3. **Namespace conflicts**: Check for duplicate model names

### Test Failures

If tests fail after regeneration:

1. Review API endpoint changes in the spec
2. Update test expectations to match new API behavior
3. Add tests for new endpoints/models

## Configuration Options

### OpenAPI Generator Additional Properties

The generation scripts use these properties:

| Property | Value | Description |
|----------|-------|-------------|
| `targetFramework` | `net10.0` | Target .NET version |
| `packageName` | `AdGuard.ApiClient` | NuGet package name |
| `packageVersion` | `1.0.0` | Package version |
| `jsonLibrary` | `Newtonsoft.Json` | JSON serialization library |
| `validatable` | `false` | Disable model validation |
| `netCoreProjectFile` | `true` | Use .NET Core project format |
| `nullableReferenceTypes` | `true` | Enable nullable reference types |

### Customizing Generation

To modify generation settings, edit the script or use `.openapi-generator-ignore` to exclude files from regeneration.

Example `.openapi-generator-ignore`:
```
# Don't regenerate helper classes
src/AdGuard.ApiClient/Helpers/*

# Don't regenerate tests
src/AdGuard.ApiClient.Test/*
```

## Version History

| Date | API Version | Generator Version | Changes |
|------|-------------|-------------------|---------|
| 2025-12-14 | 1.11 | 7.16.0 | Initial documentation |

## Related Documentation

- [AdGuard DNS API Documentation](https://adguard-dns.io/kb/private-dns/api/overview/)
- [OpenAPI Generator Documentation](https://openapi-generator.tech/docs/generators/csharp)
- [API Client Usage Guide](../../docs/guides/api-client-usage.md)
- [ConsoleUI Architecture](src/AdGuard.ConsoleUI/ARCHITECTURE.md)

## Support

For issues with:
- **OpenAPI spec**: Contact AdGuard support
- **Generation process**: Check [OpenAPI Generator issues](https://github.com/OpenAPITools/openapi-generator/issues)
- **This repository**: Open an issue on GitHub

## Future Improvements

Planned enhancements for the update process:

1. **Automated Spec Fetching**: Script to automatically check for and download new spec versions
2. **System.Text.Json Migration**: Update to use modern .NET serialization (see `src/adguard-api-client/.github/upgrades/tasks.md`)
3. **CI/CD Integration**: Automated generation and testing in GitHub Actions
4. **Version Tracking**: Maintain changelog of API version updates
