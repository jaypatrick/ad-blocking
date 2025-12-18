namespace AdGuard.ConsoleUI.Display;

/// <summary>
/// Display strategy for Device entities.
/// </summary>
public class DeviceDisplayStrategy : IDisplayStrategy<Device>
{
    /// <inheritdoc />
    public void Display(IEnumerable<Device> items)
    {
        var deviceList = items.ToList();

        if (deviceList.Count == 0)
        {
            ConsoleHelpers.ShowNoItemsMessage("devices");
            return;
        }

        var table = TableBuilderExtensions.CreateStandardTable("ID", "Name", "Type", "DNS Server ID");

        foreach (var device in deviceList)
        {
            table.AddRow(
                device.Id,
                Markup.Escape(device.Name),
                device.DeviceType.ToString(),
                device.DnsServerId);
        }

        table.Display();
    }

    /// <inheritdoc />
    public void DisplayDetails(Device device)
    {
        TableBuilderExtensions.DisplayPanel(
            $"Device: {Markup.Escape(device.Name)}",
            $"[bold]ID:[/] {device.Id}",
            $"[bold]Name:[/] {Markup.Escape(device.Name)}",
            $"[bold]Type:[/] {device.DeviceType}",
            $"[bold]DNS Server ID:[/] {device.DnsServerId}",
            $"[bold]DNS Addresses:[/]",
            $"  {device.DnsAddresses}",
            $"[bold]Settings:[/]",
            $"  {device.Settings}");
    }
}
