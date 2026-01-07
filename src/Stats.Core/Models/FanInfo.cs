namespace Stats.Core.Models;

public record FanInfo
{
    public string Name { get; init; } = string.Empty;
    public float CurrentRpm { get; init; }
    public float? TargetRpm { get; init; }
    public float? SpeedPercentage { get; init; }
    public bool IsControllable { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
