# Input Directory

This directory contains local filter rule lists that will be compiled into the final output filter list.

## Purpose

The `data/input/` directory serves as the source location for:
- **Local rule files**: Filter rules in various formats (adblock, hosts, etc.)
- **Internet list references**: Text files containing URLs to remote filter lists

## File Organization

### Local Rule Files

Place your local filter rule files in this directory. Supported formats:

- **AdBlock format** (`.txt`): Standard AdGuard/uBlock syntax
  ```
  ! Comment
  ||example.com^
  @@||allowed.com^
  ```

- **Hosts format** (`.hosts`, `.txt`): Traditional hosts file syntax
  ```
  # Comment
  127.0.0.1 blocked-domain.com
  0.0.0.0 another-blocked.com
  ```

### Internet List References

Create a file named `internet-sources.txt` (or similar) containing one URL per line:

```
https://easylist.to/easylist/easylist.txt
https://raw.githubusercontent.com/StevenBlack/hosts/master/hosts
https://filters.adtidy.org/extension/chromium/filters/2.txt
```

Lines starting with `#` are treated as comments.

## Hash Verification

All input files are verified using SHA-384 hashes to detect tampering:

- Hashes are computed before compilation
- Changes to input files are tracked
- Compilation fails if hash verification detects unexpected modifications

## Syntax Validation

Before compilation, all input files undergo:

1. **Format detection**: Automatic detection of adblock vs hosts syntax
2. **Syntax validation**: Verification of rule syntax according to format
3. **Error reporting**: Clear messages for invalid rules with line numbers

## Example Structure

```
data/input/
├── README.md                    # This file
├── custom-rules.txt             # Your custom adblock rules
├── company-blocklist.txt        # Organization-specific rules
├── hosts-additions.hosts        # Additional hosts entries
├── internet-sources.txt         # URLs to remote lists
└── .gitignore                   # Ignore sensitive/large files
```

## Compilation Process

1. **Discovery**: Scan `data/input/` for all supported files
2. **Validation**: Lint and verify syntax of each file
3. **Hashing**: Compute SHA-384 hash for integrity verification
4. **Remote fetch** (if applicable): Download internet lists with hash verification
5. **Compilation**: Merge all sources using `@adguard/hostlist-compiler`
6. **Output**: Write final adblock-format list to `data/output/adguard_user_filter.txt`

## Security

- **Hash verification**: Detects file tampering
- **Syntax validation**: Prevents injection of malformed rules
- **Format enforcement**: Final output is always in adblock syntax
- **Source tracking**: Maintains provenance of each rule

## Usage

### Adding Local Rules

1. Create a `.txt` file in `data/input/`
2. Add your filter rules in adblock or hosts format
3. Run the compiler - it will automatically discover and include the file

### Adding Internet Sources

1. Create or edit `internet-sources.txt`
2. Add one URL per line
3. Run the compiler with internet source support enabled

### Running Compilation

```bash
# TypeScript compiler
cd src/rules-compiler-typescript
deno task compile

# .NET compiler
cd src/rules-compiler-dotnet
dotnet run

# Python compiler
cd src/rules-compiler-python
rules-compiler --input-dir ../../data/input --output ../../data/output/adguard_user_filter.txt

# Rust compiler
cd src/rules-compiler-rust
cargo run --release
```

## Best Practices

1. **Organize by purpose**: Group related rules in separate files
2. **Add comments**: Use `!` or `#` to document rule purposes
3. **Test incrementally**: Add rules gradually and verify behavior
4. **Keep backups**: Maintain copies before making bulk changes
5. **Track hashes**: Note hash values for important reference files
6. **Review internet sources**: Verify legitimacy of remote list URLs

## Troubleshooting

### "Hash mismatch detected"
- File was modified since last compilation
- Verify changes were intentional
- Recompile to update hash database

### "Syntax error at line X"
- Check rule format matches file type
- Ensure proper adblock or hosts syntax
- See format examples above

### "File too large"
- Input file exceeds size limit
- Split into multiple smaller files
- Review and remove unnecessary rules

## Related Documentation

- [Configuration Reference](../../docs/configuration-reference.md)
- [Compiler Comparison](../../docs/compiler-comparison.md)
- [AdGuard Filter Syntax](https://adguard.com/kb/general/ad-filtering/create-own-filters/)
