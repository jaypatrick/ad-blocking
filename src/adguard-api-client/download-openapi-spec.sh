#!/usr/bin/env bash
# Download AdGuard DNS OpenAPI Specification
# This script attempts to download the latest OpenAPI spec from various sources

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
API_DIR="${SCRIPT_DIR}/api"
OPENAPI_JSON="${API_DIR}/openapi.json"
OPENAPI_YAML="${API_DIR}/openapi.yaml"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}=========================================="
echo "AdGuard DNS OpenAPI Spec Downloader"
echo -e "==========================================${NC}"
echo ""

# Function to try downloading from a URL
try_download() {
    local url=$1
    local description=$2
    local output_file=$3
    
    echo -e "${YELLOW}Trying: ${description}${NC}"
    echo "  URL: ${url}"
    
    if curl -f -sL "${url}" -o "${output_file}.tmp" 2>/dev/null; then
        # Check if the downloaded file is valid JSON or YAML
        if head -n 1 "${output_file}.tmp" | grep -qE "(openapi:|swagger:|\{)" 2>/dev/null; then
            echo -e "${GREEN}  ✓ Successfully downloaded!${NC}"
            mv "${output_file}.tmp" "${output_file}"
            return 0
        else
            echo -e "${RED}  ✗ Downloaded file doesn't appear to be a valid OpenAPI spec${NC}"
            rm -f "${output_file}.tmp"
        fi
    else
        echo -e "${RED}  ✗ Failed to download${NC}"
        rm -f "${output_file}.tmp"
    fi
    
    echo ""
    return 1
}

# Ensure API directory exists
mkdir -p "${API_DIR}"

# Backup existing specs if they exist
if [ -f "${OPENAPI_JSON}" ]; then
    echo -e "${YELLOW}Backing up existing JSON OpenAPI spec...${NC}"
    cp "${OPENAPI_JSON}" "${OPENAPI_JSON}.backup-$(date +%Y%m%d_%H%M%S)"
    echo -e "${GREEN}Backup created${NC}"
    echo ""
fi

if [ -f "${OPENAPI_YAML}" ]; then
    echo -e "${YELLOW}Backing up existing YAML OpenAPI spec...${NC}"
    cp "${OPENAPI_YAML}" "${OPENAPI_YAML}.backup-$(date +%Y%m%d_%H%M%S)"
    echo -e "${GREEN}Backup created${NC}"
    echo ""
fi

# Try various potential URLs
DOWNLOAD_SUCCESS=false

# Try JSON endpoints first (primary format)
JSON_URLS=(
    "https://api.adguard-dns.io/swagger/openapi.json|Official OpenAPI JSON (Primary)"
    "https://api.adguard-dns.io/static/openapi.json|Static OpenAPI JSON"
    "https://api.adguard-dns.io/swagger/v1/swagger.json|Swagger UI JSON"
    "https://api.adguard-dns.io/static/swagger.json|Static Swagger JSON"
)

for url_entry in "${JSON_URLS[@]}"; do
    IFS='|' read -r url description <<< "${url_entry}"
    if try_download "${url}" "${description}" "${OPENAPI_JSON}"; then
        DOWNLOAD_SUCCESS=true
        
        # Optionally convert to YAML for readability
        if command -v yq &> /dev/null; then
            echo -e "${YELLOW}Converting JSON to YAML for readability...${NC}"
            if yq eval -P "${OPENAPI_JSON}" > "${OPENAPI_YAML}"; then
                echo -e "${GREEN}  ✓ Converted to YAML format${NC}"
            fi
        fi
        break
    fi
done

# If JSON download failed, try YAML endpoints
if [ "${DOWNLOAD_SUCCESS}" = false ]; then
    YAML_URLS=(
        "https://api.adguard-dns.io/swagger/openapi.yaml|Official OpenAPI YAML"
        "https://api.adguard-dns.io/static/swagger.yaml|Static Swagger YAML"
        "https://api.adguard-dns.io/static/openapi.yaml|Static OpenAPI YAML"
        "https://api.adguard-dns.io/swagger/v1/swagger.yaml|Swagger UI YAML"
        "https://api.adguard-dns.io/api-docs|API Docs endpoint"
        "https://api.adguard-dns.io/oapi/openapi.yaml|OAPI directory YAML"
        "https://api.adguard-dns.io/oapi/v1/openapi.yaml|OAPI v1 directory YAML"
        "https://raw.githubusercontent.com/AdguardTeam/AdGuardDNS/master/openapi/openapi.yaml|GitHub AdGuardDNS repo"
        "https://raw.githubusercontent.com/AdguardTeam/AdGuardDNS/master/openapi.yaml|GitHub AdGuardDNS repo (root)"
        "https://raw.githubusercontent.com/AdguardTeam/DnsWebApp/main/api/openapi.yaml|GitHub DnsWebApp repo"
    )
    
    for url_entry in "${YAML_URLS[@]}"; do
        IFS='|' read -r url description <<< "${url_entry}"
        if try_download "${url}" "${description}" "${OPENAPI_YAML}"; then
            DOWNLOAD_SUCCESS=true
            
            # Convert YAML to JSON (primary format)
            if command -v yq &> /dev/null; then
                echo -e "${YELLOW}Converting YAML to JSON (primary format)...${NC}"
                if yq eval -o=json "${OPENAPI_YAML}" > "${OPENAPI_JSON}"; then
                    echo -e "${GREEN}  ✓ Converted to JSON format${NC}"
                fi
            else
                echo -e "${YELLOW}  ℹ yq not found, using YAML only${NC}"
                echo "  Install yq to convert to JSON: pip install yq"
            fi
            break
        fi
    done
fi

# Check if download was successful
if [ "${DOWNLOAD_SUCCESS}" = true ]; then
    echo -e "${GREEN}=========================================="
    echo "Download Successful!"
    echo -e "==========================================${NC}"
    echo ""
    echo "OpenAPI specification downloaded to:"
    echo "  ${OPENAPI_JSON} (primary)"
    if [ -f "${OPENAPI_YAML}" ]; then
        echo "  ${OPENAPI_YAML} (for readability)"
    fi
    echo ""
    echo "Next steps:"
    echo "  1. Verify the specification: spectral lint ${OPENAPI_JSON}"
    echo "  2. Review changes: git diff ${OPENAPI_JSON}"
    echo "  3. Regenerate API client: ./regenerate-client.sh"
    echo ""
else
    echo -e "${RED}=========================================="
    echo "Download Failed"
    echo -e "==========================================${NC}"
    echo ""
    echo -e "${YELLOW}The OpenAPI specification is not available at any known public URL.${NC}"
    echo ""
    echo "To obtain the latest OpenAPI specification:"
    echo ""
    echo "1. Check AdGuard DNS API Documentation:"
    echo "   https://adguard-dns.io/kb/private-dns/api/overview/"
    echo ""
    echo "2. Contact AdGuard support:"
    echo "   https://adguard-dns.io/support/"
    echo ""
    echo "3. Check your AdGuard DNS account dashboard for API documentation"
    echo ""
    echo "4. If you have the spec URL, download it manually:"
    echo "   curl -o ${OPENAPI_JSON} <URL>"
    echo ""
    
    # Restore backup if download failed and backup exists
    if [ -f "${OPENAPI_JSON}.backup"* ]; then
        echo -e "${YELLOW}Restoring previous OpenAPI spec...${NC}"
        # Find the most recent backup
        LATEST_BACKUP=$(ls -t "${OPENAPI_JSON}.backup"* 2>/dev/null | head -n 1)
        if [ -f "${LATEST_BACKUP}" ]; then
            cp "${LATEST_BACKUP}" "${OPENAPI_JSON}"
            echo -e "${GREEN}Previous spec restored${NC}"
        fi
    fi
    
    echo ""
    exit 1
fi
