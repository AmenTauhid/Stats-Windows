using Stats.Configuration;

namespace Stats.Tests.Configuration;

public class AppSettingsTests
{
    [Fact]
    public void DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var settings = new AppSettings();

        // Assert
        Assert.False(settings.StartWithWindows);
        Assert.False(settings.StartMinimized);
        Assert.Equal(1000, settings.UpdateIntervalMs);
        Assert.Equal("System", settings.Theme);
        Assert.Empty(settings.WidgetPositions);
        Assert.Empty(settings.EnabledWidgets);
    }

    [Fact]
    public void AlertSettings_DefaultValues()
    {
        // Arrange & Act
        var settings = new AppSettings();

        // Assert
        Assert.True(settings.EnableTemperatureAlerts);
        Assert.Equal(85, settings.CpuTempThreshold);
        Assert.Equal(85, settings.GpuTempThreshold);
        Assert.Equal(90, settings.GeneralTempThreshold);
    }

    [Fact]
    public void ModuleSettings_DefaultEnabled()
    {
        // Arrange & Act
        var settings = new AppSettings();

        // Assert
        Assert.True(settings.EnableCpuMonitoring);
        Assert.True(settings.EnableGpuMonitoring);
        Assert.True(settings.EnableMemoryMonitoring);
        Assert.True(settings.EnableDiskMonitoring);
        Assert.True(settings.EnableNetworkMonitoring);
        Assert.True(settings.EnableBatteryMonitoring);
        Assert.True(settings.EnableFanMonitoring);
        Assert.True(settings.EnableSensorMonitoring);
    }

    [Fact]
    public void WidgetPositions_CanBeModified()
    {
        // Arrange
        var settings = new AppSettings();

        // Act
        settings.WidgetPositions["CPU"] = new WidgetPosition { X = 100, Y = 200 };

        // Assert
        Assert.Single(settings.WidgetPositions);
        Assert.Equal(100, settings.WidgetPositions["CPU"].X);
        Assert.Equal(200, settings.WidgetPositions["CPU"].Y);
    }

    [Fact]
    public void EnabledWidgets_CanBeModified()
    {
        // Arrange
        var settings = new AppSettings();

        // Act
        settings.EnabledWidgets.Add("CPU");
        settings.EnabledWidgets.Add("GPU");

        // Assert
        Assert.Equal(2, settings.EnabledWidgets.Count);
        Assert.Contains("CPU", settings.EnabledWidgets);
        Assert.Contains("GPU", settings.EnabledWidgets);
    }

    [Theory]
    [InlineData("Light")]
    [InlineData("Dark")]
    [InlineData("System")]
    public void Theme_CanBeSet(string theme)
    {
        // Arrange
        var settings = new AppSettings();

        // Act
        settings.Theme = theme;

        // Assert
        Assert.Equal(theme, settings.Theme);
    }

    [Theory]
    [InlineData(500)]
    [InlineData(1000)]
    [InlineData(2000)]
    [InlineData(5000)]
    public void UpdateInterval_CanBeSet(int interval)
    {
        // Arrange
        var settings = new AppSettings();

        // Act
        settings.UpdateIntervalMs = interval;

        // Assert
        Assert.Equal(interval, settings.UpdateIntervalMs);
    }
}

public class WidgetPositionTests
{
    [Fact]
    public void DefaultValues_AreZero()
    {
        // Arrange & Act
        var position = new WidgetPosition();

        // Assert
        Assert.Equal(0, position.X);
        Assert.Equal(0, position.Y);
    }

    [Fact]
    public void CanSet_Coordinates()
    {
        // Arrange & Act
        var position = new WidgetPosition { X = 1920, Y = 1080 };

        // Assert
        Assert.Equal(1920, position.X);
        Assert.Equal(1080, position.Y);
    }
}
