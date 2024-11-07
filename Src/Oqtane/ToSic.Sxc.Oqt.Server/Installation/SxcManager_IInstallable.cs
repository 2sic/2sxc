using Oqtane.Infrastructure;
using Oqtane.Models;

namespace ToSic.Sxc.Oqt.Server.Installation;

internal partial class SxcManager : IInstallable
{
    public bool Install(Tenant tenant, string version)
    {
        return sql.ExecuteScript(tenant, GetType().Assembly, "ToSic.Sxc." + version + ".sql");
    }

    public bool Uninstall(Tenant tenant)
    {
        return sql.ExecuteScript(tenant, GetType().Assembly, "ToSic.Sxc.Uninstall.sql");
    }
}