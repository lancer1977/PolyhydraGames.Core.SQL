using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace PolyhydraGames.Data.Sql.Configuration;

public static class SqlConfig
{
    public static string GetRequiredConnectionString(this IConfiguration config, string name)
    {
        var baseConn = config.GetConnectionString(name);
        if (string.IsNullOrWhiteSpace(baseConn))
            throw new InvalidOperationException($"Missing ConnectionStrings:{name}");

        var fileKey = $"{name}SqlPasswordFile";
        var passKey = $"{name}SqlPassword";

        var passwordFile = config[fileKey];
        var passwordValue = config[passKey];

        if (!string.IsNullOrWhiteSpace(passwordFile) && !string.IsNullOrWhiteSpace(passwordValue))
            throw new InvalidOperationException($"Config error: set only one of '{fileKey}' or '{passKey}'.");

        var password =
            !string.IsNullOrWhiteSpace(passwordFile) ? ReadSecretFile(passwordFile) :
            !string.IsNullOrWhiteSpace(passwordValue) ? passwordValue :
            throw new InvalidOperationException($"Config error: missing '{fileKey}' or '{passKey}' for '{name}'.");

        var csb = new SqlConnectionStringBuilder(baseConn)
        {
            Password = password
        };

        return csb.ConnectionString;
    }

    private static string ReadSecretFile(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"SQL password file not found: {path}", path);

        return File.ReadAllText(path).Trim();
    }
}
