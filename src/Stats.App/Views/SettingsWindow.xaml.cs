using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Stats.App.Helpers;
using Stats.App.Services;
using Stats.Configuration;
using Stats.Core.Interfaces;

namespace Stats.App.Views;

public sealed partial class SettingsWindow : Window
{
    private readonly ConfigurationService _configService;
    private readonly StartupService _startupService;
    private readonly ThemeService _themeService;
    private readonly IHardwareMonitor _monitor;
    private bool _isInitializing = true;

    public SettingsWindow(
        ConfigurationService configService,
        StartupService startupService,
        ThemeService themeService,
        IHardwareMonitor monitor)
    {
        InitializeComponent();

        _configService = configService;
        _startupService = startupService;
        _themeService = themeService;
        _monitor = monitor;

        // Configure window
        var appWindow = WindowHelper.GetAppWindow(this);
        appWindow.Resize(new Windows.Graphics.SizeInt32(500, 650));
        appWindow.Title = "Settings - Stats";

        LoadSettings();
        _isInitializing = false;
    }

    private void LoadSettings()
    {
        var settings = _configService.Settings;

        // General
        StartWithWindowsToggle.IsOn = _startupService.IsStartupEnabled();
        StartMinimizedToggle.IsOn = settings.StartMinimized;

        // Theme
        var themeIndex = settings.Theme switch
        {
            "Light" => 1,
            "Dark" => 2,
            _ => 0 // System
        };
        ThemeComboBox.SelectedIndex = themeIndex;

        // Update Interval
        var intervalIndex = settings.UpdateIntervalMs switch
        {
            500 => 0,
            1000 => 1,
            2000 => 2,
            5000 => 3,
            _ => 1 // Default 1 second
        };
        UpdateIntervalComboBox.SelectedIndex = intervalIndex;
    }

    private void StartWithWindowsToggle_Toggled(object sender, RoutedEventArgs e)
    {
        if (_isInitializing) return;

        if (StartWithWindowsToggle.IsOn)
        {
            _startupService.EnableStartup();
        }
        else
        {
            _startupService.DisableStartup();
        }
    }

    private void StartMinimizedToggle_Toggled(object sender, RoutedEventArgs e)
    {
        if (_isInitializing) return;

        _configService.Settings.StartMinimized = StartMinimizedToggle.IsOn;
        _configService.Save();
    }

    private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isInitializing) return;

        if (ThemeComboBox.SelectedItem is ComboBoxItem item && item.Tag is string theme)
        {
            _configService.Settings.Theme = theme;
            _configService.Save();
            _themeService.ApplyTheme(theme);
        }
    }

    private void UpdateIntervalComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isInitializing) return;

        if (UpdateIntervalComboBox.SelectedItem is ComboBoxItem item && item.Tag is string intervalStr)
        {
            if (int.TryParse(intervalStr, out var interval))
            {
                _configService.Settings.UpdateIntervalMs = interval;
                _configService.Save();
                _monitor.SetUpdateInterval(TimeSpan.FromMilliseconds(interval));
            }
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
