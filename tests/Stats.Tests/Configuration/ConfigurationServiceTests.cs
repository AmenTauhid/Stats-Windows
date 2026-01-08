using Stats.Configuration;

namespace Stats.Tests.Configuration;

public class ConfigurationServiceTests
{
    [Fact]
    public void Constructor_CreatesDefaultSettings()
    {
        // Arrange & Act
        var service = new ConfigurationService();

        // Assert
        Assert.NotNull(service.Settings);
    }

    [Fact]
    public void GetWidgetPosition_NonExistent_ReturnsNull()
    {
        // Arrange
        var service = new ConfigurationService();

        // Act
        var position = service.GetWidgetPosition("NonExistent");

        // Assert
        Assert.Null(position);
    }

    [Fact]
    public void SetWidgetPosition_ThenGet_ReturnsCorrectPosition()
    {
        // Arrange
        var service = new ConfigurationService();

        // Act
        service.SetWidgetPosition("CPU", 100, 200);
        var position = service.GetWidgetPosition("CPU");

        // Assert
        Assert.NotNull(position);
        Assert.Equal(100, position.Value.X);
        Assert.Equal(200, position.Value.Y);
    }

    [Fact]
    public void IsWidgetEnabled_NonExistent_ReturnsFalse()
    {
        // Arrange
        var service = new ConfigurationService();

        // Act
        var isEnabled = service.IsWidgetEnabled("NonExistent");

        // Assert
        Assert.False(isEnabled);
    }

    [Fact]
    public void SetWidgetEnabled_True_AddsToEnabledWidgets()
    {
        // Arrange
        var service = new ConfigurationService();

        // Act
        service.SetWidgetEnabled("GPU", true);

        // Assert
        Assert.True(service.IsWidgetEnabled("GPU"));
    }

    [Fact]
    public void SetWidgetEnabled_False_RemovesFromEnabledWidgets()
    {
        // Arrange
        var service = new ConfigurationService();
        service.SetWidgetEnabled("GPU", true);

        // Act
        service.SetWidgetEnabled("GPU", false);

        // Assert
        Assert.False(service.IsWidgetEnabled("GPU"));
    }

    [Fact]
    public void GetEnabledWidgets_ReturnsAllEnabled()
    {
        // Arrange
        var service = new ConfigurationService();
        service.SetWidgetEnabled("CPU", true);
        service.SetWidgetEnabled("GPU", true);
        service.SetWidgetEnabled("Memory", true);

        // Act
        var enabled = service.GetEnabledWidgets().ToList();

        // Assert
        Assert.Equal(3, enabled.Count);
        Assert.Contains("CPU", enabled);
        Assert.Contains("GPU", enabled);
        Assert.Contains("Memory", enabled);
    }

    [Fact]
    public void Save_DoesNotThrow()
    {
        // Arrange
        var service = new ConfigurationService();
        service.Settings.Theme = "Dark";

        // Act & Assert (should not throw)
        var exception = Record.Exception(() => service.Save());
        Assert.Null(exception);
    }
}
