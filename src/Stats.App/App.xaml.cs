using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Navigation;
using Stats.Core.Interfaces;
using Stats.Hardware;

namespace Stats.App;

public partial class App : Application
{
    private Window? _window;

    public static IServiceProvider Services { get; private set; } = null!;

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

        // ViewModels
        services.AddTransient<MainViewModel>();
    }

    protected override async void OnLaunched(LaunchActivatedEventArgs e)
    {
        _window ??= new Window();

        if (_window.Content is not Frame rootFrame)
        {
            rootFrame = new Frame();
            rootFrame.NavigationFailed += OnNavigationFailed;
            _window.Content = rootFrame;
        }

        _ = rootFrame.Navigate(typeof(MainPage), e.Arguments);
        _window.Activate();

        // Start hardware monitoring
        var monitor = Services.GetRequiredService<IHardwareMonitor>();
        await monitor.StartAsync();
    }

    void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
    }
}
