using LibreHardwareMonitor.Hardware;
using Stats.Core.Interfaces;
using Stats.Core.Models;

namespace Stats.Hardware;

public sealed class HardwareMonitorService : IHardwareMonitor
{
    private readonly Computer _computer;
    private readonly UpdateVisitor _updateVisitor;
    private PeriodicTimer? _timer;
    private CancellationTokenSource? _cts;
    private Task? _monitorTask;
    private bool _disposed;

    public bool IsRunning { get; private set; }
    public TimeSpan UpdateInterval { get; set; } = TimeSpan.FromSeconds(1);

    public event EventHandler<CpuInfo>? CpuUpdated;
    public event EventHandler<GpuInfo>? GpuUpdated;
    public event EventHandler<MemoryInfo>? MemoryUpdated;
    public event EventHandler<IReadOnlyList<DiskInfo>>? DisksUpdated;
    public event EventHandler<IReadOnlyList<NetworkInfo>>? NetworksUpdated;
    public event EventHandler<BatteryInfo>? BatteryUpdated;
    public event EventHandler<IReadOnlyList<SensorInfo>>? SensorsUpdated;
    public event EventHandler<IReadOnlyList<FanInfo>>? FansUpdated;

    public HardwareMonitorService()
    {
        _computer = new Computer
        {
            IsCpuEnabled = true,
            IsGpuEnabled = true,
            IsMemoryEnabled = true,
            IsMotherboardEnabled = true,
            IsNetworkEnabled = true,
            IsStorageEnabled = true,
            IsBatteryEnabled = true,
            IsControllerEnabled = true
        };

        _updateVisitor = new UpdateVisitor();
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (IsRunning)
            return Task.CompletedTask;

        _computer.Open();
        IsRunning = true;

        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _timer = new PeriodicTimer(UpdateInterval);
        _monitorTask = MonitorLoopAsync(_cts.Token);

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (!IsRunning)
            return;

        _cts?.Cancel();

        if (_monitorTask != null)
        {
            try
            {
                await _monitorTask.WaitAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Expected
            }
        }

        _timer?.Dispose();
        _timer = null;
        _computer.Close();
        IsRunning = false;
    }

    private async Task MonitorLoopAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested && _timer != null)
        {
            try
            {
                if (!await _timer.WaitForNextTickAsync(ct))
                    break;

                _computer.Accept(_updateVisitor);
                ProcessHardware();
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private void ProcessHardware()
    {
        var sensors = new List<SensorInfo>();
        var fans = new List<FanInfo>();
        var disks = new List<DiskInfo>();
        var networks = new List<NetworkInfo>();

        foreach (var hardware in _computer.Hardware)
        {
            hardware.Update();
            ProcessHardwareItem(hardware, sensors, fans, disks, networks);
        }

        if (sensors.Count > 0)
            SensorsUpdated?.Invoke(this, sensors);

        if (fans.Count > 0)
            FansUpdated?.Invoke(this, fans);

        if (disks.Count > 0)
            DisksUpdated?.Invoke(this, disks);

        if (networks.Count > 0)
            NetworksUpdated?.Invoke(this, networks);
    }

    private void ProcessHardwareItem(
        IHardware hardware,
        List<SensorInfo> sensors,
        List<FanInfo> fans,
        List<DiskInfo> disks,
        List<NetworkInfo> networks)
    {
        switch (hardware.HardwareType)
        {
            case HardwareType.Cpu:
                ProcessCpu(hardware);
                CollectSensors(hardware, sensors, fans);
                break;

            case HardwareType.GpuNvidia:
            case HardwareType.GpuAmd:
            case HardwareType.GpuIntel:
                ProcessGpu(hardware);
                CollectSensors(hardware, sensors, fans);
                break;

            case HardwareType.Memory:
                ProcessMemory(hardware);
                break;

            case HardwareType.Storage:
                ProcessStorage(hardware, disks);
                CollectSensors(hardware, sensors, fans);
                break;

            case HardwareType.Network:
                ProcessNetwork(hardware, networks);
                break;

            case HardwareType.Battery:
                ProcessBattery(hardware);
                break;

            case HardwareType.Motherboard:
            case HardwareType.SuperIO:
            case HardwareType.EmbeddedController:
                CollectSensors(hardware, sensors, fans);
                break;
        }

        foreach (var subHardware in hardware.SubHardware)
        {
            subHardware.Update();
            ProcessHardwareItem(subHardware, sensors, fans, disks, networks);
        }
    }

    private void ProcessCpu(IHardware cpu)
    {
        var cores = new List<CoreInfo>();
        float totalLoad = 0;
        float packageTemp = 0;
        float packagePower = 0;

        foreach (var sensor in cpu.Sensors)
        {
            switch (sensor.SensorType)
            {
                case SensorType.Load when sensor.Name == "CPU Total":
                    totalLoad = sensor.Value ?? 0;
                    break;

                case SensorType.Temperature when sensor.Name.Contains("Package") || sensor.Name.Contains("Average"):
                    packageTemp = sensor.Value ?? 0;
                    break;

                case SensorType.Power when sensor.Name.Contains("Package"):
                    packagePower = sensor.Value ?? 0;
                    break;

                case SensorType.Load when sensor.Name.StartsWith("CPU Core"):
                    if (int.TryParse(sensor.Name.Replace("CPU Core #", ""), out int coreId))
                    {
                        var existingCore = cores.FirstOrDefault(c => c.CoreId == coreId);
                        if (existingCore == null)
                        {
                            cores.Add(new CoreInfo { CoreId = coreId, Load = sensor.Value ?? 0 });
                        }
                    }
                    break;
            }
        }

        // Update core temperatures and clocks
        foreach (var sensor in cpu.Sensors)
        {
            if (sensor.Name.StartsWith("Core #") || sensor.Name.StartsWith("CPU Core #"))
            {
                var match = System.Text.RegularExpressions.Regex.Match(sensor.Name, @"#(\d+)");
                if (match.Success && int.TryParse(match.Groups[1].Value, out int coreId))
                {
                    var core = cores.FirstOrDefault(c => c.CoreId == coreId);
                    if (core != null)
                    {
                        var index = cores.IndexOf(core);
                        cores[index] = sensor.SensorType switch
                        {
                            SensorType.Temperature => core with { Temperature = sensor.Value ?? 0 },
                            SensorType.Clock => core with { Clock = sensor.Value ?? 0 },
                            _ => core
                        };
                    }
                }
            }
        }

        var cpuInfo = new CpuInfo
        {
            Name = cpu.Name,
            TotalLoad = totalLoad,
            PackageTemperature = packageTemp,
            PackagePower = packagePower,
            Cores = cores.OrderBy(c => c.CoreId).ToList()
        };

        CpuUpdated?.Invoke(this, cpuInfo);
    }

    private void ProcessGpu(IHardware gpu)
    {
        var vendor = gpu.HardwareType switch
        {
            HardwareType.GpuNvidia => GpuVendor.Nvidia,
            HardwareType.GpuAmd => GpuVendor.Amd,
            HardwareType.GpuIntel => GpuVendor.Intel,
            _ => GpuVendor.Unknown
        };

        float coreLoad = 0, memoryLoad = 0, temperature = 0;
        float coreClock = 0, memoryClock = 0, power = 0, fanSpeed = 0;
        long memoryUsed = 0, memoryTotal = 0;

        foreach (var sensor in gpu.Sensors)
        {
            switch (sensor.SensorType)
            {
                case SensorType.Load when sensor.Name.Contains("Core"):
                    coreLoad = sensor.Value ?? 0;
                    break;

                case SensorType.Load when sensor.Name.Contains("Memory"):
                    memoryLoad = sensor.Value ?? 0;
                    break;

                case SensorType.Temperature when sensor.Name.Contains("Core") || sensor.Name == "GPU Core":
                    temperature = sensor.Value ?? 0;
                    break;

                case SensorType.Clock when sensor.Name.Contains("Core"):
                    coreClock = sensor.Value ?? 0;
                    break;

                case SensorType.Clock when sensor.Name.Contains("Memory"):
                    memoryClock = sensor.Value ?? 0;
                    break;

                case SensorType.Power when sensor.Name.Contains("GPU") || sensor.Name == "Power":
                    power = sensor.Value ?? 0;
                    break;

                case SensorType.Fan:
                    fanSpeed = sensor.Value ?? 0;
                    break;

                case SensorType.SmallData when sensor.Name.Contains("Used"):
                    memoryUsed = (long)((sensor.Value ?? 0) * 1024 * 1024); // Convert MB to bytes
                    break;

                case SensorType.SmallData when sensor.Name.Contains("Total"):
                    memoryTotal = (long)((sensor.Value ?? 0) * 1024 * 1024);
                    break;
            }
        }

        var gpuInfo = new GpuInfo
        {
            Name = gpu.Name,
            Vendor = vendor,
            CoreLoad = coreLoad,
            MemoryLoad = memoryLoad,
            Temperature = temperature,
            CoreClock = coreClock,
            MemoryClock = memoryClock,
            Power = power,
            FanSpeed = fanSpeed,
            MemoryUsed = memoryUsed,
            MemoryTotal = memoryTotal
        };

        GpuUpdated?.Invoke(this, gpuInfo);
    }

    private void ProcessMemory(IHardware memory)
    {
        float used = 0, available = 0;

        foreach (var sensor in memory.Sensors)
        {
            switch (sensor.SensorType)
            {
                case SensorType.Data when sensor.Name == "Memory Used":
                    used = sensor.Value ?? 0;
                    break;

                case SensorType.Data when sensor.Name == "Memory Available":
                    available = sensor.Value ?? 0;
                    break;
            }
        }

        var memoryInfo = new MemoryInfo
        {
            Used = (long)(used * 1024 * 1024 * 1024), // Convert GB to bytes
            Available = (long)(available * 1024 * 1024 * 1024),
            Total = (long)((used + available) * 1024 * 1024 * 1024)
        };

        MemoryUpdated?.Invoke(this, memoryInfo);
    }

    private void ProcessStorage(IHardware storage, List<DiskInfo> disks)
    {
        long readRate = 0, writeRate = 0;
        long usedSpace = 0, totalSpace = 0;
        float temperature = 0;

        foreach (var sensor in storage.Sensors)
        {
            switch (sensor.SensorType)
            {
                case SensorType.Throughput when sensor.Name.Contains("Read"):
                    readRate = (long)(sensor.Value ?? 0);
                    break;

                case SensorType.Throughput when sensor.Name.Contains("Write"):
                    writeRate = (long)(sensor.Value ?? 0);
                    break;

                case SensorType.Temperature:
                    temperature = sensor.Value ?? 0;
                    break;

                case SensorType.Data when sensor.Name.Contains("Used"):
                    usedSpace = (long)((sensor.Value ?? 0) * 1024 * 1024 * 1024);
                    break;

                case SensorType.Load when sensor.Name.Contains("Used Space"):
                    // This is percentage, we'll calculate from it if we have total
                    break;
            }
        }

        disks.Add(new DiskInfo
        {
            Name = storage.Name,
            ReadRate = readRate,
            WriteRate = writeRate,
            UsedSpace = usedSpace,
            TotalSpace = totalSpace,
            Temperature = temperature
        });
    }

    private void ProcessNetwork(IHardware network, List<NetworkInfo> networks)
    {
        long downloadRate = 0, uploadRate = 0;
        long totalDownloaded = 0, totalUploaded = 0;

        foreach (var sensor in network.Sensors)
        {
            switch (sensor.SensorType)
            {
                case SensorType.Throughput when sensor.Name.Contains("Download"):
                    downloadRate = (long)(sensor.Value ?? 0);
                    break;

                case SensorType.Throughput when sensor.Name.Contains("Upload"):
                    uploadRate = (long)(sensor.Value ?? 0);
                    break;

                case SensorType.Data when sensor.Name.Contains("Downloaded"):
                    totalDownloaded = (long)((sensor.Value ?? 0) * 1024 * 1024 * 1024);
                    break;

                case SensorType.Data when sensor.Name.Contains("Uploaded"):
                    totalUploaded = (long)((sensor.Value ?? 0) * 1024 * 1024 * 1024);
                    break;
            }
        }

        networks.Add(new NetworkInfo
        {
            AdapterName = network.Name,
            DownloadRate = downloadRate,
            UploadRate = uploadRate,
            TotalDownloaded = totalDownloaded,
            TotalUploaded = totalUploaded,
            IsConnected = downloadRate > 0 || uploadRate > 0
        });
    }

    private void ProcessBattery(IHardware battery)
    {
        float chargeLevel = 0;
        int designCapacity = 0, fullChargeCapacity = 0, remainingCapacity = 0;
        int chargeRate = 0;
        var status = BatteryStatus.NotPresent;

        foreach (var sensor in battery.Sensors)
        {
            switch (sensor.SensorType)
            {
                case SensorType.Level when sensor.Name.Contains("Charge"):
                    chargeLevel = sensor.Value ?? 0;
                    break;

                case SensorType.Energy when sensor.Name.Contains("Designed"):
                    designCapacity = (int)((sensor.Value ?? 0) * 1000); // Convert Wh to mWh
                    break;

                case SensorType.Energy when sensor.Name.Contains("Full"):
                    fullChargeCapacity = (int)((sensor.Value ?? 0) * 1000);
                    break;

                case SensorType.Energy when sensor.Name.Contains("Remaining"):
                    remainingCapacity = (int)((sensor.Value ?? 0) * 1000);
                    break;

                case SensorType.Power when sensor.Name.Contains("Charge") || sensor.Name.Contains("Discharge"):
                    chargeRate = (int)((sensor.Value ?? 0) * 1000);
                    if (sensor.Name.Contains("Discharge"))
                        chargeRate = -chargeRate;
                    break;
            }
        }

        status = chargeRate > 0 ? BatteryStatus.Charging :
                 chargeRate < 0 ? BatteryStatus.Discharging :
                 chargeLevel > 0 ? BatteryStatus.Idle : BatteryStatus.NotPresent;

        var batteryInfo = new BatteryInfo
        {
            IsPresent = status != BatteryStatus.NotPresent,
            ChargeLevel = chargeLevel,
            Status = status,
            DesignCapacity = designCapacity,
            FullChargeCapacity = fullChargeCapacity,
            RemainingCapacity = remainingCapacity,
            ChargeRate = chargeRate
        };

        BatteryUpdated?.Invoke(this, batteryInfo);
    }

    private void CollectSensors(IHardware hardware, List<SensorInfo> sensors, List<FanInfo> fans)
    {
        foreach (var sensor in hardware.Sensors)
        {
            var category = sensor.SensorType switch
            {
                SensorType.Temperature => SensorCategory.Temperature,
                SensorType.Voltage => SensorCategory.Voltage,
                SensorType.Power => SensorCategory.Power,
                SensorType.Fan => SensorCategory.Fan,
                SensorType.Clock => SensorCategory.Clock,
                SensorType.Load => SensorCategory.Load,
                SensorType.Data => SensorCategory.Data,
                SensorType.Throughput => SensorCategory.Throughput,
                _ => (SensorCategory?)null
            };

            if (category == null)
                continue;

            var unit = sensor.SensorType switch
            {
                SensorType.Temperature => "°C",
                SensorType.Voltage => "V",
                SensorType.Power => "W",
                SensorType.Fan => "RPM",
                SensorType.Clock => "MHz",
                SensorType.Load => "%",
                SensorType.Data => "GB",
                SensorType.Throughput => "B/s",
                _ => ""
            };

            sensors.Add(new SensorInfo
            {
                Name = sensor.Name,
                HardwareName = hardware.Name,
                Category = category.Value,
                Value = sensor.Value ?? 0,
                Min = sensor.Min,
                Max = sensor.Max,
                Unit = unit
            });

            if (sensor.SensorType == SensorType.Fan)
            {
                fans.Add(new FanInfo
                {
                    Name = $"{hardware.Name} - {sensor.Name}",
                    CurrentRpm = sensor.Value ?? 0,
                    SpeedPercentage = null,
                    IsControllable = false
                });
            }
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _cts?.Cancel();
        _timer?.Dispose();
        _computer.Close();
    }
}

internal class UpdateVisitor : IVisitor
{
    public void VisitComputer(IComputer computer)
    {
        computer.Traverse(this);
    }

    public void VisitHardware(IHardware hardware)
    {
        hardware.Update();
        foreach (var subHardware in hardware.SubHardware)
        {
            subHardware.Accept(this);
        }
    }

    public void VisitSensor(ISensor sensor) { }
    public void VisitParameter(IParameter parameter) { }
}
