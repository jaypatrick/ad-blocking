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
    /// Class for testing DNSServerSettingsUpdate
    /// </summary>
    public class DNSServerSettingsUpdateTests : IDisposable
    {
        private DNSServerSettingsUpdate _instance;

        public DNSServerSettingsUpdateTests()
        {
            var userRulesUpdate = new UserRulesSettingsUpdate(
                enabled: true,
                rules: new List<string> { "||example.com^" });

            _instance = new DNSServerSettingsUpdate(userRulesSettings: userRulesUpdate);
        }

        public void Dispose()
        {
            // Cleanup when everything is done.
        }

        /// <summary>
        /// Test an instance of DNSServerSettingsUpdate
        /// </summary>
        [Fact]
        public void DNSServerSettingsUpdateInstanceTest()
        {
            Assert.IsType<DNSServerSettingsUpdate>(_instance);
        }

        /// <summary>
        /// Test the property 'UserRulesSettings'
        /// </summary>
        [Fact]
        public void UserRulesSettingsTest()
        {
            // Arrange
            var userRulesUpdate = new UserRulesSettingsUpdate(
                enabled: true,
                rules: new List<string> { "||ads.com^" });
            var settingsUpdate = new DNSServerSettingsUpdate(userRulesSettings: userRulesUpdate);

            // Assert
            Assert.NotNull(settingsUpdate.UserRulesSettings);
            Assert.True(settingsUpdate.UserRulesSettings.Enabled);
            Assert.Single(settingsUpdate.UserRulesSettings.Rules);
        }

        [Fact]
        public void UserRulesSettings_WhenNull_ReturnsNull()
        {
            // Arrange
            var settingsUpdate = new DNSServerSettingsUpdate(userRulesSettings: null);

            // Assert
            Assert.Null(settingsUpdate.UserRulesSettings);
        }

        /// <summary>
        /// Test default constructor behavior
        /// </summary>
        [Fact]
        public void DefaultConstructor_CreatesInstanceWithNullSettings()
        {
            // Arrange & Act
            var settingsUpdate = new DNSServerSettingsUpdate();

            // Assert
            Assert.NotNull(settingsUpdate);
            Assert.Null(settingsUpdate.UserRulesSettings);
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
            Assert.Contains("DNSServerSettingsUpdate", result);
            Assert.Contains("UserRulesSettings", result);
        }

        /// <summary>
        /// Test ToJson method
        /// </summary>
        [Fact]
        public void ToJsonTest()
        {
            // Arrange
            var userRulesUpdate = new UserRulesSettingsUpdate(
                enabled: true,
                rules: new List<string> { "||test.com^" });
            var settingsUpdate = new DNSServerSettingsUpdate(userRulesSettings: userRulesUpdate);

            // Act
            var json = settingsUpdate.ToJson();

            // Assert
            Assert.Contains("\"user_rules_settings\":", json);
            Assert.Contains("\"enabled\": true", json);
        }

        /// <summary>
        /// Test JSON serialization with null user rules settings
        /// </summary>
        [Fact]
        public void ToJson_WithNullUserRulesSettings_DoesNotIncludeIt()
        {
            // Arrange
            var settingsUpdate = new DNSServerSettingsUpdate(userRulesSettings: null);

            // Act
            var json = settingsUpdate.ToJson();

            // Assert - null properties should not be included due to EmitDefaultValue = false
            Assert.DoesNotContain("\"user_rules_settings\":", json);
        }

        /// <summary>
        /// Test JSON serialization roundtrip
        /// </summary>
        [Fact]
        public void JsonSerializationRoundtripTest()
        {
            // Arrange
            var userRulesUpdate = new UserRulesSettingsUpdate(
                enabled: true,
                rules: new List<string> { "||blocked.com^", "@@||allowed.com^" });
            var original = new DNSServerSettingsUpdate(userRulesSettings: userRulesUpdate);

            // Act
            var json = original.ToJson();
            var deserialized = JsonConvert.DeserializeObject<DNSServerSettingsUpdate>(json);

            // Assert
            Assert.NotNull(deserialized);
            Assert.NotNull(deserialized.UserRulesSettings);
            Assert.Equal(original.UserRulesSettings.Enabled, deserialized.UserRulesSettings.Enabled);
            Assert.Equal(original.UserRulesSettings.Rules.Count, deserialized.UserRulesSettings.Rules.Count);
        }

        /// <summary>
        /// Test creating update with only enabled flag
        /// </summary>
        [Fact]
        public void CreateWithEnabledOnly_SerializesCorrectly()
        {
            // Arrange
            var userRulesUpdate = new UserRulesSettingsUpdate(enabled: false, rules: null);
            var settingsUpdate = new DNSServerSettingsUpdate(userRulesSettings: userRulesUpdate);

            // Act
            var json = settingsUpdate.ToJson();

            // Assert
            Assert.Contains("\"enabled\": false", json);
            Assert.DoesNotContain("\"rules\":", json);
        }

        /// <summary>
        /// Test creating update with only rules
        /// </summary>
        [Fact]
        public void CreateWithRulesOnly_SerializesCorrectly()
        {
            // Arrange
            var rules = new List<string> { "||ad1.com^", "||ad2.com^" };
            var userRulesUpdate = new UserRulesSettingsUpdate(enabled: null, rules: rules);
            var settingsUpdate = new DNSServerSettingsUpdate(userRulesSettings: userRulesUpdate);

            // Act
            var json = settingsUpdate.ToJson();

            // Assert
            Assert.DoesNotContain("\"enabled\":", json);
            Assert.Contains("\"rules\":", json);
            Assert.Contains("||ad1.com^", json);
            Assert.Contains("||ad2.com^", json);
        }
    }
}
