namespace AdGuard.ConsoleUI.Display;

/// <summary>
/// Display strategy for DedicatedIPv4Address entities.
/// </summary>
public class DedicatedIPDisplayStrategy : IDisplayStrategy<DedicatedIPv4Address>
{
    /// <inheritdoc />
    public void Display(IEnumerable<DedicatedIPv4Address> items)
    {
        var addresses = items.ToList();

        TableBuilderExtensions.DisplayRule("Dedicated IPv4 Addresses");

        if (addresses.Count == 0)
        {
            ConsoleHelpers.ShowNoItemsMessage("dedicated IP addresses");
            return;
        }

        var table = TableBuilderExtensions.CreateStandardTable("IP Address", "Linked Device ID", "Status");

        foreach (var address in addresses)
        {
            var deviceId = address.DeviceId ?? "N/A";
            var status = string.IsNullOrEmpty(address.DeviceId) ? "[yellow]Unlinked[/]" : "[green]Linked[/]";

            table.AddRow(
                address.Ip ?? "N/A",
                deviceId,
                status);
        }

        table.Display();
        AnsiConsole.MarkupLine($"[grey]Total: {addresses.Count} dedicated IP addresses[/]");
        AnsiConsole.WriteLine();
    }

    /// <inheritdoc />
    public void DisplayDetails(DedicatedIPv4Address address)
    {
        var status = string.IsNullOrEmpty(address.DeviceId) ? "Unlinked" : "Linked";
        
        TableBuilderExtensions.DisplayPanel(
            $"Dedicated IP: {address.Ip ?? "N/A"}",
            $"[bold]IP Address:[/] {address.Ip ?? "N/A"}",
            $"[bold]Linked Device:[/] {address.DeviceId ?? "Not linked to any device"}",
            $"[bold]Status:[/] {status}");
    }
}
