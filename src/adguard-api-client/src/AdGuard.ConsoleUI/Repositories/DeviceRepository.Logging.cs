using Microsoft.Extensions.Logging;

namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Source-generated logging for DeviceRepository.
/// Uses LoggerMessage.Define for high-performance logging.
/// </summary>
public partial class DeviceRepository
{
    // High-frequency logging delegates using LoggerMessage source generator (C# 10+)
    // These are compiled at compile-time for zero-allocation logging

    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Debug,
        Message = "Fetching all devices")]
    partial void LogFetchingAllDevices();

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Information,
        Message = "Retrieved {Count} devices")]
    partial void LogRetrievedDevices(int count);

    [LoggerMessage(
        EventId = 1003,
        Level = LogLevel.Debug,
        Message = "Fetching device with ID: {DeviceId}")]
    partial void LogFetchingDevice(string deviceId);

    [LoggerMessage(
        EventId = 1004,
        Level = LogLevel.Information,
        Message = "Retrieved device: {DeviceName} (ID: {DeviceId})")]
    partial void LogRetrievedDevice(string deviceName, string deviceId);

    [LoggerMessage(
        EventId = 1005,
        Level = LogLevel.Warning,
        Message = "Device not found: {DeviceId}")]
    partial void LogDeviceNotFound(string deviceId);

    [LoggerMessage(
        EventId = 1006,
        Level = LogLevel.Debug,
        Message = "Creating device: {DeviceName}")]
    partial void LogCreatingDevice(string deviceName);

    [LoggerMessage(
        EventId = 1007,
        Level = LogLevel.Information,
        Message = "Created device: {DeviceName} (ID: {DeviceId})")]
    partial void LogDeviceCreated(string deviceName, string deviceId);

    [LoggerMessage(
        EventId = 1008,
        Level = LogLevel.Debug,
        Message = "Deleting device with ID: {DeviceId}")]
    partial void LogDeletingDevice(string deviceId);

    [LoggerMessage(
        EventId = 1009,
        Level = LogLevel.Information,
        Message = "Deleted device: {DeviceId}")]
    partial void LogDeviceDeleted(string deviceId);

    [LoggerMessage(
        EventId = 1010,
        Level = LogLevel.Warning,
        Message = "Attempted to get device with null or empty ID")]
    partial void LogAttemptedNullDeviceId();

    [LoggerMessage(
        EventId = 1011,
        Level = LogLevel.Warning,
        Message = "Attempted to delete device with null or empty ID")]
    partial void LogAttemptedNullDeleteDeviceId();

    [LoggerMessage(
        EventId = 1012,
        Level = LogLevel.Error,
        Message = "API error while fetching devices: {ErrorCode} - {Message}")]
    partial void LogApiErrorFetchingDevices(int errorCode, string message, Exception ex);

    [LoggerMessage(
        EventId = 1013,
        Level = LogLevel.Error,
        Message = "API error while fetching device {DeviceId}: {ErrorCode} - {Message}")]
    partial void LogApiErrorFetchingDevice(string deviceId, int errorCode, string message, Exception ex);

    [LoggerMessage(
        EventId = 1014,
        Level = LogLevel.Error,
        Message = "API error while creating device {DeviceName}: {ErrorCode} - {Message}")]
    partial void LogApiErrorCreatingDevice(string deviceName, int errorCode, string message, Exception ex);

    [LoggerMessage(
        EventId = 1015,
        Level = LogLevel.Error,
        Message = "API error while deleting device {DeviceId}: {ErrorCode} - {Message}")]
    partial void LogApiErrorDeletingDevice(string deviceId, int errorCode, string message, Exception ex);

    [LoggerMessage(
        EventId = 1016,
        Level = LogLevel.Warning,
        Message = "Device not found for deletion: {DeviceId}")]
    partial void LogDeviceNotFoundForDeletion(string deviceId);
}
