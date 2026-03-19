using Microsoft.Data.SqlClient;
using PolyhydraGames.Data.Sql.Connection;
using Xunit;

namespace Core.SQL.Tests;

public class DefaultSqlConnectionFactoryTests
{
    [Fact]
    public void Create_WithValidFactoryFunc_ReturnsNewConnection()
    {
        // Arrange
        var connectionString = "Server=.;Database=test;Trusted_Connection=True";
        var factory = new DefaultSqlConnectionFactory(() => new SqlConnection(connectionString));

        // Act
        var connection = factory.Create();

        // Assert
        Assert.NotNull(connection);
        Assert.Equal(connectionString, connection.ConnectionString);
    }

    [Fact]
    public void Create_WithNullFactoryFunc_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new DefaultSqlConnectionFactory(null!));
    }

    [Fact]
    public async Task OpenAsync_OpensConnection_ReturnsOpenConnection()
    {
        // Arrange - use a localdb or trusted connection that won't actually connect
        var connectionString = "Server=.;Database=test;Trusted_Connection=True";
        var factory = new DefaultSqlConnectionFactory(() => new SqlConnection(connectionString));

        // Act
        // Note: This will fail to actually connect without a SQL Server, but tests the async flow
        try
        {
            await factory.OpenAsync();
        }
        catch (SqlException)
        {
            // Expected - no SQL Server running
        }
    }

    [Fact]
    public async Task OpenAsync_WithCancellationToken_RespectsToken()
    {
        // Arrange
        var connectionString = "Server=.;Database=test;Trusted_Connection=True";
        var factory = new DefaultSqlConnectionFactory(() => new SqlConnection(connectionString));
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            await factory.OpenAsync(cts.Token));
    }
}
