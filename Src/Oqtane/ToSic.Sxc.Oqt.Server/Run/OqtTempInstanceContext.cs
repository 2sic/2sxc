using Oqtane.Models;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;
using ToSic.Sxc.Oqt.Shared.Dev;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtTempInstanceContext
    {
        private readonly OqtaneContainer _oqtaneContainer;
        private readonly OqtaneTenantSite _oqtaneTenantSite;

        public OqtTempInstanceContext(OqtaneContainer oqtaneContainer, OqtaneTenantSite oqtaneTenantSite)
        {
            _oqtaneContainer = oqtaneContainer;
            _oqtaneTenantSite = oqtaneTenantSite;
        }

        public InstanceContext CreateContext(Module module, int pageId, ILog parentLog)
            => new InstanceContext(
                 _oqtaneTenantSite, 
                //_zoneMapper.TenantOfZone(zoneId),
                new OqtanePage(pageId, null),
                _oqtaneContainer.Init(module, parentLog),
                new OqtaneUser(WipConstants.NullUser)
            );

    }
}
