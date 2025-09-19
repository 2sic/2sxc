# Tasks: 2sxc Oqtane Multi-Database Support

**Input**: Design documents from `/specs/001-2sxc-oqtane-multi/`
**Prerequisites**: plan.md (required), research.md, data-model.md, contracts/

## Phase 3.1: Setup

- [x] T001 Confirm repo structure and targets from plan.md; no new projects required
- [ ] T002 Ensure local Oqtane dev instance is running (IISExpress at [http://localhost:44357](http://localhost:44357)) and SQL Server is reachable
- [ ] T003 [P] Create a new tests folder tree for this feature if missing:
  - tests/contract/oqtane-multi/
  - tests/integration/oqtane-multi/
  - tests/unit/oqtane-multi/
- [ ] T004 [P] Add any missing test utilities to `ToSic.Sxc.Oqt.Server.Tests` for multi-tenant setup scaffolding

## Phase 3.2: Tests First (TDD) — MUST FAIL before implementation

- [x] T005 [P] Contract test for ITenantSiteContext in `tests/contract/oqtane-multi/TenantSiteContext.contract.cs`
  - Assert resolution produces {TenantId, SiteId} from Oqtane runtime services
  - Implemented in `Src/Oqtane/ToSic.Sxc.Oqt.Server.Tests/Context/TenantSiteContextIntegrationTests.cs`
- [x] T006 [P] Contract test for IConnectionSelector in `tests/contract/oqtane-multi/ConnectionSelector.contract.cs`
  - Assert correct connection string/DbConnection is returned for current Tenant
  - Implemented in `Src/Oqtane/ToSic.Sxc.Oqt.Server.Tests/Data/OqtConnectionSelectorTests.cs`
- [x] T007 [P] Contract test for ICacheKeyBuilder in `tests/contract/oqtane-multi/CacheKeyBuilder.contract.cs`
  - Assert cache keys include TenantSiteKey and are deterministic
  - Implemented in Oqt.Server test suite (site-scoped cache key builder tests)
- [ ] T008 [P] Integration test: Two tenants with same SiteId (1) remain isolated in `tests/integration/oqtane-multi/TenantsIsolation.integration.cs`
  - Create content in Tenant A; verify not visible in Tenant B
- [ ] T009 [P] Integration test: Site alias/rename does not affect isolation in `tests/integration/oqtane-multi/SiteAliasRename.integration.cs`
  - Change alias or name; verify isolation persists

## Phase 3.3: Core Implementation (after tests are failing)

- [x] T010 [P] Implement TenantSiteKey value object in `Src/Oqtane/ToSic.Sxc.Oqt.Shared/TenantSiteKey.cs`
  - Immutable, equatable, ToString for cache keys
- [x] T011 Implement ITenantSiteContext in `Src/Oqtane/ToSic.Sxc.Oqt.Server/Services/TenantSiteContext.cs`
  - Resolve via Oqtane services; register as request-scoped in DI
  - Implemented as `OqtTenantSiteContext` (uses `AliasResolver.Alias`)
- [x] T012 Implement IConnectionSelector in `Src/Oqtane/ToSic.Sxc.Oqt.Server/Services/ConnectionSelector.cs`
  - Delegate to Oqtane’s tenant/connection provider; no static state
  - Implemented as `OqtConnectionSelector` with site/tenant precedence and tests
- [x] T013 [P] Implement ICacheKeyBuilder in `Src/Oqtane/ToSic.Sxc.Oqt.Server/Services/CacheKeyBuilder.cs`
  - Include TenantSiteKey for site-scoped caches; deterministic format
  - Implemented as `OqtCacheKeyBuilder` and registered in DI
- [x] T014 Update cache usage sites to use ICacheKeyBuilder in `Src/Oqtane/ToSic.Sxc.Oqt.Server` and `Src/Sxc/` where applicable
  - Keep core-neutral boundaries; add Oqtane-specific wiring only in Oqtane layer
- [ ] T015 Add TenantId to DataZone and other relevant EAV records (TsDynDataZone, etc.)
  - Coordinate via EAV package update; do not copy sources. Document fields in data-model.md
- [ ] T016 Add logging enrichment with TenantId/SiteId in `Src/Oqtane/ToSic.Sxc.Oqt.Server` pipeline

## Phase 3.4: Integration

- [x] T017 Wire IConnectionSelector into EAV usage so all DB calls hit the correct tenant DB
  - Implemented via `OqtSqlPlatformInfo` overriding `FindConnectionString` with site/tenant precedence; also supports named connections
- [x] T018 Validate single-tenant backward compatibility (TenantId null/0 maps to default) via a dedicated integration test `tests/integration/oqtane-multi/SingleTenantCompat.integration.cs`
- [x] T019 Guard and document migration considerations for TenantId additions (no mandatory migration)

## Phase 3.5: Polish

- [x] T020 [P] Unit tests for TenantSiteKey equality and formatting in `tests/unit/oqtane-multi/TenantSiteKey.tests.cs`
  - Implemented in `Src/Oqtane/ToSic.Sxc.Oqt.Server.Tests/Models/TenantSiteKeyTests.cs`
- [x] T021 [P] Performance check: Ensure DB selection + context resolution adds <= 1 ms overhead after warm-up
- [ ] T022 [P] Update docs: Add notes to `specs/001-2sxc-oqtane-multi/quickstart.md` with any environment gotchas
  - Documentation added under `Src/Oqtane/ToSic.Sxc.Oqt.Server/Docs/Configuration.MultiDatabase.md` and linked from `README.md` (spec quickstart TBD)
- [x] T023 Clean up logs and ensure no sensitive data emitted; review against Security Gate
  - Reviewed: no connection strings or secrets are logged. Added `Src/Oqtane/ToSic.Sxc.Oqt.Server/Docs/Logging.Guidance.md` with recommendations.

## Dependencies

- Setup (T001-T004) before Tests
- Tests (T005-T009) before Implementation (T010+)
- T010 blocks T013 and partially T014
- T011 blocks T017
- T012 blocks T017
- T013 blocks T014
- EAV update (T015) can proceed in parallel with T013 but must complete before final integration verification

## Parallel Execution Examples
 
```text
# Kick off contract tests in parallel
Task: "Contract test ITenantSiteContext" → tests/contract/oqtane-multi/TenantSiteContext.contract.cs
Task: "Contract test IConnectionSelector" → tests/contract/oqtane-multi/ConnectionSelector.contract.cs
Task: "Contract test ICacheKeyBuilder" → tests/contract/oqtane-multi/CacheKeyBuilder.contract.cs

# Kick off integration tests in parallel (different files)
Task: "Isolation across tenants" → tests/integration/oqtane-multi/TenantsIsolation.integration.cs
Task: "Alias/rename isolation" → tests/integration/oqtane-multi/SiteAliasRename.integration.cs
```

## Validation Checklist

- [ ] All contracts have corresponding tests (T005-T007)
- [ ] All entities have model tasks (TenantSiteKey, DataZone updated)
- [ ] All tests come before implementation
- [ ] Parallel tasks touch different files
- [ ] Each task specifies exact file paths
- [ ] No [P] tasks write to the same file
