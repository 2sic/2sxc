using System;
using Oqtane.Models;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Oqt.Shared.Dev;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;

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

        public ContextOfBlock CreateContext(Module module, int pageId, ILog parentLog,
            IServiceProvider serviceProvider)
        {
            var publishing = serviceProvider.Build<IPagePublishingResolver>();

            return new ContextOfBlock(
                _oqtSite,
                new SxcPage(pageId, null, serviceProvider.Build<IHttp>().QueryStringKeyValuePairs()),
                _oqtContainer.Init(module, parentLog),
                new OqtUser(WipConstants.NullUser),
                serviceProvider, publishing.GetPublishingState(module.ModuleId)
            );
        }
    }
}
