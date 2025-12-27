# Shell Scripts

Consolidated location for all shell script implementations in the ad-blocking repository.

## Structure

```
src/shell/
├── README.md          # This file
├── bash/              # Bash shell scripts
│   └── compile-rules.sh
└── zsh/               # Zsh shell scripts
    └── compile-rules.zsh
```

## Bash Scripts

### compile-rules.sh
Cross-platform Bash script for compiling AdGuard filter rules.

**Usage:**
```bash
./src/shell/bash/compile-rules.sh -c config.yaml
```

## Zsh Scripts

### compile-rules.zsh
Zsh-optimized script with native features.

**Usage:**
```zsh
./src/shell/zsh/compile-rules.zsh -c config.yaml
```

## CLI Options

| Option | Short | Description |
|--------|-------|-------------|
| `--config PATH` | `-c` | Path to configuration file |
| `--output PATH` | `-o` | Path to output file |
| `--copy-to-rules` | `-r` | Copy output to rules directory |
| `--version` | `-v` | Show version |
| `--help` | `-h` | Show help |

## Migration Notes

**Current location:** `src/shell/` ✅  
**Previous:** `src/rules-compiler-shell/`, `src/shell-scripts/`

See [Main README](../../README.md) for full documentation.
