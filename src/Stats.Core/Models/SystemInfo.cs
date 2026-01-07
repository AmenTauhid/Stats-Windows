namespace Stats.Core.Models;

public record SystemInfo
{
    public CpuInfo? Cpu { get; init; }
    public GpuInfo? Gpu { get; init; }
    public MemoryInfo? Memory { get; init; }
    public IReadOnlyList<DiskInfo> Disks { get; init; } = [];
    public IReadOnlyList<NetworkInfo> Networks { get; init; } = [];
    public BatteryInfo? Battery { get; init; }
    public IReadOnlyList<SensorInfo> Sensors { get; init; } = [];
    public IReadOnlyList<FanInfo> Fans { get; init; } = [];
    public IReadOnlyList<BluetoothDeviceInfo> BluetoothDevices { get; init; } = [];
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
