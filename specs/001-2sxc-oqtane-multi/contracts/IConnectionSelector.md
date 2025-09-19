# IConnectionSelector (contract)

Purpose: Select the correct database connection for EAV operations based on the current tenant.

Proposed shape (conceptual):

- GetConnection(TenantSiteKey key): DbConnection or connection string

Notes:

- Delegates to Oqtane configuration/services where possible.
- No global/statics; use DI.
