using ToSic.Eav.Apps.Enums;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Cms
{
    internal class OqtPagePublishingSettings: PagePublishingSettingsBase
    {
        #region Constructor / DI

        public OqtPagePublishingSettings() : base(OqtConstants.OqtLogPrefix) { }
        
        #endregion


        protected override PublishingMode LookupRequirements(int instanceId)
            => PublishingMode.DraftOptional;

    }
}
