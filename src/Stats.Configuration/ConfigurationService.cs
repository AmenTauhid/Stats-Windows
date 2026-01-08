using System.Text.Json;

namespace Stats.Configuration;

public class ConfigurationService
{
    private readonly string _settingsPath;
    private AppSettings _settings;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    public AppSettings Settings => _settings;

    public ConfigurationService()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var statsFolder = Path.Combine(appDataPath, "Stats");
        Directory.CreateDirectory(statsFolder);
        _settingsPath = Path.Combine(statsFolder, "settings.json");

        _settings = LoadSettings();
    }

    private AppSettings LoadSettings()
    {
        try
        {
            if (File.Exists(_settingsPath))
            {
                var json = File.ReadAllText(_settingsPath);
                return JsonSerializer.Deserialize<AppSettings>(json, _jsonOptions) ?? new AppSettings();
            }
        }
        catch
        {
            // If loading fails, return default settings
        }

        return new AppSettings();
    }

    public void Save()
    {
        try
        {
            var json = JsonSerializer.Serialize(_settings, _jsonOptions);
            File.WriteAllText(_settingsPath, json);
        }
        catch
        {
            // Silently fail - settings are not critical
        }
    }

    public (int X, int Y)? GetWidgetPosition(string widgetType)
    {
        if (_settings.WidgetPositions.TryGetValue(widgetType, out var position))
        {
            return (position.X, position.Y);
        }
        return null;
    }

    public void SetWidgetPosition(string widgetType, int x, int y)
    {
        _settings.WidgetPositions[widgetType] = new WidgetPosition { X = x, Y = y };
        Save();
    }

    public bool IsWidgetEnabled(string widgetType)
    {
        return _settings.EnabledWidgets.Contains(widgetType);
    }

    public void SetWidgetEnabled(string widgetType, bool enabled)
    {
        if (enabled)
            _settings.EnabledWidgets.Add(widgetType);
        else
            _settings.EnabledWidgets.Remove(widgetType);
        Save();
    }

    public IEnumerable<string> GetEnabledWidgets()
    {
        return _settings.EnabledWidgets;
    }
}
