
namespace PolyhydraGames.Data.Sql.Connection;

/// <summary>
/// Identifies how to build a usable SqlClient connection string from configuration.
/// </summary>
/// <param name="ConnectionStringKey">The key passed to IConfiguration.GetSqlConnectionString(...).</param>
/// <param name="PasswordFileKey">A configuration key that points to the password file location.</param>
public sealed record SqlConnectionStringRequest(string ConnectionStringKey, string PasswordFileKey);

public sealed record SqlConnectionStringResponse(string? ConnectionString, string? ErrorMessage);

public class PolySqlException : Exception
{
    public PolySqlException(string message) : base(message)
    {
    }
    public PolySqlException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public static class SqlConnections
{
    public static SqlConnectionStringResponse Fail(string message) => new SqlConnectionStringResponse(null, message);
    public static SqlConnectionStringResponse Pass(string connString) => new SqlConnectionStringResponse(connString, null);
}

