using AdGuard.Repositories.Abstractions;
using AdGuard.Repositories.Exceptions;

namespace AdGuard.Repositories.Common;

/// <summary>
/// Base repository class providing common functionality for all repositories.
/// </summary>
/// <typeparam name="TRepository">The concrete repository type (for logging).</typeparam>
public abstract class BaseRepository<TRepository>
{
    /// <summary>
    /// Gets the API client factory.
    /// </summary>
    protected IApiClientFactory ApiClientFactory { get; }

    /// <summary>
    /// Gets the logger.
    /// </summary>
    protected ILogger<TRepository> Logger { get; }

    /// <summary>
    /// Gets the repository name for error messages.
    /// </summary>
    protected abstract string RepositoryName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseRepository{TRepository}"/> class.
    /// </summary>
    /// <param name="apiClientFactory">The API client factory.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    protected BaseRepository(IApiClientFactory apiClientFactory, ILogger<TRepository> logger)
    {
        ApiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Executes an API operation with standard error handling.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="operation">The operation name.</param>
    /// <param name="action">The action to execute.</param>
    /// <param name="onApiError">Optional callback for logging API errors.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The operation result.</returns>
    /// <exception cref="RepositoryException">Thrown when the operation fails.</exception>
    protected async Task<TResult> ExecuteAsync<TResult>(
        string operation,
        Func<Task<TResult>> action,
        Action<int, string, Exception>? onApiError = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await action().ConfigureAwait(false);
        }
        catch (ApiException ex)
        {
            onApiError?.Invoke(ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException(RepositoryName, operation, $"Failed to execute {operation}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Executes an API operation with standard error handling (no return value).
    /// </summary>
    /// <param name="operation">The operation name.</param>
    /// <param name="action">The action to execute.</param>
    /// <param name="onApiError">Optional callback for logging API errors.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="RepositoryException">Thrown when the operation fails.</exception>
    protected async Task ExecuteAsync(
        string operation,
        Func<Task> action,
        Action<int, string, Exception>? onApiError = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await action().ConfigureAwait(false);
        }
        catch (ApiException ex)
        {
            onApiError?.Invoke(ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException(RepositoryName, operation, $"Failed to execute {operation}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Executes an API operation with 404 handling for entity not found scenarios.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="operation">The operation name.</param>
    /// <param name="entityType">The entity type name.</param>
    /// <param name="entityId">The entity identifier.</param>
    /// <param name="action">The action to execute.</param>
    /// <param name="onNotFound">Optional callback when entity is not found.</param>
    /// <param name="onApiError">Optional callback for logging API errors.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The operation result.</returns>
    /// <exception cref="EntityNotFoundException">Thrown when the entity is not found (404).</exception>
    /// <exception cref="RepositoryException">Thrown when the operation fails for other reasons.</exception>
    protected async Task<TResult> ExecuteWithEntityCheckAsync<TResult>(
        string operation,
        string entityType,
        string entityId,
        Func<Task<TResult>> action,
        Action<string>? onNotFound = null,
        Action<int, string, Exception>? onApiError = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await action().ConfigureAwait(false);
        }
        catch (ApiException ex) when (ex.ErrorCode == 404)
        {
            onNotFound?.Invoke(entityId);
            throw new EntityNotFoundException(entityType, entityId, ex);
        }
        catch (ApiException ex)
        {
            onApiError?.Invoke(ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException(RepositoryName, operation, $"Failed to execute {operation}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Executes an API operation with 404 handling for entity not found scenarios (no return value).
    /// </summary>
    /// <param name="operation">The operation name.</param>
    /// <param name="entityType">The entity type name.</param>
    /// <param name="entityId">The entity identifier.</param>
    /// <param name="action">The action to execute.</param>
    /// <param name="onNotFound">Optional callback when entity is not found.</param>
    /// <param name="onApiError">Optional callback for logging API errors.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="EntityNotFoundException">Thrown when the entity is not found (404).</exception>
    /// <exception cref="RepositoryException">Thrown when the operation fails for other reasons.</exception>
    protected async Task ExecuteWithEntityCheckAsync(
        string operation,
        string entityType,
        string entityId,
        Func<Task> action,
        Action<string>? onNotFound = null,
        Action<int, string, Exception>? onApiError = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await action().ConfigureAwait(false);
        }
        catch (ApiException ex) when (ex.ErrorCode == 404)
        {
            onNotFound?.Invoke(entityId);
            throw new EntityNotFoundException(entityType, entityId, ex);
        }
        catch (ApiException ex)
        {
            onApiError?.Invoke(ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException(RepositoryName, operation, $"Failed to execute {operation}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Validates that an ID parameter is not null or whitespace.
    /// </summary>
    /// <param name="id">The ID to validate.</param>
    /// <param name="parameterName">The parameter name for the exception.</param>
    /// <exception cref="ArgumentException">Thrown when the ID is null or whitespace.</exception>
    protected static void ValidateId(string id, string parameterName = "id")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id, parameterName);
    }
}
