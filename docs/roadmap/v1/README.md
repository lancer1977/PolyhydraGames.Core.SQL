# Core.SQL Roadmap (v1)

## Vision
_What this library is trying to accomplish._

## Current Status
- [x] **Stable** — current public design keeps `SqlConfig.GetRequiredConnectionString` as the configuration helper and `ISqlConnectionFactory` as the runtime connection seam.
- Decision record: `docs/decisions/0001-connection-string-provider-abstraction.md`.
- Hermes Kanban: t_67cccd33.

## Goals
- Keep Core.SQL a small ADO.NET helper package rather than a broad repository/ORM layer.
- Document and test connection-string resolution, secret injection, connection factory composition, execution helpers, and health probes.

## Known Gaps
- No `IConnectionStringProvider` implementation is planned unless a future consumer proves a late-bound keyed/tenant runtime requirement that `IConfiguration` plus `ISqlConnectionFactory` cannot satisfy.
- Health-check expansion should continue to consume `ISqlConnectionFactory` rather than adding a provider-only configuration seam.
