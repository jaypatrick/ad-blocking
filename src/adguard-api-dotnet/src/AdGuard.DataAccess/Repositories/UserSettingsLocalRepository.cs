using System.Text.Json;
using AdGuard.DataAccess.Abstractions;
using AdGuard.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AdGuard.DataAccess.Repositories;

/// <summary>
/// Repository implementation for user settings local persistence.
/// </summary>
public class UserSettingsLocalRepository : LocalRepositoryBase<UserSettingsEntity>, IUserSettingsLocalRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSettingsLocalRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger.</param>
    public UserSettingsLocalRepository(AdGuardDbContext context, ILogger<UserSettingsLocalRepository> logger)
        : base(context, logger)
    {
    }

    /// <inheritdoc />
    public async Task<UserSettingsEntity?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Key == key, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<UserSettingsEntity>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(s => s.Category == category)
            .OrderBy(s => s.Key)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<string?> GetValueAsync(string key, string? defaultValue = null, CancellationToken cancellationToken = default)
    {
        var setting = await GetByKeyAsync(key, cancellationToken);
        return setting?.Value ?? defaultValue;
    }

    /// <inheritdoc />
    public async Task<int> GetIntValueAsync(string key, int defaultValue = 0, CancellationToken cancellationToken = default)
    {
        var value = await GetValueAsync(key, cancellationToken: cancellationToken);
        return int.TryParse(value, out var result) ? result : defaultValue;
    }

    /// <inheritdoc />
    public async Task<bool> GetBoolValueAsync(string key, bool defaultValue = false, CancellationToken cancellationToken = default)
    {
        var value = await GetValueAsync(key, cancellationToken: cancellationToken);
        return bool.TryParse(value, out var result) ? result : defaultValue;
    }

    /// <inheritdoc />
    public async Task<T?> GetTypedValueAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        var setting = await GetByKeyAsync(key, cancellationToken);
        if (setting?.Value == null)
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<T>(setting.Value, JsonOptions);
        }
        catch (JsonException ex)
        {
            Logger.LogWarning(ex, "Failed to deserialize setting '{Key}' to type {Type}", key, typeof(T).Name);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<UserSettingsEntity> SetValueAsync(
        string key,
        string? value,
        string? category = null,
        string? description = null,
        CancellationToken cancellationToken = default)
    {
        var existing = await DbSet.FirstOrDefaultAsync(s => s.Key == key, cancellationToken);

        if (existing != null)
        {
            existing.Value = value;
            existing.ModifiedAt = DateTime.UtcNow;

            if (category != null)
            {
                existing.Category = category;
            }

            if (description != null)
            {
                existing.Description = description;
            }

            await Context.SaveChangesAsync(cancellationToken);
            return existing;
        }

        var newSetting = new UserSettingsEntity
        {
            Key = key,
            Value = value,
            Category = category,
            Description = description,
            ValueType = SettingsValueType.String
        };

        return await AddAsync(newSetting, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<UserSettingsEntity> SetTypedValueAsync<T>(
        string key,
        T value,
        string? category = null,
        string? description = null,
        CancellationToken cancellationToken = default) where T : class
    {
        var jsonValue = JsonSerializer.Serialize(value, JsonOptions);

        var existing = await DbSet.FirstOrDefaultAsync(s => s.Key == key, cancellationToken);

        if (existing != null)
        {
            existing.Value = jsonValue;
            existing.ValueType = SettingsValueType.Json;
            existing.ModifiedAt = DateTime.UtcNow;

            if (category != null)
            {
                existing.Category = category;
            }

            if (description != null)
            {
                existing.Description = description;
            }

            await Context.SaveChangesAsync(cancellationToken);
            return existing;
        }

        var newSetting = new UserSettingsEntity
        {
            Key = key,
            Value = jsonValue,
            Category = category,
            Description = description,
            ValueType = SettingsValueType.Json
        };

        return await AddAsync(newSetting, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        var itemsToDelete = await DbSet
            .Where(s => s.Key == key)
            .ToListAsync(cancellationToken);
        
        if (itemsToDelete.Count == 0)
            return false;
        
        DbSet.RemoveRange(itemsToDelete);
        await Context.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(s => s.Key == key, cancellationToken);
    }
}
