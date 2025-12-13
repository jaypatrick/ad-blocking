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
│  │   Filter     │    │   Webhook    │    │  API Client  │      │
│  │  Compiler    │───▶│     App      │───▶│   (SDK)      │      │
│  │ (TypeScript) │    │   (.NET 8)   │    │    (C#)      │      │
│  └──────────────┘    └──────────────┘    └──────────────┘      │
│         │                   │                   │               │
│         ▼                   ▼                   ▼               │
│  ┌──────────────┐    ┌──────────────┐    ┌──────────────┐      │
│  │ Filter Rules │    │   AdGuard    │    │  Console UI  │      │
│  │    (.txt)    │    │   DNS API    │    │   (CLI)      │      │
│  └──────────────┘    └──────────────┘    └──────────────┘      │
│                                                                 │
│  ┌──────────────┐    ┌──────────────┐                          │
│  │  PowerShell  │    │   Website    │                          │
│  │   Scripts    │    │   (Gatsby)   │                          │
│  └──────────────┘    └──────────────┘                          │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## Components

### 1. Filter Rules (`/rules/`)

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
- Jest 29.7.0 (testing)
- Node.js 20

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
- Local: `../../rules/adguard_user_filter.txt`
- Remote: GitHub-hosted filter lists

---

### 3. Webhook Application (`/src/webhook-app/`)

**Purpose:** .NET 8.0 application for triggering AdGuard DNS updates via webhooks.

**Technology Stack:**
- .NET 8.0 (LTS)
- C# 13
- Microsoft.Extensions (Configuration, Hosting, Logging)
- System.Threading.RateLimiting

**Key Features:**
- Rate limiting (5 requests per 60-second window)
- Resilience patterns for HTTP calls
- Dependency injection
- PowerShell integration

**Directory Structure:**
| Directory | Purpose |
|-----------|---------|
| `Extensions/` | HTTP response extensions, rate limiting handlers |
| `Infrastructure/` | Client-side rate limiting implementation |
| `Global/` | Assembly definitions |

**Environment Variables:**
- `ADGUARD_WEBHOOK_URL` - Target webhook URL
- `SECRET_KEY` - Authentication key

---

### 4. AdGuard DNS API Client (`/src/api-client/`)

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

### 5. Console UI (`/src/api-client/src/AdGuard.ConsoleUI/`)

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

### 6. Portfolio Website (`/src/website/`)

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

### 7. PowerShell Scripts (`/scripts/powershell/`)

**Purpose:** Automation and orchestration scripts.

**Key Files:**
| File | Purpose |
|------|---------|
| `Webhook-Harness.ps1` | Webhook trigger script |
| `Invoke-WebHook.psm1` | WebHook PowerShell module |
| `Webhook-Manifest.psd1` | Module manifest |

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
├── rules/
│   ├── adguard_user_filter.txt  # Main filter list
│   ├── Api/                 # Rules compilation API
│   └── Config/              # Configuration utilities
├── scripts/
│   └── powershell/          # PowerShell modules
├── src/
│   ├── api-client/          # AdGuard DNS API C# Client
│   ├── filter-compiler/     # TypeScript rule compiler
│   ├── webhook-app/         # .NET 8.0 webhook application
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
| .NET | `dotnet.yml` | Build webhook-app, API client, run tests | Push to main, PRs |
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
| Node.js | 20 | JavaScript runtime |
| Jest | 29.7.0 | Testing framework |

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
- Node.js 20+
- .NET 8.0 SDK
- PowerShell 7+

### Filter Compiler
```bash
cd src/filter-compiler
npm install
npm run build
npm run compile
```

### Webhook App
```bash
cd src/webhook-app
dotnet restore
dotnet build
dotnet run
```

### API Client
```bash
cd src/api-client
dotnet restore
dotnet build
dotnet test
```

### Website
```bash
cd src/website
npm install
npm run develop
```

---

## Testing

### TypeScript Tests
```bash
cd src/filter-compiler
npm test
```

### .NET Tests
```bash
cd src/api-client
dotnet test
```

### PowerShell Tests
```bash
cd scripts/powershell
Invoke-Pester
```

---

## Configuration

### Filter Compiler (`compiler-config.json`)
```json
{
  "sources": {
    "local": "../../rules/adguard_user_filter.txt",
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

### Webhook App (`.env`)
```env
ADGUARD_WEBHOOK_URL=https://api.adguard-dns.io/webhook
SECRET_KEY=your-secret-key
```

---

## Security

- **CodeQL Analysis:** Automated security scanning on all pushes
- **DevSkim Scanning:** Regular security vulnerability checks
- **Rate Limiting:** Webhook app limits to 5 requests per 60 seconds
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
