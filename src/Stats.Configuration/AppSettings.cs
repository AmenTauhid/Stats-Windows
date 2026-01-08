namespace Stats.Configuration;

public class AppSettings
{
    public bool StartWithWindows { get; set; } = false;
    public bool StartMinimized { get; set; } = false;
    public int UpdateIntervalMs { get; set; } = 1000;
    public string Theme { get; set; } = "System";
    public Dictionary<string, WidgetPosition> WidgetPositions { get; set; } = [];
    public HashSet<string> EnabledWidgets { get; set; } = [];
}

public class WidgetPosition
{
    public int X { get; set; }
    public int Y { get; set; }
}
