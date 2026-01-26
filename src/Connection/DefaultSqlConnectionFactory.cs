using Microsoft.Data.SqlClient;

namespace PolyhydraGames.Data.Sql.Connection;

/// <summary>
/// Simple factory built around a delegate so callers can inject their own construction logic.
/// </summary>
public sealed class DefaultSqlConnectionFactory : ISqlConnectionFactory
{
    private readonly Func<SqlConnection> _create;

    public DefaultSqlConnectionFactory(Func<SqlConnection> create)
    {
        _create = create ?? throw new ArgumentNullException(nameof(create));
    }

    public SqlConnection Create() => _create();

    public async Task<SqlConnection> OpenAsync(CancellationToken ct = default)
    {
        var conn = Create();
        await conn.OpenAsync(ct).ConfigureAwait(false);
        return conn;
    }
}
