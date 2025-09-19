# 2sxc on Oqtane: Per-tenant/site database overrides

This document explains how the Oqtane adapter selects the SQL connection string for 2sxc and how to override it per tenant and site.

## Resolution order (highest first)
 
1. Site-level override: `ConnectionStrings:Tenants:{TenantId}:Sites:{SiteId}:DefaultConnection`
2. Tenant-level override: `ConnectionStrings:Tenants:{TenantId}:DefaultConnection`
3. Platform default: Oqtane's standard `DefaultConnection`

If no override is found, 2sxc uses the platform default connection string, preserving current behavior.

## appsettings.json examples

Minimal per-tenant override:
 
```json
{
  "ConnectionStrings": {
    "Tenants": {
      "3": {
        "DefaultConnection": "Server=.;Database=oqtane_t3;Trusted_Connection=True;TrustServerCertificate=True;"
      }
    }
  }
}
```

Site-specific override for tenant 3, site 7 (takes precedence over tenant-level):
 
```json
{
  "ConnectionStrings": {
    "Tenants": {
      "3": {
        "DefaultConnection": "Server=.;Database=oqtane_t3;Trusted_Connection=True;TrustServerCertificate=True;",
        "Sites": {
          "7": {
            "DefaultConnection": "Server=.;Database=oqtane_t3_s7;Trusted_Connection=True;TrustServerCertificate=True;"
          }
        }
      }
    }
  }
}
```

## Named connections

You can also use named connections beyond `DefaultConnection`. The same precedence rules apply:

1) `ConnectionStrings:Tenants:{TenantId}:Sites:{SiteId}:{Name}`
2) `ConnectionStrings:Tenants:{TenantId}:{Name}`
3) `ConnectionStrings:{Name}`

Example with a named connection `ReportingConnection`:

```json
{
  "ConnectionStrings": {
    "ReportingConnection": "Server=.;Database=oqtane_reporting;Trusted_Connection=True;TrustServerCertificate=True;",
    "Tenants": {
      "3": {
        "ReportingConnection": "Server=.;Database=oqtane_reporting_t3;Trusted_Connection=True;TrustServerCertificate=True;",
        "Sites": {
          "7": {
            "ReportingConnection": "Server=.;Database=oqtane_reporting_t3_s7;Trusted_Connection=True;TrustServerCertificate=True;"
          }
        }
      }
    }
  }
}
```

You can also place these settings in environment-specific files (for example, `appsettings.Development.json`) or provide them via environment variables.

## Environment variables
 
Settings can be supplied via environment variables using `:` separators. For example:
 
- `ConnectionStrings:Tenants:3:DefaultConnection=...`
- `ConnectionStrings:Tenants:3:Sites:7:DefaultConnection=...`

## Notes
 
- Only SQL connection selection is affected; all other behavior remains unchanged.
- Missing or empty values are ignored; the selector falls back to the next level.
- This feature is transparent to 2sxc apps. It isolates data per tenant/site when separate databases are configured.

## Security recommendations
 
- Do not commit secrets. Prefer environment variables, Azure Key Vault, or other secret managers supported by your hosting environment.
- For local development, consider using `dotnet user-secrets` on the Oqtane Server project.

## Migration considerations

- Backwards compatible by default: If you configure nothing, 2sxc keeps using the platform `DefaultConnection`.
- Single-tenant unchanged: When TenantId/SiteId are `0/0`, behavior and cache keys remain as before.
- Named connections: Per-tenant/site overrides take precedence over platform-level named connections when provided.
- Cache scope: LightSpeed cache keys may include a short `t:{TenantId}-s:{SiteId}` suffix to avoid cross-tenant bleed; single-tenant has no suffix.
- Safe rollout: Start with tenant-level overrides, validate isolation, then add site-level where needed.
