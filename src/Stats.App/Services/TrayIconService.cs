using H.NotifyIcon;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Stats.App.Views.Widgets;
using Stats.Core.Interfaces;
using Stats.Core.Models;

namespace Stats.App.Services;

public sealed class TrayIconService : IDisposable
{
    private readonly IHardwareMonitor _monitor;
    private WidgetService? _widgetService;
    private TaskbarIcon? _trayIcon;
    private bool _disposed;
    private float _lastCpuLoad;
    private readonly Dictionary<WidgetType, ToggleMenuFlyoutItem> _widgetMenuItems = [];

    public event EventHandler? ShowDashboardRequested;
    public event EventHandler? ExitRequested;

    public TrayIconService(IHardwareMonitor monitor)
    {
        _monitor = monitor;
        _monitor.CpuUpdated += OnCpuUpdated;
    }

    public void SetWidgetService(WidgetService widgetService)
    {
        _widgetService = widgetService;
        _widgetService.WidgetVisibilityChanged += OnWidgetVisibilityChanged;
    }

    public void Initialize(XamlRoot xamlRoot)
    {
        _trayIcon = new TaskbarIcon
        {
            ToolTipText = "Stats - System Monitor",
            IconSource = CreateTextIcon("--")
        };

        _trayIcon.LeftClickCommand = new RelayCommand(OnShowDashboard);

        var contextMenu = new MenuFlyout();

        var showItem = new MenuFlyoutItem { Text = "Show Dashboard" };
        showItem.Click += (s, e) => OnShowDashboard();
        contextMenu.Items.Add(showItem);

        contextMenu.Items.Add(new MenuFlyoutSeparator());

        // Widgets submenu
        var widgetsSubmenu = new MenuFlyoutSubItem { Text = "Widgets" };

        foreach (WidgetType widgetType in Enum.GetValues<WidgetType>())
        {
            var menuItem = new ToggleMenuFlyoutItem
            {
                Text = widgetType.ToString(),
                IsChecked = _widgetService?.IsWidgetVisible(widgetType) ?? false
            };
            var type = widgetType; // Capture for closure
            menuItem.Click += (s, e) => OnWidgetToggle(type);
            widgetsSubmenu.Items.Add(menuItem);
            _widgetMenuItems[widgetType] = menuItem;
        }

        contextMenu.Items.Add(widgetsSubmenu);

        contextMenu.Items.Add(new MenuFlyoutSeparator());

        var exitItem = new MenuFlyoutItem { Text = "Exit" };
        exitItem.Click += (s, e) => OnExit();
        contextMenu.Items.Add(exitItem);

        _trayIcon.ContextFlyout = contextMenu;
    }

    private void OnWidgetToggle(WidgetType widgetType)
    {
        _widgetService?.ToggleWidget(widgetType);
    }

    private void OnWidgetVisibilityChanged(object? sender, WidgetType widgetType)
    {
        if (_widgetMenuItems.TryGetValue(widgetType, out var menuItem))
        {
            menuItem.IsChecked = _widgetService?.IsWidgetVisible(widgetType) ?? false;
        }
    }

    private void OnCpuUpdated(object? sender, CpuInfo cpu)
    {
        _lastCpuLoad = cpu.TotalLoad;
        UpdateTrayIcon();
    }

    private void UpdateTrayIcon()
    {
        if (_trayIcon == null) return;

        var loadText = _lastCpuLoad.ToString("F0");
        _trayIcon.ToolTipText = $"Stats - CPU: {loadText}%";

        // Update icon with CPU percentage
        _trayIcon.IconSource = CreateTextIcon(loadText);
    }

    private static GeneratedIconSource CreateTextIcon(string text)
    {
        return new GeneratedIconSource
        {
            Text = text,
            FontSize = 14,
            Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White)
        };
    }

    private void OnShowDashboard()
    {
        ShowDashboardRequested?.Invoke(this, EventArgs.Empty);
    }

    private void OnExit()
    {
        ExitRequested?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _monitor.CpuUpdated -= OnCpuUpdated;
        _trayIcon?.Dispose();
    }

    private class RelayCommand : System.Windows.Input.ICommand
    {
        private readonly Action _execute;

        public RelayCommand(Action execute) => _execute = execute;

#pragma warning disable CS0067
        public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => _execute();
    }
}
