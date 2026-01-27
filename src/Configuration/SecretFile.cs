namespace PolyhydraGames.Data.Sql.Configuration;

/// <summary>
/// Small helper for reading secrets from the file system (e.g. Docker Swarm secrets).
/// </summary>
public static class SecretFile
{
    public static string ReadAllTextTrimmed(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new FileNotFoundException("Secret file path was null or empty.");

        var path = filePath.Trim();
        if (!File.Exists(path))
            throw new FileNotFoundException($"Secret file not found: {path}");

        return File.ReadAllText(path).Trim();
    }
}

public static class SqlKeyNames
{
    public const string SqlPasswordFile = "SqlPasswordFile";
    public const string SqlPasswordEnv = "SqlPassword";
}