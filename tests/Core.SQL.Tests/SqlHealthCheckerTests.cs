using Microsoft.Data.SqlClient;
using PolyhydraGames.Data.Sql.Connection;
using PolyhydraGames.Data.Sql.Health;
using Xunit;

namespace Core.SQL.Tests;

public class SqlHealthCheckerTests
{
    // Fake factory that succeeds
    private class SuccessfulSqlConnectionFactory : ISqlConnectionFactory
    {
        public SqlConnection Create() => new SqlConnection("Server=.;Database=master");
        
        public Task<SqlConnection> OpenAsync(CancellationToken ct = default)
        {
            var connection = new SqlConnection("Server=.;Database=master");
            return Task.FromResult(connection);
        }
    }

    // Fake factory that throws
    private class FailingSqlConnectionFactory : ISqlConnectionFactory
    {
        public SqlConnection Create() => throw new Exception("Connection failed");
        
        public Task<SqlConnection> OpenAsync(CancellationToken ct = default)
        {
            throw new Exception("Connection failed");
        }
    }

    [Fact]
    public async Task CheckAsync_WithSuccessfulConnection_ReturnsHealthyStatus()
    {
        // Arrange
        var factory = new SuccessfulSqlConnectionFactory();

        // Act
        var result = await SqlHealthChecker.CheckAsync(factory);

        // Assert
        Assert.True(result.CanConnect);
        Assert.Null(result.Error);
        Assert.True(result.Latency > TimeSpan.Zero);
    }

    [Fact]
    public async Task CheckAsync_WithFailingConnection_ReturnsUnhealthyStatus()
    {
        // Arrange
        var factory = new FailingSqlConnectionFactory();

        // Act
        var result = await SqlHealthChecker.CheckAsync(factory);

        // Assert
        Assert.False(result.CanConnect);
        Assert.NotNull(result.Error);
        Assert.Contains("Connection failed", result.Error);
        Assert.True(result.Latency > TimeSpan.Zero);
    }

    [Fact]
    public async Task CheckAsync_WithCancellationToken_CanBeCancelled()
    {
        // Arrange
        var factory = new SuccessfulSqlConnectionFactory();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert - should not throw, just return
        var result = await SqlHealthChecker.CheckAsync(factory, cts.Token);
        
        // When cancelled before starting, it may still return but timing is immediate
        Assert.NotNull(result);
    }
}
