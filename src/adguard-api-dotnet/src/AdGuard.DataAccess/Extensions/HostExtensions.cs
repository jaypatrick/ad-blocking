using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AdGuard.DataAccess.Extensions;

/// <summary>
/// Extension methods for <see cref="IHost"/> to initialize the database.
/// </summary>
public static class HostExtensions
{
    /// <summary>
    /// Initializes the AdGuard database based on configured options.
    /// </summary>
    /// <param name="host">The host.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The host for chaining.</returns>
    public static async Task<IHost> InitializeAdGuardDatabaseAsync(
        this IHost host,
        CancellationToken cancellationToken = default)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;

        var options = services.GetService<DataAccessOptions>();
        var context = services.GetRequiredService<AdGuardDbContext>();
        var logger = services.GetRequiredService<ILogger<AdGuardDbContext>>();

        try
        {
            if (options?.AutoMigrate == true)
            {
                logger.LogInformation("Applying database migrations...");
                await context.Database.MigrateAsync(cancellationToken);
                logger.LogInformation("Database migrations applied successfully");
            }
            else if (options?.EnsureCreated == true)
            {
                logger.LogInformation("Ensuring database is created...");
                var created = await context.Database.EnsureCreatedAsync(cancellationToken);
                if (created)
                {
                    logger.LogInformation("Database created successfully");
                }
                else
                {
                    logger.LogDebug("Database already exists");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }

        return host;
    }

    /// <summary>
    /// Cleans up old data based on retention settings.
    /// </summary>
    /// <param name="host">The host.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The host for chaining.</returns>
    public static async Task<IHost> CleanupOldDataAsync(
        this IHost host,
        CancellationToken cancellationToken = default)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;

        var options = services.GetService<DataAccessOptions>();
        if (options == null)
        {
            return host;
        }

        var logger = services.GetRequiredService<ILogger<AdGuardDbContext>>();
        var context = services.GetRequiredService<AdGuardDbContext>();

        try
        {
            var now = DateTime.UtcNow;

            // Cleanup query logs
            if (options.QueryLogRetentionDays > 0)
            {
                var queryLogCutoff = now.AddDays(-options.QueryLogRetentionDays);
                var deletedQueryLogs = await context.QueryLogs
                    .Where(q => q.Timestamp < queryLogCutoff)
                    .ExecuteDeleteAsync(cancellationToken);

                if (deletedQueryLogs > 0)
                {
                    logger.LogInformation("Deleted {Count} query logs older than {Days} days", deletedQueryLogs, options.QueryLogRetentionDays);
                }
            }

            // Cleanup audit logs
            if (options.AuditLogRetentionDays > 0)
            {
                var auditLogCutoff = now.AddDays(-options.AuditLogRetentionDays);
                var deletedAuditLogs = await context.AuditLogs
                    .Where(a => a.Timestamp < auditLogCutoff)
                    .ExecuteDeleteAsync(cancellationToken);

                if (deletedAuditLogs > 0)
                {
                    logger.LogInformation("Deleted {Count} audit logs older than {Days} days", deletedAuditLogs, options.AuditLogRetentionDays);
                }
            }

            // Cleanup statistics
            if (options.StatisticsRetentionDays > 0)
            {
                var statisticsCutoff = now.AddDays(-options.StatisticsRetentionDays);
                var deletedStatistics = await context.Statistics
                    .Where(s => s.Date < statisticsCutoff)
                    .ExecuteDeleteAsync(cancellationToken);

                if (deletedStatistics > 0)
                {
                    logger.LogInformation("Deleted {Count} statistics older than {Days} days", deletedStatistics, options.StatisticsRetentionDays);
                }
            }

            // Cleanup compilation history
            if (options.CompilationHistoryRetentionDays > 0)
            {
                var compilationCutoff = now.AddDays(-options.CompilationHistoryRetentionDays);
                var deletedCompilations = await context.CompilationHistory
                    .Where(c => c.StartedAt < compilationCutoff)
                    .ExecuteDeleteAsync(cancellationToken);

                if (deletedCompilations > 0)
                {
                    logger.LogInformation("Deleted {Count} compilation history records older than {Days} days", deletedCompilations, options.CompilationHistoryRetentionDays);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while cleaning up old data");
        }

        return host;
    }
}
