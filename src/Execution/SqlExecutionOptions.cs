namespace PolyhydraGames.Data.Sql.Execution;

public sealed record SqlExecutionOptions(
    int MaxAttempts = 3,
    TimeSpan? BaseDelay = null,
    int SlowQueryWarnMs = 750
)
{
    public TimeSpan DelayForAttempt(int attempt)
    {
        var baseDelay = BaseDelay ?? TimeSpan.FromMilliseconds(100);
        // backoff: 100ms, 200ms, 400ms...
        var factor = Math.Pow(2, Math.Max(0, attempt - 1));
        return TimeSpan.FromMilliseconds(baseDelay.TotalMilliseconds * factor);
    }
}
