# Archive Directory

This directory stores processed input filter lists for archiving and audit purposes.

## Purpose

The `data/archive/` directory automatically preserves processed input files after successful compilation, providing:
- **Historical tracking**: Maintain snapshots of rules at different points in time
- **Audit trail**: Track what was compiled and when
- **Rollback capability**: Restore previous versions if needed
- **Compliance**: Meet organizational requirements for data retention

## Automatic Archiving

Archiving behavior is controlled by configuration or environment variables:

### Configuration Options

**1. Automatic Mode** (default)
```json
{
  "archiving": {
    "enabled": true,
    "mode": "automatic"
  }
}
```
Input files are automatically copied to archive after successful compilation.

**2. Interactive Mode**
```json
{
  "archiving": {
    "enabled": true,
    "mode": "interactive"
  }
}
```
User is prompted to archive after each compilation.

**3. Disabled Mode**
```json
{
  "archiving": {
    "enabled": false
  }
}
```
No archiving occurs.

### Environment Variables

| Variable | Values | Default | Description |
|----------|--------|---------|-------------|
| `ADGUARD_ARCHIVE_ENABLED` | `true`/`false` | `true` | Enable/disable archiving |
| `ADGUARD_ARCHIVE_MODE` | `automatic`/`interactive`/`disabled` | `automatic` | Archiving behavior |
| `ADGUARD_ARCHIVE_RETENTION_DAYS` | number | `90` | Days to keep archived files |

### CLI Flags

All compilers support command-line flags:

```bash
# Disable archiving for this run
npm run compile -- --no-archive

# Enable interactive prompting
npm run compile -- --archive-interactive

# Archive with custom retention
npm run compile -- --archive-retention 180
```

## Directory Structure

Archives are organized by timestamp:

```
data/archive/
├── README.md                           # This file
├── 2024-12-27_14-30-45/               # Compilation timestamp
│   ├── manifest.json                   # Metadata about this archive
│   ├── custom-rules.txt                # Archived input files
│   ├── company-blocklist.txt
│   └── internet-sources.txt
├── 2024-12-26_09-15-22/
│   ├── manifest.json
│   └── custom-rules.txt
└── .gitignore                          # Ignore archive contents
```

### Manifest File

Each archive includes a `manifest.json` with compilation metadata:

```json
{
  "timestamp": "2024-12-27T14:30:45.123Z",
  "compiler": "typescript",
  "compilerVersion": "1.0.0",
  "inputFiles": [
    {
      "name": "custom-rules.txt",
      "hash": "abc123...",
      "size": 1234,
      "format": "adblock"
    },
    {
      "name": "company-blocklist.txt",
      "hash": "def456...",
      "size": 5678,
      "format": "hosts"
    }
  ],
  "outputFile": "data/output/adguard_user_filter.txt",
  "outputHash": "xyz789...",
  "ruleCount": 150,
  "success": true,
  "compilationTimeMs": 1234
}
```

## Retention Policy

Old archives are automatically cleaned up based on retention settings:

- **Default retention**: 90 days
- **Cleanup trigger**: Before each new archive creation
- **Configurable**: Set via `ADGUARD_ARCHIVE_RETENTION_DAYS`

Archives older than the retention period are automatically deleted.

## Usage Examples

### TypeScript Compiler

```bash
# Automatic archiving (default)
cd src/rules-compiler-typescript
deno task compile

# Interactive mode
export ADGUARD_ARCHIVE_MODE=interactive
deno task compile

# Disable archiving
deno task compile -- --no-archive

# Custom retention
deno task compile -- --archive-retention 365
```

### .NET Compiler

```bash
cd src/rules-compiler-dotnet
dotnet run -- --archive-mode automatic
dotnet run -- --archive-mode interactive
dotnet run -- --no-archive
```

### Python Compiler

```bash
cd src/rules-compiler-python
rules-compiler --archive-mode automatic
rules-compiler --archive-mode interactive
rules-compiler --no-archive
```

### Rust Compiler

```bash
cd src/rules-compiler-rust
cargo run -- --archive-mode automatic
cargo run -- --archive-mode interactive
cargo run -- --no-archive
```

## Restoring from Archive

To restore a previous compilation state:

1. Navigate to the desired archive directory
2. Copy input files back to `data/input/`
3. Recompile to regenerate output

```bash
# Example: Restore from specific archive
cp data/archive/2024-12-26_09-15-22/*.txt data/input/
cd src/rules-compiler-typescript
deno task compile
```

## Security Considerations

- **Archive integrity**: Manifest includes SHA-384 hashes for verification
- **Access control**: Archives may contain sensitive filtering rules
- **Cleanup**: Old archives are automatically purged based on retention
- **Git ignore**: Archive contents not tracked by default (see `.gitignore`)

## Disk Space Management

Monitor archive disk usage:

```bash
# Check archive directory size
du -sh data/archive/

# List archives by size
du -sh data/archive/*/ | sort -h

# Clean old archives manually
find data/archive/ -type d -mtime +90 -exec rm -rf {} \;
```

## Troubleshooting

### "Archive directory not writable"
- Check permissions: `chmod 755 data/archive`
- Ensure parent directory exists
- Verify disk space available

### "Failed to create archive"
- Check disk space: `df -h`
- Verify input files are readable
- Check manifest generation didn't fail

### "Archive cleanup failed"
- Review retention policy settings
- Check for file permissions issues
- Manually remove old archives if needed

## Best Practices

1. **Set appropriate retention**: Balance storage vs. audit requirements
2. **Monitor disk usage**: Prevent runaway archive growth
3. **Review manifests**: Verify compilation success before archiving
4. **Test restoration**: Periodically verify archives are usable
5. **Backup critical archives**: Important compilations should be backed up
6. **Document changes**: Use git for tracking configuration changes

## Related Documentation

- [Input Directory](../input/README.md)
- [Output Directory](../output/)
- [Configuration Reference](../../docs/configuration-reference.md)
- [Deployment Guide](../../docs/guides/deployment-guide.md)
