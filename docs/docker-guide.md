# Docker Guide

This guide covers using Docker for development with the ad-blocking toolkit.

## Docker Development Environment

The repository includes a pre-configured Docker environment with all necessary dependencies for all compilers (TypeScript, .NET, Python, Rust) and tools.

### Dockerfile.warp

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0-noble

# Build arguments for version control
ARG NODE_VERSION=22
ARG RUST_VERSION=stable

# Includes installation of:
# - Node.js 22.x LTS
# - Python 3.12 with pip
# - Rust stable toolchain
# - PowerShell 7
# - yq (YAML processor)
# - hostlist-compiler (pre-installed globally)

WORKDIR /workspace
```

### Included Components

| Component | Version | Purpose |
|-----------|---------|---------|
| .NET SDK | 10.0 | .NET compiler, API client |
| Node.js | 22.x LTS | TypeScript compiler, hostlist-compiler |
| npm | Latest | Package management |
| Python | 3.12 | Python compiler |
| Rust | Stable | Rust compiler |
| PowerShell | 7.x | PowerShell scripts and modules |
| Git | Latest | Version control |
| yq | Latest | YAML processing for shell scripts |
| hostlist-compiler | Latest | Pre-installed globally |
| Ubuntu | 24.04 (Noble) | Base OS |

### Pre-installed Tools

- **npm global packages**: `@adguard/hostlist-compiler`, `typescript`, `ts-node`
- **Python packages**: `pytest`, `pytest-cov`, `mypy`, `ruff`, `pyyaml`, `tomlkit`
- **Rust components**: `clippy`, `rustfmt`
- **PowerShell modules**: `Pester`, `PSScriptAnalyzer`

## Building the Docker Image

### Standard Build

```bash
docker build -f Dockerfile.warp -t ad-blocking-dev .
```

### With Build Arguments

```bash
docker build -f Dockerfile.warp \
  --build-arg NODE_VERSION=22 \
  --build-arg RUST_VERSION=stable \
  -t ad-blocking-dev:custom .
```

### Multi-platform Build

```bash
docker buildx build -f Dockerfile.warp \
  --platform linux/amd64,linux/arm64 \
  -t ad-blocking-dev:multiarch .
```

## Running the Container

### Interactive Mode

```bash
docker run -it -v $(pwd):/workspace ad-blocking-dev
```

### With Environment Variables

```bash
docker run -it \
  -v $(pwd):/workspace \
  -e "AdGuard__ApiKey=your-api-key" \
  -e "DEBUG=1" \
  ad-blocking-dev
```

### Detached Mode

```bash
docker run -d \
  -v $(pwd):/workspace \
  --name ad-blocking-container \
  ad-blocking-dev \
  sleep infinity
```

Then execute commands:

```bash
docker exec ad-blocking-container npm run compile
```

## Docker Compose

The repository includes a `docker-compose.yml` for multi-service orchestration.

### Available Services

| Service | Description | Profile |
|---------|-------------|---------|
| `dev` | Main development environment | default |
| `typescript-compiler` | TypeScript rules compiler | compile |
| `dotnet-compiler` | .NET rules compiler | compile |
| `python-compiler` | Python rules compiler | compile |
| `rust-compiler` | Rust rules compiler | compile |
| `website` | Gatsby development server | website |
| `test` | Run all tests | test |
| `console-ui` | AdGuard Console UI | console |

### Basic Usage

```bash
# Start main development environment
docker compose up -d dev

# Enter the container
docker compose exec dev bash

# Stop all services
docker compose down
```

### Running Specific Services

```bash
# Run TypeScript compiler
docker compose --profile compile run --rm typescript-compiler

# Run all compilers
docker compose --profile compile up

# Start website development server
docker compose --profile website up website

# Run all tests
docker compose --profile test run --rm test

# Run AdGuard Console UI
docker compose --profile console run --rm console-ui
```

### Environment Variables

Create a `.env` file in the project root:

```bash
# .env
DEBUG=1
ADGUARD_API_KEY=your-api-key-here
```

## Development Workflow

### Initial Setup (Inside Container)

```bash
# TypeScript compiler
cd /workspace/src/rules-compiler-typescript
npm ci

# .NET projects
cd /workspace/src/rules-compiler-dotnet
dotnet restore RulesCompiler.slnx

cd /workspace/src/adguard-api-dotnet
dotnet restore src/AdGuard.ApiClient.sln

# Python compiler
cd /workspace/src/rules-compiler-python
pip install -e ".[dev]"

# Rust compiler
cd /workspace/src/rules-compiler-rust
cargo build

# Website (optional)
cd /workspace/src/website
npm ci
```

### Compiling Filter Rules

```bash
# TypeScript
cd /workspace/src/rules-compiler-typescript
npm run compile

# .NET
cd /workspace/src/rules-compiler-dotnet
dotnet run --project src/RulesCompiler.Console

# Python
cd /workspace/src/rules-compiler-python
rules-compiler

# Rust
cd /workspace/src/rules-compiler-rust
cargo run --release

# Shell (Bash)
/workspace/src/rules-compiler-shell/compile-rules.sh

# PowerShell
cd /workspace
pwsh -Command "Import-Module ./src/adguard-api-powershell/Invoke-RulesCompiler.psm1; Invoke-RulesCompiler"
```

### Running Tests

```bash
# TypeScript tests
cd /workspace/src/rules-compiler-typescript
npm test

# .NET tests
cd /workspace/src/rules-compiler-dotnet
dotnet test RulesCompiler.slnx

# Python tests
cd /workspace/src/rules-compiler-python
pytest

# Rust tests
cd /workspace/src/rules-compiler-rust
cargo test

# PowerShell tests
cd /workspace
pwsh -Command "Invoke-Pester -Path ./src/adguard-api-powershell/Tests/"

# Or run all tests via docker compose
docker compose --profile test run --rm test
```

## Warp Environment

For [Warp](https://www.warp.dev/) terminal users, a pre-built environment is available.

### Environment Details

| Property | Value |
|----------|-------|
| Docker Image | `jaysonknight/warp-env:ad-blocking` |
| Environment ID | `Egji4sZU4TNIOwNasFU73A` |

### Using with Warp

```bash
# The environment automatically runs these setup commands:
cd ad-blocking/src/rules-compiler-typescript && npm install
cd ad-blocking/src/website && npm install
cd ad-blocking/src/adguard-api-dotnet && dotnet restore
```

## CI/CD with Docker

### GitHub Actions Example

```yaml
name: Build with Docker

on: [push, pull_request]

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v4

      - name: Build Docker image
        run: docker build -f Dockerfile.warp -t ad-blocking-dev .

      - name: Run TypeScript tests
        run: |
          docker run --rm \
            -v ${{ github.workspace }}:/workspace \
            ad-blocking-dev \
            bash -c "cd /workspace/src/rules-compiler-typescript && npm ci && npm test"

      - name: Run .NET tests
        run: |
          docker run --rm \
            -v ${{ github.workspace }}:/workspace \
            ad-blocking-dev \
            bash -c "cd /workspace/src/rules-compiler-dotnet && dotnet restore && dotnet test"

      - name: Run Python tests
        run: |
          docker run --rm \
            -v ${{ github.workspace }}:/workspace \
            ad-blocking-dev \
            bash -c "cd /workspace/src/rules-compiler-python && pip install -e '.[dev]' && pytest"

      - name: Run Rust tests
        run: |
          docker run --rm \
            -v ${{ github.workspace }}:/workspace \
            ad-blocking-dev \
            bash -c "cd /workspace/src/rules-compiler-rust && cargo test"
```

## Troubleshooting

### Permission Issues

If you encounter permission issues with mounted volumes:

```bash
# Run as current user
docker run -it -v $(pwd):/workspace -u $(id -u):$(id -g) ad-blocking-dev
```

### Node Modules Issues

If `node_modules` has issues between host and container:

```bash
# Remove node_modules and reinstall inside container
docker run -it -v $(pwd):/workspace ad-blocking-dev bash -c "
  rm -rf /workspace/src/rules-compiler-typescript/node_modules
  cd /workspace/src/rules-compiler-typescript
  npm ci
"
```

### .NET Restore Failures

If .NET restore fails:

```bash
docker run -it -v $(pwd):/workspace ad-blocking-dev bash -c "
  cd /workspace/src/rules-compiler-dotnet
  dotnet nuget locals all --clear
  dotnet restore RulesCompiler.slnx
"
```

### Cargo/Rust Issues

If Rust compilation fails:

```bash
docker run -it -v $(pwd):/workspace ad-blocking-dev bash -c "
  cd /workspace/src/rules-compiler-rust
  cargo clean
  cargo build
"
```

### Container Won't Start

Check if the image exists:

```bash
docker images | grep ad-blocking-dev
```

Rebuild if necessary:

```bash
docker build --no-cache -f Dockerfile.warp -t ad-blocking-dev .
```

### Volume Caching Issues

Reset named volumes if caching causes problems:

```bash
# Remove all ad-blocking volumes
docker volume rm ad-blocking-node-ts ad-blocking-node-web ad-blocking-cargo-registry ad-blocking-cargo-git ad-blocking-nuget

# Or remove all unused volumes
docker volume prune
```
