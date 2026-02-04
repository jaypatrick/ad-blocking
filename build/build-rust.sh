#!/bin/bash
# Build script for Rust projects only
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

# Default to debug profile
BUILD_PROFILE="debug"

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --release)
            BUILD_PROFILE="release"
            shift
            ;;
        --debug)
            BUILD_PROFILE="debug"
            shift
            ;;
        -h|--help)
            cat << EOF
Usage: $0 [OPTIONS]

Build all Rust projects in the repository.

OPTIONS:
    --debug             Use debug profile (default)
    --release           Use release profile
    -h, --help          Show this help message

EXAMPLES:
    $0                  # Build in debug mode
    $0 --release        # Build in release mode

EOF
            exit 0
            ;;
        *)
            echo -e "${RED}Unknown option: $1${NC}"
            exit 1
            ;;
    esac
done

echo -e "${BLUE}Building Rust projects...${NC}"
echo -e "${BLUE}Build Profile: ${BUILD_PROFILE}${NC}"
echo ""

# Build the entire workspace
echo "→ Building Rust workspace..."
if [[ "$BUILD_PROFILE" == "release" ]]; then
    cargo build --release --workspace
else
    cargo build --workspace
fi

if [[ $? -eq 0 ]]; then
    echo -e "${GREEN}✓ Rust workspace built successfully${NC}"
    exit 0
else
    echo -e "${RED}✗ Rust workspace build failed${NC}"
    exit 1
fi
