using ToSic.Eav.Apps.Enums;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Run
{
    internal class OqtPagePublishingResolver: PagePublishingResolverBase
    {
        #region Constructor / DI

        public OqtPagePublishingResolver() : base(OqtConstants.OqtLogPrefix) { }
        
        #endregion


        protected override PublishingMode LookupRequirements(int instanceId)
            => PublishingMode.DraftOptional;

    }
}
