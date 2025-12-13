# WARP.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

Project scope
- This repo houses multiple sub-projects: a TypeScript filter compiler, an AdGuard DNS API client (C#), a small console UI for the API client, a Gatsby website, and a few automation scripts.
- CI pipelines (GitHub Actions) validate the .NET API client solution, the TypeScript compiler, and the website using Node 20 and .NET 8. Keep local commands aligned with the workflows below.

Common commands (build, lint, test)
TypeScript – rules compiler (src/rules-compiler-typescript)
- Install deps: cd src/rules-compiler-typescript && npm ci
- Type-check only: npx tsc --noEmit
- Lint: npx eslint .
- Unit tests (Jest): npm test
- Coverage: npm run test:coverage
- Build: npm run build
- Compile rules: npm run compile
  Notes
  - invoke-compiler.ts reads compiler-config.json in the same folder and writes adguard_user_filter.txt beside it. The canonical list in rules/adguard_user_filter.txt is tracked under rules/.

.NET – API client + console UI (src/adguard-api-client)
- Restore/build/test: cd src/adguard-api-client; dotnet restore AdGuard.ApiClient.sln; dotnet build AdGuard.ApiClient.sln; dotnet test AdGuard.ApiClient.sln
- Run the console UI: dotnet run --project src/AdGuard.ConsoleUI/AdGuard.ConsoleUI.csproj
  Notes
  - The client targets net8.0 and includes helpers for configuration and Polly-based retry policies (see Helpers/ConfigurationHelper.cs and Helpers/RetryPolicyHelper.cs).

Website (Gatsby) – src/website
- Install deps: cd src/website && npm ci
- Develop: npm run develop
- Build: npm run build
- Serve local build: npm run serve

PowerShell scripts
- Static analysis (same as CI): Invoke-ScriptAnalyzer -Path scripts/powershell -Recurse

Running a single test
- TypeScript (Jest)
  - By file: cd src/rules-compiler-typescript && npx jest cli.test.ts
  - By test name: npx jest -t "should write rules to a file"
- .NET (xUnit under src/adguard-api-client)
  - By class pattern: cd src/adguard-api-client && dotnet test AdGuard.ApiClient.sln --filter "FullyQualifiedName~DevicesApiTests"
  - By method pattern: dotnet test AdGuard.ApiClient.sln --filter "Name~GetAccountLimits"

Environment and secrets used by code
- The API client expects an AdGuard API credential. The console UI can prompt for a key or read it from configuration (AdGuard:ApiKey).

High-level architecture and structure
- Filter rules (rules/)
  - rules/adguard_user_filter.txt is the tracked output list consumed by AdGuard DNS.
  - rules/Api/cli.ts provides a minimal CLI that compiles using @adguard/hostlist-compiler and writes to rules/. CI type-checks this file using the filter-compiler TypeScript toolchain.
- Filter compiler (src/filter-compiler/)
  - TypeScript wrapper around @adguard/hostlist-compiler. invoke-compiler.ts loads compiler-config.json, compiles sources, and writes adguard_user_filter.txt. Jest tests cover config parsing and output writing.
  - eslint.config.mjs configures JS/TS linting via the flat config.
- API client (src/adguard-api-client/)
  - Auto-generated C# SDK for AdGuard DNS API v1.11 (see api/openapi.yaml and README.md). Targets net8.0 in AdGuard.ApiClient.csproj; uses Newtonsoft.Json and JsonSubTypes.
  - Helpers/ConfigurationHelper.cs provides fluent auth + timeouts + user agent, and Helpers/RetryPolicyHelper.cs adds Polly-based retry policies for 408/429/5xx.
  - Console UI (src/AdGuard.ConsoleUI/) is a Spectre.Console menu-driven wrapper over the SDK with a small ApiClientFactory to configure the SDK from settings or an interactive prompt.
- Scripts (scripts/)
  - scripts/linear: Node-based tool to import the repo's documentation into Linear (build with tsc; run node dist/linear-import.js). Reads .env for LINEAR_API_KEY, etc.
  - scripts/powershell: PowerShell module scaffolding and tests; CI runs PSScriptAnalyzer against the folder.
- Website (src/website/)
  - Gatsby portfolio starter used as a simple static site. CI builds and deploys to GitHub Pages.

Notes pulled from existing docs
- Root README lists prerequisites: .NET 8, Node 18+, and PowerShell 7+. It also documents the typical steps to compile filters with the TypeScript tool.
- The API client README documents OpenAPI version (1.11) and packaging guidance if you later decide to publish a NuGet package.

Alignment with CI
- .github/workflows/dotnet.yml builds and tests the API client solution with .NET 8.
- .github/workflows/typescript.yml uses Node 20, performs tsc --noEmit and eslint for the rules-compiler-typescript, and type-checks rules/Api/cli.ts via the same toolchain.
- .github/workflows/gatsby.yml builds src/website and deploys to GitHub Pages.
