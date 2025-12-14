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

**Interactive menu-driven interface** for the AdGuard DNS API, similar to the C# ConsoleUI application.

### Features

The CLI provides an interactive, user-friendly interface with the following capabilities:

- **Account Information**: View account limits and usage statistics
- **Device Management**: List and view device details
- **DNS Server Management**: List and view DNS server details with settings
- **User Rules Management**: View, upload, add, enable/disable, and clear user rules (placeholder)
- **Query Log**: View recent queries with time ranges and clear query log
- **Statistics**: View query statistics for 24h, 7d, or 30d periods
- **Filter Lists**: Browse available filter lists with descriptions
- **Web Services**: List available web services for blocking
- **Dedicated IP Addresses**: List allocated IPs and allocate new ones
- **Settings**: Configure API key, test connection, view current configuration

### Installation

From the workspace root:

```bash
cargo install --path adguard-api-cli
```

Or run directly:

```bash
cargo run --bin adguard-api-cli
```

For release build:

```bash
cargo build --release
./target/release/adguard-api-cli
```

### Configuration

The CLI supports multiple configuration methods (in order of precedence):

1. **Configuration File**: `~/.config/adguard-api-cli/config.toml`
   ```toml
   api_url = "https://api.adguard-dns.io"
   api_token = "your-api-token-here"
   ```

2. **Environment Variables**:
   - `ADGUARD_API_URL`: API base URL (default: https://api.adguard-dns.io)
   - `ADGUARD_API_TOKEN`: Authentication token

3. **Interactive Prompt**: If no API token is configured, the CLI will prompt you on first run

### Interactive Mode (Default)

Simply run the CLI without arguments to enter interactive mode:

```bash
adguard-api-cli
```

You'll be presented with a menu-driven interface:

```
╔══════════════════════════════════════════╗
║       AdGuard DNS - Console CLI         ║
╚══════════════════════════════════════════╝

? Main Menu 
  ❯ Account Info
    Devices
    DNS Servers
    User Rules
    Query Log
    Statistics
    Filter Lists
    Web Services
    Dedicated IP Addresses
    Settings
    Exit
```

### Menu Options

#### Account Info
Displays comprehensive account limits including:
- Access rules limits
- Device limits and usage
- DNS server limits
- Request quotas
- User rules limits
- Dedicated IPv4 limits

#### Devices
- **List Devices**: View all devices with ID, name, and type
- **View Device Details**: See detailed information for a specific device

#### DNS Servers
- **List DNS Servers**: View all DNS servers with device counts
- **View Server Details**: See detailed settings including user rules and filter lists

#### User Rules
Placeholder for future implementation:
- View current rules
- Upload rules from file
- Add/remove rules
- Enable/disable rules

#### Query Log
- **View Recent Queries (Last Hour)**: See queries from the last hour
- **View Today's Queries**: See all queries from today
- **View Custom Time Range**: Specify hours ago
- **Clear Query Log**: Permanently delete all query logs

#### Statistics
View query statistics for different time periods:
- Last 24 hours
- Last 7 days
- Last 30 days

Displays total queries, blocked queries, and block rate percentage.

#### Filter Lists
Browse all available AdGuard filter lists with:
- Filter ID
- Name and description
- Categories

#### Web Services
List all available web services that can be blocked, useful for configuring DNS server parental controls.

#### Dedicated IP Addresses
- **List All IP Addresses**: View allocated dedicated IPv4 addresses
- **Allocate New IP Address**: Allocate a new dedicated IPv4 to your account

#### Settings
- **Change API Key**: Update your API authentication token
- **Test Connection**: Verify API connectivity
- **View Current Configuration**: Display current API URL and token status

### Using Environment Variables

Set the token once to avoid repeated prompts:

```bash
export ADGUARD_API_TOKEN="your-token-here"
adguard-api-cli
```

Or set both URL and token:

```bash
export ADGUARD_API_URL="https://api.adguard-dns.io"
export ADGUARD_API_TOKEN="your-token-here"
adguard-api-cli
```

### Configuration File Location

The configuration file is stored at:
- **Linux/macOS**: `~/.config/adguard-api-cli/config.toml`
- **Windows**: `%APPDATA%\adguard-api-cli\config.toml`

The file is automatically created when you configure an API key through the Settings menu.

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

This Rust implementation now provides feature parity with the C# ConsoleUI application:

- **C# (src/adguard-api-client)**: Full-featured with ConsoleUI, repositories, and data access layers
  - Interactive menu-driven interface using Spectre.Console
  - Configuration management, connection testing
  - Full API coverage with rich display formatting
  
- **Rust (this)**: API client library + **Interactive CLI**, follows Rust ecosystem conventions
  - **Interactive menu-driven interface** using dialoguer and console crates
  - **Configuration file support** (TOML format)
  - **Full API coverage** matching C# functionality
  - Account info, devices, DNS servers, query log, statistics, filter lists, web services, dedicated IPs
  - Settings management (API key configuration, connection testing)

- **TypeScript (src/rules-compiler-typescript)**: Rules compilation focus
- **Python (src/rules-compiler-python)**: Rules compilation focus

### Key Features Matching C# ConsoleUI

Both implementations now provide:

✅ **Interactive Menu System**: Navigate through options using arrow keys
✅ **Account Limits Display**: View comprehensive account usage and limits
✅ **Device Management**: List and view device details
✅ **DNS Server Management**: List and view server configurations
✅ **Query Log**: View queries with time range filters and clear functionality
✅ **Statistics**: View query statistics for multiple time periods
✅ **Filter Lists**: Browse available filter lists
✅ **Web Services**: List blockable web services  
✅ **Dedicated IPs**: List and allocate dedicated IPv4 addresses
✅ **Settings Menu**: Configure API key, test connection, view configuration
✅ **Configuration Persistence**: Save settings to file for reuse
✅ **Connection Testing**: Verify API connectivity

### Advantages of Rust Implementation

- **Memory safety**: Rust's ownership system prevents common bugs
- **Performance**: Compiled to native code, efficient async runtime
- **Single binary**: No runtime dependencies, easy distribution
- **Type safety**: Strong typing with excellent error messages
- **Modern patterns**: Uses Rust 2024 edition with latest features
- **Zero-cost abstractions**: High-level code compiles to efficient machine code
- **Cross-platform**: Works on Linux, macOS, and Windows without modification

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
