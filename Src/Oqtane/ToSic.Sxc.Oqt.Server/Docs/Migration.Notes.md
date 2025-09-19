# Migration Notes: Multi-Database & Cache Scoping

These notes highlight changes that may affect existing Oqtane + 2sxc installations when upgrading to the version that introduces per-tenant/site database overrides and cache key scoping.

## What changed

- Connection selection can now be overridden per tenant and per site. If you do nothing, 2sxc continues to use the platform `DefaultConnection` (backwards-compatible).
- LightSpeed caching can now append a short scope segment like `t:{TenantId}-s:{SiteId}` to ensure cache isolation across tenants/sites. In single-tenant scenarios (TenantId=0, SiteId=0) no extra segment is added (backwards-compatible cache keys).

## Backwards compatibility

- Single-tenant setups (0/0) behave exactly as before: same connection string and same cache keys.
- If you already use named connections (e.g., `ReportingConnection`), the new precedence rules prefer per-tenant/site overrides when present, otherwise fall back to platform-level `ConnectionStrings:{Name}`.

## Opting into multi-database

1. Decide on the database topology per tenant and/or per site.
2. Provide overrides in `appsettings.json` or environment variables as documented in `Configuration.MultiDatabase.md`.
3. Verify the app starts and content is isolated as expected.

## Safe rollout checklist

- Start by adding tenant-level overrides only; test; then add site-level as needed.
- Keep the platform `DefaultConnection` intact to allow fallback during rollout.
- Validate cache behavior by navigating to tenant A and tenant B and verifying content separation.

## Troubleshooting

- Wrong database used: Double-check the effective `TenantId`/`SiteId` in logs and verify the configuration key paths.
- Cache bleed: Ensure updated components use the new cache key builder or LightSpeed scope. Clear caches after deployment.
