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
using System.ComponentModel.DataAnnotations;
using AdGuard.ApiClient.Model;
using AdGuard.ApiClient.Client;
using System.Reflection;
using Newtonsoft.Json;

namespace AdGuard.ApiClient.Test.Model
{
    /// <summary>
    /// Class for testing UserRulesSettingsUpdate
    /// </summary>
    public class UserRulesSettingsUpdateTests : IDisposable
    {
        private UserRulesSettingsUpdate _instance;

        public UserRulesSettingsUpdateTests()
        {
            _instance = new UserRulesSettingsUpdate(
                enabled: true,
                rules: new List<string> { "||example.com^" });
        }

        public void Dispose()
        {
            // Cleanup when everything is done.
        }

        /// <summary>
        /// Test an instance of UserRulesSettingsUpdate
        /// </summary>
        [Fact]
        public void UserRulesSettingsUpdateInstanceTest()
        {
            Assert.IsType<UserRulesSettingsUpdate>(_instance);
        }

        /// <summary>
        /// Test the property 'Enabled'
        /// </summary>
        [Fact]
        public void EnabledTest()
        {
            // Arrange
            var update = new UserRulesSettingsUpdate(enabled: true, rules: null);

            // Assert
            Assert.True(update.Enabled);
        }

        [Fact]
        public void Enabled_WhenNull_ReturnsNull()
        {
            // Arrange
            var update = new UserRulesSettingsUpdate(enabled: null, rules: null);

            // Assert
            Assert.Null(update.Enabled);
        }

        [Fact]
        public void Enabled_WhenFalse_ReturnsFalse()
        {
            // Arrange
            var update = new UserRulesSettingsUpdate(enabled: false, rules: null);

            // Assert
            Assert.False(update.Enabled);
        }

        /// <summary>
        /// Test the property 'Rules'
        /// </summary>
        [Fact]
        public void RulesTest()
        {
            // Arrange
            var rules = new List<string> { "||ads.example.com^", "||tracking.example.com^" };
            var update = new UserRulesSettingsUpdate(enabled: null, rules: rules);

            // Assert
            Assert.Equal(2, update.Rules.Count);
            Assert.Contains("||ads.example.com^", update.Rules);
        }

        [Fact]
        public void Rules_WhenNull_ReturnsNull()
        {
            // Arrange
            var update = new UserRulesSettingsUpdate(enabled: true, rules: null);

            // Assert
            Assert.Null(update.Rules);
        }

        [Fact]
        public void Rules_WithEmptyList_ReturnsEmptyList()
        {
            // Arrange
            var update = new UserRulesSettingsUpdate(enabled: true, rules: new List<string>());

            // Assert
            Assert.NotNull(update.Rules);
            Assert.Empty(update.Rules);
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
            Assert.Contains("UserRulesSettingsUpdate", result);
            Assert.Contains("Enabled", result);
            Assert.Contains("Rules", result);
        }

        /// <summary>
        /// Test ToJson method
        /// </summary>
        [Fact]
        public void ToJsonTest()
        {
            // Arrange
            var update = new UserRulesSettingsUpdate(
                enabled: true,
                rules: new List<string> { "||test.com^" });

            // Act
            var json = update.ToJson();

            // Assert
            Assert.Contains("\"enabled\": true", json);
            Assert.Contains("\"rules\":", json);
        }

        /// <summary>
        /// Test JSON serialization with null properties
        /// </summary>
        [Fact]
        public void ToJson_WithNullProperties_DoesNotIncludeNulls()
        {
            // Arrange
            var update = new UserRulesSettingsUpdate(enabled: null, rules: null);

            // Act
            var json = update.ToJson();

            // Assert - null properties should not be included due to EmitDefaultValue = false
            Assert.DoesNotContain("\"enabled\":", json);
            Assert.DoesNotContain("\"rules\":", json);
        }

        /// <summary>
        /// Test validation - rule length
        /// </summary>
        [Fact]
        public void Validate_WithRuleTooLong_ReturnsValidationError()
        {
            // Arrange
            var longRule = new string('x', 1025); // 1025 chars, exceeds 1024 max
            var update = new UserRulesSettingsUpdate(
                enabled: true,
                rules: new List<string> { longRule });

            var validationContext = new ValidationContext(update);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(update, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, r => r.MemberNames.Contains("Rules"));
        }

        [Fact]
        public void Validate_WithRuleAtMaxLength_IsValid()
        {
            // Arrange
            var maxLengthRule = new string('x', 1024); // Exactly 1024 chars
            var update = new UserRulesSettingsUpdate(
                enabled: true,
                rules: new List<string> { maxLengthRule });

            var validationContext = new ValidationContext(update);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(update, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void Validate_WithNullRules_IsValid()
        {
            // Arrange
            var update = new UserRulesSettingsUpdate(enabled: true, rules: null);
            var validationContext = new ValidationContext(update);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(update, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
        }

        /// <summary>
        /// Test JSON serialization roundtrip
        /// </summary>
        [Fact]
        public void JsonSerializationRoundtripTest()
        {
            // Arrange
            var original = new UserRulesSettingsUpdate(
                enabled: true,
                rules: new List<string> { "||blocked.com^" });

            // Act
            var json = original.ToJson();
            var deserialized = JsonConvert.DeserializeObject<UserRulesSettingsUpdate>(json);

            // Assert
            Assert.NotNull(deserialized);
            Assert.Equal(original.Enabled, deserialized.Enabled);
            Assert.Equal(original.Rules?.Count, deserialized.Rules?.Count);
        }
    }
}
