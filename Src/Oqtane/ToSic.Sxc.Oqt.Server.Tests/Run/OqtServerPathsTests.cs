using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Tests.Run;

public class OqtServerPathsTests
{
    [Fact]
    public void AppRootForMasterTenantUsesLegacyStructure()
    {
        var identity = new OqtTenantSiteIdentity(OqtConstants.MasterTenantId, 7);
        Equal($"2sxc\{identity.SiteId}", OqtServerPaths.GetAppRoot(identity));
    }

    [Fact]
    public void AppRootForTenantIncludesTenantAndSite()
    {
        var identity = new OqtTenantSiteIdentity(5, 3);
        Equal($"2sxc\Tenants\{identity.TenantId}\Sites\{identity.SiteId}", OqtServerPaths.GetAppRoot(identity));
    }

    [Fact]
    public void AppApiPathUsesCompositeIdentity()
    {
        var identity = new OqtTenantSiteIdentity(5, 3);
        var path = OqtServerPaths.GetAppApiPath(identity, "MyApp", "api/controller");
        Equal($"2sxc\Tenants\5\Sites\3\MyApp\api\controller", path);
    }
}
