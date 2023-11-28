using ToSic.Eav.Apps.Enums;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Cms;

internal class OqtPagePublishingGetGetSettings: PagePublishingGetSettingsBase
{
    #region Constructor / DI

    public OqtPagePublishingGetGetSettings() : base(OqtConstants.OqtLogPrefix) { }
        
    #endregion


    protected override PublishingMode LookupRequirements(int instanceId)
        => PublishingMode.DraftOptional;

}