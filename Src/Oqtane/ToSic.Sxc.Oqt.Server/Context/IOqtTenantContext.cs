namespace ToSic.Sxc.Oqt.Server.Context;

internal interface IOqtTenantContext
{
    OqtTenantContextInfo? Get();

    OqtTenantContextInfo GetRequired();
}
