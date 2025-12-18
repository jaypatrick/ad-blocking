using AdGuard.DataAccess.Entities;

namespace AdGuard.DataAccess.Abstractions;

/// <summary>
/// Repository interface for user settings local persistence.
/// </summary>
public interface IUserSettingsLocalRepository : ILocalRepository<UserSettingsEntity>
{
    /// <summary>
    /// Gets a setting by its key.
    /// </summary>
    /// <param name="key">The setting key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The setting if found, null otherwise.</returns>
    Task<UserSettingsEntity?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all settings in a category.
    /// </summary>
    /// <param name="category">The category name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Settings in the specified category.</returns>
    Task<List<UserSettingsEntity>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a setting value as a string.
    /// </summary>
    /// <param name="key">The setting key.</param>
    /// <param name="defaultValue">The default value if not found.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The setting value or default.</returns>
    Task<string?> GetValueAsync(string key, string? defaultValue = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a setting value as an integer.
    /// </summary>
    /// <param name="key">The setting key.</param>
    /// <param name="defaultValue">The default value if not found.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The setting value or default.</returns>
    Task<int> GetIntValueAsync(string key, int defaultValue = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a setting value as a boolean.
    /// </summary>
    /// <param name="key">The setting key.</param>
    /// <param name="defaultValue">The default value if not found.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The setting value or default.</returns>
    Task<bool> GetBoolValueAsync(string key, bool defaultValue = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a setting value as a typed object (deserialized from JSON).
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="key">The setting key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized value or default.</returns>
    Task<T?> GetTypedValueAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Sets a setting value.
    /// </summary>
    /// <param name="key">The setting key.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="category">Optional category.</param>
    /// <param name="description">Optional description.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated or created setting.</returns>
    Task<UserSettingsEntity> SetValueAsync(
        string key,
        string? value,
        string? category = null,
        string? description = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets a typed setting value (serialized to JSON).
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="key">The setting key.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="category">Optional category.</param>
    /// <param name="description">Optional description.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated or created setting.</returns>
    Task<UserSettingsEntity> SetTypedValueAsync<T>(
        string key,
        T value,
        string? category = null,
        string? description = null,
        CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Deletes a setting by its key.
    /// </summary>
    /// <param name="key">The setting key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the setting was deleted, false otherwise.</returns>
    Task<bool> DeleteByKeyAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a setting exists.
    /// </summary>
    /// <param name="key">The setting key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the setting exists, false otherwise.</returns>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
}
