using Microsoft.Data.SqlClient;
using System.Data;

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

        ConfigureTableExistsParameters(cmd, schema, table);

        var result = await cmd.ExecuteScalarAsync(ct).ConfigureAwait(false);
        return result is not null;
    }

    internal static void ConfigureTableExistsParameters(SqlCommand cmd, string schema, string table)
    {
        if (cmd is null) throw new ArgumentNullException(nameof(cmd));

        const int MaxSqlIdentifierLength = 128;

        var schemaParam = cmd.Parameters.Add("@schema", SqlDbType.NVarChar, MaxSqlIdentifierLength);
        schemaParam.Value = (object?)schema ?? DBNull.Value;

        var tableParam = cmd.Parameters.Add("@table", SqlDbType.NVarChar, MaxSqlIdentifierLength);
        tableParam.Value = (object?)table ?? DBNull.Value;
    }

    public static Task<bool> TableExistsAsync(SqlConnection conn, string table, CancellationToken ct = default)
        => TableExistsAsync(conn, "dbo", table, ct);
}
