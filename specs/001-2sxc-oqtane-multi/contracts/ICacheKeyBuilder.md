# ICacheKeyBuilder (contract)

Purpose: Build cache keys that include tenant/site scope where appropriate.

Proposed shape (conceptual):

- Build(namespace: string, parts: string[], key?: TenantSiteKey): string

Notes:

- Include TenantSiteKey for site-scoped caches.
- Keep format deterministic and versionable.
