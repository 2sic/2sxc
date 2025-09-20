using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Context;

internal interface IOqtTenantIdentityProvider
{
    bool TryGetIdentity(out OqtTenantSiteIdentity identity);

    OqtTenantSiteIdentity GetIdentity();
}

internal interface IOqtTenantConnectionProvider
{
    bool TryGetConnection(out OqtTenantContextInfo context);

    OqtTenantContextInfo GetConnection();
}

internal interface IOqtTenantContext : IOqtTenantIdentityProvider, IOqtTenantConnectionProvider
{
}
