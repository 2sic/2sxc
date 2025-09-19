# ITenantSiteContext (contract)

Purpose: Resolve current TenantId and SiteId from the Oqtane runtime for the current request.

Proposed shape (conceptual):
- GetCurrent(): TenantSiteKey
  - Returns a value object with { TenantId: int, SiteId: int }

Notes:
- Backed by Oqtane services (no manual header parsing).
- Request-scoped; deterministic and side-effect-free accessors.
