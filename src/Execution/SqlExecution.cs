using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using PolyhydraGames.Data.Sql.Connection;
using System.Diagnostics;

namespace PolyhydraGames.Data.Sql.Execution;

/// <summary>
/// Minimal execution wrapper with conservative retry for transient-ish SqlExceptions.
/// </summary>
public static class SqlExecution
{
    public static async Task<T> ExecuteAsync<T>(
        ISqlConnectionFactory factory,
        Func<SqlConnection, Task<T>> action,
        SqlExecutionOptions? options = null,
        ILogger? logger = null,
        CancellationToken ct = default)
    {
        if (factory is null) throw new ArgumentNullException(nameof(factory));
        if (action is null) throw new ArgumentNullException(nameof(action));

        options ??= new SqlExecutionOptions();

        Exception? last = null;

        for (var attempt = 1; attempt <= options.MaxAttempts; attempt++)
        {
            ct.ThrowIfCancellationRequested();

            try
            {
                await using var conn = await factory.OpenAsync(ct).ConfigureAwait(false);

                var sw = Stopwatch.StartNew();
                var result = await action(conn).ConfigureAwait(false);
                sw.Stop();

                if (logger is not null && sw.ElapsedMilliseconds >= options.SlowQueryWarnMs)
                    logger.LogWarning("Slow SQL operation: {ElapsedMs}ms", sw.ElapsedMilliseconds);

                return result;
            }
            catch (SqlException ex) when (attempt < options.MaxAttempts && IsTransient(ex))
            {
                last = ex;
                logger?.LogWarning(ex, "Transient SqlException on attempt {Attempt}/{MaxAttempts}. Retrying...", attempt, options.MaxAttempts);
                await Task.Delay(options.DelayForAttempt(attempt), ct).ConfigureAwait(false);
            }
            catch (Exception ex) when (attempt < options.MaxAttempts)
            {
                last = ex;
                logger?.LogWarning(ex, "Exception on attempt {Attempt}/{MaxAttempts}. Retrying...", attempt, options.MaxAttempts);
                await Task.Delay(options.DelayForAttempt(attempt), ct).ConfigureAwait(false);
            }
        }

        throw new InvalidOperationException("SQL operation failed after retries.", last);
    }

    public static Task ExecuteAsync(
        ISqlConnectionFactory factory,
        Func<SqlConnection, Task> action,
        SqlExecutionOptions? options = null,
        ILogger? logger = null,
        CancellationToken ct = default)
        => ExecuteAsync(factory, async c => { await action(c).ConfigureAwait(false); return 0; }, options, logger, ct);

    private static bool IsTransient(SqlException ex)
    {
        // Conservative list: add as you observe your environment.
        // 4060 = Cannot open database requested by login (sometimes transient during failover)
        // 10928/10929 = SQL Azure resource limits (if ever)
        // 40197/40501/40613 = SQL Azure transient/failover (if ever)
        // 1205 = deadlock victim
        // -2 = timeout (SqlClient uses -2 for timeout)
        var transientNumbers = new HashSet<int> { -2, 1205, 4060, 10928, 10929, 40197, 40501, 40613 };

        foreach (SqlError err in ex.Errors)
        {
            if (transientNumbers.Contains(err.Number))
                return true;
        }
        return false;
    }
}
