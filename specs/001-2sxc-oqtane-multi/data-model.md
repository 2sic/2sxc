# Data Model: 2sxc Oqtane Multi-Database Support (Phase 1)

Date: 2025-09-19  
Related Plan: specs/001-2sxc-oqtane-multi/plan.md

## Entities

### TenantSiteKey (value object)

- TenantId: int (required, >= 1)
- SiteId: int (required, >= 1)

Constraints:

- Must be immutable and equatable (value semantics)
- Used for scoping caches, logs, and data access

### DataZone

- ZoneId: int (existing)
- AppId: int (existing)
- SiteId: int (existing)
- TenantId: int (new, required in Oqtane multi-tenant)

Relationships:

- DataZone belongs to Site and App; in multi-tenant, SiteId is scoped by TenantId

Validation:

- In multi-tenant mode, TenantId must be present and match current TenantSiteKey
- In single-tenant mode, TenantId may be null/0 and maps to default tenant

### CacheScope (concept)

- KeyParts: string[] including serialized TenantSiteKey where site scope applies
- Namespace: string to segment cache spaces (e.g., "2sxc:views", "2sxc:data")

Rules:

- Include TenantSiteKey for any site-scoped cache
- Global caches remain unchanged

## Notes on EAV Alignment

- EAV rows used by 2sxc in Oqtane must carry TenantId where identity or filtering occurs by Site or Zone
- TsDynDataZone must include TenantId to avoid cross-tenant collisions

### Repository boundaries and ownership

- The EAV data layer (schemas, migrations, and data access primitives) lives in the sibling repository at `A:/2sxc/eav-server` and is referenced by this solution. Do not add or modify EAV tables from within this repo.
- Any schema changes (for example adding a `TenantId` column to EAV-owned tables) must be proposed and implemented in the eav-server repo. Coordinate changes by:
	- Opening an issue/PR in eav-server which references this spec, and
	- Linking back from this spec/plan to the corresponding eav-server change set and migration notes.
- Until the eav-server changes are merged and consumed here, treat multi-tenant EAV fields as a contract in this repo (interfaces, context, and selection logic) without enforcing schema changes locally.

## Open Items to Validate

- Full list of EAV tables impacted (beyond TsDynDataZone)
- Migration paths for adding TenantId without breaking existing single-tenant data
