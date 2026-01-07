namespace Stats.Core.Models;

public record BluetoothDeviceInfo
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public BluetoothConnectionStatus ConnectionStatus { get; init; }
    public int? BatteryLevel { get; init; }
    public BluetoothDeviceType DeviceType { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public enum BluetoothConnectionStatus
{
    Connected,
    Disconnected,
    Pairing
}

public enum BluetoothDeviceType
{
    Unknown,
    Headphones,
    Keyboard,
    Mouse,
    Gamepad,
    Speaker,
    Other
}
