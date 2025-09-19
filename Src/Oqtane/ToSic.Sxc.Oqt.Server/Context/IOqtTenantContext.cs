namespace ToSic.Sxc.Oqt.Server.Context;

internal interface IOqtTenantIdentityProvider
{
    bool TryGetIdentity(out OqtTenantIdentity identity);

    OqtTenantIdentity GetIdentity();
}

internal interface IOqtTenantConnectionProvider
{
    bool TryGetConnection(out OqtTenantContextInfo context);

    OqtTenantContextInfo GetConnection();
}

internal interface IOqtTenantContext : IOqtTenantIdentityProvider, IOqtTenantConnectionProvider
{
}
