#!/usr/bin/env bash
# Regenerate AdGuard API Rust Client from OpenAPI specification
# This script uses OpenAPI Generator to generate the Rust client code

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
API_DIR="${SCRIPT_DIR}/../adguard-api-client/api"
LIB_DIR="${SCRIPT_DIR}/adguard-api-lib"
OPENAPI_SPEC="${API_DIR}/openapi.json"
GENERATOR_VERSION="7.16.0"

echo "=========================================="
echo "AdGuard API Rust Client Generation Script"
echo "=========================================="
echo ""

# Check if OpenAPI spec exists
if [ ! -f "${OPENAPI_SPEC}" ]; then
    echo "ERROR: OpenAPI specification not found at: ${OPENAPI_SPEC}"
    echo ""
    echo "Please ensure the OpenAPI spec is available."
    exit 1
fi

echo "OpenAPI Spec: ${OPENAPI_SPEC}"
echo "Output Directory: ${LIB_DIR}"
echo "Generator Version: ${GENERATOR_VERSION}"
echo ""

# Create library directory if it doesn't exist
mkdir -p "${LIB_DIR}"

# Generate the Rust client using Docker
echo "Generating Rust client code..."
echo ""

docker run --rm \
    -v "${SCRIPT_DIR}:/local" \
    -v "${API_DIR}:/api" \
    openapitools/openapi-generator-cli:v${GENERATOR_VERSION} generate \
    -i /api/openapi.json \
    -g rust \
    -o /local/adguard-api-lib \
    --additional-properties=packageName=adguard-api-lib,packageVersion=1.0.0,supportAsync=true,useSingleRequestParameter=true

echo ""
echo "=========================================="
echo "Generation Complete!"
echo "=========================================="
echo ""
echo "Next steps:"
echo "  1. Review generated code in: ${LIB_DIR}/"
echo "  2. Build the library: cd ${LIB_DIR} && cargo build"
echo "  3. Run tests: cargo test"
echo ""
