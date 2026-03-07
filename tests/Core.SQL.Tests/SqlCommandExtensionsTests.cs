using Microsoft.Data.SqlClient;
using System.Data;
using Xunit;
using PolyhydraGames.Data.Sql.Commands;

namespace Core.SQL.Tests;

public class SqlCommandExtensionsTests
{
    private SqlCommand CreateCommand()
    {
        var connection = new SqlConnection("Server=.;Database=test;Trusted_Connection=True");
        return new SqlCommand { Connection = connection };
    }

    [Fact]
    public void Add_WithValidNameAndValue_ShouldAddParameter()
    {
        // Arrange
        using var cmd = CreateCommand();

        // Act
        var param = cmd.Add("@TestParam", "test value");

        // Assert
        Assert.NotNull(param);
        Assert.Equal("@TestParam", param.ParameterName);
        Assert.Equal("test value", param.Value);
    }

    [Fact]
    public void Add_WithNullValue_ShouldAddDbNull()
    {
        // Arrange
        using var cmd = CreateCommand();

        // Act
        var param = cmd.Add("@NullParam", null!);

        // Assert
        Assert.NotNull(param);
        Assert.Equal(DBNull.Value, param.Value);
    }

    [Fact]
    public void Add_WithEmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        using var cmd = CreateCommand();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => cmd.Add("", "value"));
    }

    [Fact]
    public void Add_WithNullName_ShouldThrowArgumentException()
    {
        // Arrange
        using var cmd = CreateCommand();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => cmd.Add(null!, "value"));
    }

    [Fact]
    public void AddNullable_WithNullValue_ShouldAddDbNull()
    {
        // Arrange
        using var cmd = CreateCommand();

        // Act
        var param = cmd.AddNullable<string>("@NullableParam", null);

        // Assert
        Assert.NotNull(param);
        Assert.Equal(DBNull.Value, param.Value);
    }

    [Fact]
    public void AddNullable_WithValue_ShouldAddParameter()
    {
        // Arrange
        using var cmd = CreateCommand();

        // Act
        var param = cmd.AddNullable<string>("@NullableParam", "test");

        // Assert
        Assert.NotNull(param);
        Assert.Equal("test", param.Value);
    }

    [Fact]
    public void AddNullableStruct_WithNullValue_ShouldAddDbNull()
    {
        // Arrange
        using var cmd = CreateCommand();

        // Act
        var param = cmd.AddNullableStruct<int>("@IntParam", null);

        // Assert
        Assert.NotNull(param);
        Assert.Equal(DBNull.Value, param.Value);
    }

    [Fact]
    public void AddNullableStruct_WithValue_ShouldAddParameter()
    {
        // Arrange
        using var cmd = CreateCommand();

        // Act
        var param = cmd.AddNullableStruct<int>("@IntParam", 42);

        // Assert
        Assert.NotNull(param);
        Assert.Equal("42", param.Value);
    }

    [Fact]
    public void AddUtc_WithUtcTime_ShouldSetDateTime2()
    {
        // Arrange
        using var cmd = CreateCommand();
        var utcTime = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);

        // Act
        var param = cmd.AddUtc("@UtcParam", utcTime);

        // Assert
        Assert.NotNull(param);
        Assert.Equal(SqlDbType.DateTime2, param.SqlDbType);
        Assert.Equal(utcTime, param.Value);
    }

    [Fact]
    public void AddUtc_WithLocalTime_ShouldConvertToUtc()
    {
        // Arrange
        using var cmd = CreateCommand();
        var localTime = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Local);

        // Act
        var param = cmd.AddUtc("@UtcParam", localTime);

        // Assert
        Assert.NotNull(param);
        Assert.Equal(DateTimeKind.Utc, ((DateTime)param.Value).Kind);
    }

    [Fact]
    public void AddGuid_WithValidGuid_ShouldSetUniqueIdentifier()
    {
        // Arrange
        using var cmd = CreateCommand();
        var guid = Guid.NewGuid();

        // Act
        var param = cmd.AddGuid("@GuidParam", guid);

        // Assert
        Assert.NotNull(param);
        Assert.Equal(SqlDbType.UniqueIdentifier, param.SqlDbType);
        Assert.Equal(guid, param.Value);
    }
}
