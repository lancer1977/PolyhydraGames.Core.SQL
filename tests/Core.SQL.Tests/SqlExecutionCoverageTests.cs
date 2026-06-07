using Microsoft.Data.SqlClient;
using PolyhydraGames.Data.Sql.Connection;
using PolyhydraGames.Data.Sql.Execution;
using PolyhydraGames.Data.Sql.Readers;
using PolyhydraGames.Data.Sql.Schema;
using System.Data;
using Xunit;

namespace Core.SQL.Tests;

public class SqlExecutionCoverageTests
{
    [Fact]
    public async Task ExecuteAsync_ReturnsActionResultAndOpensConnectionOnce()
    {
        var factory = new DelegateSqlConnectionFactory();

        var result = await SqlExecution.ExecuteAsync(factory, conn =>
        {
            Assert.NotNull(conn);
            return Task.FromResult("ok");
        }, new SqlExecutionOptions { MaxAttempts = 1, SlowQueryWarnMs = int.MaxValue });

        Assert.Equal("ok", result);
        Assert.Equal(1, factory.OpenCount);
    }

    [Fact]
    public async Task ExecuteAsync_VoidOverloadRunsAction()
    {
        var factory = new DelegateSqlConnectionFactory();
        var ran = false;

        await SqlExecution.ExecuteAsync(factory, conn =>
        {
            Assert.NotNull(conn);
            ran = true;
            return Task.CompletedTask;
        }, new SqlExecutionOptions { MaxAttempts = 1 });

        Assert.True(ran);
        Assert.Equal(1, factory.OpenCount);
    }

    [Fact]
    public async Task ExecuteAsync_RetriesGeneralExceptionsUntilSuccess()
    {
        var factory = new DelegateSqlConnectionFactory();
        var attempts = 0;

        var result = await SqlExecution.ExecuteAsync(factory, _ =>
        {
            attempts++;
            if (attempts < 3)
            {
                throw new InvalidOperationException($"attempt {attempts}");
            }

            return Task.FromResult(42);
        }, new SqlExecutionOptions { MaxAttempts = 3, BaseDelay = TimeSpan.Zero });

        Assert.Equal(42, result);
        Assert.Equal(3, attempts);
        Assert.Equal(3, factory.OpenCount);
    }

    [Fact]
    public async Task ExecuteAsync_ThrowsLastExceptionAfterRetries()
    {
        var factory = new DelegateSqlConnectionFactory();

        var ex = await Assert.ThrowsAsync<ApplicationException>(() =>
            SqlExecution.ExecuteAsync<int>(factory, _ => throw new ApplicationException("boom"),
                new SqlExecutionOptions { MaxAttempts = 2, BaseDelay = TimeSpan.Zero }));

        Assert.Equal("boom", ex.Message);
        Assert.Equal(2, factory.OpenCount);
    }

    [Fact]
    public async Task ExecuteAsync_HonorsCancelledTokenBeforeOpeningConnection()
    {
        var factory = new DelegateSqlConnectionFactory();
        using var source = new CancellationTokenSource();
        await source.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            SqlExecution.ExecuteAsync(factory, _ => Task.FromResult(1), ct: source.Token));

        Assert.Equal(0, factory.OpenCount);
    }

    [Fact]
    public async Task SqlSchemaInspector_NullConnectionThrowsArgumentNullException()
    {
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            SqlSchemaInspector.TableExistsAsync(null!, "dbo", "Users"));

        Assert.Equal("conn", ex.ParamName);
    }

    [Fact]
    public void SqlSchemaInspector_ConfigureTableExistsParameters_UsesExplicitNVarCharMetadata()
    {
        using var cmd = new SqlCommand();

        SqlSchemaInspector.ConfigureTableExistsParameters(cmd, "dbo", "Users");

        Assert.Equal(2, cmd.Parameters.Count);

        var schema = Assert.Single(cmd.Parameters.Cast<SqlParameter>(), p => p.ParameterName == "@schema");
        Assert.Equal(SqlDbType.NVarChar, schema.SqlDbType);
        Assert.Equal(128, schema.Size);
        Assert.Equal("dbo", schema.Value);

        var table = Assert.Single(cmd.Parameters.Cast<SqlParameter>(), p => p.ParameterName == "@table");
        Assert.Equal(SqlDbType.NVarChar, table.SqlDbType);
        Assert.Equal(128, table.Size);
        Assert.Equal("Users", table.Value);
    }

    [Fact]
    public void SqlDataReaderExtensions_NullReaderThrowsArgumentNullException()
    {
        SqlDataReader reader = null!;

        Assert.Equal("reader", Assert.Throws<ArgumentNullException>(() => reader.Get<int>("Id")).ParamName);
        Assert.Equal("reader", Assert.Throws<ArgumentNullException>(() => reader.GetNullable<int>("Id")).ParamName);
    }

    private sealed class DelegateSqlConnectionFactory : ISqlConnectionFactory
    {
        public int OpenCount { get; private set; }
        public SqlConnection Create() => new();

        public Task<SqlConnection> OpenAsync(CancellationToken ct = default)
        {
            OpenCount++;
            ct.ThrowIfCancellationRequested();
            return Task.FromResult(new SqlConnection());
        }
    }
}
