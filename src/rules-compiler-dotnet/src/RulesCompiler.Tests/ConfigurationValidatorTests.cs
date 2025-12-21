namespace RulesCompiler.Tests;

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