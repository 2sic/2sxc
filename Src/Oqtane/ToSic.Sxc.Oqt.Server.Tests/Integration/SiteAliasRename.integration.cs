namespace ToSic.Sxc.Oqt.Server.Tests.Integration;

public class SiteAliasRename
{
    [Fact(Skip = "Integration environment required: site alias rename path; manual/CI E2E only")]
    public void Site_Alias_Rename_Does_Not_Affect_Isolation()
    {
        // Arrange: Requires a tenant with a site; capture content visibility baseline.
        // Act: Rename site or change alias. Clear caches.
        // Assert: Content isolation by TenantId/SiteId persists; no cross-tenant bleed.
    }
}
