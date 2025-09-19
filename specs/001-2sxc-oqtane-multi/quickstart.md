# Quickstart: 2sxc Oqtane multi-database (multi-tenant)

This guide shows how to configure per-tenant and per-site database connections for 2sxc on Oqtane, including named connections, then validate isolation.

> For deeper implementation details and more examples, see: `Src/Oqtane/ToSic.Sxc.Oqt.Server/Docs/Configuration.MultiDatabase.md`

## Prerequisites

- Oqtane Server with 2sxc integration (this repository solution).
- Access to edit appsettings files or set environment variables in your hosting environment.
- SQL Server connection strings for your target databases.

## How connection selection works

Precedence is applied in this order (highest first):

1. Site-level override: `ConnectionStrings:Tenants:{TenantId}:Sites:{SiteId}:DefaultConnection`
2. Tenant-level override: `ConnectionStrings:Tenants:{TenantId}:DefaultConnection`
3. Platform default: Oqtane's `ConnectionStrings:DefaultConnection`

The same precedence applies to named connections by replacing `DefaultConnection` with your connection name.

## Configure per-tenant/site: DefaultConnection

Minimal per-tenant override (appsettings.json):

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

Site-specific override (takes precedence over tenant-level):

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

## Configure named connections

Precedence:

1. `ConnectionStrings:Tenants:{TenantId}:Sites:{SiteId}:{Name}`
2. `ConnectionStrings:Tenants:{TenantId}:{Name}`
3. `ConnectionStrings:{Name}`

Example for a named connection `ReportingConnection`:

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

## Environment variables

You can supply the same settings via environment variables (for example in hosting or CI/CD). Use `:` as the separator:

- `ConnectionStrings:Tenants:3:DefaultConnection=...`
- `ConnectionStrings:Tenants:3:Sites:7:DefaultConnection=...`
- `ConnectionStrings:ReportingConnection=...`
- `ConnectionStrings:Tenants:3:ReportingConnection=...`
- `ConnectionStrings:Tenants:3:Sites:7:ReportingConnection=...`

## Single-tenant compatibility

If no overrides are configured or if `TenantId`/`SiteId` are zero, 2sxc falls back to Oqtane's platform default `DefaultConnection`.

## Validate isolation

1. Prepare Oqtane with two tenants (each with its own DB). Note the TenantIds.
2. For each tenant, create one site with the same SiteId (this happens naturally as SiteId starts at 1 per tenant).
3. Install/enable 2sxc and create a simple content item in Tenant A, SiteId=1.
4. Switch to Tenant B, SiteId=1 and verify the content is not visible.
5. Inspect logs to see TenantId/SiteId enriched entries and confirm cache keys include the TenantSiteKey.
6. Rename a site or alias and confirm isolation persists.

## Troubleshooting

- Empty or whitespace connection values are ignored; the next fallback is used.
- If content bleeds across tenants, verify the expected connection overrides are set and cache keys include TenantSiteKey.
- Ensure you do not commit secrets to source control. Prefer environment variables or secret managers.
