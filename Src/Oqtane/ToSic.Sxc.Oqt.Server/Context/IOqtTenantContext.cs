namespace ToSic.Sxc.Oqt.Server.Context;

internal interface IOqtTenantContext
{
    bool TryGet(out OqtTenantContextInfo context);

    OqtTenantContextInfo GetRequired();
}