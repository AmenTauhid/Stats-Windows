namespace Stats.Core.Models;

public record BatteryInfo
{
    public bool IsPresent { get; init; }
    public float ChargeLevel { get; init; }
    public BatteryStatus Status { get; init; }
    public int DesignCapacity { get; init; }
    public int FullChargeCapacity { get; init; }
    public int RemainingCapacity { get; init; }
    public int ChargeRate { get; init; }
    public TimeSpan? TimeRemaining { get; init; }
    public int CycleCount { get; init; }
    public float HealthPercentage => DesignCapacity > 0
        ? (float)FullChargeCapacity / DesignCapacity * 100 : 100;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public enum BatteryStatus
{
    NotPresent,
    Discharging,
    Idle,
    Charging
}
