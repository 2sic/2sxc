using System;
using Oqtane.Models;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Oqt.Shared.Dev;
using ToSic.Sxc.Run;
using ToSic.Sxc.Run.Context;

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

        public IContextOfBlock CreateContext(Module module, int pageId, ILog parentLog,
            IServiceProvider serviceProvider)
        {
            var publishing = serviceProvider.Build<IPagePublishingResolver>();

            return new ContextOfBlock(
                serviceProvider,
                _oqtSite,
                new OqtUser(WipConstants.NullUser)
            ).Init(serviceProvider.Build<SxcPage>().Init(pageId),
                _oqtContainer.Init(module, parentLog),
                publishing.GetPublishingState(module.ModuleId)
            );
        }
    }
}
