namespace AdGuard.DataAccess.Extensions;

/// <summary>
/// Configuration options for the data access layer.
/// </summary>
public class DataAccessOptions
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "DataAccess";

    /// <summary>
    /// Gets or sets the database provider type.
    /// </summary>
    public DatabaseProvider Provider { get; set; } = DatabaseProvider.Sqlite;

    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    public string ConnectionString { get; set; } = "Data Source=adguard.db";

    /// <summary>
    /// Gets or sets whether to enable sensitive data logging.
    /// </summary>
    public bool EnableSensitiveDataLogging { get; set; }

    /// <summary>
    /// Gets or sets whether to enable detailed errors.
    /// </summary>
    public bool EnableDetailedErrors { get; set; }

    /// <summary>
    /// Gets or sets the command timeout in seconds.
    /// </summary>
    public int CommandTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Gets or sets whether to automatically apply migrations on startup.
    /// </summary>
    public bool AutoMigrate { get; set; }

    /// <summary>
    /// Gets or sets whether to ensure the database is created on startup.
    /// </summary>
    public bool EnsureCreated { get; set; } = true;

    /// <summary>
    /// Gets or sets the query log retention in days.
    /// </summary>
    public int QueryLogRetentionDays { get; set; } = 30;

    /// <summary>
    /// Gets or sets the audit log retention in days.
    /// </summary>
    public int AuditLogRetentionDays { get; set; } = 90;

    /// <summary>
    /// Gets or sets the statistics retention in days.
    /// </summary>
    public int StatisticsRetentionDays { get; set; } = 365;

    /// <summary>
    /// Gets or sets the compilation history retention in days.
    /// </summary>
    public int CompilationHistoryRetentionDays { get; set; } = 180;
}