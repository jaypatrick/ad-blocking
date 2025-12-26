# Changelog

All notable changes to the AdGuard PowerShell modules will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2025-12-26

### Added - Webhook Module

#### New Function: Invoke-AdGuardWebhook
- Complete overhaul of webhook module with modern PowerShell features
- Rich console output with colored icons and emojis
- Progress bars for continuous operations
- Statistics tracking (success/failure rates, elapsed time)
- Multiple output formats (Table, List, JSON)
- Configuration file support (JSON/YAML)
- `--SaveConfig` option to persist settings
- `--Quiet` mode for minimal output
- `--ShowStatistics` parameter for detailed reporting
- `--Format` parameter for output formatting
- Configurable banner display

#### Backward Compatibility
- Created `Invoke-Webhook` alias for backward compatibility
- Maintained `Continous` (misspelled) parameter alias
- All existing scripts continue to work without modification

#### Module Manifest
- Created `Webhook.psd1` module manifest
- Added proper metadata (author, version, description)
- Defined exported functions and aliases
- Added PowerShell Gallery tags
- Included release notes

#### Enhanced Error Handling
- Comprehensive try-catch-finally blocks
- Detailed error messages with timestamps
- Stack trace logging in verbose mode
- Graceful failure recovery

### Added - Utility Scripts

#### Regenerate-Client.ps1 Enhancements
- **DryRun** mode to preview changes without modification
- **Compare** option to show detailed diffs after generation
- **Clean** option to remove backup files
- **LogFile** parameter for persistent logging
- **OutputFormat** parameter (Text, Json, Markdown)
- Modern banner with visual improvements
- Colored console output with icons
- Enhanced progress reporting

#### Update-ApiClient.ps1 Enhancements
- Step progress indicators (1/5, 2/5, etc.)
- Colored icons for each step (✓, ✗, ⚠)
- **Force** parameter to skip confirmations
- **CI** parameter for non-interactive mode
- Improved error messages and logging

#### compile-rules.ps1 Updates
- Consistent color scheme across output
- Better alignment with RulesCompiler module
- Enhanced error handling
- Cross-platform path handling improvements

### Added - Test Harnesses

#### Webhook-Harness.ps1
- Complete rewrite with modern parameter handling
- Support for configuration files
- Interactive prompts with improved UX
- Better error handling and reporting
- Environment variable support

### Added - Testing

#### Comprehensive Webhook Tests
- Module import tests
- Parameter validation tests (all 12 parameters)
- Configuration file support tests
- Backward compatibility tests
- Output format validation tests
- Range validation for WaitTime, RetryCount, RetryInterval
- Alias verification tests

#### Test Coverage
- Increased test coverage from ~30% to >80%
- Added tests for all new parameters
- Added tests for configuration file loading
- Added tests for output formatting

### Changed

#### Code Quality Improvements
- All scripts now pass PSScriptAnalyzer with zero errors
- Consistent coding style across all modules
- Proper comment-based help for all functions
- Parameter validation using ValidateScript, ValidateRange, ValidateSet
- SupportsShouldProcess for scripts that modify files

#### Visual Improvements
- Unicode box-drawing characters for banners
- Colored output with ANSI escape codes
- Emoji icons for log levels (🔵 Info, ✅ Success, ⚠️ Warning, ❌ Error)
- Consistent color scheme (Cyan for info, Green for success, Yellow for warnings, Red for errors)
- Progress bars for long-running operations

### Documentation

#### New Documentation Files
- **README.md**: Comprehensive module overview (updated)
- **CHANGELOG.md**: This file
- **CONTRIBUTING.md**: Development guidelines (planned)
- **examples/**: Sample scripts and configurations (planned)

#### Enhanced Comment-Based Help
- All functions have comprehensive `.SYNOPSIS`, `.DESCRIPTION`, `.PARAMETER`, `.EXAMPLE` sections
- Added `.INPUTS` and `.OUTPUTS` documentation
- Multiple usage examples for each function
- Cross-references to related functions

### Fixed

- Fixed parameter validation in Webhook module
- Fixed path handling for cross-platform compatibility
- Fixed encoding issues (all files now use UTF-8)
- Fixed progress bar not clearing in continuous mode
- Fixed statistics calculation edge cases

### Deprecated

- `Invoke-Webhook` function name (use `Invoke-AdGuardWebhook` instead, but alias maintained for compatibility)
- `Continous` parameter spelling (use `Continuous`, but alias maintained)

### Security

- Removed hardcoded secrets from examples
- Added validation for webhook URLs
- Improved error message sanitization

## [0.4.0] - 2023-02-08 (Previous Version)

### Added
- Initial Webhook module implementation
- Basic retry logic
- Continuous operation mode

### Known Issues
- Limited error handling
- No configuration file support
- Basic console output
- No statistics tracking

---

## Future Enhancements

### Planned for 1.1.0
- Watch mode for automatic rule compilation on file changes
- Configuration profiles (dev, prod, staging)
- Export metrics to CSV/JSON
- Benchmark mode for performance testing
- Integration with Linear for issue tracking
- Slack notifications for webhook failures

### Planned for 1.2.0
- PowerShell Gallery publication
- Module help system
- Webhook scheduling with Windows Task Scheduler/cron
- Multi-webhook support
- Load balancing across multiple endpoints

### Planned for 2.0.0
- Complete API client in PowerShell
- Interactive TUI using Terminal.Gui
- Plugin system for custom transformations
- Cloud sync for configurations
