# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This repository houses multiple sub-projects for ad-blocking, network protection, and AdGuard DNS management:
- **Filter Compiler** (TypeScript) - Compiles filter rules using @adguard/hostlist-compiler
- **API Client** (C#/.NET 8) - Auto-generated SDK for AdGuard DNS API v1.11
- **Console UI** (C#/.NET 8) - Spectre.Console menu-driven wrapper for the API client
- **Webhook App** (C#/.NET 8) - Triggers AdGuard DNS webhooks with rate limiting
- **Website** (Gatsby) - Static portfolio site deployed to GitHub Pages
- **PowerShell Scripts** - Automation and webhook modules

## Common Commands

### TypeScript Filter Compiler (`src/filter-compiler/`)
```bash
cd src/filter-compiler
npm ci                    # Install dependencies
npm run build             # Build TypeScript
npm test                  # Run Jest tests
npm run test:coverage     # Run tests with coverage
npx tsc --noEmit          # Type-check only
npx eslint .              # Lint
npm run compile           # Compile filter rules
```

### .NET API Client + Console UI (`src/api-client/`)
```bash
cd src/api-client
dotnet restore AdGuard.ApiClient.sln
dotnet build AdGuard.ApiClient.sln
dotnet test AdGuard.ApiClient.sln
dotnet run --project src/AdGuard.ConsoleUI/AdGuard.ConsoleUI.csproj
```

### .NET Webhook App (`src/webhook-app/`)
```bash
cd src/webhook-app
dotnet restore Code.sln
dotnet build Code.sln
dotnet test Code.sln
dotnet run    # Requires ADGUARD_WEBHOOK_URL env var
```

### Gatsby Website (`src/website/`)
```bash
cd src/website
npm ci
npm run develop    # Dev server
npm run build      # Production build
npm run serve      # Serve local build
```

### PowerShell Scripts
```powershell
Invoke-ScriptAnalyzer -Path scripts/powershell -Recurse
```

## Running Individual Tests

### TypeScript (Jest)
```bash
cd src/filter-compiler
npx jest invoke-compiler.test.ts              # By file
npx jest -t "should write rules to a file"    # By test name
```

### .NET (xUnit)
```bash
cd src/api-client
dotnet test AdGuard.ApiClient.sln --filter "FullyQualifiedName~DevicesApiTests"   # By class
dotnet test AdGuard.ApiClient.sln --filter "Name~GetAccountLimits"                # By method
```

## Architecture

### Filter Rules (`rules/`)
- `rules/adguard_user_filter.txt` - Main tracked filter list consumed by AdGuard DNS
- `rules/Api/cli.ts` - Minimal CLI for compilation
- `rules/Config/compiler-config.json` - Compiler configuration

### Filter Compiler (`src/filter-compiler/`)
- TypeScript wrapper around @adguard/hostlist-compiler
- `invoke-compiler.ts` loads config and writes output
- ESLint flat config in `eslint.config.mjs`

### API Client (`src/api-client/`)
- Auto-generated from `api/openapi.yaml` (AdGuard DNS API v1.11)
- `Helpers/ConfigurationHelper.cs` - Fluent auth, timeouts, user agent
- `Helpers/RetryPolicyHelper.cs` - Polly-based retry for 408/429/5xx
- Uses Newtonsoft.Json and JsonSubTypes

### Console UI (`src/AdGuard.ConsoleUI/`)
- Spectre.Console menu-driven interface
- `ApiClientFactory` configures SDK from settings or interactive prompt

### Webhook App (`src/webhook-app/`)
- `Program.cs` - Main entry, wires logging + configuration
- `Infrastructure/ClientSideRateLimitedHandler.cs` - Rate limiting via FixedWindowRateLimiter
- `Extensions/HttpResponseMessage.cs` - Structured logging helpers

## Environment Variables

- `ADGUARD_WEBHOOK_URL` - Required by webhook app (format: `https://linkip.adguard-dns.com/linkip/<DEVICE_ID>/<AUTH_TOKEN>`)
- `AdGuard:ApiKey` - API credential for console UI (can also prompt interactively)
- `LINEAR_API_KEY` - For Linear import scripts (`scripts/linear/`)

## CI/CD Alignment

GitHub Actions workflows validate:
- `.github/workflows/dotnet.yml` - Builds/tests both .NET solutions with .NET 8
- `.github/workflows/typescript.yml` - Node 20, tsc --noEmit, eslint for filter-compiler
- `.github/workflows/gatsby.yml` - Builds website and deploys to GitHub Pages
- `.github/workflows/powershell.yml` - PSScriptAnalyzer on PowerShell scripts

## Prerequisites

- .NET 8.0 SDK
- Node.js 18+ (CI uses Node 20)
- PowerShell 7+ (for scripts)
