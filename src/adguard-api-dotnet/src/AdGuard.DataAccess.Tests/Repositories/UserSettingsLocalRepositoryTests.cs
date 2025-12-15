using AdGuard.DataAccess.Repositories;
using AdGuard.DataAccess.Tests.TestFixtures;
using FluentAssertions;
using Xunit;

namespace AdGuard.DataAccess.Tests.Repositories;

public class UserSettingsLocalRepositoryTests : IDisposable
{
    private readonly DatabaseFixture _fixture;

    public UserSettingsLocalRepositoryTests()
    {
        _fixture = new DatabaseFixture();
    }

    [Fact]
    public async Task SetValueAsync_ShouldCreateSetting()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new UserSettingsLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<UserSettingsLocalRepository>());

        // Act
        var result = await repository.SetValueAsync(
            "theme",
            "dark",
            category: "UI",
            description: "Application theme");

        // Assert
        result.Should().NotBeNull();
        result.Key.Should().Be("theme");
        result.Value.Should().Be("dark");
        result.Category.Should().Be("UI");
    }

    [Fact]
    public async Task SetValueAsync_ShouldUpdateExistingSetting()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new UserSettingsLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<UserSettingsLocalRepository>());

        await repository.SetValueAsync("theme", "light");

        // Act
        var result = await repository.SetValueAsync("theme", "dark");

        // Assert
        result.Value.Should().Be("dark");
        var allSettings = await repository.GetAllAsync();
        allSettings.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetValueAsync_ShouldReturnValue()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new UserSettingsLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<UserSettingsLocalRepository>());

        await repository.SetValueAsync("api_key", "secret123");

        // Act
        var result = await repository.GetValueAsync("api_key");

        // Assert
        result.Should().Be("secret123");
    }

    [Fact]
    public async Task GetValueAsync_WithDefault_ShouldReturnDefaultWhenNotFound()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new UserSettingsLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<UserSettingsLocalRepository>());

        // Act
        var result = await repository.GetValueAsync("nonexistent", "default_value");

        // Assert
        result.Should().Be("default_value");
    }

    [Fact]
    public async Task GetIntValueAsync_ShouldReturnIntValue()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new UserSettingsLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<UserSettingsLocalRepository>());

        await repository.SetValueAsync("max_retries", "5");

        // Act
        var result = await repository.GetIntValueAsync("max_retries");

        // Assert
        result.Should().Be(5);
    }

    [Fact]
    public async Task GetBoolValueAsync_ShouldReturnBoolValue()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new UserSettingsLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<UserSettingsLocalRepository>());

        await repository.SetValueAsync("auto_sync", "true");

        // Act
        var result = await repository.GetBoolValueAsync("auto_sync");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task SetTypedValueAsync_ShouldStoreAsJson()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new UserSettingsLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<UserSettingsLocalRepository>());

        var config = new TestConfig { Name = "Test", Value = 42 };

        // Act
        await repository.SetTypedValueAsync("test_config", config);
        var result = await repository.GetTypedValueAsync<TestConfig>("test_config");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Test");
        result.Value.Should().Be(42);
    }

    [Fact]
    public async Task GetByCategoryAsync_ShouldReturnCategorySettings()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new UserSettingsLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<UserSettingsLocalRepository>());

        await repository.SetValueAsync("theme", "dark", category: "UI");
        await repository.SetValueAsync("font_size", "14", category: "UI");
        await repository.SetValueAsync("api_key", "secret", category: "Auth");

        // Act
        var result = await repository.GetByCategoryAsync("UI");

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(s => s.Category == "UI");
    }

    [Fact]
    public async Task DeleteByKeyAsync_ShouldDeleteSetting()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new UserSettingsLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<UserSettingsLocalRepository>());

        await repository.SetValueAsync("temp_setting", "value");

        // Act
        var deleted = await repository.DeleteByKeyAsync("temp_setting");

        // Assert
        deleted.Should().BeTrue();
        var exists = await repository.ExistsAsync("temp_setting");
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrueWhenExists()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new UserSettingsLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<UserSettingsLocalRepository>());

        await repository.SetValueAsync("my_setting", "value");

        // Act
        var exists = await repository.ExistsAsync("my_setting");
        var notExists = await repository.ExistsAsync("other_setting");

        // Assert
        exists.Should().BeTrue();
        notExists.Should().BeFalse();
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }

    private class TestConfig
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }
}
