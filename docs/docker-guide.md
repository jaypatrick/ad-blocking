# Docker Guide

This guide covers using Docker for development with the ad-blocking toolkit.

## Docker Development Environment

The repository includes a pre-configured Docker environment with all necessary dependencies.

### Dockerfile.warp

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy

# Install Node.js 20.x LTS via NodeSource
RUN apt-get update && apt-get install -y curl gnupg && \
    curl -fsSL https://deb.nodesource.com/setup_20.x | bash - && \
    apt-get install -y nodejs git && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

# Install PowerShell
RUN apt-get update && \
    apt-get install -y wget apt-transport-https software-properties-common && \
    wget -q "https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb" && \
    dpkg -i packages-microsoft-prod.deb && \
    rm packages-microsoft-prod.deb && \
    apt-get update && \
    apt-get install -y powershell && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

# Verify installations
RUN dotnet --version && node --version && npm --version && pwsh --version && git --version

WORKDIR /workspace
```

### Included Components

| Component | Version | Purpose |
|-----------|---------|---------|
| .NET SDK | 8.0 | .NET compiler, API client |
| Node.js | 20.x LTS | TypeScript compiler, hostlist-compiler |
| npm | Latest | Package management |
| PowerShell | 7.x | PowerShell scripts and modules |
| Git | Latest | Version control |
| Ubuntu | 22.04 (Jammy) | Base OS |

## Building the Docker Image

### Standard Build

```bash
docker build -f Dockerfile.warp -t ad-blocking-dev .
```

### With Build Arguments

```bash
docker build -f Dockerfile.warp \
  --build-arg NODE_VERSION=20 \
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
  -e "AdGuard:ApiKey=your-api-key" \
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

## Development Workflow

### Initial Setup (Inside Container)

```bash
# Install hostlist-compiler globally
npm install -g @adguard/hostlist-compiler

# TypeScript compiler
cd /workspace/src/rules-compiler-typescript
npm install

# .NET projects
cd /workspace/src/rules-compiler-dotnet
dotnet restore RulesCompiler.slnx

cd /workspace/src/adguard-api-client
dotnet restore src/AdGuard.ApiClient.sln

# Website (optional)
cd /workspace/src/website
npm install
```

### Compiling Filter Rules

```bash
# TypeScript
cd /workspace/src/rules-compiler-typescript
npm run compile

# .NET
cd /workspace/src/rules-compiler-dotnet
dotnet run --project src/RulesCompiler.Console

# PowerShell
cd /workspace
pwsh -Command "Import-Module ./src/powershell/Invoke-RulesCompiler.psm1; Invoke-RulesCompiler"
```

### Running Tests

```bash
# TypeScript tests
cd /workspace/src/rules-compiler-typescript
npm test

# .NET tests
cd /workspace/src/rules-compiler-dotnet
dotnet test RulesCompiler.slnx

# PowerShell tests
cd /workspace
pwsh -Command "Invoke-Pester -Path ./src/powershell/Tests/"
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
cd ad-blocking/src/adguard-api-client && dotnet restore
```

### Creating Warp Integrations

```bash
# Slack integration
warp integration create slack --environment Egji4sZU4TNIOwNasFU73A

# Linear integration
warp integration create linear --environment Egji4sZU4TNIOwNasFU73A
```

## Docker Compose (Optional)

Create `docker-compose.yml` for more complex setups:

```yaml
version: '3.8'

services:
  dev:
    build:
      context: .
      dockerfile: Dockerfile.warp
    volumes:
      - .:/workspace
      - node_modules:/workspace/src/rules-compiler-typescript/node_modules
      - website_modules:/workspace/src/website/node_modules
    environment:
      - DEBUG=1
    working_dir: /workspace
    stdin_open: true
    tty: true

  # Optional: Run website in development mode
  website:
    build:
      context: .
      dockerfile: Dockerfile.warp
    volumes:
      - .:/workspace
      - website_modules:/workspace/src/website/node_modules
    ports:
      - "8000:8000"
    working_dir: /workspace/src/website
    command: npm run develop -- --host 0.0.0.0
    depends_on:
      - dev

volumes:
  node_modules:
  website_modules:
```

### Using Docker Compose

```bash
# Start development environment
docker-compose up -d dev

# Execute commands
docker-compose exec dev npm run compile

# Start website development server
docker-compose up website

# Stop all services
docker-compose down
```

## Custom Dockerfile

Create a custom Dockerfile for specific needs:

### With Python and Rust

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy

# Node.js
RUN apt-get update && apt-get install -y curl gnupg && \
    curl -fsSL https://deb.nodesource.com/setup_20.x | bash - && \
    apt-get install -y nodejs && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

# Python
RUN apt-get update && apt-get install -y python3 python3-pip && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

# Rust
RUN curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh -s -- -y
ENV PATH="/root/.cargo/bin:${PATH}"

# PowerShell
RUN apt-get update && \
    apt-get install -y wget apt-transport-https software-properties-common && \
    wget -q "https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb" && \
    dpkg -i packages-microsoft-prod.deb && \
    rm packages-microsoft-prod.deb && \
    apt-get update && \
    apt-get install -y powershell && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

# Install hostlist-compiler
RUN npm install -g @adguard/hostlist-compiler

WORKDIR /workspace
```

### Build and Run

```bash
docker build -f Dockerfile.custom -t ad-blocking-full .
docker run -it -v $(pwd):/workspace ad-blocking-full
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
  npm install
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

### Container Won't Start

Check if the image exists:

```bash
docker images | grep ad-blocking-dev
```

Rebuild if necessary:

```bash
docker build --no-cache -f Dockerfile.warp -t ad-blocking-dev .
```
