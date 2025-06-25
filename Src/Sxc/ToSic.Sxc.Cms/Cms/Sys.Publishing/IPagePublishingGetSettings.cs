using ToSic.Lib.DI;

namespace ToSic.Sxc.Cms.Internal.Publishing;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IPagePublishingGetSettings: IHasLog, ISwitchableService
{
    BlockPublishingSettings SettingsOfModule(int moduleId);
}