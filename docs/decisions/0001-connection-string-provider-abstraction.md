# ADR 0001: Connection-string provider abstraction

**Status:** Accepted
**Date:** 2026-06-06
**Hermes Kanban:** t_67cccd33

## Context

The March proposal in the Polyhydra vault asked whether Core.SQL should add a first-class `IConnectionStringProvider` abstraction so consumers stop duplicating connection-string and secret-loading logic.

The current repository already has two smaller seams:

- `src/Configuration/SqlConfig.cs` exposes `IConfiguration.GetRequiredConnectionString(name)`, resolves `ConnectionStrings:<name>`, enforces exactly one password source, and supports either `<Name>SqlPassword` or `<Name>SqlPasswordFile`.
- `src/Connection/ISqlConnectionFactory.cs` and `src/Connection/DefaultSqlConnectionFactory.cs` are the runtime connection seam. Callers can resolve the connection string once and inject connection construction with a `Func<SqlConnection>`.
- `src/Health/SqlHealthChecker.cs` already consumes `ISqlConnectionFactory`, so health checks validate the same runtime connection seam instead of needing a separate configuration provider dependency.

Focused tests now cover the important config failure and success paths in `tests/Core.SQL.Tests/SqlConfigTests.cs`:

- missing named connection string throws
- missing password source throws
- both password value and password file throws
- password value is applied to the generated connection string
- missing password file throws

## Decision

Do not add a first-class `IConnectionStringProvider` now. Keep the current `SqlConfig.GetRequiredConnectionString` + `ISqlConnectionFactory` pattern as the public design.

This keeps Core.SQL small and avoids introducing an overlapping abstraction whose main responsibility is already covered by `IConfiguration` plus the existing extension helper. Consumers that need dependency injection can compose the current seams directly:

```csharp
var connectionString = config.GetRequiredConnectionString("CoreSql");
services.AddSingleton<ISqlConnectionFactory>(
    _ => new DefaultSqlConnectionFactory(() => new SqlConnection(connectionString)));
```

## Consequences

- The March provider proposal is closed as implemented-by-current-pattern rather than converted into an implementation card.
- Health-check work should continue to depend on `ISqlConnectionFactory`; it should not introduce a provider-only path.
- Future work should improve documentation/examples around the existing composition pattern before adding new API.
- No follow-up implementation card is required for `IConnectionStringProvider` unless a real consumer demonstrates a need for late-bound per-tenant/keyed connection resolution that cannot be expressed by `IConfiguration` plus `ISqlConnectionFactory`.

## Deferred API shape if requirements change

If a future consumer proves that connection strings must be resolved repeatedly by key or tenant at runtime, create a separate implementation card instead of reopening this ADR in place. That card should propose exactly this additive shape:

```csharp
public interface ISqlConnectionStringProvider
{
    string GetRequiredConnectionString(string name);
}
```

Acceptance criteria for that future card should include:

- default `IConfiguration` implementation delegates to `SqlConfig.GetRequiredConnectionString`
- DI registration extensions for the provider and `ISqlConnectionFactory` composition
- unit tests proving provider delegation, missing configuration behavior, password value, password file, and factory/health-check composition
- docs showing when to use the provider versus direct `SqlConfig` calls

Until that concrete runtime requirement exists, the smaller helper-only pattern is the accepted design.
