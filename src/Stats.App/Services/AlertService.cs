using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Stats.Configuration;
using Stats.Core.Interfaces;
using Stats.Core.Models;

namespace Stats.App.Services;

public class AlertService : IDisposable
{
    private readonly IHardwareMonitor _monitor;
    private readonly ConfigurationService _configService;
    private readonly Dictionary<string, DateTime> _lastAlerts = [];
    private readonly TimeSpan _alertCooldown = TimeSpan.FromMinutes(5);
    private bool _disposed;

    public AlertService(IHardwareMonitor monitor, ConfigurationService configService)
    {
        _monitor = monitor;
        _configService = configService;

        _monitor.CpuUpdated += OnCpuUpdated;
        _monitor.GpuUpdated += OnGpuUpdated;
        _monitor.SensorsUpdated += OnSensorsUpdated;
    }

    private void OnCpuUpdated(object? sender, CpuInfo cpu)
    {
        if (!_configService.Settings.EnableTemperatureAlerts)
            return;

        var threshold = _configService.Settings.CpuTempThreshold;
        if (cpu.PackageTemperature >= threshold)
        {
            ShowTemperatureAlert("CPU", cpu.Name, cpu.PackageTemperature, threshold);
        }
    }

    private void OnGpuUpdated(object? sender, GpuInfo gpu)
    {
        if (!_configService.Settings.EnableTemperatureAlerts)
            return;

        var threshold = _configService.Settings.GpuTempThreshold;
        if (gpu.Temperature >= threshold)
        {
            ShowTemperatureAlert("GPU", gpu.Name, gpu.Temperature, threshold);
        }
    }

    private void OnSensorsUpdated(object? sender, IReadOnlyList<SensorInfo> sensors)
    {
        if (!_configService.Settings.EnableTemperatureAlerts)
            return;

        var threshold = _configService.Settings.GeneralTempThreshold;
        foreach (var sensor in sensors.Where(s => s.Category == SensorCategory.Temperature))
        {
            if (sensor.Value >= threshold)
            {
                ShowTemperatureAlert(sensor.HardwareName, sensor.Name, sensor.Value, threshold);
            }
        }
    }

    private void ShowTemperatureAlert(string component, string name, float temperature, float threshold)
    {
        var alertKey = $"{component}:{name}";

        // Check cooldown
        if (_lastAlerts.TryGetValue(alertKey, out var lastAlert))
        {
            if (DateTime.UtcNow - lastAlert < _alertCooldown)
                return;
        }

        _lastAlerts[alertKey] = DateTime.UtcNow;

        try
        {
            var builder = new AppNotificationBuilder()
                .AddText($"High Temperature Warning")
                .AddText($"{component}: {name}")
                .AddText($"Current: {temperature:F0}°C (Threshold: {threshold:F0}°C)");

            var notification = builder.BuildNotification();
            AppNotificationManager.Default.Show(notification);
        }
        catch
        {
            // Notifications may not be available in all contexts
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _monitor.CpuUpdated -= OnCpuUpdated;
        _monitor.GpuUpdated -= OnGpuUpdated;
        _monitor.SensorsUpdated -= OnSensorsUpdated;
    }
}
