# Contributing to Ad-Blocking Repository

Thank you for your interest in contributing to the ad-blocking repository! This document provides guidelines for contributing to this multi-language monorepo.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Repository Structure](#repository-structure)
- [Development Workflow](#development-workflow)
- [Coding Standards](#coding-standards)
- [Testing Requirements](#testing-requirements)
- [Pull Request Process](#pull-request-process)
- [Security](#security)

## Code of Conduct

By participating in this project, you agree to maintain a respectful, inclusive, and harassment-free environment for everyone.

## Getting Started

### Prerequisites

Install the required tools for the language you're working with:

- **TypeScript/Deno**: [Deno 2.0+](https://deno.land/)
- **.NET**: [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- **Python**: [Python 3.9+](https://www.python.org/)
- **Rust**: [Rust 1.83+](https://rustup.rs/)
- **PowerShell**: [PowerShell 7+](https://github.com/PowerShell/PowerShell)

### Initial Setup

```bash
# Clone the repository
git clone https://github.com/jaypatrick/ad-blocking.git
cd ad-blocking

# Build all projects (or specific language ecosystems)
./build.sh              # All projects
./build.sh --rust       # Rust only
./build.sh --dotnet     # .NET only
```

See [README.md](README.md) for detailed setup instructions.

## Repository Structure

This is a multi-language monorepo organized as follows:

```
ad-blocking/
├── api/                    # Centralized OpenAPI specifications
├── docs/                   # All documentation
├── tools/                  # Build and utility scripts
├── data/                   # Filter rules and compilation data
├── src/                    # Source code organized by language
│   ├── adguard-api-dotnet/         # C# API SDK
│   ├── adguard-api-rust/           # Rust API SDK  
│   ├── adguard-api-typescript/     # TypeScript API SDK
│   ├── adguard-api-powershell/     # PowerShell API client
│   ├── rules-compiler-dotnet/      # .NET rules compiler
│   ├── rules-compiler-rust/        # Rust rules compiler
│   ├── rules-compiler-typescript/  # TypeScript rules compiler
│   ├── rules-compiler-python/      # Python rules compiler
│   ├── powershell-modules/         # PowerShell modules
│   ├── shell-scripts/              # Shell script utilities
│   ├── adguard-validation/         # Rust validation library
│   └── linear/                     # Linear integration
└── .github/                # GitHub workflows and configuration
```

### Important Locations

- **OpenAPI specs**: `api/` (single source of truth, don't duplicate)
- **Documentation**: `docs/` (project-wide docs, guides, references)
- **Build scripts**: Root level (`build.sh`, `build.ps1`) and `tools/`
- **Tests**: Within each project's directory

## Development Workflow

### 1. Create a Branch

```bash
git checkout -b feature/your-feature-name
# or
git checkout -b fix/issue-description
```

### 2. Make Changes

Follow the coding standards for your language (see below).

### 3. Test Your Changes

```bash
# Test specific language ecosystem
./build.sh --rust && cargo test --workspace
./build.sh --dotnet && dotnet test

# Or use language-specific commands
cd src/rules-compiler-typescript && deno task test
cd src/rules-compiler-python && pytest
```

### 4. Lint Your Code

```bash
# TypeScript/Deno
deno lint

# .NET
dotnet format

# Python
ruff check rules_compiler/

# Rust
cargo clippy --workspace

# PowerShell
Invoke-ScriptAnalyzer -Path src/powershell-modules -Recurse
```

### 5. Commit Changes

Use clear, descriptive commit messages:

```bash
git add .
git commit -m "Add feature: description of what you added"
# or
git commit -m "Fix: description of what was fixed"
```

## Coding Standards

### General Principles

- Write clear, self-documenting code
- Follow existing patterns in each language/component
- Add comments only when code intent is not obvious
- Maintain consistency with the existing codebase

### Language-Specific Standards

#### C# (.NET)
- Use C# 14 features with .NET 10
- Enable nullable reference types (`#nullable enable`)
- Use dependency injection for services
- Use async/await for all I/O operations
- Follow [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)

#### TypeScript/Deno
- Target ES2022, Node.js 18+
- Enable strict mode in tsconfig.json
- Use `node:` prefixed imports for built-ins (`node:fs`, `node:path`)
- Use SHA-384 for hashing (never SHA-256)
- Add JSDoc for exported functions

#### Python
- Python 3.9+ compatibility
- Follow PEP 8 style guide
- Use type hints (typing module)
- Line length: 100 characters
- Use pytest for testing
- Use mypy for type checking

#### Rust
- Edition 2021
- Follow standard Rust conventions
- Use cargo fmt for formatting
- Use clippy for linting
- Document public APIs

#### PowerShell
- PowerShell 7+ (Core, not Windows PowerShell 5.1)
- Use approved verbs (Get-, Set-, New-, Invoke-, etc.)
- Module structure: `.psm1` + `.psd1` manifest
- Never use `ConvertTo-SecureString` with plaintext secrets
- Use Pester v5 for tests

### Security Practices

**NEVER commit:**
- API keys, tokens, passwords
- `.env` files (only `.env.example` templates)

**Always:**
- Use environment variables for secrets
- Validate input from external sources
- Use HTTPS for remote sources
- Verify hashes for downloaded files

## Testing Requirements

### Required Tests

All new features and bug fixes must include appropriate tests:

- **Unit tests**: Test individual functions/methods
- **Integration tests**: Test component interactions
- **End-to-end tests**: Test complete workflows (where applicable)

### Test Coverage

- Maintain existing test coverage levels
- Aim for >80% coverage for new code
- All public APIs must have tests

### Running Tests

```bash
# Run all tests
./tools/test-build-scripts.sh     # Build script tests

# Language-specific tests
cd src/rules-compiler-typescript && deno task test
cd src/rules-compiler-dotnet && dotnet test
cd src/rules-compiler-python && pytest
cd src/rules-compiler-rust && cargo test
cd src/powershell-modules && Invoke-Pester
```

## Pull Request Process

### Before Submitting

1. ✅ All tests pass locally
2. ✅ Code is linted and formatted
3. ✅ Documentation is updated (if needed)
4. ✅ Commit messages are clear
5. ✅ No sensitive data in commits

### PR Description

Include in your PR description:

- **Summary**: What does this PR do?
- **Motivation**: Why is this change needed?
- **Testing**: How was this tested?
- **Breaking Changes**: Any breaking changes?
- **Related Issues**: Link to related issues

### Review Process

1. Automated checks must pass (CI/CD workflows)
2. At least one maintainer review required
3. Address all review comments
4. Squash commits if requested
5. Maintainer will merge when approved

### CI/CD Checks

All PRs must pass:

- ✅ Build scripts tests
- ✅ Language-specific linting
- ✅ Unit and integration tests
- ✅ Security scans (CodeQL, DevSkim)
- ✅ Dependency checks

## Security

### Reporting Vulnerabilities

**DO NOT** open public issues for security vulnerabilities.

Instead, follow the process in [SECURITY.md](SECURITY.md).

### Security Requirements

- All filter compilation includes mandatory validation
- Use centralized validation library for security checks
- HTTPS enforcement for remote sources
- SHA-384 hash verification for integrity
- No plaintext secrets in code

## Questions?

- Check existing [documentation](docs/)
- Review [README.md](README.md)
- Look at similar existing code
- Ask questions in PR comments

## License

By contributing, you agree that your contributions will be licensed under the same license as the project (GPL-3.0).

---

Thank you for contributing! 🎉
