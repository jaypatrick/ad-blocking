/*
 * AdGuard DNS API - Unit Tests
 *
 * Tests for RetryPolicyHelper class
 */

using System;
using System.Threading.Tasks;
using AdGuard.ApiClient.Client;
using AdGuard.ApiClient.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AdGuard.ApiClient.Test.Helpers
{
    /// <summary>
    /// Unit tests for <see cref="RetryPolicyHelper"/> class.
    /// </summary>
    public class RetryPolicyHelperTests
    {
        private readonly Mock<ILogger> _mockLogger;

        public RetryPolicyHelperTests()
        {
            _mockLogger = new Mock<ILogger>();
        }

        #region SetLogger Tests

        [Fact]
        public void SetLogger_WithLogger_SetsLogger()
        {
            // Act - should not throw
            RetryPolicyHelper.SetLogger(_mockLogger.Object);

            // Assert - verify logger was invoked during set
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("logger initialized")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void SetLogger_WithNull_SetsNullLogger()
        {
            // Act - should not throw
            RetryPolicyHelper.SetLogger(null);

            // No exception means success
            Assert.True(true);
        }

        #endregion

        #region CreateDefaultRetryPolicy Tests

        [Fact]
        public void CreateDefaultRetryPolicy_DefaultValues_CreatesPolicy()
        {
            // Act
            var policy = RetryPolicyHelper.CreateDefaultRetryPolicy();

            // Assert
            Assert.NotNull(policy);
        }

        [Fact]
        public void CreateDefaultRetryPolicy_CustomValues_CreatesPolicy()
        {
            // Act
            var policy = RetryPolicyHelper.CreateDefaultRetryPolicy(maxRetries: 5, initialDelay: 3);

            // Assert
            Assert.NotNull(policy);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(11)]
        [InlineData(100)]
        public void CreateDefaultRetryPolicy_InvalidMaxRetries_ThrowsArgumentOutOfRangeException(int maxRetries)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                RetryPolicyHelper.CreateDefaultRetryPolicy(maxRetries: maxRetries));
            Assert.Contains("Maximum retries must be between 1 and 10", exception.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(61)]
        [InlineData(100)]
        public void CreateDefaultRetryPolicy_InvalidInitialDelay_ThrowsArgumentOutOfRangeException(int initialDelay)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                RetryPolicyHelper.CreateDefaultRetryPolicy(initialDelay: initialDelay));
            Assert.Contains("Delay must be between 1 and 60", exception.Message);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public void CreateDefaultRetryPolicy_ValidMaxRetries_CreatesPolicy(int maxRetries)
        {
            // Act
            var policy = RetryPolicyHelper.CreateDefaultRetryPolicy(maxRetries: maxRetries);

            // Assert
            Assert.NotNull(policy);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(30)]
        [InlineData(60)]
        public void CreateDefaultRetryPolicy_ValidDelay_CreatesPolicy(int delay)
        {
            // Act
            var policy = RetryPolicyHelper.CreateDefaultRetryPolicy(initialDelay: delay);

            // Assert
            Assert.NotNull(policy);
        }

        #endregion

        #region CreateDefaultRetryPolicy<T> Tests

        [Fact]
        public void CreateDefaultRetryPolicyGeneric_DefaultValues_CreatesPolicy()
        {
            // Act
            var policy = RetryPolicyHelper.CreateDefaultRetryPolicy<string>();

            // Assert
            Assert.NotNull(policy);
        }

        [Fact]
        public void CreateDefaultRetryPolicyGeneric_CustomValues_CreatesPolicy()
        {
            // Act
            var policy = RetryPolicyHelper.CreateDefaultRetryPolicy<int>(maxRetries: 5, initialDelay: 3);

            // Assert
            Assert.NotNull(policy);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(11)]
        public void CreateDefaultRetryPolicyGeneric_InvalidMaxRetries_ThrowsArgumentOutOfRangeException(int maxRetries)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                RetryPolicyHelper.CreateDefaultRetryPolicy<object>(maxRetries: maxRetries));
        }

        #endregion

        #region CreateRateLimitRetryPolicy Tests

        [Fact]
        public void CreateRateLimitRetryPolicy_DefaultValues_CreatesPolicy()
        {
            // Act
            var policy = RetryPolicyHelper.CreateRateLimitRetryPolicy();

            // Assert
            Assert.NotNull(policy);
        }

        [Fact]
        public void CreateRateLimitRetryPolicy_CustomValues_CreatesPolicy()
        {
            // Act
            var policy = RetryPolicyHelper.CreateRateLimitRetryPolicy(maxRetries: 10, baseDelay: 10);

            // Assert
            Assert.NotNull(policy);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        [InlineData(11)]
        public void CreateRateLimitRetryPolicy_InvalidMaxRetries_ThrowsArgumentOutOfRangeException(int maxRetries)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                RetryPolicyHelper.CreateRateLimitRetryPolicy(maxRetries: maxRetries));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(61)]
        public void CreateRateLimitRetryPolicy_InvalidBaseDelay_ThrowsArgumentOutOfRangeException(int baseDelay)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                RetryPolicyHelper.CreateRateLimitRetryPolicy(baseDelay: baseDelay));
        }

        #endregion

        #region ExecuteWithRetryAsync<T> Tests

        [Fact]
        public async Task ExecuteWithRetryAsync_SuccessfulCall_ReturnsResult()
        {
            // Arrange
            var expectedResult = "Success";
            Func<Task<string>> apiCall = () => Task.FromResult(expectedResult);

            // Act
            var result = await RetryPolicyHelper.ExecuteWithRetryAsync(apiCall);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task ExecuteWithRetryAsync_NullApiCall_ThrowsArgumentNullException()
        {
            // Arrange
            Func<Task<string>>? apiCall = null;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
                RetryPolicyHelper.ExecuteWithRetryAsync(apiCall!));
            Assert.Equal("apiCall", exception.ParamName);
        }

        [Fact]
        public async Task ExecuteWithRetryAsync_InvalidMaxRetries_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            Func<Task<string>> apiCall = () => Task.FromResult("test");

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                RetryPolicyHelper.ExecuteWithRetryAsync(apiCall, maxRetries: 0));
        }

        [Fact]
        public async Task ExecuteWithRetryAsync_NonRetryableError_ThrowsImmediately()
        {
            // Arrange
            var callCount = 0;
            Func<Task<string>> apiCall = () =>
            {
                callCount++;
                throw new ApiException(400, "Bad Request");
            };

            // Act & Assert
            await Assert.ThrowsAsync<ApiException>(() =>
                RetryPolicyHelper.ExecuteWithRetryAsync(apiCall, maxRetries: 3));

            // Should only be called once since 400 is not retryable
            Assert.Equal(1, callCount);
        }

        [Fact]
        public async Task ExecuteWithRetryAsync_RetryableError_RetriesUntilSuccess()
        {
            // Arrange
            var callCount = 0;
            Func<Task<string>> apiCall = () =>
            {
                callCount++;
                if (callCount < 3)
                {
                    throw new ApiException(429, "Too Many Requests");
                }
                return Task.FromResult("Success");
            };

            // Act
            var result = await RetryPolicyHelper.ExecuteWithRetryAsync(apiCall, maxRetries: 5);

            // Assert
            Assert.Equal("Success", result);
            Assert.Equal(3, callCount); // 2 retries + 1 success
        }

        #endregion

        #region ExecuteWithRetryAsync (void) Tests

        [Fact]
        public async Task ExecuteWithRetryAsyncVoid_SuccessfulCall_Completes()
        {
            // Arrange
            var executed = false;
            Func<Task> apiCall = () =>
            {
                executed = true;
                return Task.CompletedTask;
            };

            // Act
            await RetryPolicyHelper.ExecuteWithRetryAsync(apiCall);

            // Assert
            Assert.True(executed);
        }

        [Fact]
        public async Task ExecuteWithRetryAsyncVoid_NullApiCall_ThrowsArgumentNullException()
        {
            // Arrange
            Func<Task>? apiCall = null;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
                RetryPolicyHelper.ExecuteWithRetryAsync(apiCall!));
            Assert.Equal("apiCall", exception.ParamName);
        }

        [Fact]
        public async Task ExecuteWithRetryAsyncVoid_RetryableError_RetriesUntilSuccess()
        {
            // Arrange
            var callCount = 0;
            Func<Task> apiCall = () =>
            {
                callCount++;
                if (callCount < 2)
                {
                    throw new ApiException(500, "Internal Server Error");
                }
                return Task.CompletedTask;
            };

            // Act
            await RetryPolicyHelper.ExecuteWithRetryAsync(apiCall, maxRetries: 3);

            // Assert
            Assert.Equal(2, callCount);
        }

        #endregion

        #region IsRetryableException Tests

        [Fact]
        public void IsRetryableException_NullException_ReturnsFalse()
        {
            // Act
            var result = RetryPolicyHelper.IsRetryableException(null!);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData(429)]  // Rate Limit
        [InlineData(408)]  // Request Timeout
        [InlineData(500)]  // Internal Server Error
        [InlineData(502)]  // Bad Gateway
        [InlineData(503)]  // Service Unavailable
        [InlineData(504)]  // Gateway Timeout
        [InlineData(599)]  // Arbitrary 5xx error
        public void IsRetryableException_RetryableStatusCode_ReturnsTrue(int statusCode)
        {
            // Arrange
            var exception = new ApiException(statusCode, "Error");

            // Act
            var result = RetryPolicyHelper.IsRetryableException(exception);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(400)]  // Bad Request
        [InlineData(401)]  // Unauthorized
        [InlineData(403)]  // Forbidden
        [InlineData(404)]  // Not Found
        [InlineData(405)]  // Method Not Allowed
        [InlineData(409)]  // Conflict
        [InlineData(422)]  // Unprocessable Entity
        public void IsRetryableException_NonRetryableStatusCode_ReturnsFalse(int statusCode)
        {
            // Arrange
            var exception = new ApiException(statusCode, "Error");

            // Act
            var result = RetryPolicyHelper.IsRetryableException(exception);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData(200)]  // OK
        [InlineData(201)]  // Created
        [InlineData(204)]  // No Content
        [InlineData(301)]  // Moved Permanently
        [InlineData(302)]  // Found
        public void IsRetryableException_SuccessOrRedirectStatusCode_ReturnsFalse(int statusCode)
        {
            // Arrange
            var exception = new ApiException(statusCode, "Not an error");

            // Act
            var result = RetryPolicyHelper.IsRetryableException(exception);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task Policy_ExhaustsRetries_ThrowsFinalException()
        {
            // Arrange
            var callCount = 0;
            var policy = RetryPolicyHelper.CreateDefaultRetryPolicy(maxRetries: 2, initialDelay: 1);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(async () =>
            {
                await policy.ExecuteAsync(() =>
                {
                    callCount++;
                    throw new ApiException(500, "Server Error");
                });
            });

            // Should be called 3 times (initial + 2 retries)
            Assert.Equal(3, callCount);
            Assert.Equal(500, exception.ErrorCode);
        }

        [Fact]
        public async Task GenericPolicy_ExhaustsRetries_ThrowsFinalException()
        {
            // Arrange
            var callCount = 0;
            var policy = RetryPolicyHelper.CreateDefaultRetryPolicy<string>(maxRetries: 2, initialDelay: 1);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(async () =>
            {
                await policy.ExecuteAsync(() =>
                {
                    callCount++;
                    throw new ApiException(503, "Service Unavailable");
                    #pragma warning disable CS0162 // Unreachable code
                    return Task.FromResult("never");
                    #pragma warning restore CS0162
                });
            });

            Assert.Equal(3, callCount);
            Assert.Equal(503, exception.ErrorCode);
        }

        [Fact]
        public async Task RateLimitPolicy_OnlyHandles429_DoesNotRetryOtherErrors()
        {
            // Arrange
            var callCount = 0;
            var policy = RetryPolicyHelper.CreateRateLimitRetryPolicy(maxRetries: 5, baseDelay: 1);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(async () =>
            {
                await policy.ExecuteAsync(() =>
                {
                    callCount++;
                    throw new ApiException(500, "Server Error");
                });
            });

            // Should only be called once since 500 is not handled by rate limit policy
            Assert.Equal(1, callCount);
            Assert.Equal(500, exception.ErrorCode);
        }

        #endregion
    }
}
