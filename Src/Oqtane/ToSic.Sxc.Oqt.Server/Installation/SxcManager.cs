using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Repository;

namespace ToSic.Sxc.Oqt.Server.Installation
{
    /// <summary>
    /// This is probably some kind of installer-class.
    /// </summary>
    /// <remarks>
    /// WARNING: Careful when renaming / moving, the name is listed in the ModuleInfo.cs in the Client.
    /// </remarks>
    public class SxcManager : IInstallable
    {
        private ISqlRepository _sql;

        public SxcManager(ISqlRepository sql)
        {
            _sql = sql;
        }

        public bool Install(Tenant tenant, string version)
        {
            return _sql.ExecuteScript(tenant, GetType().Assembly, "ToSic.Sxc." + version + ".sql");
        }

        public bool Uninstall(Tenant tenant)
        {
            return _sql.ExecuteScript(tenant, GetType().Assembly, "ToSic.Sxc.Uninstall.sql");
        }
    }
}
