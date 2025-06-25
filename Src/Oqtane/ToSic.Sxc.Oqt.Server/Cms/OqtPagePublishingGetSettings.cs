using ToSic.Sxc.Cms.Publishing.Sys;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Cms;

internal class OqtPagePublishingGetGetSettings() : PagePublishingGetSettingsBase(OqtConstants.OqtLogPrefix)
{
    #region Constructor / DI

    #endregion


    protected override PublishingMode LookupRequirements(int instanceId)
        => PublishingMode.DraftOptional;

}