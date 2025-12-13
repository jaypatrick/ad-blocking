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
        Assert.Empty(config.Sources);
        Assert.Empty(config.Transformations);
    }

    [Fact]
    public void FilterSource_HasDefaultValues()
    {
        // Act
        var source = new FilterSource();

        // Assert
        Assert.NotNull(source.Name);
        Assert.NotNull(source.Source);
        Assert.Equal("adblock", source.Type);
        Assert.NotNull(source.Inclusions);
        Assert.NotNull(source.Exclusions);
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
}
