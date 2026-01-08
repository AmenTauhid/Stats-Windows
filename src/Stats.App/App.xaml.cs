using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Navigation;
using Stats.App.Services;
using Stats.App.Views;
using Stats.Configuration;
using Stats.Core.Interfaces;
using Stats.Hardware;
using WinRT.Interop;

namespace Stats.App;

public partial class App : Application
{
    private Window? _window;
    private AppWindow? _appWindow;
    private TrayIconService? _trayService;
    private WidgetService? _widgetService;
    private SettingsWindow? _settingsWindow;

    public static IServiceProvider Services { get; private set; } = null!;
    public static Window? MainWindow { get; private set; }

    public App()
    {
        this.InitializeComponent();

        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Hardware monitoring
        services.AddSingleton<IHardwareMonitor, HardwareMonitorService>();

        // Configuration
        services.AddSingleton<ConfigurationService>();

        // Services
        services.AddSingleton<TrayIconService>();
        services.AddSingleton<StartupService>();
        services.AddSingleton<ThemeService>();
        services.AddSingleton<AlertService>();

        // ViewModels
        services.AddSingleton<MainViewModel>();
    }

    protected override async void OnLaunched(LaunchActivatedEventArgs e)
    {
        _window = new Window
        {
            Title = "Stats - System Monitor"
        };
        MainWindow = _window;

        // Get the AppWindow for minimize/show functionality
        var hwnd = WindowNative.GetWindowHandle(_window);
        var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
        _appWindow = AppWindow.GetFromWindowId(windowId);

        var rootFrame = new Frame();
        rootFrame.NavigationFailed += OnNavigationFailed;
        _window.Content = rootFrame;

        _ = rootFrame.Navigate(typeof(MainPage), e.Arguments);
        _window.Activate();

        // Get services
        var configService = Services.GetRequiredService<ConfigurationService>();
        var monitor = Services.GetRequiredService<IHardwareMonitor>();
        var viewModel = Services.GetRequiredService<MainViewModel>();

        // Start hardware monitoring with saved update interval
        monitor.UpdateInterval = TimeSpan.FromMilliseconds(configService.Settings.UpdateIntervalMs);
        await monitor.StartAsync();

        // Initialize widget service
        _widgetService = new WidgetService(viewModel, configService);

        // Initialize tray icon
        _trayService = Services.GetRequiredService<TrayIconService>();
        _trayService.SetWidgetService(_widgetService);
        _trayService.Initialize(_window.Content.XamlRoot);
        _trayService.ShowDashboardRequested += OnShowDashboard;
        _trayService.SettingsRequested += OnShowSettings;
        _trayService.ExitRequested += OnExit;

        // Apply saved theme
        var themeService = Services.GetRequiredService<ThemeService>();
        themeService.ApplyTheme(configService.Settings.Theme);

        // Initialize alert service
        _ = Services.GetRequiredService<AlertService>();

        // Handle window close - minimize to tray instead
        _appWindow.Closing += OnWindowClosing;

        // Check if should start minimized
        if (configService.Settings.StartMinimized)
        {
            _appWindow.Hide();
        }
    }

    private void OnWindowClosing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        // Prevent window from closing, just hide it
        args.Cancel = true;
        _appWindow?.Hide();
    }

    private void OnShowDashboard(object? sender, EventArgs e)
    {
        _appWindow?.Show();
        _window?.Activate();
    }

    private void OnShowSettings(object? sender, EventArgs e)
    {
        if (_settingsWindow != null)
        {
            _settingsWindow.Activate();
            return;
        }

        var configService = Services.GetRequiredService<ConfigurationService>();
        var startupService = Services.GetRequiredService<StartupService>();
        var themeService = Services.GetRequiredService<ThemeService>();
        var monitor = Services.GetRequiredService<IHardwareMonitor>();

        _settingsWindow = new SettingsWindow(configService, startupService, themeService, monitor);
        _settingsWindow.Closed += (s, args) => _settingsWindow = null;
        _settingsWindow.Activate();
    }

    private void OnExit(object? sender, EventArgs e)
    {
        // Actually close the app
        if (_appWindow != null)
        {
            _appWindow.Closing -= OnWindowClosing;
        }

        // Close all widgets and save positions
        _widgetService?.CloseAllWidgets();

        _trayService?.Dispose();

        var monitor = Services.GetRequiredService<IHardwareMonitor>();
        monitor.Dispose();

        Exit();
    }

    void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
    }
}
