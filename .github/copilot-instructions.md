# Copilot Instructions for Ad-Blocking Repository

## Repository Overview

This is a multi-language repository for ad-blocking and DNS filtering tools, including:
- **Filter Rules**: AdGuard filter lists for blocking ads, trackers, and malware
- **API Client**: C# SDK for AdGuard DNS API
- **Rules Compilers**: Multiple implementations (TypeScript, C#, Python, Rust) for compiling filter rules
- **Website**: Gatsby-based portfolio website
- **Scripts**: PowerShell automation scripts

## Project Structure

```
ad-blocking/
├── .github/              # GitHub configuration and workflows
├── docs/                 # Documentation (API docs, guides)
├── rules/                # Filter rules and compilation configs
├── scripts/              # PowerShell automation scripts
├── src/
│   ├── api-client/              # C# AdGuard DNS API client
│   ├── rules-compiler-typescript/  # TypeScript rules compiler
│   ├── rules-compiler-dotnet/      # .NET rules compiler
│   ├── rules-compiler-python/      # Python rules compiler
│   ├── rules-compiler-rust/        # Rust rules compiler
│   └── website/                    # Gatsby website
└── README.md
```

## Build and Test Commands

### C# API Client (`src/api-client/`)
```bash
# Navigate to the solution directory
cd src/api-client/src

# Restore dependencies
dotnet restore AdGuard.ApiClient.sln

# Build
dotnet build AdGuard.ApiClient.sln --no-restore

# Run tests
dotnet test AdGuard.ApiClient.sln --no-build --verbosity normal

# Run specific test project
dotnet test AdGuard.ApiClient.Test/AdGuard.ApiClient.Test.csproj
```

**Requirements**: .NET 8.0 SDK

### TypeScript Rules Compiler (`src/rules-compiler-typescript/`)
```bash
# Install dependencies
npm ci

# Build
npm run build

# Run compiler
npm run compile

# Type check
npx tsc --noEmit

# Lint
npx eslint .

# Run tests
npm test

# Test with coverage
npm run test:coverage
```

**Requirements**: Node.js 18+

### Python Rules Compiler (`src/rules-compiler-python/`)
```bash
# Install in development mode
pip install -e ".[dev]"

# Run tests
pytest

# Type checking
mypy rules_compiler/

# Linting
ruff check rules_compiler/

# Run compiler
rules-compiler --help
```

**Requirements**: Python 3.9+

### Rust Rules Compiler (`src/rules-compiler-rust/`)
```bash
# Build
cargo build

# Build optimized release
cargo build --release

# Run tests
cargo test

# Run compiler
cargo run -- --help
```

**Requirements**: Rust toolchain

### Website (`src/website/`)
```bash
# Install dependencies
npm ci

# Development server
npm run develop

# Build for production
npm run build
```

**Requirements**: Node.js 18+, Gatsby CLI

### PowerShell Scripts (`scripts/powershell/`)
```bash
# Run PSScriptAnalyzer
pwsh -Command "Invoke-ScriptAnalyzer -Path . -Recurse"
```

**Requirements**: PowerShell 7+

## Code Conventions

### General Principles
- Follow existing patterns in each language/component
- Write clear, self-documenting code
- Use descriptive variable and function names
- Add comments only when code intent is not obvious

### C# (.NET)
- Use **C# 12** features with **.NET 8.0**
- Follow Microsoft C# coding conventions
- Use **async/await** for I/O operations
- Dependency injection for services (especially `ILogger`)
- Use nullable reference types (`#nullable enable`)
- Test with **xUnit** framework
- Use **Polly** for resilience and retry policies

### TypeScript
- **Target**: ES2022, Node.js 18+
- **Strict mode** enabled (`"strict": true`)
- Use **node:** prefixed imports for Node.js built-ins (e.g., `node:fs`, `node:path`, `node:crypto`)
- Follow functional programming patterns where appropriate
- Use **JSDoc** comments for exported functions
- Test with **Jest**
- Use **ESLint** for linting

### Python
- **Python 3.9+** compatibility
- Follow **PEP 8** style guide
- Use **type hints** (typing module)
- Line length: 100 characters (configured in ruff)
- Use **pytest** for testing
- Use **mypy** for type checking
- Use **ruff** for linting and formatting

### Rust
- **Edition 2021**
- Follow standard Rust conventions
- Use **serde** for serialization
- Use **clap** for CLI argument parsing
- Use **thiserror/anyhow** for error handling
- Use **sha2** for SHA-384 hashing

### PowerShell
- Follow PowerShell best practices
- Use approved verbs (Get-, Set-, New-, etc.)
- Avoid global aliases
- Never use `ConvertTo-SecureString` with plain text
- Test with **PSScriptAnalyzer**

## Security Best Practices

### Secrets Management
- **NEVER commit sensitive data** to the repository
- Use environment variables for secrets:
  - `ADGUARD_WEBHOOK_URL`
  - `SECRET_KEY`
  - API tokens and keys
- Check `.env.example` files for configuration templates

### Dependencies
- Regularly update dependencies to patch vulnerabilities
- Review security advisories (CodeQL, DevSkim run in CI)
- Use rate limiting for HTTP requests

### Filter Rules
- Be cautious when adding rules from untrusted sources
- Validate and test new filter rules before deployment

## Rules Compilation

### Rule Counting
- Filter out empty lines
- Filter out lines starting with `!` (comments)
- Filter out lines starting with `#` (comments)

### Hashing
- Use **SHA-384** for computing output file hashes
- Hash the compiled output file content

### Compilation Results
- Track success/failure status
- Record metrics: rule count, hash, elapsed time
- Include timestamps for compilation

## Development Workflow

### Git Workflow
- Create feature branches from `main`
- Use descriptive commit messages
- CI runs on push to `main` and pull requests

### CI/CD Pipelines
- **.NET**: Builds and tests API client
- **TypeScript**: Builds and lints TypeScript projects
- **PowerShell**: Runs PSScriptAnalyzer
- **CodeQL**: Security scanning
- **DevSkim**: Security scanning
- **Gatsby**: Website deployment

### Testing Strategy
- Write unit tests for new functionality
- Maintain existing test coverage
- Run tests locally before pushing
- CI runs all tests automatically

## Common Tasks

### Adding Dependencies
```bash
# C#
dotnet add package <PackageName>

# TypeScript/Node
npm install <package-name>

# Python
# Add to pyproject.toml [project.dependencies]

# Rust
# Add to Cargo.toml [dependencies]
```

### Running the Full Test Suite
```bash
# From repository root
cd src/api-client/src && dotnet test AdGuard.ApiClient.sln
cd ../../rules-compiler-typescript && npm test
cd ../rules-compiler-python && pytest
cd ../rules-compiler-rust && cargo test
```

### Compiling Filter Rules
```bash
# TypeScript compiler
cd src/rules-compiler-typescript
npm run compile

# PowerShell harness
cd scripts/powershell
./RulesCompiler-Harness.ps1
```

## Documentation

- **API Client**: See `src/api-client/README.md` and `docs/api/`
- **Usage Guides**: See `docs/guides/`
- **Security Policy**: See `SECURITY.md`
- **Main README**: See `README.md`

## Additional Notes

- Repository uses **GitHub Actions** for CI/CD
- **Warp environment** available with pre-configured Docker image: `jaysonknight/warp-env:ad-blocking`
- Filter rules list: `rules/adguard_user_filter.txt`
- Website deployed to **GitHub Pages**

## Azure-Specific Rule
- @azure Rule - Use Azure Best Practices: When generating code for Azure, running terminal commands for Azure, or performing operations related to Azure, invoke your `get_azure_best_practices` tool if available.