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
[ShowApiWhenReleased(ShowApiMode.Never)]
public class SxcManager(ISqlRepository sql, MasterDBContext db) : IInstallable
{
    public bool Install(Tenant tenant, string version)
    {
        if (IsCleanInstall(tenant) && !HasDb(tenant))
            return CleanInstall(tenant);

        if (HasDb20(tenant))
            return !NeedUpgradeScript(tenant, version) || sql.ExecuteScript(tenant, GetType().Assembly, "ToSic.Sxc." + version + ".sql");

        return !HasDbOld(tenant) || sql.ExecuteScript(tenant, GetType().Assembly, "ToSic.Sxc." + version + ".sql");
    }

    public bool Uninstall(Tenant tenant) 
        => sql.ExecuteScript(tenant, GetType().Assembly, "ToSic.Sxc.Uninstall.sql");

    private bool CleanInstall(Tenant tenant) 
        => sql.ExecuteScript(tenant, GetType().Assembly, "ToSic.Sxc.Install.sql");

    //private static bool? _isCleanInstalled;
    private static readonly string CleanInstalledVersion = ("20-00-00").Replace("_", ".");

    private bool IsCleanInstall(Tenant tenant)
        => db.ModuleDefinition.Any(md => md.ModuleDefinitionName == "ToSic.Sxc.Oqt.App, ToSic.Sxc.Oqtane.Client" && string.IsNullOrEmpty(md.Version));

    private bool HasDb(Tenant tenant)
    {
        var tableExists = "SELECT CASE WHEN EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME IN ('TsDynDataApp','ToSIC_EAV_Apps')) THEN 1 ELSE 0 END";

        return 1 == sql.ExecuteNonQuery(tenant, tableExists);
    }

    private bool HasDbOld(Tenant tenant)
    {
        var tableExists = "SELECT CASE WHEN EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ToSIC_EAV_Apps') THEN 1 ELSE 0 END";

        return 1 == sql.ExecuteNonQuery(tenant, tableExists);
    }

    private bool HasDb20(Tenant tenant)
    {
        var tableExists = "SELECT CASE WHEN EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TsDynDataApp') THEN 1 ELSE 0 END";

        return 1 == sql.ExecuteNonQuery(tenant, tableExists);
    }
    private bool NeedUpgradeScript(Tenant tenant, string version)
    {
        Version.TryParse(version, out var ver1);
        Version.TryParse(CleanInstalledVersion, out var ver2);
        return ver1 > ver2;
    }
}