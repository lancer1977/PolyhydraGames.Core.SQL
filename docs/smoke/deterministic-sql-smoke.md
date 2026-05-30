---
title: Deterministic SQL smoke
status: draft
owner: @DreadBreadcrumb
priority: high
complexity: 2
created: 2026-05-30
updated: 2026-05-30
tags: [smoke, Core.SQL, ci]
---

# Deterministic SQL smoke

## Purpose
Provide one repeatable command that covers restore, build, test, pack, and sample execution.

## Smoke command
```bash
./scripts/sql-smoke.sh
```

## What it proves
- NuGet restore succeeds for the solution.
- The library and smoke sample compile in Release.
- The test project runs without rebuilding.
- The package can be produced from the library project.
- The sample console app can execute against deterministic in-memory configuration.

## Notes
- The sample does not connect to a live SQL Server.
- This smoke is intended to catch packaging, restore, and configuration regression before release work expands.
