using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Context;

internal readonly record struct OqtTenantContextInfo(int TenantId, int SiteId, string ConnectionStringName, string ConnectionString)
{
    public OqtTenantSiteIdentity Identity => new(TenantId, SiteId);
}
