using Stats.Core.Models;

namespace Stats.Tests.Core;

public class CpuInfoTests
{
    [Fact]
    public void DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var cpu = new CpuInfo();

        // Assert
        Assert.Equal(string.Empty, cpu.Name);
        Assert.Equal(0, cpu.TotalLoad);
        Assert.Equal(0, cpu.PackageTemperature);
        Assert.Equal(0, cpu.PackagePower);
        Assert.Empty(cpu.Cores);
    }

    [Fact]
    public void Cores_CanBeInitialized()
    {
        // Arrange
        var cores = new List<CoreInfo>
        {
            new() { CoreId = 0, Load = 50, Temperature = 65, Clock = 3600 },
            new() { CoreId = 1, Load = 75, Temperature = 68, Clock = 3800 }
        };

        // Act
        var cpu = new CpuInfo
        {
            Name = "AMD Ryzen 9 5900X",
            Cores = cores
        };

        // Assert
        Assert.Equal(2, cpu.Cores.Count);
        Assert.Equal(0, cpu.Cores[0].CoreId);
        Assert.Equal(50, cpu.Cores[0].Load);
    }

    [Fact]
    public void Record_WithExpression_CreatesNewInstance()
    {
        // Arrange
        var original = new CpuInfo
        {
            Name = "Intel Core i9",
            TotalLoad = 50
        };

        // Act
        var modified = original with { TotalLoad = 75 };

        // Assert
        Assert.NotSame(original, modified);
        Assert.Equal("Intel Core i9", modified.Name);
        Assert.Equal(50, original.TotalLoad);
        Assert.Equal(75, modified.TotalLoad);
    }
}

public class CoreInfoTests
{
    [Fact]
    public void DefaultValues_AreZero()
    {
        // Arrange & Act
        var core = new CoreInfo();

        // Assert
        Assert.Equal(0, core.CoreId);
        Assert.Equal(0, core.Load);
        Assert.Equal(0, core.Temperature);
        Assert.Equal(0, core.Clock);
    }

    [Fact]
    public void CanInitialize_WithValues()
    {
        // Arrange & Act
        var core = new CoreInfo
        {
            CoreId = 5,
            Load = 95.5f,
            Temperature = 82.3f,
            Clock = 4500
        };

        // Assert
        Assert.Equal(5, core.CoreId);
        Assert.Equal(95.5f, core.Load);
        Assert.Equal(82.3f, core.Temperature);
        Assert.Equal(4500, core.Clock);
    }
}
