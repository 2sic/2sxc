using ToSic.Lib.DI;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Cms.Publishing;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IPagePublishingGetSettings: IHasLog, ISwitchableService
{
    BlockPublishingSettings SettingsOfModule(int moduleId);
}