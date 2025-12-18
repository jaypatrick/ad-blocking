namespace AdGuard.ConsoleUI.Services;

/// <summary>
/// Menu service for managing DNS servers.
/// Uses Repository pattern for data access and Strategy pattern for display.
/// </summary>
public class DnsServerMenuService : BaseMenuService
{
    private readonly IDnsServerRepository _dnsServerRepository;
    private readonly IDisplayStrategy<DNSServer> _displayStrategy;

    public DnsServerMenuService(
        IDnsServerRepository dnsServerRepository,
        IDisplayStrategy<DNSServer> displayStrategy)
    {
        _dnsServerRepository = dnsServerRepository ?? throw new ArgumentNullException(nameof(dnsServerRepository));
        _displayStrategy = displayStrategy ?? throw new ArgumentNullException(nameof(displayStrategy));
    }

    /// <inheritdoc />
    public override string Title => "DNS Server Management";

    /// <inheritdoc />
    protected override Dictionary<string, Func<Task>> GetMenuActions() => new()
    {
        { "List DNS Servers", ListDnsServersAsync },
        { "View Server Details", ViewServerDetailsAsync },
        { "Create DNS Server", CreateDnsServerAsync },
        { "Delete DNS Server", DeleteDnsServerAsync }
    };

    private async Task ListDnsServersAsync()
    {
        var servers = await ConsoleHelpers.WithStatusAsync(
            "Fetching DNS servers...",
            () => _dnsServerRepository.GetAllAsync());

        _displayStrategy.Display(servers);
    }

    private async Task ViewServerDetailsAsync()
    {
        var servers = await GetServersWithStatusAsync();
        if (servers.Count == 0) return;

        var selected = ConsoleHelpers.SelectItem(
            "Select a DNS server:",
            servers,
            s => $"{s.Name} ({s.Id})");

        if (selected == null) return;

        _displayStrategy.DisplayDetails(selected);
    }

    private async Task CreateDnsServerAsync()
    {
        var name = AnsiConsole.Ask<string>("DNS Server [green]name[/]:");

        var dnsServerCreate = new DNSServerCreate(name: name);

        var newServer = await ConsoleHelpers.WithStatusAsync(
            "Creating DNS server...",
            () => _dnsServerRepository.CreateAsync(dnsServerCreate));

        ConsoleHelpers.ShowSuccessWithId($"DNS Server '{newServer.Name}' created successfully!", newServer.Id);
    }

    private async Task DeleteDnsServerAsync()
    {
        var servers = await GetServersWithStatusAsync();
        if (servers.Count == 0) return;

        // Filter out default server - can't delete it
        var deletableServers = servers.Where(s => !s.Default).ToList();
        if (deletableServers.Count == 0)
        {
            ConsoleHelpers.ShowWarning("No deletable DNS servers found. Default server cannot be deleted.");
            AnsiConsole.WriteLine();
            return;
        }

        var selected = ConsoleHelpers.SelectItem(
            "Select a DNS server to [red]delete[/]:",
            deletableServers,
            s => $"{s.Name} ({s.Id})");

        if (selected == null) return;

        if (selected.DeviceIds.Count > 0)
        {
            ConsoleHelpers.ShowWarning($"Warning: This server has {selected.DeviceIds.Count} device(s) attached.");
        }

        if (!ConsoleHelpers.ConfirmAction($"Are you sure you want to delete [red]{Markup.Escape(selected.Name)}[/]?"))
        {
            ConsoleHelpers.ShowCancelled();
            return;
        }

        await ConsoleHelpers.WithStatusAsync(
            "Deleting DNS server...",
            () => _dnsServerRepository.DeleteAsync(selected.Id));

        ConsoleHelpers.ShowSuccess($"DNS Server '{selected.Name}' deleted successfully!");
    }

    private async Task<List<DNSServer>> GetServersWithStatusAsync()
    {
        var servers = await ConsoleHelpers.WithStatusAsync(
            "Fetching DNS servers...",
            () => _dnsServerRepository.GetAllAsync());

        if (servers.Count == 0)
        {
            ConsoleHelpers.ShowNoItemsMessage("DNS servers");
        }

        return servers;
    }
}
