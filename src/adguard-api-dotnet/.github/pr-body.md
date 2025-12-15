## Summary

This PR adds comprehensive integration tests for the AdGuard DNS API .NET client library.

## Changes

### New Files
- `src/AdGuard.ApiClient.Test/Integration/ApiIntegrationTests.cs` - Complete integration test suite with 13 tests
- `TEST_RESULTS.md` - Detailed documentation of test results

### Test Coverage

All 13 tests pass successfully.

The integration tests cover:
1. Account API - Account limits retrieval
2. Devices API - Device listing and management
3. DNS Servers API - DNS server listing
4. Filter Lists API - Filter list retrieval
5. Web Services API - Web services listing
6. Statistics API - Time-based statistics (7 days)
7. Query Log API - Query history (24 hours)
8. Dedicated IP Addresses API - IPv4 address management
9. Configuration Helper - Authentication validation
10. Error Handling - Invalid API key rejection
11. Cancellation Token Support - Async operation cancellation

## Test Results

- Total Tests: 13
- Passed: 13
- Failed: 0
- Duration: approximately 5.3 seconds

## Key Features

- Non-destructive read-only tests
- Proper async/await patterns with ConfigureAwait(false)
- Comprehensive error handling and null checks
- Support for environment variable configuration (ADGUARD_API_KEY)
- Detailed test output with xUnit ITestOutputHelper
- Proper resource disposal using `using` statements
- API key masking in logs for security

## Testing

All tests have been verified with a real API key and pass successfully.

See `TEST_RESULTS.md` for detailed test output and results.

## API Endpoints Tested

All major API endpoints are covered and working correctly:
- /oapi/v1/account/limits
- /oapi/v1/devices
- /oapi/v1/dns_servers
- /oapi/v1/filter_lists
- /oapi/v1/web_services
- /oapi/v1/stats/time
- /oapi/v1/query_log
- /oapi/v1/dedicated_addresses/ipv4

## Notes

- Tests use the API key from the `ADGUARD_API_KEY` environment variable
- All tests are designed to be non-destructive (read-only operations)
- Proper error handling for accounts without certain features (e.g., Dedicated IPs)
