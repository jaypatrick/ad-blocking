namespace AdGuard.DataAccess.Extensions;

/// <summary>
/// Supported database providers.
/// </summary>
public enum DatabaseProvider
{
    /// <summary>
    /// SQLite database provider.
    /// </summary>
    Sqlite,

    /// <summary>
    /// SQL Server database provider.
    /// </summary>
    SqlServer,

    /// <summary>
    /// PostgreSQL database provider.
    /// </summary>
    PostgreSql,

    /// <summary>
    /// In-memory database provider (for testing).
    /// </summary>
    InMemory
}