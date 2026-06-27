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

## Validation and artifacts

`./scripts/sql-smoke.sh` is the native validation path. It restores, builds, runs the xUnit tests, packs `PolyhydraGames.Data.SqlClient`, and runs the deterministic smoke sample.

GitHub Actions runs the same smoke script. Every run uploads the package outputs as the `core-sql-nuget` workflow artifact from `artifacts/sql-smoke/packages`. Tags matching `v*` also publish the `.nupkg` to GitHub Packages using the built-in `GITHUB_TOKEN`; no extra repository secret is required.

Dependency audit:

```bash
dotnet list Core.SQL.sln package --outdated
```

The current drift is `Microsoft.Data.SqlClient` 7.0.1 -> 7.0.2 and is tracked for the next package-maintenance slice.

## Package
The library is packable as `PolyhydraGames.Data.SqlClient` from `src/PolyhydraGames.Data.Sql.csproj`.

## Configuration failure runbook
If config resolution fails, start with `docs/runbooks/sql-config-resolution-failures.md`.

## Docs
- Roadmap: `docs/roadmap/v1/README.md`
- Portfolio roadmap: `docs/roadmaps/portfolio-roadmap.md`
- Feature index: `docs/features/README.md`
- Smoke notes: `docs/smoke/deterministic-sql-smoke.md`
