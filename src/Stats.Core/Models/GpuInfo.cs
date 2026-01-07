namespace Stats.Core.Models;

public record GpuInfo
{
    public string Name { get; init; } = string.Empty;
    public GpuVendor Vendor { get; init; }
    public float CoreLoad { get; init; }
    public float MemoryLoad { get; init; }
    public float Temperature { get; init; }
    public float CoreClock { get; init; }
    public float MemoryClock { get; init; }
    public long MemoryUsed { get; init; }
    public long MemoryTotal { get; init; }
    public float Power { get; init; }
    public float FanSpeed { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public enum GpuVendor
{
    Unknown,
    Nvidia,
    Amd,
    Intel
}
