using Stats.Core.Models;

namespace Stats.Tests.Core;

public class MemoryInfoTests
{
    [Fact]
    public void UsedPercentage_WithValidTotal_CalculatesCorrectly()
    {
        // Arrange
        var memory = new MemoryInfo
        {
            Used = 8_000_000_000L,
            Total = 16_000_000_000L,
            Available = 8_000_000_000L
        };

        // Act
        var percentage = memory.UsedPercentage;

        // Assert
        Assert.Equal(50f, percentage);
    }

    [Fact]
    public void UsedPercentage_WithZeroTotal_ReturnsZero()
    {
        // Arrange
        var memory = new MemoryInfo
        {
            Used = 0,
            Total = 0,
            Available = 0
        };

        // Act
        var percentage = memory.UsedPercentage;

        // Assert
        Assert.Equal(0f, percentage);
    }

    [Fact]
    public void UsedPercentage_FullMemory_ReturnsHundred()
    {
        // Arrange
        var memory = new MemoryInfo
        {
            Used = 16_000_000_000L,
            Total = 16_000_000_000L,
            Available = 0
        };

        // Act
        var percentage = memory.UsedPercentage;

        // Assert
        Assert.Equal(100f, percentage);
    }

    [Fact]
    public void Timestamp_DefaultsToUtcNow()
    {
        // Arrange & Act
        var before = DateTime.UtcNow;
        var memory = new MemoryInfo();
        var after = DateTime.UtcNow;

        // Assert
        Assert.InRange(memory.Timestamp, before, after);
    }

    [Fact]
    public void Record_Equality_WorksCorrectly()
    {
        // Arrange
        var timestamp = DateTime.UtcNow;
        var memory1 = new MemoryInfo
        {
            Used = 8_000_000_000L,
            Total = 16_000_000_000L,
            Available = 8_000_000_000L,
            Timestamp = timestamp
        };
        var memory2 = new MemoryInfo
        {
            Used = 8_000_000_000L,
            Total = 16_000_000_000L,
            Available = 8_000_000_000L,
            Timestamp = timestamp
        };

        // Assert
        Assert.Equal(memory1, memory2);
    }
}
