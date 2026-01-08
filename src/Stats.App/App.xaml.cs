using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Navigation;
using Stats.App.Services;
using Stats.Core.Interfaces;
using Stats.Hardware;
using WinRT.Interop;

namespace Stats.App;

public partial class App : Application
{
    private Window? _window;
    private AppWindow? _appWindow;
    private TrayIconService? _trayService;

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

        // Services
        services.AddSingleton<TrayIconService>();

        // ViewModels
        services.AddTransient<MainViewModel>();
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

        // Start hardware monitoring
        var monitor = Services.GetRequiredService<IHardwareMonitor>();
        await monitor.StartAsync();

        // Initialize tray icon
        _trayService = Services.GetRequiredService<TrayIconService>();
        _trayService.Initialize(_window.Content.XamlRoot);
        _trayService.ShowDashboardRequested += OnShowDashboard;
        _trayService.ExitRequested += OnExit;

        // Handle window close - minimize to tray instead
        _appWindow.Closing += OnWindowClosing;
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

    private void OnExit(object? sender, EventArgs e)
    {
        // Actually close the app
        if (_appWindow != null)
        {
            _appWindow.Closing -= OnWindowClosing;
        }

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
