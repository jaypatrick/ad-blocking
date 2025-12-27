# AdGuard DNS API OpenAPI Specification

## Current Version

**API Version**: 1.11 (as of the last update)

This directory contains the centralized OpenAPI 3.0 specification for the AdGuard DNS API, used by all SDK implementations in this repository.

## Public URL

The AdGuard DNS API OpenAPI specification is publicly available at:

**https://api.adguard-dns.io/swagger/openapi.json**

## About the Specification

The `openapi.json` file (primary) and optional `openapi.yaml` file define:
- All API endpoints (/oapi/v1/*)
- Request/response schemas
- Authentication methods (API Key and Bearer Token)
- Error responses
- Data models

## Centralized Location

This centralized `api/` directory serves as the single source of truth for the OpenAPI specification across all SDK implementations:

- **C# SDK**: `src/adguard-api-dotnet/` → references `../../api/openapi.json`
- **TypeScript SDK**: `src/adguard-api-typescript/` → references `../../api/openapi.json`
- **Rust SDK**: `src/adguard-api-rust/` → references `../../api/openapi.json`
- **PowerShell SDK**: `src/adguard-api-powershell/` → references `../../api/openapi.json`

## Downloading the Latest Specification

### Automated Download

Use the provided download script from the repository root:

```bash
./tools/download-openapi-spec.sh
```

Or use the update scripts in individual SDK directories.

### Manual Download

Download directly from the public URL:

```bash
# Download JSON format (primary)
curl -o api/openapi.json https://api.adguard-dns.io/swagger/openapi.json

# Convert to YAML (optional, requires yq)
yq eval -P api/openapi.json > api/openapi.yaml
```

## Manual Update Process

If you obtain an updated OpenAPI specification:

1. **Backup the current spec**:
   ```bash
   cp api/openapi.json api/openapi.json.backup
   ```

2. **Replace with new spec**:
   ```bash
   cp /path/to/new/spec.json api/openapi.json
   # or if you have YAML, convert it
   yq eval -o=json /path/to/new/spec.yaml > api/openapi.json
   ```

3. **Validate the spec** (optional but recommended):
   ```bash
   npm install -g @stoplight/spectral-cli
   spectral lint api/openapi.json
   ```

4. **Review changes**:
   ```bash
   git diff api/openapi.json
   ```

5. **Regenerate all API clients**:
   ```bash
   # C# SDK
   cd src/adguard-api-dotnet && ./regenerate-client.sh
   
   # TypeScript SDK
   cd src/adguard-api-typescript && deno task generate-types
   
   # Rust SDK
   cd src/adguard-api-rust && ./regenerate-client.sh
   
   # PowerShell SDK
   cd src/adguard-api-powershell && pwsh Generate-PowerShellClient.ps1
   ```

## Specification Details

### Base URL
```
https://api.adguard-dns.io
```

### Authentication

The API supports two authentication methods:

1. **API Key** (ApiKey)
   - Header: `Authorization`
   - Format: `ApiKey your-api-key-here`

2. **Bearer Token** (AuthToken)
   - Header: `Authorization`
   - Format: `Bearer your-access-token`

### API Endpoints

The specification includes endpoints for:

- **Account Management** (`/oapi/v1/account/*`)
  - Get account limits

- **Authentication** (`/oapi/v1/oauth_token`)
  - Generate access and refresh tokens

- **Devices** (`/oapi/v1/devices/*`)
  - Create, read, update, delete devices
  - List devices

- **DNS Servers** (`/oapi/v1/dns_servers/*`)
  - Create, read, update, delete DNS server profiles
  - List DNS servers

- **Dedicated IP Addresses** (`/oapi/v1/dedicated_addresses/*`)
  - Allocate dedicated IPv4 addresses
  - List allocated addresses

- **Filter Lists** (`/oapi/v1/filter_lists`)
  - Get available filter lists

- **Query Log** (`/oapi/v1/query_log`)
  - Get query log
  - Clear query log

- **Statistics** (`/oapi/v1/stats/*`)
  - Get time-based statistics

- **Web Services** (`/oapi/v1/web_services`)
  - List web services for blocking

## Version History

| Date | Version | Notes |
|------|---------|-------|
| 2024-12-27 | 1.11 | Centralized specification location |

## Related Documentation

- [API Client Usage Guide](../docs/guides/api-client-usage.md) - How to use the generated clients
- [AdGuard DNS API Documentation](https://adguard-dns.io/kb/private-dns/api/overview/) - Official API docs

## Contributing

If you find a public URL where the OpenAPI specification can be downloaded, please:

1. Test the URL to ensure it returns valid OpenAPI/Swagger JSON or YAML
2. Add the URL to `tools/download-openapi-spec.sh` in the URLS array
3. Submit a pull request with your changes

## Support

For issues related to:
- **The OpenAPI specification itself**: Contact AdGuard support
- **API functionality**: Check AdGuard DNS documentation
- **Client generation**: See individual SDK documentation
