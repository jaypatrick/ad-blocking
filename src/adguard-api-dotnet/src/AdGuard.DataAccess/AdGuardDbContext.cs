using AdGuard.DataAccess.Configurations;
using AdGuard.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdGuard.DataAccess;

/// <summary>
/// Entity Framework Core database context for AdGuard data persistence.
/// </summary>
public class AdGuardDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AdGuardDbContext"/> class.
    /// </summary>
    /// <param name="options">The options to be used by this context.</param>
    public AdGuardDbContext(DbContextOptions<AdGuardDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the query logs DbSet.
    /// </summary>
    public DbSet<QueryLogEntity> QueryLogs => Set<QueryLogEntity>();

    /// <summary>
    /// Gets or sets the statistics DbSet.
    /// </summary>
    public DbSet<StatisticsEntity> Statistics => Set<StatisticsEntity>();

    /// <summary>
    /// Gets or sets the audit logs DbSet.
    /// </summary>
    public DbSet<AuditLogEntity> AuditLogs => Set<AuditLogEntity>();

    /// <summary>
    /// Gets or sets the compilation history DbSet.
    /// </summary>
    public DbSet<CompilationHistoryEntity> CompilationHistory => Set<CompilationHistoryEntity>();

    /// <summary>
    /// Gets or sets the device cache DbSet.
    /// </summary>
    public DbSet<DeviceCacheEntity> DeviceCache => Set<DeviceCacheEntity>();

    /// <summary>
    /// Gets or sets the DNS server cache DbSet.
    /// </summary>
    public DbSet<DnsServerCacheEntity> DnsServerCache => Set<DnsServerCacheEntity>();

    /// <summary>
    /// Gets or sets the filter list cache DbSet.
    /// </summary>
    public DbSet<FilterListCacheEntity> FilterListCache => Set<FilterListCacheEntity>();

    /// <summary>
    /// Gets or sets the user settings DbSet.
    /// </summary>
    public DbSet<UserSettingsEntity> UserSettings => Set<UserSettingsEntity>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations
        modelBuilder.ApplyConfiguration(new QueryLogConfiguration());
        modelBuilder.ApplyConfiguration(new StatisticsConfiguration());
        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
        modelBuilder.ApplyConfiguration(new CompilationHistoryConfiguration());
        modelBuilder.ApplyConfiguration(new DeviceCacheConfiguration());
        modelBuilder.ApplyConfiguration(new DnsServerCacheConfiguration());
        modelBuilder.ApplyConfiguration(new FilterListCacheConfiguration());
        modelBuilder.ApplyConfiguration(new UserSettingsConfiguration());
    }

    /// <inheritdoc />
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        // Configure default string length
        configurationBuilder.Properties<string>()
            .HaveMaxLength(500);

        // Configure DateTime to always use UTC
        configurationBuilder.Properties<DateTime>()
            .HaveConversion<DateTimeUtcConverter>();
    }
}

/// <summary>
/// Converts DateTime values to UTC for storage.
/// </summary>
internal class DateTimeUtcConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimeUtcConverter"/> class.
    /// </summary>
    public DateTimeUtcConverter()
        : base(
            v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
    {
    }
}
