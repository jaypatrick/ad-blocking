# Ad-Blocking Repository

Various rules lists, scripts, and other tools used to block nuisances from networks.

## Project Structure

```
ad-blocking/
├── .github/                    # GitHub configuration
│   ├── workflows/              # CI/CD pipelines
│   └── ISSUE_TEMPLATE/         # Issue templates
├── docs/                       # Documentation
│   ├── api/                    # API client documentation
│   └── guides/                 # Usage guides
├── rules/                      # Filter rules
│   ├── adguard_user_filter.txt # Main filter list
│   ├── Api/                    # Rules API utilities
│   └── Config/                 # Rule compilation config
├── scripts/                    # Automation scripts
│   └── powershell/             # PowerShell modules
├── src/                        # Source code
│   ├── api-client/             # AdGuard DNS API C# client
│   ├── rules-compiler-typescript/ # TypeScript rules compiler
│   ├── rules-compiler-dotnet/  # .NET rules compiler
│   ├── rules-compiler-python/  # Python rules compiler
│   ├── rules-compiler-rust/    # Rust rules compiler
│   ├── webhook-app/            # C# webhook application
│   └── website/                # Gatsby portfolio website
├── README.md                   # This file
├── SECURITY.md                 # Security policy
└── LICENSE                     # License file
```

## Quick Start

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) - For C# components
- [Node.js 18+](https://nodejs.org/) - For TypeScript compiler and Gatsby website
- [PowerShell 7+](https://github.com/PowerShell/PowerShell) - For automation scripts
- [AdGuard HostList Compiler](https://github.com/AdguardTeam/HostlistCompiler) - For filter compilation

### Warp Environment

This repository includes a pre-configured Warp environment for development and AI-assisted workflows.

**Environment Details:**
- **Docker Image:** `jaysonknight/warp-env:ad-blocking`
  - .NET 8.0 SDK on Ubuntu Jammy
  - Node.js 20.x LTS
  - PowerShell 7
  - Git
- **Environment ID:** `Egji4sZU4TNIOwNasFU73A`

**Setup Commands (automated):**
```bash
cd ad-blocking/src/rules-compiler-typescript && npm install
cd ad-blocking/src/website && npm install
cd ad-blocking/src/webhook-app && dotnet restore
cd ad-blocking/src/api-client && dotnet restore
```

**Using with Integrations:**
```bash
# Create a Slack integration
warp integration create slack --environment Egji4sZU4TNIOwNasFU73A

# Create a Linear integration
warp integration create linear --environment Egji4sZU4TNIOwNasFU73A
```

For full repository access (opening PRs, pushing changes), authorize the Warp GitHub app.

### Configuration

1. **Environment Variables**: Create a `.env` file or set environment variables:
   ```bash
   ADGUARD_WEBHOOK_URL=https://linkip.adguard-dns.com/linkip/YOUR_DEVICE_ID/YOUR_AUTH_TOKEN
   SECRET_KEY=your-secret-key-here
   ```

   See `.env.example` files in the project for templates.

2. **Install Dependencies**:
   ```bash
   # Install compiler dependencies
   cd src/rules-compiler-typescript
   npm install

   # Install website dependencies (optional)
   cd ../website
   npm install

   # Restore .NET packages
   cd ../webhook-app
   dotnet restore

   # Restore API client packages
   cd ../api-client
   dotnet restore
   ```

### Usage

#### Compile Filter Rules
```bash
# Using TypeScript
cd src/rules-compiler-typescript
npm run build
npm run compile

# Using PowerShell
cd scripts/powershell
./RulesCompiler-Harness.ps1
```

#### Run C# Webhook Application
```bash
cd src/webhook-app
dotnet run
```

#### Trigger Webhook via PowerShell
```powershell
cd scripts/powershell
./Webhook-Harness.ps1
```

#### Use the AdGuard API Client
See the [API Client Usage Guide](docs/guides/api-client-usage.md) and [Examples](docs/guides/api-client-examples.md).

## Components

### Filter Rules (`rules/`)
The main AdGuard filter list for blocking ads, trackers, and malware. The rules list [can be found here](rules/adguard_user_filter.txt).

### Rules Compiler - TypeScript (`src/rules-compiler-typescript/`)
TypeScript-based compiler using [@adguard/hostlist-compiler](https://github.com/AdguardTeam/HostlistCompiler) to compile and transform filter rules. Includes Deno support and optional Rust CLI frontend.

### Webhook App (`src/webhook-app/`)
.NET 8.0 application for triggering AdGuard DNS webhooks with built-in rate limiting and resilience patterns.

### API Client (`src/api-client/`)
C# SDK for the [AdGuard DNS API v1.11](https://api.adguard-dns.io/static/swagger/swagger.json). Auto-generated from OpenAPI specification with full async support and Polly resilience.

### Website (`src/website/`)
Gatsby-based portfolio website deployed to GitHub Pages.

## Project Purpose

The internet is full of nuisances, and this repository helps eradicate them from networks:
1. Ads
2. Trackers
3. Malware

### How do I safeguard my network?

There are plenty of great apps that will help, but there is no one-size-fits-all solution, especially for:
- Smart devices (Echo, HomePod, etc.)
- Smart TVs
- IoT devices without installation capability

### Test Your Ad Blocking

- [AdBlock Tester](https://adblock-tester.com/)
- [AdGuard Tester](https://d3ward.github.io/toolz/adblock.html)

## Documentation

- [API Client README](src/api-client/README.md) - API client overview
- [API Client Usage Guide](docs/guides/api-client-usage.md) - Detailed usage instructions
- [API Client Examples](docs/guides/api-client-examples.md) - Code examples
- [API Reference](docs/api/) - Full API documentation

## Contributing

Please see [SECURITY.md](SECURITY.md) for security policy and vulnerability reporting.

## License

See [LICENSE](LICENSE) for details.
