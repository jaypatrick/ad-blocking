# Shell Scripts for Rules Compiler

Cross-platform shell scripts for compiling AdGuard filter rules using `@adguard/hostlist-compiler`.

## Scripts

| Script | Platform | Description |
|--------|----------|-------------|
| `compile-rules.sh` | Linux, macOS | Bash script with full feature support |
| `compile-rules.ps1` | All (PowerShell Core) | PowerShell 7+ cross-platform script |
| `compile-rules.cmd` | Windows | Batch wrapper for Windows users |

## Prerequisites

| Requirement | Version | Notes |
|-------------|---------|-------|
| Node.js | 18+ | Required for hostlist-compiler |
| hostlist-compiler | Latest | `npm install -g @adguard/hostlist-compiler` |

**For YAML support (optional):**
- **Bash:** `yq` or Python with `PyYAML`
- **PowerShell:** `powershell-yaml` module or Python with `PyYAML`

**For TOML support (optional):**
- Python 3.11+ (built-in `tomllib`) or Python with `toml` package

## Usage

### Bash (Linux/macOS)

```bash
# Make executable (first time only)
chmod +x src/rules-compiler-shell/compile-rules.sh

# Run with defaults
./src/rules-compiler-shell/compile-rules.sh

# Use YAML configuration
./src/rules-compiler-shell/compile-rules.sh -c src/rules-compiler-typescript/compiler-config.yaml

# Compile and copy to rules directory
./src/rules-compiler-shell/compile-rules.sh -r

# Show version info
./src/rules-compiler-shell/compile-rules.sh -v

# Show help
./src/rules-compiler-shell/compile-rules.sh -h
```

### PowerShell Core (All Platforms)

```powershell
# Run with defaults
./src/rules-compiler-shell/compile-rules.ps1

# Use YAML configuration
./src/rules-compiler-shell/compile-rules.ps1 -ConfigPath src/rules-compiler-typescript/compiler-config.yaml

# Compile and copy to rules directory
./src/rules-compiler-shell/compile-rules.ps1 -CopyToRules

# Short form
./src/rules-compiler-shell/compile-rules.ps1 -c config.yaml -r

# Show version info
./src/rules-compiler-shell/compile-rules.ps1 -Version

# Show help
./src/rules-compiler-shell/compile-rules.ps1 -Help
```

### Windows Batch

```cmd
REM Run with defaults
src\rules-compiler-shell\compile-rules.cmd

REM Use specific configuration
src\rules-compiler-shell\compile-rules.cmd -c config.json

REM Compile and copy to rules
src\rules-compiler-shell\compile-rules.cmd -c config.json -r

REM Show version
src\rules-compiler-shell\compile-rules.cmd -v
```

## Command-Line Options

| Option | Short | Description |
|--------|-------|-------------|
| `--config PATH` | `-c` | Path to configuration file |
| `--output PATH` | `-o` | Path to output file |
| `--copy-to-rules` | `-r` | Copy output to rules directory |
| `--format FORMAT` | `-f` | Force format (json, yaml, toml) |
| `--version` | `-v` | Show version information |
| `--help` | `-h` | Show help message |
| `--debug` | `-d` | Enable debug output |

## Configuration Formats

All scripts support multiple configuration file formats:

### JSON (Default)
```json
{
  "name": "My Filter Rules",
  "version": "1.0.0",
  "sources": [
    { "name": "Local", "source": "./rules.txt", "type": "adblock" }
  ],
  "transformations": ["Deduplicate", "Validate"]
}
```

### YAML
```yaml
name: My Filter Rules
version: 1.0.0
sources:
  - name: Local
    source: ./rules.txt
    type: adblock
transformations:
  - Deduplicate
  - Validate
```

### TOML
```toml
name = "My Filter Rules"
version = "1.0.0"
transformations = ["Deduplicate", "Validate"]

[[sources]]
name = "Local"
source = "./rules.txt"
type = "adblock"
```

## Output

Successful compilation displays:
- **Rule Count:** Number of compiled rules
- **Output Path:** Path to the generated file
- **Hash:** SHA-384 hash of the output (first 32 characters)
- **Elapsed:** Compilation time in milliseconds

Example:
```
[INFO] 2024-01-15T10:30:00Z - Starting filter compilation...
[INFO] 2024-01-15T10:30:02Z - Compilation successful!

Results:
  Rule Count:  11,707
  Output Path: /path/to/output/compiled-20240115-103000.txt
  Hash:        a1b2c3d4e5f6...
  Elapsed:     2,145ms
```

## Exit Codes

| Code | Description |
|------|-------------|
| 0 | Success |
| 1 | Error (configuration not found, compilation failed, etc.) |

## Environment Variables

| Variable | Description |
|----------|-------------|
| `DEBUG` | Set to any value to enable debug logging |

## Cross-Platform Notes

### Path Handling
- Scripts use platform-appropriate path separators
- Both absolute and relative paths are supported
- Tilde expansion (`~`) works on Unix systems

### Character Encoding
- All file operations use UTF-8 encoding
- Unicode characters in rules are fully supported

### Compiler Detection
1. First looks for globally installed `hostlist-compiler`
2. Falls back to `npx @adguard/hostlist-compiler` if not found

## Troubleshooting

### hostlist-compiler not found
```bash
# Install globally
npm install -g @adguard/hostlist-compiler

# Or ensure npx is available
which npx
```

### Permission denied (Unix)
```bash
chmod +x src/rules-compiler-shell/compile-rules.sh
```

### YAML parsing fails
```bash
# Install yq (recommended)
brew install yq          # macOS
sudo apt install yq      # Debian/Ubuntu

# Or use Python with PyYAML
pip install pyyaml
```

### TOML parsing fails
```bash
# Requires Python 3.11+ or toml package
pip install toml
```

## Integration with CI/CD

These scripts can be integrated into GitHub Actions or other CI systems:

```yaml
# .github/workflows/compile.yml
jobs:
  compile:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-node@v4
        with:
          node-version: '20'
      - run: npm install -g @adguard/hostlist-compiler
      - run: ./src/rules-compiler-shell/compile-rules.sh -c src/rules-compiler-typescript/compiler-config.json -r
```

## Related

- [TypeScript API](../../src/rules-compiler-typescript/README.md) - Node.js/TypeScript implementation
- [PowerShell Module](../adguard-api-powershell/README.md) - Full-featured PowerShell module
- [.NET Library](../../src/rules-compiler-dotnet/README.md) - C# class library and console app

## License

GPLv3 - See [LICENSE](../../LICENSE) for details.
