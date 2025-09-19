namespace ToSic.Sxc.Oqt.Server.Tests.Integration;

public class TenantsIsolation
{
    [Fact(Skip = "Integration environment required: multiple tenants with separate DBs; manual/CI E2E only")] 
    public void Content_Is_Isolated_Across_Tenants()
    {
        // Arrange: Requires Oqtane server with two tenants and configured per-tenant databases.
        // Act: Create content in Tenant A (SiteId=1), then switch to Tenant B (SiteId=1).
        // Assert: Content from Tenant A is not visible in Tenant B.
    }
}
