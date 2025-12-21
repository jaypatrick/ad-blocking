

/*
 * AdGuard DNS API - Unit Tests
 *
 * Tests for ConfigurationHelper class
 */

namespace AdGuard.ApiClient.Test.Helpers
{
    /// <summary>
    /// Unit tests for <see cref="ConfigurationHelper"/> class.
    /// </summary>
    public class ConfigurationHelperTests
    {
        private readonly Mock<ILogger> _mockLogger;

        public ConfigurationHelperTests()
        {
            _mockLogger = new Mock<ILogger>();
        }

        #region CreateWithApiKey Tests

        [Fact]
        public void CreateWithApiKey_ValidApiKey_ReturnsConfiguration()
        {
            // Arrange
            var apiKey = "test-api-key-12345";

            // Act
            var config = ConfigurationHelper.CreateWithApiKey(apiKey);

            // Assert
            Assert.NotNull(config);
            Assert.Equal("https://api.adguard-dns.io", config.BasePath);
            Assert.NotNull(config.ApiKey);
            Assert.Contains("Authorization", config.ApiKey.Keys);
            Assert.Equal($"ApiKey {apiKey}", config.ApiKey["Authorization"]);
        }

        [Fact]
        public void CreateWithApiKey_CustomBasePath_UsesCustomBasePath()
        {
            // Arrange
            var apiKey = "test-api-key";
            var customBasePath = "https://custom.api.example.com";

            // Act
            var config = ConfigurationHelper.CreateWithApiKey(apiKey, customBasePath);

            // Assert
            Assert.Equal(customBasePath, config.BasePath);
        }

        [Fact]
        public void CreateWithApiKey_WithLogger_LogsInformation()
        {
            // Arrange
            var apiKey = "test-api-key";

            // Act
            var config = ConfigurationHelper.CreateWithApiKey(apiKey, logger: _mockLogger.Object);

            // Assert
            Assert.NotNull(config);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Configuration created successfully")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        public void CreateWithApiKey_NullOrEmptyApiKey_ThrowsArgumentException(string? apiKey)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                ConfigurationHelper.CreateWithApiKey(apiKey!));
            Assert.Equal("apiKey", exception.ParamName);
            Assert.Contains("null, empty, or whitespace", exception.Message);
        }

        [Fact]
        public void CreateWithApiKey_InvalidBasePath_ThrowsArgumentException()
        {
            // Arrange
            var apiKey = "valid-api-key";
            var invalidBasePath = "not-a-valid-uri";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                ConfigurationHelper.CreateWithApiKey(apiKey, invalidBasePath));
            Assert.Equal("basePath", exception.ParamName);
            Assert.Contains("not a valid URI", exception.Message);
        }

        #endregion

        #region CreateWithBearerToken Tests

        [Fact]
        public void CreateWithBearerToken_ValidToken_ReturnsConfiguration()
        {
            // Arrange
            var accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";

            // Act
            var config = ConfigurationHelper.CreateWithBearerToken(accessToken);

            // Assert
            Assert.NotNull(config);
            Assert.Equal("https://api.adguard-dns.io", config.BasePath);
            Assert.Equal(accessToken, config.AccessToken);
        }

        [Fact]
        public void CreateWithBearerToken_CustomBasePath_UsesCustomBasePath()
        {
            // Arrange
            var accessToken = "test-token";
            var customBasePath = "https://custom.api.example.com";

            // Act
            var config = ConfigurationHelper.CreateWithBearerToken(accessToken, customBasePath);

            // Assert
            Assert.Equal(customBasePath, config.BasePath);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateWithBearerToken_NullOrEmptyToken_ThrowsArgumentException(string? token)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                ConfigurationHelper.CreateWithBearerToken(token!));
            Assert.Equal("accessToken", exception.ParamName);
        }

        #endregion

        #region CreateCustom Tests

        [Fact]
        public void CreateCustom_DefaultValues_ReturnsDefaultConfiguration()
        {
            // Act
            var config = ConfigurationHelper.CreateCustom();

            // Assert
            Assert.NotNull(config);
            Assert.Equal("https://api.adguard-dns.io", config.BasePath);
        }

        [Fact]
        public void CreateCustom_WithTimeout_SetsTimeout()
        {
            // Arrange
            var timeout = 60000;

            // Act
            var config = ConfigurationHelper.CreateCustom(timeout: timeout);

            // Assert
            Assert.Equal(TimeSpan.FromMilliseconds(timeout), config.Timeout);
        }

        [Fact]
        public void CreateCustom_WithUserAgent_SetsUserAgent()
        {
            // Arrange
            var userAgent = "MyApp/1.0";

            // Act
            var config = ConfigurationHelper.CreateCustom(userAgent: userAgent);

            // Assert
            Assert.Equal(userAgent, config.UserAgent);
        }

        [Theory]
        [InlineData(500)]     // Too low
        [InlineData(999)]     // Just below minimum
        [InlineData(300001)]  // Just above maximum
        [InlineData(1000000)] // Way too high
        public void CreateCustom_InvalidTimeout_ThrowsArgumentOutOfRangeException(int timeout)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                ConfigurationHelper.CreateCustom(timeout: timeout));
            Assert.Equal("timeout", exception.ParamName);
        }

        [Theory]
        [InlineData(1000)]    // Minimum valid
        [InlineData(30000)]   // Default
        [InlineData(300000)]  // Maximum valid
        public void CreateCustom_ValidTimeout_SetsTimeout(int timeout)
        {
            // Act
            var config = ConfigurationHelper.CreateCustom(timeout: timeout);

            // Assert
            Assert.Equal(TimeSpan.FromMilliseconds(timeout), config.Timeout);
        }

        [Fact]
        public void CreateCustom_EmptyUserAgent_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                ConfigurationHelper.CreateCustom(userAgent: ""));
            Assert.Equal("userAgent", exception.ParamName);
        }

        #endregion

        #region WithApiKey Extension Tests

        [Fact]
        public void WithApiKey_ValidConfiguration_AddsApiKey()
        {
            // Arrange
            var config = new Configuration();
            var apiKey = "test-api-key";

            // Act
            var result = config.WithApiKey(apiKey);

            // Assert
            Assert.Same(config, result);
            Assert.NotNull(config.ApiKey);
            Assert.Equal($"ApiKey {apiKey}", config.ApiKey["Authorization"]);
        }

        [Fact]
        public void WithApiKey_NullConfiguration_ThrowsArgumentNullException()
        {
            // Arrange
            Configuration? config = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                config!.WithApiKey("test-key"));
            Assert.Equal("configuration", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void WithApiKey_NullOrEmptyApiKey_ThrowsArgumentException(string? apiKey)
        {
            // Arrange
            var config = new Configuration();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                config.WithApiKey(apiKey!));
            Assert.Equal("apiKey", exception.ParamName);
        }

        [Fact]
        public void WithApiKey_ExistingApiKey_OverwritesApiKey()
        {
            // Arrange
            var config = new Configuration
            {
                ApiKey = new Dictionary<string, string>
                {
                    { "Authorization", "ApiKey old-key" }
                }
            };

            // Act
            config.WithApiKey("new-key");

            // Assert
            Assert.Equal("ApiKey new-key", config.ApiKey["Authorization"]);
        }

        #endregion

        #region WithBearerToken Extension Tests

        [Fact]
        public void WithBearerToken_ValidConfiguration_AddsBearerToken()
        {
            // Arrange
            var config = new Configuration();
            var token = "test-bearer-token";

            // Act
            var result = config.WithBearerToken(token);

            // Assert
            Assert.Same(config, result);
            Assert.Equal(token, config.AccessToken);
        }

        [Fact]
        public void WithBearerToken_NullConfiguration_ThrowsArgumentNullException()
        {
            // Arrange
            Configuration? config = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                config!.WithBearerToken("test-token"));
            Assert.Equal("configuration", exception.ParamName);
        }

        #endregion

        #region WithTimeout Extension Tests

        [Fact]
        public void WithTimeout_ValidConfiguration_SetsTimeout()
        {
            // Arrange
            var config = new Configuration();
            var timeout = 45000;

            // Act
            var result = config.WithTimeout(timeout);

            // Assert
            Assert.Same(config, result);
            Assert.Equal(TimeSpan.FromMilliseconds(timeout), config.Timeout);
        }

        [Fact]
        public void WithTimeout_NullConfiguration_ThrowsArgumentNullException()
        {
            // Arrange
            Configuration? config = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                config!.WithTimeout(30000));
            Assert.Equal("configuration", exception.ParamName);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(500)]
        [InlineData(999)]
        public void WithTimeout_TooLowTimeout_ThrowsArgumentOutOfRangeException(int timeout)
        {
            // Arrange
            var config = new Configuration();

            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                config.WithTimeout(timeout));
            Assert.Equal("timeoutMilliseconds", exception.ParamName);
        }

        [Theory]
        [InlineData(300001)]
        [InlineData(500000)]
        public void WithTimeout_TooHighTimeout_ThrowsArgumentOutOfRangeException(int timeout)
        {
            // Arrange
            var config = new Configuration();

            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                config.WithTimeout(timeout));
            Assert.Equal("timeoutMilliseconds", exception.ParamName);
        }

        #endregion

        #region WithUserAgent Extension Tests

        [Fact]
        public void WithUserAgent_ValidConfiguration_SetsUserAgent()
        {
            // Arrange
            var config = new Configuration();
            var userAgent = "TestApp/2.0";

            // Act
            var result = config.WithUserAgent(userAgent);

            // Assert
            Assert.Same(config, result);
            Assert.Equal(userAgent, config.UserAgent);
        }

        [Fact]
        public void WithUserAgent_NullConfiguration_ThrowsArgumentNullException()
        {
            // Arrange
            Configuration? config = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                config!.WithUserAgent("TestApp/1.0"));
            Assert.Equal("configuration", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void WithUserAgent_NullOrEmptyUserAgent_ThrowsArgumentException(string? userAgent)
        {
            // Arrange
            var config = new Configuration();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                config.WithUserAgent(userAgent!));
            Assert.Equal("userAgent", exception.ParamName);
        }

        #endregion

        #region ValidateAuthentication Tests

        [Fact]
        public void ValidateAuthentication_WithApiKey_ReturnsTrue()
        {
            // Arrange
            var config = ConfigurationHelper.CreateWithApiKey("test-api-key");

            // Act
            var result = ConfigurationHelper.ValidateAuthentication(config);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateAuthentication_WithBearerToken_ReturnsTrue()
        {
            // Arrange
            var config = ConfigurationHelper.CreateWithBearerToken("test-bearer-token");

            // Act
            var result = ConfigurationHelper.ValidateAuthentication(config);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateAuthentication_NoAuthentication_ReturnsFalse()
        {
            // Arrange
            var config = new Configuration();

            // Act
            var result = ConfigurationHelper.ValidateAuthentication(config);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateAuthentication_NullConfiguration_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                ConfigurationHelper.ValidateAuthentication(null!));
            Assert.Equal("configuration", exception.ParamName);
        }

        [Fact]
        public void ValidateAuthentication_EmptyApiKey_ReturnsFalse()
        {
            // Arrange
            var config = new Configuration
            {
                ApiKey = new Dictionary<string, string>
                {
                    { "Authorization", "" }
                }
            };

            // Act
            var result = ConfigurationHelper.ValidateAuthentication(config);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateAuthentication_EmptyAccessToken_ReturnsFalse()
        {
            // Arrange
            var config = new Configuration
            {
                AccessToken = ""
            };

            // Act
            var result = ConfigurationHelper.ValidateAuthentication(config);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region Fluent API Chain Tests

        [Fact]
        public void FluentChain_AllMethods_WorksCorrectly()
        {
            // Act
            var config = ConfigurationHelper.CreateCustom("https://api.example.com")
                .WithApiKey("test-key")
                .WithTimeout(60000)
                .WithUserAgent("TestApp/1.0");

            // Assert
            Assert.Equal("https://api.example.com", config.BasePath);
            Assert.Equal("ApiKey test-key", config.ApiKey?["Authorization"]);
            Assert.Equal(TimeSpan.FromMilliseconds(60000), config.Timeout);
            Assert.Equal("TestApp/1.0", config.UserAgent);
        }

        [Fact]
        public void FluentChain_CanSwitchAuthenticationTypes()
        {
            // Act
            var config = ConfigurationHelper.CreateWithApiKey("api-key")
                .WithBearerToken("bearer-token");

            // Assert
            Assert.Equal("ApiKey api-key", config.ApiKey?["Authorization"]);
            Assert.Equal("bearer-token", config.AccessToken);
        }

        #endregion
    }
}
