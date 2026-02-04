#!/bin/bash
# Build script for .NET projects only
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

# Default to Debug configuration
CONFIGURATION="Debug"

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --release)
            CONFIGURATION="Release"
            shift
            ;;
        --debug)
            CONFIGURATION="Debug"
            shift
            ;;
        -h|--help)
            cat << EOF
Usage: $0 [OPTIONS]

Build all .NET projects in the repository.

OPTIONS:
    --debug             Use Debug configuration (default)
    --release           Use Release configuration
    -h, --help          Show this help message

EXAMPLES:
    $0                  # Build in Debug mode
    $0 --release        # Build in Release mode

EOF
            exit 0
            ;;
        *)
            echo -e "${RED}Unknown option: $1${NC}"
            exit 1
            ;;
    esac
done

echo -e "${BLUE}Building .NET projects...${NC}"
echo -e "${BLUE}Configuration: ${CONFIGURATION}${NC}"
echo ""

BUILD_FAILED=false

# Build AdGuard API Client
echo "→ Building AdGuard API Client (.NET)..."
if (cd src/adguard-api-dotnet && dotnet restore AdGuard.ApiClient.slnx && dotnet build AdGuard.ApiClient.slnx --no-restore --configuration $CONFIGURATION); then
    echo -e "${GREEN}✓ AdGuard API Client built successfully${NC}"
else
    echo -e "${RED}✗ AdGuard API Client build failed${NC}"
    BUILD_FAILED=true
fi

# Build Rules Compiler .NET
echo "→ Building Rules Compiler (.NET)..."
if (cd src/rules-compiler-dotnet && dotnet restore RulesCompiler.slnx && dotnet build RulesCompiler.slnx --no-restore --configuration $CONFIGURATION); then
    echo -e "${GREEN}✓ Rules Compiler (.NET) built successfully${NC}"
else
    echo -e "${RED}✗ Rules Compiler (.NET) build failed${NC}"
    BUILD_FAILED=true
fi

echo ""

if [[ "$BUILD_FAILED" == true ]]; then
    echo -e "${RED}✗ Some builds failed.${NC}"
    exit 1
else
    echo -e "${GREEN}✓ All .NET projects built successfully!${NC}"
    exit 0
fi
