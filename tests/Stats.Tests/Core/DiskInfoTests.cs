using Stats.Core.Models;

namespace Stats.Tests.Core;

public class DiskInfoTests
{
    [Fact]
    public void UsagePercentage_WithValidTotal_CalculatesCorrectly()
    {
        // Arrange
        var disk = new DiskInfo
        {
            Name = "Samsung SSD",
            DriveLetter = "C:",
            UsedSpace = 250_000_000_000L,
            TotalSpace = 500_000_000_000L
        };

        // Act
        var percentage = disk.UsagePercentage;

        // Assert
        Assert.Equal(50f, percentage);
    }

    [Fact]
    public void UsagePercentage_WithZeroTotal_ReturnsZero()
    {
        // Arrange
        var disk = new DiskInfo
        {
            UsedSpace = 0,
            TotalSpace = 0
        };

        // Act
        var percentage = disk.UsagePercentage;

        // Assert
        Assert.Equal(0f, percentage);
    }

    [Fact]
    public void UsagePercentage_FullDisk_ReturnsHundred()
    {
        // Arrange
        var disk = new DiskInfo
        {
            UsedSpace = 500_000_000_000L,
            TotalSpace = 500_000_000_000L
        };

        // Act
        var percentage = disk.UsagePercentage;

        // Assert
        Assert.Equal(100f, percentage);
    }

    [Fact]
    public void DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var disk = new DiskInfo();

        // Assert
        Assert.Equal(string.Empty, disk.Name);
        Assert.Equal(string.Empty, disk.DriveLetter);
        Assert.Equal(0, disk.ReadRate);
        Assert.Equal(0, disk.WriteRate);
        Assert.Equal(0, disk.Temperature);
    }
}
