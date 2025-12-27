# Ad-Blocking Repository - Linear Documentation

## Project Overview

A comprehensive, multi-component ad-blocking solution designed for network-level ad, tracker, and malware blocking. This repository supports IoT devices, smart TVs, and devices without installation capability using AdGuard DNS integration.

| Property | Value |
|----------|-------|
| **Current Version** | 4.3.2.42 |
| **License** | GPLv3 |
| **Last Updated** | November 26, 2025 |
| **Total Filter Rules** | 466 compiled rules |

---

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                     Ad-Blocking System                          │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌──────────────┐    ┌──────────────┐    ┌──────────────┐      │
│  │   Filter     │    │  API Client  │    │  Console UI  │      │
│  │  Compiler    │    │   (SDK)      │    │   (CLI)      │      │
│  │ (TypeScript) │    │    (C#)      │    │ (Spectre)    │      │
│  └──────────────┘    └──────────────┘    └──────────────┘      │
│         │                   │                   │               │
│         ▼                   ▼                   ▼               │
│  ┌──────────────┐    ┌──────────────┐    ┌──────────────┐      │
│  │ Filter Rules │    │   AdGuard    │    │  PowerShell  │      │
│  │    (.txt)    │    │   DNS API    │    │   Scripts    │      │
│  └──────────────┘    └──────────────┘    └──────────────┘      │
│                                                                 │
│  ┌──────────────┐                                               │
│  │   Website    │                                               │
│  │   (Gatsby)   │                                               │
│  └──────────────┘                                               │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## Components

### 1. Filter Rules (`/data/output/`)

**Purpose:** Core blocking lists and configuration for ad, tracker, and malware domains.

**Main File:** `adguard_user_filter.txt` (466 lines)

**Blocked Categories:**
- Ad service domains (Google, Facebook, Twitter, TikTok, Amazon)
- Analytics and tracking (Google Analytics, Sentry, Bugsnag)
- Platform-specific ad servers (Apple, Samsung, Realme, Oppo)
- Error reporting services

**Allowlisted:**
- Development tools (Microsoft, Postman, DataDog)

---

### 2. Filter Compiler (`/src/filter-compiler/`)

**Purpose:** TypeScript-based rule aggregation and transformation engine.

**Technology Stack:**
- TypeScript 5.4.5
- @adguard/hostlist-compiler v1.0.39
- Deno test (testing)
- Deno 2.0+

**Key Files:**
| File | Description |
|------|-------------|
| `invoke-compiler.ts` | Main compilation logic |
| `invoke-compiler.test.ts` | Jest unit tests |
| `compiler-config.json` | Compilation configuration |
| `invoke-compiler.ps1` | PowerShell wrapper script |

**Transformations Applied:**
- Deduplication
- Compression
- Validation
- ASCII conversion
- Whitespace cleanup

**Configuration Sources:**
- Local: `../../data/output/adguard_user_filter.txt`
- Remote: GitHub-hosted filter lists

---

### 3. AdGuard DNS API Client (`/src/adguard-api-dotnet/`)

**Purpose:** C# SDK for programmatic access to AdGuard DNS API v1.11.

**Generation:** Auto-generated from OpenAPI specification using OpenAPI Generator v7.16.0

**Solution Projects:**

| Project | Purpose |
|---------|---------|
| `AdGuard.ApiClient` | Core SDK library |
| `AdGuard.ApiClient.Test` | Unit tests |
| `AdGuard.ConsoleUI` | Interactive CLI application |

**API Coverage:**
- Account management (limits, usage)
- Device management (CRUD operations)
- DNS server profiles
- Dedicated IPv4 address allocation
- Filter lists management
- Query logging and analysis
- DNS statistics and reporting
- Web services configuration

**Authentication:**
- API Key
- Bearer Token (OAuth)

**Dependencies:**
- Newtonsoft.Json v13.0.2+
- JsonSubTypes v1.8.0+
- System.ComponentModel.Annotations v5.0.0+

---

### 4. Console UI (`/src/adguard-api-dotnet/src/AdGuard.ConsoleUI/`)

**Purpose:** Interactive command-line interface for managing AdGuard DNS configurations.

**Services:**
| Service | Functionality |
|---------|---------------|
| `AccountMenuService` | Account management operations |
| `DeviceMenuService` | Device CRUD operations |
| `DnsServerMenuService` | DNS server profile management |
| `FilterListMenuService` | Filter list operations |
| `QueryLogMenuService` | Query log viewing and analysis |
| `StatisticsMenuService` | DNS statistics and reporting |

---

### 5. Portfolio Website (`/src/website/`)

**Purpose:** Gatsby-based portfolio website to showcase the project.

**Technology Stack:**
- Gatsby 5.15.0
- React 18.3.1
- gatsby-theme-portfolio-minimal

**Content Structure:**
| Directory | Content |
|-----------|---------|
| `content/sections/` | Hero, Interests, Projects, Contact |
| `content/settings.json` | Site configuration and metadata |
| `content/articles/` | Blog/article content |
| `content/images/` | Media assets |

**Deployment:** GitHub Pages

---

### 6. PowerShell Scripts (`/src/adguard-api-powershell/`)

**Purpose:** Automation and orchestration scripts for filter compilation.

**Key Files:**
| File | Purpose |
|------|---------|
| `Invoke-RulesCompiler.psm1` | Rules compiler PowerShell module |
| `RulesCompiler.psd1` | Module manifest |
| `RulesCompiler-Harness.ps1` | Interactive test harness |

---

## Directory Structure

```
ad-blocking/
├── .github/
│   ├── workflows/           # CI/CD Pipelines (8 workflows)
│   └── ISSUE_TEMPLATE/      # Issue templates
├── docs/
│   ├── README.md            # Documentation index
│   ├── api/                 # Auto-generated API docs
│   └── guides/              # Usage guides
├── data/
│   └── output/
│   ├── adguard_user_filter.txt  # Main filter list
│   ├── Api/                 # Rules compilation API
│   └── Config/              # Configuration utilities
├── src/
│   ├── adguard-api-dotnet/  # AdGuard DNS API C# Client
│   ├── filter-compiler/     # TypeScript rule compiler
│   ├── adguard-api-powershell/  # PowerShell modules
│   └── website/             # Gatsby portfolio site
├── LICENSE                  # GPLv3
├── README.md                # Main documentation
└── SECURITY.md              # Security policy
```

---

## CI/CD Pipelines

| Workflow | File | Purpose | Triggers |
|----------|------|---------|----------|
| TypeScript | `typescript.yml` | Build rules-compiler-typescript, type-check, lint | Push to main, PRs |
| .NET | `dotnet.yml` | Build API client, run tests | Push to main, PRs |
| Gatsby | `gatsby.yml` | Build website, deploy to GitHub Pages | Push to main |
| CodeQL | `codeql.yml` | Security static analysis | Push to main, schedule |
| DevSkim | `devskim.yml` | Security scanning | Schedule |
| PowerShell | `powershell.yml` | PowerShell analysis | On demand |
| Static | `static.yml` | Static code analysis | Push to main, PRs |
| Claude AI | `claude-code-review.yml` | AI-powered code review | PRs |

---

## Technology Stack Summary

### Backend
| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 8.0 | Core framework (LTS) |
| C# | 13 | Primary backend language |
| Microsoft.Extensions | 9.0.10 | DI, Configuration, Logging |
| PowerShellStandard.Library | 5.1.1 | PowerShell integration |

### Filter Compilation
| Technology | Version | Purpose |
|------------|---------|---------|
| TypeScript | 5.4.5 | Strongly-typed JavaScript |
| @adguard/hostlist-compiler | 1.0.39 | Core compilation engine |
| Deno | 2.0+ | TypeScript/JavaScript runtime |
| Deno test | built-in | Testing framework |

### Frontend
| Technology | Version | Purpose |
|------------|---------|---------|
| Gatsby | 5.15.0 | Static site generator |
| React | 18.3.1 | UI framework |

### Security & Quality
| Tool | Purpose |
|------|---------|
| CodeQL | Static code analysis |
| DevSkim | Security scanning |
| ESLint | Code linting |
| Jest | Unit testing |

---

## API Documentation

### Available APIs

| API Class | Endpoints |
|-----------|-----------|
| `AccountApi` | Account limits, usage |
| `AuthenticationApi` | API key management, OAuth |
| `DevicesApi` | Device CRUD operations |
| `DNSServersApi` | DNS server profiles |
| `DedicatedIPAddressesApi` | IPv4 address allocation |
| `FilterListsApi` | Filter list management |
| `QueryLogApi` | Query logging |
| `StatisticsApi` | DNS statistics |
| `WebServicesApi` | Web service configuration |

### Data Models

Key models include:
- `Device` - Device configuration
- `DNSServer` - DNS server profile
- `FilterList` - Filter list definition
- `QueryLogItem` - Query log entry
- `Statistics` - DNS statistics data

Full API documentation available in `/docs/api/`.

---

## Quick Start

### Prerequisites
- Deno 2.0+
- .NET 10 SDK
- PowerShell 7+

### Filter Compiler
```bash
cd src/rules-compiler-typescript
deno task compile
```

### API Client
```bash
cd src/adguard-api-dotnet
dotnet restore
dotnet build
dotnet test
```

### Website
```bash
cd src/website
npm install  # Website still uses Node.js/Gatsby
npm run develop
```

---

## Testing

### TypeScript Tests
```bash
cd src/rules-compiler-typescript
deno task test
```

### .NET Tests
```bash
cd src/adguard-api-dotnet
dotnet test
```

### PowerShell Tests
```bash
cd src/adguard-api-powershell
Invoke-Pester
```

---

## Configuration

### Filter Compiler (`compiler-config.json`)
```json
{
  "sources": {
    "local": "../../data/output/adguard_user_filter.txt",
    "remote": "https://github.com/..."
  },
  "transformations": [
    "deduplicate",
    "compress",
    "validate",
    "ascii",
    "whitespace"
  ]
}
```

---

## Security

- **CodeQL Analysis:** Automated security scanning on all pushes
- **DevSkim Scanning:** Regular security vulnerability checks
- **Authentication:** Supports API Key and OAuth Bearer tokens

See `SECURITY.md` for vulnerability reporting guidelines.

---

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Run tests (`npm test`, `dotnet test`)
5. Submit a pull request

All PRs automatically receive:
- Claude AI code review
- CodeQL security analysis
- CI/CD pipeline validation

---

## License

This project is licensed under the GNU General Public License v3.0 (GPLv3).

See `LICENSE` file for full terms.

---

## Links & Resources

- **API Documentation:** `/docs/api/`
- **Usage Guides:** `/docs/guides/`
- **Security Policy:** `SECURITY.md`
- **AdGuard DNS:** https://adguard-dns.io

---

## Roadmap & Future Work

### Potential Enhancements
- [ ] Additional filter sources integration
- [ ] Real-time filter update notifications
- [ ] Dashboard for statistics visualization
- [ ] Mobile app integration
- [ ] Docker containerization
- [ ] Kubernetes deployment manifests

---

*This documentation is maintained as part of the ad-blocking repository and should be kept in sync with codebase changes.*
