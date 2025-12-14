#!/usr/bin/env bash
# Complete workflow to download OpenAPI spec and update the API client
# Usage: ./update-api-client.sh

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
API_DIR="${SCRIPT_DIR}/api"
OPENAPI_SPEC="${API_DIR}/openapi.yaml"
OPENAPI_JSON="${API_DIR}/openapi.json"
OPENAPI_URL="https://api.adguard-dns.io/swagger/openapi.json"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}=========================================="
echo "AdGuard API Client Update Workflow"
echo -e "==========================================${NC}"
echo ""

# Ensure API directory exists
mkdir -p "${API_DIR}"

# Step 1: Download the latest OpenAPI spec
echo -e "${YELLOW}Step 1: Downloading latest OpenAPI specification...${NC}"
echo "  URL: ${OPENAPI_URL}"
echo ""

# Backup existing specs
if [ -f "${OPENAPI_JSON}" ]; then
    echo "  Backing up existing JSON spec..."
    cp "${OPENAPI_JSON}" "${OPENAPI_JSON}.backup-$(date +%Y%m%d_%H%M%S)"
fi

if [ -f "${OPENAPI_SPEC}" ]; then
    echo "  Backing up existing YAML spec..."
    cp "${OPENAPI_SPEC}" "${OPENAPI_SPEC}.backup-$(date +%Y%m%d_%H%M%S)"
fi

# Download the spec
if curl -f -sL "${OPENAPI_URL}" -o "${OPENAPI_JSON}.tmp" 2>/dev/null; then
    # Verify it's valid JSON
    if jq empty "${OPENAPI_JSON}.tmp" 2>/dev/null; then
        mv "${OPENAPI_JSON}.tmp" "${OPENAPI_JSON}"
        echo -e "${GREEN}  ✓ Successfully downloaded OpenAPI JSON spec${NC}"
        
        # Convert JSON to YAML for easier editing/viewing
        if command -v yq &> /dev/null; then
            echo "  Converting JSON to YAML..."
            yq eval -P "${OPENAPI_JSON}" > "${OPENAPI_SPEC}"
            echo -e "${GREEN}  ✓ Converted to YAML format${NC}"
        else
            echo -e "${YELLOW}  ℹ yq not found, skipping YAML conversion${NC}"
            echo "  Install yq: pip install yq"
            # Use JSON directly
            cp "${OPENAPI_JSON}" "${OPENAPI_SPEC}"
        fi
    else
        echo -e "${RED}  ✗ Downloaded file is not valid JSON${NC}"
        rm -f "${OPENAPI_JSON}.tmp"
        exit 1
    fi
else
    echo -e "${RED}  ✗ Failed to download OpenAPI spec${NC}"
    echo ""
    echo "Please check:"
    echo "  1. Internet connectivity"
    echo "  2. The URL is accessible: ${OPENAPI_URL}"
    echo "  3. No firewall blocking the request"
    echo ""
    exit 1
fi

echo ""

# Step 2: Show changes
echo -e "${YELLOW}Step 2: Reviewing changes...${NC}"
echo ""

if command -v git &> /dev/null; then
    if git rev-parse --git-dir > /dev/null 2>&1; then
        echo "Changes in OpenAPI spec:"
        git diff --stat "${OPENAPI_SPEC}" 2>/dev/null || echo "  (New file or significant changes)"
        echo ""
    fi
fi

# Step 3: Validate the spec (optional but recommended)
echo -e "${YELLOW}Step 3: Validating OpenAPI specification...${NC}"
echo ""

if command -v spectral &> /dev/null; then
    echo "Running Spectral validation..."
    if spectral lint "${OPENAPI_SPEC}" --quiet; then
        echo -e "${GREEN}  ✓ Specification is valid${NC}"
    else
        echo -e "${YELLOW}  ⚠ Specification has some issues (see above)${NC}"
        echo "  Continue anyway? (y/N)"
        read -r response
        if [[ ! "$response" =~ ^[Yy]$ ]]; then
            echo "Aborted by user"
            exit 1
        fi
    fi
else
    echo -e "${YELLOW}  ℹ spectral not found, skipping validation${NC}"
    echo "  Install spectral: npm install -g @stoplight/spectral-cli"
fi

echo ""

# Step 4: Regenerate the API client
echo -e "${YELLOW}Step 4: Regenerating API client...${NC}"
echo ""

# Check if regenerate script exists
REGEN_SCRIPT="${SCRIPT_DIR}/regenerate-client.sh"
if [ -f "${REGEN_SCRIPT}" ]; then
    echo "Running regeneration script..."
    if bash "${REGEN_SCRIPT}"; then
        echo -e "${GREEN}  ✓ API client regenerated successfully${NC}"
    else
        echo -e "${RED}  ✗ Failed to regenerate API client${NC}"
        exit 1
    fi
else
    echo -e "${YELLOW}  ℹ Regeneration script not found at: ${REGEN_SCRIPT}${NC}"
    echo ""
    echo "To regenerate the API client manually:"
    echo "  1. Install OpenAPI Generator: npm install -g @openapitools/openapi-generator-cli"
    echo "  2. Run: openapi-generator-cli generate -i ${OPENAPI_SPEC} -g csharp -o ${SCRIPT_DIR}"
fi

echo ""

# Step 5: Build and test
echo -e "${YELLOW}Step 5: Building and testing...${NC}"
echo ""

if [ -f "${SCRIPT_DIR}/AdGuard.ApiClient.slnx" ]; then
    echo "Building solution..."
    if dotnet build "${SCRIPT_DIR}/AdGuard.ApiClient.slnx" --no-restore; then
        echo -e "${GREEN}  ✓ Build successful${NC}"
    else
        echo -e "${RED}  ✗ Build failed${NC}"
        echo "Please review and fix compilation errors"
        exit 1
    fi
    
    echo ""
    echo "Running tests..."
    if dotnet test "${SCRIPT_DIR}/AdGuard.ApiClient.slnx" --no-build --verbosity quiet; then
        echo -e "${GREEN}  ✓ All tests passed${NC}"
    else
        echo -e "${RED}  ✗ Some tests failed${NC}"
        echo "Please review and fix failing tests"
        exit 1
    fi
else
    echo -e "${YELLOW}  ℹ Solution file not found, skipping build${NC}"
fi

echo ""
echo -e "${GREEN}=========================================="
echo "Update Complete!"
echo -e "==========================================${NC}"
echo ""
echo "Summary:"
echo "  • Downloaded latest OpenAPI spec from AdGuard DNS API"
echo "  • Validated specification"
echo "  • Regenerated C# API client"
echo "  • Built and tested the solution"
echo ""
echo "Next steps:"
echo "  1. Review changes: git diff"
echo "  2. Test the updated client with your application"
echo "  3. Commit the changes: git add . && git commit -m 'Update API client from latest OpenAPI spec'"
echo ""
