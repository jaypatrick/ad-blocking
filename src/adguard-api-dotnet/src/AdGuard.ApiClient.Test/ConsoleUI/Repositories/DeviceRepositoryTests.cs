using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdGuard.ApiClient.Api;
using AdGuard.ApiClient.Model;
using AdGuard.ConsoleUI.Abstractions;
using AdGuard.Repositories.Implementations;
using AdGuard.Repositories.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AdGuard.ApiClient.Test.ConsoleUI.Repositories;

/// <summary>
/// Unit tests for <see cref="DeviceRepository"/>.
/// </summary>
public class DeviceRepositoryTests : RepositoryTestBase<DeviceRepository>
{
    /// <inheritdoc />
    protected override DeviceRepository CreateRepository()
    {
        return new DeviceRepository(ApiClientFactoryMock.Object, LoggerMock.Object);
    }

    /// <inheritdoc />
    protected override DeviceRepository CreateRepositoryWithNullFactory()
    {
        return new DeviceRepository(null!, LoggerMock.Object);
    }

    /// <inheritdoc />
    protected override DeviceRepository CreateRepositoryWithNullLogger()
    {
        return new DeviceRepository(ApiClientFactoryMock.Object, null!);
    }

    #region GetAllAsync Tests


    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithNullId_ThrowsArgumentException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => repository.GetByIdAsync(null!));
    }

    [Fact]
    public async Task GetByIdAsync_WithEmptyId_ThrowsArgumentException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => repository.GetByIdAsync(string.Empty));
    }

    [Fact]
    public async Task GetByIdAsync_WithWhitespaceId_ThrowsArgumentException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => repository.GetByIdAsync("   "));
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithNullDevice_ThrowsArgumentNullException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => repository.CreateAsync(null!));
    }

    #endregion

    #region DeleteAsync Tests


    [Fact]
    public async Task DeleteAsync_WithNullId_ThrowsArgumentException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => repository.DeleteAsync(null!));
    }

    [Fact]
    public async Task DeleteAsync_WithEmptyId_ThrowsArgumentException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => repository.DeleteAsync(string.Empty));
    }

    #endregion
}
