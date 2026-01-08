using Microsoft.UI.Xaml;

namespace Stats.App.Services;

public class ThemeService
{
    public void ApplyTheme(string theme)
    {
        var requestedTheme = theme switch
        {
            "Light" => ElementTheme.Light,
            "Dark" => ElementTheme.Dark,
            _ => ElementTheme.Default // System
        };

        // Apply theme to all windows
        if (App.MainWindow?.Content is FrameworkElement mainContent)
        {
            mainContent.RequestedTheme = requestedTheme;
        }
    }

    public void ApplyThemeToWindow(Window window, string theme)
    {
        var requestedTheme = theme switch
        {
            "Light" => ElementTheme.Light,
            "Dark" => ElementTheme.Dark,
            _ => ElementTheme.Default
        };

        if (window.Content is FrameworkElement content)
        {
            content.RequestedTheme = requestedTheme;
        }
    }

    public static ElementTheme GetElementTheme(string theme)
    {
        return theme switch
        {
            "Light" => ElementTheme.Light,
            "Dark" => ElementTheme.Dark,
            _ => ElementTheme.Default
        };
    }
}
