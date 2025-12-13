using Microsoft.Extensions.DependencyInjection;
using RulesCompiler.Abstractions;
using RulesCompiler.Configuration;
using RulesCompiler.Helpers;
using RulesCompiler.Services;

namespace RulesCompiler.Extensions;

/// <summary>
/// Extension methods for registering RulesCompiler services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds RulesCompiler services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddRulesCompiler(this IServiceCollection services)
    {
        // Register helpers
        services.AddSingleton<CommandHelper>();

        // Register services
        services.AddSingleton<IConfigurationReader, ConfigurationReader>();
        services.AddSingleton<IFilterCompiler, FilterCompiler>();
        services.AddSingleton<IOutputWriter, OutputWriter>();
        services.AddSingleton<IRulesCompilerService, RulesCompilerService>();

        return services;
    }
}
