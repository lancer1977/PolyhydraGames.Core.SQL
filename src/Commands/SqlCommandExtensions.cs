using Microsoft.Data.SqlClient;
using System.Data;

namespace PolyhydraGames.Data.Sql.Commands;

/// <summary>
/// Small helpers for parameter creation to reduce null/DbType mistakes.
/// </summary>
public static class SqlCommandExtensions
{
    public static SqlParameter Add(this SqlCommand cmd, string name, object value)
    {
        if (cmd is null) throw new ArgumentNullException(nameof(cmd));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Parameter name was null/empty.", nameof(name));

        var p = cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
        return p;
    }

    public static SqlParameter AddNullable<T>(this SqlCommand cmd, string name, T? value) where T : class
        => cmd.Add(name, value ?? (object)DBNull.Value);

    public static SqlParameter AddNullableStruct<T>(this SqlCommand cmd, string name, T? value) where T : struct
        => cmd.Add(name, value?.ToString() ?? (object)DBNull.Value);

    public static SqlParameter AddUtc(this SqlCommand cmd, string name, DateTime utc)
    {
        if (utc.Kind != DateTimeKind.Utc)
            utc = DateTime.SpecifyKind(utc, DateTimeKind.Utc);

        var p = cmd.Parameters.Add(name, SqlDbType.DateTime2);
        p.Value = utc;
        return p;
    }

    public static SqlParameter AddGuid(this SqlCommand cmd, string name, Guid value)
    {
        var p = cmd.Parameters.Add(name, SqlDbType.UniqueIdentifier);
        p.Value = value;
        return p;
    }
}
