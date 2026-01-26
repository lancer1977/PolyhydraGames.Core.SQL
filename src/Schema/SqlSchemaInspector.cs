using Microsoft.Data.SqlClient;

namespace PolyhydraGames.Data.Sql.Schema;

/// <summary>
/// Lightweight schema checks (no ORM).
/// </summary>
public static class SqlSchemaInspector
{
    public static async Task<bool> TableExistsAsync(SqlConnection conn, string schema, string table, CancellationToken ct = default)
    {
        if (conn is null) throw new ArgumentNullException(nameof(conn));

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
SELECT 1
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @table;";

        cmd.Parameters.AddWithValue("@schema", schema);
        cmd.Parameters.AddWithValue("@table", table);

        var result = await cmd.ExecuteScalarAsync(ct).ConfigureAwait(false);
        return result is not null;
    }

    public static Task<bool> TableExistsAsync(SqlConnection conn, string table, CancellationToken ct = default)
        => TableExistsAsync(conn, "dbo", table, ct);
}
