# Project Atlas

## Purpose

`Core.SQL` provides SQL Server connection-string resolution, command helpers, execution retry helpers, schema inspection, and health checks for PolyhydraGames services.

## Primary Validation

```bash
./scripts/sql-smoke.sh
```

## Package Surface

- Main project: `src/PolyhydraGames.Data.Sql.csproj`
- Tests: `tests/Core.SQL.Tests`
- Smoke sample: `samples/Core.SQL.Smoke`
- Package artifact: `artifacts/sql-smoke/packages`

## Tracking

- CI/artifact issue: https://github.com/lancer1977/PolyhydraGames.Core.SQL/issues/4
- Repo-health audit: https://github.com/lancer1977/PolyhydraGames.Core.SQL/issues/5
