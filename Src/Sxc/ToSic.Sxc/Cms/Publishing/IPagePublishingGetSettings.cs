using ToSic.Eav.DI;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Cms.Publishing
{
    public interface IPagePublishingGetSettings: IHasLog, ISwitchableService
    {
        BlockPublishingSettings SettingsOfModule(int moduleId);
    }
}
