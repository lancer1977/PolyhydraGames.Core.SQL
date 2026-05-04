# Core.SQL portfolio roadmap

## Snapshot
- 90-day evidence: 9 commits, 29 files changed
- Last signal: `2ed651c`
- Last modified areas: `docs`, `tests`, `src`
- Stack: .NET
- Docs folder: yes
- Roadmap folder: no
- Features docs: yes
- Tests indexed: yes

## Implemented now (V1 baseline)
- SQL connection/factory/command behavior is tracked in `core-application-logic.md` and `core-capabilities.md`.
- Test coverage exists in `Core.SQL.Tests` for config, execution, and health checks.
- CI gating and pipeline assumptions have begun to be documented in YAML and readme paths.

## Gaps identified
- No explicit compatibility policy for connection option and command extension behavior changes.
- Pipeline/test/doc linkage not yet standardized.
- Runtime/environment assumptions for package usage could be clearer.

## V1 (stability)
- [ ] Add release checklist binding docs changes to test matrix for connection factories and command helpers.
- [ ] Add deterministic smoke for pipeline + package restore + sample execution.
- [ ] Add backward compatibility notes for API signatures and execution options.
- [ ] Document operational runbook for SQL config resolution failures.

## V2 (confidence)
- [ ] Expand tests for failure modes, invalid options, and async execution boundaries.
- [ ] Add versioned changelog entries for breaking SQL behavior.
- [ ] Standardize docs and feature mapping with release pipeline rules.
- [ ] Add environment matrix for provider compatibility.

## V10 (scale)
- [ ] Introduce compatibility matrix and deprecation windows for public SQL APIs.
- [ ] Add quality gates for docs, tests, and package publish in one workflow.
- [ ] Publish long-range reliability expectations (timeouts, retries, observability).
- [ ] Establish ownership model by API area and test ownership.

## Deployment readiness
- [ ] Every config/API change has tested and documented impact.
- [ ] CI pipeline and test suite updated in same PR.
- [ ] Changelog includes compatibility notes for runtime consumers.
