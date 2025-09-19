using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Shared.Interfaces;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Server.Context;

/// <summary>
/// Default Oqtane implementation for resolving the current Tenant/Site composite key.
/// </summary>
public class OqtTenantSiteContext(AliasResolver aliasResolver) : ITenantSiteContext
{
    public TenantSiteKey Current
    {
        get
        {
            // AliasResolver ensures Alias is initialized when accessed
            var alias = aliasResolver.Alias;
            var tenantId = alias?.TenantId ?? 0;
            var siteId = alias?.SiteId ?? 0;
            return new TenantSiteKey(tenantId, siteId);
        }
    }
}
