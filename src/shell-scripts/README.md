# Shell Scripts for AdGuard Filter Compiler

This directory contains shell script implementations for compiling AdGuard filter rules. The scripts provide a cross-platform interface to the `@adguard/hostlist-compiler` CLI tool.

## Directory Structure

```
shell-scripts/
├── bash/           # Bash-specific scripts (Linux, macOS, WSL)
│   ├── compile-rules.sh
│   └── README.md
└── zsh/            # Zsh-specific scripts (macOS default, Linux)
    ├── compile-rules.zsh
    └── README.md
```

## Available Scripts

### Bash (`bash/compile-rules.sh`)
Standard Bourne-Again Shell implementation, compatible with:
- Linux (all distributions)
- macOS
- Windows Subsystem for Linux (WSL)
- Git Bash (Windows)
- Any POSIX-compliant shell with bash compatibility

**Features:**
- Full environment variable support
- JSON, YAML, and TOML configuration formats
- Color-coded output
- Debug mode
- Cross-platform path handling

**Documentation:** See [bash/README.md](bash/README.md)

### Zsh (`zsh/compile-rules.zsh`)
Z Shell implementation optimized for modern shell features:
- macOS (default shell since Catalina)
- Linux with zsh installed
- Oh My Zsh users

**Features:**
- Zsh-specific optimizations
- Extended globbing support
- Rich parameter handling
- Same environment variable support as bash
- Zsh-native path manipulation

**Documentation:** See [zsh/README.md](zsh/README.md)

## Environment Variables

All scripts support the following environment variables:

| Variable | Description | Default |
|----------|-------------|---------|
| `ADGUARD_COMPILER_CONFIG` | Path to configuration file | `compiler-config.json` |
| `ADGUARD_COMPILER_OUTPUT` | Output file path | `output/compiled-{timestamp}.txt` |
| `ADGUARD_COMPILER_RULES_DIR` | Rules directory for copying | `../../data` |
| `ADGUARD_COMPILER_FORMAT` | Force config format (json/yaml/toml) | Auto-detect |
| `ADGUARD_COMPILER_VERBOSE` | Enable verbose output (1/true) | `false` |
| `ADGUARD_COMPILER_COPY_TO_RULES` | Auto-copy to rules dir (1/true) | `false` |
| `DEBUG` | Enable debug mode (1/true) | `false` |

### Priority Order
```
CLI Parameters > Config Files > Environment Variables > Defaults
```

## Quick Start

### Using Bash
```bash
# Basic usage
cd src/shell-scripts/bash
./compile-rules.sh

# With environment variables
export ADGUARD_COMPILER_CONFIG="config.yaml"
export ADGUARD_COMPILER_COPY_TO_RULES=true
./compile-rules.sh

# With CLI parameters
./compile-rules.sh -c config.yaml -r -d
```

### Using Zsh
```zsh
# Basic usage
cd src/shell-scripts/zsh
./compile-rules.zsh

# With environment variables
export ADGUARD_COMPILER_CONFIG="config.yaml"
export ADGUARD_COMPILER_COPY_TO_RULES=true
./compile-rules.zsh

# With CLI parameters
./compile-rules.zsh -c config.yaml -r -d
```

## Common Usage Examples

### Example 1: Compile with YAML Config
```bash
./compile-rules.sh -c my-config.yaml
```

### Example 2: Compile and Copy to Rules Directory
```bash
./compile-rules.sh -c config.json -r
```

### Example 3: Use Environment Variables
```bash
export ADGUARD_COMPILER_CONFIG="production-config.yaml"
export ADGUARD_COMPILER_OUTPUT="/var/www/data/output/filter.txt"
export ADGUARD_COMPILER_COPY_TO_RULES=true

./compile-rules.sh
```

### Example 4: Debug Mode
```bash
DEBUG=1 ./compile-rules.sh -c config.json
```

## Configuration Formats

All scripts support multiple configuration formats:

### JSON (`.json`)
```json
{
  "name": "My Filter List",
  "version": "1.0.0",
  "sources": [
    {
      "name": "Source 1",
      "source": "https://example.com/rules.txt"
    }
  ]
}
```

### YAML (`.yaml`, `.yml`)
Requires `yq` or Python with PyYAML:
```yaml
name: My Filter List
version: 1.0.0
sources:
  - name: Source 1
    source: https://example.com/rules.txt
```

### TOML (`.toml`)
Requires Python 3.11+ or the `toml` package:
```toml
name = "My Filter List"
version = "1.0.0"

[[sources]]
name = "Source 1"
source = "https://example.com/rules.txt"
```

## Prerequisites

### Required
- **Bash** (for bash scripts) or **Zsh** (for zsh scripts)
- **Node.js** 18+ 
- **@adguard/hostlist-compiler**: `npm install -g @adguard/hostlist-compiler`

### Optional (for YAML/TOML support)
- **yq** - For YAML parsing: `brew install yq` (macOS) or download from [GitHub](https://github.com/mikefarah/yq)
- **Python 3** with PyYAML - For YAML: `pip install pyyaml`
- **Python 3.11+** or `toml` package - For TOML: `pip install toml`

## Platform Support

| Platform | Bash Script | Zsh Script | Notes |
|----------|-------------|------------|-------|
| Linux | ✅ | ✅ | Native support |
| macOS | ✅ | ✅ | Zsh is default shell (macOS 10.15+) |
| Windows (WSL) | ✅ | ✅ | Full support via WSL |
| Windows (Git Bash) | ✅ | ⚠️ | Bash recommended |
| FreeBSD | ✅ | ✅ | Should work, untested |

## Choosing Between Bash and Zsh

**Use Bash when:**
- Maximum compatibility is needed
- Deploying to servers/containers
- CI/CD pipelines
- You're on Linux

**Use Zsh when:**
- On macOS (default shell)
- Using Oh My Zsh
- Want modern shell features
- Interactive use

## Troubleshooting

### "hostlist-compiler: command not found"
```bash
npm install -g @adguard/hostlist-compiler
```

### YAML parsing fails
```bash
# Install yq
brew install yq  # macOS
# OR install PyYAML
pip install pyyaml
```

### TOML parsing fails
```bash
# For Python 3.11+, tomllib is built-in
# For older versions:
pip install toml
```

### Permission denied
```bash
chmod +x compile-rules.sh  # or compile-rules.zsh
```

## Related Documentation

- [Environment Variables Reference](../../docs/ENVIRONMENT_VARIABLES.md)
- [PowerShell Modules](../powershell-modules/README.md)
- [Main Project README](../../README.md)

## Contributing

When contributing to shell scripts:
1. Test on both bash and zsh when possible
2. Use POSIX-compliant features where applicable
3. Add platform-specific code only when necessary
4. Update both README files for changes
5. Test on Linux and macOS
6. Include environment variable support

## License

GPLv3 - See [LICENSE](../../LICENSE) for details.
