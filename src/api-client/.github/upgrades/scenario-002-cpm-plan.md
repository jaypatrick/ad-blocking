# SCENARIO-002: Central Package Management - Execution Plan

**Status:** ?? In Progress  
**Started:** 2025-01-24  
**Priority:** MEDIUM  
**Estimated Time:** 1 hour  

---

## ?? Overview

Implement Central Package Management (CPM) to manage all NuGet package versions from a single `Directory.Packages.props` file at the solution root. This provides a single source of truth for package versions across all 3 projects.

### Why Central Package Management?

1. **Single Source of Truth**: All package versions defined in one place
2. **Consistency**: Same package versions across all projects automatically
3. **Easier Updates**: Update security patches once, applies everywhere
4. **Reduced Conflicts**: No more merge conflicts on package versions
5. **Better Visibility**: Easy to see all dependencies at a glance

---

## ?? Current State Analysis

### Project Structure
```
src/api-client/
??? src/
?   ??? AdGuard.ApiClient/
?   ?   ??? AdGuard.ApiClient.csproj
?   ??? AdGuard.ConsoleUI/
?   ?   ??? AdGuard.ConsoleUI.csproj
?   ??? AdGuard.ApiClient.Test/
?       ??? AdGuard.ApiClient.Test.csproj
??? AdGuard.ApiClient.slnx
```

### Current Package Management
- ? **Decentralized**: Versions in each project file
- ? **Inconsistent**: Microsoft.Extensions.Logging.Abstractions has v10.0.0 in multiple projects
- ?? **Opt-Out**: AdGuard.ApiClient explicitly disables CPM with `<ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>`

---

## ?? Package Inventory

### AdGuard.ApiClient (4 packages)
```xml
<PackageReference Include="JsonSubTypes" Version="2.0.1" />
<PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="10.0.0" />
<PackageReference Include="Newtonsoft.Json" Version="13.0.4" />
<PackageReference Include="Polly" Version="8.6.5" />
```

### AdGuard.ConsoleUI (9 packages)
```xml
<PackageReference Include="Microsoft.Extensions.Configuration" Version="10.0.0" />
<PackageReference Include="Microsoft.Extensions.Configuration.EnvironmentVariables" Version="10.0.0" />
<PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="10.0.0" />
<PackageReference Include="Microsoft.Extensions.Configuration.UserSecrets" Version="10.0.0" />
<PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="10.0.0" />
<PackageReference Include="Microsoft.Extensions.Hosting" Version="10.0.0" />
<PackageReference Include="Microsoft.Extensions.Logging" Version="10.0.0" />
<PackageReference Include="Microsoft.Extensions.Logging.Console" Version="10.0.0" />
<PackageReference Include="Spectre.Console" Version="0.54.0" />
```

### AdGuard.ApiClient.Test (5 packages)
```xml
<PackageReference Include="coverlet.collector" Version="6.0.4" />
<PackageReference Include="Microsoft.Extensions.Logging" Version="10.0.0" />
<PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="10.0.0" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="18.0.1" />
<PackageReference Include="Moq" Version="4.20.72" />
<PackageReference Include="xunit" Version="2.9.3" />
<PackageReference Include="xunit.runner.visualstudio" Version="3.1.5" />
```

**Total Unique Packages:** 16

---

## ?? Step-by-Step Execution

### Phase 1: Create Directory.Packages.props

#### ? STEP 2.1: Create Directory.Packages.props File
**Location:** `src/api-client/Directory.Packages.props`

**Content:**
```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  
  <ItemGroup>
    <!-- API Client Dependencies -->
    <PackageVersion Include="JsonSubTypes" Version="2.0.1" />
    <PackageVersion Include="Newtonsoft.Json" Version="13.0.4" />
    <PackageVersion Include="Polly" Version="8.6.5" />
    
    <!-- Microsoft.Extensions (v10.0.0) -->
    <PackageVersion Include="Microsoft.Extensions.Configuration" Version="10.0.0" />
    <PackageVersion Include="Microsoft.Extensions.Configuration.EnvironmentVariables" Version="10.0.0" />
    <PackageVersion Include="Microsoft.Extensions.Configuration.Json" Version="10.0.0" />
    <PackageVersion Include="Microsoft.Extensions.Configuration.UserSecrets" Version="10.0.0" />
    <PackageVersion Include="Microsoft.Extensions.DependencyInjection" Version="10.0.0" />
    <PackageVersion Include="Microsoft.Extensions.Hosting" Version="10.0.0" />
    <PackageVersion Include="Microsoft.Extensions.Logging" Version="10.0.0" />
    <PackageVersion Include="Microsoft.Extensions.Logging.Abstractions" Version="10.0.0" />
    <PackageVersion Include="Microsoft.Extensions.Logging.Console" Version="10.0.0" />
    
    <!-- UI Dependencies -->
    <PackageVersion Include="Spectre.Console" Version="0.54.0" />
    
    <!-- Testing Dependencies -->
    <PackageVersion Include="coverlet.collector" Version="6.0.4" />
    <PackageVersion Include="Microsoft.NET.Test.Sdk" Version="18.0.1" />
    <PackageVersion Include="Moq" Version="4.20.72" />
    <PackageVersion Include="xunit" Version="2.9.3" />
    <PackageVersion Include="xunit.runner.visualstudio" Version="3.1.5" />
  </ItemGroup>
</Project>
```

**Status:** [ ] Not Started  
**Time:** 5 minutes

---

### Phase 2: Update Project Files

#### ? STEP 2.2: Update AdGuard.ApiClient.csproj
**File:** `src/AdGuard.ApiClient/AdGuard.ApiClient.csproj`

**Changes:**
1. Remove `<ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>`
2. Remove `Version` attributes from all `<PackageReference>` elements

**Before:**
```xml
<PropertyGroup>
  <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
</PropertyGroup>
<ItemGroup>
  <PackageReference Include="JsonSubTypes" Version="2.0.1" />
  <PackageReference Include="Newtonsoft.Json" Version="13.0.4" />
  <!-- etc -->
</ItemGroup>
```

**After:**
```xml
<!-- Property removed -->
<ItemGroup>
  <PackageReference Include="JsonSubTypes" />
  <PackageReference Include="Newtonsoft.Json" />
  <!-- etc -->
</ItemGroup>
```

**Status:** [ ] Not Started  
**Time:** 5 minutes

---

#### ? STEP 2.3: Update AdGuard.ConsoleUI.csproj
**File:** `src/AdGuard.ConsoleUI/AdGuard.ConsoleUI.csproj`

**Changes:**
- Remove `Version` attributes from all 9 `<PackageReference>` elements

**Before:**
```xml
<PackageReference Include="Microsoft.Extensions.Configuration" Version="10.0.0" />
```

**After:**
```xml
<PackageReference Include="Microsoft.Extensions.Configuration" />
```

**Status:** [ ] Not Started  
**Time:** 5 minutes

---

#### ? STEP 2.4: Update AdGuard.ApiClient.Test.csproj
**File:** `src/AdGuard.ApiClient.Test/AdGuard.ApiClient.Test.csproj`

**Changes:**
- Remove `Version` attributes from all package references
- Keep `PrivateAssets` and `IncludeAssets` attributes for coverlet.collector

**Before:**
```xml
<PackageReference Include="coverlet.collector" Version="6.0.4">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
</PackageReference>
```

**After:**
```xml
<PackageReference Include="coverlet.collector">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
</PackageReference>
```

**Status:** [ ] Not Started  
**Time:** 5 minutes

---

### Phase 3: Verification

#### ? STEP 2.5: Restore and Build
```bash
cd D:\source\ad-blocking\src\api-client
dotnet restore
dotnet build
```

**Expected Result:** Build succeeds, packages restored from central management

**Status:** [ ] Not Started  
**Time:** 3 minutes

---

#### ? STEP 2.6: Verify Package Versions
```bash
# List all package references to verify versions
dotnet list package
```

**Expected:** All packages show v10.0.0 for Microsoft.Extensions.* family

**Status:** [ ] Not Started  
**Time:** 2 minutes

---

#### ? STEP 2.7: Run Tests
```bash
dotnet test
```

**Expected Result:** All tests pass

**Status:** [ ] Not Started  
**Time:** 3 minutes

---

### Phase 4: Documentation

#### ? STEP 2.8: Create CPM Documentation
Create `src/api-client/docs/PACKAGE-MANAGEMENT.md`

**Content:**
```markdown
# Central Package Management

This solution uses [Central Package Management (CPM)](https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management) to manage NuGet package versions.

## Overview

All package versions are defined in `Directory.Packages.props` at the solution root. Individual project files reference packages without specifying versions.

## Adding a New Package

1. Add version to `Directory.Packages.props`:
   ```xml
   <PackageVersion Include="NewPackage" Version="1.0.0" />
   ```

2. Reference in project file (no version):
   ```xml
   <PackageReference Include="NewPackage" />
   ```

## Updating Package Versions

Update the version in `Directory.Packages.props` only. All projects will automatically use the new version.

## Benefits

- Single source of truth for package versions
- Consistent versions across all projects
- Easier security updates
- Reduced merge conflicts
```

**Status:** [ ] Not Started  
**Time:** 5 minutes

---

#### ? STEP 2.9: Update tasks.md
Mark SCENARIO-002 as complete

**Status:** [ ] Not Started  
**Time:** 2 minutes

---

#### ? STEP 2.10: Commit Changes
```bash
git add -A
git commit -m "feat: Implement Central Package Management (CPM)

- Created Directory.Packages.props with 16 package definitions
- Updated 3 project files to use centralized versions
- Removed ManagePackageVersionsCentrally=false from ApiClient
- All packages now managed from single source of truth
- Build and tests passing

Benefits:
- Consistent package versions across solution
- Easier dependency updates
- Reduced merge conflicts
- Better visibility of all dependencies

Closes: SCENARIO-002"
```

**Status:** [ ] Not Started  
**Time:** 2 minutes

---

## ?? Success Criteria

- [ ] Directory.Packages.props created with all 16 packages
- [ ] All 3 project files updated (versions removed)
- [ ] ManagePackageVersionsCentrally property removed from ApiClient
- [ ] Solution builds successfully
- [ ] All tests pass
- [ ] `dotnet list package` shows correct versions
- [ ] Documentation created
- [ ] Changes committed

---

## ?? Potential Issues & Solutions

### Issue 1: Package Version Conflicts
**Symptom:** Build errors about version conflicts  
**Solution:** Verify all projects using same version in Directory.Packages.props

### Issue 2: Package Not Found
**Symptom:** NuGet restore fails  
**Solution:** Check package name spelling in Directory.Packages.props matches project files

### Issue 3: Tests Fail
**Symptom:** Previously passing tests fail  
**Solution:** Verify test package versions match previous versions

---

## ?? Progress Tracking

**Phase 1:** Create Directory.Packages.props  
**Status:** [ ] Not Started  
**Time Remaining:** 5 minutes

**Phase 2:** Update Project Files  
**Status:** [ ] Not Started  
**Time Remaining:** 15 minutes

**Phase 3:** Verification  
**Status:** [ ] Not Started  
**Time Remaining:** 8 minutes

**Phase 4:** Documentation  
**Status:** [ ] Not Started  
**Time Remaining:** 7 minutes

**Total Progress:** 0/10 steps (0%)

---

## ?? Rollback Plan

If issues occur:

```bash
# Revert all changes
git checkout HEAD -- Directory.Packages.props
git checkout HEAD -- src/AdGuard.ApiClient/AdGuard.ApiClient.csproj
git checkout HEAD -- src/AdGuard.ConsoleUI/AdGuard.ConsoleUI.csproj
git checkout HEAD -- src/AdGuard.ApiClient.Test/AdGuard.ApiClient.Test.csproj

# Or reset entire branch
git reset --hard HEAD
```

---

**Next Review:** After Phase 3 completion  
**Last Updated:** 2025-01-24
