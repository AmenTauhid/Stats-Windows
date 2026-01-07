namespace Stats.Core.Models;

public record SensorInfo
{
    public string Name { get; init; } = string.Empty;
    public string HardwareName { get; init; } = string.Empty;
    public SensorCategory Category { get; init; }
    public float Value { get; init; }
    public float? Min { get; init; }
    public float? Max { get; init; }
    public string Unit { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public enum SensorCategory
{
    Temperature,
    Voltage,
    Power,
    Fan,
    Clock,
    Load,
    Data,
    Throughput
}
