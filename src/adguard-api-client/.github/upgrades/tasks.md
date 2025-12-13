# AdGuard Solution Modernization Tasks

**Solution:** AdGuard.ApiClient.slnx  
**Target Framework:** .NET 8.0  
**Current Version:** 4.3.2.42  
**Generated:** 2025-01-24  
**Status:** Not Started

---

## ?? Overview

This document tracks modernization tasks for the AdGuard DNS API Client solution. The goal is to adopt modern .NET 8 patterns, improve performance, and reduce dependencies while maintaining functionality.

**Total Scenarios:** 5  
**Estimated Total Effort:** 8.5-10 hours  

---

## ?? Modernization Scenarios

### SCENARIO-001: Remove ConfigureAwait(false) Calls ?

**Priority:** HIGH | **Effort:** 30 minutes | **Breaking Changes:** None

**Status:** [ ] Not Started

#### Description
Remove unnecessary `ConfigureAwait(false)` calls from async methods in the generated API client code. Starting with .NET 6+, `ConfigureAwait(false)` is no longer needed in library code as the async infrastructure has been improved.

#### Affected Files
- `src/AdGuard.ApiClient/Api/*.cs` (all API classes)
  - `AccountApi.cs`
  - `AuthenticationApi.cs`
  - `DedicatedIPAddressesApi.cs`
  - `DevicesApi.cs`
  - `DNSServersApi.cs`
  - `FilterListsApi.cs`
  - `QueryLogApi.cs`
  - `StatisticsApi.cs`
  - `WebServicesApi.cs`

#### Tasks

- [ ] **TASK-001.1:** Inventory all `ConfigureAwait(false)` usage
  - [ ] Run search for `.ConfigureAwait(false)` across API classes
  - [ ] Document count per file
  - [ ] Verify all are in async API methods

- [ ] **TASK-001.2:** Remove ConfigureAwait from DedicatedIPAddressesApi
  - [ ] Open `src/AdGuard.ApiClient/Api/DedicatedIPAddressesApi.cs`
  - [ ] Remove `.ConfigureAwait(false)` from `AllocateDedicatedIPv4AddressAsync` (line ~410)
  - [ ] Remove `.ConfigureAwait(false)` from `ListDedicatedIPv4AddressesAsync` (line ~520)
  - [ ] Verify code compiles

- [ ] **TASK-001.3:** Remove ConfigureAwait from remaining API classes
  - [ ] Process AccountApi.cs
  - [ ] Process AuthenticationApi.cs
  - [ ] Process DevicesApi.cs
  - [ ] Process DNSServersApi.cs
  - [ ] Process FilterListsApi.cs
  - [ ] Process QueryLogApi.cs
  - [ ] Process StatisticsApi.cs
  - [ ] Process WebServicesApi.cs

- [ ] **TASK-001.4:** Build and test
  - [ ] Run `dotnet build src/api-client`
  - [ ] Run `dotnet test src/api-client`
  - [ ] Verify all tests pass

- [ ] **TASK-001.5:** Update documentation
  - [ ] Add note to README about .NET 8 async patterns
  - [ ] Update CHANGELOG with changes

---

### SCENARIO-002: Adopt Central Package Management (CPM) ??

**Priority:** MEDIUM | **Effort:** 1 hour | **Breaking Changes:** None

**Status:** [ ] Not Started

#### Description
Implement Central Package Management to manage all NuGet package versions from a single `Directory.Packages.props` file. This provides a single source of truth for package versions across all projects.

#### Benefits
- Consistent package versions across solution
- Easier security updates
- Reduced merge conflicts
- Better dependency tracking

#### Tasks

- [ ] **TASK-002.1:** Create Directory.Packages.props
  - [ ] Create file at solution root: `src/api-client/Directory.Packages.props`
  - [ ] Add MSBuild property: `<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>`
  - [ ] Set baseline structure

- [ ] **TASK-002.2:** Extract AdGuard.ApiClient package versions
  - [ ] Add `Newtonsoft.Json` version 13.0.4
  - [ ] Add `JsonSubTypes` version 2.0.1
  - [ ] Add `Microsoft.Extensions.Logging.Abstractions` version 10.0.0
  - [ ] Add `Polly` version 8.6.5

- [ ] **TASK-002.3:** Extract AdGuard.ConsoleUI package versions
  - [ ] Add `Microsoft.Extensions.Configuration` version 10.0.0
  - [ ] Add `Microsoft.Extensions.Configuration.EnvironmentVariables` version 10.0.0
  - [ ] Add `Microsoft.Extensions.Configuration.Json` version 10.0.0
  - [ ] Add `Microsoft.Extensions.Configuration.UserSecrets` version 10.0.0
  - [ ] Add `Microsoft.Extensions.DependencyInjection` version 10.0.0
  - [ ] Add `Microsoft.Extensions.Hosting` version 10.0.0
  - [ ] Add `Microsoft.Extensions.Logging` version 10.0.0
  - [ ] Add `Microsoft.Extensions.Logging.Console` version 10.0.0
  - [ ] Add `Spectre.Console` version 0.54.0

- [ ] **TASK-002.4:** Extract AdGuard.ApiClient.Test package versions
  - [ ] Add `coverlet.collector` version 6.0.4
  - [ ] Add `Microsoft.NET.Test.Sdk` version 18.0.1
  - [ ] Add `Moq` version 4.20.72
  - [ ] Add `xunit` version 2.9.3
  - [ ] Add `xunit.runner.visualstudio` version 3.1.5

- [ ] **TASK-002.5:** Update project files
  - [ ] Update `AdGuard.ApiClient.csproj`: Remove version attributes, keep package references
  - [ ] Update `AdGuard.ConsoleUI.csproj`: Remove version attributes
  - [ ] Update `AdGuard.ApiClient.Test.csproj`: Remove version attributes
  - [ ] Remove `<ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>` from ApiClient project

- [ ] **TASK-002.6:** Verify and test
  - [ ] Run `dotnet restore src/api-client`
  - [ ] Run `dotnet build src/api-client`
  - [ ] Run `dotnet test src/api-client`
  - [ ] Verify NuGet package resolution works correctly

---

### SCENARIO-003: Leverage C# 12 Primary Constructors ??

**Priority:** MEDIUM | **Effort:** 2 hours | **Breaking Changes:** None

**Status:** [ ] Not Started

#### Description
Refactor classes in AdGuard.ConsoleUI to use C# 12 primary constructors. This eliminates boilerplate field declarations and constructor assignments, making the code more concise and readable.

#### Affected Projects
- AdGuard.ConsoleUI (C# 12, .NET 8)

#### Target Classes (~25 classes)

**Repositories (6 classes):**
- `DeviceRepository`
- `DnsServerRepository`
- `AccountRepository`
- `StatisticsRepository`
- `FilterListRepository`
- `QueryLogRepository`

**Display Strategies (6 classes):**
- `DeviceDisplayStrategy`
- `DnsServerDisplayStrategy`
- `FilterListDisplayStrategy`
- `AccountLimitsDisplayStrategy`
- `StatisticsDisplayStrategy`
- `QueryLogDisplayStrategy`

**Menu Services (7 classes):**
- `BaseMenuService`
- `DeviceMenuService`
- `DnsServerMenuService`
- `AccountMenuService`
- `StatisticsMenuService`
- `FilterListMenuService`
- `QueryLogMenuService`

**Other (6 classes):**
- `ApiClientFactory`
- `ConsoleApplication`
- Any other classes with constructor injection

#### Tasks

- [ ] **TASK-003.1:** Update Repository classes
  - [ ] Refactor `DeviceRepository` to use primary constructor
  - [ ] Refactor `DnsServerRepository` to use primary constructor
  - [ ] Refactor `AccountRepository` to use primary constructor
  - [ ] Refactor `StatisticsRepository` to use primary constructor
  - [ ] Refactor `FilterListRepository` to use primary constructor
  - [ ] Refactor `QueryLogRepository` to use primary constructor
  - [ ] Build and verify no errors

- [ ] **TASK-003.2:** Update Display Strategy classes
  - [ ] Refactor `DeviceDisplayStrategy` to use primary constructor
  - [ ] Refactor `DnsServerDisplayStrategy` to use primary constructor
  - [ ] Refactor `FilterListDisplayStrategy` to use primary constructor
  - [ ] Refactor `AccountLimitsDisplayStrategy` to use primary constructor
  - [ ] Refactor `StatisticsDisplayStrategy` to use primary constructor
  - [ ] Refactor `QueryLogDisplayStrategy` to use primary constructor
  - [ ] Build and verify no errors

- [ ] **TASK-003.3:** Update Menu Service classes
  - [ ] Refactor `BaseMenuService` to use primary constructor (if applicable)
  - [ ] Refactor `DeviceMenuService` to use primary constructor
  - [ ] Refactor `DnsServerMenuService` to use primary constructor
  - [ ] Refactor `AccountMenuService` to use primary constructor
  - [ ] Refactor `StatisticsMenuService` to use primary constructor
  - [ ] Refactor `FilterListMenuService` to use primary constructor
  - [ ] Refactor `QueryLogMenuService` to use primary constructor
  - [ ] Build and verify no errors

- [ ] **TASK-003.4:** Update remaining classes
  - [ ] Refactor `ApiClientFactory` to use primary constructor
  - [ ] Refactor `ConsoleApplication` to use primary constructor
  - [ ] Search for other classes with DI constructors
  - [ ] Refactor any additional classes found

- [ ] **TASK-003.5:** Testing and verification
  - [ ] Run `dotnet build src/api-client/src/AdGuard.ConsoleUI`
  - [ ] Run all unit tests
  - [ ] Perform manual smoke test of ConsoleUI
  - [ ] Verify DI container resolution works correctly

- [ ] **TASK-003.6:** Code review
  - [ ] Review all changes for consistency
  - [ ] Ensure private field usage is correctly converted to parameter usage
  - [ ] Verify XML documentation is preserved
  - [ ] Update ARCHITECTURE.md if needed

---

### SCENARIO-004: Migrate from Newtonsoft.Json to System.Text.Json ??

**Priority:** HIGH | **Effort:** 3-4 hours | **Breaking Changes:** Minimal

**Status:** [ ] Not Started

#### Description
Migrate from Newtonsoft.Json to System.Text.Json for JSON serialization. This provides better performance (2-5x faster), lower memory allocation, and removes an external dependency by using the built-in .NET 8 serializer.

#### Affected Projects
- AdGuard.ApiClient (Core SDK)

#### Impact
- ~50+ model classes
- All API client serialization/deserialization
- JsonSubTypes v2.0.1 dependency (polymorphic serialization)

#### Tasks

- [ ] **TASK-004.1:** Analysis and planning
  - [ ] Audit all Newtonsoft.Json attributes usage
  - [ ] Identify polymorphic serialization needs (JsonSubTypes)
  - [ ] Document custom converters or settings
  - [ ] Identify potential breaking changes
  - [ ] Create migration strategy document

- [ ] **TASK-004.2:** Update project file
  - [ ] Remove `Newtonsoft.Json` package reference from AdGuard.ApiClient.csproj
  - [ ] Add `System.Text.Json` (if not already implicit)
  - [ ] Evaluate `JsonSubTypes` replacement strategy

- [ ] **TASK-004.3:** Update Client infrastructure
  - [ ] Update `ApiClient.cs` serialization configuration
  - [ ] Replace `JsonConvert` calls with `JsonSerializer`
  - [ ] Configure `JsonSerializerOptions` (property naming, null handling)
  - [ ] Update `ClientUtils.cs` if it contains JSON logic

- [ ] **TASK-004.4:** Update Model classes (Phase 1 - Simple models)
  - [ ] Replace `[JsonProperty]` with `[JsonPropertyName]`
  - [ ] Replace `[JsonConverter]` with System.Text.Json equivalents
  - [ ] Replace `[JsonIgnore]` with System.Text.Json version
  - [ ] Test: Device, DNSServer, FilterList models

- [ ] **TASK-004.5:** Update Model classes (Phase 2 - Complex models)
  - [ ] Handle polymorphic types (if JsonSubTypes was used)
  - [ ] Implement custom converters if needed
  - [ ] Update Statistics, QueryLogItem models
  - [ ] Handle date/time serialization

- [ ] **TASK-004.6:** Update Model classes (Phase 3 - Remaining)
  - [ ] Process all remaining model files
  - [ ] Update enum serialization
  - [ ] Update collection handling
  - [ ] Update nullable reference types handling

- [ ] **TASK-004.7:** Update API classes
  - [ ] Update request serialization
  - [ ] Update response deserialization
  - [ ] Update error handling
  - [ ] Test all API endpoints

- [ ] **TASK-004.8:** Handle polymorphism
  - [ ] Identify classes using JsonSubTypes
  - [ ] Implement `JsonDerivedType` attribute (System.Text.Json equivalent)
  - [ ] Create custom converters if needed
  - [ ] Test polymorphic serialization/deserialization

- [ ] **TASK-004.9:** Testing phase 1 - Unit tests
  - [ ] Update test project to use System.Text.Json
  - [ ] Run existing unit tests
  - [ ] Fix any failing tests
  - [ ] Add tests for new serialization scenarios

- [ ] **TASK-004.10:** Testing phase 2 - Integration
  - [ ] Test against AdGuard DNS API (if test account available)
  - [ ] Verify request/response serialization
  - [ ] Test error scenarios
  - [ ] Verify ConsoleUI still works correctly

- [ ] **TASK-004.11:** Performance validation
  - [ ] Run performance benchmarks (if available)
  - [ ] Compare memory allocation
  - [ ] Compare serialization speed
  - [ ] Document improvements

- [ ] **TASK-004.12:** Documentation
  - [ ] Update README with System.Text.Json info
  - [ ] Update CHANGELOG
  - [ ] Document any breaking changes
  - [ ] Update migration guide for consumers

---

### SCENARIO-005: Modernize OpenAPI Generator Configuration ??

**Priority:** LOW | **Effort:** 2 hours | **Breaking Changes:** Potentially Moderate

**Status:** [ ] Not Started

#### Description
Update OpenAPI Generator configuration to generate modern .NET 8 code with System.Text.Json, proper async patterns, and current best practices. This requires regenerating all API client code.

#### Prerequisites
- SCENARIO-004 (System.Text.Json migration) should inform this configuration

#### Tasks

- [ ] **TASK-005.1:** Backup current generated code
  - [ ] Create git branch: `feature/openapi-modernization`
  - [ ] Document current generator version (7.16.0)
  - [ ] Archive current configuration
  - [ ] Note any manual modifications to generated code

- [ ] **TASK-005.2:** Update OpenAPI Generator version
  - [ ] Research latest stable OpenAPI Generator version
  - [ ] Check for .NET 8 support
  - [ ] Review release notes for breaking changes
  - [ ] Update generator tool

- [ ] **TASK-005.3:** Create new generator configuration
  - [ ] Set `jsonLibrary=System.Text.Json`
  - [ ] Set `targetFramework=net8.0`
  - [ ] Set `langVersion=12.0` (C# 12)
  - [ ] Configure to avoid ConfigureAwait(false)
  - [ ] Enable nullable reference types
  - [ ] Review other modern options

- [ ] **TASK-005.4:** Test generation with new config
  - [ ] Generate code to temporary directory
  - [ ] Review generated code quality
  - [ ] Check for ConfigureAwait removal
  - [ ] Verify System.Text.Json usage
  - [ ] Compare with current code

- [ ] **TASK-005.5:** Handle custom modifications
  - [ ] Identify manual changes in current generated code
  - [ ] Document required post-generation modifications
  - [ ] Create scripts/templates if needed
  - [ ] Plan for ongoing maintenance

- [ ] **TASK-005.6:** Regenerate API client
  - [ ] Backup src/AdGuard.ApiClient directory
  - [ ] Run OpenAPI Generator with new config
  - [ ] Review generated code
  - [ ] Apply any required manual modifications

- [ ] **TASK-005.7:** Fix compilation issues
  - [ ] Resolve any breaking changes
  - [ ] Update references in ConsoleUI project
  - [ ] Fix any incompatibilities
  - [ ] Build successfully

- [ ] **TASK-005.8:** Update tests
  - [ ] Update test project for any API changes
  - [ ] Fix failing tests
  - [ ] Add new tests if needed
  - [ ] Verify 100% test pass rate

- [ ] **TASK-005.9:** Integration testing
  - [ ] Test ConsoleUI application
  - [ ] Verify all menu operations work
  - [ ] Test against live API (if possible)
  - [ ] Validate error handling

- [ ] **TASK-005.10:** Documentation
  - [ ] Update README with new generation instructions
  - [ ] Document generator version and config
  - [ ] Update CHANGELOG
  - [ ] Create regeneration guide

---

## ?? Progress Tracking

### Summary Status

| Scenario | Priority | Status | Progress | Completion |
|----------|----------|--------|----------|------------|
| SCENARIO-001: Remove ConfigureAwait | HIGH | [ ] Not Started | 0/5 tasks | 0% |
| SCENARIO-002: Central Package Management | MEDIUM | [ ] Not Started | 0/6 tasks | 0% |
| SCENARIO-003: Primary Constructors | MEDIUM | [ ] Not Started | 0/6 tasks | 0% |
| SCENARIO-004: System.Text.Json Migration | HIGH | [ ] Not Started | 0/12 tasks | 0% |
| SCENARIO-005: OpenAPI Generator Update | LOW | [ ] Not Started | 0/10 tasks | 0% |

**Overall Progress:** 0/39 main tasks (0%)

---

## ?? Recommended Execution Order

1. **SCENARIO-001** (ConfigureAwait) - Quick win, no dependencies, immediate benefit
2. **SCENARIO-002** (CPM) - Infrastructure improvement, helps with dependency management
3. **SCENARIO-003** (Primary Constructors) - ConsoleUI modernization, independent work
4. **SCENARIO-004** (System.Text.Json) - Major performance improvement, prepare for #5
5. **SCENARIO-005** (OpenAPI Generator) - Consider for next major version bump

---

## ?? Notes and Considerations

### General Guidelines
- Create feature branches for each scenario
- Run full test suite after each scenario completion
- Update documentation as you go
- Consider PR reviews for significant changes

### Testing Strategy
- Unit tests must pass: `dotnet test`
- Manual testing of ConsoleUI required
- Consider integration tests with live API (if test account available)
- Performance benchmarks for System.Text.Json migration

### Risk Mitigation
- Backup before major changes (especially #4 and #5)
- Incremental commits with clear messages
- Feature branches for each scenario
- Rollback plan for each scenario

### Dependencies Between Scenarios
- SCENARIO-004 ? SCENARIO-005 (OpenAPI config should match System.Text.Json)
- SCENARIO-001 and SCENARIO-002 are independent
- SCENARIO-003 is independent (ConsoleUI only)

---

## ?? Related Documentation

- [Solution Documentation](../../docs/LINEAR_DOCUMENTATION.md)
- [ConsoleUI Architecture](../AdGuard.ConsoleUI/ARCHITECTURE.md)
- [OpenAPI Generator Documentation](https://openapi-generator.tech/docs/generators/csharp)
- [System.Text.Json Migration Guide](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/migrate-from-newtonsoft)
- [C# 12 Primary Constructors](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-12#primary-constructors)
- [Central Package Management](https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management)

---

## ? Completion Checklist

When all scenarios are complete:

- [ ] All unit tests passing
- [ ] ConsoleUI smoke tested
- [ ] Documentation updated (README, CHANGELOG, ARCHITECTURE)
- [ ] Git branches merged to main
- [ ] Version number bumped appropriately
- [ ] Release notes created
- [ ] NuGet package published (if applicable)

---

**Last Updated:** 2025-01-24  
**Maintained By:** Development Team  
**Review Frequency:** After each scenario completion
