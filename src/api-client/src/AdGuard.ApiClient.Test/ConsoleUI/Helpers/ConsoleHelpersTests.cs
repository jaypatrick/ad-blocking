using AdGuard.ConsoleUI.Helpers;
using Xunit;

namespace AdGuard.ApiClient.Test.ConsoleUI.Helpers;

/// <summary>
/// Unit tests for <see cref="ConsoleHelpers"/>.
/// </summary>
public class ConsoleHelpersTests
{
    #region CreateProgressBar Tests

    [Theory]
    [InlineData(0, "[green][/][grey]\u2591\u2591\u2591\u2591\u2591\u2591\u2591\u2591\u2591\u2591[/]")]
    [InlineData(50, "[green]\u2588\u2588\u2588\u2588\u2588[/][grey]\u2591\u2591\u2591\u2591\u2591[/]")]
    [InlineData(100, "[green]\u2588\u2588\u2588\u2588\u2588\u2588\u2588\u2588\u2588\u2588[/][grey][/]")]
    public void CreateProgressBar_WithPercentage_ReturnsCorrectBar(double percentage, string expected)
    {
        // Act
        var result = ConsoleHelpers.CreateProgressBar(percentage);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CreateProgressBar_At70Percent_ReturnsYellowBar()
    {
        // Act
        var result = ConsoleHelpers.CreateProgressBar(75);

        // Assert
        Assert.Contains("[yellow]", result);
    }

    [Fact]
    public void CreateProgressBar_At90Percent_ReturnsRedBar()
    {
        // Act
        var result = ConsoleHelpers.CreateProgressBar(95);

        // Assert
        Assert.Contains("[red]", result);
    }

    [Fact]
    public void CreateProgressBar_WithCustomWidth_ReturnsCorrectLength()
    {
        // Act
        var result = ConsoleHelpers.CreateProgressBar(50, 20);

        // Assert - 50% of 20 = 10 filled + 10 empty = 20 total bars
        Assert.Contains(new string('\u2588', 10), result);
        Assert.Contains(new string('\u2591', 10), result);
    }

    [Fact]
    public void CreateProgressBar_OverHundred_ClampsToMax()
    {
        // Act
        var result = ConsoleHelpers.CreateProgressBar(150);

        // Assert - Should be fully filled (10 filled, 0 empty)
        Assert.Contains(new string('\u2588', 10), result);
    }

    [Fact]
    public void CreateProgressBar_Negative_ClampsToZero()
    {
        // Act
        var result = ConsoleHelpers.CreateProgressBar(-50);

        // Assert - Should be fully empty
        Assert.Contains(new string('\u2591', 10), result);
    }

    #endregion

    #region GetPercentageMarkup Tests

    [Theory]
    [InlineData(50, "[green]50.0%[/]")]
    [InlineData(75, "[yellow]75.0%[/]")]
    [InlineData(95, "[red]95.0%[/]")]
    public void GetPercentageMarkup_WithValue_ReturnsCorrectColor(double percentage, string expected)
    {
        // Act
        var result = ConsoleHelpers.GetPercentageMarkup(percentage);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetPercentageMarkup_AtExactly70_ReturnsYellow()
    {
        // Act
        var result = ConsoleHelpers.GetPercentageMarkup(70);

        // Assert
        Assert.Contains("[yellow]", result);
    }

    [Fact]
    public void GetPercentageMarkup_AtExactly90_ReturnsRed()
    {
        // Act
        var result = ConsoleHelpers.GetPercentageMarkup(90);

        // Assert
        Assert.Contains("[red]", result);
    }

    [Fact]
    public void GetPercentageMarkup_At69Point9_ReturnsGreen()
    {
        // Act
        var result = ConsoleHelpers.GetPercentageMarkup(69.9);

        // Assert
        Assert.Contains("[green]", result);
    }

    #endregion

    #region SelectItem Tests

    [Fact]
    public void SelectItem_WithEmptyList_ReturnsNull()
    {
        // Arrange
        var items = new List<string>();

        // Act
        var result = ConsoleHelpers.SelectItem("Title", items, x => x);

        // Assert
        Assert.Null(result);
    }

    #endregion
}
