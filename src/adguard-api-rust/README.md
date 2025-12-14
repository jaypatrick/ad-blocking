# AdGuard DNS API - Rust Implementation

This directory contains the Rust implementation of the AdGuard DNS API client, consisting of:

- **adguard-api-lib**: Auto-generated Rust API client library from OpenAPI specification
- **adguard-api-cli**: Command-line interface for interacting with the AdGuard DNS API

## Overview

The Rust implementation provides:
- Type-safe API client library generated from OpenAPI spec (v1.11)
- Async/await support using Tokio runtime
- CLI tool with comprehensive subcommands for all API operations
- Environment variable support for configuration

## Prerequisites

- Rust toolchain (1.75 or later recommended)
- Docker (for regenerating the API client from OpenAPI spec)

## Building

Build the entire workspace:

```bash
cd src/adguard-api-rust
cargo build
```

Build for release (optimized):

```bash
cargo build --release
```

## Library (adguard-api-lib)

The API client library is auto-generated from the OpenAPI specification at `../adguard-api-client/api/openapi.json`.

### Regenerating the Client

To regenerate the API client from the latest OpenAPI spec:

```bash
./regenerate-client.sh
```

This script uses Docker to run the OpenAPI Generator with the Rust generator configuration.

### Usage in Rust Projects

Add to your `Cargo.toml`:

```toml
[dependencies]
adguard-api-lib = { path = "../adguard-api-rust/adguard-api-lib" }
tokio = { version = "1.40", features = ["full"] }
```

Example usage:

```rust
use adguard_api_lib::apis::configuration::Configuration;
use adguard_api_lib::apis::devices_api;

#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error>> {
    let mut config = Configuration::new();
    config.base_path = "https://api.adguard-dns.io".to_string();
    config.bearer_access_token = Some("your-api-token".to_string());

    // List devices
    let devices = devices_api::list_devices(&config).await?;
    println!("{:?}", devices);

    Ok(())
}
```

## CLI (adguard-api-cli)

Command-line interface for the AdGuard DNS API.

### Installation

From the workspace root:

```bash
cargo install --path adguard-api-cli
```

Or run directly:

```bash
cargo run --bin adguard-api-cli -- --help
```

### Configuration

The CLI can be configured via command-line arguments or environment variables:

- `--api-url` or `ADGUARD_API_URL`: API base URL (default: https://api.adguard-dns.io)
- `--api-token` or `ADGUARD_API_TOKEN`: Authentication token (required for most operations)

### Commands

#### Account Operations

```bash
# Get account limits
adguard-api-cli --api-token YOUR_TOKEN account limits
```

#### Device Operations

```bash
# List all devices
adguard-api-cli --api-token YOUR_TOKEN devices list

# Get specific device
adguard-api-cli --api-token YOUR_TOKEN devices get DEVICE_ID

# Remove a device
adguard-api-cli --api-token YOUR_TOKEN devices remove DEVICE_ID
```

#### DNS Server Operations

```bash
# List DNS servers
adguard-api-cli --api-token YOUR_TOKEN dns-servers list
```

#### Filter Lists

```bash
# List available filter lists
adguard-api-cli --api-token YOUR_TOKEN filter-lists list
```

#### Statistics

```bash
# Get time-based statistics (last 24 hours)
adguard-api-cli --api-token YOUR_TOKEN statistics time
```

#### Web Services

```bash
# List web services
adguard-api-cli --api-token YOUR_TOKEN web-services list
```

### Using Environment Variables

Set the token once:

```bash
export ADGUARD_API_TOKEN="your-token-here"
adguard-api-cli devices list
```

## Architecture

### Workspace Structure

```
adguard-api-rust/
├── Cargo.toml              # Workspace configuration
├── regenerate-client.sh    # Script to regenerate API client
├── adguard-api-lib/        # Auto-generated API client library
│   ├── Cargo.toml
│   ├── src/
│   │   ├── apis/          # API endpoint modules
│   │   └── models/        # Data models
│   └── docs/              # API documentation
└── adguard-api-cli/        # CLI application
    ├── Cargo.toml
    └── src/
        └── main.rs        # CLI implementation
```

### Code Generation

The API client is generated using [OpenAPI Generator](https://openapi-generator.tech/) with the Rust generator:

- Generator: `rust`
- Version: 7.16.0
- Features: Async support, single request parameter structs
- TLS: rustls (default) or native-tls (optional feature)

### Dependencies

Key dependencies:

- **reqwest**: HTTP client with async support
- **tokio**: Async runtime
- **serde/serde_json**: Serialization
- **clap**: CLI argument parsing
- **anyhow**: Error handling

## Development

### Running Tests

```bash
cargo test
```

### Code Formatting

```bash
cargo fmt
```

### Linting

```bash
cargo clippy
```

### Building Release Binaries

```bash
cargo build --release
```

Release binaries will be in `target/release/`.

## Comparison with Other Implementations

This Rust implementation follows the same patterns as the other language implementations in the repository:

- **C# (src/adguard-api-client)**: Full-featured with ConsoleUI, repositories, and data access layers
- **TypeScript (src/rules-compiler-typescript)**: Rules compilation focus
- **Python (src/rules-compiler-python)**: Rules compilation focus
- **Rust (this)**: API client library + CLI, follows Rust ecosystem conventions

### Advantages

- **Memory safety**: Rust's ownership system prevents common bugs
- **Performance**: Compiled to native code, efficient async runtime
- **Type safety**: Strong typing with excellent error messages
- **Modern patterns**: Uses Rust 2024 edition with latest features
- **Zero-cost abstractions**: High-level code compiles to efficient machine code

## Contributing

When contributing to the Rust implementation:

1. Follow Rust naming conventions (snake_case for functions/variables)
2. Use `#[must_use]` for important result types
3. Apply `#[non_exhaustive]` to error enums for forward compatibility
4. Prefer `map_while` over `filter_map` for IO iterator chains
5. Run `cargo fmt` and `cargo clippy` before committing

## License

GPL-3.0 - See LICENSE file in repository root

## Related Documentation

- [AdGuard DNS API Documentation](https://adguard-dns.io/kb/private-dns/api/overview/)
- [OpenAPI Specification](../adguard-api-client/api/openapi.json)
- [C# API Client README](../adguard-api-client/README.md)
