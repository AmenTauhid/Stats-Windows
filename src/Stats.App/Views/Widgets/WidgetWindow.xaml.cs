using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Stats.App.Helpers;
using Windows.Graphics;

namespace Stats.App.Views.Widgets;

public enum WidgetType
{
    Cpu,
    Gpu,
    Memory,
    Network,
    Disk,
    Battery
}

public sealed partial class WidgetWindow : Window
{
    private bool _isDragging;
    private PointInt32 _dragStartPosition;
    private Windows.Foundation.Point _dragStartPointer;

    public WidgetType WidgetType { get; }
    public event EventHandler? WidgetClosed;

    public WidgetWindow(WidgetType widgetType, UIElement content, string title)
    {
        InitializeComponent();

        WidgetType = widgetType;
        TitleText.Text = title;
        WidgetContent.Content = content;

        // Configure window
        ConfigureWindow();

        // Setup drag behavior
        HeaderGrid.PointerPressed += HeaderGrid_PointerPressed;
        HeaderGrid.PointerMoved += HeaderGrid_PointerMoved;
        HeaderGrid.PointerReleased += HeaderGrid_PointerReleased;
    }

    private void ConfigureWindow()
    {
        var appWindow = WindowHelper.GetAppWindow(this);

        // Remove title bar
        WindowHelper.RemoveTitleBar(this);

        // Set size based on widget type
        var (width, height) = GetWidgetSize();
        WindowHelper.SetWindowSize(this, width, height);

        // Make always on top
        WindowHelper.SetAlwaysOnTop(this, true);

        // Make semi-transparent
        WindowHelper.MakeWindowTransparent(this, 230);

        // Set window to not show in taskbar
        appWindow.IsShownInSwitchers = false;
    }

    private (int Width, int Height) GetWidgetSize()
    {
        return WidgetType switch
        {
            WidgetType.Cpu => (200, 120),
            WidgetType.Gpu => (200, 120),
            WidgetType.Memory => (200, 100),
            WidgetType.Network => (220, 100),
            WidgetType.Disk => (200, 100),
            WidgetType.Battery => (180, 100),
            _ => (200, 120)
        };
    }

    public void SetPosition(int x, int y)
    {
        WindowHelper.SetWindowPosition(this, x, y);
    }

    public (int X, int Y) GetPosition()
    {
        return WindowHelper.GetWindowPosition(this);
    }

    private void HeaderGrid_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        _isDragging = true;
        _dragStartPosition = WindowHelper.GetAppWindow(this).Position;
        _dragStartPointer = e.GetCurrentPoint(HeaderGrid).Position;
        HeaderGrid.CapturePointer(e.Pointer);
    }

    private void HeaderGrid_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (!_isDragging) return;

        var currentPoint = e.GetCurrentPoint(HeaderGrid).Position;
        var deltaX = (int)(currentPoint.X - _dragStartPointer.X);
        var deltaY = (int)(currentPoint.Y - _dragStartPointer.Y);

        WindowHelper.SetWindowPosition(
            this,
            _dragStartPosition.X + deltaX,
            _dragStartPosition.Y + deltaY);
    }

    private void HeaderGrid_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        _isDragging = false;
        HeaderGrid.ReleasePointerCapture(e.Pointer);
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        WidgetClosed?.Invoke(this, EventArgs.Empty);
        Close();
    }
}
