namespace PolyhydraGames.Data.Sql.Health;

public sealed record SqlHealthStatus(
    bool CanConnect,
    TimeSpan Latency,
    string? Error = null
);
