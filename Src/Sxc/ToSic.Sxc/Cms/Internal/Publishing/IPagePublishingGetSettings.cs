using ToSic.Lib.DI;

namespace ToSic.Sxc.Cms.Internal.Publishing;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IPagePublishingGetSettings: IHasLog, ISwitchableService
{
    BlockPublishingSettings SettingsOfModule(int moduleId);
}