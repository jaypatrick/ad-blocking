using RulesCompiler.Configuration;
using RulesCompiler.Models;
using Xunit;

namespace RulesCompiler.Tests;

public class ModelsTests
{
    [Fact]
    public void CompilerConfiguration_HasDefaultValues()
    {
        // Act
        var config = new CompilerConfiguration();

        // Assert
        Assert.NotNull(config.Name);
        Assert.NotNull(config.Sources);
        Assert.NotNull(config.Transformations);
        Assert.NotNull(config.Inclusions);
        Assert.NotNull(config.Exclusions);
        Assert.NotNull(config.InclusionsSources);
        Assert.NotNull(config.ExclusionsSources);
        Assert.Empty(config.Sources);
        Assert.Empty(config.Transformations);
        Assert.Null(config.Description);
        Assert.Null(config.Version);
        Assert.Null(config.License);
        Assert.Null(config.Homepage);
    }

    [Fact]
    public void FilterSource_HasDefaultValues()
    {
        // Act
        var source = new FilterSource();

        // Assert
        Assert.Null(source.Name); // Name is now optional
        Assert.NotNull(source.Source);
        Assert.Equal("adblock", source.Type);
        Assert.NotNull(source.Inclusions);
        Assert.NotNull(source.Exclusions);
        Assert.NotNull(source.InclusionsSources);
        Assert.NotNull(source.ExclusionsSources);
        Assert.NotNull(source.Transformations);
        Assert.Empty(source.Transformations);
    }

    [Fact]
    public void CompilerResult_HasDefaultValues()
    {
        // Act
        var result = new CompilerResult();

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.ConfigName);
        Assert.NotNull(result.OutputPath);
        Assert.NotNull(result.StandardOutput);
        Assert.NotNull(result.StandardError);
        Assert.Equal(0, result.RuleCount);
        Assert.Null(result.ConfigVersion);
    }

    [Fact]
    public void VersionInfo_HasDefaultValues()
    {
        // Act
        var info = new VersionInfo();

        // Assert
        Assert.NotNull(info.ModuleVersion);
        Assert.NotNull(info.DotNetVersion);
        Assert.NotNull(info.Platform);
    }

    [Fact]
    public void PlatformInfo_HasDefaultValues()
    {
        // Act
        var info = new PlatformInfo();

        // Assert
        Assert.NotNull(info.OSName);
        Assert.NotNull(info.OSVersion);
        Assert.NotNull(info.Architecture);
        Assert.False(info.IsWindows);
        Assert.False(info.IsLinux);
        Assert.False(info.IsMacOS);
    }

    [Theory]
    [InlineData(ConfigurationFormat.Json)]
    [InlineData(ConfigurationFormat.Yaml)]
    [InlineData(ConfigurationFormat.Toml)]
    public void ConfigurationFormat_HasExpectedValues(ConfigurationFormat format)
    {
        // Just verify the enum values exist and can be used
        Assert.True(Enum.IsDefined(typeof(ConfigurationFormat), format));
    }

    [Fact]
    public void CompilerOptions_HasDefaultValues()
    {
        // Act
        var options = CompilerOptions.Default;

        // Assert
        Assert.True(options.ValidateConfig);
        Assert.False(options.Verbose);
        Assert.False(options.CopyToRules);
        Assert.False(options.FailOnWarnings);
        Assert.Null(options.ConfigPath);
        Assert.Null(options.OutputPath);
    }
}

public class TransformationTests
{
    [Theory]
    [InlineData("RemoveComments")]
    [InlineData("Compress")]
    [InlineData("RemoveModifiers")]
    [InlineData("Validate")]
    [InlineData("ValidateAllowIp")]
    [InlineData("Deduplicate")]
    [InlineData("InvertAllow")]
    [InlineData("RemoveEmptyLines")]
    [InlineData("TrimLines")]
    [InlineData("InsertFinalNewLine")]
    [InlineData("ConvertToAscii")]
    public void TransformationHelper_IsValid_ReturnsTrue_ForValidTransformations(string transformation)
    {
        Assert.True(TransformationHelper.IsValid(transformation));
    }

    [Theory]
    [InlineData("removecomments")] // case-insensitive
    [InlineData("DEDUPLICATE")]
    [InlineData("ValidateAllowIp")]
    public void TransformationHelper_IsValid_CaseInsensitive(string transformation)
    {
        Assert.True(TransformationHelper.IsValid(transformation));
    }

    [Theory]
    [InlineData("Invalid")]
    [InlineData("NotATransformation")]
    [InlineData("")]
    public void TransformationHelper_IsValid_ReturnsFalse_ForInvalidTransformations(string transformation)
    {
        Assert.False(TransformationHelper.IsValid(transformation));
    }

    [Fact]
    public void TransformationHelper_AllTransformations_ContainsAllValues()
    {
        Assert.Equal(11, TransformationHelper.AllTransformations.Count);
    }

    [Fact]
    public void TransformationHelper_RecommendedTransformations_ContainsCommonDefaults()
    {
        Assert.Contains("Validate", TransformationHelper.RecommendedTransformations);
        Assert.Contains("Deduplicate", TransformationHelper.RecommendedTransformations);
        Assert.Contains("InsertFinalNewLine", TransformationHelper.RecommendedTransformations);
    }

    [Fact]
    public void TransformationHelper_GetInvalidTransformations_ReturnsInvalid()
    {
        // Arrange
        var transformations = new[] { "Validate", "Invalid", "Deduplicate", "BadOne" };

        // Act
        var invalid = TransformationHelper.GetInvalidTransformations(transformations);

        // Assert
        Assert.Equal(2, invalid.Count);
        Assert.Contains("Invalid", invalid);
        Assert.Contains("BadOne", invalid);
    }

    [Fact]
    public void TransformationHelper_TryParse_ReturnsTrue_ForValid()
    {
        Assert.True(TransformationHelper.TryParse("Deduplicate", out var result));
        Assert.Equal(Transformation.Deduplicate, result);
    }
}

public class SourceTypeTests
{
    [Theory]
    [InlineData("adblock")]
    [InlineData("hosts")]
    [InlineData("ADBLOCK")]
    [InlineData("HOSTS")]
    public void SourceTypeHelper_IsValid_ReturnsTrue_ForValidTypes(string sourceType)
    {
        Assert.True(SourceTypeHelper.IsValid(sourceType));
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("other")]
    [InlineData("")]
    public void SourceTypeHelper_IsValid_ReturnsFalse_ForInvalidTypes(string sourceType)
    {
        Assert.False(SourceTypeHelper.IsValid(sourceType));
    }
}

public class ConfigurationValidatorTests
{
    [Fact]
    public void Validate_ReturnsError_WhenNameEmpty()
    {
        // Arrange
        var config = new CompilerConfiguration
        {
            Name = "",
            Sources = [new FilterSource { Source = "test.txt" }]
        };

        // Act
        var result = ConfigurationValidator.Validate(config);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Field == "name");
    }

    [Fact]
    public void Validate_ReturnsError_WhenNoSources()
    {
        // Arrange
        var config = new CompilerConfiguration
        {
            Name = "Test",
            Sources = []
        };

        // Act
        var result = ConfigurationValidator.Validate(config);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Field == "sources");
    }

    [Fact]
    public void Validate_ReturnsError_WhenSourceEmpty()
    {
        // Arrange
        var config = new CompilerConfiguration
        {
            Name = "Test",
            Sources = [new FilterSource { Source = "" }]
        };

        // Act
        var result = ConfigurationValidator.Validate(config);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Field.Contains("source"));
    }

    [Fact]
    public void Validate_ReturnsError_WhenInvalidTransformation()
    {
        // Arrange
        var config = new CompilerConfiguration
        {
            Name = "Test",
            Sources = [new FilterSource { Source = "test.txt" }],
            Transformations = ["Invalid", "Deduplicate"]
        };

        // Act
        var result = ConfigurationValidator.Validate(config);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Field == "transformations");
    }

    [Fact]
    public void Validate_ReturnsError_WhenInvalidSourceType()
    {
        // Arrange
        var config = new CompilerConfiguration
        {
            Name = "Test",
            Sources = [new FilterSource { Source = "test.txt", Type = "invalid" }]
        };

        // Act
        var result = ConfigurationValidator.Validate(config);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Field.Contains("type"));
    }

    [Fact]
    public void Validate_ReturnsValid_ForCorrectConfiguration()
    {
        // Arrange
        var config = new CompilerConfiguration
        {
            Name = "Test",
            Sources = [new FilterSource { Source = "test.txt", Type = "adblock" }],
            Transformations = ["Validate", "Deduplicate"]
        };

        // Act
        var result = ConfigurationValidator.Validate(config);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_ReturnsWarning_ForInvalidRegexPattern()
    {
        // Arrange
        var config = new CompilerConfiguration
        {
            Name = "Test",
            Sources = [new FilterSource { Source = "test.txt" }],
            Exclusions = ["/[invalid regex/"]
        };

        // Act
        var result = ConfigurationValidator.Validate(config);

        // Assert
        Assert.True(result.IsValid); // Warnings don't make it invalid
        Assert.NotEmpty(result.Warnings);
    }
}
