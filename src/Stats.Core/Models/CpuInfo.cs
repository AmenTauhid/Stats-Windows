namespace Stats.Core.Models;

public record CpuInfo
{
    public string Name { get; init; } = string.Empty;
    public float TotalLoad { get; init; }
    public float PackageTemperature { get; init; }
    public float PackagePower { get; init; }
    public IReadOnlyList<CoreInfo> Cores { get; init; } = [];
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public record CoreInfo
{
    public int CoreId { get; init; }
    public float Load { get; init; }
    public float Temperature { get; init; }
    public float Clock { get; init; }
}
