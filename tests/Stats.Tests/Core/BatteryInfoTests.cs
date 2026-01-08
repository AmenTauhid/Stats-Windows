using Stats.Core.Models;

namespace Stats.Tests.Core;

public class BatteryInfoTests
{
    [Fact]
    public void HealthPercentage_WithValidCapacity_CalculatesCorrectly()
    {
        // Arrange
        var battery = new BatteryInfo
        {
            IsPresent = true,
            DesignCapacity = 50000,
            FullChargeCapacity = 45000
        };

        // Act
        var health = battery.HealthPercentage;

        // Assert
        Assert.Equal(90f, health);
    }

    [Fact]
    public void HealthPercentage_WithZeroDesign_ReturnsHundred()
    {
        // Arrange
        var battery = new BatteryInfo
        {
            DesignCapacity = 0,
            FullChargeCapacity = 45000
        };

        // Act
        var health = battery.HealthPercentage;

        // Assert
        Assert.Equal(100f, health);
    }

    [Fact]
    public void HealthPercentage_PerfectBattery_ReturnsHundred()
    {
        // Arrange
        var battery = new BatteryInfo
        {
            DesignCapacity = 50000,
            FullChargeCapacity = 50000
        };

        // Act
        var health = battery.HealthPercentage;

        // Assert
        Assert.Equal(100f, health);
    }

    [Fact]
    public void HealthPercentage_DegradedBattery_CalculatesCorrectly()
    {
        // Arrange - battery at 80% health
        var battery = new BatteryInfo
        {
            DesignCapacity = 100000,
            FullChargeCapacity = 80000
        };

        // Act
        var health = battery.HealthPercentage;

        // Assert
        Assert.Equal(80f, health);
    }

    [Theory]
    [InlineData(BatteryStatus.NotPresent)]
    [InlineData(BatteryStatus.Discharging)]
    [InlineData(BatteryStatus.Idle)]
    [InlineData(BatteryStatus.Charging)]
    public void BatteryStatus_AllValuesExist(BatteryStatus status)
    {
        // Arrange & Act
        var battery = new BatteryInfo { Status = status };

        // Assert
        Assert.Equal(status, battery.Status);
    }

    [Fact]
    public void TimeRemaining_CanBeNull()
    {
        // Arrange & Act
        var battery = new BatteryInfo { TimeRemaining = null };

        // Assert
        Assert.Null(battery.TimeRemaining);
    }

    [Fact]
    public void TimeRemaining_CanHaveValue()
    {
        // Arrange
        var timeRemaining = TimeSpan.FromHours(3.5);

        // Act
        var battery = new BatteryInfo { TimeRemaining = timeRemaining };

        // Assert
        Assert.Equal(timeRemaining, battery.TimeRemaining);
    }
}
