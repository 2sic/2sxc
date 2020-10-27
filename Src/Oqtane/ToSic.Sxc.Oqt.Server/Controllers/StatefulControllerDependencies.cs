using Oqtane.Repository;
using ToSic.Eav.Run;
using ToSic.Sxc.Oqt.Server.Repository;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    /// <summary>
    /// Helper to ensure DI on all controllers, without constantly having to update the parameters on the constructor
    /// </summary>
    public class StatefulControllerDependencies
    {
        internal readonly IZoneMapper ZoneMapper;
        internal readonly ITenantResolver TenantResolver;
        internal readonly IUserResolver UserResolver;

        public StatefulControllerDependencies(IZoneMapper zoneMapper, ITenantResolver tenantResolver,
            IUserResolver userResolver)
        {
            ZoneMapper = zoneMapper;
            TenantResolver = tenantResolver;
            UserResolver = userResolver;
        }
    }
}
