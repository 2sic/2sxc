# Performance Guidance

Recommendations when using multi-database overrides and cache scoping in 2sxc on Oqtane.

## Database connections

- Prefer tenant-level overrides when possible. Use site-level only if you truly need per-site isolation.
- Keep connection strings pooled (default ADO.NET connection pooling applies). Avoid frequent changes at runtime.

## Configuration access

- Overrides are read via the Oqtane `IConfigManager`. Avoid per-request recomputation by keeping selectors small and stateless.

## Caching

- LightSpeed cache keys now support a tenant/site scope segment to avoid cross-tenant pollution.
- Scope only where the cached content is tenant/site-specific.
- Cache key composition is designed to be deterministic and short to minimize memory overhead.

## Measurements

- Use Application Insights or your preferred APM to track DB call counts and latency by tenant.
- Confirm cache hit-ratios before/after rollout and ensure they remain within expected bounds.
