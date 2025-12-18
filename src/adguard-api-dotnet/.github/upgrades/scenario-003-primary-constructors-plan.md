# SCENARIO-003: Primary Constructors - Execution Plan

**Status:** ?? In Progress  
**Started:** 2025-01-24  
**Priority:** MEDIUM  
**Estimated Time:** 2 hours  

---

## ?? Overview

Refactor classes in AdGuard.ConsoleUI to use C# 12 primary constructors. This eliminates boilerplate field declarations and constructor assignments, making code more concise and readable while maintaining full functionality.

### Why Primary Constructors?

1. **Less Boilerplate**: Eliminate private field declarations and constructor parameter assignments
2. **Clearer Intent**: Dependencies visible at class declaration level
3. **Reduced Code**: Save 5-10 lines per class (×25 classes = 125-250 lines saved)
4. **Modern C# 12**: Leverage latest language features in .NET 8
5. **Better Readability**: Focus on what the class does, not how it's wired up

---

## ?? Scope: AdGuard.ConsoleUI Only

**Project:** `src/AdGuard.ConsoleUI/` (C# 12, .NET 8)  
**Target Classes:** ~25 classes with constructor injection  
**Excluded:** AdGuard.ApiClient (generated code - will be handled in SCENARIO-005)

---

## ?? Class Inventory

### Repositories (6 classes)
| Class | File | Constructor Parameters | Lines to Save |
|-------|------|----------------------|---------------|
| `DeviceRepository` | `Repositories/DeviceRepository.cs` | 2 (ILogger, IApiClientFactory) | ~8 |
| `DnsServerRepository` | `Repositories/DnsServerRepository.cs` | 2 | ~8 |
| `AccountRepository` | `Repositories/AccountRepository.cs` | 2 | ~8 |
| `StatisticsRepository` | `Repositories/StatisticsRepository.cs` | 2 | ~8 |
| `FilterListRepository` | `Repositories/FilterListRepository.cs` | 2 | ~8 |
| `QueryLogRepository` | `Repositories/QueryLogRepository.cs` | 2 | ~8 |

### Display Strategies (6 classes)
| Class | File | Constructor Parameters | Lines to Save |
|-------|------|----------------------|---------------|
| `DeviceDisplayStrategy` | `Display/DeviceDisplayStrategy.cs` | 0-1 | ~5 |
| `DnsServerDisplayStrategy` | `Display/DnsServerDisplayStrategy.cs` | 0-1 | ~5 |
| `FilterListDisplayStrategy` | `Display/FilterListDisplayStrategy.cs` | 0-1 | ~5 |
| `AccountLimitsDisplayStrategy` | `Display/AccountLimitsDisplayStrategy.cs` | 0-1 | ~5 |
| `StatisticsDisplayStrategy` | `Display/StatisticsDisplayStrategy.cs` | 0-1 | ~5 |
| `QueryLogDisplayStrategy` | `Display/QueryLogDisplayStrategy.cs` | 0-1 | ~5 |

### Menu Services (7 classes)
| Class | File | Constructor Parameters | Lines to Save |
|-------|------|----------------------|---------------|
| `BaseMenuService` | `Services/BaseMenuService.cs` | Abstract (varies) | ~0 (template) |
| `DeviceMenuService` | `Services/DeviceMenuService.cs` | 3-4 | ~10 |
| `DnsServerMenuService` | `Services/DnsServerMenuService.cs` | 3-4 | ~10 |
| `AccountMenuService` | `Services/AccountMenuService.cs` | 3-4 | ~10 |
| `StatisticsMenuService` | `Services/StatisticsMenuService.cs` | 3-4 | ~10 |
| `FilterListMenuService` | `Services/FilterListMenuService.cs` | 3-4 | ~10 |
| `QueryLogMenuService` | `Services/QueryLogMenuService.cs` | 3-4 | ~10 |

### Core Classes (3 classes)
| Class | File | Constructor Parameters | Lines to Save |
|-------|------|----------------------|---------------|
| `ApiClientFactory` | `ApiClientFactory.cs` | 2-3 (IConfiguration, ILogger) | ~8 |
| `ConsoleApplication` | `ConsoleApplication.cs` | 3-4 | ~10 |
| Other helper classes | Various | Varies | ~5-10 each |

**Total Estimated Lines Saved:** 150-200 lines

---

## ?? Primary Constructor Pattern

### Before (Traditional Pattern)
```csharp
public class DeviceRepository : IDeviceRepository
{
    private readonly ILogger<DeviceRepository> _logger;
    private readonly IApiClientFactory _factory;

    public DeviceRepository(
        ILogger<DeviceRepository> logger,
        IApiClientFactory factory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public async Task<List<Device>> GetAllAsync()
    {
        _logger.LogDebug("Fetching all devices");
        // Use _factory here
    }
}
```

### After (C# 12 Primary Constructor)
```csharp
public class DeviceRepository(
    ILogger<DeviceRepository> logger,
    IApiClientFactory factory) : IDeviceRepository
{
    public async Task<List<Device>> GetAllAsync()
    {
        logger.LogDebug("Fetching all devices");
        // Use factory here
    }
}
```

**Changes:**
- ? Constructor parameters moved to class declaration
- ? Private fields removed
- ? Constructor body removed
- ? Null checks removed (handled by DI container)
- ? Parameter names used directly in methods

---

## ?? Step-by-Step Execution

### Phase 1: Repository Classes (Warmup - Simple Pattern)

#### ? STEP 3.1: Refactor DeviceRepository
**File:** `Repositories/DeviceRepository.cs`

**Current Pattern:**
```csharp
private readonly ILogger<DeviceRepository> _logger;
private readonly IApiClientFactory _factory;

public DeviceRepository(ILogger<DeviceRepository> logger, IApiClientFactory factory)
{
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _factory = factory ?? throw new ArgumentNullException(nameof(factory));
}
```

**New Pattern:**
```csharp
public class DeviceRepository(
    ILogger<DeviceRepository> logger,
    IApiClientFactory factory) : IDeviceRepository
{
    // Use logger and factory directly in methods
}
```

**Find/Replace Strategy:**
1. Update class declaration to include primary constructor
2. Remove private field declarations
3. Remove constructor method
4. Update all `_logger` references to `logger`
5. Update all `_factory` references to `factory`

**Status:** [ ] Not Started  
**Time:** 8 minutes

---

#### ? STEP 3.2: Refactor Remaining Repository Classes
Apply same pattern to:
- `DnsServerRepository`
- `AccountRepository`
- `StatisticsRepository`
- `FilterListRepository`
- `QueryLogRepository`

**Status:** [ ] Not Started  
**Time:** 30 minutes (6 minutes each)

---

### Phase 2: Display Strategy Classes

#### ? STEP 3.3: Refactor Display Strategies
These classes likely have minimal or no dependencies. Update all 6:
- `DeviceDisplayStrategy`
- `DnsServerDisplayStrategy`
- `FilterListDisplayStrategy`
- `AccountLimitsDisplayStrategy`
- `StatisticsDisplayStrategy`
- `QueryLogDisplayStrategy`

**Note:** Some may have no constructor parameters (just default constructor). Those can be left as-is or simplified.

**Status:** [ ] Not Started  
**Time:** 20 minutes

---

### Phase 3: Menu Service Classes

#### ? STEP 3.4: Refactor BaseMenuService (If Applicable)
**File:** `Services/BaseMenuService.cs`

Check if this abstract class has constructor parameters that can be converted.

**Status:** [ ] Not Started  
**Time:** 5 minutes

---

#### ? STEP 3.5: Refactor Menu Services
Apply pattern to all 6 concrete menu services:
- `DeviceMenuService`
- `DnsServerMenuService`
- `AccountMenuService`
- `StatisticsMenuService`
- `FilterListMenuService`
- `QueryLogMenuService`

**These typically have 3-4 parameters:** repository, display strategy, logger

**Status:** [ ] Not Started  
**Time:** 40 minutes (7 minutes each)

---

### Phase 4: Core Classes

#### ? STEP 3.6: Refactor ApiClientFactory
**File:** `ApiClientFactory.cs`

**Status:** [ ] Not Started  
**Time:** 8 minutes

---

#### ? STEP 3.7: Refactor ConsoleApplication
**File:** `ConsoleApplication.cs`

**Status:** [ ] Not Started  
**Time:** 8 minutes

---

#### ? STEP 3.8: Find and Refactor Other Classes
Search for remaining classes with constructor injection:

```powershell
# Search for constructor patterns
Get-ChildItem -Recurse -Filter *.cs | 
  Select-String -Pattern "public \w+\(" | 
  Where-Object { $_.Line -notmatch "static|async|Task" }
```

**Status:** [ ] Not Started  
**Time:** 10 minutes

---

### Phase 5: Verification and Testing

#### ? STEP 3.9: Build and Fix Compilation Errors
```bash
cd D:\source\ad-blocking\src\api-client
dotnet build src/AdGuard.ConsoleUI
```

**Expected Issues:**
- References to old field names (e.g., `_logger` instead of `logger`)
- Missing parameter conversions

**Status:** [ ] Not Started  
**Time:** 10 minutes

---

#### ? STEP 3.10: Run Unit Tests
```bash
dotnet test
```

**Status:** [ ] Not Started  
**Time:** 3 minutes

---

#### ? STEP 3.11: Manual Smoke Test
Run ConsoleUI and verify:
- Application starts
- Menus display correctly
- At least one operation from each menu works
- No runtime errors

**Status:** [ ] Not Started  
**Time:** 5 minutes

---

### Phase 6: Documentation and Commit

#### ? STEP 3.12: Update ARCHITECTURE.md
Update the architecture documentation to reflect primary constructor usage:

Add section:
```markdown
### C# 12 Primary Constructors

All classes with constructor injection use C# 12 primary constructors for conciseness:

\`\`\`csharp
public class DeviceRepository(
    ILogger<DeviceRepository> logger,
    IApiClientFactory factory) : IDeviceRepository
{
    // Dependencies accessible as parameters directly
}
\`\`\`

This pattern eliminates boilerplate while maintaining:
- Full dependency injection support
- Type safety
- Null reference safety (via DI container)
```

**Status:** [ ] Not Started  
**Time:** 5 minutes

---

#### ? STEP 3.13: Update tasks.md
Mark SCENARIO-003 as complete

**Status:** [ ] Not Started  
**Time:** 2 minutes

---

#### ? STEP 3.14: Commit Changes
```bash
git add -A
git commit -m "refactor: Adopt C# 12 primary constructors in ConsoleUI

- Refactored 22 classes to use primary constructors
- Eliminated ~180 lines of boilerplate code
- Removed private field declarations and constructor bodies
- All dependencies now declared at class level
- Updated ARCHITECTURE.md with primary constructor pattern

Benefits:
- Clearer code intent
- Reduced boilerplate
- Modern C# 12 patterns
- Improved readability

Files changed: 22
Lines removed: ~180
Lines added: ~40
Net reduction: ~140 lines

Closes: SCENARIO-003"
```

**Status:** [ ] Not Started  
**Time:** 2 minutes

---

## ?? Success Criteria

- [ ] All 22+ classes refactored to primary constructors
- [ ] Zero compilation errors
- [ ] All unit tests passing
- [ ] ConsoleUI smoke test successful
- [ ] ARCHITECTURE.md updated
- [ ] Code reduction: 140+ lines
- [ ] No functional changes (pure refactoring)

---

## ?? Important Notes

### Null Checking Strategy
**Old:** Explicit null checks in constructor  
**New:** DI container guarantees non-null (no checks needed)

```csharp
// OLD - explicit null check
_logger = logger ?? throw new ArgumentNullException(nameof(logger));

// NEW - trust DI container
// No check needed - DI will never pass null
```

### Field Naming Convention Change
**Old:** `_camelCase` (private fields)  
**New:** `camelCase` (parameters)

All references must be updated:
- `_logger.LogInformation` ? `logger.LogInformation`
- `_factory.CreateDevicesApi` ? `factory.CreateDevicesApi`

### XML Documentation
Preserve all XML documentation comments. They remain valid with primary constructors.

---

## ?? Find/Replace Patterns

### Pattern 1: Repository Classes
```regex
// Find
private readonly ILogger<(\w+)> _logger;
private readonly IApiClientFactory _factory;

public \1\(ILogger<\1> logger, IApiClientFactory factory\)
{
    _logger = logger.*
    _factory = factory.*
}

// Replace with class declaration update
public class \1(
    ILogger<\1> logger,
    IApiClientFactory factory) : I\1
```

### Pattern 2: Reference Updates
```regex
// Find: _logger
// Replace: logger

// Find: _factory
// Replace: factory
```

---

## ?? Progress Tracking

**Phase 1:** Repositories (6 classes)  
**Status:** [ ] Not Started  
**Progress:** 0/6  
**Time Remaining:** 38 minutes

**Phase 2:** Display Strategies (6 classes)  
**Status:** [ ] Not Started  
**Progress:** 0/6  
**Time Remaining:** 20 minutes

**Phase 3:** Menu Services (7 classes)  
**Status:** [ ] Not Started  
**Progress:** 0/7  
**Time Remaining:** 45 minutes

**Phase 4:** Core Classes (3+ classes)  
**Status:** [ ] Not Started  
**Progress:** 0/3  
**Time Remaining:** 26 minutes

**Phase 5:** Verification  
**Status:** [ ] Not Started  
**Time Remaining:** 18 minutes

**Phase 6:** Documentation  
**Status:** [ ] Not Started  
**Time Remaining:** 9 minutes

**Total Progress:** 0/14 steps (0%)  
**Estimated Time Remaining:** 2 hours

---

## ?? Rollback Plan

If issues occur:

```bash
# Revert all changes
git checkout HEAD -- src/AdGuard.ConsoleUI/

# Or revert specific file
git checkout HEAD -- src/AdGuard.ConsoleUI/Repositories/DeviceRepository.cs
```

---

## ?? Reference Example

### Complete Before/After for DeviceRepository

**BEFORE:**
```csharp
using Microsoft.Extensions.Logging;
using AdGuard.ApiClient.Model;
using AdGuard.ConsoleUI.Abstractions;

namespace AdGuard.ConsoleUI.Repositories;

public class DeviceRepository : IDeviceRepository
{
    private readonly ILogger<DeviceRepository> _logger;
    private readonly IApiClientFactory _factory;

    public DeviceRepository(
        ILogger<DeviceRepository> logger,
        IApiClientFactory factory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public async Task<List<Device>> GetAllAsync()
    {
        _logger.LogDebug("Fetching all devices");
        var api = _factory.CreateDevicesApi();
        return await api.ListDevicesAsync();
    }
}
```

**AFTER:**
```csharp
using Microsoft.Extensions.Logging;
using AdGuard.ApiClient.Model;
using AdGuard.ConsoleUI.Abstractions;

namespace AdGuard.ConsoleUI.Repositories;

public class DeviceRepository(
    ILogger<DeviceRepository> logger,
    IApiClientFactory factory) : IDeviceRepository
{
    public async Task<List<Device>> GetAllAsync()
    {
        logger.LogDebug("Fetching all devices");
        var api = factory.CreateDevicesApi();
        return await api.ListDevicesAsync();
    }
}
```

**Changes:**
- Lines removed: 8
- Lines added: 2
- Net reduction: 6 lines
- Readability: ? Improved

---

**Next Review:** After Phase 3 completion  
**Last Updated:** 2025-01-24
