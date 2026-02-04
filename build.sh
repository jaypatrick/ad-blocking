#!/bin/bash
# Root-level build script wrapper for ad-blocking repository
# Delegates to build/build.sh

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Delegate to the build directory script
exec "$SCRIPT_DIR/build/build.sh" "$@"
