using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PolyhydraGames.Data.Sql.Configuration;
using Xunit;

namespace Core.SQL.Tests;

public class SqlConfigTests
{
    private IConfiguration CreateConfig(Dictionary<string, string?> values)
    {
        var builder = new ConfigurationBuilder();
        foreach (var kvp in values)
        {
            builder.AddInMemoryCollection(new[] { new KeyValuePair<string, string?>(kvp.Key, kvp.Value) });
        }
        return builder.Build();
    }

    [Fact]
    public void GetRequiredConnectionString_WithMissingConnectionString_ThrowsInvalidOperation()
    {
        // Arrange
        var config = CreateConfig(new Dictionary<string, string?>
        {
            { "ConnectionStrings:MyDB", "" }
        });

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            config.GetRequiredConnectionString("MyDB"));
    }

    [Fact]
    public void GetRequiredConnectionString_WithMissingPassword_ThrowsInvalidOperation()
    {
        // Arrange
        var config = CreateConfig(new Dictionary<string, string?>
        {
            { "ConnectionStrings:MyDB", "Server=.;Database=test" }
            // No MyDBSqlPassword or MyDBSqlPasswordFile
        });

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            config.GetRequiredConnectionString("MyDB"));
    }

    [Fact]
    public void GetRequiredConnectionString_WithBothPasswordSources_ThrowsInvalidOperation()
    {
        // Arrange
        var config = CreateConfig(new Dictionary<string, string?>
        {
            { "ConnectionStrings:MyDB", "Server=.;Database=test" },
            { "MyDBSqlPassword", "testpassword" },
            { "MyDBSqlPasswordFile", "/path/to/file" }
        });

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            config.GetRequiredConnectionString("MyDB"));
    }

    [Fact]
    public void GetRequiredConnectionString_WithPasswordValue_SetsPassword()
    {
        // Arrange
        var config = CreateConfig(new Dictionary<string, string?>
        {
            { "ConnectionStrings:MyDB", "Server=.;Database=test" },
            { "MyDBSqlPassword", "testpassword123" }
        });

        // Act
        var result = config.GetRequiredConnectionString("MyDB");

        // Assert
        var csb = new SqlConnectionStringBuilder(result);
        Assert.Equal("testpassword123", csb.Password);
    }

    [Fact]
    public void GetRequiredConnectionString_WithPasswordFileNotFound_ThrowsFileNotFoundException()
    {
        // Arrange
        var config = CreateConfig(new Dictionary<string, string?>
        {
            { "ConnectionStrings:MyDB", "Server=.;Database=test" },
            { "MyDBSqlPasswordFile", "/nonexistent/path/to/password.txt" }
        });

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() =>
            config.GetRequiredConnectionString("MyDB"));
    }
}
