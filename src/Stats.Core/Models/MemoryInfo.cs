namespace Stats.Core.Models;

public record MemoryInfo
{
    public long Used { get; init; }
    public long Available { get; init; }
    public long Total { get; init; }
    public float UsedPercentage => Total > 0 ? (float)Used / Total * 100 : 0;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
