namespace PolyhydraGames.Data.Sql.Connection;

/// <summary>
/// Identifies how to build a usable SqlClient connection string from configuration.
/// </summary>
/// <param name="ConnectionStringKey">The key passed to IConfiguration.GetSqlConnectionString(...).</param>
/// <param name="PasswordFileKey">A configuration key that points to the password file location.</param>
public sealed record SqlConnectionStringRequest(string ConnectionStringKey, string PasswordFileKey);
