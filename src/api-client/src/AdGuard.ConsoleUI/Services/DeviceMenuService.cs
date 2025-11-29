using AdGuard.ApiClient.Client;
using AdGuard.ApiClient.Model;
using Spectre.Console;

namespace AdGuard.ConsoleUI.Services;

public class DeviceMenuService
{
    private readonly ApiClientFactory _apiClientFactory;

    public DeviceMenuService(ApiClientFactory apiClientFactory)
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
                    .Title("[green]Device Management[/]")
                    .PageSize(10)
                    .HighlightStyle(new Style(Color.Green))
                    .AddChoices(new[]
                    {
                        "List Devices",
                        "View Device Details",
                        "Create Device",
                        "Delete Device",
                        "Back"
                    }));

            AnsiConsole.WriteLine();

            try
            {
                switch (choice)
                {
                    case "List Devices":
                        await ListDevicesAsync();
                        break;
                    case "View Device Details":
                        await ViewDeviceDetailsAsync();
                        break;
                    case "Create Device":
                        await CreateDeviceAsync();
                        break;
                    case "Delete Device":
                        await DeleteDeviceAsync();
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

    private async Task ListDevicesAsync()
    {
        var devices = await AnsiConsole.Status()
            .StartAsync("Fetching devices...", async ctx =>
            {
                using var api = _apiClientFactory.CreateDevicesApi();
                return await api.ListDevicesAsync();
            });

        if (devices.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No devices found.[/]");
            AnsiConsole.WriteLine();
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[green]ID[/]")
            .AddColumn("[green]Name[/]")
            .AddColumn("[green]Type[/]")
            .AddColumn("[green]DNS Server ID[/]");

        foreach (var device in devices)
        {
            table.AddRow(
                device.Id,
                Markup.Escape(device.Name),
                device.DeviceType.ToString(),
                device.DnsServerId);
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    private async Task ViewDeviceDetailsAsync()
    {
        var devices = await GetDevicesListAsync();
        if (devices.Count == 0) return;

        var deviceChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a device:")
                .PageSize(10)
                .AddChoices(devices.Select(d => $"{d.Name} ({d.Id})")));

        var selectedId = devices.First(d => $"{d.Name} ({d.Id})" == deviceChoice).Id;

        var device = await AnsiConsole.Status()
            .StartAsync("Fetching device details...", async ctx =>
            {
                using var api = _apiClientFactory.CreateDevicesApi();
                return await api.GetDeviceAsync(selectedId);
            });

        DisplayDeviceDetails(device);
    }

    private static void DisplayDeviceDetails(Device device)
    {
        var panel = new Panel(new Rows(
            new Markup($"[bold]ID:[/] {device.Id}"),
            new Markup($"[bold]Name:[/] {Markup.Escape(device.Name)}"),
            new Markup($"[bold]Type:[/] {device.DeviceType}"),
            new Markup($"[bold]DNS Server ID:[/] {device.DnsServerId}"),
            new Markup($"[bold]DNS Addresses:[/]"),
            new Markup($"  {device.DnsAddresses}"),
            new Markup($"[bold]Settings:[/]"),
            new Markup($"  {device.Settings}")))
        {
            Header = new PanelHeader($"[green]Device: {Markup.Escape(device.Name)}[/]"),
            Border = BoxBorder.Rounded
        };

        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();
    }

    private async Task CreateDeviceAsync()
    {
        // First get DNS servers to let user select one
        var dnsServers = await AnsiConsole.Status()
            .StartAsync("Fetching DNS servers...", async ctx =>
            {
                using var api = _apiClientFactory.CreateDnsServersApi();
                return await api.ListDNSServersAsync();
            });

        if (dnsServers.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No DNS servers available. Please create a DNS server first.[/]");
            AnsiConsole.WriteLine();
            return;
        }

        var name = AnsiConsole.Ask<string>("Device [green]name[/]:");

        var deviceType = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select device [green]type[/]:")
                .PageSize(10)
                .AddChoices(new[]
                {
                    "WINDOWS",
                    "ANDROID",
                    "MAC",
                    "IOS",
                    "LINUX",
                    "ROUTER",
                    "SMART_TV",
                    "GAME_CONSOLE",
                    "UNKNOWN"
                }));

        var dnsServerChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select [green]DNS Server[/]:")
                .PageSize(10)
                .AddChoices(dnsServers.Select(s => $"{s.Name} ({s.Id})")));

        var selectedDnsServerId = dnsServers.First(s => $"{s.Name} ({s.Id})" == dnsServerChoice).Id;

        var deviceCreate = new DeviceCreate(
            deviceType: deviceType,
            dnsServerId: selectedDnsServerId,
            name: name);

        var newDevice = await AnsiConsole.Status()
            .StartAsync("Creating device...", async ctx =>
            {
                using var api = _apiClientFactory.CreateDevicesApi();
                return await api.CreateDeviceAsync(deviceCreate);
            });

        AnsiConsole.MarkupLine($"[green]Device '{Markup.Escape(newDevice.Name)}' created successfully![/]");
        AnsiConsole.MarkupLine($"[grey]ID: {newDevice.Id}[/]");
        AnsiConsole.WriteLine();
    }

    private async Task DeleteDeviceAsync()
    {
        var devices = await GetDevicesListAsync();
        if (devices.Count == 0) return;

        var deviceChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a device to [red]delete[/]:")
                .PageSize(10)
                .AddChoices(devices.Select(d => $"{d.Name} ({d.Id})")));

        var selectedDevice = devices.First(d => $"{d.Name} ({d.Id})" == deviceChoice);

        if (!AnsiConsole.Confirm($"Are you sure you want to delete [red]{Markup.Escape(selectedDevice.Name)}[/]?", false))
        {
            AnsiConsole.MarkupLine("[grey]Deletion cancelled.[/]");
            AnsiConsole.WriteLine();
            return;
        }

        await AnsiConsole.Status()
            .StartAsync("Deleting device...", async ctx =>
            {
                using var api = _apiClientFactory.CreateDevicesApi();
                await api.RemoveDeviceAsync(selectedDevice.Id);
            });

        AnsiConsole.MarkupLine($"[green]Device '{Markup.Escape(selectedDevice.Name)}' deleted successfully![/]");
        AnsiConsole.WriteLine();
    }

    private async Task<List<Device>> GetDevicesListAsync()
    {
        var devices = await AnsiConsole.Status()
            .StartAsync("Fetching devices...", async ctx =>
            {
                using var api = _apiClientFactory.CreateDevicesApi();
                return await api.ListDevicesAsync();
            });

        if (devices.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No devices found.[/]");
            AnsiConsole.WriteLine();
        }

        return devices;
    }
}
