---
title: SQL config resolution failure runbook
status: draft
owner: @DreadBreadcrumb
priority: high
complexity: 2
created: 2026-05-30
updated: 2026-05-30
tags: [runbook, Core.SQL, configuration]
---

# SQL config resolution failure runbook

## When to use this
Use this runbook when a smoke, local launch, or CI job fails while resolving a SQL connection string or secret source.

## Common failure modes
- `Missing ConnectionStrings:<name>`
  - The base connection string key is absent or empty.
- `Config error: set only one of '<name>SqlPasswordFile' or '<name>SqlPassword'`
  - Both secret sources were set at once.
- `Config error: missing '<name>SqlPasswordFile' or '<name>SqlPassword' for '<name>'`
  - The base connection string exists, but no password source was provided.
- `SQL password file not found: <path>`
  - The configured secret file path does not exist in the runner/container.

## Triage checklist
- [ ] Confirm the connection string name used by the smoke or app entrypoint.
- [ ] Verify the base value exists under `ConnectionStrings:<name>`.
- [ ] Ensure exactly one password source is set: either `<name>SqlPasswordFile` or `<name>SqlPassword`.
- [ ] If using a file path, confirm the file is mounted and readable in the target environment.
- [ ] Re-run `dotnet test tests/Core.SQL.Tests/Core.SQL.Tests.csproj -c Release --filter FullyQualifiedName~SqlConfigTests` to validate the failure shape locally.

## Recovery steps
1. Fix the missing or conflicting config entry.
2. Re-run `scripts/sql-smoke.sh`.
3. If the smoke still fails, inspect the exact exception message and compare it with the failure modes above.

## Related validation
- `tests/Core.SQL.Tests/SqlConfigTests.cs`
- `scripts/sql-smoke.sh`
- `samples/Core.SQL.Smoke/Program.cs`
