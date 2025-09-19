# Logging Guidance

Guidelines for logging with the multi-database and cache scoping features in 2sxc for Oqtane.

## What to log

- Enrich logs with `TenantId` and `SiteId` when diagnosing multi-tenant issues (context only, not secrets).
- Log which connection name was used (e.g., `DefaultConnection`, `ReportingConnection`) without printing the connection string.

## What NOT to log

- Never log full connection strings or credentials.
- Avoid logging raw cache keys from user input. Prefer structured logging with fields `Namespace`, `TenantId`, `SiteId`, and `PartCount`.

## Suggested patterns

- Use structured logs: `logger.LogInformation("Resolved connection", new { name, tenantId, siteId });`
- For cache-related logs, prefer stating the namespace and whether a tenant/site scope was applied.
