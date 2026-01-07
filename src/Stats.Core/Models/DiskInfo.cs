namespace Stats.Core.Models;

public record DiskInfo
{
    public string Name { get; init; } = string.Empty;
    public string DriveLetter { get; init; } = string.Empty;
    public long ReadRate { get; init; }
    public long WriteRate { get; init; }
    public long UsedSpace { get; init; }
    public long TotalSpace { get; init; }
    public float Temperature { get; init; }
    public float UsagePercentage => TotalSpace > 0 ? (float)UsedSpace / TotalSpace * 100 : 0;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
