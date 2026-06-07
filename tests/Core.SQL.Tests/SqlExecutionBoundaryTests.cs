using Microsoft.Data.SqlClient;
using PolyhydraGames.Data.Sql.Connection;
using PolyhydraGames.Data.Sql.Execution;
using Xunit;

namespace Core.SQL.Tests;

public class SqlExecutionBoundaryTests
{
    [Fact]
    public async Task ExecuteAsync_WithMaxAttemptsZero_DoesNotOpenConnectionAndThrowsInvalidOperationException()
    {
        var factory = new RecordingSqlConnectionFactory();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            SqlExecution.ExecuteAsync<int>(
                factory,
                _ => Task.FromResult(1),
                new SqlExecutionOptions { MaxAttempts = 0 }));

        Assert.Equal("SQL operation failed after retries.", ex.Message);
        Assert.Null(ex.InnerException);
        Assert.Equal(0, factory.OpenCount);
    }

    [Fact]
    public async Task ExecuteAsync_CancelsDuringRetryDelay_StopsBeforeSecondAttempt()
    {
        var factory = new RecordingSqlConnectionFactory();
        var firstAttemptReached = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var cts = new CancellationTokenSource();

        _ = Task.Run(async () =>
        {
            await firstAttemptReached.Task;
            await Task.Delay(25);
            await cts.CancelAsync();
        });

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            SqlExecution.ExecuteAsync(
                factory,
                _ =>
                {
                    firstAttemptReached.TrySetResult();
                    throw new InvalidOperationException("boom");
                },
                new SqlExecutionOptions { MaxAttempts = 3, BaseDelay = TimeSpan.FromMinutes(1) },
                ct: cts.Token));

        Assert.Equal(1, factory.OpenCount);
        Assert.True(firstAttemptReached.Task.IsCompleted);
    }

    private sealed class RecordingSqlConnectionFactory : ISqlConnectionFactory
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
