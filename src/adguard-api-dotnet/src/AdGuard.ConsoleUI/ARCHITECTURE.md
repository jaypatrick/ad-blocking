# AdGuard Console UI Architecture

This document describes the object-oriented architecture and design patterns used in the AdGuard Console UI application.

## Overview

The AdGuard Console UI is a command-line application that provides a user-friendly interface for managing AdGuard DNS services. The application follows modern OOP principles and design patterns to ensure maintainability, testability, and extensibility.

## Design Patterns

### 1. Repository Pattern

**Purpose**: Abstracts data access logic from the UI layer, providing a clean separation of concerns.

**Location**: `Repositories/` and `Abstractions/`

**Interfaces**:
- `IDeviceRepository` - Device CRUD operations
- `IDnsServerRepository` - DNS server management
- `IAccountRepository` - Account limits retrieval
- `IStatisticsRepository` - Statistics data access
- `IFilterListRepository` - Filter list management
- `IQueryLogRepository` - Query log operations

**Benefits**:
- Testable: Repositories can be easily mocked for unit testing
- Maintainable: Data access logic is centralized
- Flexible: Different data sources can be swapped without changing UI code

**Example**:
```csharp
public interface IDeviceRepository
{
    Task<List<Device>> GetAllAsync();
    Task<Device> GetByIdAsync(string id);
    Task<Device> CreateAsync(DeviceCreate device);
    Task DeleteAsync(string id);
}
```

### 2. Strategy Pattern

**Purpose**: Encapsulates display algorithms for different entity types, allowing the display format to vary independently from the services that use them.

**Location**: `Display/`

**Interface**:
```csharp
public interface IDisplayStrategy<T>
{
    void Display(T item);
    void DisplayList(IEnumerable<T> items);
}
```

**Implementations**:
- `DeviceDisplayStrategy` - Displays device information in tables
- `DnsServerDisplayStrategy` - Displays DNS server details
- `AccountLimitsDisplayStrategy` - Displays account usage with progress bars
- `StatisticsDisplayStrategy` - Displays DNS query statistics
- `FilterListDisplayStrategy` - Displays filter list information
- `QueryLogDisplayStrategy` - Displays query log entries

**Benefits**:
- Open/Closed Principle: New display formats can be added without modifying existing code
- Single Responsibility: Each strategy focuses on one entity type
- Testable: Display logic can be tested in isolation

### 3. Template Method Pattern

**Purpose**: Defines the skeleton of the menu loop algorithm in a base class, allowing subclasses to customize specific steps.

**Location**: `Services/BaseMenuService.cs`

**Structure**:
```csharp
public abstract class BaseMenuService : IMenuService
{
    public abstract string Title { get; }
    protected abstract Dictionary<string, Func<Task>> GetMenuActions();

    public virtual async Task ShowAsync()
    {
        // Template method: common menu loop logic
        while (true)
        {
            var actions = GetMenuActions();
            var choice = PromptForChoice(actions.Keys);
            await actions[choice].Invoke();
        }
    }
}
```

**Benefits**:
- Code Reuse: Common menu logic defined once in base class
- Consistency: All menus behave similarly
- Extensibility: Subclasses only override what they need

### 4. Factory Pattern

**Purpose**: Centralizes the creation of API client instances, managing configuration and lifecycle.

**Location**: `ApiClientFactory.cs` and `Abstractions/IApiClientFactory.cs`

**Interface**:
```csharp
public interface IApiClientFactory
{
    IDevicesApi CreateDevicesApi();
    IDnsServersApi CreateDnsServersApi();
    IAccountApi CreateAccountApi();
    IStatisticsApi CreateStatisticsApi();
    IFilterListsApi CreateFilterListsApi();
    IQueryLogApi CreateQueryLogApi();
}
```

**Benefits**:
- Centralized Configuration: API configuration managed in one place
- Testability: Factory can be mocked to inject test doubles
- Lifecycle Management: Proper disposal of API clients

### 5. Dependency Injection

**Purpose**: Inverts control of dependency creation, enabling loose coupling and testability.

**Location**: `Program.cs`

**Registration**:
```csharp
services.AddSingleton<IApiClientFactory>(sp => sp.GetRequiredService<ApiClientFactory>());
services.AddSingleton<IDeviceRepository, DeviceRepository>();
services.AddSingleton<IDisplayStrategy<Device>, DeviceDisplayStrategy>();
services.AddSingleton<IMenuService, DeviceMenuService>();
// ... other registrations
```

**Benefits**:
- Loose Coupling: Components depend on abstractions
- Testability: Dependencies can be replaced with mocks
- Flexibility: Implementations can be changed without modifying consumers

## SOLID Principles

### Single Responsibility Principle (SRP)
Each class has one reason to change:
- Repositories: Only data access logic
- Display Strategies: Only presentation logic
- Menu Services: Only user interaction flow
- Helpers: Only utility functions

### Open/Closed Principle (OCP)
- New display strategies can be added without modifying existing code
- New menu services can be registered without changing the console application
- Dictionary-based menu routing replaces switch statements

### Liskov Substitution Principle (LSP)
- All repository implementations can substitute their interfaces
- All display strategies are interchangeable for their entity types

### Interface Segregation Principle (ISP)
- `IMenuService` has only two methods: `Title` and `ShowAsync()`
- `IDisplayStrategy<T>` has focused display methods
- Repository interfaces expose only necessary operations

### Dependency Inversion Principle (DIP)
- High-level modules (menu services) depend on abstractions (repositories)
- Low-level modules (API clients) are accessed through factory interface

## Exception Handling

**Location**: `Exceptions/`

**Hierarchy**:
```
AdGuardConsoleException (base)
├── RepositoryException
│   └── EntityNotFoundException
├── ApiNotConfiguredException
├── ValidationException
└── MenuOperationException
```

**Features**:
- Rich context: Exceptions carry operation details
- Type safety: Specific exceptions for specific scenarios
- Logging integration: Exceptions include structured data for logging

## Logging

All components use `Microsoft.Extensions.Logging.ILogger<T>` for structured logging:

- **Debug**: Internal operations, parameter values
- **Information**: Successful operations, counts
- **Warning**: Non-critical issues (e.g., entity not found)
- **Error**: Exceptions with full context

**Example**:
```csharp
_logger.LogDebug("Fetching device with ID: {DeviceId}", id);
_logger.LogInformation("Retrieved {Count} devices", devices.Count);
_logger.LogError(ex, "API error while fetching devices: {ErrorCode}", ex.ErrorCode);
```

## Helper Classes

### ConsoleHelpers

**Location**: `Helpers/ConsoleHelpers.cs`

Provides common console UI operations:
- `WithStatusAsync<T>()` - Shows spinner during async operations
- `SelectItem<T>()` - Interactive item selection
- `ShowSuccess()` / `ShowError()` - Styled messages
- `CreateProgressBar()` - Progress bar creation
- `GetPercentageMarkup()` - Color-coded percentage display

### TableBuilderExtensions

**Location**: `Helpers/TableBuilderExtensions.cs`

Fluent extensions for Spectre.Console tables:
- `CreateStandardTable()` - Consistent table styling
- `Display()` - Renders table to console
- `DisplayPanel()` - Renders panel with content
- `DisplayRule()` - Renders styled rule/divider

## Project Structure

```
AdGuard.ConsoleUI/
├── Abstractions/           # Interfaces
│   ├── IApiClientFactory.cs
│   ├── IMenuService.cs
│   ├── IDeviceRepository.cs
│   ├── IDnsServerRepository.cs
│   ├── IAccountRepository.cs
│   ├── IStatisticsRepository.cs
│   ├── IFilterListRepository.cs
│   └── IQueryLogRepository.cs
├── Display/                # Strategy implementations
│   ├── IDisplayStrategy.cs
│   ├── DeviceDisplayStrategy.cs
│   ├── DnsServerDisplayStrategy.cs
│   ├── AccountLimitsDisplayStrategy.cs
│   ├── StatisticsDisplayStrategy.cs
│   ├── FilterListDisplayStrategy.cs
│   └── QueryLogDisplayStrategy.cs
├── Exceptions/             # Custom exceptions
│   └── AdGuardConsoleException.cs
├── Helpers/                # Utility classes
│   ├── ConsoleHelpers.cs
│   └── TableBuilderExtensions.cs
├── Repositories/           # Data access implementations
│   ├── DeviceRepository.cs
│   ├── DnsServerRepository.cs
│   ├── AccountRepository.cs
│   ├── StatisticsRepository.cs
│   ├── FilterListRepository.cs
│   └── QueryLogRepository.cs
├── Services/               # Menu services
│   ├── BaseMenuService.cs
│   ├── DeviceMenuService.cs
│   ├── DnsServerMenuService.cs
│   ├── AccountMenuService.cs
│   ├── StatisticsMenuService.cs
│   ├── FilterListMenuService.cs
│   └── QueryLogMenuService.cs
├── ApiClientFactory.cs     # Factory implementation
├── ConsoleApplication.cs   # Main application loop
└── Program.cs              # Entry point and DI configuration
```

## Testing

**Location**: `AdGuard.ConsoleUI.Tests/`

### Unit Tests
- Repository tests with mocked API clients
- Exception tests for custom exception types
- Helper tests for utility functions

### Test Patterns
- **Arrange-Act-Assert**: Clear test structure
- **Mocking**: Uses Moq for dependency isolation
- **Naming**: `MethodName_Scenario_ExpectedResult`

**Example**:
```csharp
[Fact]
public async Task GetAllAsync_ReturnsDevices_WhenApiSucceeds()
{
    // Arrange
    var expectedDevices = new List<Device> { new Device { Id = "1", Name = "Test" } };
    _mockDevicesApi.Setup(x => x.ListDevicesAsync(...)).ReturnsAsync(expectedDevices);

    // Act
    var result = await _repository.GetAllAsync();

    // Assert
    Assert.Equal(expectedDevices, result);
}
```

## Extensibility

### Adding a New Entity Type

1. Create interface in `Abstractions/I{Entity}Repository.cs`
2. Create implementation in `Repositories/{Entity}Repository.cs`
3. Create display strategy in `Display/{Entity}DisplayStrategy.cs`
4. Create menu service in `Services/{Entity}MenuService.cs`
5. Register in `Program.cs` DI container
6. Add to `ConsoleApplication.cs` menu dictionary

### Adding a New Display Format

1. Create new strategy implementing `IDisplayStrategy<T>`
2. Register in DI container (replaces default)
3. No changes needed in menu services

## Migration from Previous Design

### Before (Issues)
- ~500 lines of duplicated code across 6 menu services
- Direct API client coupling in UI layer
- Switch statements for menu routing
- No separation between data access and presentation
- Limited testability due to tight coupling

### After (Improvements)
- Shared base class eliminates duplication
- Repository pattern isolates data access
- Dictionary-based routing (Open/Closed)
- Strategy pattern separates display logic
- Full testability through dependency injection
