# Stats Windows

An open-source system monitoring application for Windows, inspired by [macOS Stats](https://github.com/exelban/stats). Built with **WinUI 3** and **.NET 10**.

Stats Windows provides real-time information about your system hardware, including CPU, GPU, Memory, Disk, Network, Battery, Fans, and Sensors. It features a modern Windows 11-style interface with system tray integration and desktop widgets.

## Features

### Hardware Monitoring
- **CPU**: Total/per-core load, temperature, clock speeds, power consumption
- **GPU**: Load, temperature, memory usage, clock speeds (NVIDIA/AMD/Intel)
- **Memory**: Used/available/total RAM with percentage
- **Disk**: Read/write speeds, storage usage, temperature
- **Network**: Upload/download speeds per adapter
- **Battery**: Charge level, health percentage, time remaining
- **Fans**: Fan speeds (RPM) from motherboard/GPU
- **Sensors**: All available temperature, voltage, and power readings

### UI Components
- **Dashboard**: Main window with comprehensive hardware overview
- **System Tray**: Minimizes to tray with dynamic CPU percentage icon
- **Desktop Widgets**: Transparent, always-on-top overlays for CPU, GPU, Memory, Network
- **Settings**: Configure themes, update intervals, alerts, and module toggles

### Additional Features
- Temperature alerts with Windows notifications
- Start with Windows option
- Light/Dark/System theme support
- Configurable update intervals (500ms - 5s)
- Widget position persistence

## Screenshots

*Coming soon*

## Key Technologies

| Technology | Purpose |
|------------|---------|
| [WinUI 3](https://learn.microsoft.com/windows/apps/winui/winui3/) | Modern Windows UI framework |
| [.NET 10](https://dotnet.microsoft.com/) | Runtime and SDK |
| [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor) | Hardware sensor access |
| [H.NotifyIcon.WinUI](https://github.com/HavenDV/H.NotifyIcon) | System tray support |
| [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/) | MVVM pattern implementation |

## Project Structure

```
Stats-Windows/
├── src/
│   ├── Stats.App/                    # WinUI 3 Application
│   │   ├── Views/                    # XAML pages and windows
│   │   ├── ViewModels/               # MVVM ViewModels
│   │   ├── Services/                 # App services (Tray, Widget, Alert, etc.)
│   │   ├── Helpers/                  # Win32 interop helpers
│   │   └── Assets/                   # App icons and images
│   │
│   ├── Stats.Core/                   # Core Library
│   │   ├── Models/                   # Data models (CpuInfo, GpuInfo, etc.)
│   │   └── Interfaces/               # Service contracts
│   │
│   ├── Stats.Hardware/               # Hardware Monitoring
│   │   └── HardwareMonitorService.cs # LibreHardwareMonitor wrapper
│   │
│   └── Stats.Configuration/          # Settings & Persistence
│       ├── AppSettings.cs            # Settings model
│       └── ConfigurationService.cs   # JSON persistence
│
└── tests/
    └── Stats.Tests/                  # Unit tests
```

## Getting Started

### Prerequisites

1. **Windows 10 version 1809 (Build 17763)** or later
2. **[Visual Studio 2022 Preview](https://visualstudio.microsoft.com/vs/preview/)** with:
   - .NET Desktop Development workload
   - Windows App SDK C# Templates
3. **[.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)**

### Building from Source

1. Clone the repository:
   ```bash
   git clone https://github.com/amentauhid/Stats-Windows.git
   cd Stats-Windows
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the solution:
   ```bash
   # Debug build
   dotnet build

   # Release build
   dotnet build --configuration Release
   ```

4. Run the application:
   ```bash
   dotnet run --project src/Stats.App/Stats.App.csproj
   ```

### Running Tests

```bash
dotnet test
```

### Publishing

#### Unpackaged (Standalone EXE)
```bash
dotnet publish src/Stats.App/Stats.App.csproj -c Release -r win-x64 --self-contained
```

The output will be in `src/Stats.App/bin/Release/net10.0-windows10.0.22621.0/win-x64/publish/`

#### MSIX Package
Change `WindowsPackageType` from `None` to `MSIX` in `Stats.App.csproj`, then build in Visual Studio for MSIX packaging.

## Configuration

Settings are stored in `%LocalAppData%\Stats\settings.json`:

```json
{
  "StartWithWindows": false,
  "StartMinimized": false,
  "UpdateIntervalMs": 1000,
  "Theme": "System",
  "EnableTemperatureAlerts": true,
  "CpuTempThreshold": 85,
  "GpuTempThreshold": 85
}
```

## Architecture

### Data Flow
```
LibreHardwareMonitor → HardwareMonitorService → Events → MainViewModel → UI
                                                      ↘
                                                       AlertService → Notifications
```

### Key Classes
- **HardwareMonitorService**: Wraps LibreHardwareMonitor, fires typed events
- **MainViewModel**: Subscribes to hardware events, exposes bindable properties
- **TrayIconService**: Manages system tray icon and context menu
- **WidgetService**: Creates and manages transparent widget windows
- **AlertService**: Monitors temperatures and shows notifications

## Admin Rights

Some hardware sensors (especially CPU temperature and power) require administrator privileges. The app includes an `app.manifest` requesting elevation.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## Acknowledgments

- [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor) for hardware monitoring
- [macOS Stats](https://github.com/exelban/stats) for inspiration
- [H.NotifyIcon](https://github.com/HavenDV/H.NotifyIcon) for system tray support

## License

[MIT License](LICENSE)
