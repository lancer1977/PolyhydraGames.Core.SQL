using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PolyhydraGames.Data.Sql.Configuration;
using PolyhydraGames.Data.Sql.Connection;

var config = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["ConnectionStrings:CoreSql"] = "Server=.;Database=CoreSQL;Trusted_Connection=True",
        ["CoreSqlSqlPassword"] = "smoke-password"
    })
    .Build();

var resolved = config.GetRequiredConnectionString("CoreSql");
var builder = new SqlConnectionStringBuilder(resolved);
var factory = new DefaultSqlConnectionFactory(() => new SqlConnection(resolved));
var connection = factory.Create();

Console.WriteLine("Core.SQL smoke sample");
Console.WriteLine($"Database: {builder.InitialCatalog}");
Console.WriteLine($"Password configured: {!string.IsNullOrWhiteSpace(builder.Password)}");
Console.WriteLine($"Factory created connection: {connection is not null}");

return 0;
