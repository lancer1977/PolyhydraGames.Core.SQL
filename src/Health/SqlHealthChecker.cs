using PolyhydraGames.Data.Sql.Connection;
using System.Diagnostics;

namespace PolyhydraGames.Data.Sql.Health;

public static class SqlHealthChecker
{
    public static async Task<SqlHealthStatus> CheckAsync(ISqlConnectionFactory factory, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            await using var conn = await factory.OpenAsync(ct).ConfigureAwait(false);
            sw.Stop();
            return new SqlHealthStatus(true, sw.Elapsed);
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new SqlHealthStatus(false, sw.Elapsed, ex.Message);
        }
    }
}
