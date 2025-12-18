# Configuration Reference

All rules compilers in this repository use the same configuration schema based on [@adguard/hostlist-compiler](https://github.com/AdguardTeam/HostlistCompiler).

## Supported Formats

| Format | Extension | Library |
|--------|-----------|---------|
| JSON | `.json` | Native |
| YAML | `.yaml`, `.yml` | yaml (Node), YamlDotNet (.NET), PyYAML (Python), serde_yaml (Rust) |
| TOML | `.toml` | @iarna/toml (Node), Tomlyn (.NET), tomli (Python), toml (Rust) |

## Configuration Schema

### Root-Level Properties

| Property | Type | Required | Default | Description |
|----------|------|----------|---------|-------------|
| `name` | string | **Yes** | - | Name of the compiled filter list |
| `description` | string | No | - | Description of the filter list |
| `homepage` | string | No | - | Homepage URL for the filter list |
| `license` | string | No | - | License identifier (e.g., "GPL-3.0") |
| `version` | string | No | - | Version number of the filter list |
| `sources` | array | **Yes** | - | List of filter sources to compile |
| `transformations` | array | No | `[]` | Global transformations to apply |
| `inclusions` | array | No | `[]` | Global inclusion patterns |
| `inclusions_sources` | array | No | `[]` | Files containing inclusion patterns |
| `exclusions` | array | No | `[]` | Global exclusion patterns |
| `exclusions_sources` | array | No | `[]` | Files containing exclusion patterns |

### Source Properties

Each source in the `sources` array supports:

| Property | Type | Required | Default | Description |
|----------|------|----------|---------|-------------|
| `source` | string | **Yes** | - | URL or local file path to the filter list |
| `name` | string | No | - | Human-readable name for this source |
| `type` | string | No | `"adblock"` | Format: `"adblock"` or `"hosts"` |
| `transformations` | array | No | `[]` | Source-specific transformations |
| `inclusions` | array | No | `[]` | Source-specific inclusion patterns |
| `inclusions_sources` | array | No | `[]` | Files with source inclusion patterns |
| `exclusions` | array | No | `[]` | Source-specific exclusion patterns |
| `exclusions_sources` | array | No | `[]` | Files with source exclusion patterns |

## Transformations

Transformations modify the filter rules during compilation. They are always applied in this fixed order, regardless of configuration order:

| Transformation | Description | Use Case |
|---------------|-------------|----------|
| `RemoveComments` | Removes all comment lines (`!` or `#`) | Reduce file size |
| `Compress` | Converts hosts format to adblock syntax | Hosts file sources |
| `RemoveModifiers` | Removes unsupported modifiers from rules | AdGuard DNS compatibility |
| `Validate` | Removes dangerous or incompatible rules | Safety and compatibility |
| `ValidateAllowIp` | Like Validate but allows IP address rules | When IP rules needed |
| `Deduplicate` | Removes duplicate rules | Reduce file size |
| `InvertAllow` | Converts `@@` exception rules to blocking rules | Strict blocking |
| `RemoveEmptyLines` | Removes blank lines | Clean output |
| `TrimLines` | Trims leading/trailing whitespace | Clean output |
| `InsertFinalNewLine` | Ensures file ends with newline | POSIX compliance |
| `ConvertToAscii` | Converts IDN domains to punycode | Compatibility |

### Recommended Transformation Sets

#### For AdGuard DNS

```yaml
transformations:
  - Validate
  - RemoveModifiers
  - Deduplicate
  - RemoveEmptyLines
  - InsertFinalNewLine
```

#### For Hosts File Sources

```yaml
transformations:
  - Compress
  - Validate
  - Deduplicate
  - RemoveEmptyLines
  - InsertFinalNewLine
```

#### For Maximum Compatibility

```yaml
transformations:
  - RemoveComments
  - RemoveModifiers
  - Validate
  - Deduplicate
  - TrimLines
  - RemoveEmptyLines
  - InsertFinalNewLine
  - ConvertToAscii
```

## Pattern Matching

Inclusion and exclusion patterns filter which rules are kept or removed.

### Pattern Types

| Type | Syntax | Example | Matches |
|------|--------|---------|---------|
| Plain string | `text` | `google.com` | Exact match |
| Wildcard | `*text*` | `*tracking*` | Contains "tracking" |
| Prefix wildcard | `*.text` | `*.example.com` | Ends with ".example.com" |
| Suffix wildcard | `text*` | `analytics*` | Starts with "analytics" |
| Regex | `/pattern/` | `/^ad[0-9]+\./` | Regex match |
| Comment | `! text` | `! ignore this` | Ignored |

### Pattern Examples

#### Exclusions (Remove matching rules)

```yaml
exclusions:
  - "*.google.com"        # Remove rules for Google subdomains
  - "*analytics*"         # Remove analytics-related rules
  - "/^(www\.)?facebook\.com$/"  # Remove Facebook rules
```

#### Inclusions (Keep only matching rules)

```yaml
inclusions:
  - "*ad*"                # Keep only ad-related rules
  - "*tracker*"           # Keep only tracker rules
  - "*.doubleclick.net"   # Keep DoubleClick rules
```

### Pattern Files

Instead of inline patterns, you can reference external files:

```yaml
exclusions_sources:
  - whitelist.txt
  - my-allowlist.txt

inclusions_sources:
  - blocklist-patterns.txt
```

Pattern file format (one pattern per line, comments with `!`):

```
! This is a comment
*.google.com
*analytics*
/tracking\d+\./
```

## Example Configurations

### JSON

```json
{
  "name": "My Filter List",
  "description": "Custom ad-blocking filter",
  "version": "1.0.0",
  "homepage": "https://example.com/filters",
  "license": "GPL-3.0",
  "sources": [
    {
      "name": "Local Rules",
      "source": "rules/local.txt",
      "type": "adblock"
    },
    {
      "name": "EasyList",
      "source": "https://easylist.to/easylist/easylist.txt",
      "type": "adblock",
      "transformations": ["RemoveModifiers", "Validate"]
    },
    {
      "name": "Steven Black Hosts",
      "source": "https://raw.githubusercontent.com/StevenBlack/hosts/master/hosts",
      "type": "hosts",
      "transformations": ["Compress", "Validate"]
    }
  ],
  "transformations": [
    "Deduplicate",
    "RemoveEmptyLines",
    "InsertFinalNewLine"
  ],
  "exclusions": [
    "*.google.com",
    "/analytics/"
  ]
}
```

### YAML

```yaml
name: My Filter List
description: Custom ad-blocking filter
version: "1.0.0"
homepage: https://example.com/filters
license: GPL-3.0

sources:
  - name: Local Rules
    source: rules/local.txt
    type: adblock

  - name: EasyList
    source: https://easylist.to/easylist/easylist.txt
    type: adblock
    transformations:
      - RemoveModifiers
      - Validate

  - name: Steven Black Hosts
    source: https://raw.githubusercontent.com/StevenBlack/hosts/master/hosts
    type: hosts
    transformations:
      - Compress
      - Validate

transformations:
  - Deduplicate
  - RemoveEmptyLines
  - InsertFinalNewLine

exclusions:
  - "*.google.com"
  - "/analytics/"
```

### TOML

```toml
name = "My Filter List"
description = "Custom ad-blocking filter"
version = "1.0.0"
homepage = "https://example.com/filters"
license = "GPL-3.0"

transformations = ["Deduplicate", "RemoveEmptyLines", "InsertFinalNewLine"]
exclusions = ["*.google.com", "/analytics/"]

[[sources]]
name = "Local Rules"
source = "rules/local.txt"
type = "adblock"

[[sources]]
name = "EasyList"
source = "https://easylist.to/easylist/easylist.txt"
type = "adblock"
transformations = ["RemoveModifiers", "Validate"]

[[sources]]
name = "Steven Black Hosts"
source = "https://raw.githubusercontent.com/StevenBlack/hosts/master/hosts"
type = "hosts"
transformations = ["Compress", "Validate"]
```

## Advanced Configuration

### Source-Specific Overrides

Each source can have its own transformations, inclusions, and exclusions that override or extend global settings:

```yaml
name: Advanced Filter

# Global transformations applied to all sources
transformations:
  - Deduplicate
  - InsertFinalNewLine

# Global exclusions applied to all sources
exclusions:
  - "*.google.com"

sources:
  - name: Strict Source
    source: https://example.com/strict.txt
    # Source-specific transformations (in addition to global)
    transformations:
      - RemoveComments
      - Validate
    # Source-specific exclusions (in addition to global)
    exclusions:
      - "*.facebook.com"

  - name: Permissive Source
    source: https://example.com/permissive.txt
    # Only source-specific inclusions
    inclusions:
      - "*ad*"
      - "*tracker*"
```

### Using External Pattern Files

```yaml
name: Filter with External Patterns

sources:
  - name: Main List
    source: https://example.com/list.txt

exclusions_sources:
  - config/global-whitelist.txt
  - config/user-whitelist.txt

inclusions_sources:
  - config/must-block.txt
```

### Multiple Sources with Different Types

```yaml
name: Combined Filter

sources:
  # Adblock format sources
  - name: EasyList
    source: https://easylist.to/easylist/easylist.txt
    type: adblock

  - name: EasyPrivacy
    source: https://easylist.to/easylist/easyprivacy.txt
    type: adblock

  # Hosts format sources (will be converted to adblock)
  - name: Steven Black
    source: https://raw.githubusercontent.com/StevenBlack/hosts/master/hosts
    type: hosts
    transformations:
      - Compress  # Converts hosts to adblock format

  # Local file
  - name: Custom Rules
    source: ./my-rules.txt
    type: adblock

transformations:
  - Validate
  - Deduplicate
  - RemoveEmptyLines
  - InsertFinalNewLine
```

## Validation

### .NET Compiler Validation

The .NET compiler supports configuration validation before compilation:

```bash
dotnet run --project src/RulesCompiler.Console -- -c config.yaml --validate
```

This checks:
- Required fields are present
- Sources are accessible
- Transformations are valid
- Pattern syntax is correct

### Common Validation Errors

| Error | Cause | Solution |
|-------|-------|----------|
| `name is required` | Missing `name` field | Add `name` property |
| `sources is required` | Missing `sources` array | Add at least one source |
| `Invalid transformation` | Unknown transformation | Check spelling, use valid name |
| `Source not found` | Invalid file path | Verify file exists |
| `Invalid regex pattern` | Bad regex in pattern | Fix regex syntax |

## CLI Options by Compiler

| Option | TypeScript | .NET | Python | Rust |
|--------|------------|------|--------|------|
| Config file | `-c`, `--config` | `-c`, `--config` | `-c`, `--config` | `-c`, `--config` |
| Output file | `-o`, `--output` | `-o`, `--output` | `-o`, `--output` | `-o`, `--output` |
| Copy to rules | `-r`, `--copy-to-rules` | `--copy` | `-r`, `--copy-to-rules` | `-r`, `--copy-to-rules` |
| Debug output | `-d`, `--debug` | `--verbose` | `-d`, `--debug` | `-d`, `--debug` |
| Validate only | - | `--validate` | - | - |
| Version | `--version` | `-v`, `--version` | `-V`, `--version` | `-V`, `--version` |
| Help | `--help` | `--help` | `--help` | `--help` |
