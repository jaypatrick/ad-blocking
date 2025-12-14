/*
 * AdGuard DNS API
 *
 * DNS API documentation
 *
 * The version of the OpenAPI document: 1.11
 */

using Xunit;

using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using AdGuard.ApiClient.Model;
using AdGuard.ApiClient.Client;
using System.Reflection;
using Newtonsoft.Json;

namespace AdGuard.ApiClient.Test.Model
{
    /// <summary>
    /// Class for testing UserRulesSettings
    /// </summary>
    public class UserRulesSettingsTests : IDisposable
    {
        private UserRulesSettings _instance;

        public UserRulesSettingsTests()
        {
            _instance = new UserRulesSettings(
                enabled: true,
                rules: new List<string> { "||example.com^", "@@||allowed.com^" },
                rulesCount: 2);
        }

        public void Dispose()
        {
            // Cleanup when everything is done.
        }

        /// <summary>
        /// Test an instance of UserRulesSettings
        /// </summary>
        [Fact]
        public void UserRulesSettingsInstanceTest()
        {
            Assert.IsType<UserRulesSettings>(_instance);
        }

        /// <summary>
        /// Test the property 'Enabled'
        /// </summary>
        [Fact]
        public void EnabledTest()
        {
            // Arrange
            var settings = new UserRulesSettings(enabled: true, rules: new List<string>(), rulesCount: 0);

            // Assert
            Assert.True(settings.Enabled);
        }

        [Fact]
        public void Enabled_WhenFalse_ReturnsFalse()
        {
            // Arrange
            var settings = new UserRulesSettings(enabled: false, rules: new List<string>(), rulesCount: 0);

            // Assert
            Assert.False(settings.Enabled);
        }

        /// <summary>
        /// Test the property 'Rules'
        /// </summary>
        [Fact]
        public void RulesTest()
        {
            // Arrange
            var rules = new List<string> { "||ads.example.com^", "||tracking.example.com^" };
            var settings = new UserRulesSettings(enabled: true, rules: rules, rulesCount: 2);

            // Assert
            Assert.Equal(2, settings.Rules.Count);
            Assert.Contains("||ads.example.com^", settings.Rules);
            Assert.Contains("||tracking.example.com^", settings.Rules);
        }

        [Fact]
        public void Rules_WithNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new UserRulesSettings(enabled: true, rules: null!, rulesCount: 0));
        }

        /// <summary>
        /// Test the property 'RulesCount'
        /// </summary>
        [Fact]
        public void RulesCountTest()
        {
            // Arrange
            var settings = new UserRulesSettings(
                enabled: true,
                rules: new List<string> { "rule1", "rule2", "rule3" },
                rulesCount: 3);

            // Assert
            Assert.Equal(3, settings.RulesCount);
        }

        [Fact]
        public void RulesCount_WithZero_ReturnsZero()
        {
            // Arrange
            var settings = new UserRulesSettings(enabled: true, rules: new List<string>(), rulesCount: 0);

            // Assert
            Assert.Equal(0, settings.RulesCount);
        }

        /// <summary>
        /// Test ToString method
        /// </summary>
        [Fact]
        public void ToStringTest()
        {
            // Act
            var result = _instance.ToString();

            // Assert
            Assert.Contains("UserRulesSettings", result);
            Assert.Contains("Enabled", result);
            Assert.Contains("Rules", result);
            Assert.Contains("RulesCount", result);
        }

        /// <summary>
        /// Test ToJson method
        /// </summary>
        [Fact]
        public void ToJsonTest()
        {
            // Arrange
            var settings = new UserRulesSettings(
                enabled: true,
                rules: new List<string> { "||test.com^" },
                rulesCount: 1);

            // Act
            var json = settings.ToJson();

            // Assert
            Assert.Contains("\"enabled\": true", json);
            Assert.Contains("\"rules\":", json);
            Assert.Contains("\"rules_count\": 1", json);
        }

        /// <summary>
        /// Test JSON serialization roundtrip
        /// </summary>
        [Fact]
        public void JsonSerializationRoundtripTest()
        {
            // Arrange
            var original = new UserRulesSettings(
                enabled: true,
                rules: new List<string> { "||blocked.com^", "@@||allowed.com^" },
                rulesCount: 2);

            // Act
            var json = original.ToJson();
            var deserialized = JsonConvert.DeserializeObject<UserRulesSettings>(json);

            // Assert
            Assert.NotNull(deserialized);
            Assert.Equal(original.Enabled, deserialized.Enabled);
            Assert.Equal(original.Rules.Count, deserialized.Rules.Count);
            Assert.Equal(original.RulesCount, deserialized.RulesCount);
        }
    }
}
