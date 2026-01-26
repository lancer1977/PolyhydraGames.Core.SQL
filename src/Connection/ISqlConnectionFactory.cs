using Microsoft.Data.SqlClient;

namespace PolyhydraGames.Data.Sql.Connection;

/// <summary>
/// Creates <see cref="SqlConnection"/> instances, usually preconfigured with a connection string.
/// </summary>
public interface ISqlConnectionFactory
{
    /// <summary>Create a connection (not opened).</summary>
    SqlConnection Create();

    /// <summary>Create and open a connection.</summary>
    Task<SqlConnection> OpenAsync(CancellationToken ct = default);
}
