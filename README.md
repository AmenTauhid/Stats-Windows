# Stats Windows

An open-source system monitoring application for Windows, built with **WinUI 3** and **.NET 10**.

Stats Windows provides real-time information about your system hardware, including CPU, GPU, Memory, Disk, Network, and more. It is designed with a modern Windows 11-style interface and utilizes the latest Windows App SDK.

## Features

Monitor your system in real-time with widgets for:

-   **CPU**: Usage, temperature, and clock speeds.
-   **GPU**: Usage, VRAM, and temperature.
-   **Memory**: RAM usage and available memory.
-   **Disk**: Read/Write speeds and storage usage.
-   **Network**: Upload/Download speeds.
-   **Battery**: Status, charge level, and health.
-   **Fan**: Fan speeds (RPM).
-   **Bluetooth**: Connected device status.
-   **Sensors**: Various system sensor readings.

## Key Technologies

-   **Framework**: [WinUI 3 (Windows App SDK)](https://learn.microsoft.com/windows/apps/winui/winui3/)
-   **Runtime**: [.NET 10](https://dotnet.microsoft.com/)
-   **Pattern**: MVVM (Model-View-ViewModel) using [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)
-   **System Integration**: [H.NotifyIcon](https://github.com/HavenDV/H.NotifyIcon) for system tray support.

## Project Structure

The solution is organized into modular projects:

-   `src/Stats.App`: The main WinUI 3 application containing Views, ViewModels, and UI logic.
-   `src/Stats.Core`: Shared models (Hardware info definitions) and interfaces.
-   `src/Stats.Configuration`: Settings management and configuration services.
-   `src/Stats.Hardware`: Implementation of hardware monitoring services.

## Getting Started

### Prerequisites

To build and run this project, you need:

1.  **Windows 10 version 1809 (Build 17763)** or later.
2.  **[Visual Studio 2022Preview](https://visualstudio.microsoft.com/vs/preview/)** (required for .NET 10).
3.  **[.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)**.
4.  **Windows App SDK** workload installed in Visual Studio.

### Installation

1.  Clone the repository:
    ```bash
    git clone https://github.com/amentauhid/Stats-Windows.git
    cd Stats-Windows
    ```

2.  Open the solution file `Stats-Windows.sln` in Visual Studio 2022.

3.  Restore dependencies:
    ```bash
    dotnet restore
    ```

4.  Set `Stats.App` as the startup project.

5.  Run the application (F5).

> **Note:** Just like standard WinUI applications, ensure you have Developer Mode enabled in Windows Settings if you encounter deployment issues.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1.  Fork the repository
2.  Create your feature branch (`git checkout -b feature/AmazingFeature`)
3.  Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4.  Push to the branch (`git push origin feature/AmazingFeature`)
5.  Open a Pull Request

## License

[MIT License](LICENSE)
