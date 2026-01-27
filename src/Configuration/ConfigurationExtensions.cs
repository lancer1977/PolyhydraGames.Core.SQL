using Microsoft.Extensions.Configuration;
using PolyhydraGames.Data.Sql.Connection;

namespace PolyhydraGames.Data.Sql.Configuration;

/// <summary>
/// IConfiguration helpers for building secure SQL connection strings.
/// </summary>
public static class ConfigurationExtensions
{
    public static bool IsValid(this SqlConnectionStringResponse response)
    {
        return string.IsNullOrEmpty(response.ErrorMessage);
    }

    public static string GetRequiredConnectionString(this IConfiguration config, string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new Exception($"Key was not provided for GetConnectionString!");
        }
        var result = config.GetSqlConnectionString(key);
        if (!result.IsValid())
        {
            throw new Exception(result.ErrorMessage);
        }

        return result.ConnectionString!;
    }
    /// <summary>
    /// Reads a base connection string from <c>ConnectionStrings:{key}</c>,
    /// reads a password file location from <paramref name="request"/>,
    /// and returns a SqlClient-valid connection string with Password=... injected.
    /// </summary>
    public static SqlConnectionStringResponse GetSqlConnectionString(this IConfiguration config, SqlConnectionStringRequest request)
    { 

        var baseConn = config.GetConnectionString(request.ConnectionStringKey);
        if (string.IsNullOrWhiteSpace(baseConn)) return SqlConnection.Fail($"Missing ConnectionStrings:{request.ConnectionStringKey}");

        var passwordFile = config[request.PasswordFileKey];
        var password = SecretFile.ReadAllTextTrimmed(passwordFile);
        var result = baseConn.WithPassword(password);
        return SqlConnection.Pass(result);
    }

    public static SqlConnectionStringResponse GetSqlConnectionString(this IConfiguration config, string connectionStringKey)
    {
        

        var request = new SqlConnectionStringRequest(connectionStringKey, "SqlPasswordFile");
        return config.GetSqlConnectionString(request);
    }

}
