using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Repository;

namespace ToSic.Sxc.Oqt.Server.Installation;

/// <summary>
/// This is probably some kind of installer-class.
/// </summary>
/// <remarks>
/// WARNING: Careful when renaming / moving, the name is listed in the ModuleInfo.cs in the Client.
/// </remarks>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class SxcManager(ISqlRepository sql) : IInstallable
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