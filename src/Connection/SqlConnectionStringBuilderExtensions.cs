using Microsoft.Data.SqlClient;

namespace PolyhydraGames.Data.Sql.Connection;

/// <summary>
/// Helpers for producing SqlClient-valid connection strings.
/// </summary>
public static class SqlConnectionStringBuilderExtensions
{
    /// <summary>
    /// Takes a base connection string and injects Password=... from <paramref name="password"/>.
    /// Any existing Password or PasswordFile entries are removed.
    /// </summary>
    public static string WithPassword(this string baseConnectionString, string password)
    {
        if (string.IsNullOrWhiteSpace(baseConnectionString))
            throw new ArgumentException("Connection string was null/empty.", nameof(baseConnectionString));
        if (password is null)
            throw new ArgumentNullException(nameof(password));

        var b = new SqlConnectionStringBuilder(baseConnectionString);

        // Normalize: ensure we don't carry weird custom keys forward
        // SqlConnectionStringBuilder ignores unknown keys but will keep them in ConnectionString.
        // We'll remove common ones explicitly.
        RemoveIfPresent(b, "PasswordFile");
        b.Password = password;

        return b.ConnectionString;
    }

    private static void RemoveIfPresent(SqlConnectionStringBuilder b, string key)
    {
        if (b.ContainsKey(key))
            b.Remove(key);
    }
}
