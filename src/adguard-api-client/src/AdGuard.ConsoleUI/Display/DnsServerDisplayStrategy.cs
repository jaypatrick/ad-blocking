namespace AdGuard.ConsoleUI.Display;

/// <summary>
/// Display strategy for DNSServer entities.
/// </summary>
public class DnsServerDisplayStrategy : IDisplayStrategy<DNSServer>
{
    /// <inheritdoc />
    public void Display(IEnumerable<DNSServer> items)
    {
        var serverList = items.ToList();

        if (serverList.Count == 0)
        {
            ConsoleHelpers.ShowNoItemsMessage("DNS servers");
            return;
        }

        var table = TableBuilderExtensions.CreateStandardTable("ID", "Name", "Default", "Devices");

        foreach (var server in serverList)
        {
            table.AddRow(
                server.Id,
                Markup.Escape(server.Name),
                server.Default ? "[green]Yes[/]" : "No",
                server.DeviceIds.Count.ToString());
        }

        table.Display();
    }

    /// <inheritdoc />
    public void DisplayDetails(DNSServer server)
    {
        var deviceList = server.DeviceIds.Count > 0
            ? string.Join(", ", server.DeviceIds)
            : "[grey]None[/]";

        TableBuilderExtensions.DisplayPanel(
            $"DNS Server: {Markup.Escape(server.Name)}",
            $"[bold]ID:[/] {server.Id}",
            $"[bold]Name:[/] {Markup.Escape(server.Name)}",
            $"[bold]Default:[/] {(server.Default ? "[green]Yes[/]" : "No")}",
            $"[bold]Device IDs:[/] {deviceList}",
            $"[bold]Settings:[/]",
            $"  {server.Settings}");
    }
}
