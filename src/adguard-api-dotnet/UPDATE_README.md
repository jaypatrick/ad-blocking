# AdGuard API Client - Update and Maintenance Guide

This guide explains how to update the AdGuard DNS API client from the latest OpenAPI specification.

## Quick Start

### Complete Update Workflow (Recommended)

Download the latest spec, regenerate the client, build, and test in one command:

```bash
# Linux/macOS
./update-api-client.sh

# Windows (PowerShell)
.\Update-ApiClient.ps1
```

### Download Spec Only

If you just want to download the latest OpenAPI specification:

```bash
# Quick download
./quick-download.sh

# Or download with retry attempts to multiple URLs
./download-openapi-spec.sh
```

### Regenerate Client Only

If you already have an updated spec and just want to regenerate the client:

```bash
# Linux/macOS
./regenerate-client.sh

# Windows (PowerShell)
.\Regenerate-Client.ps1
```

## OpenAPI Specification

**Public URL**: https://api.adguard-dns.io/swagger/openapi.json

The specification is stored locally at:
- `api/openapi.json` - JSON format (primary)
- `api/openapi.yaml` - YAML format (optional, for readability)

## Available Scripts

| Script | Description | Platform |
|--------|-------------|----------|
| `update-api-client.sh` | **Complete workflow**: Download spec, regenerate, build, test | Linux/macOS |
| `Update-ApiClient.ps1` | **Complete workflow**: Download spec, regenerate, build, test | Windows/PowerShell |
| `quick-download.sh` | Quick download of OpenAPI spec only | Linux/macOS |
| `download-openapi-spec.sh` | Download spec with retry from multiple URLs | Linux/macOS |
| `regenerate-client.sh` | Regenerate C# client from existing spec | Linux/macOS |
| `Regenerate-Client.ps1` | Regenerate C# client from existing spec | Windows/PowerShell |

## Step-by-Step Manual Process

### 1. Download the Latest OpenAPI Specification

```bash
curl -o api/openapi.json https://api.adguard-dns.io/swagger/openapi.json
```

Optional: Convert JSON to YAML for better readability:

```bash
# Requires yq: pip install yq
yq eval -P api/openapi.json > api/openapi.yaml
```

### 2. Validate the Specification

```bash
# Install Spectral if needed
npm install -g @stoplight/spectral-cli

# Validate
spectral lint api/openapi.json
```

### 3. Review Changes

```bash
git diff api/openapi.json
```

### 4. Regenerate the API Client

```bash
# Install OpenAPI Generator if needed
npm install -g @openapitools/openapi-generator-cli

# Generate C# client
openapi-generator-cli generate \
    -i api/openapi.json \
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

### 5. Build the Solution

```bash
dotnet restore AdGuard.ApiClient.slnx
dotnet build AdGuard.ApiClient.slnx --no-restore
```

### 6. Run Tests

```bash
dotnet test AdGuard.ApiClient.slnx --no-build
```

### 7. Test ConsoleUI

```bash
cd src/AdGuard.ConsoleUI
dotnet run
```

## Prerequisites

### Required Tools

- **.NET 10 SDK** - For building the solution
  ```bash
  # Check version
  dotnet --version
  ```

- **OpenAPI Generator CLI** - For generating the client
  ```bash
  npm install -g @openapitools/openapi-generator-cli
  ```

### Optional Tools

- **yq** - For converting JSON to YAML
  ```bash
  pip install yq
  ```

- **Spectral** - For OpenAPI spec validation
  ```bash
  npm install -g @stoplight/spectral-cli
  ```

- **jq** - For JSON validation and processing
  ```bash
  # Ubuntu/Debian
  sudo apt-get install jq
  
  # macOS
  brew install jq
  ```

## Troubleshooting

### Cannot Download Spec

**Error**: `Could not resolve host: api.adguard-dns.io`

**Solutions**:
1. Check internet connectivity
2. Check firewall/proxy settings
3. Try downloading from a browser first
4. Use a VPN if the domain is blocked in your region

### Build Errors After Regeneration

**Common causes**:
1. Breaking changes in the API
2. Custom modifications overwritten
3. Dependency version mismatches

**Solutions**:
1. Review the spec diff: `git diff api/openapi.json`
2. Check backup files in `.backup-*` directories
3. Manually merge custom modifications from `Helpers/` directory
4. Update dependencies: `dotnet restore`

### Test Failures

**Common causes**:
1. API endpoints changed
2. Response schemas changed
3. Authentication requirements changed

**Solutions**:
1. Review failing test details
2. Update test expectations to match new API behavior
3. Add new tests for new endpoints/models

### OpenAPI Generator Errors

**Error**: `openapi-generator-cli: command not found`

**Solution**:
```bash
npm install -g @openapitools/openapi-generator-cli
```

**Error**: `Unsupported generator version`

**Solution**:
```bash
npm update -g @openapitools/openapi-generator-cli
```

## Configuration

### OpenAPI Generator Settings

Current generation settings (in `regenerate-client.sh` and `Regenerate-Client.ps1`):

| Setting | Value | Description |
|---------|-------|-------------|
| `targetFramework` | `net10.0` | Target .NET version |
| `packageName` | `AdGuard.ApiClient` | NuGet package name |
| `packageVersion` | `1.0.0` | Package version |
| `jsonLibrary` | `Newtonsoft.Json` | JSON serialization library |
| `validatable` | `false` | Disable model validation |
| `netCoreProjectFile` | `true` | Use .NET Core project format |
| `nullableReferenceTypes` | `true` | Enable nullable reference types |

### Customizing Generation

To exclude files from regeneration, add them to `.openapi-generator-ignore`:

```
# Example: Preserve custom helper classes
src/AdGuard.ApiClient/Helpers/*

# Example: Preserve custom configurations
src/AdGuard.ApiClient/Configuration/*
```

## Project Structure

```
src/adguard-api-dotnet/
├── api/
│   ├── openapi.json           # Primary OpenAPI spec (JSON)
│   ├── openapi.yaml           # Optional OpenAPI spec (YAML, for readability)
│   └── README.md              # API spec documentation
├── src/
│   ├── AdGuard.ApiClient/     # Generated API client (auto-generated)
│   ├── AdGuard.ConsoleUI/     # Console application (custom)
│   └── AdGuard.ApiClient.Test/ # Tests (auto-generated + custom)
├── docs/                      # API documentation (auto-generated)
├── update-api-client.sh       # Complete update workflow (Bash)
├── Update-ApiClient.ps1       # Complete update workflow (PowerShell)
├── quick-download.sh          # Quick spec download (Bash)
├── download-openapi-spec.sh   # Spec download with retries (Bash)
├── regenerate-client.sh       # Regenerate client (Bash)
├── Regenerate-Client.ps1      # Regenerate client (PowerShell)
├── OPENAPI_UPDATE_GUIDE.md    # Comprehensive update guide
└── README.md                  # Main API client README
```

## Best Practices

1. **Always backup** before regenerating:
   - Scripts automatically create backups with timestamps
   - Backups are stored in `.backup-YYYYMMDD_HHMMSS/` directories

2. **Review changes** after downloading new spec:
   ```bash
   git diff api/openapi.json
   ```

3. **Validate spec** before regenerating:
   ```bash
   spectral lint api/openapi.json
   ```

4. **Test thoroughly** after regeneration:
   - Run unit tests: `dotnet test`
   - Test ConsoleUI application
   - Test with real API endpoints (if possible)

5. **Preserve customizations**:
   - Keep custom code in `Helpers/` directories
   - Use `.openapi-generator-ignore` to protect files
   - Document any manual modifications

6. **Version control**:
   - Commit the OpenAPI spec: `api/openapi.json`
   - Commit generated code changes
   - Tag releases: `git tag v1.2.0`

## Documentation

- **[OPENAPI_UPDATE_GUIDE.md](OPENAPI_UPDATE_GUIDE.md)** - Comprehensive guide with all details
- **[api/README.md](api/README.md)** - OpenAPI specification documentation
- **[src/AdGuard.ApiClient/README.md](src/AdGuard.ApiClient/README.md)** - Generated client documentation
- **[docs/guides/api-client-usage.md](../../docs/guides/api-client-usage.md)** - Usage examples

## Support

- **AdGuard DNS API**: https://adguard-dns.io/kb/private-dns/api/overview/
- **OpenAPI Generator**: https://openapi-generator.tech/docs/generators/csharp
- **Repository Issues**: https://github.com/jaypatrick/ad-blocking/issues

## Version History

| Date | API Version | Generator Version | Changes |
|------|-------------|-------------------|---------|
| 2025-12-14 | 1.11 | 7.16.0 | Added automated update scripts and documentation |

## Future Improvements

See `src/adguard-api-dotnet/.github/upgrades/tasks.md` for planned modernization tasks:

- [ ] Migrate to System.Text.Json (SCENARIO-004)
- [ ] Update OpenAPI Generator configuration (SCENARIO-005)
- [ ] Implement CI/CD automation for spec updates
- [ ] Add change detection and notification system
