# Core.SQL

Core.SQL is a small SQL Server helper library for connection-string resolution, lightweight execution helpers, and health checks.

## Tags

- core
- core-sql
- dotnet
- sql
- database
- docs

## Related Repos

- [`Core`](../Core/)
- [`Core.Models`](../Core.Models/)
- [`Core.Interfaces`](../Core.Interfaces/)
- [`Core.Extensions`](../Core.Extensions/)
- [`Core.Serialization`](../Core.Serialization/)

## Repository layout
- `src/PolyhydraGames.Data.Sql.csproj` — library project
- `tests/Core.SQL.Tests/Core.SQL.Tests.csproj` — focused xUnit coverage
- `samples/Core.SQL.Smoke/Core.SQL.Smoke.csproj` — deterministic smoke console app
- `scripts/sql-smoke.sh` — one-command restore/build/test/pack/sample smoke

## Quick start
```bash
dotnet restore Core.SQL.sln
dotnet build Core.SQL.sln -c Release --no-restore
dotnet test tests/Core.SQL.Tests/Core.SQL.Tests.csproj -c Release --no-build
./scripts/sql-smoke.sh
```

## Package
The library is packable as `PolyhydraGames.Data.SqlClient` from `src/PolyhydraGames.Data.Sql.csproj`.

## Configuration failure runbook
If config resolution fails, start with `docs/runbooks/sql-config-resolution-failures.md`.

## Docs
- Roadmap: `docs/roadmap/v1/README.md`
- Portfolio roadmap: `docs/roadmaps/portfolio-roadmap.md`
- Feature index: `docs/features/README.md`
- Smoke notes: `docs/smoke/deterministic-sql-smoke.md`
