#!/usr/bin/env bash
# Quick download of the latest AdGuard DNS API OpenAPI specification
# Usage: ./quick-download.sh

set -euo pipefail

OPENAPI_URL="https://api.adguard-dns.io/swagger/openapi.json"
OUTPUT_FILE="api/openapi.json"

echo "Downloading AdGuard DNS API OpenAPI specification..."
echo "URL: ${OPENAPI_URL}"
echo ""

# Create api directory if it doesn't exist
mkdir -p "$(dirname "${OUTPUT_FILE}")"

# Backup existing file if it exists
if [ -f "${OUTPUT_FILE}" ]; then
    BACKUP="${OUTPUT_FILE}.backup-$(date +%Y%m%d_%H%M%S)"
    echo "Backing up existing file to: ${BACKUP}"
    cp "${OUTPUT_FILE}" "${BACKUP}"
fi

# Download the specification
if curl -f -sL "${OPENAPI_URL}" -o "${OUTPUT_FILE}"; then
    echo "✓ Successfully downloaded OpenAPI specification"
    echo "  Saved to: ${OUTPUT_FILE}"
    echo ""
    echo "Next steps:"
    echo "  1. Convert to YAML (optional): yq eval -P ${OUTPUT_FILE} > api/openapi.yaml"
    echo "  2. Regenerate API client: ./regenerate-client.sh"
    echo "  3. Or run complete workflow: ./update-api-client.sh"
else
    echo "✗ Failed to download specification"
    exit 1
fi
