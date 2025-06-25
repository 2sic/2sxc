using ToSic.Lib.DI;

namespace ToSic.Sxc.Cms.Publishing.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IPagePublishingGetSettings: IHasLog, ISwitchableService
{
    BlockPublishingSettings SettingsOfModule(int moduleId);
}