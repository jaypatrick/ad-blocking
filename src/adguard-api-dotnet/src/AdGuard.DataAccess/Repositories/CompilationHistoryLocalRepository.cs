using AdGuard.DataAccess.Abstractions;
using AdGuard.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AdGuard.DataAccess.Repositories;

/// <summary>
/// Repository implementation for compilation history local persistence.
/// </summary>
public class CompilationHistoryLocalRepository : LocalRepositoryBase<CompilationHistoryEntity>, ICompilationHistoryLocalRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompilationHistoryLocalRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger.</param>
    public CompilationHistoryLocalRepository(AdGuardDbContext context, ILogger<CompilationHistoryLocalRepository> logger)
        : base(context, logger)
    {
    }

    /// <inheritdoc />
    public async Task<List<CompilationHistoryEntity>> GetByTimeRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(c => c.StartedAt >= start && c.StartedAt <= end)
            .OrderByDescending(c => c.StartedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<CompilationHistoryEntity>> GetByFilterListAsync(string filterListName, int limit = 50, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(c => c.FilterListName == filterListName)
            .OrderByDescending(c => c.StartedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<CompilationHistoryEntity>> GetFailedAsync(int limit = 50, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(c => !c.Success)
            .OrderByDescending(c => c.StartedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<CompilationHistoryEntity>> GetSuccessfulAsync(int limit = 50, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(c => c.Success)
            .OrderByDescending(c => c.StartedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<CompilationHistoryEntity>> GetRecentAsync(int count = 20, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .OrderByDescending(c => c.StartedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CompilationHistoryEntity?> GetLatestByConfigPathAsync(string configurationPath, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(c => c.ConfigurationPath == configurationPath)
            .OrderByDescending(c => c.StartedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CompilationHistoryEntity?> GetByOutputHashAsync(string outputHash, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(c => c.OutputHash == outputHash)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CompilationStatistics> GetStatisticsAsync(int days = 30, CancellationToken cancellationToken = default)
    {
        var startDate = DateTime.UtcNow.AddDays(-days);
        var compilations = await DbSet
            .AsNoTracking()
            .Where(c => c.StartedAt >= startDate)
            .ToListAsync(cancellationToken);

        if (compilations.Count == 0)
        {
            return new CompilationStatistics();
        }

        var successful = compilations.Where(c => c.Success).ToList();

        return new CompilationStatistics
        {
            TotalCompilations = compilations.Count,
            SuccessfulCompilations = successful.Count,
            FailedCompilations = compilations.Count - successful.Count,
            AverageDurationMs = compilations.Average(c => c.DurationMs),
            TotalRulesCompiled = successful.Sum(c => c.RuleCount),
            AverageRulesPerCompilation = successful.Count > 0 ? successful.Average(c => c.RuleCount) : 0
        };
    }

    /// <inheritdoc />
    public async Task<int> DeleteOlderThanAsync(DateTime olderThan, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(c => c.StartedAt < olderThan)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
