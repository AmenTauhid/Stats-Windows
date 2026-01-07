using Stats.Core.Models;

namespace Stats.Core.Interfaces;

public interface IHardwareMonitor : IDisposable
{
    bool IsRunning { get; }
    TimeSpan UpdateInterval { get; set; }

    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);

    event EventHandler<CpuInfo>? CpuUpdated;
    event EventHandler<GpuInfo>? GpuUpdated;
    event EventHandler<MemoryInfo>? MemoryUpdated;
    event EventHandler<IReadOnlyList<DiskInfo>>? DisksUpdated;
    event EventHandler<IReadOnlyList<NetworkInfo>>? NetworksUpdated;
    event EventHandler<BatteryInfo>? BatteryUpdated;
    event EventHandler<IReadOnlyList<SensorInfo>>? SensorsUpdated;
    event EventHandler<IReadOnlyList<FanInfo>>? FansUpdated;
}
