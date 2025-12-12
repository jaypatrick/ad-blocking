using System;
using System.Collections.Generic;
using AdGuard.ConsoleUI.Exceptions;
using Xunit;

namespace AdGuard.ApiClient.Test.ConsoleUI.Exceptions;

/// <summary>
/// Unit tests for custom exceptions.
/// </summary>
public class ExceptionTests
{
    #region AdGuardConsoleException Tests

    [Fact]
    public void AdGuardConsoleException_DefaultConstructor_CreatesInstance()
    {
        // Act
        var exception = new AdGuardConsoleException();

        // Assert
        Assert.NotNull(exception);
    }

    [Fact]
    public void AdGuardConsoleException_WithMessage_SetsMessage()
    {
        // Arrange
        var message = "Test error message";

        // Act
        var exception = new AdGuardConsoleException(message);

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void AdGuardConsoleException_WithInnerException_SetsInnerException()
    {
        // Arrange
        var innerException = new InvalidOperationException("Inner");
        var message = "Outer message";

        // Act
        var exception = new AdGuardConsoleException(message, innerException);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Same(innerException, exception.InnerException);
    }

    #endregion

    #region RepositoryException Tests

    [Fact]
    public void RepositoryException_SetsAllProperties()
    {
        // Arrange
        var repositoryName = "DeviceRepository";
        var operation = "GetById";
        var message = "Device not found";

        // Act
        var exception = new RepositoryException(repositoryName, operation, message);

        // Assert
        Assert.Equal(repositoryName, exception.RepositoryName);
        Assert.Equal(operation, exception.Operation);
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void RepositoryException_WithInnerException_SetsAll()
    {
        // Arrange
        var innerException = new InvalidOperationException();

        // Act
        var exception = new RepositoryException(
            "TestRepo", "TestOp", "Test message", innerException);

        // Assert
        Assert.Same(innerException, exception.InnerException);
    }

    #endregion

    #region EntityNotFoundException Tests

    [Fact]
    public void EntityNotFoundException_SetsEntityTypeAndId()
    {
        // Arrange
        var entityType = "Device";
        var entityId = "device-123";

        // Act
        var exception = new EntityNotFoundException(entityType, entityId);

        // Assert
        Assert.Equal(entityType, exception.EntityType);
        Assert.Equal(entityId, exception.EntityId);
        Assert.Contains(entityType, exception.Message);
        Assert.Contains(entityId, exception.Message);
    }

    [Fact]
    public void EntityNotFoundException_SetsRepositoryName()
    {
        // Arrange
        var entityType = "DNSServer";

        // Act
        var exception = new EntityNotFoundException(entityType, "id");

        // Assert
        Assert.Equal("DNSServerRepository", exception.RepositoryName);
        Assert.Equal("GetById", exception.Operation);
    }

    #endregion

    #region ApiNotConfiguredException Tests

    [Fact]
    public void ApiNotConfiguredException_DefaultMessage_IsSet()
    {
        // Act
        var exception = new ApiNotConfiguredException();

        // Assert
        Assert.Contains("API client is not configured", exception.Message);
    }

    [Fact]
    public void ApiNotConfiguredException_CustomMessage_IsSet()
    {
        // Arrange
        var message = "Custom API error";

        // Act
        var exception = new ApiNotConfiguredException(message);

        // Assert
        Assert.Equal(message, exception.Message);
    }

    #endregion

    #region ValidationException Tests

    [Fact]
    public void ValidationException_SetsParameterName()
    {
        // Arrange
        var paramName = "deviceId";
        var message = "Device ID is required";

        // Act
        var exception = new ValidationException(paramName, message);

        // Assert
        Assert.Equal(paramName, exception.ParameterName);
        Assert.Equal(message, exception.Message);
    }

    #endregion

    #region MenuOperationException Tests

    [Fact]
    public void MenuOperationException_SetsAllProperties()
    {
        // Arrange
        var menuName = "DeviceMenu";
        var operation = "CreateDevice";
        var message = "Failed to create device";
        var innerException = new InvalidOperationException();

        // Act
        var exception = new MenuOperationException(menuName, operation, message, innerException);

        // Assert
        Assert.Equal(menuName, exception.MenuName);
        Assert.Equal(operation, exception.Operation);
        Assert.Equal(message, exception.Message);
        Assert.Same(innerException, exception.InnerException);
    }

    #endregion
}
