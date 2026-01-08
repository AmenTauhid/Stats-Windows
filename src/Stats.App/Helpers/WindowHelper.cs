using System.Runtime.InteropServices;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinRT.Interop;

namespace Stats.App.Helpers;

public static class WindowHelper
{
    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_LAYERED = 0x00080000;
    private const int WS_EX_TRANSPARENT = 0x00000020;
    private const int LWA_ALPHA = 0x00000002;

    private static readonly IntPtr HWND_TOPMOST = new(-1);
    private static readonly IntPtr HWND_NOTOPMOST = new(-2);

    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOACTIVATE = 0x0010;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern nint GetWindowLongPtr(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern nint SetWindowLongPtr(IntPtr hWnd, int nIndex, nint dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    public static void MakeWindowTransparent(Window window, byte opacity = 255)
    {
        var hwnd = WindowNative.GetWindowHandle(window);

        // Get current extended style
        var exStyle = GetWindowLongPtr(hwnd, GWL_EXSTYLE);

        // Add layered window style
        exStyle |= WS_EX_LAYERED;

        // Set the new extended style
        SetWindowLongPtr(hwnd, GWL_EXSTYLE, exStyle);

        // Set the opacity (255 = fully opaque, 0 = fully transparent)
        SetLayeredWindowAttributes(hwnd, 0, opacity, LWA_ALPHA);
    }

    public static void MakeWindowClickThrough(Window window)
    {
        var hwnd = WindowNative.GetWindowHandle(window);

        var exStyle = GetWindowLongPtr(hwnd, GWL_EXSTYLE);
        exStyle |= WS_EX_LAYERED | WS_EX_TRANSPARENT;

        SetWindowLongPtr(hwnd, GWL_EXSTYLE, exStyle);
    }

    public static void RemoveClickThrough(Window window)
    {
        var hwnd = WindowNative.GetWindowHandle(window);

        var exStyle = GetWindowLongPtr(hwnd, GWL_EXSTYLE);
        exStyle &= ~WS_EX_TRANSPARENT;

        SetWindowLongPtr(hwnd, GWL_EXSTYLE, exStyle);
    }

    public static void SetAlwaysOnTop(Window window, bool alwaysOnTop)
    {
        var hwnd = WindowNative.GetWindowHandle(window);

        var insertAfter = alwaysOnTop ? HWND_TOPMOST : HWND_NOTOPMOST;

        SetWindowPos(hwnd, insertAfter, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
    }

    public static void RemoveTitleBar(Window window)
    {
        var appWindow = GetAppWindow(window);
        if (appWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.SetBorderAndTitleBar(false, false);
        }
    }

    public static void SetWindowSize(Window window, int width, int height)
    {
        var appWindow = GetAppWindow(window);
        appWindow.Resize(new Windows.Graphics.SizeInt32(width, height));
    }

    public static void SetWindowPosition(Window window, int x, int y)
    {
        var appWindow = GetAppWindow(window);
        appWindow.Move(new Windows.Graphics.PointInt32(x, y));
    }

    public static (int X, int Y) GetWindowPosition(Window window)
    {
        var appWindow = GetAppWindow(window);
        var position = appWindow.Position;
        return (position.X, position.Y);
    }

    public static AppWindow GetAppWindow(Window window)
    {
        var hwnd = WindowNative.GetWindowHandle(window);
        var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
        return AppWindow.GetFromWindowId(windowId);
    }
}
