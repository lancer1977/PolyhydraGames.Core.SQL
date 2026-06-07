---
title: SQL release checklist
status: draft
owner: @DreadBreadcrumb
priority: high
complexity: 2
created: 2026-06-06
updated: 2026-06-06
tags: [runbook, release, checklist, Core.SQL]
---

# SQL release checklist

Use this checklist before a Core.SQL release PR, docs refresh, or packaging pass that touches connection factories, command helpers, config resolution, health checks, or the deterministic smoke.

## Release gate checklist
- [ ] Review the current roadmap notes in `docs/roadmaps/portfolio-roadmap.md`.
- [ ] Verify the docs you changed still describe the current library surface:
  - `docs/core-sql.md`
  - `docs/features/core-application-logic.md`
  - `docs/features/core-capabilities.md`
  - `docs/features/visual-studio-solution.md`
  - `docs/runbooks/sql-config-resolution-failures.md`
  - `docs/smoke/deterministic-sql-smoke.md`
- [ ] Confirm the validation matrix still covers:
  - connection factories
  - command helpers
  - config resolution
  - health checks
  - deterministic smoke
  - package restore/build
- [ ] Capture the exact command outputs or CI evidence in the PR or release notes.

## Focused test matrix
- Connection factories: `tests/Core.SQL.Tests/DefaultSqlConnectionFactoryTests.cs`
- Command helpers: `tests/Core.SQL.Tests/SqlCommandExtensionsTests.cs`
- Config resolution: `tests/Core.SQL.Tests/SqlConfigTests.cs`
- Health checks: `tests/Core.SQL.Tests/SqlHealthCheckerTests.cs`
- Execution and option guardrails: `tests/Core.SQL.Tests/SqlExecutionCoverageTests.cs`, `tests/Core.SQL.Tests/SqlExecutionOptionsTests.cs`, `tests/Core.SQL.Tests/SqlExecutionArgumentValidationTests.cs`
- Deterministic smoke glue: `scripts/sql-smoke.sh` and `docs/smoke/deterministic-sql-smoke.md`

## Required commands
```bash
cd /home/lancer1977/code/Core.SQL
dotnet restore Core.SQL.sln
dotnet build Core.SQL.sln -c Release --no-restore
dotnet test Core.SQL.sln
./scripts/sql-smoke.sh
```

## Package / restore / build checks
- `dotnet restore Core.SQL.sln`
- `dotnet build Core.SQL.sln -c Release --no-restore`
- `dotnet pack src/PolyhydraGames.Data.Sql.csproj -c Release --no-build -o artifacts/sql-smoke/packages`
- `dotnet run --project samples/Core.SQL.Smoke/Core.SQL.Smoke.csproj -c Release --no-restore --no-build`

## Release notes to record
- whether any docs-only change still required a test rerun
- whether package restore/build completed cleanly
- any regression found in the targeted tests or smoke
