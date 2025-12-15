namespace AdGuard.ConsoleUI.Services;

/// <summary>
/// Menu service for managing dedicated IP addresses.
/// Uses Repository pattern for data access and Strategy pattern for display.
/// </summary>
public class DedicatedIPMenuService : IMenuService
{
    private readonly IDedicatedIPRepository _dedicatedIPRepository;
    private readonly IDisplayStrategy<DedicatedIPv4Address> _displayStrategy;

    public DedicatedIPMenuService(
        IDedicatedIPRepository dedicatedIPRepository,
        IDisplayStrategy<DedicatedIPv4Address> displayStrategy)
    {
        _dedicatedIPRepository = dedicatedIPRepository ?? throw new ArgumentNullException(nameof(dedicatedIPRepository));
        _displayStrategy = displayStrategy ?? throw new ArgumentNullException(nameof(displayStrategy));
    }

    /// <inheritdoc />
    public string Title => "Dedicated IP Addresses";

    /// <inheritdoc />
    public async Task ShowAsync()
    {
        var running = true;

        while (running)
        {
            var choice = ConsoleHelpers.SelectChoice(
                "[green]Dedicated IP Addresses[/]",
                "List All IP Addresses",
                "Allocate New IP Address",
                "Back");

            try
            {
                switch (choice)
                {
                    case "List All IP Addresses":
                        await ListAllAddressesAsync();
                        break;
                    case "Allocate New IP Address":
                        await AllocateAddressAsync();
                        break;
                    case "Back":
                        running = false;
                        break;
                }
            }
            catch (ApiException ex)
            {
                ConsoleHelpers.ShowApiError(ex);
            }
        }
    }

    private async Task ListAllAddressesAsync()
    {
        var addresses = await ConsoleHelpers.WithStatusAsync(
            "Fetching dedicated IP addresses...",
            () => _dedicatedIPRepository.GetAllAsync());

        _displayStrategy.Display(addresses);
    }

    private async Task AllocateAddressAsync()
    {
        var confirm = AnsiConsole.Confirm("Allocate a new dedicated IPv4 address?");
        if (!confirm)
        {
            return;
        }

        var address = await ConsoleHelpers.WithStatusAsync(
            "Allocating dedicated IP address...",
            () => _dedicatedIPRepository.AllocateAsync());

        AnsiConsole.MarkupLine($"[green]✓ Allocated IP address: {address.Ip}[/]");
        AnsiConsole.WriteLine();

        _displayStrategy.DisplayDetails(address);
    }
}
