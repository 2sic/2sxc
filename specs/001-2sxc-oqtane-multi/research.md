# Research: 2sxc Oqtane Multi-Database Support (Phase 0)

Date: 2025-09-19  
Related Plan: specs/001-2sxc-oqtane-multi/plan.md

## Decision 1: Resolve TenantId and SiteId at request time

Decision: Use Oqtane runtime context (HttpContext.Items + DI services) to obtain current Tenant and Site. Prefer official Oqtane services (e.g., SiteState/Tenant resolver) over custom parsing.  
Rationale: Aligns with Anti-Abstraction and Integration-First gates; leverages platform-provided context and lifecycle.  
Alternatives: Parse alias/host headers manually (rejected: brittle, higher maintenance); cache per-request in static storage (rejected: thread-safety and testability concerns).

## Decision 2: Select per-tenant database connection

Decision: Implement a small adapter that asks Oqtane’s connection/tenant services for the correct connection string for the current Tenant; use DI to inject an IConnectionSelector that provides DbConnection/DbContext options for EAV.  
Rationale: Minimal glue; preserves core/integration boundaries.  
Alternatives: Maintain our own connection string registry (rejected: duplication of Oqtane config); ambient transaction + connection interceptor (rejected: complexity vs. value).

## Decision 3: Persist TenantId in EAV rows and TsDynDataZone

Decision: Add TenantId where required in EAV records used by 2sxc in Oqtane; ensure DataZone keys include TenantId; treat identity as {TenantId, SiteId}. Coordinate with external EAV repo via normal dependency update.  
Rationale: Prevents cross-tenant bleed when SiteId overlaps across databases; meets FR-003/FR-008.  
Alternatives: Infer tenant solely from connection (rejected: some caches/identities persist outside DB context like exports, background jobs).

## Decision 4: Caching and logging scope includes TenantSiteKey

Decision: Introduce a TenantSiteKey value object {TenantId, SiteId}; update cache key composition to include this key where the scope is site-bound; adjust logging scopes to enrich with TenantId/SiteId.  
Rationale: Ensures cache isolation; improves observability.  
Alternatives: Only TenantId (rejected: Site-level separation is required for multi-site tenants); only SiteId (rejected: SiteId overlaps across tenants).

## Decision 5: Backward compatibility for single-tenant setups

Decision: When TenantId is missing in legacy data, default to a sentinel or derive from single-tenant context; migration scripts optional and additive; runtime tolerant of null TenantId by mapping to default tenant where applicable.  
Rationale: Preserve existing deployments without mandatory migrations.  
Alternatives: Hard migration required (rejected: increases upgrade friction).

## Open Questions Resolved

- Which Oqtane services expose Tenant/Site at runtime? Use the official context providers (to be finalized during Phase 1 contract drafting with concrete interface names from Oqtane source).
- Which EAV tables require TenantId? At minimum, TsDynDataZone and any row that keys or filters by Site/App; full list to be validated when drafting data-model.md.

## Risks and Mitigations

- Risk: Hidden code paths referencing SiteId alone.  
  Mitigation: Search for SiteId usages in Oqtane integration layer; add tests covering cross-tenant SiteId overlaps.
- Risk: Cache key changes causing over-invalidation.  
  Mitigation: Version cache keys; ensure graceful fallback on misses; measure performance.
- Risk: EAV dependency timing.  
  Mitigation: Feature-flag in code paths only for EAV changes behind version checks, not user-facing flags; coordinate package updates.

## Performance Notes

- Add < 1 ms overhead target: obtain Tenant/Site from request-scoped services (no I/O).  
- Cache key composition is string concat of small structs; negligible cost.

## Outcome

All NEEDS CLARIFICATION items identified in the plan are addressed sufficiently to proceed to Phase 1 design.
