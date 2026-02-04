#!/bin/bash
# Build script for Python projects only
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

Build all Python projects in the repository.

OPTIONS:
    -h, --help          Show this help message

EXAMPLES:
    $0                  # Build all Python projects

REQUIREMENTS:
    - Python 3.9 or later must be installed

EOF
            exit 0
            ;;
        *)
            echo -e "${RED}Unknown option: $1${NC}"
            exit 1
            ;;
    esac
done

echo -e "${BLUE}Building Python projects...${NC}"
echo ""

# Check if Python is installed
if ! command -v python3 &> /dev/null; then
    echo -e "${RED}✗ Python 3 is not installed. Please install Python 3 to build Python projects.${NC}"
    exit 1
fi

BUILD_FAILED=false

# Build Rules Compiler Python
echo "→ Building Rules Compiler (Python)..."
if (cd src/rules-compiler-python && python3 -m pip install --quiet -e ".[dev]" && python3 -m mypy rules_compiler/); then
    echo -e "${GREEN}✓ Rules Compiler (Python) built successfully${NC}"
else
    echo -e "${RED}✗ Rules Compiler (Python) build failed${NC}"
    BUILD_FAILED=true
fi

echo ""

if [[ "$BUILD_FAILED" == true ]]; then
    echo -e "${RED}✗ Some builds failed.${NC}"
    exit 1
else
    echo -e "${GREEN}✓ All Python projects built successfully!${NC}"
    exit 0
fi
