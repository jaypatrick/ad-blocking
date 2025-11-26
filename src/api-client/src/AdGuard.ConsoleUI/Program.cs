using AdGuard.ConsoleUI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace AdGuard.ConsoleUI;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var configuration = BuildConfiguration();
            var services = ConfigureServices(configuration);
            var serviceProvider = services.BuildServiceProvider();

            var app = serviceProvider.GetRequiredService<ConsoleApplication>();
            await app.RunAsync();

            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return 1;
        }
    }

    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables("ADGUARD_")
            .Build();
    }

    private static IServiceCollection ConfigureServices(IConfiguration configuration)
    {
        var services = new ServiceCollection();

        services.AddSingleton(configuration);
        services.AddSingleton<ApiClientFactory>();
        services.AddSingleton<ConsoleApplication>();
        services.AddSingleton<DeviceMenuService>();
        services.AddSingleton<DnsServerMenuService>();
        services.AddSingleton<StatisticsMenuService>();
        services.AddSingleton<AccountMenuService>();
        services.AddSingleton<FilterListMenuService>();
        services.AddSingleton<QueryLogMenuService>();

        return services;
    }
}
