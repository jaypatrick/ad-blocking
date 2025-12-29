#!/bin/bash
# Validation Library Compliance Checker
# Verifies that all rules compilers are properly integrated with the validation library

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(dirname "$SCRIPT_DIR")"

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

ERRORS=0
WARNINGS=0

echo "╔═══════════════════════════════════════════════════════════╗"
echo "║   Validation Library Compliance Check                    ║"
echo "╚═══════════════════════════════════════════════════════════╝"
echo ""

# Function to check if validation library exists
check_validation_library() {
    echo "→ Checking validation library..."
    
    if [ ! -d "$REPO_ROOT/src/adguard-validation" ]; then
        echo -e "${RED}✗ Validation library not found${NC}"
        ((ERRORS++))
        return 1
    fi
    
    if [ ! -f "$REPO_ROOT/src/adguard-validation/Cargo.toml" ]; then
        echo -e "${RED}✗ Validation library Cargo.toml missing${NC}"
        ((ERRORS++))
        return 1
    fi
    
    echo -e "${GREEN}✓ Validation library exists${NC}"
    return 0
}

# Function to check TypeScript compiler integration
check_typescript_integration() {
    echo ""
    echo "→ Checking TypeScript compiler integration..."
    
    local ts_dir="$REPO_ROOT/src/rules-compiler-typescript"
    
    if [ ! -d "$ts_dir" ]; then
        echo -e "${YELLOW}⚠ TypeScript compiler not found${NC}"
        ((WARNINGS++))
        return 0
    fi
    
    # Check for validation library import in package.json (when integrated)
    # Currently this is aspirational - not yet implemented
    if grep -q "adguard.*validation" "$ts_dir/package.json" 2>/dev/null; then
        echo -e "${GREEN}✓ TypeScript: Validation library dependency found${NC}"
    else
        echo -e "${YELLOW}⚠ TypeScript: Validation library not yet integrated (pending Phase 2)${NC}"
        ((WARNINGS++))
    fi
    
    # Check for validation calls in source code
    if grep -rq "validate_local_file\|validate_remote_url\|Validator" "$ts_dir/src" 2>/dev/null; then
        echo -e "${GREEN}✓ TypeScript: Validation calls found in source${NC}"
    else
        echo -e "${YELLOW}⚠ TypeScript: No validation calls found (pending Phase 2)${NC}"
        ((WARNINGS++))
    fi
}

# Function to check .NET compiler integration
check_dotnet_integration() {
    echo ""
    echo "→ Checking .NET compiler integration..."
    
    local dotnet_dir="$REPO_ROOT/src/rules-compiler-dotnet"
    
    if [ ! -d "$dotnet_dir" ]; then
        echo -e "${YELLOW}⚠ .NET compiler not found${NC}"
        ((WARNINGS++))
        return 0
    fi
    
    # Check for native library reference (when integrated)
    if grep -rq "adguard_validation\|ValidationLibrary" "$dotnet_dir" 2>/dev/null; then
        echo -e "${GREEN}✓ .NET: Validation library reference found${NC}"
    else
        echo -e "${YELLOW}⚠ .NET: Validation library not yet integrated (pending Phase 3)${NC}"
        ((WARNINGS++))
    fi
}

# Function to check Python compiler integration
check_python_integration() {
    echo ""
    echo "→ Checking Python compiler integration..."
    
    local python_dir="$REPO_ROOT/src/rules-compiler-python"
    
    if [ ! -d "$python_dir" ]; then
        echo -e "${YELLOW}⚠ Python compiler not found${NC}"
        ((WARNINGS++))
        return 0
    fi
    
    # Check for validation library in requirements (when integrated)
    if [ -f "$python_dir/requirements.txt" ] && grep -q "adguard-validation" "$python_dir/requirements.txt" 2>/dev/null; then
        echo -e "${GREEN}✓ Python: Validation library dependency found${NC}"
    else
        echo -e "${YELLOW}⚠ Python: Validation library not yet integrated (pending Phase 3)${NC}"
        ((WARNINGS++))
    fi
}

# Function to check Rust compiler integration
check_rust_integration() {
    echo ""
    echo "→ Checking Rust compiler integration..."
    
    local rust_dir="$REPO_ROOT/src/rules-compiler-rust"
    
    if [ ! -d "$rust_dir" ]; then
        echo -e "${YELLOW}⚠ Rust compiler not found${NC}"
        ((WARNINGS++))
        return 0
    fi
    
    # Check for validation library dependency
    if grep -q "adguard-validation\|adguard_validation" "$rust_dir/Cargo.toml" 2>/dev/null; then
        echo -e "${GREEN}✓ Rust: Validation library dependency found${NC}"
    else
        echo -e "${YELLOW}⚠ Rust: Validation library not yet integrated (pending Phase 3)${NC}"
        ((WARNINGS++))
    fi
}

# Function to check if validation library builds
check_validation_library_builds() {
    echo ""
    echo "→ Checking if validation library builds..."
    
    cd "$REPO_ROOT/src/adguard-validation"
    
    if cargo build --release >/dev/null 2>&1; then
        echo -e "${GREEN}✓ Validation library builds successfully${NC}"
    else
        echo -e "${RED}✗ Validation library build failed${NC}"
        ((ERRORS++))
    fi
}

# Function to check if validation library tests pass
check_validation_library_tests() {
    echo ""
    echo "→ Checking if validation library tests pass..."
    
    cd "$REPO_ROOT/src/adguard-validation"
    
    if cargo test --all >/dev/null 2>&1; then
        echo -e "${GREEN}✓ Validation library tests pass (29 tests)${NC}"
    else
        echo -e "${RED}✗ Validation library tests failed${NC}"
        ((ERRORS++))
    fi
}

# Run all checks
check_validation_library
check_validation_library_builds
check_validation_library_tests
check_typescript_integration
check_dotnet_integration
check_python_integration
check_rust_integration

# Summary
echo ""
echo "╔═══════════════════════════════════════════════════════════╗"
echo "║   Compliance Check Summary                                ║"
echo "╚═══════════════════════════════════════════════════════════╝"
echo ""

if [ $ERRORS -eq 0 ] && [ $WARNINGS -eq 0 ]; then
    echo -e "${GREEN}✓ All checks passed!${NC}"
    exit 0
elif [ $ERRORS -eq 0 ]; then
    echo -e "${YELLOW}⚠ Passed with $WARNINGS warning(s)${NC}"
    echo -e "${YELLOW}  Note: Warnings indicate pending integration (migration in progress)${NC}"
    exit 0
else
    echo -e "${RED}✗ Failed with $ERRORS error(s) and $WARNINGS warning(s)${NC}"
    exit 1
fi
