# AdGuard.ConsoleUI

A console-based user interface for managing AdGuard DNS accounts, devices, DNS servers, and viewing statistics and query logs.

## Overview

AdGuard.ConsoleUI is a .NET 10 console application that provides an interactive menu-driven interface for the AdGuard DNS API. It uses [Spectre.Console](https://spectreconsole.net/) for rich terminal UI elements including colored text, tables, panels, and interactive prompts.

## Features

- **Device Management**: List, view, create, and delete devices
- **DNS Server Management**: List, view, create, and delete DNS servers
- **Statistics Viewing**: View query statistics for 24 hours, 7 days, 30 days, or custom time ranges
- **Query Log**: View recent queries and clear query log
- **Filter Lists**: View available filter lists
- **Web Services**: Browse available web services for blocking
- **Dedicated IP Addresses**: List and allocate dedicated IPv4 addresses
- **Account Information**: Display account limits and usage with visual progress bars
- **Connection Testing**: Test API connectivity
- **API Key Configuration**: Configure API key interactively or via settings

## Prerequisites

- .NET 10 SDK or runtime
- AdGuard DNS account with API access
- API key from [AdGuard DNS Dashboard](https://adguard-dns.io/dashboard/#/settings/api)

## Installation

### Build from Source

```bash
cd src/adguard-api-dotnet/src/AdGuard.ConsoleUI
dotnet build
```

### Run the Application

```bash
dotnet run --project src/adguard-api-dotnet/src/AdGuard.ConsoleUI
```

Or after building:

```bash
cd src/adguard-api-dotnet/src/AdGuard.ConsoleUI/bin/Debug/net10.0
./AdGuard.ConsoleUI
```

## Configuration

### Option 1: appsettings.json

Create or modify `appsettings.json` in the application directory:

```json
{
  "AdGuard": {
    "ApiKey": "your-api-key-here"
  }
}
```

### Option 2: Environment Variables

Set the API key using environment variables with the `ADGUARD_` prefix. Environment variables have higher precedence than `appsettings.json`.

**Linux/macOS:**
```bash
export ADGUARD_AdGuard__ApiKey="your-api-key-here"
dotnet run
```

**Windows (PowerShell):**
```powershell
$env:ADGUARD_AdGuard__ApiKey="your-api-key-here"
dotnet run
```

**Windows (Command Prompt):**
```cmd
set ADGUARD_AdGuard__ApiKey=your-api-key-here
dotnet run
```

**Note:** The double underscore (`__`) in `AdGuard__ApiKey` is the .NET configuration convention for representing a colon (`:`) in environment variable names. The `ADGUARD_` prefix is stripped by the configuration system, so `ADGUARD_AdGuard__ApiKey` maps to `AdGuard:ApiKey`.

**Configuration Precedence:**
1. Environment variables (highest priority)
2. `appsettings.json`
3. Interactive prompt (if no API key is found)

### Option 3: Interactive Configuration

If no API key is configured, the application will prompt you to enter one on startup.

## Usage

### Main Menu

After launching, you'll see the main menu with the following options:

| Menu Item | Description |
|-----------|-------------|
| Devices | Manage devices (list, view, create, delete) |
| DNS Servers | Manage DNS servers (list, view, create, delete) |
| Statistics | View query statistics for various time ranges |
| Query Log | View recent queries and clear logs |
| Filter Lists | View available filter lists |
| Web Services | Browse available web services for blocking |
| Dedicated IP Addresses | List and allocate dedicated IPv4 addresses |
| Account Info | Display account limits and usage |
| Settings | Change API key, test connection |
| Exit | Close the application |

### Device Management

- **List Devices**: Shows all devices in a table with ID, name, type, and DNS server
- **View Device Details**: Select a device to see full details including DNS addresses and settings
- **Create Device**: Interactive wizard to create a new device
- **Delete Device**: Select and confirm device deletion

### DNS Server Management

- **List DNS Servers**: Shows all DNS servers with ID, name, default status, and device count
- **View Server Details**: Select a server to see full details including device IDs and settings
- **Create DNS Server**: Enter a name to create a new DNS server
- **Delete DNS Server**: Select and confirm deletion (cannot delete default server)

### Statistics

View query statistics for:
- Last 24 hours
- Last 7 days
- Last 30 days
- Custom time range (specify days ago)

Statistics include total queries, blocked queries, and block rate percentage.

### Query Log

- **View Recent Queries (Last Hour)**: Shows queries from the last hour
- **View Today's Queries**: Shows all queries from today
- **View Custom Time Range**: Specify hours ago to view
- **Clear Query Log**: Permanently delete all query logs

### Web Services

- **List Web Services**: Browse all available web services that can be blocked
  - View service ID and name
  - Useful for identifying services to block in DNS server settings

### Dedicated IP Addresses

- **List All IP Addresses**: View all allocated dedicated IPv4 addresses
  - Shows IP address, linked device ID, and status (linked/unlinked)
- **Allocate New IP Address**: Allocate a new dedicated IPv4 address to your account
  - Useful for dedicated DNS filtering

### Settings

- **Change API Key**: Enter a new API key
- **Test Connection**: Verify API connectivity

## Project Structure

```
AdGuard.ConsoleUI/
├── Program.cs                  # Application entry point and DI configuration
├── appsettings.json           # Default configuration template
├── AdGuard.ConsoleUI.csproj   # Project file
├── README.md                  # This file
└── Services/
    ├── ApiClientFactory.cs      # Factory for creating API clients
    ├── ConsoleApplication.cs    # Main application orchestrator
    ├── AccountMenuService.cs    # Account information display
    ├── DeviceMenuService.cs     # Device CRUD operations
    ├── DnsServerMenuService.cs  # DNS server CRUD operations
    ├── FilterListMenuService.cs # Filter list display
    ├── QueryLogMenuService.cs   # Query log viewing and clearing
    ├── StatisticsMenuService.cs # Statistics display
    ├── WebServiceMenuService.cs # Web services browsing
    └── DedicatedIPMenuService.cs # Dedicated IP address management
```

## Dependencies

| Package | Version | Description |
|---------|---------|-------------|
| Spectre.Console | 0.49.1 | Rich console UI library |
| Microsoft.Extensions.Configuration | 8.0.0 | Configuration abstractions |
| Microsoft.Extensions.Configuration.Json | 8.0.0 | JSON configuration provider |
| Microsoft.Extensions.Configuration.EnvironmentVariables | 8.0.0 | Environment variables provider |
| Microsoft.Extensions.DependencyInjection | 8.0.0 | Dependency injection container |
| Microsoft.Extensions.Logging | 8.0.0 | Logging abstractions |
| Microsoft.Extensions.Hosting | 8.0.0 | Generic host services |
| AdGuard.ApiClient | (project ref) | AdGuard DNS API client |

## Architecture

### Dependency Injection

The application uses Microsoft.Extensions.DependencyInjection for managing service lifetimes. All services are registered as singletons:

```csharp
services.AddSingleton<ApiClientFactory>();
services.AddSingleton<ConsoleApplication>();
services.AddSingleton<DeviceMenuService>();
// ... other services
```

### ApiClientFactory

The `ApiClientFactory` is responsible for:
- Managing API configuration and API key
- Creating API client instances (AccountApi, DevicesApi, etc.)
- Testing API connectivity

### Menu Services

Each menu service handles a specific domain:
- Receives `ApiClientFactory` via constructor injection
- Provides interactive menu loop via `ShowAsync()`
- Handles API exceptions with user-friendly error messages

### Configuration

Configuration is built from multiple sources in order of precedence:
1. `appsettings.json` (optional)
2. Environment variables with `ADGUARD_` prefix

## Error Handling

- API exceptions display error codes and messages to the user
- Invalid API keys trigger authentication error messages
- Unexpected exceptions are displayed with stack traces

## Testing

Unit tests are located in `AdGuard.ApiClient.Test/ConsoleUI/`:

```bash
# Run all tests
dotnet test src/adguard-api-dotnet/src/AdGuard.ApiClient.Test

# Run only ConsoleUI tests
dotnet test src/adguard-api-dotnet/src/AdGuard.ApiClient.Test --filter "FullyQualifiedName~ConsoleUI"
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## License

See the repository root for license information.
