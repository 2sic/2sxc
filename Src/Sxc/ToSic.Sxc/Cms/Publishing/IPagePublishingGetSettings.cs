using ToSic.Lib.DI;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Cms.Publishing
{
    public interface IPagePublishingGetSettings: IHasLog, ISwitchableService
    {
        BlockPublishingSettings SettingsOfModule(int moduleId);
    }
}
