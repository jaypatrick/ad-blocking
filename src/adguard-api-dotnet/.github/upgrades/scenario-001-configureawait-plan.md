# SCENARIO-001: Remove ConfigureAwait(false) - Execution Plan

**Status:** ?? In Progress  
**Started:** 2025-01-24  
**Priority:** HIGH  
**Estimated Time:** 30 minutes  

---

## ?? Overview

Remove all `.ConfigureAwait(false)` calls from the AdGuard.ApiClient generated API code. In .NET 6+, this pattern is no longer necessary in library code as the async infrastructure automatically handles context correctly.

### Why Remove ConfigureAwait(false)?

1. **.NET 8 Improvement**: The async infrastructure in modern .NET handles context correctly by default
2. **Simpler Code**: Reduces visual noise and makes async code more readable
3. **Library Best Practice**: Microsoft's guidance for .NET 6+ library code no longer recommends ConfigureAwait(false)
4. **No Breaking Changes**: This is a safe refactoring with zero functional impact

---

## ?? Affected Files (9 API Classes)

| File | Estimated Occurrences | Status |
|------|----------------------|---------|
| `src/AdGuard.ApiClient/Api/AccountApi.cs` | ~10-15 | [ ] Not Started |
| `src/AdGuard.ApiClient/Api/AuthenticationApi.cs` | ~5-8 | [ ] Not Started |
| `src/AdGuard.ApiClient/Api/DedicatedIPAddressesApi.cs` | 2 | [ ] Not Started |
| `src/AdGuard.ApiClient/Api/DevicesApi.cs` | ~20-25 | [ ] Not Started |
| `src/AdGuard.ApiClient/Api/DNSServersApi.cs` | ~15-20 | [ ] Not Started |
| `src/AdGuard.ApiClient/Api/FilterListsApi.cs` | ~10-15 | [ ] Not Started |
| `src/AdGuard.ApiClient/Api/QueryLogApi.cs` | ~8-12 | [ ] Not Started |
| `src/AdGuard.ApiClient/Api/StatisticsApi.cs` | ~8-12 | [ ] Not Started |
| `src/AdGuard.ApiClient/Api/WebServicesApi.cs` | ~5-8 | [ ] Not Started |

**Total Estimated:** 80-120 occurrences

---

## ?? Step-by-Step Execution

### Phase 1: Setup and Inventory

#### ? STEP 1.1: Create Feature Branch
```bash
git checkout -b feature/remove-configureawait
git status
```

**Status:** [ ] Not Started  
**Time:** 1 minute

---

#### ? STEP 1.2: Run Initial Search and Count
Search for all ConfigureAwait usage:

```bash
# PowerShell
cd D:\source\ad-blocking\src\api-client\src\AdGuard.ApiClient
Get-ChildItem -Path Api -Filter *.cs -Recurse | 
    Select-String -Pattern "\.ConfigureAwait\(" | 
    Group-Object Path | 
    Select-Object Count, Name
```

**Expected Output:** List of files with counts

**Status:** [ ] Not Started  
**Time:** 2 minutes

---

### Phase 2: Remove ConfigureAwait (File by File)

#### ? STEP 2.1: DedicatedIPAddressesApi.cs (Smallest File First)
**File:** `src/AdGuard.ApiClient/Api/DedicatedIPAddressesApi.cs`  
**Expected Changes:** 2 occurrences

**Lines to modify:**
- Line ~410: `AllocateDedicatedIPv4AddressAsync` method
- Line ~520: `ListDedicatedIPv4AddressesAsync` method

**Find:** `.ConfigureAwait(false)`  
**Replace:** `` (empty string)

**Verification:**
```bash
dotnet build src/AdGuard.ApiClient/AdGuard.ApiClient.csproj
```

**Status:** [ ] Not Started  
**Time:** 2 minutes

---

#### ? STEP 2.2: AuthenticationApi.cs
**File:** `src/AdGuard.ApiClient/Api/AuthenticationApi.cs`  
**Expected Changes:** 5-8 occurrences

**Status:** [ ] Not Started  
**Time:** 3 minutes

---

#### ? STEP 2.3: AccountApi.cs
**File:** `src/AdGuard.ApiClient/Api/AccountApi.cs`  
**Expected Changes:** 10-15 occurrences

**Status:** [ ] Not Started  
**Time:** 3 minutes

---

#### ? STEP 2.4: StatisticsApi.cs
**File:** `src/AdGuard.ApiClient/Api/StatisticsApi.cs`  
**Expected Changes:** 8-12 occurrences

**Status:** [ ] Not Started  
**Time:** 3 minutes

---

#### ? STEP 2.5: QueryLogApi.cs
**File:** `src/AdGuard.ApiClient/Api/QueryLogApi.cs`  
**Expected Changes:** 8-12 occurrences

**Status:** [ ] Not Started  
**Time:** 3 minutes

---

#### ? STEP 2.6: FilterListsApi.cs
**File:** `src/AdGuard.ApiClient/Api/FilterListsApi.cs`  
**Expected Changes:** 10-15 occurrences

**Status:** [ ] Not Started  
**Time:** 3 minutes

---

#### ? STEP 2.7: DNSServersApi.cs
**File:** `src/AdGuard.ApiClient/Api/DNSServersApi.cs`  
**Expected Changes:** 15-20 occurrences

**Status:** [ ] Not Started  
**Time:** 4 minutes

---

#### ? STEP 2.8: DevicesApi.cs (Largest File)
**File:** `src/AdGuard.ApiClient/Api/DevicesApi.cs`  
**Expected Changes:** 20-25 occurrences

**Status:** [ ] Not Started  
**Time:** 4 minutes

---

#### ? STEP 2.9: WebServicesApi.cs
**File:** `src/AdGuard.ApiClient/Api/WebServicesApi.cs`  
**Expected Changes:** 5-8 occurrences

**Status:** [ ] Not Started  
**Time:** 3 minutes

---

### Phase 3: Verification and Testing

#### ? STEP 3.1: Verify No ConfigureAwait Remains
```bash
# Search again to confirm all removed
cd D:\source\ad-blocking\src\api-client\src\AdGuard.ApiClient
Get-ChildItem -Path Api -Filter *.cs -Recurse | 
    Select-String -Pattern "\.ConfigureAwait\("
```

**Expected Result:** No matches found

**Status:** [ ] Not Started  
**Time:** 1 minute

---

#### ? STEP 3.2: Build Solution
```bash
cd D:\source\ad-blocking\src\api-client
dotnet restore
dotnet build --no-restore
```

**Expected Result:** Build succeeds with 0 errors

**Status:** [ ] Not Started  
**Time:** 2 minutes

---

#### ? STEP 3.3: Run Unit Tests
```bash
dotnet test --no-build --verbosity normal
```

**Expected Result:** All tests pass

**Status:** [ ] Not Started  
**Time:** 2 minutes

---

#### ? STEP 3.4: Test ConsoleUI Integration
```bash
cd D:\source\ad-blocking\src\api-client\src\AdGuard.ConsoleUI
dotnet build
dotnet run
```

**Manual Test:** Launch ConsoleUI and verify menu navigation works

**Status:** [ ] Not Started  
**Time:** 3 minutes

---

### Phase 4: Documentation and Commit

#### ? STEP 4.1: Update tasks.md
Mark SCENARIO-001 tasks as complete in `.github/upgrades/tasks.md`

**Status:** [ ] Not Started  
**Time:** 1 minute

---

#### ? STEP 4.2: Create Commit
```bash
git add -A
git commit -m "refactor: Remove ConfigureAwait(false) from API client code

- Removed all .ConfigureAwait(false) calls from 9 API classes
- Modern .NET 8 async infrastructure handles context correctly
- No functional changes, pure code quality improvement
- All unit tests passing

Closes SCENARIO-001"
```

**Status:** [ ] Not Started  
**Time:** 1 minute

---

#### ? STEP 4.3: Push Branch
```bash
git push origin feature/remove-configureawait
```

**Status:** [ ] Not Started  
**Time:** 1 minute

---

## ?? Multi-Replace Strategy

For efficiency, you can use the `multi_replace_string_in_file` tool to replace all occurrences across multiple files in one operation.

### Example Pattern:

```csharp
// Before
var localVarResponse = await this.AsynchronousClient.PostAsync<DedicatedIPv4Address>(
    "/oapi/v1/dedicated_addresses/ipv4", 
    localVarRequestOptions, 
    this.Configuration, 
    cancellationToken).ConfigureAwait(false);

// After
var localVarResponse = await this.AsynchronousClient.PostAsync<DedicatedIPv4Address>(
    "/oapi/v1/dedicated_addresses/ipv4", 
    localVarRequestOptions, 
    this.Configuration, 
    cancellationToken);
```

---

## ? Success Criteria

- [ ] All 9 API files processed
- [ ] Zero occurrences of `.ConfigureAwait(` in API folder
- [ ] Solution builds with 0 errors
- [ ] All unit tests pass (100%)
- [ ] ConsoleUI smoke test passes
- [ ] Changes committed to feature branch
- [ ] tasks.md updated

---

## ?? Progress Tracking

**Files Completed:** 0/9 (0%)  
**Estimated Time Remaining:** 30 minutes  
**Actual Time Spent:** 0 minutes  

---

## ?? Rollback Plan

If issues are discovered:

```bash
# Discard all changes
git checkout main
git branch -D feature/remove-configureawait

# Or revert specific file
git checkout HEAD -- src/AdGuard.ApiClient/Api/[FileName].cs
```

---

## ?? Notes

- All changes are in auto-generated code (OpenAPI Generator output)
- Future regeneration of API client may reintroduce ConfigureAwait(false)
- Consider updating OpenAPI Generator config in SCENARIO-005
- This is a safe, non-breaking change
- Focus on *WithHttpInfoAsync methods where ConfigureAwait appears

---

**Last Updated:** 2025-01-24  
**Next Review:** After Phase 3 completion
