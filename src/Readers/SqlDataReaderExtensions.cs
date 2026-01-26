using Microsoft.Data.SqlClient;

namespace PolyhydraGames.Data.Sql.Readers;

/// <summary>
/// Typed reader helpers.
/// </summary>
public static class SqlDataReaderExtensions
{
    public static T Get<T>(this SqlDataReader reader, string column)
    {
        if (reader is null) throw new ArgumentNullException(nameof(reader));
        var ordinal = reader.GetOrdinal(column);
        if (reader.IsDBNull(ordinal))
            throw new InvalidOperationException($"Column '{column}' was NULL but requested as {typeof(T).Name}.");

        return (T)reader.GetValue(ordinal);
    }

    public static T? GetNullable<T>(this SqlDataReader reader, string column)
    {
        if (reader is null) throw new ArgumentNullException(nameof(reader));
        var ordinal = reader.GetOrdinal(column);
        if (reader.IsDBNull(ordinal))
            return default;

        return (T)reader.GetValue(ordinal);
    }

    public static string GetStringSafe(this SqlDataReader reader, string column)
        => reader.GetNullable<string>(column) ?? string.Empty;
}
