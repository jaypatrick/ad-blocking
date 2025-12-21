namespace AdGuard.ApiClient.Test.ConsoleUI.Repositories;

/// <summary>
/// Base class for repository unit tests providing common test infrastructure.
/// </summary>
/// <typeparam name="TRepository">The repository type being tested.</typeparam>
public abstract class RepositoryTestBase<TRepository> where TRepository : class
{
    /// <summary>
    /// Gets the mocked API client factory.
    /// </summary>
    protected Mock<IApiClientFactory> ApiClientFactoryMock { get; }

    /// <summary>
    /// Gets the mocked logger.
    /// </summary>
    protected Mock<ILogger<TRepository>> LoggerMock { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryTestBase{TRepository}"/> class.
    /// </summary>
    protected RepositoryTestBase()
    {
        ApiClientFactoryMock = new Mock<IApiClientFactory>();
        LoggerMock = new Mock<ILogger<TRepository>>();
    }

    /// <summary>
    /// Creates an instance of the repository under test.
    /// </summary>
    /// <returns>The repository instance.</returns>
    protected abstract TRepository CreateRepository();

    /// <summary>
    /// Creates an instance of the repository with a null API client factory.
    /// </summary>
    /// <returns>The repository instance.</returns>
    protected abstract TRepository CreateRepositoryWithNullFactory();

    /// <summary>
    /// Creates an instance of the repository with a null logger.
    /// </summary>
    /// <returns>The repository instance.</returns>
    protected abstract TRepository CreateRepositoryWithNullLogger();

    #region Common Constructor Tests

    /// <summary>
    /// Verifies that the constructor throws ArgumentNullException when the API client factory is null.
    /// </summary>
    [Fact]
    public void Constructor_WithNullFactory_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => CreateRepositoryWithNullFactory());

        Assert.Equal("apiClientFactory", exception.ParamName);
    }

    /// <summary>
    /// Verifies that the constructor throws ArgumentNullException when the logger is null.
    /// </summary>
    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => CreateRepositoryWithNullLogger());

        Assert.Equal("logger", exception.ParamName);
    }

    /// <summary>
    /// Verifies that the constructor creates a valid instance when given valid parameters.
    /// </summary>
    [Fact]
    public void Constructor_WithValidFactory_CreatesInstance()
    {
        // Act
        var repository = CreateRepository();

        // Assert
        Assert.NotNull(repository);
    }

    #endregion
}
