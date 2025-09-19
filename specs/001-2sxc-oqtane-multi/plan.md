# Implementation Plan: 2sxc Oqtane Multi-Database Support

**Branch**: `001-2sxc-oqtane-multi` | **Date**: 2025-09-19 | **Spec**: specs/001-2sxc-oqtane-multi/spec.md
**Input**: Feature specification from `/specs/001-2sxc-oqtane-multi/spec.md`

## Execution Flow (/plan command scope)
```
1. Load feature spec from Input path
   → If not found: ERROR "No feature spec at {path}"
2. Fill Technical Context (scan for NEEDS CLARIFICATION)
   → Detect Project Type from context (web=frontend+backend, mobile=app+api)
   → Set Structure Decision based on project type
3. Fill the Constitution Check section based on the content of the constitution document.
4. Evaluate Constitution Check section below
   → If violations exist: Document in Complexity Tracking
   → If no justification possible: ERROR "Simplify approach first"
   → Update Progress Tracking: Initial Constitution Check
5. Execute Phase 0 → research.md
   → If NEEDS CLARIFICATION remain: ERROR "Resolve unknowns"
6. Execute Phase 1 → contracts, data-model.md, quickstart.md, agent-specific template file (e.g., `CLAUDE.md` for Claude Code, `.github/copilot-instructions.md` for GitHub Copilot, `GEMINI.md` for Gemini CLI, `QWEN.md` for Qwen Code or `AGENTS.md` for opencode).
7. Re-evaluate Constitution Check section
   → If new violations: Refactor design, return to Phase 1
   → Update Progress Tracking: Post-Design Constitution Check
8. Plan Phase 2 → Describe task generation approach (DO NOT create tasks.md)
9. STOP - Ready for /tasks command
```

**IMPORTANT**: The /plan command STOPS at step 7. Phases 2-4 are executed by other commands:
- Phase 2: /tasks command creates tasks.md
- Phase 3-4: Implementation execution (manual or via tools)

## Summary
Enable 2sxc on Oqtane to run correctly in multi-tenant installations where each tenant has its own database. On each request, select the correct tenant database using the current Oqtane Tenant and Site context and treat identity as a composite {TenantId, SiteId}. Ensure tenant-scoped caching, APIs, and logging. Persist TenantId in 2sxc/EAV records where needed (including TsDynDataZone). No feature flag. Backwards compatible with single-tenant setups.

## Technical Context
**Language/Version**: C# 13+ (.NET 9 for Oqtane; .NET Framework 4.x for DNN unaffected)  
**Primary Dependencies**: Oqtane framework integration, 2sxc core (`Src/Sxc/ToSic.Sxc*`), EAV data layer (external repo `A:/2sxc/eav-server`)  
**Storage**: SQL Server per-tenant databases in Oqtane; Master DB provides Tenant metadata; EAV tables updated to carry TenantId where required  
**Testing**: xUnit test projects under `Src/Oqtane.Tests/` and `Src/Sxc/ToSic.Sxc*Tests/`; integration tests against local Oqtane instance  
**Target Platform**: Windows dev; Oqtane on IISExpress (http://localhost:44357); SQL Server localdb/instance; DNN unchanged  
**Project Type**: single (library + platform adapters); web host is Oqtane but feature impacts backend libraries only  
**Performance Goals**: Tenant/Site resolution and DB selection ≤ 1 ms overhead after warm-up  
**Constraints**: Maintain strict core/integration boundaries; do not copy EAV sources; preserve backwards compatibility for single-tenant  
**Scale/Scope**: Multi-tenant Oqtane with N tenants; SiteId values repeat per tenant; composite identity must prevent cross-tenant bleed

## Constitution Check
*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

Simplicity Gate
- [x] ≤ 3 new projects introduced by this feature (none planned)
- [x] No speculative features; only requirements from spec

Anti-Abstraction Gate
- [x] Use Oqtane platform features directly for tenant context and connection strings
- [x] Avoid new generic wrappers; prefer minimal adapters where needed

Integration-First Gate
- [ ] API/contracts defined and reviewed (to be produced in Phase 1)
- [ ] Contract/integration tests outlined before implementation (Phase 1)

Boundary Gate
- [x] Core-neutral code stays in `Src/Sxc/`
- [x] Oqtane specifics in `Src/Oqtane/`
- [x] No sources pulled from `A:/2sxc/eav-server` into this repo

Security Gate
- [x] Inputs validated; explicit error handling on tenant resolution/DB selection
- [x] Logging avoids sensitive data; least-privilege connection usage

## Project Structure

### Documentation (this feature)
```
specs/001-2sxc-oqtane-multi/
├── plan.md              # This file (/plan command output)
├── research.md          # Phase 0 output (/plan command)
├── data-model.md        # Phase 1 output (/plan command)
├── quickstart.md        # Phase 1 output (/plan command)
├── contracts/           # Phase 1 output (/plan command)
└── tasks.md             # Phase 2 output (/tasks command - NOT created by /plan)
```

### Source Code (repository root)
```
# Option 1: Single project (DEFAULT)
src/
├── models/
├── services/
├── cli/
└── lib/

tests/
├── contract/
├── integration/
└── unit/

# Option 2: Web application (when "frontend" + "backend" detected)
backend/
├── src/
│   ├── models/
│   ├── services/
│   └── api/
└── tests/

frontend/
├── src/
│   ├── components/
│   ├── pages/
│   └── services/
└── tests/

# Option 3: Mobile + API (when "iOS/Android" detected)
api/
└── [same as backend above]

ios/ or android/
└── [platform-specific structure]
```

**Structure Decision**: DEFAULT to Option 1 — feature touches backend libraries and adapters only; no frontend deliverables.

## Phase 0: Outline & Research
1. Extract unknowns from Technical Context above:
   - Confirm exact Oqtane tenant context APIs to resolve TenantId and SiteId at request time [resolve]
   - Determine safest mechanism to select per-tenant connection string (Oqtane connection manager vs. DI factory) [resolve]
   - Identify all 2sxc/EAV tables needing TenantId for isolation (including TsDynDataZone) [resolve]
   - Define caching scope keys to include {TenantId, SiteId} without breaking existing caches [resolve]
   - Backwards compatibility strategy when TenantId is absent in legacy data [resolve]

2. Generate and dispatch research agents:
   - Task: Research Oqtane APIs for tenant/site resolution and per-tenant DB access
   - Task: Research EAV data model impact and migration approach for TenantId
   - Task: Best practices for multi-tenant caching and logging scoping in .NET

3. Consolidate findings in `research.md` using Decision/Rationale/Alternatives format.

**Output**: research.md with all NEEDS CLARIFICATION resolved

## Phase 1: Design & Contracts
Prerequisites: research.md complete

1. Extract entities from feature spec → `data-model.md`:
   - TenantSiteKey: {TenantId:int, SiteId:int} value object
   - DataZone: include TenantId; relationships to Site/App
   - CacheScope: contract for building keys including TenantSiteKey

2. Generate API contracts from functional requirements:
   - ITenantSiteContext: resolve current TenantId/SiteId
   - IConnectionSelector: choose correct DB connection for EAV operations per tenant
   - ICacheKeyBuilder: build tenant-scoped cache keys
   - Output contract stubs and test scaffolding to `/contracts/`

3. Generate contract tests from contracts:
   - One test per contract to assert schema/behavior (failing initially)

4. Extract test scenarios from acceptance criteria:
   - Two tenants, same SiteId values, isolation verified; alias/rename unaffected
   - Quickstart test steps reflect scenario

5. Update agent file incrementally:
   - Run `.specify/scripts/powershell/update-agent-context.ps1 -AgentType copilot` after Phase 1 docs exist

**Output**: data-model.md, /contracts/*, failing tests, quickstart.md, agent-specific file

## Phase 2: Task Planning Approach
This section describes what the /tasks command will do - DO NOT execute during /plan

Task Generation Strategy:
- Load `.specify/templates/tasks-template.md` as base
- Generate tasks from Phase 1 design docs (contracts, data model, quickstart)
- Each contract → contract test task [P]
- Each entity → model creation task [P]
- Each user story → integration test task
- Implementation tasks to make tests pass

Ordering Strategy:
- TDD order: Tests before implementation
- Dependency order: Models before services before integration
- Mark [P] for parallel execution (independent files)

Estimated Output: 25-30 numbered, ordered tasks in tasks.md

**IMPORTANT**: This phase is executed by the /tasks command, NOT by /plan

## Phase 3+: Future Implementation
These phases are beyond the scope of the /plan command

Phase 3: Task execution (/tasks command creates tasks.md)  
Phase 4: Implementation (execute tasks.md following constitutional principles)  
Phase 5: Validation (run tests, execute quickstart.md, performance validation)

## Complexity Tracking
Fill ONLY if Constitution Check has violations that must be justified

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|

## Progress Tracking
This checklist is updated during execution flow

Phase Status:
- [ ] Phase 0: Research complete (/plan command)
- [ ] Phase 1: Design complete (/plan command)
- [ ] Phase 2: Task planning complete (/plan command - describe approach only)
- [ ] Phase 3: Tasks generated (/tasks command)
- [ ] Phase 4: Implementation complete
- [ ] Phase 5: Validation passed

Gate Status:
- [ ] Initial Constitution Check: PASS
- [ ] Post-Design Constitution Check: PASS
- [ ] All NEEDS CLARIFICATION resolved
- [ ] Complexity deviations documented

---
Based on Constitution v1.0.0 - See `/memory/constitution.md`
