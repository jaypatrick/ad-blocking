#!/bin/bash
# Build script for TypeScript/Deno projects only
# Part of the ad-blocking repository build system

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
cd "$REPO_ROOT"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -h|--help)
            cat << EOF
Usage: $0 [OPTIONS]

Build all TypeScript/Deno projects in the repository.

OPTIONS:
    -h, --help          Show this help message

EXAMPLES:
    $0                  # Build all TypeScript projects

REQUIREMENTS:
    - Deno 2.0 or later must be installed

EOF
            exit 0
            ;;
        *)
            echo -e "${RED}Unknown option: $1${NC}"
            exit 1
            ;;
    esac
done

echo -e "${BLUE}Building TypeScript/Deno projects...${NC}"
echo ""

# Check if Deno is installed
if ! command -v deno &> /dev/null; then
    echo -e "${RED}✗ Deno is not installed. Please install Deno to build TypeScript projects.${NC}"
    exit 1
fi

BUILD_FAILED=false

# Build Rules Compiler TypeScript
echo "→ Building Rules Compiler (TypeScript)..."
if (cd src/rules-compiler-typescript && deno task generate:types && deno task check); then
    echo -e "${GREEN}✓ Rules Compiler (TypeScript) built successfully${NC}"
else
    echo -e "${RED}✗ Rules Compiler (TypeScript) build failed${NC}"
    BUILD_FAILED=true
fi

# Build AdGuard API TypeScript
echo "→ Building AdGuard API Client (TypeScript)..."
if (cd src/adguard-api-typescript && deno task generate:types && deno task check); then
    echo -e "${GREEN}✓ AdGuard API Client (TypeScript) built successfully${NC}"
else
    echo -e "${RED}✗ AdGuard API Client (TypeScript) build failed${NC}"
    BUILD_FAILED=true
fi

# Build Linear tool
echo "→ Building Linear Import Tool (TypeScript)..."
if (cd src/linear && deno task generate:types && deno task check); then
    echo -e "${GREEN}✓ Linear Import Tool built successfully${NC}"
else
    echo -e "${RED}✗ Linear Import Tool build failed${NC}"
    BUILD_FAILED=true
fi

echo ""

if [[ "$BUILD_FAILED" == true ]]; then
    echo -e "${RED}✗ Some builds failed.${NC}"
    exit 1
else
    echo -e "${GREEN}✓ All TypeScript projects built successfully!${NC}"
    exit 0
fi
