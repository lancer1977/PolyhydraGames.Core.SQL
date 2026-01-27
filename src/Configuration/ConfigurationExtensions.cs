using Microsoft.Extensions.Configuration;
using PolyhydraGames.Data.Sql.Connection;

namespace PolyhydraGames.Data.Sql.Configuration;

/// <summary>
/// IConfiguration helpers for building secure SQL connection strings.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Reads a base connection string from <c>ConnectionStrings:{key}</c>,
    /// reads a password file location from <paramref name="request"/>,
    /// and returns a SqlClient-valid connection string with Password=... injected.
    /// </summary>
    public static string GetSqlConnectionString(this IConfiguration config, SqlConnectionStringRequest request)
    {
        if (config is null) throw new ArgumentNullException(nameof(config));
        if (request is null) throw new ArgumentNullException(nameof(request));

        var baseConn = config.GetSqlConnectionString(request.ConnectionStringKey);
        if (string.IsNullOrWhiteSpace(baseConn))
            throw new InvalidOperationException($"Missing ConnectionStrings:{request.ConnectionStringKey}");

        var passwordFile = config[request.PasswordFileKey];
        var password = SecretFile.ReadAllTextTrimmed(passwordFile);

        return baseConn.WithPassword(password);
    }

    public static string GetSqlConnectionString(this IConfiguration config, string connectionStringKey)
    {
        if (config is null) throw new ArgumentNullException(nameof(config));

        var request = new SqlConnectionStringRequest(connectionStringKey, "SqlPasswordFile");
        return config.GetSqlConnectionString(request);
    }
}
