using Stats.App.ViewModels;
using Stats.App.Views.Widgets;
using Stats.Configuration;

namespace Stats.App.Services;

public class WidgetService
{
    private readonly MainViewModel _viewModel;
    private readonly ConfigurationService _configService;
    private readonly Dictionary<WidgetType, WidgetWindow> _activeWidgets = [];

    public event EventHandler<WidgetType>? WidgetVisibilityChanged;

    public WidgetService(MainViewModel viewModel, ConfigurationService configService)
    {
        _viewModel = viewModel;
        _configService = configService;
    }

    public bool IsWidgetVisible(WidgetType widgetType) => _activeWidgets.ContainsKey(widgetType);

    public void ToggleWidget(WidgetType widgetType)
    {
        if (_activeWidgets.ContainsKey(widgetType))
        {
            HideWidget(widgetType);
        }
        else
        {
            ShowWidget(widgetType);
        }
    }

    public void ShowWidget(WidgetType widgetType)
    {
        if (_activeWidgets.ContainsKey(widgetType))
            return;

        var (content, title) = CreateWidgetContent(widgetType);
        var widget = new WidgetWindow(widgetType, content, title);

        // Restore position if saved
        var position = _configService.GetWidgetPosition(widgetType.ToString());
        if (position.HasValue)
        {
            widget.SetPosition(position.Value.X, position.Value.Y);
        }
        else
        {
            // Default position based on widget type
            var defaultPos = GetDefaultPosition(widgetType);
            widget.SetPosition(defaultPos.X, defaultPos.Y);
        }

        widget.WidgetClosed += (s, e) =>
        {
            SaveWidgetPosition(widgetType);
            _activeWidgets.Remove(widgetType);
            WidgetVisibilityChanged?.Invoke(this, widgetType);
        };

        widget.Activate();
        _activeWidgets[widgetType] = widget;
        WidgetVisibilityChanged?.Invoke(this, widgetType);
    }

    public void HideWidget(WidgetType widgetType)
    {
        if (!_activeWidgets.TryGetValue(widgetType, out var widget))
            return;

        SaveWidgetPosition(widgetType);
        widget.Close();
        _activeWidgets.Remove(widgetType);
        WidgetVisibilityChanged?.Invoke(this, widgetType);
    }

    public void SaveAllPositions()
    {
        foreach (var (type, widget) in _activeWidgets)
        {
            SaveWidgetPosition(type);
        }
    }

    private void SaveWidgetPosition(WidgetType widgetType)
    {
        if (_activeWidgets.TryGetValue(widgetType, out var widget))
        {
            var pos = widget.GetPosition();
            _configService.SetWidgetPosition(widgetType.ToString(), pos.X, pos.Y);
        }
    }

    private (Microsoft.UI.Xaml.UIElement Content, string Title) CreateWidgetContent(WidgetType widgetType)
    {
        return widgetType switch
        {
            WidgetType.Cpu => (new CpuWidgetContent(_viewModel), "CPU"),
            WidgetType.Gpu => (new GpuWidgetContent(_viewModel), "GPU"),
            WidgetType.Memory => (new MemoryWidgetContent(_viewModel), "Memory"),
            WidgetType.Network => (new NetworkWidgetContent(_viewModel), "Network"),
            _ => throw new ArgumentException($"Unknown widget type: {widgetType}")
        };
    }

    private static (int X, int Y) GetDefaultPosition(WidgetType widgetType)
    {
        // Position widgets in top-right corner, stacked vertically
        const int rightMargin = 20;
        const int topMargin = 50;
        const int spacing = 140;

        var index = (int)widgetType;
        return (
            X: 1920 - 220 - rightMargin, // Assume 1920 width, adjust at runtime if needed
            Y: topMargin + (index * spacing)
        );
    }

    public void CloseAllWidgets()
    {
        SaveAllPositions();
        foreach (var widget in _activeWidgets.Values.ToList())
        {
            widget.Close();
        }
        _activeWidgets.Clear();
    }
}
