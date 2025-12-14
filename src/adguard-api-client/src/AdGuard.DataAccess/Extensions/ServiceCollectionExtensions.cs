using AdGuard.DataAccess.Abstractions;
using AdGuard.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace AdGuard.DataAccess.Extensions;

/// <summary>
/// Extension methods for registering data access services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the AdGuard data access services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAdGuardDataAccess(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = new DataAccessOptions();
        configuration.GetSection(DataAccessOptions.SectionName).Bind(options);

        return services.AddAdGuardDataAccess(options);
    }

    /// <summary>
    /// Adds the AdGuard data access services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">The options configuration action.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAdGuardDataAccess(
        this IServiceCollection services,
        Action<DataAccessOptions> configureOptions)
    {
        var options = new DataAccessOptions();
        configureOptions(options);

        return services.AddAdGuardDataAccess(options);
    }

    /// <summary>
    /// Adds the AdGuard data access services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="options">The data access options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAdGuardDataAccess(
        this IServiceCollection services,
        DataAccessOptions options)
    {
        // Register options
        services.TryAddSingleton(options);

        // Register DbContext
        services.AddDbContext<AdGuardDbContext>((serviceProvider, dbOptions) =>
        {
            ConfigureDbContext(dbOptions, options, serviceProvider);
        });

        // Register repositories
        services.TryAddScoped<IQueryLogLocalRepository, QueryLogLocalRepository>();
        services.TryAddScoped<IStatisticsLocalRepository, StatisticsLocalRepository>();
        services.TryAddScoped<IAuditLogLocalRepository, AuditLogLocalRepository>();
        services.TryAddScoped<ICompilationHistoryLocalRepository, CompilationHistoryLocalRepository>();
        services.TryAddScoped<IUserSettingsLocalRepository, UserSettingsLocalRepository>();

        // Register Unit of Work
        services.TryAddScoped<ILocalUnitOfWork, LocalUnitOfWork>();

        return services;
    }

    /// <summary>
    /// Adds the AdGuard data access services with SQLite to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">The SQLite connection string.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAdGuardDataAccessSqlite(
        this IServiceCollection services,
        string connectionString = "Data Source=adguard.db")
    {
        return services.AddAdGuardDataAccess(options =>
        {
            options.Provider = DatabaseProvider.Sqlite;
            options.ConnectionString = connectionString;
        });
    }

    /// <summary>
    /// Adds the AdGuard data access services with SQL Server to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">The SQL Server connection string.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAdGuardDataAccessSqlServer(
        this IServiceCollection services,
        string connectionString)
    {
        return services.AddAdGuardDataAccess(options =>
        {
            options.Provider = DatabaseProvider.SqlServer;
            options.ConnectionString = connectionString;
        });
    }

    /// <summary>
    /// Adds the AdGuard data access services with PostgreSQL to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">The PostgreSQL connection string.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAdGuardDataAccessPostgreSql(
        this IServiceCollection services,
        string connectionString)
    {
        return services.AddAdGuardDataAccess(options =>
        {
            options.Provider = DatabaseProvider.PostgreSql;
            options.ConnectionString = connectionString;
        });
    }

    /// <summary>
    /// Adds the AdGuard data access services with in-memory database to the service collection.
    /// Useful for testing.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="databaseName">The in-memory database name.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAdGuardDataAccessInMemory(
        this IServiceCollection services,
        string databaseName = "AdGuardTestDb")
    {
        return services.AddAdGuardDataAccess(options =>
        {
            options.Provider = DatabaseProvider.InMemory;
            options.ConnectionString = databaseName;
        });
    }

    private static void ConfigureDbContext(
        DbContextOptionsBuilder dbOptions,
        DataAccessOptions options,
        IServiceProvider serviceProvider)
    {
        var loggerFactory = serviceProvider.GetService<ILoggerFactory>();

        switch (options.Provider)
        {
            case DatabaseProvider.Sqlite:
                dbOptions.UseSqlite(options.ConnectionString, sqliteOptions =>
                {
                    sqliteOptions.CommandTimeout(options.CommandTimeoutSeconds);
                });
                break;

            case DatabaseProvider.SqlServer:
                dbOptions.UseSqlServer(options.ConnectionString, sqlServerOptions =>
                {
                    sqlServerOptions.CommandTimeout(options.CommandTimeoutSeconds);
                    sqlServerOptions.EnableRetryOnFailure(3);
                });
                break;

            case DatabaseProvider.PostgreSql:
                dbOptions.UseNpgsql(options.ConnectionString, npgsqlOptions =>
                {
                    npgsqlOptions.CommandTimeout(options.CommandTimeoutSeconds);
                    npgsqlOptions.EnableRetryOnFailure(3);
                });
                break;

            case DatabaseProvider.InMemory:
                dbOptions.UseInMemoryDatabase(options.ConnectionString);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(options.Provider), options.Provider, "Unsupported database provider");
        }

        if (options.EnableSensitiveDataLogging)
        {
            dbOptions.EnableSensitiveDataLogging();
        }

        if (options.EnableDetailedErrors)
        {
            dbOptions.EnableDetailedErrors();
        }

        if (loggerFactory != null)
        {
            dbOptions.UseLoggerFactory(loggerFactory);
        }
    }
}
