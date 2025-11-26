using AdGuard.ApiClient.Client;
using AdGuard.ApiClient.Model;
using Spectre.Console;

namespace AdGuard.ConsoleUI.Services;

public class DnsServerMenuService
{
    private readonly ApiClientFactory _apiClientFactory;

    public DnsServerMenuService(ApiClientFactory apiClientFactory)
    {
        _apiClientFactory = apiClientFactory;
    }

    public async Task ShowAsync()
    {
        var running = true;

        while (running)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]DNS Server Management[/]")
                    .PageSize(10)
                    .HighlightStyle(new Style(Color.Green))
                    .AddChoices(new[]
                    {
                        "List DNS Servers",
                        "View Server Details",
                        "Create DNS Server",
                        "Delete DNS Server",
                        "Back"
                    }));

            AnsiConsole.WriteLine();

            try
            {
                switch (choice)
                {
                    case "List DNS Servers":
                        await ListDnsServersAsync();
                        break;
                    case "View Server Details":
                        await ViewServerDetailsAsync();
                        break;
                    case "Create DNS Server":
                        await CreateDnsServerAsync();
                        break;
                    case "Delete DNS Server":
                        await DeleteDnsServerAsync();
                        break;
                    case "Back":
                        running = false;
                        break;
                }
            }
            catch (ApiException ex)
            {
                AnsiConsole.MarkupLine($"[red]API Error ({ex.ErrorCode}): {ex.Message}[/]");
                AnsiConsole.WriteLine();
            }
        }
    }

    private async Task ListDnsServersAsync()
    {
        var servers = await AnsiConsole.Status()
            .StartAsync("Fetching DNS servers...", async ctx =>
            {
                using var api = _apiClientFactory.CreateDnsServersApi();
                return await api.ListDNSServersAsync();
            });

        if (servers.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No DNS servers found.[/]");
            AnsiConsole.WriteLine();
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[green]ID[/]")
            .AddColumn("[green]Name[/]")
            .AddColumn("[green]Default[/]")
            .AddColumn("[green]Devices[/]");

        foreach (var server in servers)
        {
            table.AddRow(
                server.Id,
                Markup.Escape(server.Name),
                server.Default ? "[green]Yes[/]" : "No",
                server.DeviceIds.Count.ToString());
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    private async Task ViewServerDetailsAsync()
    {
        var servers = await GetServersListAsync();
        if (servers.Count == 0) return;

        var serverChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a DNS server:")
                .PageSize(10)
                .AddChoices(servers.Select(s => $"{s.Name} ({s.Id})")));

        var selectedServer = servers.First(s => $"{s.Name} ({s.Id})" == serverChoice);
        DisplayServerDetails(selectedServer);
    }

    private static void DisplayServerDetails(DNSServer server)
    {
        var deviceList = server.DeviceIds.Count > 0
            ? string.Join(", ", server.DeviceIds)
            : "[grey]None[/]";

        var panel = new Panel(new Rows(
            new Markup($"[bold]ID:[/] {server.Id}"),
            new Markup($"[bold]Name:[/] {Markup.Escape(server.Name)}"),
            new Markup($"[bold]Default:[/] {(server.Default ? "[green]Yes[/]" : "No")}"),
            new Markup($"[bold]Device IDs:[/] {deviceList}"),
            new Markup($"[bold]Settings:[/]"),
            new Markup($"  {server.Settings}")))
        {
            Header = new PanelHeader($"[green]DNS Server: {Markup.Escape(server.Name)}[/]"),
            Border = BoxBorder.Rounded
        };

        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();
    }

    private async Task CreateDnsServerAsync()
    {
        var name = AnsiConsole.Ask<string>("DNS Server [green]name[/]:");

        var dnsServerCreate = new DNSServerCreate(name: name);

        var newServer = await AnsiConsole.Status()
            .StartAsync("Creating DNS server...", async ctx =>
            {
                using var api = _apiClientFactory.CreateDnsServersApi();
                return await api.CreateDNSServerAsync(dnsServerCreate);
            });

        AnsiConsole.MarkupLine($"[green]DNS Server '{Markup.Escape(newServer.Name)}' created successfully![/]");
        AnsiConsole.MarkupLine($"[grey]ID: {newServer.Id}[/]");
        AnsiConsole.WriteLine();
    }

    private async Task DeleteDnsServerAsync()
    {
        var servers = await GetServersListAsync();
        if (servers.Count == 0) return;

        // Filter out default server - can't delete it
        var deletableServers = servers.Where(s => !s.Default).ToList();
        if (deletableServers.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No deletable DNS servers found. Default server cannot be deleted.[/]");
            AnsiConsole.WriteLine();
            return;
        }

        var serverChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a DNS server to [red]delete[/]:")
                .PageSize(10)
                .AddChoices(deletableServers.Select(s => $"{s.Name} ({s.Id})")));

        var selectedServer = deletableServers.First(s => $"{s.Name} ({s.Id})" == serverChoice);

        if (selectedServer.DeviceIds.Count > 0)
        {
            AnsiConsole.MarkupLine($"[yellow]Warning: This server has {selectedServer.DeviceIds.Count} device(s) attached.[/]");
        }

        if (!AnsiConsole.Confirm($"Are you sure you want to delete [red]{Markup.Escape(selectedServer.Name)}[/]?", false))
        {
            AnsiConsole.MarkupLine("[grey]Deletion cancelled.[/]");
            AnsiConsole.WriteLine();
            return;
        }

        await AnsiConsole.Status()
            .StartAsync("Deleting DNS server...", async ctx =>
            {
                using var api = _apiClientFactory.CreateDnsServersApi();
                await api.RemoveDNSServerAsync(selectedServer.Id);
            });

        AnsiConsole.MarkupLine($"[green]DNS Server '{Markup.Escape(selectedServer.Name)}' deleted successfully![/]");
        AnsiConsole.WriteLine();
    }

    private async Task<List<DNSServer>> GetServersListAsync()
    {
        var servers = await AnsiConsole.Status()
            .StartAsync("Fetching DNS servers...", async ctx =>
            {
                using var api = _apiClientFactory.CreateDnsServersApi();
                return await api.ListDNSServersAsync();
            });

        if (servers.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No DNS servers found.[/]");
            AnsiConsole.WriteLine();
        }

        return servers;
    }
}
