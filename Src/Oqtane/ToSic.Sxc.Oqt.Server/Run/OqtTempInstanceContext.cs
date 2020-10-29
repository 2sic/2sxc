using Oqtane.Models;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;
using ToSic.Sxc.Oqt.Shared.Dev;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtTempInstanceContext
    {
        private readonly OqtContainer _oqtContainer;
        private readonly OqtSite _oqtSite;

        public OqtTempInstanceContext(OqtContainer oqtContainer, OqtSite oqtSite)
        {
            _oqtContainer = oqtContainer;
            _oqtSite = oqtSite;
        }

        public InstanceContext CreateContext(Module module, int pageId, ILog parentLog)
            => new InstanceContext(
                 _oqtSite, 
                //_zoneMapper.TenantOfZone(zoneId),
                new OqtPage(pageId, null),
                _oqtContainer.Init(module, parentLog),
                new OqtUser(WipConstants.NullUser)
            );

    }
}
