# Rules Compiler (Python)

Python API for compiling AdGuard filter rules using `@adguard/hostlist-compiler`.

## Installation

```bash
# Install from source
cd src/rules-compiler-python
pip install -e .

# Install with development dependencies
pip install -e ".[dev]"
```

## Prerequisites

| Requirement | Version | Notes |
|-------------|---------|-------|
| Python | 3.9+ | Required |
| Node.js | 18+ | Required for hostlist-compiler |
| hostlist-compiler | Latest | `npm install -g @adguard/hostlist-compiler` |

## CLI Usage

```bash
# Use default config (compiler-config.json)
rules-compiler

# Use specific configuration file
rules-compiler -c compiler-config.yaml

# Compile and copy to rules directory
rules-compiler -c config.json -r

# Show version info
rules-compiler -v

# Enable debug output
rules-compiler -c config.yaml -d

# Show help
rules-compiler -h
```

### CLI Options

| Option | Short | Description |
|--------|-------|-------------|
| `--config PATH` | `-c` | Path to configuration file |
| `--output PATH` | `-o` | Path to output file |
| `--copy-to-rules` | `-r` | Copy output to rules directory |
| `--format FORMAT` | `-f` | Force format (json, yaml, toml) |
| `--version` | `-v` | Show version information |
| `--debug` | `-d` | Enable debug output |
| `--help` | `-h` | Show help message |

## Python API

### Basic Usage

```python
from rules_compiler import RulesCompiler

# Create compiler
compiler = RulesCompiler()

# Compile rules
result = compiler.compile("compiler-config.yaml", copy_to_rules=True)

if result.success:
    print(f"Compiled {result.rule_count} rules")
    print(f"Output: {result.output_path}")
else:
    print(f"Error: {result.error_message}")
```

### Reading Configuration

```python
from rules_compiler import read_configuration, ConfigurationFormat

# Auto-detect format from extension
config = read_configuration("config.yaml")
print(f"Name: {config.name}")
print(f"Sources: {len(config.sources)}")

# Force specific format
config = read_configuration("config.txt", format=ConfigurationFormat.YAML)
```

### Version Information

```python
from rules_compiler import get_version_info

info = get_version_info()
print(f"Module: {info.module_version}")
print(f"Python: {info.python_version}")
print(f"Node.js: {info.node_version}")
print(f"Platform: {info.platform.os_name}")
```

### Using the Compiler Class

```python
from rules_compiler import RulesCompiler, ConfigurationFormat

compiler = RulesCompiler(debug=True)

# Read and inspect configuration
config = compiler.read_config("config.yaml")
print(f"Will compile {len(config.sources)} sources")

# Compile with options
result = compiler.compile(
    config_path="config.yaml",
    output_path="my-rules.txt",
    copy_to_rules=True,
    format=ConfigurationFormat.YAML,
)

# Access result details
print(f"Success: {result.success}")
print(f"Rules: {result.rule_count}")
print(f"Hash: {result.output_hash}")
print(f"Time: {result.elapsed_ms}ms")
```

## Configuration Formats

### JSON

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

## Running Tests

```bash
cd src/rules-compiler-python

# Install dev dependencies
pip install -e ".[dev]"

# Run tests
pytest

# Run with coverage
pytest --cov=rules_compiler --cov-report=term-missing

# Run specific test file
pytest tests/test_config.py

# Run with verbose output
pytest -v
```

## Type Checking

```bash
# Run mypy
mypy rules_compiler
```

## Linting

```bash
# Run ruff
ruff check rules_compiler

# Auto-fix issues
ruff check --fix rules_compiler
```

## API Reference

### Classes

| Class | Description |
|-------|-------------|
| `RulesCompiler` | Main compiler class |
| `CompilerResult` | Result of a compilation operation |
| `CompilerConfiguration` | Configuration file model |
| `FilterSource` | Source filter list definition |
| `VersionInfo` | Component version information |
| `PlatformInfo` | Platform-specific information |

### Enums

| Enum | Values |
|------|--------|
| `ConfigurationFormat` | `JSON`, `YAML`, `TOML` |

### Functions

| Function | Description |
|----------|-------------|
| `compile_rules()` | Compile filter rules (functional API) |
| `read_configuration()` | Read configuration from file |
| `detect_format()` | Detect format from file extension |
| `to_json()` | Convert configuration to JSON |
| `get_version_info()` | Get version information |

## License

GPLv3 - See [LICENSE](../../LICENSE) for details.
