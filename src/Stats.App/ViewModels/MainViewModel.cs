using System.Collections.ObjectModel;
using Microsoft.UI.Dispatching;
using Stats.Core.Interfaces;
using Stats.Core.Models;

namespace Stats.App.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly IHardwareMonitor _monitor;
    private readonly DispatcherQueue _dispatcherQueue;

    public MainViewModel(IHardwareMonitor monitor)
    {
        _monitor = monitor;
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        Title = "Stats";

        _monitor.CpuUpdated += OnCpuUpdated;
        _monitor.MemoryUpdated += OnMemoryUpdated;
        _monitor.GpuUpdated += OnGpuUpdated;
        _monitor.DisksUpdated += OnDisksUpdated;
        _monitor.NetworksUpdated += OnNetworksUpdated;
        _monitor.BatteryUpdated += OnBatteryUpdated;
    }

    // CPU Properties
    [ObservableProperty]
    private string _cpuName = "Loading...";

    [ObservableProperty]
    private float _cpuLoad;

    [ObservableProperty]
    private float _cpuTemperature;

    // Memory Properties
    [ObservableProperty]
    private string _memoryUsed = "0 GB";

    [ObservableProperty]
    private string _memoryTotal = "0 GB";

    [ObservableProperty]
    private float _memoryPercentage;

    // GPU Properties
    [ObservableProperty]
    private string _gpuName = "Loading...";

    [ObservableProperty]
    private float _gpuLoad;

    [ObservableProperty]
    private float _gpuTemperature;

    // Disk Properties
    [ObservableProperty]
    private ObservableCollection<DiskViewModel> _disks = [];

    // Network Properties
    [ObservableProperty]
    private ObservableCollection<NetworkViewModel> _networks = [];

    [ObservableProperty]
    private string _totalDownloadSpeed = "0 B/s";

    [ObservableProperty]
    private string _totalUploadSpeed = "0 B/s";

    // Battery Properties
    [ObservableProperty]
    private bool _hasBattery;

    [ObservableProperty]
    private float _batteryLevel;

    [ObservableProperty]
    private string _batteryStatus = "Unknown";

    [ObservableProperty]
    private string _batteryHealth = "0%";

    [ObservableProperty]
    private string _batteryTimeRemaining = "";

    // Event Handlers
    private void OnCpuUpdated(object? sender, CpuInfo cpu)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            CpuName = cpu.Name;
            CpuLoad = cpu.TotalLoad;
            CpuTemperature = cpu.PackageTemperature;
        });
    }

    private void OnMemoryUpdated(object? sender, MemoryInfo memory)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            MemoryUsed = FormatBytes(memory.Used);
            MemoryTotal = FormatBytes(memory.Total);
            MemoryPercentage = memory.UsedPercentage;
        });
    }

    private void OnGpuUpdated(object? sender, GpuInfo gpu)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            GpuName = gpu.Name;
            GpuLoad = gpu.CoreLoad;
            GpuTemperature = gpu.Temperature;
        });
    }

    private void OnDisksUpdated(object? sender, IReadOnlyList<DiskInfo> disks)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            Disks.Clear();
            foreach (var disk in disks)
            {
                Disks.Add(new DiskViewModel
                {
                    Name = disk.Name,
                    ReadSpeed = FormatSpeed(disk.ReadRate),
                    WriteSpeed = FormatSpeed(disk.WriteRate),
                    Temperature = disk.Temperature,
                    UsagePercentage = disk.UsagePercentage
                });
            }
        });
    }

    private void OnNetworksUpdated(object? sender, IReadOnlyList<NetworkInfo> networks)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            long totalDown = 0, totalUp = 0;

            Networks.Clear();
            foreach (var net in networks.Where(n => n.DownloadRate > 0 || n.UploadRate > 0 || n.IsConnected))
            {
                Networks.Add(new NetworkViewModel
                {
                    Name = net.AdapterName,
                    DownloadSpeed = FormatSpeed(net.DownloadRate),
                    UploadSpeed = FormatSpeed(net.UploadRate),
                    IsConnected = net.IsConnected
                });
                totalDown += net.DownloadRate;
                totalUp += net.UploadRate;
            }

            TotalDownloadSpeed = FormatSpeed(totalDown);
            TotalUploadSpeed = FormatSpeed(totalUp);
        });
    }

    private void OnBatteryUpdated(object? sender, BatteryInfo battery)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            HasBattery = battery.IsPresent;
            if (!battery.IsPresent)
            {
                BatteryStatus = "No Battery";
                return;
            }

            BatteryLevel = battery.ChargeLevel;
            BatteryStatus = battery.Status switch
            {
                Core.Models.BatteryStatus.Charging => "Charging",
                Core.Models.BatteryStatus.Discharging => "Discharging",
                Core.Models.BatteryStatus.Idle => "Plugged In",
                _ => "Unknown"
            };
            BatteryHealth = $"{battery.HealthPercentage:F0}%";
            BatteryTimeRemaining = battery.TimeRemaining.HasValue
                ? $"{battery.TimeRemaining.Value.Hours}h {battery.TimeRemaining.Value.Minutes}m"
                : "";
        });
    }

    // Formatters
    private static string FormatBytes(long bytes)
    {
        const long GB = 1024L * 1024 * 1024;
        return $"{bytes / (double)GB:F1} GB";
    }

    private static string FormatSpeed(long bytesPerSecond)
    {
        if (bytesPerSecond < 1024)
            return $"{bytesPerSecond} B/s";
        if (bytesPerSecond < 1024 * 1024)
            return $"{bytesPerSecond / 1024.0:F1} KB/s";
        if (bytesPerSecond < 1024 * 1024 * 1024)
            return $"{bytesPerSecond / (1024.0 * 1024):F1} MB/s";
        return $"{bytesPerSecond / (1024.0 * 1024 * 1024):F2} GB/s";
    }
}

// Sub-ViewModels for collections
public partial class DiskViewModel : ObservableObject
{
    [ObservableProperty]
    private string _name = "";

    [ObservableProperty]
    private string _readSpeed = "0 B/s";

    [ObservableProperty]
    private string _writeSpeed = "0 B/s";

    [ObservableProperty]
    private float _temperature;

    [ObservableProperty]
    private float _usagePercentage;
}

public partial class NetworkViewModel : ObservableObject
{
    [ObservableProperty]
    private string _name = "";

    [ObservableProperty]
    private string _downloadSpeed = "0 B/s";

    [ObservableProperty]
    private string _uploadSpeed = "0 B/s";

    [ObservableProperty]
    private bool _isConnected;
}
