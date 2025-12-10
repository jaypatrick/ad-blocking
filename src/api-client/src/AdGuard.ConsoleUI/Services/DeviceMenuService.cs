using AdGuard.ApiClient.Model;
using AdGuard.ConsoleUI.Display;
using AdGuard.ConsoleUI.Helpers;
using AdGuard.ConsoleUI.Repositories;
using Spectre.Console;

namespace AdGuard.ConsoleUI.Services;

/// <summary>
/// Menu service for managing devices.
/// Uses Repository pattern for data access and Strategy pattern for display.
/// </summary>
public class DeviceMenuService : BaseMenuService
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IDnsServerRepository _dnsServerRepository;
    private readonly IDisplayStrategy<Device> _displayStrategy;

    public DeviceMenuService(
        IDeviceRepository deviceRepository,
        IDnsServerRepository dnsServerRepository,
        IDisplayStrategy<Device> displayStrategy)
    {
        _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
        _dnsServerRepository = dnsServerRepository ?? throw new ArgumentNullException(nameof(dnsServerRepository));
        _displayStrategy = displayStrategy ?? throw new ArgumentNullException(nameof(displayStrategy));
    }

    /// <inheritdoc />
    public override string Title => "Device Management";

    /// <inheritdoc />
    protected override Dictionary<string, Func<Task>> GetMenuActions() => new()
    {
        { "List Devices", ListDevicesAsync },
        { "View Device Details", ViewDeviceDetailsAsync },
        { "Create Device", CreateDeviceAsync },
        { "Delete Device", DeleteDeviceAsync }
    };

    private async Task ListDevicesAsync()
    {
        var devices = await ConsoleHelpers.WithStatusAsync(
            "Fetching devices...",
            () => _deviceRepository.GetAllAsync());

        _displayStrategy.Display(devices);
    }

    private async Task ViewDeviceDetailsAsync()
    {
        var devices = await GetDevicesWithStatusAsync();
        if (devices.Count == 0) return;

        var selected = ConsoleHelpers.SelectItem(
            "Select a device:",
            devices,
            d => $"{d.Name} ({d.Id})");

        if (selected == null) return;

        var device = await ConsoleHelpers.WithStatusAsync(
            "Fetching device details...",
            () => _deviceRepository.GetByIdAsync(selected.Id));

        _displayStrategy.DisplayDetails(device);
    }

    private async Task CreateDeviceAsync()
    {
        var dnsServers = await ConsoleHelpers.WithStatusAsync(
            "Fetching DNS servers...",
            () => _dnsServerRepository.GetAllAsync());

        if (dnsServers.Count == 0)
        {
            ConsoleHelpers.ShowError("No DNS servers available. Please create a DNS server first.");
            return;
        }

        var name = AnsiConsole.Ask<string>("Device [green]name[/]:");

        var deviceType = ConsoleHelpers.SelectChoice(
            "Select device [green]type[/]:",
            "WINDOWS", "ANDROID", "MAC", "IOS", "LINUX",
            "ROUTER", "SMART_TV", "GAME_CONSOLE", "UNKNOWN");

        var selectedServer = ConsoleHelpers.SelectItem(
            "Select [green]DNS Server[/]:",
            dnsServers,
            s => $"{s.Name} ({s.Id})");

        if (selectedServer == null) return;

        var deviceCreate = new DeviceCreate(
            deviceType: deviceType,
            dnsServerId: selectedServer.Id,
            name: name);

        var newDevice = await ConsoleHelpers.WithStatusAsync(
            "Creating device...",
            () => _deviceRepository.CreateAsync(deviceCreate));

        ConsoleHelpers.ShowSuccessWithId($"Device '{newDevice.Name}' created successfully!", newDevice.Id);
    }

    private async Task DeleteDeviceAsync()
    {
        var devices = await GetDevicesWithStatusAsync();
        if (devices.Count == 0) return;

        var selected = ConsoleHelpers.SelectItem(
            "Select a device to [red]delete[/]:",
            devices,
            d => $"{d.Name} ({d.Id})");

        if (selected == null) return;

        if (!ConsoleHelpers.ConfirmAction($"Are you sure you want to delete [red]{Markup.Escape(selected.Name)}[/]?"))
        {
            ConsoleHelpers.ShowCancelled();
            return;
        }

        await ConsoleHelpers.WithStatusAsync(
            "Deleting device...",
            () => _deviceRepository.DeleteAsync(selected.Id));

        ConsoleHelpers.ShowSuccess($"Device '{selected.Name}' deleted successfully!");
    }

    private async Task<List<Device>> GetDevicesWithStatusAsync()
    {
        var devices = await ConsoleHelpers.WithStatusAsync(
            "Fetching devices...",
            () => _deviceRepository.GetAllAsync());

        if (devices.Count == 0)
        {
            ConsoleHelpers.ShowNoItemsMessage("devices");
        }

        return devices;
    }
}
