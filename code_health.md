# Code Health

Last reviewed: 2026-06-27

## Native Validation

```bash
./scripts/sql-smoke.sh
dotnet list Core.SQL.sln package --outdated
devstudio validate --repo /home/lancer1977/code/Core.SQL
```

## Current Findings

- Deterministic smoke passes locally: restore, build, 38 tests, pack, and sample run.
- CI runs the smoke script and uploads `core-sql-nuget` package artifacts.
- Tagged releases publish `.nupkg` files to GitHub Packages with the built-in `GITHUB_TOKEN`.
- Dependency drift is limited to `Microsoft.Data.SqlClient` 7.0.1 -> 7.0.2.
- Generated Dev Studio runtime state remains untracked.

## Follow-Up Backlog

- Apply the `Microsoft.Data.SqlClient` patch update in a package-maintenance slice.
- Add a NuGet package readme to remove the current pack warning.
- Decide whether this repo should adopt central package management with the rest of the Core family.
- Add more public API examples for downstream connection-string and health-check consumers.
