# AdGuard API Rust - Quick Start Examples

This file contains quick reference examples for using the Rust API implementation.

## CLI Quick Reference

### Setup

```bash
# Export your API key (recommended - works across all languages)
export ADGUARD_API_KEY="your-api-key-here"

# Or using .NET-compatible format (backward compatible)
export ADGUARD_AdGuard__ApiKey="your-token-here"

# Or using legacy Rust format (backward compatible)
export ADGUARD_API_TOKEN="your-token-here"

# Or pass it with each command
adguard-api-cli --api-token YOUR_TOKEN <command>
```

### Common Commands

```bash
# Get account information
adguard-api-cli account limits

# List all devices
adguard-api-cli devices list

# Get a specific device
adguard-api-cli devices get <device-id>

# Remove a device
adguard-api-cli devices remove <device-id>

# List DNS servers
adguard-api-cli dns-servers list

# List filter lists
adguard-api-cli filter-lists list

# Get statistics (last 24 hours)
adguard-api-cli statistics time

# List web services
adguard-api-cli web-services list
```

## Library Usage Examples

### Basic Setup

```rust
use adguard_api_lib::apis::configuration::Configuration;

#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error>> {
    // Create configuration
    let mut config = Configuration::new();
    config.base_path = "https://api.adguard-dns.io".to_string();
    // Try standard format first, then fall back to legacy formats
    config.bearer_access_token = Some(
        std::env::var("ADGUARD_API_KEY")
            .or_else(|_| std::env::var("ADGUARD_API_KEY")
            .or_else(|_| std::env::var("ADGUARD_AdGuard__ApiKey")))
            .or_else(|_| std::env::var("ADGUARD_API_TOKEN"))?
    );
    
    // Use the API...
    
    Ok(())
}
```

### List Devices

```rust
use adguard_api_lib::apis::{configuration::Configuration, devices_api};

#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error>> {
    let mut config = Configuration::new();
    config.base_path = "https://api.adguard-dns.io".to_string();
    // Try .NET-compatible format first, fallback to legacy format
    config.bearer_access_token = Some(
        std::env::var("ADGUARD_API_KEY")
            .or_else(|_| std::env::var("ADGUARD_AdGuard__ApiKey"))
            .or_else(|_| std::env::var("ADGUARD_API_TOKEN"))?
    );
    
    let devices = devices_api::list_devices(&config).await?;
    
    for device in devices {
        println!("Device: {} (ID: {})", device.name, device.id);
    }
    
    Ok(())
}
```

### Get Account Limits

```rust
use adguard_api_lib::apis::{configuration::Configuration, account_api};

#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error>> {
    let mut config = Configuration::new();
    config.base_path = "https://api.adguard-dns.io".to_string();
    // Try .NET-compatible format first, fallback to legacy format
    config.bearer_access_token = Some(
        std::env::var("ADGUARD_API_KEY")
            .or_else(|_| std::env::var("ADGUARD_AdGuard__ApiKey"))
            .or_else(|_| std::env::var("ADGUARD_API_TOKEN"))?
    );
    
    let limits = account_api::get_account_limits(&config).await?;
    
    println!("Access level: {}", limits.access_level);
    println!("Devices: {}/{}", limits.devices.current, limits.devices.limit);
    println!("DNS servers: {}/{}", limits.dns_servers.current, limits.dns_servers.limit);
    
    Ok(())
}
```

### Get Device Details

```rust
use adguard_api_lib::apis::{configuration::Configuration, devices_api};

#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error>> {
    let mut config = Configuration::new();
    config.base_path = "https://api.adguard-dns.io".to_string();
    // Try .NET-compatible format first, fallback to legacy format
    config.bearer_access_token = Some(
        std::env::var("ADGUARD_API_KEY")
            .or_else(|_| std::env::var("ADGUARD_AdGuard__ApiKey"))
            .or_else(|_| std::env::var("ADGUARD_API_TOKEN"))?
    );
    
    let device_id = "your-device-id";
    let params = devices_api::GetDeviceParams {
        device_id: device_id.to_string(),
    };
    
    let device = devices_api::get_device(&config, params).await?;
    
    println!("Device: {}", device.name);
    println!("Status: {:?}", device.status);
    
    Ok(())
}
```

### List DNS Servers

```rust
use adguard_api_lib::apis::{configuration::Configuration, dns_servers_api};

#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error>> {
    let mut config = Configuration::new();
    config.base_path = "https://api.adguard-dns.io".to_string();
    // Try .NET-compatible format first, fallback to legacy format
    config.bearer_access_token = Some(
        std::env::var("ADGUARD_API_KEY")
            .or_else(|_| std::env::var("ADGUARD_AdGuard__ApiKey"))
            .or_else(|_| std::env::var("ADGUARD_API_TOKEN"))?
    );
    
    let servers = dns_servers_api::list_dns_servers(&config).await?;
    
    for server in servers {
        println!("DNS Server: {} (ID: {})", server.name, server.id);
    }
    
    Ok(())
}
```

### Get Statistics

```rust
use adguard_api_lib::apis::{configuration::Configuration, statistics_api};

#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error>> {
    let mut config = Configuration::new();
    config.base_path = "https://api.adguard-dns.io".to_string();
    // Try .NET-compatible format first, fallback to legacy format
    config.bearer_access_token = Some(
        std::env::var("ADGUARD_API_KEY")
            .or_else(|_| std::env::var("ADGUARD_AdGuard__ApiKey"))
            .or_else(|_| std::env::var("ADGUARD_API_TOKEN"))?
    );
    
    // Get stats for last 24 hours
    let now_ms = std::time::SystemTime::now()
        .duration_since(std::time::UNIX_EPOCH)?
        .as_millis() as i64;
    let day_ago_ms = now_ms - (24 * 60 * 60 * 1000);
    
    let params = statistics_api::GetTimeQueriesStatsParams {
        time_from_millis: day_ago_ms,
        time_to_millis: now_ms,
        devices: None,
        countries: None,
    };
    
    let stats = statistics_api::get_time_queries_stats(&config, params).await?;
    
    println!("Total queries: {}", stats.queries.total);
    println!("Blocked: {}", stats.queries.blocked);
    
    Ok(())
}
```

### Error Handling

```rust
use adguard_api_lib::apis::{configuration::Configuration, devices_api, Error};

#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error>> {
    let mut config = Configuration::new();
    config.base_path = "https://api.adguard-dns.io".to_string();
    // Try .NET-compatible format first, fallback to legacy format
    config.bearer_access_token = Some(
        std::env::var("ADGUARD_API_KEY")
            .or_else(|_| std::env::var("ADGUARD_AdGuard__ApiKey"))
            .or_else(|_| std::env::var("ADGUARD_API_TOKEN"))?
    );
    
    match devices_api::list_devices(&config).await {
        Ok(devices) => {
            println!("Found {} devices", devices.len());
        }
        Err(Error::Reqwest(e)) => {
            eprintln!("Network error: {}", e);
        }
        Err(Error::ResponseError(content)) => {
            eprintln!("API error (status {}): {}", content.status, content.content);
        }
        Err(e) => {
            eprintln!("Other error: {:?}", e);
        }
    }
    
    Ok(())
}
```

## Testing Without API Token

The library and CLI will work without a token for endpoints that don't require authentication (if any). However, most endpoints require authentication.

```bash
# This will fail if the endpoint requires auth
adguard-api-cli devices list

# Error: API authentication required
```

## Environment Variables

The CLI supports two environment variable naming conventions for maximum compatibility:

**Recommended (.NET-compatible format)**:
- `ADGUARD_AdGuard__ApiKey` - API authentication token (compatible with adguard-api-dotnet)
- `ADGUARD_AdGuard__BaseUrl` - API base URL (compatible with adguard-api-dotnet)

**Legacy format** (backward compatible):
- `ADGUARD_API_TOKEN` - API authentication token
- `ADGUARD_API_URL` - API base URL (default: https://api.adguard-dns.io)

## Building Examples

To build and run these examples:

1. Create a new binary in the workspace:
```bash
cargo new --bin examples/list_devices
```

2. Add dependencies to the example's Cargo.toml:
```toml
[dependencies]
adguard-api-lib = { path = "../../adguard-api-lib" }
tokio = { version = "1.40", features = ["full"] }
```

3. Run the example:
```bash
cargo run --bin list_devices
```
