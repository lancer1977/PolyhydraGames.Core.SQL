using PolyhydraGames.Data.Sql.Execution;
using Xunit;

namespace Core.SQL.Tests;

public class SqlExecutionOptionsTests
{
    [Fact]
    public void DelayForAttempt_FirstAttempt_ReturnsBaseDelay()
    {
        // Arrange
        var options = new SqlExecutionOptions(BaseDelay: TimeSpan.FromMilliseconds(100));

        // Act
        var result = options.DelayForAttempt(1);

        // Assert
        Assert.Equal(TimeSpan.FromMilliseconds(100), result);
    }

    [Fact]
    public void DelayForAttempt_SecondAttempt_ReturnsDoubleBaseDelay()
    {
        // Arrange
        var options = new SqlExecutionOptions(BaseDelay: TimeSpan.FromMilliseconds(100));

        // Act
        var result = options.DelayForAttempt(2);

        // Assert
        Assert.Equal(TimeSpan.FromMilliseconds(200), result);
    }

    [Fact]
    public void DelayForAttempt_ThirdAttempt_ReturnsQuadrupleBaseDelay()
    {
        // Arrange
        var options = new SqlExecutionOptions(BaseDelay: TimeSpan.FromMilliseconds(100));

        // Act
        var result = options.DelayForAttempt(3);

        // Assert
        Assert.Equal(TimeSpan.FromMilliseconds(400), result);
    }

    [Fact]
    public void DelayForAttempt_FirstAttemptWithDefaultBaseDelay_Returns100ms()
    {
        // Arrange - default BaseDelay is null, should use 100ms
        var options = new SqlExecutionOptions();

        // Act
        var result = options.DelayForAttempt(1);

        // Assert
        Assert.Equal(TimeSpan.FromMilliseconds(100), result);
    }

    [Fact]
    public void DelayForAttempt_ZeroAttempt_ReturnsBaseDelay()
    {
        // Arrange
        var options = new SqlExecutionOptions(BaseDelay: TimeSpan.FromMilliseconds(100));

        // Act
        var result = options.DelayForAttempt(0);

        // Assert - attempt 0 uses factor 2^0 = 1
        Assert.Equal(TimeSpan.FromMilliseconds(100), result);
    }

    [Fact]
    public void DelayForAttempt_NegativeAttempt_ReturnsBaseDelay()
    {
        // Arrange
        var options = new SqlExecutionOptions(BaseDelay: TimeSpan.FromMilliseconds(100));

        // Act
        var result = options.DelayForAttempt(-1);

        // Assert - negative attempts use factor 2^0 = 1
        Assert.Equal(TimeSpan.FromMilliseconds(100), result);
    }

    [Fact]
    public void DelayForAttempt_LargeAttempt_ExponentialBackoff()
    {
        // Arrange
        var options = new SqlExecutionOptions(BaseDelay: TimeSpan.FromMilliseconds(50));

        // Act & Assert
        Assert.Equal(TimeSpan.FromMilliseconds(50), options.DelayForAttempt(1));    // 50 * 2^0
        Assert.Equal(TimeSpan.FromMilliseconds(100), options.DelayForAttempt(2));  // 50 * 2^1
        Assert.Equal(TimeSpan.FromMilliseconds(200), options.DelayForAttempt(3));  // 50 * 2^2
        Assert.Equal(TimeSpan.FromMilliseconds(400), options.DelayForAttempt(4));  // 50 * 2^3
        Assert.Equal(TimeSpan.FromMilliseconds(800), options.DelayForAttempt(5));  // 50 * 2^4
    }
}
