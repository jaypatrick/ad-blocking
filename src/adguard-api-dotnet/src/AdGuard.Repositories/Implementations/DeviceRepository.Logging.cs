namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Logging methods for <see cref="DeviceRepository"/>.
/// </summary>
public partial class DeviceRepository
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "Fetching all devices")]
    private partial void LogFetchingAllDevices();

    [LoggerMessage(Level = LogLevel.Information, Message = "Retrieved {Count} devices")]
    private partial void LogRetrievedDevices(int count);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Fetching device with ID: {Id}")]
    private partial void LogFetchingDevice(string id);

    [LoggerMessage(Level = LogLevel.Information, Message = "Retrieved device: {Name} ({Id})")]
    private partial void LogRetrievedDevice(string name, string id);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Device not found with ID: {Id}")]
    private partial void LogDeviceNotFound(string id);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Creating device: {Name}")]
    private partial void LogCreatingDevice(string name);

    [LoggerMessage(Level = LogLevel.Information, Message = "Device created: {Name} ({Id})")]
    private partial void LogDeviceCreated(string name, string id);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Updating device with ID: {Id}")]
    private partial void LogUpdatingDevice(string id);

    [LoggerMessage(Level = LogLevel.Information, Message = "Device updated: {Name} ({Id})")]
    private partial void LogDeviceUpdated(string name, string id);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Deleting device with ID: {Id}")]
    private partial void LogDeletingDevice(string id);

    [LoggerMessage(Level = LogLevel.Information, Message = "Device deleted: {Id}")]
    private partial void LogDeviceDeleted(string id);

    [LoggerMessage(Level = LogLevel.Error, Message = "API error in {Operation}: {ErrorCode} - {ErrorMessage}")]
    private partial void LogApiError(string operation, int errorCode, string errorMessage, Exception ex);
}
