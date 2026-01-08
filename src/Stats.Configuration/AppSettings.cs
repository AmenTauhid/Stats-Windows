namespace Stats.Configuration;

public class AppSettings
{
    public bool StartWithWindows { get; set; } = false;
    public bool StartMinimized { get; set; } = false;
    public int UpdateIntervalMs { get; set; } = 1000;
    public string Theme { get; set; } = "System";
    public Dictionary<string, WidgetPosition> WidgetPositions { get; set; } = [];
    public HashSet<string> EnabledWidgets { get; set; } = [];

    // Temperature Alerts
    public bool EnableTemperatureAlerts { get; set; } = true;
    public float CpuTempThreshold { get; set; } = 85;
    public float GpuTempThreshold { get; set; } = 85;
    public float GeneralTempThreshold { get; set; } = 90;

    // Module Enable/Disable
    public bool EnableCpuMonitoring { get; set; } = true;
    public bool EnableGpuMonitoring { get; set; } = true;
    public bool EnableMemoryMonitoring { get; set; } = true;
    public bool EnableDiskMonitoring { get; set; } = true;
    public bool EnableNetworkMonitoring { get; set; } = true;
    public bool EnableBatteryMonitoring { get; set; } = true;
    public bool EnableFanMonitoring { get; set; } = true;
    public bool EnableSensorMonitoring { get; set; } = true;
}

public class WidgetPosition
{
    public int X { get; set; }
    public int Y { get; set; }
}
