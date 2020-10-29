using Oqtane.Models;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;
using ToSic.Sxc.Oqt.Shared.Dev;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtTempInstanceContext
    {
        private readonly OqtaneContainer _oqtaneContainer;
        private readonly OqtSite _oqtSite;

        public OqtTempInstanceContext(OqtaneContainer oqtaneContainer, OqtSite oqtSite)
        {
            _oqtaneContainer = oqtaneContainer;
            _oqtSite = oqtSite;
        }

        public InstanceContext CreateContext(Module module, int pageId, ILog parentLog)
            => new InstanceContext(
                 _oqtSite, 
                //_zoneMapper.TenantOfZone(zoneId),
                new OqtanePage(pageId, null),
                _oqtaneContainer.Init(module, parentLog),
                new OqtaneUser(WipConstants.NullUser)
            );

    }
}
