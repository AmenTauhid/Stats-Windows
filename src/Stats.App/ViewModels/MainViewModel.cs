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
    }

    [ObservableProperty]
    private string _cpuName = "Loading...";

    [ObservableProperty]
    private float _cpuLoad;

    [ObservableProperty]
    private float _cpuTemperature;

    [ObservableProperty]
    private string _memoryUsed = "0 GB";

    [ObservableProperty]
    private string _memoryTotal = "0 GB";

    [ObservableProperty]
    private float _memoryPercentage;

    [ObservableProperty]
    private string _gpuName = "Loading...";

    [ObservableProperty]
    private float _gpuLoad;

    [ObservableProperty]
    private float _gpuTemperature;

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

    private static string FormatBytes(long bytes)
    {
        const long GB = 1024L * 1024 * 1024;
        return $"{bytes / (double)GB:F1} GB";
    }
}
